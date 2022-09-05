using GraphX.Common.Interfaces;
using GraphX.Logic.Algorithms.EdgeRouting;
using System.Collections.Generic;

namespace GraphXOrthogonalEr.AlgorithmTools
{
    internal class PriorityAlgorithm<TVertex, TEdge> 
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        PriorityPoint startPoint { get; set; }
        PriorityPoint destinationPoint { get; set; }
        OrthogonalVisibilityGraph<TVertex, TEdge> OrthogonalVisibilityGraph { get; set; }
        public PriorityAlgorithm(PriorityPoint startPoint, PriorityPoint destinationPoint, OrthogonalVisibilityGraph<TVertex, TEdge> orthogonalVisibilityGraph)
        {
            this.startPoint = startPoint;
            this.destinationPoint = destinationPoint;
            OrthogonalVisibilityGraph = orthogonalVisibilityGraph;
        }
        public List<PriorityPoint> CalculatePath()
        {
            // if target and source are the same, return source.
            if (startPoint.DireciontPoint.Point == destinationPoint.DireciontPoint.Point)
                return new List<PriorityPoint>() { startPoint };
            bool destinationReached = false;
            // path 
            List<PriorityPoint> path = new List<PriorityPoint>();
            PriorityPoint currentPoint = startPoint;
            // queue
            PriorityQueueB<PriorityPoint> priority = new PriorityQueueB<PriorityPoint>(new PriorityPointComparer());
            while (!destinationReached)
            {
                // getting 'neighbours' of current vertex
                var neighbours = OrthogonalVisibilityGraph.InitializeNeighbours(currentPoint);
                AddNeighboursToQueue(ref destinationReached, ref currentPoint, priority, neighbours);
                if (destinationReached == true)
                    break;
                // cheapest vertex
                currentPoint = priority.Pop();
                if (currentPoint.DireciontPoint.Point == destinationPoint.DireciontPoint.Point)
                    destinationReached = true;
            }
            path.Add(destinationPoint);
            path.Add(currentPoint);
            var toAddInList = currentPoint.ParentPoint;
            while ( toAddInList != null)
            {
                path.Add(toAddInList);
                toAddInList = toAddInList.ParentPoint;
            }
            path.Add(startPoint);
            path.Reverse();
            return path;
        }

        private void AddNeighboursToQueue(ref bool destinationReached, ref PriorityPoint currentPoint, PriorityQueueB<PriorityPoint> priority, List<PriorityPoint> neighbours)
        {
            foreach (var neighbour in neighbours)
            {
                neighbour.CalculateCost(destinationPoint);
                bool neighbourIsnotInPq = true; // PQ - priority queue
                for (int i = 0; i < priority.Count; i++)
                {
                    if (priority[i].DireciontPoint == currentPoint.DireciontPoint && priority[i].Cost >= neighbour.Cost) // if pq contains neighbour and its cost less than old member update it
                    {
                        priority[i] = neighbour;
                        priority.Update(i);
                        neighbourIsnotInPq = false; // nbr is in Pq
                    }
                }
                if (neighbourIsnotInPq)
                    priority.Push(neighbour);
                if (neighbour.DireciontPoint.Point == destinationPoint.DireciontPoint.Point)
                {
                    currentPoint = neighbour;
                    destinationReached = true;
                }
            }
        }
    }
}
