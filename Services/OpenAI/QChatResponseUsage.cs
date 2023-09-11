using System;
using System.Linq;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QChatResponseUsage
    {
        public int PromptTokens { get; set; }

        public int CompletionTokens { get; set; }

        public int TotalTokens { get; set; }
    }
}
