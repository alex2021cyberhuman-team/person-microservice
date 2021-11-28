using System;

namespace Conduit.Person.BusinessLogic
{
    public struct Empty : IEquatable<Empty>
    {
        public bool Equals(Empty other)
        {
            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is Empty other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(Empty left, Empty right)
        {
            return true;
        }

        public static bool operator !=(Empty left, Empty right)
        {
            return true;
        }
    }
}
