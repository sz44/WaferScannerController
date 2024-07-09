using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMProjectTektronix
{

    [Serializable]
    public class OperationFailedException : Exception
    {
        public OperationFailedException()
        {
            
        }

        public OperationFailedException(string message):base(message)
        {
            
        }

        public OperationFailedException(string message, Exception innerException):base(message, innerException)
        {
            
        }
    }
}
