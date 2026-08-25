# TooliSafe AES-256 Crypto Utility

This repository/module contains a clean, isolated AES-256 encryption and decryption utility designed for the TooliSafe application. 

It provides straightforward methods to secure both strings and files using standard cryptography libraries in .NET (`System.Security.Cryptography`).

## Features

- **String Encryption & Decryption:** Uses a fixed 8-byte salt and 1000 iterations for PBKDF2 (SHA-256) key derivation. This maintains backward compatibility with older TooliSafe string formats.
- **Asynchronous File Encryption & Decryption:** Uses a secure, dynamically generated 16-byte salt and 10,000 iterations for PBKDF2 (SHA-256). The salt is prepended to the resulting encrypted file.
- **Isolated Logic:** Fully decoupled from UI elements, app-specific states, and error dialogs for easy testing and portability.

## Usage Examples

### Encrypting and Decrypting a String

```csharp
using TooliSafe.Crypto;

string myPassword = "SuperSecretPassword123!";
string textToSecure = "This is a confidential message.";

// Encrypt
byte[] encryptedData = AES256.EncryptStringToBytes(textToSecure, myPassword);

// Decrypt
string decryptedText = AES256.DecryptStringFromBytes(encryptedData, myPassword);
```

### Encrypting and Decrypting a File

```csharp
using System.Threading.Tasks;
using TooliSafe.Crypto;

public async Task ProcessFiles()
{
    string password = "StrongFilePassword456!";
    string inputFile = "cleartext_document.pdf";
    string encryptedFile = "document.tsafe";
    string decryptedFile = "restored_document.pdf";

    // Encrypt File
    await AES256.EncryptFileAsync(inputFile, encryptedFile, password);

    // Decrypt File
    bool success = await AES256.DecryptFileAsync(encryptedFile, decryptedFile, password);
    
    if (success)
    {
        // Decryption successful
    }
}
```

## Security & Compatibility Notes
- **String Encryption:** The fixed salt (`0x1` to `0x8`) and `1000` iterations are preserved specifically to guarantee backward compatibility with legacy TooliSafe backups.
- **File Encryption:** A new random salt is generated every time a file is encrypted, making it highly secure against precomputed dictionary attacks. The iteration count is set to `10000` to increase resistance against brute-force attacks while maintaining acceptable performance.
