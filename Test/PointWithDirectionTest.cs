using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GraphxOrtho.Models.Tools;

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
                Point = new System.Windows.Point(1.0,4.0) , 
                Direction = Direction.West
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new System.Windows.Point(4.0, 1.0),
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
                Point = new System.Windows.Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new System.Windows.Point(4.0, 1.0),
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
                Point = new System.Windows.Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new System.Windows.Point(4.0, 1.0),
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
                Point = new System.Windows.Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new System.Windows.Point(1.0, 1.0),
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
                Point = new System.Windows.Point(1.0, 4.0),
                Direction = Direction.East
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new System.Windows.Point(1.0, 1.0),
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
                Point = new System.Windows.Point(1.0, 4.0),
                Direction = Direction.West
            };
            PointWithDirection target = new PointWithDirection()
            {
                Point = new System.Windows.Point(4.0, 1.0),
                Direction = Direction.South
            };
            int boundsExpected = 3;

            //act
            int boundsActual = PointWithDirection.GetSdByTwoPoints(source, target);

            Assert.AreEqual(boundsExpected, boundsActual, "Bounds is not equal");
        }

    }
}
