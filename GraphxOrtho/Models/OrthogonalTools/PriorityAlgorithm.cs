using GraphX.Common.Interfaces;
using GraphX.Logic.Algorithms.EdgeRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphxOrtho.Models.OrthogonalTools
{
    internal class PriorityAlgorithm<TVertex, TEdge> 
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex
    {
        PriorityPoint startPoint { get; set; }
        PriorityPoint destinationPoint { get; set; }
        OrthogonalVisibilityGraphMod<TVertex, TEdge> OrthogonalVisibilityGraph { get; set; }
        public PriorityAlgorithm(PriorityPoint startPoint, PriorityPoint destinationPoint, OrthogonalVisibilityGraphMod<TVertex, TEdge> orthogonalVisibilityGraph)
        {
            this.startPoint = startPoint;
            this.destinationPoint = destinationPoint;
            OrthogonalVisibilityGraph = orthogonalVisibilityGraph;
        }
        public List<PriorityPoint> CalculatePath()
        {
            // если конец совпадает с началом, заканчиваем алгоритм.
            if (startPoint.DireciontPoint.Point == destinationPoint.DireciontPoint.Point)
                return new List<PriorityPoint>() { startPoint };
            bool destinationReached = false;
            // Путь, для добавления 
            List<PriorityPoint> path = new List<PriorityPoint>();
            PriorityPoint currentPoint = startPoint;
            PriorityQueueB<PriorityPoint> priority = new PriorityQueueB<PriorityPoint>(new PriorityPointComparer());
            while (!destinationReached)
            {
                // получаем соседей
                if (currentPoint.DireciontPoint.Point == destinationPoint.DireciontPoint.Point)
                    break;
                var neighbours = OrthogonalVisibilityGraph.InitializeNeighbours(currentPoint);

                foreach (var neighbour in neighbours)
                {
                    neighbour.CalculateCost(destinationPoint);
                    priority.Push(neighbour);
                    if(neighbour.DireciontPoint.Point == destinationPoint.DireciontPoint.Point)
                    {
                        currentPoint = neighbour;
                        destinationReached = true;
                    }
                }
                if (destinationReached == true)
                    break;
                // cheapest vertex
                currentPoint = priority.Pop();
                if(currentPoint.DireciontPoint.Point == destinationPoint.DireciontPoint.Point)
                    destinationReached = true;
            }
            path.Add(currentPoint);
            var toAddInList = currentPoint.ParentPoint;
            while ( toAddInList != null)
            {
                path.Add(toAddInList);
                toAddInList = toAddInList.ParentPoint;
            }
            return path;
        }
    }
}
