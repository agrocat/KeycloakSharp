using KeycloakIntegration.Classes;
using RestSharp;
using System.Security.Cryptography;

namespace KeycloakIntegration
{
	public class AuthService
	{
        private readonly KeycloakConfig kcconfig;

        public AuthService(KeycloakConfig config) {
            kcconfig = config;
		}

        public AuthResponse Login(LoginRequest request)
        {
            try { return Auth(request); }
            catch (AuthException ex) { throw ex; }
        }

        public AuthResponse Refresh(RefreshRequest request)
        {
            try { return Auth(request); }
            catch (AuthException) { throw; }
        }

		public AuthResponse Auth(AuthRequest request)
		{
			var client = new RestClient();
			var httpReq = new RestRequest(kcconfig.TokenURL)
			{
				Method = Method.Post
			};

            httpReq.AddParameter("client_id", kcconfig.ClientID);
            httpReq.AddParameter("client_secret", kcconfig.ClientSecret);
            httpReq.AddParameter("grant_type", request.GrantType);
            httpReq.AddParameter("scope", request.Scope);

            if (request.Username != null && request.Password != null)
            {
                httpReq.AddParameter("username", request.Username);
                httpReq.AddParameter("password", request.Password);
            }

            if (request.RefreshToken != null)
                httpReq.AddParameter("refresh_token", request.RefreshToken);

            var resp = client.Execute<AuthResponse>(httpReq);

            var kcResp = resp.Data ?? new();
            kcResp.IsSuccessful = resp.IsSuccessful;

            if (!kcResp.IsSuccessful)
            {
                var msg = "A problem ocurred with Keycloak";
#if DEBUG
                msg += $":\n{resp.Content}";
#endif
                throw new AuthException(msg, resp.ErrorException);
            }

            return kcResp;

        }

    }
}

