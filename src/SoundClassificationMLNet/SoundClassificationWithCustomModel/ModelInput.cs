using System.Drawing;
using Microsoft.ML.Transforms.Image;

namespace SoundClassificationWithCustomModel
{
    public class ModelInput
    {
        [ImageType(ImageSettings.Height, ImageSettings.Width)]
        public Image ImageSource { get; set; }
    }
}
