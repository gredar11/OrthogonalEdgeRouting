using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GraphxOrtho.Models.AlgorithmTools
{
    internal class GeometryAnalizator
    {
        
        private static bool HorizontalLineIsOnVertesLevel(OrthogonalVertex vertex, Line line)
        {
            return line.Y1 >= vertex.Position.Y && line.Y1 <= vertex.Position.Y + vertex.VertexControl.ActualHeight;
        }
        private static bool VerticalLineIsOnVertesLevel(OrthogonalVertex vertex, Line line)
        {
            return line.X1 >= vertex.Position.X && line.X1 <= vertex.Position.X + vertex.VertexControl.ActualWidth;
        }
        public double GetLinesYatX(double x, Line line)
        {
            double k = (line.Y1 - line.Y2) / (line.X1 - line.X2);
            double b = line.Y1 - k * line.X1;
            return k * x + b;
        }
        public static bool LineIntersectsOrthogonalVertex( OrthogonalVertex orthogonalVertex, Line line)
        {
            double top = orthogonalVertex.Position.Y + orthogonalVertex.VertexControl.ActualHeight;
            double bottom = orthogonalVertex.Position.Y;
            double left = orthogonalVertex.Position.X;
            double right = orthogonalVertex.Position.X + orthogonalVertex.VertexControl.ActualWidth;
            if (IsLineHorizontal(line) && HorizontalLineIsOnVertesLevel(orthogonalVertex,line))
            {
                double[] lineDiapazon = {line.X1, line.X2};
                Array.Sort(lineDiapazon);
                return (lineDiapazon[0]<= left && lineDiapazon[1] >= left) || (lineDiapazon[0] <= right && lineDiapazon[1] >= right);    
            }
            if (IsLineVertical(line) && VerticalLineIsOnVertesLevel(orthogonalVertex, line))
            {
                double[] lineDiapazon = { line.Y1, line.Y2 };
                Array.Sort(lineDiapazon);
                return (lineDiapazon[0] <= bottom && lineDiapazon[1] >= bottom) || (lineDiapazon[0] <= top && lineDiapazon[1] >= top);
            }
            return false;
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
