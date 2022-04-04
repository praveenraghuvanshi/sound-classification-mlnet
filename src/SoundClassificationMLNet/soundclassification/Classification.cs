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
        private string MODEL_INPUT_NAME = "modelInput";
        private string MODEL_OUTPUT_NAME = "modelOutput";
        private string ASSETS_FOLDER = "assets";
        private string MODEL_FILE_NAME = "SoundClassifier.onnx";
        private string LABEL_FILE_NAME = "labels.txt";

        private string _assetsLocation;
        private string _modelFile;
        private List<string> _classes;

        public Classification()
        {
            _assetsLocation = Path.Combine(Directory.GetCurrentDirectory(), ASSETS_FOLDER);
            _modelFile = Path.Combine(_assetsLocation, MODEL_FILE_NAME);
            _classes = File.ReadLines(Path.Combine(_assetsLocation, LABEL_FILE_NAME)).ToList();
        }

    public string Classify(string spectrogramImage)
        {
            var mlContext = new MLContext(seed: 1);
            var model = BuildModel(mlContext);
            var input = BuildInput(spectrogramImage);
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

            var pipeline = mlContext.Transforms.ResizeImages(resizing: ImageResizingEstimator.ResizingKind.Fill, outputColumnName: MODEL_INPUT_NAME, imageWidth: ImageSettings.Width, imageHeight: ImageSettings.Height, inputColumnName: nameof(ModelInput.ImageSource))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: MODEL_INPUT_NAME))
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: _modelFile, outputColumnName: MODEL_OUTPUT_NAME, inputColumnName: MODEL_INPUT_NAME));

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
