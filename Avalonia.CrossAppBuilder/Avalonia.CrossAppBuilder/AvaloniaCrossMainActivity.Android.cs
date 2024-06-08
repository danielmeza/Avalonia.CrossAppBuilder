using Avalonia.Android;

namespace Avalonia.CrossAppBuilder;

public class AvaloniaCrossMainActivity<TApp> : AvaloniaMainActivity<TApp>
    where TApp : Application, ICrossAppBuilder, new()
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .UseCrossAppBuilder<TApp>();
    }
}