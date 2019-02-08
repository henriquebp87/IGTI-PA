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
    public class OccurrenceController : BaseController
    {
        private readonly IOptions<DynamicsOptions> _instance;
        private readonly IOccurrenceService _occurrenceService;

        public OccurrenceController(IDynamicsTokenService dynamicsTokenService, IOccurrenceService occurrenceService, IOptions<DynamicsOptions> instance, IHttpClientFactory httpClientFactory)
            : base(dynamicsTokenService, httpClientFactory)
        {
            _occurrenceService = occurrenceService;
            _instance = instance;
        }

        /// <summary>
        /// Lista todas ocorrências.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await BearerToken();
            var ocurrence = await _occurrenceService.GetAllAsync(_httpClient, _instance.Value.Instance);
            return Ok(ocurrence);
        }

        /// <summary>
        /// Lista ocorrência pelo incidentId.
        /// </summary>
        /// <returns></returns>
        [HttpGet("IncidentId/{incidentId}")]
        public async Task<IActionResult> GetByOccurrenceId(string incidentId)
        {
            await BearerToken();
            var ocurrence = await _occurrenceService.GetByIncidentIdAsync(_httpClient, incidentId, _instance.Value.Instance);
            return Ok(ocurrence);
        }

        /// <summary>
        /// Lista todas as ocorrências de um solicitante.
        /// </summary>
        /// <param name="primarycontactid"></param>
        /// <returns></returns>
        [HttpGet("PrimaryContactId/{primarycontactid}")]
        public async Task<IActionResult> GetByPrimaryContactId(string primarycontactid)
        {
            await BearerToken();

            var ocurrence = await _occurrenceService.GetByPrimaryContactIdAsync(_httpClient, primarycontactid, _instance.Value.Instance);

            return Ok(ocurrence);
        }

        /// <summary>
        /// Pesquisa as ocorrências conforme os parâmetros de filtro, ordenação e paginação.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("Search")]
        public async Task<IActionResult> GetSearch([FromQuery] OccurrenceSearchRequestViewModel model)
        {
            await BearerToken();

            var occurrences = await _occurrenceService.GetSearchAsync(_httpClient, model, _instance.Value.Instance);

            return Ok(occurrences);
        }

        /// <summary>
        /// Lista os impactos de ocorrências.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Impacts")]
        public async Task<IActionResult> GetImpacts()
        {
            await BearerToken();

            var impacts = await _occurrenceService.GetImpactsAsync(_httpClient, _instance.Value.Instance);

            return Ok(impacts);
        }

        /// <summary>
        /// Lista os tipos de ocorrências.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Types")]
        public async Task<IActionResult> GetTypes()
        {
            await BearerToken();

            var types = await _occurrenceService.GetTypesAsync(_httpClient, _instance.Value.Instance);

            return Ok(types);
        }

        /// <summary>
        /// Lista as urgências de ocorrências.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Urgencies")]
        public async Task<IActionResult> GetUrgencies()
        {
            await BearerToken();

            var urgencies = await _occurrenceService.GetUrgenciesAsync(_httpClient, _instance.Value.Instance);

            return Ok(urgencies);
        }

        /// <summary>
        /// Lista os status de ocorrências.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Status")]
        public async Task<IActionResult> GetStatus()
        {
            await BearerToken();

            var status = _occurrenceService.GetStatus();

            return Ok(status);
        }

        /// <summary>
        /// Busca uma ocorrência por id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            await BearerToken();

            var occurrence = await _occurrenceService.GetByIdAsync(_httpClient, id, _instance.Value.Instance);

            return Ok(occurrence);
        }

        /// <summary>
        /// Lista ocorrência pelo ticketNumber.
        /// </summary>
        /// <returns></returns>
        [HttpGet("TicketNumber/{ticketnumber}")]
        public async Task<IActionResult> GetByTicketNumber(string ticketnumber)
        {
            await BearerToken();
            var ocurrence = await _occurrenceService.GetByTicketNumberAsync(_httpClient, ticketnumber, _instance.Value.Instance);
            return Ok(ocurrence);
        }

        /// <summary>
        /// Cadastra uma nova ocorrência.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OccurrenceRequestViewModel model)
        {
            await BearerToken();

            var createdOccurrence = await _occurrenceService.PostAsync(_httpClient, model, _instance.Value.Instance);

            return Ok(createdOccurrence);
        }

        [HttpPatch("{id}/Resolve")]
        public async Task<IActionResult> PatchResolveAsync([FromRoute] Guid id, [FromBody] OccurrenceResolveRequestViewModel model)
        {
            await BearerToken();

            await _occurrenceService.PatchResolveAsync(_httpClient, id, model, _instance.Value.Instance);

            return Ok();
        }
    }
}