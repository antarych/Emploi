using System;
using System.Runtime.Serialization;

namespace OrganizationManagement.Domain
{
    public class OrgExceptions
    {
        [Serializable]
        public class OrganizationNotFoundException : Exception
        {
            public OrganizationNotFoundException()
            {
            }

            public OrganizationNotFoundException(string message) : base(message)
            {
            }

            public OrganizationNotFoundException(string message, Exception inner) : base(message, inner)
            {
            }

            protected OrganizationNotFoundException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
