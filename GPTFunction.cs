namespace GaleForce.GPT
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenAILib;

    public class GPTFunction
    {
        public GPTFunction()
        {
        }

        public GPTFunction(string instructions, GPTService service = null)
        {
            this.Instructions = instructions;
            this.Service = service;
        }

        public string Instructions { get; set; }

        public string Prompt { get; set; }

        public GPTService Service { get; }

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

            return await this.Service.Do(this.On(prompt), retries);
        }
    }

    public class GPTService
    {
        public static int DefaultRetries = 3;
        public static int DefaultRetryDelayMS = 1000;

        public GPTService(AzureOpenAIClient azureOpenAIClient)
        {
            this.AzureOpenAIClient = azureOpenAIClient;
        }

        public GPTService(OpenAIClient openAIClient)
        {
            this.OpenAIClient = openAIClient;
        }

        public AzureOpenAIClient AzureOpenAIClient { get; set; }

        public OpenAIClient OpenAIClient { get; set; }

        public bool UseInstructionsInPrompt { get; set; } = false;

        public Func<string, string, string> FakeReturn { get; set; } //instructions, prompt, return

        public async Task<string> Do(GPTFunction function, int retries = int.MinValue)
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
                    () => this.AzureOpenAIClient.GetChatCompletionAsync(sequence),
                    retries);
            }

            if (this.OpenAIClient != null)
            {
                return await this.Retry(
                    () => this.OpenAIClient.GetChatCompletionAsync(sequence),
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