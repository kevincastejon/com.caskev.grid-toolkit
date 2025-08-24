using NUnit.Framework;
using UnityEngine;
using Caskev.GridToolkit;

namespace GridToolkitTests
{
    public class Raycasting_Tests
    {
        TestTile[,] rm;

        //[SetUp]
        //public void Setup()
        //{
        //    rm = GridFactory.Build(7, 7, MajorOrder.ROW_MAJOR_ORDER, (x, y) => !(x == 3 && y >= 0 && y <= 6));
        //}

        //[Test]
        //public void GetLineOfSight_AllOverloads_IsClearFalse_WhenBlocked()
        //{
        //    var start = GridUtils.GetTile(rm, 1, 3, MajorOrder.ROW_MAJOR_ORDER);
        //    var end = GridUtils.GetTile(rm, 5, 3, MajorOrder.ROW_MAJOR_ORDER);
        //    var los1 = Raycasting.GetLineOfSight(rm, start, end, includeStart: true, includeDestination: true, majorOrder: MajorOrder.ROW_MAJOR_ORDER);
        //    Assert.GreaterOrEqual(los1.Length, 2);
        //    var los2 = Raycasting.GetLineOfSight(rm, start, length: 6, directionAngle: 0f, includeStart: true, includeDestination: true, majorOrder: MajorOrder.ROW_MAJOR_ORDER);
        //    Assert.GreaterOrEqual(los2.Length, 2);
        //    var los3 = Raycasting.GetLineOfSight(rm, start, length: 6, direction: Vector2.right, includeStart: true, includeDestination: true, majorOrder: MajorOrder.ROW_MAJOR_ORDER);
        //    Assert.GreaterOrEqual(los3.Length, 2);
        //    var los4 = Raycasting.GetLineOfSight(rm, out bool clear1, start, end, includeStart: true, includeDestination: true, majorOrder: MajorOrder.ROW_MAJOR_ORDER);
        //    var los5 = Raycasting.GetLineOfSight(rm, out bool clear2, start, length: 6, directionAngle: 0f, includeStart: true, includeDestination: true, majorOrder: MajorOrder.ROW_MAJOR_ORDER);
        //    var los6 = Raycasting.GetLineOfSight(rm, out bool clear3, start, length: 6, direction: Vector2.right, includeStart: true, includeDestination: true, majorOrder: MajorOrder.ROW_MAJOR_ORDER);
        //    Assert.IsFalse(clear1);
        //    Assert.IsFalse(clear2);
        //    Assert.IsFalse(clear3);
        //    Assert.IsTrue(los4.Length >= 2 && los5.Length >= 2 && los6.Length >= 2);
        //}
    }
}
