using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beeper.Console.MidiFiles
{
    public class Class1
    {
        public Class1()
        { 
            Try23();
            //Try1();
        }

        public CancellationTokenSource? cancellationTokenSource { get; private set; }

        public void Try23()
        {
            Task.Run(() =>
            {

                var midiFile = MidiFile.Read(@"D:\Repos\Beeper\Beeper.Console\MidiFiles\for_elise_by_beethoven.mid");
                TempoMap tempoMap = midiFile.GetTempoMap();

                using var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
                using var playback = midiFile.GetPlayback(outputDevice);

                var notes = midiFile.GetNotes().Take(330).ToArray();

                int i = 0;
                NoteOnEvent? onEvent = null;
                NoteOffEvent? offEvent = null;

                for (; i < notes.Length; i++)
                {
                    Note note = notes[i];
                    cancellationTokenSource = new CancellationTokenSource();

                    onEvent = new NoteOnEvent(note.NoteNumber, note.Velocity)
                    {
                        Channel = note.Channel,
                        DeltaTime = note.Time,
                    };

                    offEvent = new NoteOffEvent(note.NoteNumber, note.Velocity)
                    {
                        Channel = note.Channel,
                        DeltaTime = note.Time,
                    };
                    outputDevice.SendEvent(onEvent);
                    try
                    {

                        Task.Run(async () =>
                        {
                            try
                            {
                                var start = note.TimeAs<MetricTimeSpan>(tempoMap);
                                var end = note.EndTimeAs<MetricTimeSpan>(tempoMap);
                                await Task.Delay(end - start, cancellationTokenSource.Token);
                            }
                            catch (Exception)
                            {
                            }

                            outputDevice.SendEvent(offEvent);
                        }, cancellationTokenSource.Token).Wait();

                    }
                    catch (Exception)
                    {

                    }
                    //Thread.Sleep(note.TimeAs<MetricTimeSpan>(tempoMap));
                }
            });
        }

        public void Try2()
        {

            var midiFile = MidiFile.Read(@"D:\Repos\Beeper\Beeper.Console\MidiFiles\for_elise_by_beethoven.mid");
            TempoMap tempoMap = midiFile.GetTempoMap();

            using var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
            using var playback = midiFile.GetPlayback(outputDevice);

            var notes = midiFile.GetNotes().Take(330).ToArray();

            int i = 0;
            NoteOnEvent? onEvent = null;
            NoteOffEvent? offEvent = null;
            CancellationTokenSource? cancellationTokenSource = null;


            for (; i < notes.Length; i++)
            {
                Note note = notes[i];
                cancellationTokenSource = new CancellationTokenSource();

                onEvent = new NoteOnEvent(note.NoteNumber, note.Velocity)
                {
                    Channel = note.Channel,
                    DeltaTime = note.Time,                    
                };

                offEvent = new NoteOffEvent(note.NoteNumber, note.Velocity)
                {
                    Channel = note.Channel,
                    DeltaTime = note.Time,                    
                };
                outputDevice.SendEvent(onEvent);
                try
                {

                    Task.Run(async () =>
                    {
                        try
                        {
                            var start = note.TimeAs<MetricTimeSpan>(tempoMap);
                            var end = note.EndTimeAs<MetricTimeSpan>(tempoMap);
                            await Task.Delay(end- start, cancellationTokenSource.Token);
                        }
                        catch (Exception)
                        {
                        }

                        outputDevice.SendEvent(offEvent);
                    }, cancellationTokenSource.Token).Wait();

                }
                catch (Exception)
                {

                }
                //Thread.Sleep(note.TimeAs<MetricTimeSpan>(tempoMap));

            }


        }


        Playback? playback;
        private TempoMap _tempoMap2;

        public void Try1()
        {
            var midiFile = MidiFile.Read(@"D:\Repos\Beeper\Beeper.Console\MidiFiles\for_elise_by_beethoven.mid");
            _tempoMap2 = midiFile.GetTempoMap();

            using var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
            playback = midiFile.GetPlayback(outputDevice);
            //playback.Speed = 2.0;
            //var chunks = midiFile.GetTrackChunks().ToArray();
            playback.MoveToStart();
            //playback.MoveForward();
            playback.InterruptNotesOnStop = true;
            

            playback.EventPlayed += Playback_EventPlayed;
            playback.NotesPlaybackStarted += Playback_NotesPlaybackStarted;
            Task.Run(() => playback.Play());
            Thread.Sleep(2_000);
            playback.Stop();
            Thread.Sleep(2_000);
            playback.Start();
            Thread.Sleep(2_000);



            SpinWait.SpinUntil(() => !playback.IsRunning);
            playback.Dispose();
        }

        private void Playback_NotesPlaybackStarted(object? sender, NotesEventArgs e)
        {
            var color = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Green;
            var note = e.Notes.Single();
            if (note.NoteName == Melanchall.DryWetMidi.MusicTheory.NoteName.E)
            {
                System.Console.WriteLine("\tSkipping e");
                // mobing to next 
                //playback.Stop();
                //playback.MoveForward(note.TimeAs<MetricTimeSpan>(_tempoMap2));
                //playback.Start();

                System.Console.ForegroundColor = color;
                return;
            }
            System.Console.WriteLine($"Note {e.Notes.Single().NoteName}");

            var x = e.Notes.First();
            System.Console.ForegroundColor = color;
            
        }

        private void Playback_EventPlayed(object? sender, MidiEventPlayedEventArgs e)
        {
            var color = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine(e);
            System.Console.ForegroundColor = color;
        }
    }


    public class LowLevelKeyboardListener
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public event EventHandler<KeyPressedArgs> OnKeyPressed;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public LowLevelKeyboardListener()
        {
            _proc = HookCallback;
        }

        public void HookKeyboard()
        {
            _hookID = SetHook(_proc);
        }

        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                //if (OnKeyPressed != null) { OnKeyPressed(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode))); }
                OnKeyPressed?.Invoke(this, new());
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }

    public class KeyPressedArgs : EventArgs
    {
        //public Key KeyPressed { get; private set; }

        //public KeyPressedArgs(Key key)
        //{
        //    KeyPressed = key;
        //}
    }
}
