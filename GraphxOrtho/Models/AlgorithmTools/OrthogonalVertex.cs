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

namespace GraphxOrtho.Models.AlgorithmTools
{
    public class OrthogonalVertex
    {
        public VertexControl VertexControl { get; }
        public SystemWindows.Point Position { get; }
        public List<Line> HorizontalSegments { get; }
        public List<Line> VerticalSegments { get; }
        public double MarginToEdge { get; }
        public OrthogonalVertex(VertexControl control, Point leftTopPoint, Point rightBottomPoint, double marginBetweenEdgeAndNode)
        {
            this.VertexControl = control;
            this.Position = VertexControl.GetPosition();
            this.VerticalSegments = new List<Line>();
            this.HorizontalSegments = new List<Line>();
            this.MarginToEdge = marginBetweenEdgeAndNode;
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
        }
    }
}
