using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sdl.LC.AddonBlueprint.Interfaces;
using System;
using System.Buffers;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sdl.LC.AddonBlueprint.Infrastructure
{
    public class AddonAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private readonly ITenantProvider _tenantProvider;

		public AddonAuthenticationHandler(
		   IOptionsMonitor<AuthenticationSchemeOptions> options,
		   ILoggerFactory logger,
		   UrlEncoder encoder,
		   ISystemClock clock,
		   ITenantProvider tenantProvider)
		   : base(options, logger, encoder, clock)
		{
			_tenantProvider = tenantProvider;
		}

		/// <summary>
		/// Handles the authentication.
		/// </summary>
		/// <returns>An authenticate result.</returns>
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var signatureHeader = Request.Headers["X-LC-Signature"].SingleOrDefault();
			var algoHeader = Request.Headers["X-LC-Signature-Algo"].SingleOrDefault();
			var timeHeader = Request.Headers["X-LC-Transmission-Time"].SingleOrDefault();
			var tenantHeader = Request.Headers["X-LC-Tenant"].SingleOrDefault();

			var tenant = await _tenantProvider.GetTenantSecret(tenantHeader);
			if (tenant == null || string.IsNullOrEmpty(tenant.TenantId) || string.IsNullOrEmpty(tenant.PublicKey))
			{
				return AuthenticateResult.Fail("Could not authenticate.");
			}

			var path = Request.Path.Value;
			var contents = await Request.BodyReader.ReadAsync();
			// Reset the request body stream position
			Request.Body.Position = 0;
			var isVerifiedSignature = ValidateSignature(timeHeader, tenantHeader, path, algoHeader, signatureHeader, tenant.PublicKey, contents.Buffer.ToArray());
			if (!isVerifiedSignature)
			{
				return AuthenticateResult.Fail("Could not authenticate.");
			}

			var claims = new[] {
				new Claim("X-LC-Tenant", tenantHeader)
			};
			var identity = new ClaimsIdentity(claims, Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);

			return AuthenticateResult.Success(ticket);
		}

		/// <summary>
		/// Returns the CRC for the given array of bytes.
		/// </summary>
		/// <param name="array">The byte array.</param>
		/// <returns>The CRC for the given byte array.</returns>
		private long CalculateCrc(byte[] array)
		{
			string srchash = string.Empty;
			using (Crc32 crcCalc = new Crc32())
			{
				foreach (byte b in crcCalc.ComputeHash(array))
				{
					srchash += b.ToString("x2", CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture);
				}
			}
			return long.Parse(srchash, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Validates the signature.
		/// </summary>
		/// <param name="transmissionTime">The transmission time.</param>
		/// <param name="tenantId">The tenant id.</param>
		/// <param name="addOnPath">The addon path.</param>
		/// <param name="algoName">The algorithm name.</param>
		/// <param name="signatureHeader">The signature header.</param>
		/// <param name="publicKey">The public key.</param>
		/// <param name="payloadBytes">The payload.</param>
		/// <returns>True if verified.</returns>
		private bool ValidateSignature(string transmissionTime, string tenantId, string addOnPath, string algoName, string signatureHeader, string publicKey, byte[] payloadBytes)
		{
			var crc32Hash = CalculateCrc(payloadBytes);

			//sign tenantid
			string signInfo = transmissionTime + "|" + tenantId + "|" + addOnPath + "|" + crc32Hash;

			byte[] certBytes = Convert.FromBase64String(publicKey);
			byte[] signature = Convert.FromBase64String(signatureHeader);

			RSA rsa = RSA.Create();

			rsa.ImportSubjectPublicKeyInfo(certBytes, out _);

			return rsa.VerifyData(Encoding.ASCII.GetBytes(signInfo), signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
		}     

    }
}
