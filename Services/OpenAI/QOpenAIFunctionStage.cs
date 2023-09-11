using System;
using System.Linq;

namespace GaleForce.GPT.Services.OpenAI
{
    public enum QOpenAIFunctionStage
    {
        None,
        ArgumentsPopulated,
        Executing,
        ResponseContentPopulated,
        ResponseError,
        ReadyToRespond,
        Used
    }
}
