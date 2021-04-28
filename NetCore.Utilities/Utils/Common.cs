using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography;

namespace NetCore.Utilities.Utils
{
    public static class Common
    {
        /// <summary>
        /// Data Protection 지정
        /// </summary>
        /// <param name="services">등록할 서비스</param>
        /// <param name="keyPath">키 경로</param>
        /// <param name="applicationName">Application 이름</param>
        /// <param name="crytoType">암호화 유형</param>
        public static void SetDataProtection(IServiceCollection services, string keyPath, string applicationName, Enum crytoType)
        {
            // https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/data-protection/configuration/overview.md 참고
            var builder = services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                    .SetApplicationName(applicationName);

            // Chaining Method - 사슬과 같이 연결하여 여러번 실행

            switch (crytoType)
            {
                // 관리되지 않는 암호화키
                case Enums.CryptoType.Unmanaged:
                    // AES
                    // Advanced Encrytion Standard
                    // Two-way: 암호화, 복호화
                    builder.UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            // SHA
                            // Secure Hash Algorithm
                            // One-way: 암호화
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                        });
                    break;
                // 관리되는 암호화키
                case Enums.CryptoType.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration()
                        {
                            // A type that subclasses SymmetricAlgorithm
                            EncryptionAlgorithmType = typeof(Aes),

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // A type that subclasses KeyedHashAlgorithm
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        });
                    break;
                // Cng - Cbc Type
                case Enums.CryptoType.CngCbc:
                    // Windows CNG algorithm using CBC
                    // CNG Algorithm
                    // Cryptography API : Next Generation
                    // CBC-mode
                    // Cipher Block Chaining
                    builder.UseCustomCryptographicAlgorithms(
                        new CngCbcAuthenticatedEncryptorConfiguration()
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // Passed to BCryptOpenAlgorithmProvider
                            HashAlgorithm = "SHA512",
                            HashAlgorithmProvider = null
                        });
                    break;
                // Cng - GCM Type
                case Enums.CryptoType.CngGcm:
                    // Windows CNG algorithm using Galois / Counter Mode encryption
                    // Galois/Counter Mode
                    // GCM
                    builder.UseCustomCryptographicAlgorithms(
                        new CngGcmAuthenticatedEncryptorConfiguration()
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256
                        });
                    break;
            }
        }
    }
}
