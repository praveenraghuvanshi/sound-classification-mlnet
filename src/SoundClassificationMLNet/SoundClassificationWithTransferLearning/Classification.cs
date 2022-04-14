using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Vision;
using static Microsoft.ML.DataOperationsCatalog;

namespace SoundClassificationWithTransferLearning
{
    /// <summary>
    /// Classifies the sound
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/image-classification-api-transfer-learning"/>
    /// </summary>
    public class Classification
    {
        private static string AudioImageDatasetPath = @"D:\temp\dataset\images_original";
        private string[] _supportedExtensions = new[] {".jpg", ".png"};

        private MLContext _mlContext;
        private ITransformer _trainedModel;

        /// <summary>
        /// Classify a sound
        /// </summary>
        /// <param name="audioFile">An audio file with fully qualified path</param>
        /// <returns>Predicted genre</returns>
        public string Classify(string audioFile)
        {
            if (_mlContext == null || _trainedModel == null)
            {
                return "Model not trained";
            }

            ClassifySingleImage(_mlContext, _trainedModel, audioFile);

            return audioFile;
        }

        public void TrainAndEvaluate()
        {
            // Load Dataset
            IEnumerable<SoundImageData> images = LoadImagesFromDirectory(folder: AudioImageDatasetPath);

            // ML Context
            var mlContext = new MLContext(1);
            IDataView imageData = mlContext.Data.LoadFromEnumerable(images);
            IDataView shuffledData = mlContext.Data.ShuffleRows(imageData);

            // Pipeline
            var preprocessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: nameof(ModelInput.Label),
                    outputColumnName: nameof(ModelInput.LabelAsKey))
                .Append(mlContext.Transforms.LoadRawImageBytes(
                    outputColumnName: "Image",
                    imageFolder: AudioImageDatasetPath,
                    inputColumnName: nameof(ModelInput.ImagePath)));

            IDataView preProcessedData = preprocessingPipeline
                .Fit(shuffledData)
                .Transform(shuffledData);

            // Split Dataset : Train/Test/Validation
            TrainTestData trainSplit = mlContext.Data.TrainTestSplit(data: preProcessedData, testFraction: 0.3);
            TrainTestData validationTestSplit = mlContext.Data.TrainTestSplit(trainSplit.TestSet);
            IDataView trainSet = trainSplit.TrainSet;                // 70% of total dataset
            IDataView validationSet = validationTestSplit.TrainSet;  // 90% of 30% of total dataset
            IDataView testSet = validationTestSplit.TestSet;         // 10% of 30% of total dataset

            // Classifier Options
            // Architecture - ResnetV2101
            var classifierOptions = new ImageClassificationTrainer.Options()
            {
                FeatureColumnName = "Image",
                LabelColumnName = "LabelAsKey",
                ValidationSet = validationSet,
                Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
                MetricsCallback = (metrics) => Console.WriteLine(metrics),
                TestOnTrainSet = false,
                ReuseTrainSetBottleneckCachedValues = true,
                ReuseValidationSetBottleneckCachedValues = true
            };

            // Training Pipeline
            var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(classifierOptions)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(nameof(ModelOutput.PredictedLabel)));

            // Train Model
            ITransformer trainedModel = trainingPipeline.Fit(trainSet);

            _mlContext = mlContext;
            _trainedModel = trainedModel;
        }

        /// <summary>
        /// Loads images from specified directory
        /// </summary>
        /// <param name="folder">Directory containing images</param>
        /// <param name="useFolderNameAsLabel">True, if name needs to be preserved</param>
        /// <returns>A collection of images</returns>
        private IEnumerable<SoundImageData> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        {
            var files = Directory.GetFiles(folder, "*", searchOption: SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                if (!_supportedExtensions.Contains(extension))
                {
                    continue;
                }

                var label = Path.GetFileName(file);

                if (useFolderNameAsLabel)
                {
                    label = Directory.GetParent(file).Name;
                }
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new SoundImageData
                {
                    ImagePath = file,
                    Label = label
                };
            }
        }

        private void OutputPrediction(ModelOutput prediction)
        {
            string imageName = Path.GetFileName(prediction.ImagePath);
            Console.WriteLine($"Image: {imageName} | Actual Value: {prediction.Label} | Predicted Value: {prediction.PredictedLabel}");
        }

        void ClassifySingleImage(MLContext mlContext, ITransformer trainedModel, string imageToBePredicted)
        {
            PredictionEngine<ModelInput, ModelOutput> predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);
            ModelInput image = CreateModelInput(mlContext, imageToBePredicted);// mlContext.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: true).First();
            ModelOutput prediction = predictionEngine.Predict(image);
            Console.WriteLine("Classifying single image");
            OutputPrediction(prediction);
        }

        private ModelInput CreateModelInput(MLContext mlContext, string imageToBePredicted)
        {
            var imageBytes = File.ReadAllBytes(imageToBePredicted);
            var input = new ModelInput
            {
                ImagePath = imageToBePredicted,
                Label = "blues",
                Image = imageBytes
            };

            return input;
        }
    }
}
