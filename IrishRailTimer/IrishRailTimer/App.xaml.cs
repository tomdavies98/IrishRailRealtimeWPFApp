using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IrishRailTimer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainModel = new MainModel();
            var mainVm = new MainViewModel(mainModel);
            var mainWin = new MainWindow(mainVm);
            mainWin.Show();

            base.OnStartup(e);
        }

    }
}
