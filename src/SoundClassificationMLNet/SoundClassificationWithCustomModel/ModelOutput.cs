using Microsoft.ML.Data;

namespace SoundClassificationWithCustomModel
{
    public class ModelOutput
    {
        [ColumnName("modelOutput")]
        public float[] Score { get; set; }
    }
}
