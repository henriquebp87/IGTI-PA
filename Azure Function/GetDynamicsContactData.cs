using Dynamics365Api.Application.Interfaces;
using Dynamics365Api.Functions.DependencySetup.Injection;
using Dynamics365Api.Functions.Models;
using Dynamics365Api.Functions.Options;
using Dynamics365Api.Infra.Token.Services;
using KeyVault.Service.Definition;
using KeyVault.Service.Implementation;
using Logging.Domain.Entities;
using Logging.Service.Formatter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AD = Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Dynamics365Api.Functions
{
    public static class GetDynamicsContactData
    {
        [FunctionName("GetDynamicsContactData")]
        public static async Task<IActionResult> Run(
                                        [HttpTrigger(AuthorizationLevel.Function, methods: "get")]
                                        HttpRequest req,
                                        [Inject] IContactService _contactService,
                                        [Inject] IEntitlementContactRoleService _entitlementContactRoleService,
                                        [Inject] IEntitlementService _entitlementService,
                                        [Inject] IAccountService _accountService,
                                        [Inject] IHttpClientFactory _httpClientFactory,
                                        [Inject] ISecretService _secretService,
                                        ILogger log, //default logger
                                        ExecutionContext context
            )
        {
            var onlyEnabled = true;
            var _httpClient = _httpClientFactory.CreateClient("Dynamics365");

            //Logging
            var storageAccountKey = _secretService.GetSecretAsync("PortalLogsStorage-StorageAccountKey").Result.Value;
            var storageAccountName = _secretService.GetSecretAsync("PortalLogsStorage-StorageAccountName").Result.Value;
            var storageAccountTableName = _secretService.GetSecretAsync("PortalLogs-TableName").Result.Value;
            var _customLogger = new Logging.Service.LogService(new LoggingOptions
            {
                LogLevel = LogLevel.Information,
                StorageAccountKey = storageAccountKey,
                StorageAccountName = storageAccountName,
                TableName = storageAccountTableName
            });

            try
            {
                var config = new ConfigurationBuilder()
                                .SetBasePath(context.FunctionAppDirectory)
                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();

                log.LogInformation("Iniciar function.");

                log.LogInformation("Pegar token.");
                var _dynamicsTokenService = ConfigureDynamicsTokenService(_httpClient, req, config["KeyVaultBaseUrl"], config["InstanceDynamics"], new Guid(config["KeyVaultClientId"]), config["KeyVaultClientSecret"]);
                var token = await _dynamicsTokenService.GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string email = req.Query["email"];

                log.LogInformation("Pegar contato.");
                Domain.Entities.DynamicsContact contact;
                dynamic jsonObject = new JObject();
                jsonObject.Add("ResultState", "Success");
                jsonObject.Add("ErrorMessage", "");

                try
                {
                    contact = await _contactService.GetByMailAsync(_httpClient, email, config["InstanceDynamics"], onlyEnabled);
                    jsonObject["IdContact"] = contact.contactid;
                }
                catch (KeyNotFoundException ex)
                {
                    //Armazena log no table storage
                    var logState = new LogState("AzureFunctionDynamics", Guid.NewGuid(), null);
                    _customLogger.LogError(LogFormatter.Format(logState, ex));

                    jsonObject["ResultState"] = "Failed";
                    jsonObject["ErrorMessage"] = "Usu�rio n�o cadastrado na organiza��o.";
                    return new NotFoundObjectResult(jsonObject);
                }

                if (contact._parentcustomerid_value.HasValue)
                {
                    log.LogInformation("Pegar account number e parent account id.");
                    try
                    {
                        var account = await _accountService.GetByAccountIdAsync(_httpClient, contact._parentcustomerid_value?.ToString(), config["InstanceDynamics"], onlyEnabled);
                        jsonObject["AccountNumber"] = account.accountnumber;
                        jsonObject["AccountId"] = account.accountid;
                        jsonObject["AccountName"] = account.name;
                        jsonObject["AccountCPNJ"] = account.axt_cnpj;
                        jsonObject["AccountCPNJView"] = account.axt_cnpjview;
                        jsonObject["AccountAxt_Razaosocial"] = account.axt_razaosocial;
                        if (account._parentaccountid_value.HasValue)
                        {
                            jsonObject["ParentAccountId"] = account._parentaccountid_value;
                        }
                        else
                        {
                            jsonObject["ParentAccountId"] = account.accountid;
                        }
                    }
                    catch (KeyNotFoundException ex)
                    {
                        //Armazena log no table storage
                        var logState = new LogState("AzureFunctionDynamics", Guid.NewGuid(), null);
                        _customLogger.LogError(LogFormatter.Format(logState, ex));

                        jsonObject["ResultState"] = "Failed";
                        jsonObject["ErrorMessage"] = "Organiza��o n�o encontrada.";
                        return new NotFoundObjectResult(jsonObject);
                    }
                }
                else
                {
                    jsonObject["AccountNumber"] = 
                    jsonObject["ParentAccountId"] = 
                    jsonObject["AccountId"] = 
                    jsonObject["AccountName"] = 
                    jsonObject["AccountCPNJ"] = 
                    jsonObject["AccountCPNJView"] = 
                    jsonObject["AccountAxt_Razaosocial"] = null;
                }

                log.LogInformation("Pegar entitlementcontactrole.");
                try
                {
                    var entitlementContactRoles = await _entitlementContactRoleService.GetByContactIdAsync(_httpClient, contact.contactid.ToString(), config["InstanceDynamics"], onlyEnabled);
                    var arrayResult = new List<ContactData>();

                    foreach (var entry in entitlementContactRoles)
                    {
                        try
                        {
                            var entitlement = await _entitlementService.GetByEntitlementIdAsync(_httpClient, entry._axt_entitlementid_value.ToString(), config["InstanceDynamics"], onlyEnabled);
                            var aux = new ContactData
                            {
                                IdAccount = entitlement._customerid_value,
                                IdServiceLine = entitlement._axt_servicelineid_value,
                                IdAxtRole = entry.axt_role,
                                IdEntitlement = entitlement.entitlementid,
                                AxtName = entry.axt_name
                            };
                            arrayResult.Add(aux);
                        }
                        catch (KeyNotFoundException)
                        {                            
                        }
                    }

                    jsonObject["Entries"] = JToken.FromObject(arrayResult);
                }
                catch (KeyNotFoundException ex)
                {
                    //Armazena log no table storage
                    var logState = new LogState("AzureFunctionDynamics", Guid.NewGuid(), null);
                    _customLogger.LogError(LogFormatter.Format(logState, ex));

                    jsonObject["ResultState"] = "Failed";
                    jsonObject["ErrorMessage"] = "Usu�rio n�o cont�m perfil de acesso.";
                    return new NotFoundObjectResult(jsonObject);
                }

                return new OkObjectResult(jsonObject);
            }
            catch (Exception ex)
            {
                //Armazena log no table storage
                var logState = new LogState("AzureFunctionDynamics", Guid.NewGuid(), null);
                _customLogger.LogError(LogFormatter.Format(logState, ex));

                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);
                return new BadRequestResult();
            }
        }

        private static DynamicsTokenService ConfigureDynamicsTokenService(HttpClient client, HttpRequest req, string keyVaultBaseUrl, string urlDynamics, Guid keyVaultClientId, string keyVaultClientSecret)
        {

            var _secretService = new SecretService(keyVaultBaseUrl, keyVaultClientId, keyVaultClientSecret);
            var clientId = _secretService.GetSecretAsync("Dynamics365-AppCredentialClientId").Result;
            var clientSecret = _secretService.GetSecretAsync("Dynamics365-AppCredentialClientSecret").Result;

            //Create Service Instance
            var ap = AD.AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(urlDynamics)).Result;
            var authContext = new AD.AuthenticationContext(ap.Authority);
            var clientCred = new AD.ClientCredential(clientId.Value, clientSecret.Value);
            return new DynamicsTokenService(authContext, clientCred, ap.Resource);
        }
    }
}