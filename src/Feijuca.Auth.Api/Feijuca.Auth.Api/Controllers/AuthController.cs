﻿using Application.Commands.Auth;
using Application.Requests.Auth;
using Application.Responses;
using Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Authenticate a user and return a valid JWT token and details.
        /// </summary>
        /// <returns>A status code related to the operation.</returns>
        [HttpPost]
        [Route("{tenant}/auth/login", Name = nameof(Login))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromRoute] string tenant, [FromBody] LoginUserRequest loginUserRequest, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LoginCommand(tenant, loginUserRequest), cancellationToken);

            if (result.IsSuccess)
            {
                var response = Result<TokenDetailsResponse>.Success(result.Response);
                return Ok(response.Response); // Retorna o token e os detalhes
            }

            var responseError = Result<TokenDetailsResponse>.Failure(result.Error);
            return BadRequest(responseError);
        }

        /// <summary>
        /// Logout a user and invalidate the session token.
        /// </summary>
        /// <returns>A status code related to the operation.</returns>
        [HttpPost]
        [Route("{tenant}/auth/logout", Name = nameof(Logout))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout([FromRoute] string tenant, [FromBody] RefreshTokenRequest logoutUserRequest, CancellationToken cancellationToken)
        {
            // Chame um método para realizar o logout no Keycloak
            var result = await _mediator.Send(new SignoutCommand(tenant, logoutUserRequest.RefreshToken), cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { Message = "Logout successful" });
            }

            var responseError = Result<string>.Failure(result.Error);
            return BadRequest(responseError);
        }

        /// <summary>
        /// Return a valid JWT token and details about them refreshed.
        /// </summary>
        /// <returns>A status code related to the operation.</returns>
        [HttpPut]
        [Route("{tenant}/auth/tokens/refresh", Name = nameof(RefreshToken))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromRoute] string tenant, [FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(tenant, request.RefreshToken), cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Response); // Retorna o token atualizado
            }

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Return a valid JWT token and details about them refreshed.
        /// </summary>
        /// <returns>A status code related to the operation.</returns>
        [HttpGet]
        [Route("/auth/decode", Name = nameof(DecodeToken))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult DecodeToken()
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return BadRequest();
            }


            var usuarioId = identity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var username = identity.FindFirst("preferred_username")!.Value;
            var email = identity.FindFirst(ClaimTypes.Email)!.Value;
            var fullName = identity.FindFirst("name")!.Value;

            var nameParts = fullName.Split(' ');
            var firstName = nameParts.FirstOrDefault();
            var lastName = nameParts.Length > 1 ? nameParts[^1] : null;

            var userResponse = new UserResponse(Guid.Parse(usuarioId), username, email, firstName!, lastName!);
            return Ok(userResponse);
        }
    }
}
