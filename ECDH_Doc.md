# ECDH Key Exchange Module (NIST P-256)

A standalone, zero-dependency C# implementation of an Elliptic-Curve Diffie-Hellman (ECDH) key exchange protocol using the **NIST P-256 (secp256r1)** curve.

This module extracts core cryptographic key generation, wire payload formatting/parsing, and shared secret derivation from application-specific code into a clean, platform-agnostic library.

---

## 🌟 Features

- **Zero Third-Party Dependencies:** Uses standard .NET Base Class Library (`System.Security.Cryptography.ECDiffieHellman`).
- **Cross-Platform Compatibility:** Runs seamlessly across .NET 6/7/8/9, MAUI, iOS, Android, Windows, macOS, and Linux.
- **NIST P-256 (secp256r1) Curve:** Native hardware-accelerated elliptic curve cryptography.
- **Standard Key Exports:** Public keys exported in ASN.1 `SubjectPublicKeyInfo` (SPKI) format; private keys in `PKCS#8`.
- **SHA-256 Key Derivation:** Derives a uniform 256-bit symmetric key from the shared secret point using SHA-256 hash reduction.
- **Backward & Forward Compatibility:** Supports wire formatting compatible with legacy flat-key exchange formats (`key: DH1:<Base64>`).
- **Optional Wire Metadata:** Supports optional appended metadata (e.g. Telegram Chat IDs, transport markers) without breaking key parsing.

---

## 🔒 Cryptographic Overview

### Why NIST P-256?
`ECDiffieHellman` in .NET natively supports NIST P-256 across all target platforms without requiring third-party libraries (such as BouncyCastle or NSec). This ensures minimal binary size, zero native binding issues, and maximum stability across mobile and desktop runtimes.

### Protocol Flow
1. **Party A (Initiator):**
   - Calls `ECDH.GenerateKeyPair()` to obtain `(PubKeyA, PrivKeyA)`.
   - Sends `key: DH1:<PubKeyA>` to Party B via an external channel (e.g., messaging app, Telegram bot).
   - Temporarily stores `PrivKeyA` in local pending state (e.g., `PendingPrivateKey`).

2. **Party B (Responder):**
   - Receives `key: DH1:<PubKeyA>` and parses `PubKeyA` via `ECDH.TryParseExchangePayload(...)`.
   - Calls `ECDH.GenerateKeyPair()` to produce `(PubKeyB, PrivKeyB)`.
   - Derives the Shared Secret via `ECDH.DeriveSharedSecretBase64(PrivKeyB, PubKeyA)`.
   - Sends back `key: DH1:<PubKeyB>` to Party A.

3. **Party A (Finalization):**
   - Receives `key: DH1:<PubKeyB>`.
   - Derives the identical Shared Secret via `ECDH.DeriveSharedSecretBase64(PrivKeyA, PubKeyB)`.
   - Clears `PrivKeyA` from pending state to ensure **Forward Secrecy**.

---

## 🛠️ API Reference

### 1. Key Generation
```csharp
EcdhKeyPair keyPair = ECDH.GenerateKeyPair();
string publicKeyBase64 = keyPair.PublicKeyBase64;   // SubjectPublicKeyInfo (SPKI)
string privateKeyBase64 = keyPair.PrivateKeyBase64; // PKCS#8
```

### 2. Derived Shared Secret
```csharp
// Returns a Base64-encoded 256-bit shared key derived via SHA-256
string sharedSecretBase64 = ECDH.DeriveSharedSecretBase64(myPrivateKeyBase64, peerPublicKeyBase64);

// Alternatively, obtain raw byte array (32 bytes)
byte[] sharedSecretBytes = ECDH.DeriveSharedSecretBytes(myPrivateKeyBytes, peerPublicKeyBytes);
```

### 3. Creating Transport Payloads
```csharp
// Standard ECDH payload
string payload = ECDH.CreateExchangePayload(keyPair.PublicKeyBase64);
// Output: "key: DH1:<Base64PubKey>"

// ECDH payload with optional metadata (e.g. Telegram Chat ID)
string payloadWithMeta = ECDH.CreateExchangePayload(keyPair.PublicKeyBase64, optionalMetadata: "12345678");
// Output: "key: DH1:<Base64PubKey> TGID:12345678"
```

### 4. Parsing Incoming Payloads
```csharp
if (ECDH.IsDhMessage(incomingText))
{
    if (ECDH.TryParseExchangePayload(incomingText, out string peerPubKey, out string metaId))
    {
        // Use peerPubKey to derive shared secret
    }
}
```

---

## 🔄 Compatibility Matrix

| Transport Format | Sender Version | Receiver Version | Behavior |
| :--- | :--- | :--- | :--- |
| `key: <FlatKey>` | Legacy (v1.1.8) | Legacy (v1.1.8) | Standard raw key exchange (legacy behavior). |
| `key: <FlatKey>` | Legacy (v1.1.8) | Modern (v1.1.9+) | Receiver recognizes absent `DH1:` marker, falls back seamlessly to flat key. |
| `key: DH1:<PubKey>` | Modern (v1.1.9+) | Legacy (v1.1.8) | Receiver strips outer `key: ` prefix and uses raw `DH1:<PubKey>` string as flat key fallback. |
| `key: DH1:<PubKey>` | Modern (v1.1.9+) | Modern (v1.1.9+) | Full 2-pass ECDH key agreement with local Shared Secret derivation. |

---

## 🛡️ Security Best Practices

1. **Ephemeral Key Disposal:** Once the shared secret is computed, immediately zero/nullify ephemeral private keys (`PendingPrivateKey`) to maintain forward secrecy.
2. **Secure Key Storage:** Store derived shared secrets in hardware-backed secure storage (e.g. Android Keystore, iOS Keychain) whenever possible.
3. **Transport Layer Security:** The ECDH handshake exchanges public parameters over an untrusted transport; authenticating the peer's public key out-of-band prevents Man-In-The-Middle (MitM) attacks.

---

## 📄 License
This module is released under the MIT License.
