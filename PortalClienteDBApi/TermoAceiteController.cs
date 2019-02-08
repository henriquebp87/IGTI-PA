using PortalClienteDBApi.Application.Interfaces;
using PortalClienteDBApi.Application.Interfaces.Base;
using PortalClienteDBApi.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PortalClienteDBApi.Controllers
{
    [ApiController]
    [Route("api/portaldb/[controller]")]
    public class TermoAceiteController : Controller
    {
        private readonly ITermoAceiteService _termoAceiteService;
        private readonly ICrudService<TermoAceiteContatoViewModel> _termoAceiteContatoService;

        public TermoAceiteController(ITermoAceiteService termoAceiteService, ICrudService<TermoAceiteContatoViewModel> termoAceiteContatoService)
        {
            _termoAceiteService = termoAceiteService;
            _termoAceiteContatoService = termoAceiteContatoService;
        }

        /// <summary>
        /// Lista todos os termos de aceite cadastrados.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = await _termoAceiteService.GetAllAsync();
            return Ok(lista);
        }

        /// <summary>
        /// Busca um termo de aceite pelo id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var termo = await _termoAceiteService.GetByIdAsync(id);
            if (termo == null)
                return NotFound();

            return Ok(termo);
        }

        /// <summary>
        /// Pesquisa os termos de aceite conforme os parâmetros de filtro.
        /// </summary>
        /// <param name="model">Parâmetros de filtro.</param>
        /// <returns></returns>
        [HttpGet("Pesquisar")]
        public async Task<IActionResult> Get([FromQuery]TermoAceiteSearchViewModel model)
        {
            var termosAceite = await _termoAceiteService.SearchAsync(model);
            return Ok(termosAceite);
        }

        /// <summary>
        /// Lista os termos de aceite cadastrados por paginação
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("ListarPaginado")]
        public async Task<IActionResult> Get(int pageSize, int pageNumber)
        {
            var lista = await _termoAceiteService.GetAllByPaginationAsync(pageSize, pageNumber);
            return Ok(lista);
        }

        /// <summary>
        /// Lista os termos de aceite que o usuário (contato) ainda não aceitou, de acordo com o cliente, linha de serviço e perfil.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ListarTermosParaAceite")]
        public async Task<IActionResult> Post([FromBody]ListarTermosParaAceiteViewModel model)
        {
            var listaTermosParaAceite = await _termoAceiteService.GetTermosParaAceite(model.EntitlementsList, model.IdContact.Value);
            return Ok(listaTermosParaAceite);
        }

        /// <summary>
        /// Cria um termo de aceite
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TermoAceiteViewModel model)
        {
            model.Guid = Guid.NewGuid();
            await _termoAceiteService.CreateAsync(model);
            return Ok(model);
        }

        /// <summary>
        /// Ação quando o usuário aceita um termo de aceite
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Aceitar")]
        public async Task<IActionResult> Post([FromBody]TermoAceiteContatoViewModel model)
        {
            await _termoAceiteContatoService.CreateAsync(model);
            return Ok(model);
        }

        /// <summary>
        /// Editar termo de aceite
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]TermoAceiteViewModel model)
        {
            model.Id = id;
            await _termoAceiteService.EditAsync(model);
            return NoContent();
        }

        /// <summary>
        /// Exclusão (lógica) de um termo de aceite
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _termoAceiteService.DeleteAsync(id);
            return NoContent();
        }
    }
}