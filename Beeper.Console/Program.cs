// See https://aka.ms/new-console-template for more information
using Beeper.Wpf;
using System.Diagnostics;

namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        PrintUsage(new());

        using var process = new Process();

        // https://stackoverflow.com/a/45286568
        AppDomain.CurrentDomain.DomainUnload += (s, e) => TryKill(process);
        AppDomain.CurrentDomain.ProcessExit += (s, e) => TryKill(process);
        AppDomain.CurrentDomain.UnhandledException += (s, e) => TryKill(process);
        Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            TryKill(process);
        };

        try
        {
            var arg = new StartupArguments(args);
            Console.Clear();
            PrintUsage(arg);

            process.StartInfo.FileName = "Beeper.Wpf.exe";
            process.StartInfo.Arguments = string.Join(" ", args);
            process.Start();
            process.WaitForExit();
        }
        catch (ArgumentException ex)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = color;
            PressAnyKeyToExit();
        }
        catch (Exception ex)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine(ex.Message);
            Console.WriteLine("Unexpected error occured!");
            Console.ForegroundColor = color;
            PressAnyKeyToExit();
        }
        finally
        {
            TryKill(process);
        }
    }

    private static void TryKill(Process? process)
    {
        try
        {
            process?.Kill(); 
            process?.WaitForExit();
            process?.Dispose();
        }
        catch (Exception)
        {
            process?.Dispose();
        }
    }

    private static void PrintUsage(StartupArguments arg)
    {
        Console.WriteLine("Usage");
        foreach (var argument in arg.Arguments)
        {
            Console.WriteLine(argument.Shortcut);
            Console.WriteLine(argument.FullName);
            Console.WriteLine($"\t{argument.Description}");
            Console.WriteLine($"\tRequired: {argument.Required}");
            Console.WriteLine($"\tDefaultValue: {argument.DefaultValue}");
            Console.WriteLine($"\t\tCurrentValue: {argument.Value}");
            Console.WriteLine();
        }
    }

    private static void PressAnyKeyToExit()
    {
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
        Environment.Exit(0);
    }
}