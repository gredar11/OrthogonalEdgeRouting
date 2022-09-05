using GraphX.Common.Interfaces;
using GraphX.Measure;
using System;
using System.Windows.Shapes;

namespace GraphXOrthogonalEr.GeometryTools
{
    public class GeometryAnalizator<TVertex, TEdge>
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        
        private static bool HorizontalLineIsOnVertexLevel(OvgVertex<TVertex, TEdge> vertex, Line line)
        {
            return line.Y1 >= vertex.Position.Y - vertex.MarginToEdge && line.Y1 <= vertex.Position.Y + vertex.SizeOfVertex.Height + vertex.MarginToEdge;
        }
        private static bool VerticalLineIsOnVertexLevel(OvgVertex<TVertex, TEdge> vertex, Line line)
        {
            return line.X1 >= vertex.Position.X - vertex.MarginToEdge && line.X1 <= vertex.Position.X + vertex.SizeOfVertex.Width + vertex.MarginToEdge;
        }
        public double GetLinesYatX(double x, Line line)
        {
            double k = (line.Y1 - line.Y2) / (line.X1 - line.X2);
            double b = line.Y1 - k * line.X1;
            return k * x + b;
        }
        /// <summary>
        /// Check if Line intersects boundary of orthogonal vertex
        /// </summary>
        /// <param name="orthogonalVertex"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool LineIntersectsOrthogonalVertex(OvgVertex<TVertex, TEdge> orthogonalVertex, Line line)
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
        public static void SetConnectionPointsToVerticesOfEdge(OvgVertex<TVertex, TEdge> sourceVertex, OvgVertex<TVertex, TEdge> targetVertex, TEdge edge)
        {
            if (sourceVertex.Position.X == targetVertex.Position.X && sourceVertex.Position.Y == targetVertex.Position.Y)
                throw new Exception("Points in the same place");
            // if line is horizontal
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
            //  if line is vertical
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
            // if line has angle
            var pointsToAdd = GetConnectionPointOfTwoVertex(sourceVertex, targetVertex);
            sourceVertex.ConnectionPoints[edge] = pointsToAdd[0];
            targetVertex.ConnectionPoints[edge] = pointsToAdd[1];
        }
        private static Point[] GetConnectionPointOfTwoVertex(OvgVertex<TVertex, TEdge> sourceVertex, OvgVertex<TVertex, TEdge> targetVertex)
        {
            Point[] connectionPoints = new Point[2];
            // Line parameters
            double[] kAndB = CalculateKandB(new Point(sourceVertex.Position.X + sourceVertex.SizeOfVertex.Width / 2, sourceVertex.Position.Y + sourceVertex.SizeOfVertex.Height / 2),
                new Point(targetVertex.Position.X + targetVertex.SizeOfVertex.Width / 2, targetVertex.Position.Y + targetVertex.SizeOfVertex.Height / 2));
            double k = kAndB[0];
            double b = kAndB[1];
            // 1 and 3 quadrants.
            if (VerticesIsIn1And3Quadrants(sourceVertex, targetVertex))
            {
                SetConnectionPointsToVertices13(sourceVertex, targetVertex, connectionPoints, k, b);
            }
            if (VerticesIsIn2And4Quadrants(sourceVertex, targetVertex))
            {
                SetConnectionPointsToVertices24(sourceVertex, targetVertex, connectionPoints, k, b);
            }
            return connectionPoints;
        }

        private static void SetConnectionPointsToVertices24(OvgVertex<TVertex, TEdge> sourceVertex, OvgVertex<TVertex, TEdge> targetVertex, Point[] connectionPoints, double k, double b)
        {
            bool sourceIsUnderTarget = sourceVertex.Position.Y < targetVertex.Position.Y;
            var leftTopV = sourceIsUnderTarget ? targetVertex : sourceVertex;
            var rightBottomV = sourceIsUnderTarget ? sourceVertex : targetVertex;

            // left bottom vertex connection point
            var topSideIntersectionX = CalculateX(k, b, rightBottomV.Position.Y + rightBottomV.SizeOfVertex.Height);
            var leftIntersectionY = CalculateY(k, b, rightBottomV.Position.X);

            if (PointLiesOnTopOrBottomSide(rightBottomV, topSideIntersectionX))
                connectionPoints[rightBottomV == sourceVertex ? 0 : 1] = new Point(topSideIntersectionX, rightBottomV.Position.Y + rightBottomV.SizeOfVertex.Height);

            if (PointLiesOnLeftOrRightSide(rightBottomV, leftIntersectionY))
                connectionPoints[rightBottomV == sourceVertex ? 0 : 1] = new Point(rightBottomV.Position.X, leftIntersectionY);
            // left bottom vertex connection point
            var botSideIntersectionX = CalculateX(k, b, leftTopV.Position.Y);
            var rightSideIntersectionY = CalculateY(k, b, leftTopV.Position.X + leftTopV.SizeOfVertex.Width);

            if (PointLiesOnTopOrBottomSide(leftTopV, botSideIntersectionX))
                connectionPoints[leftTopV == sourceVertex ? 0 : 1] = new Point(botSideIntersectionX, leftTopV.Position.Y);

            if (PointLiesOnLeftOrRightSide(leftTopV, rightSideIntersectionY))
                connectionPoints[leftTopV == sourceVertex ? 0 : 1] = new Point(leftTopV.Position.X + leftTopV.SizeOfVertex.Width, rightSideIntersectionY);
        }

