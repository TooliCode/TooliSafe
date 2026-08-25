using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TooliSafe.Security
{
    /// <summary>
    /// Represents an ECDH key pair consisting of standard Base64-encoded ASN.1 structures.
    /// </summary>
    public sealed class EcdhKeyPair
    {
        /// <summary>
        /// Gets the public key encoded as Base64 (SubjectPublicKeyInfo format).
        /// </summary>
        public string PublicKeyBase64 { get; }

        /// <summary>
        /// Gets the private key encoded as Base64 (PKCS#8 format).
        /// </summary>
        public string PrivateKeyBase64 { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcdhKeyPair"/> class.
        /// </summary>
        /// <param name="publicKeyBase64">The public key in Base64 (SubjectPublicKeyInfo format).</param>
        /// <param name="privateKeyBase64">The private key in Base64 (PKCS#8 format).</param>
        public EcdhKeyPair(string publicKeyBase64, string privateKeyBase64)
        {
            PublicKeyBase64 = publicKeyBase64 ?? throw new ArgumentNullException(nameof(publicKeyBase64));
            PrivateKeyBase64 = privateKeyBase64 ?? throw new ArgumentNullException(nameof(privateKeyBase64));
        }
    }

    /// <summary>
    /// Provides Elliptic-Curve Diffie-Hellman (ECDH) key exchange logic using NIST P-256 (secp256r1).
    /// Fully self-contained, platform-independent, and free of framework/app-specific dependencies.
    /// </summary>
    public static class ECDH
    {
        /// <summary>
        /// Marker prefix embedded inside key payloads to identify ECDH version 1 exchanges.
        /// </summary>
        public const string DH_MARKER = "DH1:";

        /// <summary>
        /// Outer prefix used by legacy and current transport formats for key auto-discovery.
        /// </summary>
        public const string KEY_EXCHANGE_PREFIX = "key: ";

        /// <summary>
        /// Generates a new ECDH key pair on the NIST P-256 (secp256r1) curve.
        /// </summary>
        /// <returns>An <see cref="EcdhKeyPair"/> containing the public (SPKI) and private (PKCS#8) keys as Base64 strings.</returns>
        public static EcdhKeyPair GenerateKeyPair()
        {
            using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
            
            byte[] publicKeyBytes = ecdh.ExportSubjectPublicKeyInfo();
            byte[] privateKeyBytes = ecdh.ExportPkcs8PrivateKey();

            string pubKeyBase64 = Convert.ToBase64String(publicKeyBytes);
            string privKeyBase64 = Convert.ToBase64String(privateKeyBytes);

            return new EcdhKeyPair(pubKeyBase64, privKeyBase64);
        }

        /// <summary>
        /// Computes the shared secret (derived via SHA-256) from a local private key and a peer's public key.
        /// </summary>
        /// <param name="privateKeyPkcs8Base64">The local private key in Base64 PKCS#8 format.</param>
        /// <param name="peerPublicKeySpkiBase64">The remote peer's public key in Base64 SubjectPublicKeyInfo format.</param>
        /// <returns>The derived 256-bit shared secret as a Base64 string.</returns>
        public static string DeriveSharedSecretBase64(string privateKeyPkcs8Base64, string peerPublicKeySpkiBase64)
        {
            byte[] rawSecret = DeriveSharedSecretBytes(
                Convert.FromBase64String(privateKeyPkcs8Base64),
                Convert.FromBase64String(peerPublicKeySpkiBase64)
            );

            return Convert.ToBase64String(rawSecret);
        }

        /// <summary>
        /// Computes the shared secret (derived via SHA-256) from raw byte arrays.
        /// </summary>
        /// <param name="privateKeyPkcs8">Local private key bytes in PKCS#8 format.</param>
        /// <param name="peerPublicKeySpki">Peer public key bytes in SubjectPublicKeyInfo format.</param>
        /// <returns>A 32-byte array containing the SHA-256 derived shared secret.</returns>
        public static byte[] DeriveSharedSecretBytes(byte[] privateKeyPkcs8, byte[] peerPublicKeySpki)
        {
            if (privateKeyPkcs8 == null || privateKeyPkcs8.Length == 0)
                throw new ArgumentException("Private key bytes cannot be null or empty.", nameof(privateKeyPkcs8));

            if (peerPublicKeySpki == null || peerPublicKeySpki.Length == 0)
                throw new ArgumentException("Peer public key bytes cannot be null or empty.", nameof(peerPublicKeySpki));

            using var localEcdh = ECDiffieHellman.Create();
            localEcdh.ImportPkcs8PrivateKey(privateKeyPkcs8, out _);

            using var peerEcdh = ECDiffieHellman.Create();
            peerEcdh.ImportSubjectPublicKeyInfo(peerPublicKeySpki, out _);

            // Derives a 256-bit key using SHA-256 hash reduction on the raw Diffie-Hellman shared point
            return localEcdh.DeriveKeyFromHash(peerEcdh.PublicKey, HashAlgorithmName.SHA256);
        }

        /// <summary>
        /// Formats an ECDH public key into a wire transport message payload.
        /// </summary>
        /// <param name="publicKeyBase64">Base64-encoded public key (SubjectPublicKeyInfo).</param>
        /// <param name="optionalMetadata">Optional metadata tag (e.g., Telegram Chat ID, User ID).</param>
        /// <returns>A formatted string ready for transport (e.g., "key: DH1:<Base64PubKey> TGID:<ID>").</returns>
        public static string CreateExchangePayload(string publicKeyBase64, string optionalMetadata = null)
        {
            if (string.IsNullOrWhiteSpace(publicKeyBase64))
                throw new ArgumentException("Public key cannot be null or empty.", nameof(publicKeyBase64));

            var sb = new StringBuilder();
            sb.Append(KEY_EXCHANGE_PREFIX);
            sb.Append(DH_MARKER);
            sb.Append(publicKeyBase64.Trim());

            if (!string.IsNullOrWhiteSpace(optionalMetadata))
            {
                sb.Append(" TGID:");
                sb.Append(optionalMetadata.Trim());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether a text message contains an ECDH version 1 exchange payload.
        /// </summary>
        /// <param name="messageText">The raw incoming message text.</param>
        /// <returns><c>true</c> if the message contains the ECDH marker; otherwise <c>false</c>.</returns>
        public static bool IsDhMessage(string messageText)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                return false;

            string trimmed = messageText.Trim();
            if (!trimmed.StartsWith(KEY_EXCHANGE_PREFIX, StringComparison.OrdinalIgnoreCase))
                return false;

            int colonIndex = trimmed.IndexOf(':');
            if (colonIndex < 0) return false;

            string payload = trimmed.Substring(colonIndex + 1).Trim();
            return payload.StartsWith(DH_MARKER, StringComparison.Ordinal);
        }

        /// <summary>
        /// Parses an incoming key exchange payload to extract the peer's public key and optional metadata.
        /// </summary>
        /// <param name="rawMessage">The received message text.</param>
        /// <param name="publicKeyBase64">Out parameter for the extracted Base64 public key (SPKI).</param>
        /// <param name="optionalMetadata">Out parameter for extracted metadata (e.g., Telegram Chat ID), or null if absent.</param>
        /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParseExchangePayload(string rawMessage, out string publicKeyBase64, out string optionalMetadata)
        {
            publicKeyBase64 = null;
            optionalMetadata = null;

            if (string.IsNullOrWhiteSpace(rawMessage))
                return false;

            string trimmed = rawMessage.Trim();
            if (!trimmed.StartsWith(KEY_EXCHANGE_PREFIX, StringComparison.OrdinalIgnoreCase))
                return false;

            int colonIndex = trimmed.IndexOf(':');
            if (colonIndex < 0) return false;

            string keyPart = trimmed.Substring(colonIndex + 1).Trim();
            if (!keyPart.StartsWith(DH_MARKER, StringComparison.Ordinal))
                return false;

            string rawAfterMarker = keyPart.Substring(DH_MARKER.Length);

            // Extract public key up to whitespace or newline
            int wsIndex = rawAfterMarker.IndexOfAny(new[] { '\r', '\n', ' ', '\t' });
            if (wsIndex >= 0)
            {
                publicKeyBase64 = rawAfterMarker.Substring(0, wsIndex).Trim();
                string trailing = rawAfterMarker.Substring(wsIndex).Trim();

                const string tgidMarker = "TGID:";
                int tgidIdx = trailing.IndexOf(tgidMarker, StringComparison.Ordinal);
                if (tgidIdx >= 0)
                {
                    string idPart = trailing.Substring(tgidIdx + tgidMarker.Length).Trim();
                    int idEnd = idPart.IndexOfAny(new[] { '\r', '\n', ' ', '\t' });
                    if (idEnd >= 0)
                        idPart = idPart.Substring(0, idEnd);

                    if (idPart.Length > 0 && idPart.All(char.IsDigit))
                    {
                        optionalMetadata = idPart;
                    }
                }
            }
            else
            {
                publicKeyBase64 = rawAfterMarker.Trim();
            }

            return !string.IsNullOrWhiteSpace(publicKeyBase64);
        }
    }
}
