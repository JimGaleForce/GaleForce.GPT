using System;
using System.Linq;

namespace GaleForce.GPT.Models
{
    public class QAudioData
    {
        public byte[] AudioData { get; set; }

        public string AudioType { get; set; } = "audio/mpeg";

        public string FileName { get; set; } = "recording.mp3";
    }
}
