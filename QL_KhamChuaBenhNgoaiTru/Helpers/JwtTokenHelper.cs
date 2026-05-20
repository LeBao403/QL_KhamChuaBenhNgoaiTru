using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Helpers
{
    public class JwtUserInfo
    {
        public int MaTK { get; set; }
        public string Username { get; set; }
        public string MaBN { get; set; }
        public string MaNV { get; set; }
        public int? MaChucVu { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }

    public static class JwtTokenHelper
    {
        private const string DefaultIssuer = "QL_KhamChuaBenhNgoaiTru";
        private const string DefaultAudience = "QL_KhamChuaBenhNgoaiTru.Client";

        public static string GenerateToken(TaiKhoan taiKhoan, BenhNhan benhNhan = null, NhanVien nhanVien = null)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(GetExpireMinutes());
            var header = new Dictionary<string, object>
            {
                ["alg"] = "HS256",
                ["typ"] = "JWT"
            };

            var payload = new Dictionary<string, object>
            {
                ["iss"] = GetIssuer(),
                ["aud"] = GetAudience(),
                ["iat"] = ToUnixTime(now),
                ["nbf"] = ToUnixTime(now.AddSeconds(-30)),
                ["exp"] = ToUnixTime(expires),
                ["sub"] = taiKhoan.MaTK.ToString(),
                ["maTK"] = taiKhoan.MaTK,
                ["username"] = taiKhoan.Username ?? ""
            };

            if (benhNhan != null)
            {
                payload["maBN"] = benhNhan.MaBN ?? "";
                payload["role"] = "BenhNhan";
            }

            if (nhanVien != null)
            {
                payload["maNV"] = nhanVien.MaNV ?? "";
                payload["maChucVu"] = nhanVien.MaChucVu ?? 0;
                payload["role"] = "NhanVien";
            }

            var encodedHeader = Base64UrlEncode(JsonConvert.SerializeObject(header));
            var encodedPayload = Base64UrlEncode(JsonConvert.SerializeObject(payload));
            var unsignedToken = encodedHeader + "." + encodedPayload;
            var signature = Sign(unsignedToken);
            return unsignedToken + "." + signature;
        }

        public static bool TryValidateToken(string token, out JwtUserInfo userInfo)
        {
            userInfo = null;
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var parts = token.Split('.');
            if (parts.Length != 3)
                return false;

            var unsignedToken = parts[0] + "." + parts[1];
            var expectedSignature = Sign(unsignedToken);
            if (!SlowEquals(expectedSignature, parts[2]))
                return false;

            Dictionary<string, object> payload;
            try
            {
                payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlDecodeToString(parts[1]));
            }
            catch
            {
                return false;
            }

            if (!StringEquals(GetClaim(payload, "iss"), GetIssuer()) ||
                !StringEquals(GetClaim(payload, "aud"), GetAudience()))
            {
                return false;
            }

            var exp = GetLongClaim(payload, "exp");
            if (exp == null || FromUnixTime(exp.Value) <= DateTime.UtcNow)
                return false;

            var maTK = GetIntClaim(payload, "maTK");
            if (maTK == null)
                return false;

            userInfo = new JwtUserInfo
            {
                MaTK = maTK.Value,
                Username = GetClaim(payload, "username"),
                MaBN = GetClaim(payload, "maBN"),
                MaNV = GetClaim(payload, "maNV"),
                MaChucVu = GetIntClaim(payload, "maChucVu"),
                ExpiresAtUtc = FromUnixTime(exp.Value)
            };
            return true;
        }

        public static int GetExpireMinutes()
        {
            return int.TryParse(ConfigurationManager.AppSettings["JwtExpireMinutes"], out var minutes) && minutes > 0
                ? minutes
                : 120;
        }

        private static string Sign(string value)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GetSecret())))
            {
                return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
            }
        }

        private static string GetIssuer()
        {
            return ConfigurationManager.AppSettings["JwtIssuer"] ?? DefaultIssuer;
        }

        private static string GetAudience()
        {
            return ConfigurationManager.AppSettings["JwtAudience"] ?? DefaultAudience;
        }

        private static string GetSecret()
        {
            var secret = ConfigurationManager.AppSettings["JwtSecret"];
            if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
                return "DEV_ONLY_CHANGE_THIS_JWT_SECRET_QLKCB_2026";
            return secret;
        }

        private static long ToUnixTime(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private static DateTime FromUnixTime(long unixTime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(unixTime);
        }

        private static string Base64UrlEncode(string value)
        {
            return Base64UrlEncode(Encoding.UTF8.GetBytes(value));
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static string Base64UrlDecodeToString(string value)
        {
            var base64 = value.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        private static string GetClaim(Dictionary<string, object> payload, string key)
        {
            return payload.TryGetValue(key, out var value) ? value?.ToString() : null;
        }

        private static int? GetIntClaim(Dictionary<string, object> payload, string key)
        {
            var value = GetClaim(payload, key);
            return int.TryParse(value, out var result) ? result : (int?)null;
        }

        private static long? GetLongClaim(Dictionary<string, object> payload, string key)
        {
            var value = GetClaim(payload, key);
            return long.TryParse(value, out var result) ? result : (long?)null;
        }

        private static bool StringEquals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.Ordinal);
        }

        private static bool SlowEquals(string a, string b)
        {
            var left = Encoding.UTF8.GetBytes(a ?? "");
            var right = Encoding.UTF8.GetBytes(b ?? "");
            var diff = (uint)left.Length ^ (uint)right.Length;
            var max = Math.Max(left.Length, right.Length);
            for (var i = 0; i < max; i++)
            {
                diff |= (uint)((i < left.Length ? left[i] : 0) ^ (i < right.Length ? right[i] : 0));
            }
            return diff == 0;
        }
    }
}
