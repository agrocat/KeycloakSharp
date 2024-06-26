﻿using KeycloakIntegration;
using KeycloakIntegration.Classes;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakTester.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly AuthService auth;

        public AuthController(IConfiguration configuration)
        {
            // Get Keycloak configuration from appsettings.json
            var kcconfig = configuration.GetSection("Keycloak").Get<KeycloakConfig>();
            auth = new(kcconfig);
        }

        [HttpPost("")]
        public ActionResult<AuthResponse> Post([FromForm] string grant_type,
                                               [FromForm] string? username,
                                               [FromForm] string? password,
                                               [FromForm] string? refresh_token)
        {
            var authReq = new AuthRequest()
            {
                GrantType = grant_type,
                Username = username,
                Password = password,
                RefreshToken = refresh_token
            };

            try
            {
                var resp = auth.Auth(authReq);
                return Ok(resp);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try { return Ok(auth.Login(request)); }
            catch(Exception) { return StatusCode(500, "Error ocurred logging in"); }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            try { return Ok(auth.Refresh(request)); }
            catch (Exception) { return StatusCode(500, "Error ocurred refreshing token"); }
        }

    }
}