        private static bool VerticesIsIn2And4Quadrants(OvgVertex<TVertex, TEdge> sourceVertex, OvgVertex<TVertex, TEdge> targetVertex)
        {
            return targetVertex.Position.X > sourceVertex.Position.X && targetVertex.Position.Y < sourceVertex.Position.Y
                            || sourceVertex.Position.X > targetVertex.Position.X && sourceVertex.Position.Y < targetVertex.Position.Y;
        }

        private static void SetConnectionPointsToVertices13(OvgVertex<TVertex, TEdge> sourceVertex, OvgVertex<TVertex, TEdge> targetVertex, Point[] connectionPoints, double k, double b)
        {
            bool sourceIsUnderTarget = sourceVertex.Position.Y < targetVertex.Position.Y;
            var leftBottomV = sourceIsUnderTarget ? sourceVertex : targetVertex;
            var rightTopV = sourceIsUnderTarget ? targetVertex : sourceVertex;

            // left bottom vertex connection point
            var topSideIntersectionX = CalculateX(k, b, leftBottomV.Position.Y + leftBottomV.SizeOfVertex.Height);
            var rightIntersectionY = CalculateY(k, b, leftBottomV.Position.X + leftBottomV.SizeOfVertex.Width);
            // if intersects top side
            if (PointLiesOnTopOrBottomSide(leftBottomV, topSideIntersectionX))
                connectionPoints[leftBottomV == sourceVertex ? 0 : 1] = new Point(topSideIntersectionX, leftBottomV.Position.Y + leftBottomV.SizeOfVertex.Height);
            // if intersects right side
            if (PointLiesOnLeftOrRightSide(leftBottomV, rightIntersectionY))
                connectionPoints[leftBottomV == sourceVertex ? 0 : 1] = new Point(leftBottomV.Position.X + leftBottomV.SizeOfVertex.Width, rightIntersectionY);
            // left bottom vertex connection point
            var botSideIntersectionX = CalculateX(k, b, rightTopV.Position.Y);
            var leftSideIntersectionY = CalculateY(k, b, rightTopV.Position.X);
            // if line intersects bottom side
            if (PointLiesOnTopOrBottomSide(rightTopV, botSideIntersectionX))
                connectionPoints[rightTopV == sourceVertex ? 0 : 1] = new Point(botSideIntersectionX, rightTopV.Position.Y);
            // if line intersects left side
            if (PointLiesOnLeftOrRightSide(rightTopV, leftSideIntersectionY))
                connectionPoints[rightTopV == sourceVertex ? 0 : 1] = new Point(rightTopV.Position.X, leftSideIntersectionY);
        }

        private static bool PointLiesOnLeftOrRightSide(OvgVertex<TVertex, TEdge> ovgVertex, double leftSideIntersectionY)
        {
            return leftSideIntersectionY >= ovgVertex.Position.Y && leftSideIntersectionY <= ovgVertex.Position.Y + ovgVertex.SizeOfVertex.Height;
        }

        private static bool PointLiesOnTopOrBottomSide(OvgVertex<TVertex, TEdge> ovgVertex, double botSideIntersectionX)
        {
            return botSideIntersectionX >= ovgVertex.Position.X && botSideIntersectionX <= ovgVertex.Position.X + ovgVertex.SizeOfVertex.Width;
        }

        private static bool VerticesIsIn1And3Quadrants(OvgVertex<TVertex, TEdge> sourceVertex, OvgVertex<TVertex, TEdge> targetVertex)
        {
            return targetVertex.Position.X > sourceVertex.Position.X && targetVertex.Position.Y > sourceVertex.Position.Y
                            || sourceVertex.Position.X > targetVertex.Position.X && sourceVertex.Position.Y > targetVertex.Position.Y;
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
