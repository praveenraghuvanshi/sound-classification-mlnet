using System;
using SoundClassification;

namespace SoundClassificationConsoleApp
{
    class Program
    {
        private static string testImage = @"testdata\classical00095.png";

        static void Main(string[] args)
        {
            Console.WriteLine("Sound Classification using ML.Net");

            ClassifySound();
        }

        private static void ClassifySound()
        {
            Classification classification = new Classification();
            var sound = classification.Classify(testImage);

            Console.WriteLine($"Predicted sound is of {sound}");
        }
    }
}
