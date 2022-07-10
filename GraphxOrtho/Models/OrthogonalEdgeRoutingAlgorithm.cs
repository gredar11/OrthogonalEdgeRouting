using GraphX.Common.Interfaces;
using GraphX.Measure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphxOrtho.Models
{
    internal class OrthogonalEdgeRoutingAlgorithm<TVertex, TEdge> :
        IExternalEdgeRouting<TVertex, TEdge>
    {
        public IDictionary<TVertex, Rect> VertexSizes { get; set; }
        public IDictionary<TVertex, Point> VertexPositions { get; set; }


        public Rect AreaRectangle { get; set; }

        readonly Dictionary<TEdge, Point[]> _edgeRoutes = new Dictionary<TEdge, Point[]>();
        public IDictionary<TEdge, Point[]> EdgeRoutes { get { return _edgeRoutes; } }
        public void Compute(CancellationToken cancellationToken)
        {
            
        }

        public Point[] ComputeSingle(TEdge edge)
        {
            return null;
        }

        public void UpdateVertexData(TVertex vertex, Point position, Rect size)
        {
            
        }
    }
}
