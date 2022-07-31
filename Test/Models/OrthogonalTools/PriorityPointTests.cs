using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphxOrtho.Models.OrthogonalTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphxOrtho.Models.OrthogonalTools.Tests
{
    [TestClass()]
    public class PriorityPointTests
    {
        [TestMethod()]
        public void ManhattanDistanceTest()
        {
            var actual = PriorityPoint.ManhattanDistance(new GraphX.Measure.Point(1,1), new GraphX.Measure.Point(4,9));
            var expected = 11.0;
            Assert.AreEqual(expected,actual);
        }
    }
}