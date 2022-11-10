using Beeper.Console.MidiFiles;
using Ownskit.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Beeper.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        KeyboardListener KListener = new KeyboardListener();
        private Class1 class1;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
            
            class1 = new Class1();
            //MainWindow window = new MainWindow();
            //window.Show();
        }

        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Escape:
                case Key.LeftCtrl:
                case Key.LeftAlt:
                case Key.LeftShift:
                case Key.RightCtrl:
                case Key.RightAlt:
                case Key.RightShift:
                    return;
            }
            Debug.WriteLine(args.Key.ToString());
            Debug.WriteLine(args.ToString()); // Prints the text of pressed button, takes in account big and small letters. E.g. "Shift+a" => "A"#
            class1.cancellationTokenSource.Cancel();

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            KListener.Dispose();
        }
    }
}
