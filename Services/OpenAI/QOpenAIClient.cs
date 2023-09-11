using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GaleForce.GPT.Utilities;
using Newtonsoft.Json;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QOpenAIClient
    {
        public string ApiKey { get; set; }

        public string OrgId { get; set; }

        public QOpenAIClient(string apiKey, string orgId = null)
        {
            ApiKey = apiKey;
            OrgId = orgId;
        }

        public async Task<string> Transcribe(QOpenAIWhisper whisper)
        {
            var endPoint = "https://api.openai.com/v1/audio/transcriptions";
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(whisper.AudioData);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");

                    content.Add(fileContent, "file", whisper.FileName);
                    content.Add(new StringContent(whisper.Model), "model");

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ApiKey);

                    var response = await client.PostAsync(endPoint, content);
                    var responseData = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<QOpenAIWhisperResponse>(responseData);

                    return data.Text;
                }
            }
        }

        // chat
        public async Task<QChatResponse> Chat(QOpenAIChatRequest chatRequest, bool populateFunctions = false)
        {
            var url = "https://api.openai.com/v1/chat/completions";
            var apiKey = ApiKey;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var messages = new List<Dictionary<string, string>>();
            foreach (var message in chatRequest.ChatMessages)
            {
                messages.Add(
                    new Dictionary<string, string>
                    { { "role", message.Type.ToString().ToLower() }, { "content", message.Content } });
                if (message.Name != null)
                {
                    messages.Last().Add("name", message.Name);
                }
            }

            var functions = chatRequest.Functions == null
                ? null
                : GPTFunctionUtils.AsJsonArray(GPTFunctionUtils.GPTFunctionJson(chatRequest.Functions));

            var payload = new { model = chatRequest.Model, messages, functions };

            var jsonParams = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var jsonPayload = JsonConvert.SerializeObject(payload, jsonParams);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<QChatResponse>(responseContent);
                if (responseObject._HasFunctionRequest)
                {
                    var fnInfo = responseObject.Choices.First().Message.Function_Call;
                    var fn = GPTFunctionUtils.PopulateByName(chatRequest.Functions, fnInfo.Name, fnInfo.Arguments);
                    if (fn != null)
                    {
                        await fn.ChangeStage(QOpenAIFunctionStage.ArgumentsPopulated);

                        if (fn._HasFunctionResponse)
                        {
                            chatRequest.ChatMessages
                                .Add(
                                    new QOpenAIChatItem
                                    {
                                        Type = QOpenAIChatItemRole.Function,
                                        Name = fnInfo.Name,
                                        Content = JsonConvert.SerializeObject(fn.GetResult())
                                    });

                            await fn.ChangeStage(QOpenAIFunctionStage.ReadyToRespond);
                        }
                    }
                }

                return responseObject;
            }
            else
            {
                throw new Exception("API request failed:" + response.ToString() + payload);
            }
        }
    }
}
