using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;

namespace KMeansAlg
{
    public class KMeans
    {
        private List<KMPoint> _points;
        private int _clustersCount;
        private List<Cluster> _clusters;

        public KMeans(List<KMPoint> points, List<Cluster> clusters)
        {
            if (points == null)
                throw new NullReferenceException("points is null");

            if (clusters == null)
                throw new NullReferenceException("clusters is null");

            _clustersCount = clusters.Count;
            _clusters = clusters;
            _points = points;
        }

        public KMeans(List<KMPoint> points, int clustersCount)
        {
            uint currentColor = 35;

            if (points == null)
                throw new NullReferenceException("points is null");

            _points = points;
            _clustersCount = clustersCount;
            _clusters = new List<Cluster>();
            Array colorsArray = Enum.GetValues(typeof(KnownColor));
            KnownColor[] allColors = new KnownColor[colorsArray.Length];
            Array.Copy(colorsArray, allColors, colorsArray.Length);
            
            foreach (var point in points)
            {
                if (clustersCount != 0)
                {
                    _clusters.Add(new Cluster(point, Color.FromName(allColors[currentColor].ToString())));
                    clustersCount--;
                    currentColor++;

                    if (currentColor >= colorsArray.Length)
                        currentColor = 35;
                }
                else
                    AddToCluster(point);
            }
        }

        public bool Calculate()
        {
            bool changed = false;

            Parallel.ForEach(_clusters, cluster =>
            {
                if (cluster.CalcCenter())
                    changed = true;
            });

            if (changed)
            {
                foreach (var cluster in _clusters)
                    cluster.ClearPoints();

                foreach (var point in _points)
                    AddToCluster(point);
            }

            return changed;
        }

        private void AddToCluster(KMPoint point)
        {
            double minDistance = 0;
            Cluster current = null;

            if (point == null)
                throw new NullReferenceException("point is null");

            foreach (var cluster in _clusters)
            {
                double distance = point.GetDistanceTo(cluster.Center);

                if (current == null || minDistance > distance)
                {
                    minDistance = distance;
                    current = cluster;
                }
            }

            if (current != null)
                current.AddPoint(point);
        }

        public void DrawClusters(Graphics graphics, bool drawLine)
        {
            if (graphics == null)
                throw new NullReferenceException("graphics is null");

            foreach (var cluster in _clusters)
            {
                KMPoint center = cluster.Center;

                foreach (var point in cluster.Points)
                {
                    if (point == cluster.Center)
                        continue;

                    graphics.DrawLine(cluster.Pen, point.X - 2, point.Y - 2, point.X + 2, point.Y + 2);
                    graphics.DrawLine(cluster.Pen, point.X - 2, point.Y + 2, point.X + 2, point.Y - 2);

                    if (drawLine)
                        graphics.DrawLine(cluster.Pen, center.X, center.Y, point.X, point.Y);
                }

                graphics.DrawRectangle(cluster.Pen, center.X - 3, center.Y - 3, 6, 6);
                graphics.FillRectangle(cluster.Brush, center.X - 3, center.Y - 3, 6, 6);
            }
        }
    }
}
