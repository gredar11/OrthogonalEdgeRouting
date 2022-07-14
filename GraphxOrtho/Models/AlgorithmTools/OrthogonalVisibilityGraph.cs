using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GraphxOrtho.Models.AlgorithmTools
{
    public class OrthogonalVisibilityGraph
    {
        public List<OrthogonalVertex> MainGraphVertices { get; set; }

        public OrthogonalVisibilityGraph(List<OrthogonalVertex> mainGraphVertices)
        {
            MainGraphVertices = mainGraphVertices;
            foreach (var vertex in MainGraphVertices)
            {
                CutHorizontalSegments(vertex);
                CutVerticalSegments(vertex);
            }
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
                    if(IsHorizontalLineIntersectsFigure(vertex, segment))
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
            if(vertex.Position.X > segmentParentVertex.Position.X)
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
            if(verticalSegment.X1 >= horizontalDiapazonX[0] && verticalSegment.X1 <= horizontalDiapazonX[1]
                && horizontalSegment.Y1 >= verticalDiapazonY[0] && horizontalSegment.Y1 <= verticalDiapazonY[1])
                return new PointWithDirection() { Point = new System.Windows.Point(verticalSegment.X1,horizontalSegment.Y1) };
            return null;
        }
    }
}
