using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Auth
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(string email);
    }
}
