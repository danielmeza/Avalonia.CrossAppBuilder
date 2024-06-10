
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Serilog;


[GitHubActions(
    "build",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    EnableGitHubToken = true,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(Compile) },
    ImportSecrets = new[] { nameof(NuGetKey) },
    CacheKeyFiles = new[] { "**/global.json", "**/*.csproj" },
    CacheIncludePatterns = new[] { ".nuke/temp", "~/.nuget/packages" }
    )]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] [Secret] readonly string NuGetKey;

    [GitRepository] readonly GitRepository Repository;

    [GitVersion] readonly GitVersion GitVersion;

    GitHubActions GitHubActions => GitHubActions.Instance;

    [Solution(GenerateProjects = true)] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / ".output";
    AbsolutePath RestoreOutputDirectory => OutputDirectory / ".restore";
    AbsolutePath BuildOutputDirectory => OutputDirectory / ".build";
    AbsolutePath PackagesOutputDirectory => OutputDirectory / ".packages";

    Target Print => _ => _
        .Executes(() =>
        {
            Log.Information("Solution path = {Value}", Solution);
            Log.Information("Solution directory = {Value}", Solution.Directory);

            Log.Information("Commit = {Value}", Repository.Commit);
            Log.Information("Branch = {Value}", Repository.Branch);
            Log.Information("Tags = {Value}", Repository.Tags);

            Log.Information("main branch = {Value}", Repository.IsOnMainBranch());
            Log.Information("main/master branch = {Value}", Repository.IsOnMainOrMasterBranch());
            Log.Information("release/* branch = {Value}", Repository.IsOnReleaseBranch());
            Log.Information("hotfix/* branch = {Value}", Repository.IsOnHotfixBranch());

            Log.Information("Https URL = {Value}", Repository.HttpsUrl);
            Log.Information("SSH URL = {Value}", Repository.SshUrl);
        });

    Target Clean => _ => _
        .DependsOn(Print)
        .Before(Restore)
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();
            DotNetTasks.DotNetClean(_ => _
                .SetConfiguration(Configuration)
                .SetProcessWorkingDirectory(SourceDirectory));
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetWorkloadRestore(_ => _
                .SetProcessWorkingDirectory(SourceDirectory));
            DotNetTasks.DotNetRestore(_ => _
                .SetProcessWorkingDirectory(SourceDirectory)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Triggers(Pack)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(_ => _
                .SetConfiguration(Configuration)
                .SetNoRestore(true)
            );
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(PackagesOutputDirectory / "*.nupkg")
        .Executes(() =>
        {
            
            var publishCombinations =
                from project in Solution.GetAllProjects("*")
                    .Where( p=> p.GetProperty("IsPackable") == "true")
                from framework in project.GetTargetFrameworks()
                select new { project, framework };
            
            DotNetTasks.DotNetPack(_ => _
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(PackagesOutputDirectory)
                .CombineWith(publishCombinations, (_, v) => _
                    .SetProject(v.project)
                )
            );
        });
}