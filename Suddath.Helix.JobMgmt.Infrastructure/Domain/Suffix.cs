using Suddath.Helix.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    //Single Value ValueObject
    public class Suffix : ValueObject
    {
        private readonly string _value;
        private Suffix(string value)
        {
            _value = value;
        }
        public static Suffix Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException("Value is null or empty");
            return new Suffix(value);
        }

        public static implicit operator string(Suffix type)
        {
            return type._value;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return _value;
        }
    }
}
