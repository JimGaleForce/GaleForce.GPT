namespace TestGaleForce.GPT
{
    using GaleForce.GPT.Utilities;
    using OpenAILib;

    [TestClass]
    public class OpenAILibTests
    {
        private AzureOpenAIClient chat35Client;

        private GPTService service { get; set; }

        [TestInitialize]
        public void Init()
        {
            string apiKey = Environment.GetEnvironmentVariable("ApiKey");
            string resName = Environment.GetEnvironmentVariable("ResourceName");
            string embedDeploy = Environment.GetEnvironmentVariable("EmbedDeploymentId");
            string embedApi = Environment.GetEnvironmentVariable("EmbedApiVersion");
            string completionDeploy = Environment.GetEnvironmentVariable("CompletionDeploymentId");
            string completionApi = Environment.GetEnvironmentVariable("CompletionApiVersion");
            string chatDeploy = Environment.GetEnvironmentVariable("ChatDeploymentId");
            string chat35Deploy = Environment.GetEnvironmentVariable("Chat35DeploymentId");
            string chatApi = Environment.GetEnvironmentVariable("ChatApiVersion");

            chat35Client = new AzureOpenAIClient(
                apiKey: apiKey,
                resourceName: resName,
                deploymentId: chat35Deploy,
                apiVersion: chatApi);

            service = new GPTService(chat35Client);
        }

        // [TestMethod]
        public async Task TestMethod1()
        {
            var writeTextBackwards = new GPTFunction("Write text backwards", service);
            var result = await writeTextBackwards.ActOn("This is a test");
        }
    }
}