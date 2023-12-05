using System;
namespace KeycloakIntegration
{
	public class AuthException : Exception
	{
		public AuthException() { }
		public AuthException(string msg, Exception? innerException) : base(msg, innerException) { }
	}
}

