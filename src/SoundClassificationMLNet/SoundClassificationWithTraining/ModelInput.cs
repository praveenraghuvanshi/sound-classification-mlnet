using System.Drawing;
using Microsoft.ML.Transforms.Image;

namespace SoundClassificationWithTraining
{
    public class ModelInput
    {
        [ImageType(ImageSettings.Height, ImageSettings.Width)]
        public Bitmap ImageSource { get; set; }
    }
}
