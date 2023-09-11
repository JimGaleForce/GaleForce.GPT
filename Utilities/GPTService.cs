namespace GaleForce.GPT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenAILib;

    public class GPTService
    {
        public static int DefaultRetries = 3;
        public static int DefaultRetryDelayMS = 1000;

        public GPTService(AzureOpenAIClient azureOpenAIClient, string model = null)
        {
            AzureOpenAIClient = azureOpenAIClient;
            if (!string.IsNullOrEmpty(model))
            {
                Model = model;
            }
        }

        public GPTService(OpenAIClient openAIClient, string model = null)
        {
            OpenAIClient = openAIClient;
            if (!string.IsNullOrEmpty(model))
            {
                Model = model;
            }
        }

        public AzureOpenAIClient AzureOpenAIClient { get; set; }

        public OpenAIClient OpenAIClient { get; set; }

        public bool UseInstructionsInPrompt { get; set; } = false;

        public Func<string, string, string> FakeReturn { get; set; } //instructions, prompt, return

        public string Model { get; set; }

        public async Task<string> Do(GPTFunction function, int retries = int.MinValue, string model = null)
        {
            if (retries == int.MinValue)
            {
                retries = DefaultRetries;
            }

            var sequence = new List<ChatMessage>();
            var prompt = UseInstructionsInPrompt
                ? (function.Instructions ?? string.Empty) + "\n" + function.Prompt
                : function.Prompt;
            var inst = UseInstructionsInPrompt ? null : function.Instructions;

            if (FakeReturn != null)
            {
                return FakeReturn(inst, prompt);
            }

            if (!string.IsNullOrEmpty(inst))
            {
                sequence.Add(new ChatMessage(ChatRole.System, inst));
            }

            sequence.Add(new ChatMessage(ChatRole.User, prompt));

            if (AzureOpenAIClient != null)
            {
                return await Retry(
                    () => AzureOpenAIClient
                        .GetChatCompletionAsync(
                            sequence,
                            (chatComp) =>
                            {
                                if (!string.IsNullOrEmpty(model ?? Model))
                                {
                                    chatComp.Model(model ?? Model);
                                }
                            }),
                    retries);
            }

            if (OpenAIClient != null)
            {
                return await Retry(
                    () => OpenAIClient
                        .GetChatCompletionAsync(
                            sequence,
                            (chatComp) =>
                            {
                                if (!string.IsNullOrEmpty(model ?? Model))
                                {
                                    chatComp.Model(model ?? Model);
                                }
                            }),
                    retries);
            }

            return null;
        }

        private async Task<string> Retry(Func<Task<string>> func, int retries)
        {
            Exception e = null;
            var count = retries;
            var delay = DefaultRetryDelayMS;
            while (count-- > 0)
            {
                try
                {
                    var result = await func();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(delay); //todo: pass in service default
                    delay += delay;
                    e = ex;
                }
            }

            throw e;
        }
    }
}