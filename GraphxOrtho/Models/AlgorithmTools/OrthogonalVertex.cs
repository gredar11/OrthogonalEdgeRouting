using GraphX.Controls;
using SystemWindows = System.Windows ;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System;

namespace GraphxOrtho.Models.AlgorithmTools
{
    internal class OrthogonalVertex
    {
        public VertexControl VertexControl { get; set; }
        public SystemWindows.Point Position { get; set; }
        List<Line> HorizontalSegments { get; set; }
        List<Line> VerticalSegments { get; set; }
        internal bool LineIntersectsVertex(Line line, OrthogonalVertex vertex)
        {
            
            return false;
        }
        //internal Func<double,double> GetLineDelegate(Line line)
        //{
            
        //}
        public OrthogonalVertex(VertexControl control, double heightOfCanvas, double widhtOfCanvas)
        {
            this.VertexControl = control;
            this.Position = VertexControl.GetPosition();
            this.VerticalSegments = new List<Line>();
            this.HorizontalSegments = new List<Line>();
            // adding HorizontalSegments
            HorizontalSegments.Add(new Line()
            {
                X1 = 0,
                Y1 = Position.Y,
                X2 = widhtOfCanvas,
                Y2 = Position.Y,
                StrokeThickness = 0.5
            });
            HorizontalSegments.Add(new Line()
            {
                X1 = 0,
                Y1 = Position.Y + VertexControl.ActualHeight,
                X2 = widhtOfCanvas,
                Y2 = Position.Y + VertexControl.ActualHeight,
                StrokeThickness = 0.5
            });
            // adding VerticalSegments
            VerticalSegments.Add(new Line()
            {
                X1 = Position.X,
                Y1 = 0,
                X2 = Position.X,
                Y2 = heightOfCanvas,
                StrokeThickness = 0.5
            });
            VerticalSegments.Add(new Line()
            {
                X1 = Position.X + VertexControl.ActualWidth,
                Y1 = 0,
                X2 = Position.X + VertexControl.ActualWidth,
                Y2 = heightOfCanvas,
                StrokeThickness = 0.5
            });
        }


        
    }
}
