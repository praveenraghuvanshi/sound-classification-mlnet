using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Data.Analysis;

namespace SoundClassificationDotNetInteractive
{
    class Program
    {
        private static string Features3SecondsPath = "features_30_sec.csv";
        private static string SampleAudioFilePath = "classical.00095.wav";
        private static string[] Labels = {"blues", "classical", "country", "disco", "hiphop", "jazz", "metal", "pop", "reggae", "rock"};

        private static string AudioFileDatasetPath = @"D:\temp\dataset\genres_original";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ExploratoryDataAnalysis();

            // PlayAudio(SampleAudioFilePath);

            PlotMelSpectrogram(SampleAudioFilePath);

            SplitDataset(AudioFileDatasetPath, Labels);
        }

        private static void PlotMelSpectrogram(string sampleAudioFilePath)
        {
            (double[] audio, int sampleRate) = ReadWavMono(sampleAudioFilePath);
            var sg = new Spectrogram.Spectrogram(sampleRate, fftSize: 4096, stepSize: 500, maxFreq: 3000);
            sg.Add(audio);

            Bitmap bmp = sg.GetBitmapMel(melBinCount: 250);
            bmp.Save("sample.png");
        }

        private static void PlayAudio(string sampleAudioFilePath)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(sampleAudioFilePath);
            player.Play();

            System.Threading.Thread.Sleep(5000);

            player.Stop();
        }

        private static void ExploratoryDataAnalysis()
        {
            DataFrameAnalysis();

            AudioFileAnalysis();
        }

        private static void DataFrameAnalysis()
        {
            var soundDataFrame = DataFrame.LoadCsv(Features3SecondsPath);
        }

        private static void AudioFileAnalysis()
        {
            // Load Audio file
            using var afr = new NAudio.Wave.AudioFileReader(SampleAudioFilePath);
            int sampleRate = afr.WaveFormat.SampleRate;
            int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
            int sampleCount = (int)(afr.Length / bytesPerSample);
            int channelCount = afr.WaveFormat.Channels;
        }

        private static (double[] audio, int sampleRate) ReadWavMono(string filePath, double multiplier = 16_000)
        {
            using var afr = new NAudio.Wave.AudioFileReader(filePath);
            int sampleRate = afr.WaveFormat.SampleRate;
            int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
            int sampleCount = (int)(afr.Length / bytesPerSample);
            int channelCount = afr.WaveFormat.Channels;
            var audio = new List<double>(sampleCount);
            var buffer = new float[sampleRate * channelCount];
            int samplesRead = 0;
            while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
                audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
            return (audio.ToArray(), sampleRate);
        }

        private static void SplitDataset(string directory, string[] labels)
        {
            var trainPath = Path.Combine(directory, "train");
            var testPath = Path.Combine(directory, "test");
            var validationPath = Path.Combine(directory, "val");

            // Train
            if (!Directory.Exists(trainPath))
            {
                Directory.CreateDirectory(trainPath);
                foreach (var label in labels)
                {
                    var subDirectory = Path.Combine(trainPath, label);
                    Directory.CreateDirectory(subDirectory);

                    // Copy Files
                    var sourcePath = Path.Combine(directory, label);
                    var top70Files = Directory.EnumerateFiles(sourcePath).Take(70);
                    foreach (var file in top70Files)
                    {
                        File.Copy(file, Path.Combine(subDirectory, Path.GetFileName(file)));
                    }
                }
            }

            // Test
            if (!Directory.Exists(testPath))
            {
                Directory.CreateDirectory(testPath);
                foreach (var label in labels)
                {
                    var subDirectory = Path.Combine(testPath, label);
                    Directory.CreateDirectory(subDirectory);

                    // Copy Files
                    var sourcePath = Path.Combine(directory, label);
                    var allFiles = Directory.EnumerateFiles(sourcePath).ToList();
                    var testFiles = allFiles.GetRange(70, 20);
                    foreach (var file in testFiles)
                    {
                        File.Copy(file, Path.Combine(subDirectory, Path.GetFileName(file)));
                    }
                }
            }

            // Validation
            if (!Directory.Exists(validationPath))
            {
                Directory.CreateDirectory(validationPath);
                foreach (var label in labels)
                {
                    var subDirectory = Path.Combine(validationPath, label);
                    Directory.CreateDirectory(subDirectory);

                    // Copy Files
                    var sourcePath = Path.Combine(directory, label);
                    var allFiles = Directory.EnumerateFiles(sourcePath).ToList();
                    var testFiles = allFiles.GetRange(90, 10);
                    foreach (var file in testFiles)
                    {
                        File.Copy(file, Path.Combine(subDirectory, Path.GetFileName(file)));
                    }
                }
            }
        }
    }
}
