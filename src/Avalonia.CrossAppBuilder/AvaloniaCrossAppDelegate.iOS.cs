using Avalonia.iOS;

namespace Avalonia.CrossAppBuilder;

public class AvaloniaCrossAppDelegate<TApp> : AvaloniaAppDelegate<TApp>
    where TApp : Application, ICrossAppBuilder, new()
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .UseCrossAppBuilder<TApp>();
    }
}

