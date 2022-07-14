using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphxOrtho.Models.AlgorithmTools
{
    public static class DebugHelper
    {
        /// <summary>
        /// Helps to visualize 2-dimensional arrays while debugging.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pad"></param>
        /// <returns></returns>
        public static string Test2D(this Array source, int pad = 10)
        {
            var result = "";
            for (int i = source.GetLowerBound(0); i <= source.GetUpperBound(0); i++)
            {
                for (int j = source.GetLowerBound(1); j <= source.GetUpperBound(1); j++)
                    result += source.GetValue(i, j).ToString().PadLeft(pad);
                result += "\n";
            }
            return result;
        }
    }
}
