namespace SoundClassificationWithTransferLearning
{
    /// <summary>
    /// Represents the output of ML Model
    /// </summary>
    public class ModelOutput
    {
        /// <summary>
        /// Gets or sets the fully qualified path of an image.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the original genre of sound.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or set the value predicted by the model
        /// </summary>
        public string PredictedLabel { get; set; }
    }
}
