using System;
using System.Linq;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QChatResponseChoice
    {
        public int Index { get; set; }

        public QChatResponseMessage Message { get; set; }

        public string Finish_Reason { get; set; }
    }
}
