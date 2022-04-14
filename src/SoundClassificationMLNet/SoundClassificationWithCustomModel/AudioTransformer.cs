using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using NAudio.Wave;
using Spectrogram;

namespace SoundClassificationWithCustomModel
{
    public class AudioTransformer
    {
        private string MelSpectroExtension = "-melspectro.png";

        private int _fftSize = 4096;
        private int _stepSize = 500;
        private int _maxFreq = 3000;
        private int _melBinCount = 250;

        public string TransformAndSave(string audioFile)
        {
            // Create Spectrogram
            var spectrogramFile = CreateSpectrogram(audioFile);
            return spectrogramFile;
        }

        private string CreateSpectrogram(string fileName)
        {
            (double[] audio, int sampleRate) = ReadWavMono(fileName);
            var sg = new SpectrogramGenerator(sampleRate, fftSize: _fftSize, stepSize: _stepSize, maxFreq: _maxFreq);
            sg.Add(audio);

            // Create a traditional (linear) Spectrogram
            // sg.SaveImage("hal.png");

            // Create a Mel Spectrogram
            Bitmap bmp = sg.GetBitmapMel(melBinCount: _melBinCount);
            
            var spectrogramName = fileName.Substring(0, fileName.Length - 4) + MelSpectroExtension;

            if (File.Exists(spectrogramName))
            {
                return spectrogramName;
            }

            bmp.Save(spectrogramName, ImageFormat.Png);

            return spectrogramName;
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
    }
}
