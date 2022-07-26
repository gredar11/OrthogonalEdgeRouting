using GraphX.Controls;
using SystemWindows = System.Windows ;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System;
using System.Windows.Media;
using System.Windows;
using GraphX.Common.Interfaces;
using GraphX;

namespace GraphxOrtho.Models.AlgorithmTools
{
    public class OrthogonalVertex
    {
        public VertexControl VertexControl { get; }
        public Point Position { get; }
        public List<Line> HorizontalSegments { get; }
        public List<Line> VerticalSegments { get; }
        public List<Point> ConnectionPoints { get; set; }
        public double MarginToEdge { get; }
        public OrthogonalVertex(VertexControl control, Point leftTopPoint, Point rightBottomPoint, double marginBetweenEdgeAndNode)
        {
            VertexControl = control;
            Position = VertexControl.GetPosition();
            VerticalSegments = new List<Line>();
            HorizontalSegments = new List<Line>();
            ConnectionPoints = new List<Point>();
            MarginToEdge = marginBetweenEdgeAndNode;
            // adding HorizontalSegments
            HorizontalSegments.Add(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = leftTopPoint.X - MarginToEdge,
                Y1 = Position.Y - MarginToEdge,
                X2 = rightBottomPoint.X + MarginToEdge,
                Y2 = Position.Y - MarginToEdge,
                StrokeThickness = 0.5
            });
            HorizontalSegments.Add(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = leftTopPoint.X - MarginToEdge,
                Y1 = Position.Y + VertexControl.ActualHeight + MarginToEdge,
                X2 = rightBottomPoint.X + MarginToEdge,
                Y2 = Position.Y + VertexControl.ActualHeight + MarginToEdge,
                StrokeThickness = 0.5
            });
            // adding VerticalSegments
            VerticalSegments.Add(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = Position.X - MarginToEdge,
                Y1 = leftTopPoint.Y - MarginToEdge,
                X2 = Position.X - MarginToEdge,
                Y2 = rightBottomPoint.Y + MarginToEdge,
                StrokeThickness = 0.5
            });
            VerticalSegments.Add(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = Position.X + VertexControl.ActualWidth + MarginToEdge,
                Y1 = leftTopPoint.Y - MarginToEdge,
                X2 = Position.X + VertexControl.ActualWidth + MarginToEdge,
                Y2 = rightBottomPoint.Y + MarginToEdge,
                StrokeThickness = 0.5
            });
            SetConnectionEdges( leftTopPoint, rightBottomPoint);
        }
        public void SetConnectionEdges( Point leftTop, Point rightBottom)
        {
            var relatedEdges = VertexControl.RootArea.GetRelatedEdgeControls(VertexControl);
            foreach (var edge in relatedEdges)
            {
                var edgeData = edge as EdgeControl;
                if (VertexControl.Equals(edgeData.Source))
                {
                    Point conPoint = GetSourcePointOfEdge(edgeData);
                    AddSegmentByPoint(conPoint, leftTop, rightBottom);
                    ConnectionPoints.Add(conPoint);
                }
                if (VertexControl.Equals(edgeData.Target))
                {
                    Point conPoint = GetTargetPointOfEdge(edgeData);
                    AddSegmentByPoint(conPoint, leftTop, rightBottom);
                    ConnectionPoints.Add(conPoint);
                }
            }
        }
        public void AddSegmentByPoint(Point connectionPoint, Point leftTop, Point rightBottom)
        {
            double topSide = Position.Y;
            double bottomSide = Position.Y + VertexControl.ActualHeight;
            double leftSide = Position.X;
            double rightSide = Position.X + VertexControl.ActualWidth;
            if(connectionPoint.Y == topSide)
                VerticalSegments.Add(new Line() { 
                    X1 = connectionPoint.X, Y1 = topSide, 
                    X2 = connectionPoint.X, Y2 = leftTop.Y - MarginToEdge, Name = "conn", Stroke = Brushes.Red
                });
            if (connectionPoint.X == rightSide)
                HorizontalSegments.Add(new Line() { 
                    X1 = rightSide, Y1 = connectionPoint.Y, 
                    X2 = rightBottom.X + MarginToEdge, Y2 = connectionPoint.Y,
                    Stroke = Brushes.Red,
                    Name = "conn"
                });
            if(connectionPoint.Y == bottomSide)
                VerticalSegments.Add(new Line() { 
                    X1 = connectionPoint.X, Y1 = bottomSide, 
                    X2 = connectionPoint.X, Y2 = rightBottom.Y + MarginToEdge,
                    Name = "conn",
                    Stroke = Brushes.Red
                });
            if (connectionPoint.X == leftSide)
                HorizontalSegments.Add(new Line() { 
                    X1 = leftSide, Y1 = connectionPoint.Y, 
                    X2 = leftTop.X - MarginToEdge, Y2 = connectionPoint.Y,
                    Stroke = Brushes.Red,
                    Name = "conn"
                });
        }
        private System.Windows.Point GetSourcePointOfEdge(EdgeControl edgeControl)
        {
            var commonEdge = edgeControl.DataContext as IGraphXCommonEdge;
            System.Windows.Point sourceConnPoint = new System.Windows.Point();
            var routedEdge = edgeControl.Edge as IRoutingInfo;
            var routeInformation = routedEdge.RoutingPoints;
            var hasRouteInfo = routeInformation != null && routeInformation.Length > 1;

            var sourceSize = new System.Windows.Size
            {
                Width = edgeControl.Source.ActualWidth,
                Height = edgeControl.Source.ActualHeight
            };
            var centerPos = new System.Windows.Point
            {
                X = (GraphAreaBase.GetFinalX(edgeControl.Source)) + sourceSize.Width * 0.5,
                Y = (GraphAreaBase.GetFinalY(edgeControl.Source)) + sourceSize.Height * 0.5
            };
            var leftCornerPos = new System.Windows.Point
            {
                X = (GraphAreaBase.GetFinalX(edgeControl.Source)),
                Y = (GraphAreaBase.GetFinalY(edgeControl.Source))
            };
            var targetCenterPos = new System.Windows.Point
            {
                X = (GraphAreaBase.GetFinalX(edgeControl.Target)),
                Y = (GraphAreaBase.GetFinalY(edgeControl.Target))
            };
            if (commonEdge?.SourceConnectionPointId != null)
            {
                var sourceCp = edgeControl.Source.GetConnectionPointById(commonEdge.SourceConnectionPointId.Value, true);
                if (sourceCp == null)
                {
                    throw new System.Exception("");
                }

            }
            else
                sourceConnPoint = GeometryHelper.GetEdgeEndpoint(
                    centerPos, new System.Windows.Rect(leftCornerPos, sourceSize), 
                    (hasRouteInfo ? routeInformation[1].ToWindows() : (targetCenterPos)), 
                    edgeControl.Source.VertexShape);

            return sourceConnPoint;
        }
        private System.Windows.Point GetTargetPointOfEdge(EdgeControl edgeControl)
        {

            var commonEdge = edgeControl.DataContext as IGraphXCommonEdge;
            System.Windows.Point TargetConnPoint = new System.Windows.Point();
            var routedEdge = edgeControl.Edge as IRoutingInfo;
            var routeInformation = routedEdge.RoutingPoints;
            var hasRouteInfo = routeInformation != null && routeInformation.Length > 1;

            var TargetSize = new System.Windows.Size
            {
                Width = edgeControl.Target.ActualWidth,
                Height = edgeControl.Target.ActualHeight
            };
            var centerPos = new System.Windows.Point
            {
                X = (GraphAreaBase.GetFinalX(edgeControl.Target)) + TargetSize.Width * 0.5,
                Y = (GraphAreaBase.GetFinalY(edgeControl.Target)) + TargetSize.Height * 0.5
            };
            var leftCornerPos = new System.Windows.Point
            {
                X = (GraphAreaBase.GetFinalX(edgeControl.Target)),
                Y = (GraphAreaBase.GetFinalY(edgeControl.Target))
            };
            var targetCenterPos = new System.Windows.Point
            {
                X = (GraphAreaBase.GetFinalX(edgeControl.Source)),
                Y = (GraphAreaBase.GetFinalY(edgeControl.Source))
            };
            if (commonEdge?.TargetConnectionPointId != null)
            {
                var TargetCp = edgeControl.Target.GetConnectionPointById(commonEdge.TargetConnectionPointId.Value, true);
                if (TargetCp == null)
                {
                    throw new System.Exception("");
                }

            }
            else
                TargetConnPoint = GeometryHelper.GetEdgeEndpoint(
                    centerPos, new System.Windows.Rect(leftCornerPos, TargetSize),
                    (hasRouteInfo ? routeInformation[1].ToWindows() : (targetCenterPos)),
                    edgeControl.Target.VertexShape);

            return TargetConnPoint;
        }

    }
}
