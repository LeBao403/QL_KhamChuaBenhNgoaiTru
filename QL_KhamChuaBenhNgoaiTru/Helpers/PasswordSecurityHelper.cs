using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace QL_KhamChuaBenhNgoaiTru.Helpers
{
    public static class PasswordSecurityHelper
    {
        private const string Prefix = "PBKDF2";
        private const int Iterations = 100000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public static List<string> ValidatePassword(string password, string username = null, string email = null)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Vui lòng nhập mật khẩu.");
                return errors;
            }

            if (password.Length < 8)
                errors.Add("Mật khẩu phải có ít nhất 8 ký tự.");
            if (!password.Any(char.IsUpper))
                errors.Add("Mật khẩu phải có ít nhất 1 chữ hoa.");
            if (!password.Any(char.IsLower))
                errors.Add("Mật khẩu phải có ít nhất 1 chữ thường.");
            if (!password.Any(char.IsDigit))
                errors.Add("Mật khẩu phải có ít nhất 1 chữ số.");
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                errors.Add("Mật khẩu phải có ít nhất 1 ký tự đặc biệt.");
            if (password.Any(char.IsWhiteSpace))
                errors.Add("Mật khẩu không được chứa khoảng trắng.");

            var normalizedPassword = password.ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(username) && username.Length >= 4 &&
                normalizedPassword.Contains(username.Trim().ToLowerInvariant()))
            {
                errors.Add("Mật khẩu không được chứa tên đăng nhập.");
            }

            var emailName = email?.Split('@').FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(emailName) && emailName.Length >= 4 &&
                normalizedPassword.Contains(emailName.Trim().ToLowerInvariant()))
            {
                errors.Add("Mật khẩu không được chứa phần tên email.");
            }

            return errors;
        }

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password is required.", nameof(password));

            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var key = pbkdf2.GetBytes(KeySize);
                return string.Join("$", Prefix, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
            }
        }

        public static bool IsHashed(string passwordHash)
        {
            return !string.IsNullOrWhiteSpace(passwordHash) &&
                   passwordHash.StartsWith(Prefix + "$", StringComparison.Ordinal);
        }

        public static string EnsureHashed(string passwordOrHash)
        {
            if (string.IsNullOrWhiteSpace(passwordOrHash))
                return passwordOrHash;

            return IsHashed(passwordOrHash) ? passwordOrHash : HashPassword(passwordOrHash);
        }

        public static bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedPassword))
                return false;

            if (!IsHashed(storedPassword))
                return SlowEquals(password, storedPassword);

            var parts = storedPassword.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations))
                return false;

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expectedKey = Convert.FromBase64String(parts[3]);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    var actualKey = pbkdf2.GetBytes(expectedKey.Length);
                    return SlowEquals(actualKey, expectedKey);
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool SlowEquals(string a, string b)
        {
            return SlowEquals(System.Text.Encoding.UTF8.GetBytes(a), System.Text.Encoding.UTF8.GetBytes(b));
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            var max = Math.Max(a.Length, b.Length);
            for (var i = 0; i < max; i++)
            {
                var left = i < a.Length ? a[i] : (byte)0;
                var right = i < b.Length ? b[i] : (byte)0;
                diff |= (uint)(left ^ right);
            }
            return diff == 0;
        }
    }
}
