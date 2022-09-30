using GraphX.Common.Interfaces;
using GraphX.Measure;
using GraphXOrthogonalEr.GeometryTools;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GraphXOrthogonalEr.AlgorithmTools
{
    public class OrthogonalEdgeRoutingAlgorithm<TVertex, TEdge> :
        IExternalEdgeRouting<TVertex, TEdge>
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        public IDictionary<TVertex, Rect> VertexSizes { get; set; }
        public IDictionary<TVertex, Point> VertexPositions { get; set; }
        public IDictionary<TVertex, OvgVertex<TVertex, TEdge>> OvgVertices { get; set; }
        public OrthogonalVisibilityGraph<TVertex, TEdge> OrthogonalVisibilityGraph { get; set; }
        public Rect AreaRectangle { get; set; }
        readonly Dictionary<TEdge, Point[]> _edgeRoutes = new Dictionary<TEdge, Point[]>();
        public IDictionary<TEdge, Point[]> EdgeRoutes { get { return _edgeRoutes; } }

        public IMutableBidirectionalGraph<TVertex, TEdge> Graph { get; set; }

        public void Compute(CancellationToken cancellationToken)
        {
            OvgVertices = new Dictionary<TVertex, OvgVertex<TVertex, TEdge>>();
            // setting borders for graph area
            Point leftTopEndOfGraph = new Point(0, 0);
            Point rightBottomEndOfGraph = new Point(0, 0);
            GetBorderAreaPoints(ref leftTopEndOfGraph, ref rightBottomEndOfGraph);
            // creating ovgVertices see [OvgVertex].
            foreach (var vertex in VertexSizes)
            {
                OvgVertices[vertex.Key] = new OvgVertex<TVertex, TEdge>(vertex.Value, leftTopEndOfGraph, rightBottomEndOfGraph, 5.0);
            }
            // Creating connection points for each edge with ovgVertices
            foreach (var edge in Graph.Edges)
            {
                AddConnectionPoints(edge, leftTopEndOfGraph, rightBottomEndOfGraph);
            }
            // Creating orthogonal visibility graph. See more in  M. Wybrow, K. Marriott, and P.J. Stuckey. Orthogonal connector routing.
            // In Proceedings of 17th International Symposium on Graph Drawing(GD '09), LNCS 5849, pages 219–231.Spring - Verlag, 2010. (https://www.adaptagrams.org/documentation/libavoid.html)
            OrthogonalVisibilityGraph = new OrthogonalVisibilityGraph<TVertex, TEdge>(OvgVertices.Values.ToList());
            // creating route for each of edge
            foreach (var edge in Graph.Edges)
            {
                List<Point> routingPathPoints = GetRoutePoints(edge);
                if (EdgeRoutes.ContainsKey(edge))
                    EdgeRoutes[edge] = routingPathPoints.Count > 2 ? routingPathPoints.ToArray() : null;
                else EdgeRoutes.Add(edge, routingPathPoints.Count > 2 ? routingPathPoints.ToArray() : null);
            }
        }

        private List<Point> GetRoutePoints(TEdge edge)
        {
            var pathPoints = DrawOrthogonalEdge(this, edge);
            List<Point> routingPathPoints = new List<Point>();
            foreach (var point in pathPoints)
            {
                routingPathPoints.Add(point.DireciontPoint.Point);
            }

            return routingPathPoints;
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
        private List<PriorityPoint> DrawOrthogonalEdge(OrthogonalEdgeRoutingAlgorithm<TVertex, TEdge> algorithmBaseClass, TEdge edge)
        {
            OvgVertex<TVertex, TEdge> startOvgVertex = algorithmBaseClass.OvgVertices[edge.Source];
            OvgVertex<TVertex, TEdge> endOvgVertex = algorithmBaseClass.OvgVertices[edge.Target];

            Point startPoint = startOvgVertex.ConnectionPoints[edge];
            Point endPoint = endOvgVertex.ConnectionPoints[edge];

            IEnumerable<PointWithDirection> orthogonalVertices = algorithmBaseClass.OrthogonalVisibilityGraph.BiderectionalGraph.Vertices;
            // initializing points with entrance/exit direction
            PointWithDirection strartPointInAdjacecnyGraph = (from v in orthogonalVertices where v.Point == startPoint select v).FirstOrDefault();
            strartPointInAdjacecnyGraph.Direction = startOvgVertex.GetDirectionOfPoint(strartPointInAdjacecnyGraph.Point, true);

            PointWithDirection endPointInAdjacecnyGraph = (from v in orthogonalVertices where v.Point == endPoint select v).FirstOrDefault();
            endPointInAdjacecnyGraph.Direction = endOvgVertex.GetDirectionOfPoint(endPointInAdjacecnyGraph.Point, false);

            PriorityPoint start = new PriorityPoint(strartPointInAdjacecnyGraph, null);
            PriorityPoint end = new PriorityPoint(endPointInAdjacecnyGraph, null);

            PriorityAlgorithm<TVertex, TEdge> algorithm = new PriorityAlgorithm<TVertex, TEdge>(start, end, OrthogonalVisibilityGraph);
            
            PriorityPoint.DistanceFactor = 1.0;

            return algorithm.CalculatePath();
        }
    }
}
