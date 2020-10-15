using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace InstallTemplateApp
{
    public static class InstallAppFunction
    {
        private static IConfiguration Configuration { set; get; }

        static InstallAppFunction()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));
            Configuration = builder.Build();
        }

        [FunctionName("install")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var appId = Configuration[Constants.AppIdConfigurationKey];
            var packageKey = Configuration[Constants.PackageKeyConfigurationKey];
            var ownerTenantId = Configuration[Constants.OwnerIdConfigurationKey];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var config = JsonConvert.DeserializeObject<IDictionary<string, string>>(requestBody);

            var appInstallRequest = new CreateInstallTicketRequest
            {
                InstallDetails = new List<TemplateAppInstallDetails>
                {
                    new TemplateAppInstallDetails
                    {
                        AppId = new Guid(appId),
                        PackageKey = packageKey,
                        OwnerTenantId = new Guid(ownerTenantId),
                        Config = new TemplateAppConfigurationRequest
                        {
                            Configuration = config
                        }
                    }
                }
            };

            InstallTicket ticket = null;
            var credentials = await SignIn(
                authorityUrl: string.Format(Constants.PowerBIApiAuthorityUrlFormat, ownerTenantId),
                resourceUrl: Constants.PowerBIApiResourceUrl,
                clientId: Configuration[Constants.ClientIdConfigurationKey],
                clientSecret: Configuration[Constants.ClientSecretConfigurationKey]);

            var endpointUrl = Constants.PowerBIApiUrl;
            using (var client = new PowerBIClient(new Uri(endpointUrl), credentials))
            {
                ticket = await client.TemplateApps.CreateInstallTicketAsync(appInstallRequest);
            }

            var baseUrl = Constants.PowerBIApiBaseUrl;
            var redirectUrl = string.Format(Constants.RedirectUrlFormat, baseUrl, appId, packageKey.Replace("=", string.Empty), ownerTenantId);

            return new ContentResult() { Content = RedirectWithData(redirectUrl, ticket.Ticket), ContentType = "text/html" };
        }

        public static string RedirectWithData(string url, string ticket)
        {
            StringBuilder s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<form name='form' action='{0}' method='post' enctype='application/json'>", url);
            s.AppendFormat("<input type='hidden' name='ticket' value='{0}' />", ticket);
            s.Append("</form></body></html>");
            return s.ToString();
        }

        public static async Task<TokenCredentials> SignIn(string authorityUrl, string clientId, string resourceUrl, string clientSecret)
        {
            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);

            // Authenticate using created credentials
            var authenticationContext = new AuthenticationContext(authorityUrl, false);
            var authenticationResult = await authenticationContext.AcquireTokenAsync(resourceUrl, clientCredential).ConfigureAwait(false);
            var credentials = new TokenCredentials(authenticationResult.AccessToken);

            return credentials;
        }
    }
}
