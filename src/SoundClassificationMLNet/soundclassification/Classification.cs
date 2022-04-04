using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;

namespace SoundClassification
{
    public class Classification
    {
        private string MODEL_INPUT_NAME = "modelInput";
        private string MODEL_OUTPUT_NAME = "modelOutput";
        private string MODEL_FILE = @"assets\SoundClassifier.onnx";

        private string[] CLASSES = { "blues", "classical", "country", "disco", "hiphop", "jazz", "metal", "pop", "reggae", "rock" };

        public string Classify(string spectrogramImage)
        {
            var mlContext = new MLContext(seed: 1);
            var model = BuildModel(mlContext);
            var input = BuildInput(spectrogramImage);
            var predictedOutput = Predict(mlContext, model, input);

            var maxScore = predictedOutput.Score.Max();
            var maxIndex = predictedOutput.Score.ToList().IndexOf(maxScore);

            var classifiedSound = CLASSES[maxIndex];

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
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: MODEL_FILE, outputColumnName: MODEL_OUTPUT_NAME, inputColumnName: MODEL_INPUT_NAME));

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
