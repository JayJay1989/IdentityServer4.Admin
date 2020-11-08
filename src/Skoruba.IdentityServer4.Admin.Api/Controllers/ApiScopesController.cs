﻿using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.Api.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Api.Dtos.ApiResources;
using Skoruba.IdentityServer4.Admin.Api.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Api.Mappers;
using Skoruba.IdentityServer4.Admin.Api.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class ApiScopesController : ControllerBase
    {
        private readonly IApiErrorResources _errorResources;
        private readonly IApiScopeService _apiScopeService;

        public ApiScopesController(IApiErrorResources errorResources, IApiScopeService apiScopeService)
        {
            _errorResources = errorResources;
            _apiScopeService = apiScopeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiScopesApiDto>> GetScopes(string search, int page = 1, int pageSize = 10)
        {
            var apiScopesDto = await _apiScopeService.GetApiScopesAsync(search, page, pageSize);
            var apiScopesApiDto = apiScopesDto.ToApiScopeApiModel<ApiScopesApiDto>();

            return Ok(apiScopesApiDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiScopeApiDto>> GetScope(int id)
        {
            var apiScopesDto = await _apiScopeService.GetApiScopeAsync(id);
            var apiScopeApiDto = apiScopesDto.ToApiScopeApiModel<ApiScopeApiDto>();

            return Ok(apiScopeApiDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostScope([FromBody]ApiScopeApiDto apiScopeApi)
        {
            var apiScope = apiScopeApi.ToApiScopeApiModel<ApiScopeDto>();
            
            if (!apiScope.Id.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var id = await _apiScopeService.AddApiScopeAsync(apiScope);
            apiScope.Id = id;

            return CreatedAtAction(nameof(GetScope), new {scopeId = id}, apiScope);
        }

        [HttpPut]
        public async Task<IActionResult> PutScope([FromBody]ApiScopeApiDto apiScopeApi)
        {
            var apiScope = apiScopeApi.ToApiScopeApiModel<ApiScopeDto>();
            
            await _apiScopeService.GetApiScopeAsync(apiScope.Id);

            await _apiScopeService.UpdateApiScopeAsync(apiScope);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScope(int id)
        {
            var apiScope = new ApiScopeDto { Id = id };

            await _apiScopeService.GetApiScopeAsync(apiScope.Id);

            await _apiScopeService.DeleteApiScopeAsync(apiScope);

            return Ok();
        }
    }
}