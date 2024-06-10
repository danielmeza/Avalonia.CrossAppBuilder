namespace Avalonia.CrossAppBuilder;

/// <summary>
/// Declare the required cross app build, implement this in the shared project
/// to configure shared settings.
/// </summary>
public interface ICrossAppBuilder
{
    /// <summary>
    /// Configures shared common settings. 
    /// </summary>
    /// <remarks>
    /// This method is invoked right away, you can use the <paramref name="builder"/> to execute code in
    /// initialization stages. 
    /// </remarks>
    /// <param name="builder"></param>
    static abstract void CrossAppBuild(AppBuilder builder);
}