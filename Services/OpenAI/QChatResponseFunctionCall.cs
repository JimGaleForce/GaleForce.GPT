using System;
using System.Linq;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QChatResponseFunctionCall
    {
        public string Name { get; set; }

        public string Arguments { get; set; }
    }
}
