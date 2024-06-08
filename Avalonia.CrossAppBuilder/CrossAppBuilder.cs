namespace Avalonia.CrossAppBuilder;

/// <summary>
/// Cross app builder class to extend  the <see cref="AppBuilder"/>
/// </summary>
public static class CrossAppBuilder
{
    /// <summary>
    /// Configures the app using the <typeparamref name="TApp"/> as a <see cref="ICrossAppBuilder"/>
    /// implementation.
    /// </summary>
    /// <remarks>
    /// To use this method, implements the <see cref="ICrossAppBuilder"/> interfaces in the <typeparamref name="TApp"/>
    /// class.
    /// </remarks>
    /// <example>
    /// In the target project entry point class this:
    /// <code>
    ///  public static AppBuilder BuildAvaloniaApp()
    ///     => CrossAppBuilder.Configure&lt;App&gt;()
    /// </code>
    /// </example>
    /// <typeparam name="TApp"></typeparam>
    /// <returns></returns>
    public static AppBuilder Configure<TApp>()
        where TApp : Application, ICrossAppBuilder, new()
    {
        return AppBuilder.Configure<TApp>()
            .UseCrossAppBuilder<TApp>();
    }
    
    /// <summary>
    /// Use the <see cref="ICrossAppBuilder"/> to initialize common cross configuration by invoking the
    /// <see cref="ICrossAppBuilder.CrossAppBuild(AppBuilder)"/> method
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static AppBuilder UseCrossAppBuilder<T>(this AppBuilder builder)
        where T : ICrossAppBuilder
    {
        T.CrossAppBuild(builder);
        return builder;
    }
}