using System;
using System.Collections.Generic;
using System.Drawing;
using KMeansAlg;

namespace MaxMinAlg
{
    public class MaxMin
    {
        public List<KMPoint> _points;
        public List<Cluster> _clusters;
        private KnownColor[] _allColors;
        private int _colorIndex = 35;

        public MaxMin(List<KMPoint> points)
        {
            if (points == null)
                throw new NullReferenceException("points is null");

            _points = points;
            _clusters = new List<Cluster>();
            Array _colorsArray = Enum.GetValues(typeof(KnownColor));
            _allColors = new KnownColor[_colorsArray.Length];
            Array.Copy(_colorsArray, _allColors, _colorsArray.Length);

            if (points.Count > 1)
            {
                KMPoint max = null;
                double maxDistance = 0;

                _clusters.Add(new Cluster(points[0], Color.FromName(_allColors[_colorIndex++].ToString())));

                foreach (var point in _points)
                {
                    double distance = point.GetDistanceTo(_clusters[0].Center);

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        max = point;
                    }
                }

                _clusters.Add(new Cluster(max, Color.FromName(_allColors[_colorIndex++].ToString())));

                foreach (var point in _points)
                    AddToCluster(point);
            }
        }

        public bool Calculate()
        {
            Cluster current = null;
            KMPoint max = null;
            bool newCore = false;
            double maxDistance = 0;
            double minDistance = 0;
            double count = 0;

            for (var i = 0; i < _clusters.Count; i++)
            {
                for (var j = i + 1; j < _clusters.Count; j++)
                {
                    minDistance += _clusters[i].Center.GetDistanceTo(_clusters[j].Center);
                    count++;
                }
            }

            minDistance /= count;
            minDistance /= 2;

            foreach (var cluster in _clusters)
            {
                foreach (var point in cluster.Points)
                {
                    double distance = point.GetDistanceTo(cluster.Center);

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        current = cluster;
                        max = point;
                    }
                }
            }

            if (max != null && maxDistance > minDistance)
            {
                newCore = true;
                current.Points.Remove(max);
            }

            if (newCore)
            {
                if (_colorIndex >= _allColors.Length)
                    _colorIndex = 35;

                _clusters.Add(new Cluster(max, Color.FromName(_allColors[_colorIndex++].ToString())));

                foreach (var cluster in _clusters)
                    cluster.ClearPoints();

                foreach (var point in _points)
                    AddToCluster(point);
            }

            return newCore;
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
