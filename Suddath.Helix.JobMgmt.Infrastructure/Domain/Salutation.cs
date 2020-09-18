using Suddath.Helix.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    //Single Value ValueObject
    public class Salutation : ValueObject
    {
        private readonly string _value;
        private Salutation(string value)
        {
            _value = value;
        }
        public static Salutation Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException("Value is null or empty");
            return new Salutation(value);
        }

        public static implicit operator string(Salutation type)
        {
            return type._value;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return _value;
        }
    }
}
