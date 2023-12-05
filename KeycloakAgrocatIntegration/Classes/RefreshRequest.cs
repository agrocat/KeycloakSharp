using System;
namespace KeycloakIntegration.Classes
{
	public class RefreshRequest : AuthRequest
	{

		public new string? RefreshToken
		{
			get => base.RefreshToken;
			set => base.RefreshToken = value;
		}

		// Property created for json parsing issues
		public string? refresh_token { 
			get => RefreshToken; 
			set => RefreshToken = value; 
		}

		public RefreshRequest()
		{
			GrantType = "refresh_token";
		}

	}
}

