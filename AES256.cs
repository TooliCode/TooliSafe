using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TooliSafe.Crypto
{
    /// <summary>
    /// Provides clean, isolated AES-256 encryption and decryption methods for strings and files.
    /// </summary>
    public static class AES256
    {
        // Fixed salt for string encryption (maintained for backward compatibility).
        private static readonly byte[] StringSalt = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8 };
        private const int StringIterations = 1000;
        private const int FileIterations = 10000;

        // ═══════════════════════════════════════════════════════════════════════
        // --- STRING ENCRYPTION ---
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Encrypts a plaintext string using AES-256 and returns the encrypted bytes.
        /// </summary>
        public static byte[] EncryptStringToBytes(string plainText, string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, StringSalt, StringIterations, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();
            aes.Key = deriveBytes.GetBytes(32); // 256-bit Key
            aes.IV = deriveBytes.GetBytes(16);  // 128-bit Block size

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return ms.ToArray();
        }

        /// <summary>
        /// Decrypts an AES-256 encrypted byte array back into a plaintext string.
        /// </summary>
        public static string DecryptStringFromBytes(byte[] cipherText, string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, StringSalt, StringIterations, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();
            aes.Key = deriveBytes.GetBytes(32);
            aes.IV = deriveBytes.GetBytes(16);

            using var ms = new MemoryStream(cipherText);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }

        // ═══════════════════════════════════════════════════════════════════════
        // --- FILE ENCRYPTION ---
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Asynchronously encrypts a file using AES-256. Generates a random salt and prepends it to the destination file.
        /// </summary>
        public static async Task EncryptFileAsync(string inputFile, string outputFile, string password)
        {
            // Generate dynamic salt
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            
            using var rfc = new Rfc2898DeriveBytes(password, salt, FileIterations, HashAlgorithmName.SHA256);
            byte[] keyBytes = rfc.GetBytes(32);
            byte[] ivBytes = rfc.GetBytes(16);

            using var fsIn = File.OpenRead(inputFile);
            using var fsOut = File.Create(outputFile);
            
            // Write unencrypted salt to the beginning of the file
            await fsOut.WriteAsync(salt, 0, salt.Length);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            using (var cs = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                await fsIn.CopyToAsync(cs);
                await cs.FlushFinalBlockAsync(); // Guarantees the final AES block is written
            }
        }

        /// <summary>
        /// Asynchronously decrypts an AES-256 encrypted file. Reads the dynamic salt from the first 16 bytes of the file.
        /// Returns false if the file is too short or decryption fails.
        /// </summary>
        public static async Task<bool> DecryptFileAsync(string inputFile, string outputFile, string password)
        {
            try
            {
                using var fsIn = File.OpenRead(inputFile);
                
                // Read salt (first 16 bytes)
                byte[] salt = new byte[16];
                int read = await fsIn.ReadAsync(salt, 0, salt.Length);
                if (read < 16) return false; // File is invalid or corrupted

                using var rfc = new Rfc2898DeriveBytes(password, salt, FileIterations, HashAlgorithmName.SHA256);
                byte[] keyBytes = rfc.GetBytes(32);
                byte[] ivBytes = rfc.GetBytes(16);

                using var aes = Aes.Create();
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using var fsOut = File.Create(outputFile);
                using var cs = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read);
                
                await cs.CopyToAsync(fsOut);
                return true;
            }
            catch (CryptographicException)
            {
                // Usually fails due to an incorrect password or corrupted file
                return false;
            }
            catch (Exception)
            {
                // Catch general IO errors
                return false;
            }
        }
    }
}