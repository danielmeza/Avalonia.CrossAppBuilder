# Avalonia.CrossAppBuilder
When creating Avalonia solutions is commonly to use several projects initializations configuration using AppBuilder, 
most of these configuration apply to all projects and ends duplicated in each target entry point, Avalonia.CrossAppBuilder 
solve that by providing a central way to configure avalonia cross settings.

## Usage

In your share project implement the ICrossAppBuilder interface in any class in your project, ej App.cs
```csharp
using Avalonia;
namespace MyAwesomeApp
{
   public class App : Application, ICrossAppBuilder
   {     
       public override void Initialize()
       {
           AvaloniaXamlLoader.Load(this);
       }
       
       public static void CrossAppBuild(AppBuilder builder)
       {
           builder.UseReactiveUI();
       }   
   }
}
```

In your target projects (Desktop, Browser):
```csharp
using Avalonia;
public class Program 
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    
    public static AppBuilder BuildAvaloniaApp()
            => CrossAppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
}
```

In your target projects (iOS):
```csharp
[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaCrossAppDelegate<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder);
    }
}
```

In your target projects (Android):
```csharp
[Activity(
    Label = "DispensersController.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaCrossMainActivity<App>
{        
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder);
    }
}
```

In your target projects (iOS) when you have a custom AvaloniaAppDelegate:
```csharp
[Register("AppDelegate")]
public partial class AppDelegate : CustomAvaloniaAppDelegate<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .UseCrossAppBuilder<App>();
    }
}
```

In your target projects (Android) when you have a custom AvaloniaMainActivity:
```csharp
[Activity(
    Label = "DispensersController.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : CustomAvaloniaMainActivity<App>
{        
      protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .UseCrossAppBuilder<App>();
    }
}
```