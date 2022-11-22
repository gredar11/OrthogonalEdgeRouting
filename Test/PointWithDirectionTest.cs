using GraphX.Measure;
using GraphXOrthogonalEr.AlgorithmTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class PointWithDirectionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // arrange
            PointWithDirection source = new PointWithDirection() 
            { 
                Point = new Point(1.0,4.0) , 
                Direction = Direction.West
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new Point(4.0, 1.0),
                Direction = Direction.West
            };
            int boundsExpected = 4;

            //act
            int boundsActual = PointWithDirection.GetSdByTwoPoints(source, target);

            Assert.AreEqual(boundsExpected, boundsActual, "Bounds is not equal");
        }
        [TestMethod]
        public void TestMethod2()
        {
            // arrange
            PointWithDirection source = new PointWithDirection()
            {
                Point = new Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new Point(4.0, 1.0),
                Direction = Direction.East
            };
            int boundsExpected = 2;

            //act
            int boundsActual = PointWithDirection.GetSdByTwoPoints(source, target);

            Assert.AreEqual(boundsExpected, boundsActual, "Bounds is not equal");
        }
        [TestMethod]
        public void TestMethod3()
        {
            // arrange
            PointWithDirection source = new PointWithDirection()
            {
                Point = new Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new Point(4.0, 1.0),
                Direction = Direction.South
            };
            int boundsExpected = 1;

            //act
            int boundsActual = PointWithDirection.GetSdByTwoPoints(source, target);

            Assert.AreEqual(boundsExpected, boundsActual, "Bounds is not equal");
        }
        [TestMethod]
        public void TestMethod4()
        {
            // arrange
            PointWithDirection source = new PointWithDirection()
            {
                Point = new Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new Point(1.0, 1.0),
                Direction = Direction.East
            };
            int boundsExpected = 4;

            //act
            int boundsActual = PointWithDirection.GetSdByTwoPoints(source, target);

            Assert.AreEqual(boundsExpected, boundsActual, "Bounds is not equal");
        }
        [TestMethod]
        public void TestMethod5()
        {
            // arrange
            PointWithDirection source = new PointWithDirection()
            {
                Point = new Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new Point(1.0, 1.0),
                Direction = Direction.West
            };
            int boundsExpected = 2;

            //act
            int boundsActual = PointWithDirection.GetSdByTwoPoints(source, target);

            Assert.AreEqual(boundsExpected, boundsActual, "Bounds is not equal");
        }
        [TestMethod]
        public void TestMethod6()
        {
            // arrange
            PointWithDirection source = new PointWithDirection()
            {
                Point = new Point(1.0, 4.0),
                Direction = Direction.West
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new Point(4.0, 1.0),
                Direction = Direction.South
            };
            int boundsExpected = 3;

            //act
            int boundsActual = PointWithDirection.GetSdByTwoPoints(source, target);

            Assert.AreEqual(boundsExpected, boundsActual, "Bounds is not equal");
        }
        [TestMethod]
        public void GeneralMethod()
        {
            // arrange
            PointWithDirection source = new PointWithDirection()
            {
                Point = new Point(0.0, 0.0),
                Direction = Direction.North
            };
            double[,] points = new double[2, 8] { { -1, 0, 1, 1, 1, 0, -1, -1 }, { 1, 1, 1, 0, -1, -1, -1, 0 } };
            PointWithDirection[] targets = new PointWithDirection[8];
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = new PointWithDirection() { Point = new Point(points[0, i], points[1, i]), Direction = Direction.North };
            }
            PointWithDirection target = new PointWithDirection()
            {
                Point = new Point(4.0, 1.0),
                Direction = Direction.South
            };
            Direction[] directions = new Direction[] {Direction.North, Direction.East, Direction.South, Direction.West};
            // Matrix with calculated Sd
            int[,] expectedSdMatrix = new int[8, 4]
            {
                { 2, 3, 2, 1},
                { 0, 1, 4, 1},
                { 2, 1, 2, 3},
                { 4, 3, 2, 3},
                { 4, 3, 2, 3},
                { 4, 3, 4, 3},
                { 4, 3, 2, 3},
                { 4, 3, 2, 3},
            };
            int[,] actualMatrix = new int[8, 4];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    targets[i].Direction = directions[j];
                    actualMatrix[i, j] = PointWithDirection.GetSdByTwoPoints(source, targets[i]);
                    //assert
                    Assert.AreEqual(expectedSdMatrix[i,j], actualMatrix[i,j], "Bounds is not equal");
                }
            }
        }
        
    }
}
