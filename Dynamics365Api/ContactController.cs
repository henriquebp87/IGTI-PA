using Dynamics365Api.Application.Interfaces;
using Dynamics365Api.Controllers.Base;
using Dynamics365Api.Domain.Entities;
using Dynamics365Api.Infra.Token.Interfaces;
using Dynamics365Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dynamics365Api.Controllers
{
    [Route("api/dynamics/[controller]")]
    public class ContactController : BaseController
    {
        private readonly IOptions<DynamicsOptions> _instance;
        private readonly IContactService _contactService;

        public ContactController(IDynamicsTokenService dynamicsTokenService, IContactService contactService, IOptions<DynamicsOptions> instance, IHttpClientFactory httpClientFactory)
            : base(dynamicsTokenService, httpClientFactory)
        {
            _contactService = contactService;
            _instance = instance;
        }

        /// <summary>
        /// Lista todos os contados.
        /// </summary>
        /// <returns> 200 TESTE</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(bool onlyEnabled = true)
        {
            await BearerToken();
            var contact = await _contactService.GetAll(_httpClient, _instance.Value.Instance, onlyEnabled);
            return Ok(contact);
        }

        /// <summary>
        /// Lista contato pelo contactId.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ContactId/{contactid}")]
        public async Task<IActionResult> GetByContactId(string contactid, bool onlyEnabled = true)
        {
            await BearerToken();
            var contact = await _contactService.GetByContactIdAsync(_httpClient, contactid, _instance.Value.Instance, onlyEnabled);
            return Ok(contact);
        }

        /// <summary>
        /// Lista contato pelo mail.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Mail/{mail}")]
        public async Task<IActionResult> GetByMail(string mail, bool onlyEnabled = true)
        {
            await BearerToken();
            var contact = await _contactService.GetByMailAsync(_httpClient, mail, _instance.Value.Instance, onlyEnabled);
            return Ok(contact);
        }

        /// <summary>
        /// Criar contato no Dynamics365.
        /// </summary>
        /// <returns>
        /// Retorna o valor de acordo com o Id informado
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] NewDynamicsContact model)
        {
            await BearerToken();
            var retorno = await _contactService.GetByMailAsync(_httpClient, model.emailaddress1, _instance.Value.Instance, true);
            if(retorno == null)
            {
                var contact = await _contactService.CreateContactAsync(_httpClient, model, _instance.Value.InstanceApi);
                retorno = await _contactService.GetByMailAsync(_httpClient, model.emailaddress1, _instance.Value.Instance, true);
                return Ok(retorno);
            }
            return Ok(retorno);
        }
    }
}