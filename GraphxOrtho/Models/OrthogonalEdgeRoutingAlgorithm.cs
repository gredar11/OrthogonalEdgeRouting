using GraphX.Common.Interfaces;
using GraphX.Measure;
using GraphxOrtho.Models.OrthogonalTools;
using QuickGraph;
using System.Collections.Generic;
using System.Threading;

namespace GraphxOrtho.Models
{
    internal class OrthogonalEdgeRoutingAlgorithm<TVertex, TEdge> :
        IExternalEdgeRouting<TVertex, TEdge> 
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        public IDictionary<TVertex, Rect> VertexSizes { get; set; }
        public IDictionary<TVertex, Point> VertexPositions { get; set; }
        public IDictionary<TVertex, OvgVertex<TVertex>> OvgVertices { get; set; }
        public Rect AreaRectangle { get; set; }
        readonly Dictionary<TEdge, Point[]> _edgeRoutes = new Dictionary<TEdge, Point[]>();
        public IDictionary<TEdge, Point[]> EdgeRoutes { get { return _edgeRoutes; } }

        public IMutableBidirectionalGraph<TVertex, TEdge> Graph { get; set; }
        
        public void Compute(CancellationToken cancellationToken)
        {
            OvgVertices = new Dictionary<TVertex, OvgVertex<TVertex>>();
            Point leftTopEndOfGraph = new Point(0, 0);
            Point rightBottomEndOfGraph = new Point(0, 0);
            GetBorderAreaPoints(ref leftTopEndOfGraph, ref rightBottomEndOfGraph);

            foreach(var vertex in VertexSizes)
            {
                OvgVertices[vertex.Key] = new OvgVertex<TVertex>(vertex.Value, leftTopEndOfGraph, rightBottomEndOfGraph, 5.0);
            }
            foreach(var edge in Graph.Edges)
            {
                AddConnectionPoints(edge);
            }
        }

        private void GetBorderAreaPoints(ref Point leftTopEndOfGraph, ref Point rightBottomEndOfGraph)
        {
            foreach (var vertex in VertexSizes)
            {
                Point positionOfNode = vertex.Value.Location;
                if (positionOfNode.X + vertex.Value.Width > rightBottomEndOfGraph.X)
                    rightBottomEndOfGraph.X = positionOfNode.X + vertex.Value.Width;
                if (positionOfNode.X < leftTopEndOfGraph.X)
                    leftTopEndOfGraph.X = positionOfNode.X;
                if (positionOfNode.Y + vertex.Value.Height > rightBottomEndOfGraph.Y)
                    rightBottomEndOfGraph.Y = positionOfNode.Y + vertex.Value.Height;
                if (positionOfNode.Y < leftTopEndOfGraph.Y)
                    leftTopEndOfGraph.Y = positionOfNode.Y;
                // creating vertex with bounds and vertical + horizontal segments
            }
        }

        public Point[] ComputeSingle(TEdge edge)
        {
            return null;
        }

        public void UpdateVertexData(TVertex vertex, Point position, Rect size)
        {
            
        }
        private void AddConnectionPoints(TEdge edge)
        {
            var source = OvgVertices[edge.Source];
            var target = OvgVertices[edge.Target];
            GeometryAnalizator<TVertex,TEdge>.SetConnectionPointsToVerticesOfEdge(source, target, edge);

        }
    }
}
