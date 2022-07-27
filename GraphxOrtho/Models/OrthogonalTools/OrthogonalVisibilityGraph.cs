using GraphX.Common.Interfaces;
using GraphX.Measure;
using GraphxOrtho.Models.AlgorithmTools;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;

namespace GraphxOrtho.Models.OrthogonalTools
{
    public class OrthogonalVisibilityGraphMod<TVertex, TEdge> 
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        public List<OvgVertex<TVertex>> MainGraphVertices { get; set; }
        public AdjacencyGraph<PointWithDirection, Edge<PointWithDirection>> AdjacencyGraph { get; set; }
        public OrthogonalVisibilityGraphMod(List<OvgVertex<TVertex>> mainGraphVertices)
        {
            AdjacencyGraph = new AdjacencyGraph<PointWithDirection, Edge<PointWithDirection>>();
            MainGraphVertices = mainGraphVertices;
            foreach (var vertex in MainGraphVertices)
            {
                CutHorizontalSegments(vertex);
                CutVerticalSegments(vertex);
            }
            AddOvgToZoomControl();
        }
        public void CutHorizontalSegments(OvgVertex<TVertex> currentVertex)
        {
            // Проходим по каждому сегменту узла
            foreach (var segment in currentVertex.HorizontalSegments)
            {
                // если узел пересекает 
                foreach (OvgVertex<TVertex> vertex in MainGraphVertices)
                {
                    // пропускаем если это тот же узел, что и текущий по итерации.
                    if (vertex.Equals(currentVertex))
                        continue;
                    // если сегмент пересекает фигуру узла, то обрезаем его в зависимости от положения
                    // этих узлов. ->
                    if (GeometryAnalizator<TVertex, TEdge>.LineIntersectsOrthogonalVertex(vertex, segment))
                    {
                        // -> то обрезаем его в зависимости от положения этих узлов.
                        CutHorizontalSegment(segment, vertex, currentVertex);
                    }

                }
            }
        }
        public void CutVerticalSegments(OvgVertex<TVertex> currentVertex)
        {
            // Проходим по каждому сегменту узла
            foreach (var segment in currentVertex.VerticalSegments)
            {
                // если узел пересекает 
                foreach (OvgVertex<TVertex> vertex in MainGraphVertices)
                {
                    // пропускаем если это тот же узел, что и текущий по итерации.
                    if (vertex.Equals(currentVertex))
                        continue;
                    // если сегмент пересекает фигуру узла, то обрезаем его в зависимости от положения
                    // этих узлов. ->
                    if (GeometryAnalizator<TVertex,TEdge>.LineIntersectsOrthogonalVertex(vertex, segment))
                    {
                        // -> то обрезаем его в зависимости от положения этих узлов.
                        CutVerticalSegment(segment, vertex, currentVertex);
                    }

                }
            }
        }
        
        private void CutHorizontalSegment(Line horSegment, OvgVertex<TVertex> vertex, OvgVertex<TVertex> segmentParentVertex)
        {
            if (vertex.Position.X > segmentParentVertex.Position.X)
            {
                // обрезаем справа
                double newX2 = vertex.Position.X - vertex.MarginToEdge;

                if (newX2 < horSegment.X2 && horSegment.X2 > horSegment.X1)
                    horSegment.X2 = newX2;
                else if (newX2 < horSegment.X1 && horSegment.X2 < horSegment.X1)
                    horSegment.X1 = newX2;
            }
            else
            {
                // обрезаем слева
                double newX1 = vertex.Position.X + vertex.SizeOfVertex.Width + vertex.MarginToEdge;
                if (newX1 > horSegment.X2 && horSegment.X2 < horSegment.X1)
                    horSegment.X2 = newX1;
                else if (newX1 > horSegment.X1 && horSegment.X2 > horSegment.X1)
                    horSegment.X1 = newX1;
            }
        }
        private void CutVerticalSegment(Line vertivcalSegment, OvgVertex<TVertex> vertex, OvgVertex<TVertex> segmentParentVertex)
        {
            if (vertex.Position.Y > segmentParentVertex.Position.Y)
            {
                // обрезаем сверху
                double newY2 = vertex.Position.Y - vertex.MarginToEdge;
                if (newY2 < vertivcalSegment.Y2 && vertivcalSegment.Y2 > vertivcalSegment.Y1)
                    vertivcalSegment.Y2 = newY2;

                else if (newY2 < vertivcalSegment.Y1 && vertivcalSegment.Y2 < vertivcalSegment.Y1)
                    vertivcalSegment.Y1 = newY2;
            }
            else
            {
                // обрезаем снизу
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
            if (verticalSegment.X1 >= horizontalDiapazonX[0] && verticalSegment.X1 <= horizontalDiapazonX[1]
                && horizontalSegment.Y1 >= verticalDiapazonY[0] && horizontalSegment.Y1 <= verticalDiapazonY[1])
                return new PointWithDirection() { Point = new System.Windows.Point(verticalSegment.X1, horizontalSegment.Y1) };
            return null;
        }
        public void AddOvgToZoomControl()
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
            // vertices of Ovg
            // dictionary of segments for adding edges of graph
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
            // printing all edges of Ovg
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
                        AdjacencyGraph.AddVerticesAndEdge(edge);
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
                        AdjacencyGraph.AddVerticesAndEdge(edge);
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
            // ptinting all vertices of Ovg

        }
        public void AddConnectionSegment(OvgVertex<TVertex> ovgVertex, Point connectionPoint)
        {
            double topSide = ovgVertex.Position.Y;
            double bottomSide = ovgVertex.Position.Y + ovgVertex.SizeOfVertex.Height;
            double leftSide = ovgVertex.Position.X;
            double rightSide = ovgVertex.Position.X + ovgVertex.SizeOfVertex.Width;
            if (connectionPoint.Y == topSide)
                AdjacencyGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new System.Windows.Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new System.Windows.Point { X = connectionPoint.X, Y = connectionPoint.Y - ovgVertex.MarginToEdge} }));
            
            if (connectionPoint.X == rightSide)
                AdjacencyGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new System.Windows.Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new System.Windows.Point { X = connectionPoint.X + ovgVertex.MarginToEdge, Y = connectionPoint.Y } }));

            if (connectionPoint.Y == bottomSide)
                AdjacencyGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new System.Windows.Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new System.Windows.Point { X = connectionPoint.X, Y = connectionPoint.Y + ovgVertex.MarginToEdge} }));

            if (connectionPoint.X == leftSide)
                AdjacencyGraph.AddVerticesAndEdge(new Edge<PointWithDirection>(new PointWithDirection() { Point = new System.Windows.Point(connectionPoint.X, connectionPoint.Y) }, new PointWithDirection() { Point = new System.Windows.Point { X = connectionPoint.X - ovgVertex.MarginToEdge, Y = connectionPoint.Y} }));

        }
        private static bool IsLineHorizontal(Line line)
        {
            return line.Y1 == line.Y2;
        }
        private void TestFunction()
        {
            var graph = new AdjacencyGraph<OvgVertex<TVertex>, Edge<OvgVertex<TVertex>>>();

        }
    }
}
