﻿using GraphX.Common.Interfaces;
using GraphX.Measure;
using GraphXOrthogonalEr.AlgorithmTools;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;

namespace GraphXOrthogonalEr.GeometryTools
{
    public class OrthogonalVisibilityGraphMod<TVertex, TEdge>
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        public List<OvgVertex<TVertex, TEdge>> MainGraphVertices { get; set; }
        public BidirectionalGraph<PointWithDirection, Edge<PointWithDirection>> BiderectionalGraph { get; set; }
        public OrthogonalVisibilityGraphMod(List<OvgVertex<TVertex, TEdge>> mainGraphVertices)
        {
            BiderectionalGraph = new BidirectionalGraph<PointWithDirection, Edge<PointWithDirection>>();
            MainGraphVertices = mainGraphVertices;
            foreach (var vertex in MainGraphVertices)
            {
                CutHorizontalSegments(vertex);
                CutVerticalSegments(vertex);
            }
            CreateBiderectionalGraphWithConnectionPoints();
        }
        public void CutHorizontalSegments(OvgVertex<TVertex, TEdge> currentVertex)
        {
            foreach (var segment in currentVertex.HorizontalSegments)
            {
                foreach (OvgVertex<TVertex, TEdge> vertex in MainGraphVertices)
                {
                    if (vertex.Equals(currentVertex))
                        continue;
                    if (GeometryAnalizator<TVertex, TEdge>.LineIntersectsOrthogonalVertex(vertex, segment))
                    {
                        CutHorizontalSegment(segment, vertex, currentVertex);
                    }

                }
            }
        }
        public void CutVerticalSegments(OvgVertex<TVertex, TEdge> currentVertex)
        {
            foreach (var segment in currentVertex.VerticalSegments)
            {
                foreach (OvgVertex<TVertex, TEdge> vertex in MainGraphVertices)
                {
                    if (vertex.Equals(currentVertex))
                        continue;
                    if (GeometryAnalizator<TVertex, TEdge>.LineIntersectsOrthogonalVertex(vertex, segment))
                    {
                        CutVerticalSegment(segment, vertex, currentVertex);
                    }

                }
            }
        }

