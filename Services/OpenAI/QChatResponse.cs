using System;
using System.Collections.Generic;
using System.Linq;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QChatResponse
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public int Created { get; set; }

        public string Model { get; set; }

        public List<QChatResponseChoice> Choices { get; set; }

        public QChatResponseUsage Usage { get; set; }

        public bool _HasFunctionRequest
        {
            get { return Choices != null && Choices.Count > 0 && Choices.First().Finish_Reason == "function_call"; }
        }

        public string GetText()
        {
            return Choices != null && Choices.Count > 0 ? Choices.First().Message.Content : null;
        }
    }
}
