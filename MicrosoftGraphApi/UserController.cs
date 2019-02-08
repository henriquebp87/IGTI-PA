using AutoMapper;
using KeyVault.Service.Definition;
using MicrosoftGraphApi.Application.DTO;
using MicrosoftGraphApi.Application.Services.Definition;
using MicrosoftGraphApi.Application.ViewModels;
using MicrosoftGraphApi.Controllers.Base;
using MicrosoftGraphApi.Infra.Token.Services.Definition;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicrosoftGraphApi.Controllers
{
    [Route("api/graph/[controller]")]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUserService _azureAdUserService;
        private readonly IDomainService _domainService;
        private readonly IGroupService _groupService;

        public UserController(IGraphTokenService graphTokenService, IUserService azureAdUserService, IHttpClientFactory httpClientFactory, IMapper mapper, IGroupService groupService, ISecretService secretService, IDomainService domainService)
            : base(graphTokenService, httpClientFactory, secretService)
        {
            _azureAdUserService = azureAdUserService;
            _domainService = domainService;
            _mapper = mapper;
        }

        #region HTTP Get

        [HttpGet("List")]
        public async Task<IActionResult> List([FromQuery]ListUserRequestViewModel model)
        {
            BearerToken(model.TenantId.ToString());
            var users = await _azureAdUserService.ListAsync(_httpClient, model.TenantId, model.AccountNumber, model.PageSize, model.SkipToken, model.Filter);
            var response = _mapper.Map<UsersResponse, ListUserResponseViewModel>(users);
            return Ok(response);
        }

        [HttpGet("ByUPN")]
        public async Task<IActionResult> Get(Guid tenantId, string userPrincipalName)
        {
            BearerToken(tenantId.ToString());
            var user = await _azureAdUserService.GetAsync(_httpClient, userPrincipalName);
            return Ok(user);
        }

        [HttpGet("ByUPN/{userPrincipalName}/beta")]
        public async Task<IActionResult> GetByUpnBeta([Required][FromQuery] Guid tenantId, [FromRoute] string userPrincipalName)
        {
            BearerToken(tenantId.ToString());
            var user = await _azureAdUserService.GetByUpnBetaAsync(_httpClient, userPrincipalName);
            return Ok(user);
        }

        [HttpGet("ByEmail")]
        public async Task<IActionResult> GetByEmail(Guid tenantId, string email)
        {
            BearerToken(tenantId.ToString());
            var user = await _azureAdUserService.GetByEmailAsync(_httpClient, email);
            return Ok(user);
        }

        [HttpGet("UpdateExtension")]
        public async Task<IActionResult> UpdateExtension()
        {
            //BearerToken();
            var users = await _azureAdUserService.UpdateExtensionAsync(_httpClient);
            return Ok(users);
        }

        //'48x48', '64x64', '96x96', '120x120', '240x240', '360x360','432x432', '504x504', and '648x648'.
        [HttpGet("GetPhoto")]
        public async Task<IActionResult> GetPhoto(Guid tenantId, string userPrincipalName, string size = "48x48")
        {
            BearerToken(tenantId.ToString());
            var photo = await _azureAdUserService.GetPhotoAsync(_httpClient, userPrincipalName, size);
            return Ok(photo);
        }

        #endregion

        #region HTTP Patch

        [HttpPatch("UpdateUserExtension/{tenantId}")]
        public async Task<IActionResult> UpdateUserExtension(Guid tenantId, [FromBody] IEnumerable<UpdateUserExtensionViewModel> users)
        {
            BearerToken(tenantId.ToString());
            foreach (var item in users)
            {
                await _azureAdUserService.UpdateUserExtensionAsync(_httpClient, item);
            }
            return Ok();
        }

        [HttpPatch("UpdatePassword/{tenantId}/{userPrincipalName}")]
        public async Task<IActionResult> UpdatePassword(Guid tenantId, string userPrincipalName, [FromBody] UpdateUserPasswordModel updateUsermodel)
        {
            BearerToken(tenantId.ToString());
            var model = _mapper.Map<UpdateUserModel>(updateUsermodel);
            await _azureAdUserService.UpdateAsync(_httpClient, userPrincipalName, model);
            return Ok();
        }

        [HttpPatch("UpdateFields/{tenantId}/{userPrincipalName}")]
        public async Task<IActionResult> UpdateFields(Guid tenantId, string userPrincipalName, [FromBody] UpdateUserModel updateUsermodel)
        {
            BearerToken(tenantId.ToString());
            await _azureAdUserService.UpdateAsync(_httpClient, userPrincipalName, updateUsermodel);
            return Ok();
        }

        [HttpPatch("Disable/ByUPN/{userPrincipalName}")]
        public async Task<IActionResult> DisableByUpn([Required][FromQuery] Guid tenantId, [FromRoute] string userPrincipalName)
        {
            BearerToken(tenantId.ToString());

            await _azureAdUserService.DisableByUpnAsync(_httpClient, userPrincipalName);

            return Ok();
        }

        #endregion

        #region HTTP Post

        [HttpPost("Create/{tenantId}")]
        public async Task<IActionResult> Create(Guid tenantId, [FromBody] CreateUserModel createUserModel)
        {
            BearerToken(tenantId.ToString());
            await _azureAdUserService.CreateAsync(_httpClient, createUserModel);
            return Ok();
        }

        #endregion

        #region HTTP Delete

        [HttpDelete("ByUPN/{userPrincipalName}")]
        public async Task<IActionResult> DeleteByUpn([Required][FromQuery] Guid tenantId, [FromRoute] string userPrincipalName)
        {
            BearerToken(tenantId.ToString());

            await _azureAdUserService.DeleteByUpnAsync(_httpClient, userPrincipalName);

            return Ok();
        }

        #endregion
    }
}