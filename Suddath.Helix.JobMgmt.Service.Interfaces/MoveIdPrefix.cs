using System;
using System.Collections.Generic;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{

    public class MoveIdPrefix
    {
        public static MoveIdPrefix GMMS_SI { get; } = new MoveIdPrefix(0, "SI");
        public static MoveIdPrefix MOVERS_SUITE { get; } = new MoveIdPrefix(1, "MS");
        public static MoveIdPrefix GMMS_GS { get; } = new MoveIdPrefix(2, "GS");
        public static MoveIdPrefix HELIX2 { get; } = new MoveIdPrefix(3, "HE");
        public static MoveIdPrefix HELIX3 { get; } = new MoveIdPrefix(3, "HX");

        public string Name { get; private set; }
        public int Value { get; private set; }

        private MoveIdPrefix(int val, string name)
        {
            Value = val;
            Name = name;
        }

        public static IEnumerable<MoveIdPrefix> List()
        {
            // alternately, use a dictionary keyed by value
            return new[] { GMMS_SI, MOVERS_SUITE, GMMS_GS, HELIX2, HELIX3 };
        }

        public static MoveIdPrefix FromString(string moveIdPrefixString)
        {
            return List().Single(r => String.Equals(r.Name, moveIdPrefixString, StringComparison.OrdinalIgnoreCase));
        }

        public static MoveIdPrefix FromValue(int value)
        {
            return List().Single(r => r.Value == value);
        }
    }
}
