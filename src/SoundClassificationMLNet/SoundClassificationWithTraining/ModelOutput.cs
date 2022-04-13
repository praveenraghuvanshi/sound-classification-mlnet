using Microsoft.ML.Data;

namespace SoundClassificationWithTraining
{
    public class ModelOutput
    {
        [ColumnName("modelOutput")]
        public float[] Score { get; set; }
    }
}
