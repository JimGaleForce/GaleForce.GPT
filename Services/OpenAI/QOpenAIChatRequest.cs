using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QOpenAIChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "gpt-4";

        [JsonPropertyName("messages")]
        public List<QOpenAIChatItem> ChatMessages { get; set; }

        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; } = 1;

        [JsonPropertyName("top_p")]
        public double? TopP { get; set; } = 1;

        [JsonPropertyName("n")]
        public int? N { get; set; } = 1;

        [JsonPropertyName("stream")]
        public bool? Stream { get; set; } = false;

        [JsonPropertyName("stop")]
        public string[] Stop { get; set; }

        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; set; } = 0;

        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; set; } = 0;

        [JsonPropertyName("logit_bias")]
        public Dictionary<string, int> LogitBias { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("functions")]
        public List<QOpenAIFunction> Functions { get; set; }

        public bool _IsReadyToResubmit()
        {
            return Functions.Any(f => f._IsReadyToResubmit);
        }

        public async Task _SetUsed()
        {
            var fns = Functions.Where(f => f._IsReadyToResubmit).ToList();
            foreach (var fn in fns)
            {
                await fn.ChangeStage(QOpenAIFunctionStage.Used);
            }
        }
    }
}
