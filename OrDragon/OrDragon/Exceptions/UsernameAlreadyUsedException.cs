using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrDragon.Exceptions
{
    class UsernameAlreadyUsedException : Exception
    {
        public UsernameAlreadyUsedException()
        {
        }

        public UsernameAlreadyUsedException(string message) : base(message)
        {
        }

        public UsernameAlreadyUsedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
