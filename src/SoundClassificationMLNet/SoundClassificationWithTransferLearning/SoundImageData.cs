namespace SoundClassificationWithTransferLearning
{
    /// <summary>
    /// Information about the image representation of sound such as Spectrogram
    /// </summary>
    public class SoundImageData
    {
        /// <summary>
        /// Gets or sets the fully qualified path where the image is stored
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the genre. Its the predicted output
        /// </summary>
        public string Label { get; set; }
    }
}
