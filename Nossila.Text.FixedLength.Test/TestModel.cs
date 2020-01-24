using System;
using System.Collections.Generic;
using System.Text;

namespace Nossila.Text.FixedLength.Test
{
    public sealed class Movie
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public int ReleaseYear { get; set; }
        public decimal Budget { get; set; }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Director, ReleaseYear, Budget);
        }
    }
}
