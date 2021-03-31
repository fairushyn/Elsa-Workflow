using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elsa_Workflow.Models
{
    public class HashedPassword
    {
        public string Salt { get; }
        public string Hashed { get; }

        public HashedPassword(byte[] hashed, byte[] salt)
        {
            Hashed = Convert.ToBase64String(hashed);
            Salt = Convert.ToBase64String(salt);
        }

        public HashedPassword(string hashed, string salt)
        {
            Hashed = hashed;
            Salt = salt;
        }
    }
}
