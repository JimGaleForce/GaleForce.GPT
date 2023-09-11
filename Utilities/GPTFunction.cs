namespace GaleForce.GPT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenAILib;

    public class GPTFunction
    {
        public GPTFunction()
        {
        }

        public GPTFunction(string instructions, GPTService service = null, string model = null)
        {
            Instructions = instructions;
            Service = service;
            Model = model;
        }

        public string Instructions { get; set; }

        public string Prompt { get; set; }

        public GPTService Service { get; }

        public string Model { get; set; }

        public GPTFunction SetInstruction(string instructions)
        {
            Instructions = instructions;
            return this;
        }

        public GPTFunction SetPrompt(string prompt)
        {
            Prompt = prompt;
            return this;
        }

        public GPTFunction On(string prompt)
        {
            Prompt = prompt;
            return this;
        }

        public async Task<string> ActOn(string prompt, int retries = int.MinValue)
        {
            if (Service == null)
            {
                throw new Exception("Missing service");
            }

            return await Service.Do(On(prompt), retries, Model);
        }
    }
}