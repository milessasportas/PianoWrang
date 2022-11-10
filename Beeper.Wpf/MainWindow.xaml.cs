using Ownskit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Beeper.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
            InterceptKeys.KeyPressed += (_, _) => Debug.WriteLine("leeeel");
        }


        KeyboardListener KListener = new KeyboardListener();


        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            Debug.WriteLine(args.Key.ToString());
            Debug.WriteLine(args.ToString()); // Prints the text of pressed button, takes in account big and small letters. E.g. "Shift+a" => "A"
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            KListener.Dispose();
        }
    }
}
