using System.Collections.Generic;
using System.Windows.Shapes;
using GraphX.Common.Interfaces;
using GraphX.Measure;
using System.Linq;

namespace GraphxOrtho.Models.OrthogonalTools
{
    internal class OvgVertex<TVertex> where TVertex : class, IGraphXVertex
    {
        public IGraphXVertex DataVertex { get; set; }
        public Point Position { get; }
        public double MarginToEdge { get; }
        public Rect SizeOfVertex { get; }
        public List<Line> HorizontalSegments { get; }
        public List<Line> VerticalSegments { get; }
        public Dictionary<IGraphXEdge<TVertex>, System.Windows.Point> ConnectionPoints { get; set; }

        public OvgVertex(Rect vertexSize, Point leftTopPoint, Point rightBottomPoint, double marginBetweenEdgeAndNode, IDictionary<IGraphXVertex, Rect> neighbours)
        {
            SizeOfVertex = vertexSize;
            Position = SizeOfVertex.Location;
            VerticalSegments = new List<Line>();
            HorizontalSegments = new List<Line>();
            MarginToEdge = marginBetweenEdgeAndNode;
            // adding HorizontalSegments
            SetBordersToVertex(leftTopPoint, rightBottomPoint);
            SetConnectionEdges(leftTopPoint, rightBottomPoint, neighbours);
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

        public void SetConnectionEdges(Point leftTop, Point rightBottom, IDictionary<IGraphXVertex, Rect> neighbours)
        {
            var topNeighbours = from v in neighbours where v.Value.Location.X == Position.X && v.Value.Location.Y > Position.Y select v;
            var bottomNeighbours = from v in neighbours where v.Value.Location.Y < Position.Y select v;
            var rightNeighbours = from v in neighbours where (v.Value.Location.X == Position.X && v.Value.Location.Y < Position.Y) || v.Value.Location.Y > Position.Y select v;


            //var relatedEdges = VertexControl.RootArea.GetRelatedEdgeControls(VertexControl);
            //foreach (var edge in relatedEdges)
            //{
            //    var edgeData = edge as EdgeControl;
            //    if (VertexControl.Equals(edgeData.Source))
            //    {
            //        Point conPoint = GetSourcePointOfEdge(edgeData);
            //        AddSegmentByPoint(conPoint, leftTop, rightBottom);
            //        ConnectionPoints.Add(conPoint);
            //    }
            //    if (VertexControl.Equals(edgeData.Target))
            //    {
            //        Point conPoint = GetTargetPointOfEdge(edgeData);
            //        AddSegmentByPoint(conPoint, leftTop, rightBottom);
            //        ConnectionPoints.Add(conPoint);
            //    }
            //}
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
        
    }
    internal struct PolarPoint
    {
        public double Radius;
        public double Angle;
    }
}
