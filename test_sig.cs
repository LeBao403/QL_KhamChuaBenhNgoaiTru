using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string checksumKey = "your_checksum_key";
        long amount = 10000;
        string cancelUrl = "https://yourdomain.com/cancel";
        string description = "Thanh toan don hang";
        long orderCode = 123456;
        string returnUrl = "https://yourdomain.com/success";

        string data = $""amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            Console.WriteLine(BitConverter.ToString(hash).Replace("-", "").ToLower());
        }
    }
}
