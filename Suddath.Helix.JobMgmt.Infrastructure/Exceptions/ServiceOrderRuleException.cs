using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Exceptions
{
    public class ServiceOrderRuleException : Exception
    {
        public ServiceOrderRuleException()
        {
        }

        public ServiceOrderRuleException(string message)
            : base(message)
        {
        }

        public ServiceOrderRuleException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
