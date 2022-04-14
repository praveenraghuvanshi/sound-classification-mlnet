using System;

namespace SoundClassificationWithTransferLearning
{
    /// <summary>
    /// Represents an input to the ML Model
    /// </summary>
    public class ModelInput
    {
        /// <summary>
        /// Gets or sets the Sound Image Content in bytes
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the numeric representation of label/class
        /// </summary>
        public UInt32 LabelAsKey { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified path of image
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the label or the predicted genre.
        /// </summary>
        public string Label { get; set; }
    }
}
