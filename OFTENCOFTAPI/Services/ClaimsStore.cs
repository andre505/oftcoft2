using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Services
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("View Users", "View Users"),
            new Claim("Create Draw", "Create Draw"),
        };

    }
}
