using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.Utils
{
    public static class GlobalUtil
    {
        public static string GenerateDigit(Random rng, int number)
        {
            StringBuilder token = new StringBuilder();
            for (int i = 0; i < number; i++)
            {
                token.Append(rng.Next(10));
            };

            return token.ToString();
        }
    }
}
