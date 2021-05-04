using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Bridges;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Services.Svcs
{
    public class PasswordHasher : IPasswordHasher
    {
        private DBFirstDbContext _context;

        public PasswordHasher(DBFirstDbContext context)
        {
            _context = context;
        }

        #region Private Method

        // Password => GUIDSalt, RNGSalt, PasswordHash

        private string GetGUIDSalt()
        {
            return Guid.NewGuid().ToString();
        }

        private string GetRNGSalt()
        {
            // Generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        // ToLower() => 아이디 & 비밀번호 대소문자 처리
        private string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            // Derive a 256-bit subkey (use HMACHA1 with 10,000 iterations)
            // Pbkdf2
            // Password-based key derivation function 2
            // ToLower(): 소문자 변환
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId.ToLower() + password.ToLower() + guidSalt,
                salt: Encoding.UTF8.GetBytes(rngSalt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 45000, //10000, 25000, 45000
                numBytesRequested: 256 / 8));
        }

        private bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
        }

        private PasswordHashInfo PasswordInfo(string userId, string password)
        {
            string guidSalt = GetGUIDSalt();
            string rngSalt = GetRNGSalt();

            var passwordInfo = new PasswordHashInfo()
            {
                GUIDSalt = guidSalt,
                RNGSalt = rngSalt,
                PasswordHash = GetPasswordHash(userId, password, guidSalt, rngSalt)
            };

            return passwordInfo;
        }

        #endregion

        // ----------------------------------------------------
        // 명시적 인터페이스 구현
        // ----------------------------------------------------

        string IPasswordHasher.GetGUIDSalt()
        {
            return GetGUIDSalt();
        }

        string IPasswordHasher.GetRNGSalt()
        {
            return GetRNGSalt();
        }

        string IPasswordHasher.GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt);
        }

        bool IPasswordHasher.CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            return CheckThePasswordInfo(userId, password, guidSalt, rngSalt, passwordHash);
        }

        PasswordHashInfo IPasswordHasher.SetPasswordInfo(string userId, string password)
        {
            return PasswordInfo(userId, password);
        }
    }
}