        private void CutHorizontalSegment(Line horSegment, OvgVertex<TVertex, TEdge> vertex, OvgVertex<TVertex, TEdge> segmentParentVertex)
        {
            if (vertex.Position.X > segmentParentVertex.Position.X)
            {
                double newX2 = vertex.Position.X - vertex.MarginToEdge;

                if (newX2 < horSegment.X2 && horSegment.X2 > horSegment.X1)
                    horSegment.X2 = newX2;
                else if (newX2 < horSegment.X1 && horSegment.X2 < horSegment.X1)
                    horSegment.X1 = newX2;
            }
            else
            {
                double newX1 = vertex.Position.X + vertex.SizeOfVertex.Width + vertex.MarginToEdge;
                if (newX1 > horSegment.X2 && horSegment.X2 < horSegment.X1)
                    horSegment.X2 = newX1;
                else if (newX1 > horSegment.X1 && horSegment.X2 > horSegment.X1)
                    horSegment.X1 = newX1;
            }
        }
        private void CutVerticalSegment(Line vertivcalSegment, OvgVertex<TVertex, TEdge> vertex, OvgVertex<TVertex, TEdge> segmentParentVertex)
        {
            if (vertex.Position.Y > segmentParentVertex.Position.Y)
            {
                double newY2 = vertex.Position.Y - vertex.MarginToEdge;
                if (newY2 < vertivcalSegment.Y2 && vertivcalSegment.Y2 > vertivcalSegment.Y1)
                    vertivcalSegment.Y2 = newY2;

                else if (newY2 < vertivcalSegment.Y1 && vertivcalSegment.Y2 < vertivcalSegment.Y1)
                    vertivcalSegment.Y1 = newY2;
            }
            else
            {
                double newY1 = vertex.Position.Y + vertex.SizeOfVertex.Height + vertex.MarginToEdge;

                if (newY1 > vertivcalSegment.Y2 && vertivcalSegment.Y2 < vertivcalSegment.Y1)
                    vertivcalSegment.Y2 = newY1;

                else if (newY1 > vertivcalSegment.Y1 && vertivcalSegment.Y2 > vertivcalSegment.Y1)
                    vertivcalSegment.Y1 = newY1;
            }
        }
        public static PointWithDirection GetIntersectionOfTwoLines(Line horizontalSegment, Line verticalSegment)
        {
            var horizontalDiapazonX = new double[] { horizontalSegment.X1, horizontalSegment.X2 };
            var verticalDiapazonY = new double[] { verticalSegment.Y1, verticalSegment.Y2 };
            Array.Sort(verticalDiapazonY);
            Array.Sort(horizontalDiapazonX);
            // if lines are CROSSED:
            if (verticalSegment.X1 >= horizontalDiapazonX[0] && verticalSegment.X1 <= horizontalDiapazonX[1]
                && horizontalSegment.Y1 >= verticalDiapazonY[0] && horizontalSegment.Y1 <= verticalDiapazonY[1])
                return new PointWithDirection() { Point = new Point(verticalSegment.X1, horizontalSegment.Y1) };
            return null;
        }
        // Creating Biderectional graph with connection points. It represents orthogonal visibility graph
        // Possible edge routing goes on graph's edges.
        public void CreateBiderectionalGraphWithConnectionPoints()
        {
            List<Line> horizontalSegments = new List<Line>();
            List<Line> verticalSegments = new List<Line>();

            foreach (var vertex in MainGraphVertices)
            {
                foreach (var segment in vertex.HorizontalSegments)
                {
                    //graphArea.AddCustomChildControl(segment);
                    horizontalSegments.Add(segment);
                }
                foreach (var segment in vertex.VerticalSegments)
                {
                    //graphArea.AddCustomChildControl(segment);
                    verticalSegments.Add(segment);
                }
            }
            // Making intersection of all segments(horizontal and vertical)
            Dictionary<Line, List<PointWithDirection>> segmentsWithPoints = new Dictionary<Line, List<PointWithDirection>>();
            foreach (var hsegment in horizontalSegments)
            {
                foreach (var vsegment in verticalSegments)
                {
                    var intersection = GetIntersectionOfTwoLines(hsegment, vsegment);
                    if (intersection != null)
                    {
                        if (!segmentsWithPoints.ContainsKey(hsegment) || segmentsWithPoints[hsegment] == null)
                        {
                            segmentsWithPoints[hsegment] = new List<PointWithDirection>();
                        }
                        segmentsWithPoints[hsegment].Add(intersection);
                        if (!segmentsWithPoints.ContainsKey(vsegment) || segmentsWithPoints[vsegment] == null)
                        {
                            segmentsWithPoints[vsegment] = new List<PointWithDirection>();
                        }
                        segmentsWithPoints[vsegment].Add(intersection);
                    }
                }
            }
            // Addition edges and vertices to graph.
            foreach (var lineSegment in segmentsWithPoints)
            {
                if (IsLineHorizontal(lineSegment.Key))
                {
                    var points = lineSegment.Value;
                    var pointssortedByX = (points.OrderBy(l => l.Point.X)).ToList();
                    if (pointssortedByX.Count() <= 1)
                        continue;
                    for (int i = 1; i < pointssortedByX.Count; i++)
                    {
                        Edge<PointWithDirection> edge = new Edge<PointWithDirection>(pointssortedByX[i - 1], pointssortedByX[i]);
                        BiderectionalGraph.AddVerticesAndEdge(edge);
                    }
                }
                else
                {
                    var points = lineSegment.Value;
                    var pointssortedByY = (points.OrderBy(l => l.Point.Y)).ToList();
                    if (pointssortedByY.Count() <= 1)
                        continue;
                    for (int i = 1; i < pointssortedByY.Count; i++)
                    {
                        Edge<PointWithDirection> edge = new Edge<PointWithDirection>(pointssortedByY[i - 1], pointssortedByY[i]);
                        BiderectionalGraph.AddVerticesAndEdge(edge);
                    }
                }
            }
            foreach (var vertex in MainGraphVertices)
            {
                foreach (var connPoint in vertex.ConnectionPoints.Values)
                {
                    AddConnectionSegment(vertex, connPoint);
                }
            }
        }
        public void AddConnectionSegment(OvgVertex<TVertex, TEdge> ovgVertex, Point connectionPoint)
        {
            double bottomSide = ovgVertex.Position.Y;
            double topSide = ovgVertex.Position.Y + ovgVertex.SizeOfVertex.Height;
            double leftSide = ovgVertex.Position.X;
            double rightSide = ovgVertex.Position.X + ovgVertex.SizeOfVertex.Width;
            if (connectionPoint.Y == bottomSide)
                BiderectionalGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new Point { X = connectionPoint.X, Y = connectionPoint.Y - ovgVertex.MarginToEdge } }));

            if (connectionPoint.X == rightSide)
                BiderectionalGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new Point { X = connectionPoint.X + ovgVertex.MarginToEdge, Y = connectionPoint.Y } }));

            if (connectionPoint.Y == topSide)
                BiderectionalGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new Point { X = connectionPoint.X, Y = connectionPoint.Y + ovgVertex.MarginToEdge } }));

            if (connectionPoint.X == leftSide)
                BiderectionalGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new Point { X = connectionPoint.X - ovgVertex.MarginToEdge, Y = connectionPoint.Y } }));

        }
        private static bool IsLineHorizontal(Line line)
        {
            return line.Y1 == line.Y2;
        }
        public List<PriorityPoint> InitializeNeighbours(PriorityPoint parentPoint)
        {
            // находим соседние ветви
            var outEdges = BiderectionalGraph.OutEdges(parentPoint.DireciontPoint);
            var inEdges = BiderectionalGraph.InEdges(parentPoint.DireciontPoint);
            var allAjacenceEdges = outEdges.Union(inEdges);
            // добавляем узлы-соседей если они не были прошлыми родителями
            List<PointWithDirection> neighbours = new List<PointWithDirection>();
            foreach (Edge<PointWithDirection> edge in allAjacenceEdges)
            {
                var otherSideVertex = parentPoint.DireciontPoint.Point == edge.Source.Point ? edge.Target : edge.Source;
                if ((parentPoint.ParentPoint == null || parentPoint.ParentPoint.DireciontPoint.Point != otherSideVertex.Point)
                    && parentPoint.DireciontPoint.Point != otherSideVertex.Point && !neighbours.Contains(otherSideVertex))
                    neighbours.Add(otherSideVertex);
            }
            List<PriorityPoint> neigrboursToReturn = new List<PriorityPoint>();
            foreach (var neighbour in neighbours)
            {
                // по каждому узлу создаем узел приоритета.
                PriorityPoint priorityPoint = new PriorityPoint(neighbour, parentPoint);
                // для точки с направлением устанавливаем направление.
                priorityPoint.DireciontPoint.Direction = GetDestinationPointDirection(parentPoint.DireciontPoint.Point, priorityPoint.DireciontPoint.Point, parentPoint.DireciontPoint.Direction);
                neigrboursToReturn.Add(priorityPoint);
            }
            return neigrboursToReturn;
        }
        public Direction GetDestinationPointDirection(Point source, Point target, Direction parentdir)
        {
            if (source == target)
                return parentdir;
            if (source.X == target.X)
            {
                if (target.Y > source.Y)
                    return Direction.North;
                if (target.Y < source.Y)
                    return Direction.South;
            }
            if (source.Y == target.Y)
            {
                if (target.X > source.X)
                    return Direction.East;
                if (target.X < source.X)
                    return Direction.West;
            }
            throw new Exception("Can't get Direction to points!");
        }
    }
}
