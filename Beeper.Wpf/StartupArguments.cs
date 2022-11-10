﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beeper.Wpf
{
    public class StartupArguments
    {
        public StartupArguments()
        {
            Song = new(value => (File.Exists(value) && value.EndsWith(".mid"), "Either file doesn't exist or isn't a `.mid` file"))
            {
                Description = "Path to the song `.mid` file to play",
                FullName = $"{StartupArgument.ArguementDeclerator}song",
                Shortcut= $"{StartupArgument.ArguementDeclerator}s",
                Required = $"Only of {StartupArgument.ArguementDeclerator}sd isn't used",
            };
            SongsDirectory = new(value => (Directory.Exists(value) && Directory.EnumerateFiles(value).Any(), "Either directory is empty, or it doesn't exist."))
            {
                Description = "Path to the song `.mid` file to play",
                FullName = $"{StartupArgument.ArguementDeclerator}song",
                Shortcut= $"{StartupArgument.ArguementDeclerator}s",
                Required = $"Only of {StartupArgument.ArguementDeclerator}sd isn't used",                
            };
            MaxNotes = new(value => (true, string.Empty))
            {
                Description = "Path to the directory (folder) where the all song `.mid` files are located. A random song will be chosen every time.",
                FullName = $"{StartupArgument.ArguementDeclerator}songdirectory",
                Shortcut= $"{StartupArgument.ArguementDeclerator}sd",
                Required = $"Only if {StartupArgument.ArguementDeclerator}s isn't used",
                DefaultValue = "60",
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
                string argument = arguments[i];
                var argumentToSet = argument.ToLower() switch
                {
                    // "https://stackoverflow.com/a/65642709" wtf
                    var value when value == MaxNotes.Shortcut => MaxNotes,
                    var value when value == MaxNotes.FullName => MaxNotes,
                    var value when value == Song.Shortcut => Song,
                    var value when value == Song.FullName => Song,
                    var value when value == SongsDirectory.Shortcut => SongsDirectory,
                    var value when value == SongsDirectory.FullName => SongsDirectory,
                    _ => throw new ArgumentException($"The Argument {argument} was not recognized as propper argument.", argument),
                };

                try
                {
                    argumentToSet.Value = arguments[++i];
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

        public IEnumerable<StartupArgument> Arguments => new[]
        {
            Song,
            MaxNotes,
            SongsDirectory,
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

        public string Value { get; set; }

        public string DefaultValue { get; set; }

        private Func<string, (bool isValid, string errorMessage)> _ValueValidator { get; }

        public const string ArguementDeclerator = "-";

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
