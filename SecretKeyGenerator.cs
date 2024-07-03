using System;
using System.Security.Cryptography;

public class SecretKeyGenerator
{
	public static string GenerateSecretKey()
	{
		using (var rng = new RNGCryptoServiceProvider())
		{
			var key = new byte[32]; // 256-bit key
			rng.GetBytes(key);
			return Convert.ToBase64String(key);
		}
	}
}