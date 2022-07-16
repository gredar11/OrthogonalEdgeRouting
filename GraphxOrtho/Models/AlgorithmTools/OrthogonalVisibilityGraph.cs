using GraphX.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using QuickGraph;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphxOrtho.Models.AlgorithmTools
{
    public class OrthogonalVisibilityGraph
    {
        public List<OrthogonalVertex> MainGraphVertices { get; set; }
        public AdjacencyGraph<PointWithDirection, Edge<PointWithDirection>> AdjacencyGraph { get; set; }
        public OrthogonalVisibilityGraph(List<OrthogonalVertex> mainGraphVertices, ZoomControl zoomControl, GraphAreaExample graphArea)
        {
            AdjacencyGraph = new AdjacencyGraph<PointWithDirection, Edge<PointWithDirection>>();
            MainGraphVertices = mainGraphVertices;
            foreach (var vertex in MainGraphVertices)
            {
                CutHorizontalSegments(vertex);
                CutVerticalSegments(vertex);
            }
            AddOvgToZoomControl(zoomControl);
        }
        public void CutHorizontalSegments(OrthogonalVertex currentVertex)
        {
            // Проходим по каждому сегменту узла
            foreach (var segment in currentVertex.HorizontalSegments)
            {
                // если узел пересекает 
                foreach (OrthogonalVertex vertex in MainGraphVertices)
                {
                    // пропускаем если это тот же узел, что и текущий по итерации.
                    if (vertex.Equals(currentVertex))
                        continue;
                    // если сегмент пересекает фигуру узла, то обрезаем его в зависимости от положения
                    // этих узлов. ->
                    if (IsHorizontalLineIntersectsFigure(vertex, segment))
                    {
                        // -> то обрезаем его в зависимости от положения этих узлов.
                        CutHorizontalSegment(segment, vertex, currentVertex);
                    }

                }
            }
        }
        public void CutVerticalSegments(OrthogonalVertex currentVertex)
        {
            // Проходим по каждому сегменту узла
            foreach (var segment in currentVertex.VerticalSegments)
            {
                // если узел пересекает 
                foreach (OrthogonalVertex vertex in MainGraphVertices)
                {
                    // пропускаем если это тот же узел, что и текущий по итерации.
                    if (vertex.Equals(currentVertex))
                        continue;
                    // если сегмент пересекает фигуру узла, то обрезаем его в зависимости от положения
                    // этих узлов. ->
                    if (IsVerticalLineIntersectsFigure(vertex, segment))
                    {
                        // -> то обрезаем его в зависимости от положения этих узлов.
                        CutVerticalSegment(segment, vertex, currentVertex);
                    }

                }
            }
        }
        private bool IsHorizontalLineIntersectsFigure(OrthogonalVertex v1, Line line)
        {
            if (line.Y1 >= v1.Position.Y && line.Y1 <= v1.Position.Y + v1.VertexControl.ActualHeight)
                return true;
            return false;
        }
        private bool IsVerticalLineIntersectsFigure(OrthogonalVertex v1, Line line)
        {
            if (line.X1 >= v1.Position.X && line.X1 <= v1.Position.X + v1.VertexControl.ActualWidth)
                return true;
            return false;
        }
        private void CutHorizontalSegment(Line horSegment, OrthogonalVertex vertex, OrthogonalVertex segmentParentVertex)
        {
            if (vertex.Position.X > segmentParentVertex.Position.X)
            {
                double newX2 = vertex.Position.X - vertex.MarginToEdge;
                if (newX2 < horSegment.X2)
                    horSegment.X2 = newX2;
            }
            else
            {
                double newX1 = vertex.Position.X + vertex.VertexControl.ActualWidth + vertex.MarginToEdge;
                if (newX1 > horSegment.X1)
                    horSegment.X1 = newX1;
            }
        }
        private void CutVerticalSegment(Line vertivcalSegment, OrthogonalVertex vertex, OrthogonalVertex segmentParentVertex)
        {
            if (vertex.Position.Y > segmentParentVertex.Position.Y)
            {
                double newY2 = vertex.Position.Y - vertex.MarginToEdge;
                if (newY2 < vertivcalSegment.Y2)
                    vertivcalSegment.Y2 = newY2;
            }
            else
            {
                double newY1 = vertex.Position.Y + vertex.VertexControl.ActualHeight + vertex.MarginToEdge;
                if (newY1 > vertivcalSegment.Y1)
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
        public void AddOvgToZoomControl(ZoomControl zoomctrl)
        {

            // Graph for vertex and edges search
            //OrthogonalVisibilityGraph graph = new OrthogonalVisibilityGraph(orthogonalVertices, zoomctrl, graphArea);
            // Vertical and horizontal segments of final graph
            List<Line> horizontalSegments = new List<Line>();
            List<Line> verticalSegments = new List<Line>();

            //#region Границы графа
            //var horBounder1 = new Line()
            //{
            //    Name = "Line2",
            //    Stroke = Brushes.Gray,
            //    X1 = 0,
            //    X2 = zoomctrl.ActualWidth,
            //    Y1 = 0,
            //    Y2 = 0,
            //    StrokeThickness = 0.5
            //};
            //var verBounder1 = (new Line()
            //{
            //    Name = "Line2",
            //    Stroke = Brushes.Gray,
            //    X1 = 0,
            //    X2 = 0,
            //    Y1 = 0,
            //    Y2 = zoomctrl.ActualHeight,
            //    StrokeThickness = 0.5
            //});
            //var horBounder2 = (new Line()
            //{
            //    Name = "Line2",
            //    Stroke = Brushes.Gray,
            //    X1 = 0,
            //    X2 = zoomctrl.ActualWidth,
            //    Y1 = zoomctrl.ActualHeight,
            //    Y2 = zoomctrl.ActualHeight,
            //    StrokeThickness = 0.5
            //});
            //var verBounder2 = (new Line()
            //{
            //    Name = "Line2",
            //    Stroke = Brushes.Gray,
            //    X1 = zoomctrl.ActualWidth,
            //    X2 = zoomctrl.ActualWidth,
            //    Y1 = 0,
            //    Y2 = zoomctrl.ActualHeight,
            //    StrokeThickness = 0.5
            //});
            //#endregion
            //horizontalSegments.Add(horBounder1);
            //horizontalSegments.Add(horBounder2);
            //verticalSegments.Add(verBounder1);
            //verticalSegments.Add(verBounder2);
            // adding all segments to concrete collection
            foreach (var orthogonalVertex in MainGraphVertices)
            {
                foreach (var segment in orthogonalVertex.HorizontalSegments)
                {
                    //graphArea.AddCustomChildControl(segment);
                    horizontalSegments.Add(segment);
                }
                foreach (var segment in orthogonalVertex.VerticalSegments)
                {
                    //graphArea.AddCustomChildControl(segment);
                    verticalSegments.Add(segment);
                }
            }
            // vertices of Ovg
            List<PointWithDirection> pointsForOvg = new List<PointWithDirection>();
            // dictionary of segments for adding edges of graph
            Dictionary<Line, List<PointWithDirection>> segmentsWithPoints = new Dictionary<Line, List<PointWithDirection>>();
            foreach (var hsegment in horizontalSegments)
            {
                foreach (var vsegment in verticalSegments)
                {
                    var intersection = OrthogonalVisibilityGraph.GetIntersectionOfTwoLines(hsegment, vsegment);
                    if (intersection != null && !pointsForOvg.Contains(intersection))
                    {
                        pointsForOvg.Add(intersection);
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
            // ptinting all vertices of Ovg

        }
        private static bool IsLineHorizontal(Line line)
        {
            return line.Y1 == line.Y2;
        }
        private void TestFunction()
        {
            var graph = new AdjacencyGraph<OrthogonalVertex, Edge<OrthogonalVertex>>();

        }
    }
}
