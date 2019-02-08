using Dynamics365Api.Application.Interfaces;
using Dynamics365Api.Application.ViewModels;
using Dynamics365Api.Controllers.Base;
using Dynamics365Api.Infra.Token.Interfaces;
using Dynamics365Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dynamics365Api.Controllers
{
    [Route("api/dynamics/[controller]")]
    public class EntitlementContactRoleController : BaseController
    {
        private readonly IOptions<DynamicsOptions> _instance;
        private readonly IEntitlementContactRoleService _entitlementContactRoleService;
        private readonly IContactService _contactService;

        public EntitlementContactRoleController(IContactService contactService, IEntitlementContactRoleService entitlementContactRoleService, IDynamicsTokenService dynamicsTokenService, IOptions<DynamicsOptions> instance, IHttpClientFactory httpClientFactory)
            : base(dynamicsTokenService, httpClientFactory)
        {
            _entitlementContactRoleService = entitlementContactRoleService;
            _instance = instance;
            _contactService = contactService;
        }

        /// <summary>
        /// Lista todos perfil de acesso de um contato pelo contactid.
        /// </summary>
        /// <returns></returns>
        [HttpGet("ContactId/{contactid}")]
        public async Task<IActionResult> GetByContactId(string contactid, bool onlyEnabled = true)
        {
            await BearerToken();
            var entitlementContactRole = await _entitlementContactRoleService.GetByContactIdAsync(_httpClient, contactid, _instance.Value.Instance, onlyEnabled);
            return Ok(entitlementContactRole);
        }

        /// <summary>
        /// Lista todos perfil de acesso de um contato pelo contactid.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Mail/{mail}")]
        public async Task<IActionResult> GetByContactMail(string mail, bool onlyEnabled = true)
        {
            await BearerToken();
            var contact = await _contactService.GetByMailAsync(_httpClient, mail, _instance.Value.Instance, onlyEnabled);
            if (contact == null)
            {
                return Ok(contact);
            }
            else
            {
                var entitlementContactRole = await _entitlementContactRoleService.GetByContactIdAsync(_httpClient, contact.contactid.ToString(), _instance.Value.Instance, onlyEnabled);
                return Ok(entitlementContactRole);
            }
        }

        [HttpGet("ByContactMail/{mail}")]
        public async Task<IActionResult> GetByContactMailAddress(string mail)
        {
            await BearerToken();

            var contactId = await _contactService.GetIdByMailAsync(_httpClient, mail, _instance.Value.Instance);
            var entitlementContactRoles = await _entitlementContactRoleService.GetByContactIdAsync(_httpClient, contactId, _instance.Value.Instance);

            return Ok(entitlementContactRoles);
        }

        /// <summary>
        /// Lista todos perfil de acesso de uma empresa pelo entitlementid.
        /// </summary>
        /// <returns></returns>
        [HttpGet("EntitlementId/{entitlementid}")]
        public async Task<IActionResult> GetByEntitlementId(string entitlementid, bool onlyEnabled = true)
        {
            await BearerToken();
            var entitlementContactRole = await _entitlementContactRoleService.GetByEntitlementIdAsync(_httpClient, entitlementid, _instance.Value.Instance, onlyEnabled);
            return Ok(entitlementContactRole);
        }

        [HttpGet("IsContractManagerContact/{contactId}")]
        public async Task<IActionResult> GetIsContractManagerContact(Guid contactId)
        {
            await BearerToken();

            var isContractManagerContact = await _entitlementContactRoleService.GetIsContractManagerContactAsync(_httpClient, contactId, _instance.Value.Instance);

            return Ok(isContractManagerContact);
        }

        [HttpPut("Replace/{contactId}")]
        public async Task<IActionResult> Replace(Guid contactId, [FromBody] ContactAssociateAccessProfilesRequestViewModel model)
        {
            await BearerToken();

            var result = await _entitlementContactRoleService.ReplaceAsync(_httpClient, contactId, model, _instance.Value.Instance);

            return Ok(result);
        }

        [HttpPut("Insert/{contactId}")]
        public async Task<IActionResult> Insert(Guid contactId, [FromBody] ContactAssociateAccessProfilesRequestViewModel model)
        {
            await BearerToken();
            foreach (var item in model.PerfisAcesso)
            {
                var entitlementContactRole = await _entitlementContactRoleService.GetByContactIdAndEntitlementIdAsync(_httpClient, contactId.ToString(), item._axt_entitlementid_value.ToString(), _instance.Value.Instance, true);
                if (entitlementContactRole == null)
                {
                    await _entitlementContactRoleService.InsertAsync(_httpClient, contactId, item, _instance.Value.InstanceApi);
                }
            }
            return Ok();
        }
    }
}