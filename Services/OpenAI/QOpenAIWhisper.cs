using System;
using System.Linq;
using GaleForce.GPT.Models;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QOpenAIWhisper : QAudioData
    {
        public string Model { get; set; } = "whisper-1";
    }
}
