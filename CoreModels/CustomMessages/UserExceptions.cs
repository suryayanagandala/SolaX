using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.CustomMessages
{
    public class UserNotFound:Exception
    {
        public UserNotFound(string message):base(message) { }
        
    }

    public class EmailExists : Exception
    {
        public EmailExists(string message):base(message)
        {
            
        }
    }


    public class DublicateUserException : Exception
    {
        public DublicateUserException(string message):base (message) 
        {
            
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message):base(message)
        {
            
        }
    }


    public class InActiveModeExceptions : Exception
    {
        public InActiveModeExceptions(string message):base(message) 
        {
            
        }
    }

    public class DataExistsException : Exception
    {
        public DataExistsException(string message):base(message)
        {
            
        }
    }


    public class ExpiryException : Exception
    {
        public ExpiryException(string message):base(message)
        {
            
        }
    }
}
