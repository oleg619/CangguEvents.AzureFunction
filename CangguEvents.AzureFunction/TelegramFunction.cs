using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace CangguEvents.AzureFunction
{
    public static class TelegramFunction
    {
        [FunctionName("Telegram")]
        public static async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var handler = GetTelegramMessageHandler();


            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Update>(requestBody);

            await handler.Handle(update);

            return Ok();
        }

        [FunctionName("Time")]
        public static async Task<IActionResult> Time(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            await Task.CompletedTask;
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(DateTime.UtcNow);
        }
        
        private static TelegramMessageHandler GetTelegramMessageHandler()
        {
            var token = GetEnvironmentVariable("Token");
            var handler = new TelegramMessageHandler(new TelegramBotClient(token));
            return handler;
        }

        private static string GetEnvironmentVariable(string name) =>
            Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        private static IActionResult Ok() => new OkResult();
    }
}