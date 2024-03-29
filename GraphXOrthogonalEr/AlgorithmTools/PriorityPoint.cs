﻿using GraphX.Measure;
using System;

namespace GraphXOrthogonalEr.AlgorithmTools
{
    public class PriorityPoint
    {
        // Value to divide distances to. For punish many bends
        private static double _distanceFactor = 1.0;
        public static double DistanceFactor { 
            get { return _distanceFactor; } 
            set 
            { 
                if(value <= 0.0)
                    _distanceFactor = 1.0;
                else
                    _distanceFactor = value;
            } 
        }
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
            Cost = (ParentPoint.LengthOfPart + mDistancevd + mDistancevv)  + sV + sD + (ParentPoint.DireciontPoint.Direction == DireciontPoint.Direction ? 0 : 2);
        }
        public static double ManhattanDistance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
        private static double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2.0) + Math.Pow(p1.Y - p2.Y, 2.0));
        }
        public override string ToString()
        {
            return DireciontPoint.Point.ToString();
        }
    }
}
