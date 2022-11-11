using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beeper.Wpf
{
    public class MidiFilePlayer
    {
        private MidiFile _MidiFile;
        private TempoMap _TempoMap;
        private CancellationTokenSource _NotePlayingCancelSource;

        /// <summary>
        /// Defaults to "Microsoft GS Wavetable Synth"
        /// </summary>
        public string OutputDeviceName { get; set; } = "Microsoft GS Wavetable Synth";

        public MidiFilePlayer(string midiFilePath = @"D:\Repos\Beeper\Beeper.Console\MidiFiles\for_elise_by_beethoven.mid")
        {
            _MidiFile = MidiFile.Read(midiFilePath);
            _TempoMap = _MidiFile.GetTempoMap();
        }

        public void SkipToNextNote()
        {
            _NotePlayingCancelSource?.Cancel();
            _NotePlayingCancelSource?.Dispose();
            _NotePlayingCancelSource = new CancellationTokenSource();
        }

        public Task StartPlaying(int notesToPlay = int.MaxValue)
        {
            return Task.Run(async () =>
            {
                using var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
                using var playback = _MidiFile.GetPlayback(outputDevice);

                var notes = _MidiFile.GetNotes();
                _NotePlayingCancelSource ??= new CancellationTokenSource();

                foreach (var notesByStartTime in notes.GroupBy(g => g.Time).Take(notesToPlay))
                {
                    var timespanToStartNextNote = notesByStartTime
                        .Select(n => n.EndTimeAs<MetricTimeSpan>(_TempoMap) - n.TimeAs<MetricTimeSpan>(_TempoMap))
                        .Min()
                    ;

                    foreach (var note in notesByStartTime)
                    {
                        outputDevice.SendEvent(new NoteOnEvent(note.NoteNumber, note.Velocity)
                        {
                            Channel = note.Channel,
                            DeltaTime = note.Time,
                        });

                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                var start = note.TimeAs<MetricTimeSpan>(_TempoMap);
                                var end = note.EndTimeAs<MetricTimeSpan>(_TempoMap);
                                await Task.Delay(end - start, _NotePlayingCancelSource.Token);
                            }
                            catch (Exception)
                            {
                            }

                            outputDevice.SendEvent(new NoteOffEvent(note.NoteNumber, note.Velocity)
                            {
                                Channel = note.Channel,
                                DeltaTime = note.Time,
                            });
                        });
                    }

                    try
                    {
                        await Task.Delay(timespanToStartNextNote, _NotePlayingCancelSource.Token);
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }
    }
}
