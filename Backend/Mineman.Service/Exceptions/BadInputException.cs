using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Service.Exceptions
{
    public class BadInputException : Exception
    {
        public BadInputException(string message): base(message)
        {

        }
    }
}
