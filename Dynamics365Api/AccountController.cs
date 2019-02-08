using Dynamics365Api.Application.Interfaces;
using Dynamics365Api.Controllers.Base;
using Dynamics365Api.Infra.Token.Interfaces;
using Dynamics365Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dynamics365Api.Controllers
{
    [Route("api/dynamics/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IOptions<DynamicsOptions> _instance;
        private readonly IAccountService _accountService;

        public AccountController(IDynamicsTokenService dynamicsTokenService, IAccountService accountService, IOptions<DynamicsOptions> instance, IHttpClientFactory httpClientFactory)
            : base(dynamicsTokenService, httpClientFactory)
        {
            _accountService = accountService;
            _instance = instance;
        }

        /// <summary>
        /// Lista todos os clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(bool onlyEnabled = true)
        {
            await BearerToken();
            var account = await _accountService.GetAll(_httpClient, _instance.Value.Instance, onlyEnabled);
            return Ok(account);
        }

        /// <summary>
        /// Lista cliente pelo accountId.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountId/{accountid}")]
        public async Task<IActionResult> GetByAccountId(string accountid, bool onlyEnabled = true)
        {
            await BearerToken();
            var account = await _accountService.GetByAccountIdAsync(_httpClient, accountid, _instance.Value.Instance, onlyEnabled);
            return Ok(account);
        }

        /// <summary>
        /// Lista cliente pelo accountnumber.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountNumber/{accountnumber}")]
        public async Task<IActionResult> GetByAccountNumber(string accountnumber, bool onlyEnabled = true)
        {
            await BearerToken();
            var account = await _accountService.GetByAccountNumberAsync(_httpClient, accountnumber, _instance.Value.Instance, onlyEnabled);
            return Ok(account);
        }
    }
}