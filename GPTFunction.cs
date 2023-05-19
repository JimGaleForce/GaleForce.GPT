namespace GaleForce.GPT
{
    using System;
    using System.Collections.Generic;
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

        public async Task<string> ActOn(string prompt)
        {
            if (this.Service == null)
            {
                throw new Exception("Missing service");
            }

            return await this.Service.Do(this.On(prompt));
        }
    }

    public class GPTService
    {
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

        public async Task<string> Do(GPTFunction function)
        {
            var sequence = new List<ChatMessage>();
            var prompt = this.UseInstructionsInPrompt
                ?
                (function.Instructions ?? "") + "\n" + function.Prompt
                : function.Prompt;
            var inst = this.UseInstructionsInPrompt ? null : function.Instructions;

            if (!string.IsNullOrEmpty(inst))
            {
                sequence.Add(new ChatMessage(ChatRole.System, inst));
            }

            sequence.Add(new ChatMessage(ChatRole.User, prompt));

            if (this.AzureOpenAIClient != null)
            {
                return await this.AzureOpenAIClient.GetChatCompletionAsync(sequence);
            }

            if (this.OpenAIClient != null)
            {
                return await this.OpenAIClient.GetChatCompletionAsync(sequence);
            }

            return null;
        }
    }
}