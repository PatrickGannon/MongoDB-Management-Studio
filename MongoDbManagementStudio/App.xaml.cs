using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace MongoDBManagementStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CompositionContainer _container;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        [Import]
        public MainWindow AppWindow { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                Compose();
                AppWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (_container != null)
            {
                _container.Dispose();
            }
        }

        private void Compose()
        {
            var catalog = new AggregateCatalog();
            var execAssembly = Assembly.GetExecutingAssembly();
            catalog.Catalogs.Add(new AssemblyCatalog(execAssembly));

            var path = Path.GetDirectoryName(execAssembly.Location);
            catalog.Catalogs.Add(new DirectoryCatalog(path));
            
            var extDir = EnsureExtensionDirExists(path);
            catalog.Catalogs.Add(new DirectoryCatalog(extDir));

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
        }

        private static string EnsureExtensionDirExists(string rootPath)
        {
            var extDir = Path.Combine(rootPath, "Extensions");
            if (!Directory.Exists(extDir))
                Directory.CreateDirectory(extDir);
            return extDir;
        }
    }
}
