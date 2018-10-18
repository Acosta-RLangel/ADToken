using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Linq;

namespace SecureAzFunc
{
    public static class SecureAzFunc
    {
        [FunctionName("SecureAzFunc")]
        public static async Task<IActionResult> Run([HttpTrigger(Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel.Anonymous)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var nameHeader = req.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].ToString();

            JsonWebTokenHandler handler = new JsonWebTokenHandler();
            string tokenString = req.Headers["X-MS-TOKEN-AAD-ID-TOKEN"].ToString();

            var token = handler.ReadJsonWebToken(tokenString);


            StringBuilder sb = new StringBuilder();

            token.Claims.ToList().ForEach(a =>
            {
                sb.Append($"{a.Type} :: {a.Value}");
                sb.Append(System.Environment.NewLine);
            });

            var claimString = sb.ToString();
            string[] nameparts = nameHeader.Split('@');
            var name = nameparts[0];

            return new OkObjectResult($"Hello, {name}.{System.Environment.NewLine}{System.Environment.NewLine}CLAIMS:{System.Environment.NewLine}{claimString}");
        }
    }
}
