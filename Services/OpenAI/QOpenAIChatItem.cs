using System;
using System.Linq;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QOpenAIChatItem
    {
        public QOpenAIChatItemRole Type { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public QOpenAIChatItem()
        {
        }

        public QOpenAIChatItem(QOpenAIChatItemRole type, string content, string name = null)
        {
            Type = type;
            Name = name;
            Content = content;
        }
    }
}
