using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KMeansAlg
{
    public class KMPoint
    {
        public readonly int x;
        public readonly int y;

        public KMPoint (int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public System.Drawing.Point ConvertToPoint()
        {
            return new System.Drawing.Point(x, y);
        }

        public double GetDistanceTo(KMPoint point)
        {
            if (point == null)
                throw new NullReferenceException("point is null");

            return Math.Sqrt(Math.Pow(Math.Abs(x - point.x), 2) + Math.Pow(Math.Abs(y - point.y), 2));
        }

        public double UpdateDispersion(List<KMPoint> points)
        {
            double dispersion = 0;

            if (points == null)
                throw new NullReferenceException();

            foreach (KMPoint point in points)
            {
                if (point == this)
                    continue;

                dispersion += Math.Pow(GetDistanceTo(point), 2);
            }

            return dispersion;
        }
    }
}
