using System;
using System.Runtime.Serialization;


namespace UserManagement.Domain
{
    [Serializable]
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException()
        {
        }

        public AccountNotFoundException(string message) : base(message)
        {
        }

        public AccountNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AccountNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException()
        {
        }

        public IncorrectPasswordException(string message) : base(message)
        {
        }

        public IncorrectPasswordException(string message, Exception inner) : base(message, inner)
        {
        }

        protected IncorrectPasswordException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class IncorrectTokenException : Exception
    {
        public IncorrectTokenException()
        {
        }

        public IncorrectTokenException(string message) : base(message)
        {
        }

        public IncorrectTokenException(string message, Exception inner) : base(message, inner)
        {
        }

        protected IncorrectTokenException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class AccountAlreadyExistsException : Exception
    {
        public AccountAlreadyExistsException()
        {
        }

        public AccountAlreadyExistsException(string message) : base(message)
        {
        }

        public AccountAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AccountAlreadyExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
