using GraphX.Measure;
using GraphxOrtho.Models.AlgorithmTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphxOrtho.Models.OrthogonalTools
{
    internal class PriorityPointComparer : IComparer<PriorityPoint>
    {
        public int Compare(PriorityPoint source, PriorityPoint target)
        {
            if(source.Cost > target .Cost)
                return 1;
            if(source.Cost < target.Cost)
                return -1;
            if(source.ParentPoint == target.ParentPoint)
            {
                if (source.ParentPoint.DireciontPoint.Direction == source.DireciontPoint.Direction)
                {
                    return -1; 
                }
                else if (target.ParentPoint.DireciontPoint.Direction == target.DireciontPoint.Direction)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
