using GraphX.Measure;
using GraphxOrtho.Models.AlgorithmTools;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphxOrtho.Models.OrthogonalTools
{
    public class PriorityPoint
    {
        public PointWithDirection DireciontPoint { get; set; }
        public PriorityPoint ParentPoint { get; set; }
        public double LengthOfPart { get; set; } = 0;
        public double Cost { get; set; }
        public PriorityPoint(PointWithDirection pointWithDirection, PriorityPoint parentPoint)
        {
            DireciontPoint = pointWithDirection;
            ParentPoint = parentPoint;
            if (parentPoint != null)
                LengthOfPart = parentPoint.LengthOfPart + DistanceBetweenPoints(parentPoint.DireciontPoint.Point, DireciontPoint.Point);
        }
        public void CalculateCost(PriorityPoint destination)
        {
            double sV = PointWithDirection.GetSdByTwoPoints(ParentPoint.DireciontPoint, destination.DireciontPoint);
            double sD = PointWithDirection.GetSdByTwoPoints(DireciontPoint, destination.DireciontPoint);
            double mDistancevv = ManhattanDistance(ParentPoint.DireciontPoint.Point, DireciontPoint.Point);
            double mDistancevd = ManhattanDistance(DireciontPoint.Point, destination.DireciontPoint.Point);
            Cost = LengthOfPart + mDistancevd + mDistancevv + sV + sD;
        }
        public double ManhattanDistance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
        private double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2.0) + Math.Pow(p1.Y - p2.Y, 2.0));
        }
    }
}
