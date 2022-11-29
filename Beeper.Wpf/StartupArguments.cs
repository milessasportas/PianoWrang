using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beeper.Wpf
{
    public class StartupArguments
    {
        public StartupArguments()
        {
            Song = new(value => (File.Exists(value) && value.EndsWith(".mid"), $"Either file `{value}` doesn't exist or isn't a `.mid` file"))
            {
                Description = "Path to the song `.mid` file to play. This will be override anything set in `-sd`.",
                FullName = $"{StartupArgument.ArguementDeclerator}song",
                Shortcut= $"{StartupArgument.ArguementDeclerator}s",
                Required = $"Only of {StartupArgument.ArguementDeclerator}sd isn't used",
                NeedsValue = true,
            };
            SongsDirectory = new(value => (Directory.Exists(value) && Directory.EnumerateFiles(value).Any(), $"Either directory `{value}` is empty, or it doesn't exist."))
            {
                Description = "Path to the directory (folder) where the all song `.mid` files are located. A random song will be chosen every time.",
                FullName = $"{StartupArgument.ArguementDeclerator}songdirectory",
                Shortcut= $"{StartupArgument.ArguementDeclerator}sd",
                Required = $"Only of {StartupArgument.ArguementDeclerator}s isn't used",                
                DefaultValue = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MidiFiles"),
                NeedsValue = true,
            };
            MaxNotes = new(value => (int.TryParse(value, out _), $"The value `{value}` doesn't seem to be a valid integer number."))
            {
                Description = "Path to the directory (folder) where the all song `.mid` files are located. A random song will be chosen every time.",
                FullName = $"{StartupArgument.ArguementDeclerator}maxnotes",
                Shortcut= $"{StartupArgument.ArguementDeclerator}mn",
                Required = $"No",
                DefaultValue = "30",
                NeedsValue = true,
            };
            AutoPlay = new(_ => (true, string.Empty))
            {
                Description = $"When this argument is passed, then the music will automatically start playing and a keypress will skip to next note. " +
                        $"Default behaviour is that a keypress will cause a note to play (no autoplay).",
                FullName = $"{StartupArgument.ArguementDeclerator}autoplay",
                Shortcut = $"{StartupArgument.ArguementDeclerator}ap",
                Required = $"No",
                DefaultValue = "",
                NeedsValue = false,
            };
            TimeOutInSeconds = new(value => (int.TryParse(value, out _), $"The value `{value}` doesn't seem to be a valid integer number."))
            {
                Description = $"The maximum time in seconds wich to run this app.",
                FullName = $"{StartupArgument.ArguementDeclerator}timeout",
                Shortcut = $"{StartupArgument.ArguementDeclerator}to",
                Required = $"No",
                DefaultValue = "180",
                NeedsValue = true,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentException"></exception>
        public StartupArguments(string[] arguments) : this()
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                string argument = arguments[i].ToLowerInvariant();
                var argumentToSet = Arguments.FirstOrDefault(e => e.Shortcut.ToLowerInvariant() == argument || e.FullName.ToLowerInvariant() == argument)
                    ?? throw new ArgumentException($"The Argument {argument} was not recognized as propper argument.", argument);

                try
                {
                    argumentToSet.Value = argumentToSet.NeedsValue ? arguments[++i] : "WasPassed";
                }
                catch (Exception)
                {
                    throw new ArgumentException($"The Argument {argument} is missing the value to set it to.", argument);
                }

                if (!argumentToSet.Validate(out var errorMessage))
                {
                    throw new ArgumentException(errorMessage, argument);
                }
            }
        }

        public StartupArgument Song { get; set; }

        public StartupArgument MaxNotes { get; set; }

        public StartupArgument SongsDirectory { get; set; }

        public StartupArgument AutoPlay { get; set; }

        public StartupArgument TimeOutInSeconds { get; set; }

        public IEnumerable<StartupArgument> Arguments => new[]
        {
            Song,
            SongsDirectory,
            MaxNotes,
            AutoPlay,
            TimeOutInSeconds,
        };
    }

    public record class StartupArgument
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueValidator">
        /// In: the <see cref="Value"/> <br/>
        /// Out: <br/>
        ///     bool -> isValid <br/>
        ///     string -> errorMessage
        /// 
        /// </param>
        public StartupArgument(Func<string, (bool isValid, string errorMessage)> valueValidator)
        {
            _ValueValidator = valueValidator;
        }

        public string Shortcut { get; set; }

        public string Description { get; set; }

        public string FullName { get; set; }

        public string Required { get; set; }

        public string Value { get => _value ?? DefaultValue; set => _value = value; }

        public string DefaultValue { get; set; }

        private Func<string, (bool isValid, string errorMessage)> _ValueValidator { get; }

        /// <summary>
        /// Wether the Arguemnt is followed by it's value
        /// </summary>
        public bool NeedsValue { get;  set; }

        public const string ArguementDeclerator = "-";
        private string _value;

        public string ToArgumment()
        {
            return $"{ArguementDeclerator}{Shortcut} {Value}";
        }

        public bool Validate()
        {
            return Validate(out _);
        }

        public bool Validate(out string errorMessage)
        {
            var result = _ValueValidator(Value);
            errorMessage = result.errorMessage;
            return result.isValid;
        }



    }
}
