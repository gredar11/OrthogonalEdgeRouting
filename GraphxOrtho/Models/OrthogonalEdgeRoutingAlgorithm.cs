using GraphX.Common.Interfaces;
using GraphX.Measure;
using GraphxOrtho.Models.OrthogonalTools;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
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
        public OrthogonalVisibilityGraphMod<TVertex, TEdge> OrthogonalVisibilityGraph { get; set; }
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

            foreach (var vertex in VertexSizes)
            {
                OvgVertices[vertex.Key] = new OvgVertex<TVertex>(vertex.Value, leftTopEndOfGraph, rightBottomEndOfGraph, 5.0);
            }
            foreach (var edge in Graph.Edges)
            {
                AddConnectionPoints(edge, leftTopEndOfGraph, rightBottomEndOfGraph);
            }
            OrthogonalVisibilityGraph = new OrthogonalVisibilityGraphMod<TVertex, TEdge>(OvgVertices.Values.ToList());
            foreach (var edge in Graph.Edges)
            {
                var ovgstart = OvgVertices[edge.Source];
                var ovgend = OvgVertices[edge.Target];
                var pathPoints = DrawOrthogonalEdge(this, edge, 
                    new Point( ovgstart.Position.X + ovgstart.SizeOfVertex.Width/2, ovgstart.Position.Y + ovgstart.SizeOfVertex.Height / 2),
                    new Point(ovgend.Position.X + ovgend.SizeOfVertex.Width/2, ovgend.Position.Y + ovgend.SizeOfVertex.Height / 2));
                List<Point> routingPathPoints = new List<Point>();
                foreach (var point in pathPoints)
                {
                    routingPathPoints.Add(point.DireciontPoint.Point);
                }
                if (EdgeRoutes.ContainsKey(edge))
                    EdgeRoutes[edge] = routingPathPoints.Count > 2 ? routingPathPoints.ToArray() : null;
                else EdgeRoutes.Add(edge, routingPathPoints.Count > 2 ? routingPathPoints.ToArray() : null);
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
            }
        }

        public Point[] ComputeSingle(TEdge edge)
        {
            return null;
        }

        public void UpdateVertexData(TVertex vertex, Point position, Rect size)
        {

        }
        private void AddConnectionPoints(TEdge edge, Point leftTopPoint, Point rightBotPoint)
        {
            var source = OvgVertices[edge.Source];
            var target = OvgVertices[edge.Target];
            GeometryAnalizator<TVertex, TEdge>.SetConnectionPointsToVerticesOfEdge(source, target, edge);
            source.SetConnectionEdges(leftTopPoint, rightBotPoint, edge);
            target.SetConnectionEdges(leftTopPoint, rightBotPoint, edge);
        }
        private List<PriorityPoint> DrawOrthogonalEdge(OrthogonalEdgeRoutingAlgorithm<TVertex, TEdge> algorithmBaseClass, TEdge edge, Point p1, Point p2)
        {
            var edgeToDraw = edge;
            var startVertex = algorithmBaseClass.OvgVertices[edgeToDraw.Source];
            var endVertex = algorithmBaseClass.OvgVertices[edgeToDraw.Target];
            var startPoint = startVertex.ConnectionPoints[edgeToDraw];
            var endPoint = endVertex.ConnectionPoints[edgeToDraw];
            var orthogonalVertices = algorithmBaseClass.OrthogonalVisibilityGraph.BiderectionalGraph.Vertices;

            var strartPointInAdjacecnyGraph = (from v in orthogonalVertices where v.Point == startPoint select v).FirstOrDefault();
            strartPointInAdjacecnyGraph.Direction = startVertex.GetDirectionOfPoint(strartPointInAdjacecnyGraph.Point, true);
            var endPointInAdjacecnyGraph = (from v in orthogonalVertices where v.Point == endPoint select v).FirstOrDefault();
            endPointInAdjacecnyGraph.Direction = endVertex.GetDirectionOfPoint(endPointInAdjacecnyGraph.Point, false);

            PriorityPoint start = new PriorityPoint(strartPointInAdjacecnyGraph, null);
            PriorityPoint end = new PriorityPoint(endPointInAdjacecnyGraph, null);
            PriorityAlgorithm<TVertex, TEdge> algorithm = new PriorityAlgorithm<TVertex, TEdge>(start, end, OrthogonalVisibilityGraph);
            PriorityPoint.DistanceFactor = 1.0;
            var path = algorithm.CalculatePath(new PriorityPoint(new AlgorithmTools.PointWithDirection() { Point=p1}, null), new PriorityPoint(new AlgorithmTools.PointWithDirection() { Point = p2 }, null));
            return path;
        }
    }
}
