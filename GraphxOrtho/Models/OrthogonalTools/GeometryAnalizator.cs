using GraphX.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphX.Measure;

using System.Windows.Shapes;

namespace GraphxOrtho.Models.OrthogonalTools
{
    public class GeometryAnalizator<TVertex, TEdge>
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        
        private static bool HorizontalLineIsOnVertexLevel(OvgVertex<TVertex> vertex, Line line)
        {
            return line.Y1 >= vertex.Position.Y - vertex.MarginToEdge && line.Y1 <= vertex.Position.Y + vertex.SizeOfVertex.Height + vertex.MarginToEdge;
        }
        private static bool VerticalLineIsOnVertexLevel(OvgVertex<TVertex> vertex, Line line)
        {
            return line.X1 >= vertex.Position.X - vertex.MarginToEdge && line.X1 <= vertex.Position.X + vertex.SizeOfVertex.Width + vertex.MarginToEdge;
        }
        public double GetLinesYatX(double x, Line line)
        {
            double k = (line.Y1 - line.Y2) / (line.X1 - line.X2);
            double b = line.Y1 - k * line.X1;
            return k * x + b;
        }
        public static bool LineIntersectsOrthogonalVertex(OvgVertex<TVertex> orthogonalVertex, Line line)
        {
            double top = orthogonalVertex.Position.Y + orthogonalVertex.SizeOfVertex.Height + orthogonalVertex.MarginToEdge;
            double bottom = orthogonalVertex.Position.Y - orthogonalVertex.MarginToEdge;
            double left = orthogonalVertex.Position.X - orthogonalVertex.MarginToEdge;
            double right = orthogonalVertex.Position.X + orthogonalVertex.SizeOfVertex.Width + orthogonalVertex.MarginToEdge;
            if (IsLineHorizontal(line) && HorizontalLineIsOnVertexLevel(orthogonalVertex,line))
            {
                double[] lineDiapazon = {line.X1, line.X2};
                Array.Sort(lineDiapazon);
                return (lineDiapazon[0]<= left && lineDiapazon[1] >= left) || (lineDiapazon[0] <= right && lineDiapazon[1] >= right);    
            }
            if (IsLineVertical(line) && VerticalLineIsOnVertexLevel(orthogonalVertex, line))
            {
                double[] lineDiapazon = { line.Y1, line.Y2 };
                Array.Sort(lineDiapazon);
                return (lineDiapazon[0] <= bottom && lineDiapazon[1] >= bottom) || (lineDiapazon[0] <= top && lineDiapazon[1] >= top);
            }
            return false;
        }
        public static void SetConnectionPointsToVerticesOfEdge(OvgVertex<TVertex> sourceVertex, OvgVertex<TVertex> targetVertex, TEdge edge)
        {
            if (sourceVertex.Position.X == targetVertex.Position.X && sourceVertex.Position.Y == targetVertex.Position.Y)
                throw new Exception("Points in the same place");
            // если линия горизонтальна
            if(sourceVertex.Position.Y == targetVertex.Position.Y)
            {
                if(sourceVertex.Position.X > targetVertex.Position.X)
                {
                    sourceVertex.ConnectionPoints[edge] = new Point(sourceVertex.SizeOfVertex.Left, sourceVertex.Position.Y + sourceVertex.SizeOfVertex.Height / 2);
                    targetVertex.ConnectionPoints[edge] = new Point(targetVertex.SizeOfVertex.Right, targetVertex.Position.Y + targetVertex.SizeOfVertex.Height / 2);
                    return;
                }
                sourceVertex.ConnectionPoints[edge] = new Point(sourceVertex.SizeOfVertex.Right, sourceVertex.Position.Y + sourceVertex.SizeOfVertex.Height / 2);
                targetVertex.ConnectionPoints[edge] = new Point(targetVertex.SizeOfVertex.Left, targetVertex.Position.Y + targetVertex.SizeOfVertex.Height / 2);
                return;
            }
            // если линия вертикальная
            if (sourceVertex.Position.X == targetVertex.Position.X)
            {
                if(sourceVertex.Position.Y > targetVertex.Position.Y)
                {
                    sourceVertex.ConnectionPoints[edge] = new Point(sourceVertex.Position.X + sourceVertex.SizeOfVertex.Width / 2, sourceVertex.Position.Y);
                    targetVertex.ConnectionPoints[edge] = new Point(targetVertex.Position.X + targetVertex.SizeOfVertex.Width / 2, targetVertex.Position.Y + targetVertex.SizeOfVertex.Height);
                    return;
                }
                targetVertex.ConnectionPoints[edge] = new Point(targetVertex.Position.X + targetVertex.SizeOfVertex.Width / 2, targetVertex.Position.Y);
                sourceVertex.ConnectionPoints[edge] = new Point(sourceVertex.Position.X + sourceVertex.SizeOfVertex.Width / 2, sourceVertex.Position.Y + sourceVertex.SizeOfVertex.Height);
                return;
            }
            // если линия под наклоном.
            var pointsToAdd = GetConnectionPointOfTwoVertex(sourceVertex, targetVertex);
            sourceVertex.ConnectionPoints[edge] = pointsToAdd[0];
            targetVertex.ConnectionPoints[edge] = pointsToAdd[1];
        }
        private static Point[] GetConnectionPointOfTwoVertex(OvgVertex<TVertex> sourceVertex, OvgVertex<TVertex> targetVertex)
        {
            Point[] connectionPoints = new Point[2];
            // Line parameters
            double[] kAndB = CalculateKandB(new Point(sourceVertex.Position.X + sourceVertex.SizeOfVertex.Width / 2, sourceVertex.Position.Y + sourceVertex.SizeOfVertex.Height / 2),
                new Point(targetVertex.Position.X + targetVertex.SizeOfVertex.Width / 2, targetVertex.Position.Y + targetVertex.SizeOfVertex.Height / 2));
            double k = kAndB[0];
            double b = kAndB[1];
            // 1 and 3 quadrants.
            if (targetVertex.Position.X > sourceVertex.Position.X && targetVertex.Position.Y > sourceVertex.Position.Y
                || sourceVertex.Position.X > targetVertex.Position.X && sourceVertex.Position.Y > targetVertex.Position.Y)
            {
                bool sourceIsUnderTarget = sourceVertex.Position.Y < targetVertex.Position.Y;
                var leftBottomV = sourceIsUnderTarget ? sourceVertex : targetVertex;
                var rightTopV = sourceIsUnderTarget ? targetVertex : sourceVertex;

                // left bottom vertex connection point
                var topSideIntersectionX = CalculateX(k, b, leftBottomV.Position.Y + leftBottomV.SizeOfVertex.Height);
                var rightIntersectionY = CalculateY(k, b, leftBottomV.Position.X + leftBottomV.SizeOfVertex.Width);
                // if intersects top side
                if (topSideIntersectionX >= leftBottomV.Position.X && topSideIntersectionX <= leftBottomV.Position.X + leftBottomV.SizeOfVertex.Width) 
                    connectionPoints[leftBottomV == sourceVertex ? 0 : 1] = new Point(topSideIntersectionX, leftBottomV.Position.Y + leftBottomV.SizeOfVertex.Height);
                // if intersects right side
                if(rightIntersectionY >= leftBottomV.Position.Y && rightIntersectionY <= leftBottomV.Position.Y + leftBottomV.SizeOfVertex.Height)
                    connectionPoints[leftBottomV == sourceVertex ? 0 : 1] = new Point(leftBottomV.Position.X + leftBottomV.SizeOfVertex.Width, rightIntersectionY);
                // left bottom vertex connection point
                var botSideIntersectionX = CalculateX(k, b, rightTopV.Position.Y);
                var leftSideIntersectionY = CalculateY(k, b, rightTopV.Position.X);
                // if line intersects bottom side
                if (botSideIntersectionX >= rightTopV.Position.X && botSideIntersectionX <= rightTopV.Position.X + rightTopV.SizeOfVertex.Width)
                    connectionPoints[rightTopV == sourceVertex ? 0 : 1] = new Point(botSideIntersectionX, rightTopV.Position.Y);
                // if line intersects left side
                if (leftSideIntersectionY >= rightTopV.Position.Y && leftSideIntersectionY <= rightTopV.Position.Y + rightTopV.SizeOfVertex.Height)
                    connectionPoints[rightTopV == sourceVertex ? 0 : 1] = new Point(rightTopV.Position.X, leftSideIntersectionY);
            }
            if (targetVertex.Position.X > sourceVertex.Position.X && targetVertex.Position.Y < sourceVertex.Position.Y
                || sourceVertex.Position.X > targetVertex.Position.X && sourceVertex.Position.Y < targetVertex.Position.Y)
            {
                bool sourceIsUnderTarget = sourceVertex.Position.Y < targetVertex.Position.Y;
                var leftTopV = sourceIsUnderTarget ? targetVertex : sourceVertex;
                var rightBottomV = sourceIsUnderTarget ? sourceVertex : targetVertex;

                // left bottom vertex connection point
                var topSideIntersectionX = CalculateX(k, b, rightBottomV.Position.Y + rightBottomV.SizeOfVertex.Height);
                var leftIntersectionY = CalculateY(k, b, rightBottomV.Position.X);

                if (topSideIntersectionX >= rightBottomV.Position.X && topSideIntersectionX <= rightBottomV.Position.X + rightBottomV.SizeOfVertex.Width)
                    connectionPoints[rightBottomV == sourceVertex ? 0 : 1] = new Point(topSideIntersectionX, rightBottomV.Position.Y + rightBottomV.SizeOfVertex.Height);

                if (leftIntersectionY >= rightBottomV.Position.Y && leftIntersectionY <= rightBottomV.Position.Y + rightBottomV.SizeOfVertex.Height)
                    connectionPoints[rightBottomV == sourceVertex ? 0 : 1] = new Point(rightBottomV.Position.X, leftIntersectionY);
                // left bottom vertex connection point
                var botSideIntersectionX = CalculateX(k, b, leftTopV.Position.Y);
                var rightSideIntersectionY = CalculateY(k, b, leftTopV.Position.X + leftTopV.SizeOfVertex.Width);

                if (botSideIntersectionX >= leftTopV.Position.X && botSideIntersectionX <= leftTopV.Position.X + leftTopV.SizeOfVertex.Width)
                    connectionPoints[leftTopV == sourceVertex ? 0 : 1] = new Point(botSideIntersectionX, leftTopV.Position.Y);

                if (rightSideIntersectionY >= leftTopV.Position.Y && rightSideIntersectionY <= leftTopV.Position.Y + leftTopV.SizeOfVertex.Height)
                    connectionPoints[leftTopV == sourceVertex ? 0 : 1] = new Point(leftTopV.Position.X + leftTopV.SizeOfVertex.Width, rightSideIntersectionY);
            }
            return connectionPoints;
        }
        private static double[] CalculateKandB(Point p1, Point p2)
        {
            double k = (p1.Y - p2.Y) / (p1.X - p2.X);
            double b = p1.Y - k * p1.X;
            return new double[] {k, b};
        }
        private static double CalculateY(double k, double b, double x)
        {
            return k * x + b;
        }
        private static double CalculateX(double k, double b, double y)
        {
            return (y - b) / k;
        }
        public static bool IsLineVertical(Line line)
        {
            return line.X1 == line.X2;
        }
        public static bool IsLineHorizontal(Line line)
        {
            return line.Y1 == line.Y2;
        }
    }
}
