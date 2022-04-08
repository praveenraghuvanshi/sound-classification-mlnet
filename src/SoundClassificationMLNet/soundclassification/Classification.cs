using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;

namespace SoundClassification
{
    public class Classification
    {
        private string ModelInputName = "modelInput";
        private string ModelOutputName = "modelOutput";
        private string AssetsFolder = "assets";
        private string ModelFileName = "SoundClassifier.onnx";
        private string LabelFileName = "labels.txt";
        private string AudioFileExtension = ".wav";

        private string _assetsLocation;
        private string _modelFile;
        private List<string> _classes;
        private readonly AudioTransformer _audioTransformer;

        public Classification()
        {
            _assetsLocation = Path.Combine(Directory.GetCurrentDirectory(), AssetsFolder);
            _modelFile = Path.Combine(_assetsLocation, ModelFileName);
            _classes = File.ReadLines(Path.Combine(_assetsLocation, LabelFileName)).ToList();
            _audioTransformer = new AudioTransformer();
        }

        public string Classify(string audioFile)
        {
            var fileExtension = Path.GetExtension(audioFile);
            if (fileExtension == AudioFileExtension)
            {
                audioFile = _audioTransformer.TransformAndSave(audioFile);
            }

            var mlContext = new MLContext(seed: 1);
            var model = BuildModel(mlContext);
            var input = BuildInput(audioFile);
            var predictedOutput = Predict(mlContext, model, input);

            var maxScore = predictedOutput.Score.Max();
            var maxIndex = predictedOutput.Score.ToList().IndexOf(maxScore);

            var classifiedSound = _classes[maxIndex];

            return classifiedSound;
        }

        private ModelOutput Predict(MLContext mlContext, ITransformer model, ModelInput input)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
            var predictedOutput = predictionEngine.Predict(input);
            return predictedOutput;
        }

        private ITransformer BuildModel(MLContext mlContext)
        {
            var emptyInput = new List<ModelInput>();
            var data = mlContext.Data.LoadFromEnumerable(emptyInput);

            var pipeline = mlContext.Transforms.ResizeImages(resizing: ImageResizingEstimator.ResizingKind.Fill, outputColumnName: ModelInputName, imageWidth: ImageSettings.Width, imageHeight: ImageSettings.Height, inputColumnName: nameof(ModelInput.ImageSource))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: ModelInputName))
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: _modelFile, outputColumnName: ModelOutputName, inputColumnName: ModelInputName));

            var model = pipeline.Fit(data);

            return model;
        }

        private ModelInput BuildInput(string inputImage)
        {
            Bitmap testImage = (Bitmap)Image.FromFile(inputImage);

            ModelInput inputData = new ModelInput()
            {
                ImageSource = testImage
            };

            return inputData;
        }
    }
}
