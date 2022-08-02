using GraphX.Measure;
using System;
using System.Collections.Generic;
namespace GraphXOrthogonalEr.AlgorithmTools
{
    public class PointWithDirection
    {
        public Point Point { get; set; }
        public Direction Direction { get; set; }
        /// <summary>
        /// Method calculates heuristic cost with two points. It depends of incoming direction of first point
        /// and needed direction of second. In this case North direction means that you going up on x axis, and South vice versa.
        /// Wpf Canvas y axis has "direction from top to bottom".
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>Cost depended on bounds needed.</returns>
        public static int GetSdByTwoPoints(PointWithDirection source, PointWithDirection target)
        {
            //Get sets of directions with dirns function
            HashSet<Direction> directions = dirns(source.Point, target.Point);
            //DirsFromTargetTosource
            HashSet<Direction> directionsFromTargetTosource = dirns(target.Point, source.Point);

            // possible turns
            Direction turnToleft = TurnInDefiniteDirection(target.Direction, TurnDirection.left);
            Direction turnToright = TurnInDefiniteDirection(target.Direction, TurnDirection.right);
            Direction turnReverse = TurnInDefiniteDirection(target.Direction, TurnDirection.reverse);
            // source direction as HashSet for readability.
            var sourceDirectionAsHashSet = new HashSet<Direction>() { source.Direction };
            var targetDirectionAsHashSet = new HashSet<Direction>() { target.Direction };
            // if source is on the line with target and they have same directions, then return 0 #CHECKED
            if (source.Direction == target.Direction && directions.SetEquals(sourceDirectionAsHashSet))
                return 0;
            // when its needed to add 1 bound to reach target point.
            if ((turnToleft == source.Direction || turnToright == source.Direction) && directions.Contains(source.Direction) && !directions.Contains(turnReverse))
                return 1;
            // 2 bound
            if ((source.Direction == target.Direction && !directions.SetEquals(sourceDirectionAsHashSet) && directions.Contains(source.Direction))
                || (source.Direction == turnReverse && !directions.SetEquals(targetDirectionAsHashSet) && !directions.SetEquals(new HashSet<Direction> { turnReverse})))
                return 2;
            // 3 bound
            if ((turnToleft == source.Direction || turnToright == source.Direction) && (!directions.Contains(source.Direction) || !directionsFromTargetTosource.Contains(source.Direction)) )
                return 3;
            // 4 bound 
            if ((source.Direction == turnReverse && (directions.SetEquals(targetDirectionAsHashSet) || directions.SetEquals(new HashSet<Direction> { turnReverse })))
                || (source.Direction == target.Direction && !directions.Contains(source.Direction)))
                return 4;
            throw new Exception($"Can't find Sd. of points {source} with {source.Direction} " +
                $"and {target} with {target.Direction}");
        }
        /// <summary>
        /// dirns(v1,v2) function from article. Helps to calculate possible directions
        /// to reach target point from source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>HashSet of directions.</returns>
        public static HashSet<Direction> dirns(Point source, Point target)
        {
            if (target.X == source.X && target.Y == source.Y)
                return new HashSet<Direction>() { Direction.Stop };
            HashSet<Direction> result = new HashSet<Direction>();
            if (target.Y > source.Y)
                result.Add(Direction.North);
            if (target.X > source.X)
                result.Add(Direction.East);
            if (target.Y < source.Y)
                result.Add(Direction.South);
            if (target.X < source.X)
                result.Add(Direction.West);

            return result;
        }

        /// <summary>
        /// Returns a new direction based on the original direction with a rotation option.
        /// </summary>
        /// <param name="currentDirection"></param>
        /// <param name="turnSide">1 - right, 0 - reverse, </param>
        /// <returns></returns>
        public static Direction TurnInDefiniteDirection(Direction currentDirection, TurnDirection turnDirection)
        {
            switch (currentDirection)
            {
                case Direction.North:
                    if (turnDirection == TurnDirection.right)
                        return Direction.East;
                    if (turnDirection == TurnDirection.left)
                        return Direction.West;
                    if (turnDirection == TurnDirection.reverse)
                        return Direction.South;
                    break;
                case Direction.East:
                    if (turnDirection == TurnDirection.right)
                        return Direction.South;
                    if (turnDirection == TurnDirection.left)
                        return Direction.North;
                    if (turnDirection == TurnDirection.reverse)
                        return Direction.West;
                    break;
                case Direction.South:
                    if (turnDirection == TurnDirection.right)
                        return Direction.West;
                    if (turnDirection == TurnDirection.left)
                        return Direction.East;
                    if (turnDirection == TurnDirection.reverse)
                        return Direction.North;
                    break;
                case Direction.West:
                    if (turnDirection == TurnDirection.right)
                        return Direction.North;
                    if (turnDirection == TurnDirection.left)
                        return Direction.South;
                    if (turnDirection == TurnDirection.reverse)
                        return Direction.East;
                    break;
            }
            return Direction.Stop;
        }
        public override string ToString()
        {
            return $"({Point.X:F3} ; {Point.Y:F3}), {Direction} | " + base.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj is PointWithDirection) return ((PointWithDirection)obj).Point.X == this.Point.X && ((PointWithDirection)obj).Point.Y == this.Point.Y;
            return false;
        }
        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }
    }
    public enum TurnDirection
    {
        right, left, reverse
    }
    public enum Direction
    {
        North = 1,
        East = 2,
        South = 3,
        West = 4,
        Stop = 0
    }

    
}
