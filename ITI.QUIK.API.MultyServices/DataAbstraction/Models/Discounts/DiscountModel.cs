using System.Drawing;
using System.Xml.Linq;

namespace DataAbstraction.Models.Discounts
{
    public class DiscountModel
    {
        public double DLong { get; set; }
        public double DShort { get; set; }
        public double DLong_min { get; set; }
        public double DShort_min { get; set; }
        public double KLong { get; set; }
        public double KShort { get; set; }

        public override string ToString()
        {
            return $"d({DLong}/{DShort}), dMin({DLong_min}/{DShort_min}), k({KLong}/{KShort});";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DiscountModel))
                return false;

            return Equals((DiscountModel)obj);
        }

        public bool Equals(DiscountModel other)
        {
            //Check for null and compare run-time types.
            if ((other == null) || !this.GetType().Equals(other.GetType()))
            {
                return false;
            }

            if (DLong != other.DLong) return false;
            if (DShort != other.DShort) return false;

            if (DLong_min != other.DLong_min) return false;
            if (DShort_min!= other.DShort_min) return false;

            if (KLong != other.KLong) return false;
            return KShort == other.KShort;
        }

        public static bool operator == (DiscountModel m1, DiscountModel m2)
        {
            if (m1 is null)
            {
                return m2 is null;
            }

            return m1.Equals(m2);
        }
        public static bool operator != (DiscountModel m1, DiscountModel m2)
        {
            if (m1 is null)
            {
                return m2 is not null;
            }

            return !m1.Equals(m2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DLong, DShort, DLong_min, DShort_min, KLong, KShort);
        }
    }
}
