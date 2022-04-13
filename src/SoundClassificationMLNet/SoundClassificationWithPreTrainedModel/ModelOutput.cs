using Microsoft.ML.Data;

namespace SoundClassificationWithPreTrainedModel
{
    public class ModelOutput
    {
        [ColumnName("modelOutput")]
        public float[] Score { get; set; }
    }
}
