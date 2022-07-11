﻿using Priority_Queue;
using System;
using System.Windows;
using System.Collections.Generic;
namespace GraphxOrtho.Models
{
    public class PointWithDirection
    {
        public Point Point { get; set; }
        public Direction Direction { get; set; }
        /// <summary>
        /// Method calculates heuristic cost with two points. It depends of incoming direction of first point
        /// and needed direction of second.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>Cost depended on bounds needed.</returns>
        public static int GetSdByTwoPoints(PointWithDirection source, PointWithDirection target)
        {
            //Get sets of directions with dirns function
            HashSet<Direction> directions = dirns(source.Point, target.Point);
            // possible turns
            Direction turnToleft = TurnInDefiniteDirection(target.Direction, TurnDirection.left);
            Direction turnToright = TurnInDefiniteDirection(target.Direction, TurnDirection.right);
            Direction turnReverse = TurnInDefiniteDirection(target.Direction, TurnDirection.reverse);
            // source direction as HashSet for readability.
            var sourceDirectionAsHashSet = new HashSet<Direction>() { source.Direction };
            var targetDirectionAsHashSet = new HashSet<Direction>() { target.Direction };
            // if source is on the line with target and they have same directions, then return 0
            if (source.Direction == target.Direction && directions.SetEquals(sourceDirectionAsHashSet))
                return 0;
            // when its needed to add 1 bound to reach target point.
            if ((turnToleft == source.Direction || turnToright == source.Direction) && directions.Contains(source.Direction))
                return 1;
            // 2 bound
            if ((source.Direction == target.Direction && !directions.SetEquals(sourceDirectionAsHashSet) && directions.Contains(source.Direction))
                || (source.Direction == turnReverse && !directions.SetEquals(sourceDirectionAsHashSet)))
                return 2;
            // 3 bound
            if ((turnToleft == source.Direction || turnToright == source.Direction) && !directions.Contains(source.Direction))
                return 3;
            // 4 bound 
            if ((source.Direction == turnReverse && directions.SetEquals(targetDirectionAsHashSet))
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

    //public static void Do()
    //{
    //    SimplePriorityQueue<string> priorityQueue = new SimplePriorityQueue<string>();
    //    priorityQueue.Enqueue("4 - Joseph", 4);
    //    priorityQueue.Enqueue("2 - Tyler", 0); //Note: Priority = 0 right now!
    //    priorityQueue.Enqueue("1 - Jason", 1);
    //    priorityQueue.Enqueue("4 - Ryan", 4);
    //    priorityQueue.Enqueue("3 - Valerie", 3);

    //    priorityQueue.UpdatePriority("2 - Tyler", 2);

    //    while (priorityQueue.Count != 0)
    //    {
    //        string nextUser = priorityQueue.Dequeue();
    //        Console.WriteLine(nextUser);
    //    }
    //}
}