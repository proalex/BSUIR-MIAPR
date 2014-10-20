using System;
using System.Collections.Generic;

namespace KMeansAlg
{
    public class KMPoint
    {
        public readonly int X;
        public readonly int Y;

        public KMPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public System.Drawing.Point ConvertToPoint()
        {
            return new System.Drawing.Point(X, Y);
        }

        public double GetDistanceTo(KMPoint point)
        {
            if (point == null)
                throw new NullReferenceException("point is null");

            return Math.Sqrt(Math.Pow(Math.Abs(X - point.X), 2) + Math.Pow(Math.Abs(Y - point.Y), 2));
        }

        public double UpdateDispersion(List<KMPoint> points)
        {
            double dispersion = 0;

            if (points == null)
                throw new NullReferenceException();

            foreach (var point in points)
            {
                if (point == this)
                    continue;

                dispersion += Math.Pow(GetDistanceTo(point), 2);
            }

            return dispersion;
        }
    }
}