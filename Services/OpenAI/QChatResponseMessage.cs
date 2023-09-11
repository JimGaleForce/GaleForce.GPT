using System;
using System.Linq;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QChatResponseMessage
    {
        public string Role { get; set; }

        public string Content { get; set; }

        public QChatResponseFunctionCall Function_Call { get; set; }
    }
}
