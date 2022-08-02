using GraphX.Common.Interfaces;
using GraphX.Measure;
using GraphXOrthogonalEr.AlgorithmTools;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace GraphXOrthogonalEr.GeometryTools
{
    public class OvgVertex<TVertex> where TVertex : class, IGraphXVertex
    {
        public IGraphXVertex DataVertex { get; set; }
        public Point Position { get; }
        public double MarginToEdge { get; }
        public Rect SizeOfVertex { get; }
        public List<Line> HorizontalSegments { get; }
        public List<Line> VerticalSegments { get; }
        public Dictionary<IGraphXEdge<TVertex>, Point> ConnectionPoints { get; set; }

        public OvgVertex(Rect vertexSize, Point leftTopPoint, Point rightBottomPoint, double marginBetweenEdgeAndNode)
        {
            ConnectionPoints = new Dictionary<IGraphXEdge<TVertex>, Point>();
            SizeOfVertex = vertexSize;
            Position = SizeOfVertex.Location;
            VerticalSegments = new List<Line>();
            HorizontalSegments = new List<Line>();
            MarginToEdge = marginBetweenEdgeAndNode;
            // adding HorizontalSegments
            SetBordersToVertex(leftTopPoint, rightBottomPoint);
        }

        private void SetBordersToVertex(Point leftTopPoint, Point rightBottomPoint)
        {
            HorizontalSegments.Add(new Line()
            {
                X1 = leftTopPoint.X - MarginToEdge,
                Y1 = Position.Y - MarginToEdge,
                X2 = rightBottomPoint.X + MarginToEdge,
                Y2 = Position.Y - MarginToEdge,
            });
            HorizontalSegments.Add(new Line()
            {
                X1 = leftTopPoint.X - MarginToEdge,
                Y1 = Position.Y + SizeOfVertex.Height + MarginToEdge,
                X2 = rightBottomPoint.X + MarginToEdge,
                Y2 = Position.Y + SizeOfVertex.Height + MarginToEdge
            });
            // adding VerticalSegments
            VerticalSegments.Add(new Line()
            {
                X1 = Position.X - MarginToEdge,
                Y1 = leftTopPoint.Y - MarginToEdge,
                X2 = Position.X - MarginToEdge,
                Y2 = rightBottomPoint.Y + MarginToEdge
            });
            VerticalSegments.Add(new Line()
            {
                X1 = Position.X + SizeOfVertex.Width + MarginToEdge,
                Y1 = leftTopPoint.Y - MarginToEdge,
                X2 = Position.X + SizeOfVertex.Width + MarginToEdge,
                Y2 = rightBottomPoint.Y + MarginToEdge
            });
        }

        public void SetConnectionEdges(Point leftTop, Point rightBottom, IGraphXEdge<TVertex> edge)
        {
            AddSegmentByPoint(ConnectionPoints[edge], leftTop, rightBottom);
        }

        public void AddSegmentByPoint(Point connectionPoint, Point leftTop, Point rightBottom)
        {
            double topSide = Position.Y;
            double bottomSide = Position.Y + SizeOfVertex.Height;
            double leftSide = Position.X;
            double rightSide = Position.X + SizeOfVertex.Width;
            if (connectionPoint.Y == topSide)
                VerticalSegments.Add(new Line()
                {
                    X1 = connectionPoint.X,
                    Y1 = topSide,
                    X2 = connectionPoint.X,
                    Y2 = leftTop.Y - MarginToEdge
                });
            if (connectionPoint.X == rightSide)
                HorizontalSegments.Add(new Line()
                {
                    X1 = rightSide,
                    Y1 = connectionPoint.Y,
                    X2 = rightBottom.X + MarginToEdge,
                    Y2 = connectionPoint.Y
                });
            if (connectionPoint.Y == bottomSide)
                VerticalSegments.Add(new Line()
                {
                    X1 = connectionPoint.X,
                    Y1 = bottomSide,
                    X2 = connectionPoint.X,
                    Y2 = rightBottom.Y + MarginToEdge
                });
            if (connectionPoint.X == leftSide)
                HorizontalSegments.Add(new Line()
                {
                    X1 = leftSide,
                    Y1 = connectionPoint.Y,
                    X2 = leftTop.X - MarginToEdge,
                    Y2 = connectionPoint.Y
                });
        }
        public Direction GetDirectionOfPoint(Point connectionPoint, bool source)
        {
            double bottomSide = Position.Y;
            double topSide = Position.Y + SizeOfVertex.Height;
            double leftSide = Position.X;
            double rightSide = Position.X + SizeOfVertex.Width;
            if (connectionPoint.Y == topSide)
                return source ? Direction.North : Direction.South;
            if (connectionPoint.X == rightSide)
                return source ? Direction.East : Direction.West;
            if (connectionPoint.Y == bottomSide)
                return source ? Direction.South : Direction.North;
            if (connectionPoint.X == leftSide)
                return source ? Direction.West : Direction.East;
            throw new System.Exception("Can't define direction");
        }
    }
    
}
