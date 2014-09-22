using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KMeansAlg
{
    public class KMeans
    {
        private List<KMPoint> Points;
        private uint ClustersCount;
        private List<Cluster> Clusters;

        public KMeans(List<KMPoint> points, uint clustersCount)
        {
            uint currentColor = 35;

            if (points == null)
                throw new NullReferenceException("points is null");

            Points = points;
            ClustersCount = clustersCount;
            Clusters = new List<Cluster>();
            System.Array colorsArray = Enum.GetValues(typeof(KnownColor));
            KnownColor[] allColors = new KnownColor[colorsArray.Length];
            Array.Copy(colorsArray, allColors, colorsArray.Length);
            
            foreach (KMPoint point in points)
            {
                if (clustersCount != 0)
                {
                    Clusters.Add(new Cluster(point, Color.FromName(allColors[currentColor].ToString())));
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
            bool notChanged = true;

            Parallel.ForEach(Clusters, cluster =>
            {
                if (cluster.CalcCenter())
                    notChanged = false;
            });

            if (!notChanged)
            {
                foreach (Cluster cluster in Clusters)
                    cluster.ClearPoints();

                Parallel.ForEach(Points, point =>
                {
                    AddToCluster(point);
                });
            }

            return notChanged;
        }

        private void AddToCluster(KMPoint point)
        {
            double minDistance = 0;
            Cluster current = null;

            if (point == null)
                throw new NullReferenceException("point is null");

            foreach (Cluster cluster in Clusters)
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

            foreach (Cluster cluster in Clusters)
            {
                KMPoint center = cluster.Center;

                foreach (KMPoint point in cluster.Points)
                {
                    if (point == cluster.Center)
                        continue;

                    graphics.DrawLine(cluster.Pen, point.x - 2, point.y - 2, point.x + 2, point.y + 2);
                    graphics.DrawLine(cluster.Pen, point.x - 2, point.y + 2, point.x + 2, point.y - 2);

                    if (drawLine)
                        graphics.DrawLine(cluster.Pen, center.x, center.y, point.x, point.y);
                }

                graphics.DrawRectangle(cluster.Pen, center.x - 3, center.y - 3, 6, 6);
                graphics.FillRectangle(cluster.Brush, center.x - 3, center.y - 3, 6, 6);
            }
        }
    }
}
