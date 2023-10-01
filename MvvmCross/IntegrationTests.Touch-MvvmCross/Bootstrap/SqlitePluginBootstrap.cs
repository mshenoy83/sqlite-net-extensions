using MvvmCross.Platform.Plugins;

namespace IntegrationTests.TouchMvvmCross.Bootstrap
{
    public class SqlitePluginBootstrap
        : MvxLoaderPluginBootstrapAction<MvvmCross.Plugins.Sqlite.PluginLoader, MvvmCross.Plugins.Sqlite.iOS.Plugin>
	{}
}
