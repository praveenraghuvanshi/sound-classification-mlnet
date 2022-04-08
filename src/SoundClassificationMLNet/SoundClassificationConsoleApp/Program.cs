using System;
using System.Diagnostics;
using System.IO;
using SoundClassification;

namespace SoundClassificationConsoleApp
{
    class Program
    {
        private static string testDataPath = Path.Combine(Environment.CurrentDirectory, "testdata");

        // Audio Files

        // classical
        private static string testImage = Path.Combine(testDataPath, "images", "classical", "classical00095.png");
        private static string testAudio = Path.Combine(testDataPath, "audio", "classical", "classical.00095.wav");

        // Hip-hop
        // private static string testImage = Path.Combine(testDataPath, "images", "hiphop", "hiphop00095.png");
        // private static string testAudio = Path.Combine(testDataPath, "audio", "hiphop", "hiphop.00095.wav");

        static void Main(string[] args)
        {
            Console.WriteLine("Sound Classification using ML.Net");

            var fileToBeClassified = testImage;

            var classification = new Classification();
            var stopWatch = Stopwatch.StartNew();

            // Classification using Image file
            var classifiedSound = classification.Classify(fileToBeClassified);

            // Classification using Audio file
            // var classifiedSound = classification.Classify(testAudio);

            stopWatch.Stop();

            PrintResult(fileToBeClassified, classifiedSound);

            Console.WriteLine($"\nElapsed Time in ms: {stopWatch.ElapsedMilliseconds}");
        }

        private static void PrintResult(string testFile, string classifiedSound)
        {
            var inputFileName = Path.GetFileName(testFile);
            Console.WriteLine($"Input File: {inputFileName}");
            Console.ForegroundColor = inputFileName.Contains(classifiedSound)
                ? ConsoleColor.Green
                : ConsoleColor.DarkRed;
            Console.WriteLine($"\nClassified Sound: {classifiedSound}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
