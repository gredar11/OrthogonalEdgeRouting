using System.Collections.Generic;

namespace GraphXOrthogonalEr.AlgorithmTools
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
            if (source.ParentPoint != null && target.ParentPoint != null) 
            {
                return source.ParentPoint.LengthOfPart > target.ParentPoint.LengthOfPart ? -1 : 1;
            }
            return 0;
        }
    }
}
