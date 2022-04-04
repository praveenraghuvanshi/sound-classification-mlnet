using Microsoft.ML.Data;

namespace SoundClassification
{
    public class ModelOutput
    {
        [ColumnName("modelOutput")]
        public float[] Score { get; set; }
    }
}
