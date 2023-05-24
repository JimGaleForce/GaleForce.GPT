namespace GaleForce.GPT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenAILib;

    public class GPTFunction
    {
        public GPTFunction()
        {
        }

        public GPTFunction(string instructions, GPTService service = null, string model = null)
        {
            this.Instructions = instructions;
            this.Service = service;
            this.Model = model;
        }

        public string Instructions { get; set; }

        public string Prompt { get; set; }

        public GPTService Service { get; }

        public string Model { get; set; }

        public GPTFunction SetInstruction(string instructions)
        {
            this.Instructions = instructions;
            return this;
        }

        public GPTFunction SetPrompt(string prompt)
        {
            this.Prompt = prompt;
            return this;
        }

        public GPTFunction On(string prompt)
        {
            this.Prompt = prompt;
            return this;
        }

        public async Task<string> ActOn(string prompt, int retries = int.MinValue)
        {
            if (this.Service == null)
            {
                throw new Exception("Missing service");
            }

            return await this.Service.Do(this.On(prompt), retries, this.Model);
        }
    }

    public class GPTService
    {
        public static int DefaultRetries = 3;
        public static int DefaultRetryDelayMS = 1000;

        public GPTService(AzureOpenAIClient azureOpenAIClient, string model = null)
        {
            this.AzureOpenAIClient = azureOpenAIClient;
            if (!string.IsNullOrEmpty(model))
            {
                this.Model = model;
            }
        }

        public GPTService(OpenAIClient openAIClient, string model = null)
        {
            this.OpenAIClient = openAIClient;
            if (!string.IsNullOrEmpty(model))
            {
                this.Model = model;
            }
        }

        public AzureOpenAIClient AzureOpenAIClient { get; set; }

        public OpenAIClient OpenAIClient { get; set; }

        public bool UseInstructionsInPrompt { get; set; } = false;

        public Func<string, string, string> FakeReturn { get; set; } //instructions, prompt, return

        public string Model { get; set; }

        public async Task<string> Do(
            GPTFunction function,
            int retries = int.MinValue,
            string model = null)
        {
            if (retries == int.MinValue)
            {
                retries = DefaultRetries;
            }

            var sequence = new List<ChatMessage>();
            var prompt = this.UseInstructionsInPrompt
                ? (function.Instructions ?? string.Empty) + "\n" + function.Prompt
                : function.Prompt;
            var inst = this.UseInstructionsInPrompt ? null : function.Instructions;

            if (this.FakeReturn != null)
            {
                return this.FakeReturn(inst, prompt);
            }

            if (!string.IsNullOrEmpty(inst))
            {
                sequence.Add(new ChatMessage(ChatRole.System, inst));
            }

            sequence.Add(new ChatMessage(ChatRole.User, prompt));

            if (this.AzureOpenAIClient != null)
            {
                return await this.Retry(
                    () => this.AzureOpenAIClient
                        .GetChatCompletionAsync(
                            sequence,
                            (chatComp) =>
                            {
                                if (!string.IsNullOrEmpty(model ?? this.Model))
                                {
                                    chatComp.Model(model ?? this.Model);
                                }
                            }),
                    retries);
            }

            if (this.OpenAIClient != null)
            {
                return await this.Retry(
                    () => this.OpenAIClient
                        .GetChatCompletionAsync(
                            sequence,
                            (chatComp) =>
                            {
                                if (!string.IsNullOrEmpty(model ?? this.Model))
                                {
                                    chatComp.Model(model ?? this.Model);
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
                    Thread.Sleep(1000); //todo: pass in service default
                    e = ex;
                }
            }

            throw e;
        }
    }
}