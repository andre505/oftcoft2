using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Services
{
    public class Helper
    {
        private IConfiguration _configuration;

        public Helper() { }
        public Helper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsTransactionAutheticated(string clientSecretkey, string clientSignature)
        {
           
            try
            {
                //check if secret key exists 
                string secretKey = _configuration.GetSection("nationaluptake_secretkey").Value;

                if (clientSecretkey == secretKey )
                {
                    //check hash
                    string today = DateTime.UtcNow.ToString("dd-MM-yyyy");

                    //localhash
                    string toSignLocal = secretKey + today;
                    string localSignature = Helper.GenerateSHA512String(toSignLocal);


                    if (localSignature == clientSignature)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                else
                {
                    return false;
                }

               
               
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex.Message + "##Helper:IsTransactionAutheticated" + ex.StackTrace);
                //WebLog.Log("agentIDx:error here");
                return false;
            }
        }



        private static readonly Random Random = new Random();
        public static string RandomString(int length, Mode mode = Mode.AlphaNumeric)
        {
            var characters = new List<char>();

            if (mode == Mode.Numeric || mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaNumericLower)
                for (var c = '0'; c <= '9'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaUpper)
                for (var c = 'A'; c <= 'Z'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericLower || mode == Mode.AlphaLower)
                for (var c = 'a'; c <= 'z'; c++)
                    characters.Add(c);

            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[Random.Next(s.Count)]).ToArray());
        }
        public enum Mode
        {
            AlphaNumeric = 1,
            AlphaNumericUpper = 2,
            AlphaNumericLower = 3,
            AlphaUpper = 4,
            AlphaLower = 5,
            Numeric = 6
        }

        public static string GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }

    public class AppSettings
    {
        public string Secret { get; set; }
    }
}
