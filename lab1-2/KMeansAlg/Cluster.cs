using System;
using System.Collections.Generic;
using System.Drawing;

namespace KMeansAlg
{
    public class Cluster
    {
        public List<KMPoint> Points
        {
            get;
            private set;
        }

        public KMPoint Center
        {
            get;
            private set;
        }

        public SolidBrush Brush
        {
            get;
            private set;
        }

        public Pen Pen
        {
            get;
            private set;
        }

        public Cluster(KMPoint center, Color color)
        {
            if (center == null)
                throw new NullReferenceException("center is null");

            if (color == null)
                throw new NullReferenceException("color is null");

            Center = center;
            Pen = new Pen(color);
            Brush = new SolidBrush(color);
            Points = new List<KMPoint>();
            Points.Add(center);
        }

        public void AddPoint(KMPoint point)
        {
            if (point == null)
                throw new NullReferenceException("point is null");

            if (point == Center)
                return;

            Points.Add(point);
        }

        public void ClearPoints()
        {
            Points.Clear();
            Points.Add(Center);
        }

        public bool CalcCenter()
        {
            double minDispersion = Center.UpdateDispersion(Points);
            KMPoint newCenter = null;

            foreach (var point in Points)
            {
                if (point == Center)
                    continue;

                double currDispersion = point.UpdateDispersion(Points);

                if (currDispersion < minDispersion)
                {
                    newCenter = point;
                    minDispersion = currDispersion;
                }
            }

            if (newCenter != null)
            {
                Center = newCenter;
                return true;
            }

            return false;
        }
    }
}
