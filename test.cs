using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        long amount = 100000;
        string cancelUrl = "https://localhost:44326/Staff/ThuNgan/PayOSReturn";
        string description = "Phi dat lich Online";
        long orderCode = 1776625567; // some code
        string returnUrl = cancelUrl;
        string checksumKey = "8a54aff326112e3e9f5e4da68d85c2c4e01356fe45275e7d210cd54a3d02e077";

        string data = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            Console.WriteLine(BitConverter.ToString(hash).Replace("-", "").ToLower());
        }
    }
}
