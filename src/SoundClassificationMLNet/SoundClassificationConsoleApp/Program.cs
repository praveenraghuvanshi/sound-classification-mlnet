using System;
using SoundClassification;

namespace SoundClassificationConsoleApp
{
    class Program
    {
        // private static string testImage = @"testdata\classical\classical00095.png"; // classical
        private static string testImage = @"testdata\hiphop\hiphop00095.png"; // hip-hop


        static void Main(string[] args)
        {
            Console.WriteLine("Sound Classification using ML.Net");

            ClassifySound();
        }

        private static void ClassifySound()
        {
            Classification classification = new Classification();
            var sound = classification.Classify(testImage);

            Console.WriteLine($"Predicted Genre: {sound}");
        }
    }
}
