using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrDragon.Exceptions
{
    public static class OracleExceptions
    {
        public static void Parse(OracleException oraex)
        {
            switch (oraex.Number)
            {
                case 20500:
                    throw new UsernameAlreadyUsedException();
                    break;
                case 20501:
                    throw new InvalidLoginException();
                    break;
                default:
                    break;
            }
        }
    }
}