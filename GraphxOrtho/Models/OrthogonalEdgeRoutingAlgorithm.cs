using GraphX.Common.Interfaces;
using GraphX.Measure;
using QuickGraph;
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
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        public IDictionary<TVertex, Rect> VertexSizes { get; set; }
        public IDictionary<TVertex, Point> VertexPositions { get; set; }
        public Rect AreaRectangle { get; set; }
        readonly Dictionary<TEdge, Point[]> _edgeRoutes = new Dictionary<TEdge, Point[]>();
        public IDictionary<TEdge, Point[]> EdgeRoutes { get { return _edgeRoutes; } }

        public IMutableBidirectionalGraph<TVertex, TEdge> Graph { get; set; }
        
        public void Compute(CancellationToken cancellationToken)
        {
            foreach (var item in Graph.Edges)
            {
                var source = item.Source;
                var srcPos = VertexPositions[source];

            }
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
