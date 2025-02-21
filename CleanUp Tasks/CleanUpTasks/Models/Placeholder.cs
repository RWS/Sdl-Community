using System;

namespace SDLCommunityCleanUpTasks.Models
{
	public class Placeholder : IEquatable<Placeholder>
    {
        public string Content { get; set; }
        public bool IsTagPair { get; set; }

        public bool Equals(Placeholder other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return other.Content == Content &&
                       other.IsTagPair == IsTagPair;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Placeholder)obj;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return 17 * IsTagPair.GetHashCode();
        }
    }
}