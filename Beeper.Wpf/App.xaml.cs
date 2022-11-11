using Ownskit.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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
        private KeyboardListener KListener = new KeyboardListener();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var args = new StartupArguments(e.Args);

            string file = args.Song.Value;
            if (!args.Song.Validate())
            {
                var songs = Directory.GetFiles(args.SongsDirectory.Value, "*.mid");
                file = songs[random.Next(0, songs.Length)];
            }

            var player = new MidiFilePlayer(file);
            player.StartPlaying(int.Parse(args.MaxNotes.Value));

            KListener.KeyDown += (_, _) => player.SkipToNextNote();
        }


        private void Application_Exit(object sender, ExitEventArgs e)
        {
            KListener.Dispose();
        }
    }
}
