using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Caskev.GridToolkit;

namespace GridToolkitTests
{
    public class Pathfinding_Tests
    {
        TestTile[,] _gridRowMajorOrder;
        TestTile[,] _gridColMajorOrder;

        [SetUp]
        public void Setup()
        {
            bool[,] map = new bool[20, 25] {
                { true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true  },
                { true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true  },
                { true , false, true , true , true , true , true , true , true , true , false, false, true , true , true , true , true , false, true , true , true , true , true , true , true  },
                { true , false, true , true , true , true , true , true , true , true , false, false, true , true , true , true , true , false, true , true , true , true , true , true , true  },
                { true , false, true , true , true , false, true , true , true , true , false, false, true , true , true , true , true , false, true , true , true , true , true , true , true  },
                { true , false, true , true , true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true  },
                { true , false, true , true , true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true  },
                { true , false, true , true , true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true  },
                { true , false, false, false, false, false, false, false, false, true , false, false, false, false, false, false, false, false, false, false, false, false, false, true , true  },
                { true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true  },
                { true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true  },
                { true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true , true , false, false, false, false, true , false, true , true  },
                { false, false, true , true , false, false, false, true , false, false, false, false, false, false, true , true , true , false, true , true , true , true , false, true , true  },
                { true , true , true , true , true , true , false, true , true , true , true , true , true , false, true , true , true , false, true , true , true , true , false, true , true  },
                { true , true , true , true , true , true , false, true , true , true , true , true , true , false, true , true , true , false, true , true , true , true , false, true , true  },
                { true , true , false, false, false, false, false, true , true , true , true , true , true , false, true , true , true , false, true , true , true , true , false, true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , false, false, false, false, false, false, true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true  },
            };

            _gridRowMajorOrder = GridFactory.Build(map, MajorOrder.ROW_MAJOR_ORDER);
            _gridColMajorOrder = GridFactory.Build(map, MajorOrder.COLUMN_MAJOR_ORDER);
        }

        //[Test]
        //public void GenerateDirectionMap_Sync_Covers_Basics()
        //{
        //    var target = GridUtils.GetTile(rm, 4, 4, MajorOrder.ROW_MAJOR_ORDER);
        //    var dm = Pathfinding.GenerateDirectionMap(rm, target, MajorOrder.ROW_MAJOR_ORDER);
        //    var start = GridUtils.GetTile(rm, 0, 0, MajorOrder.ROW_MAJOR_ORDER);
        //    Assert.IsTrue(dm.IsTileAccessible(rm, target));
        //    var backTarget = dm.GetTargetTile(rm);
        //    Assert.IsTrue(GridUtils.TileEquals(target, backTarget));
        //    var nextFromStart = dm.GetNextTileFromTile(rm, start);
        //    var dirFromStart = dm.GetNextTileDirectionFromTile(rm, start);
        //    Assert.IsNotNull(nextFromStart);
        //    Assert.AreNotEqual(NextTileDirection.NONE, dirFromStart);
        //    var toPath = dm.GetPathToTarget(rm, start, includeStart: true, includeTarget: true);
        //    var fromPath = dm.GetPathFromTarget(rm, start, includeDestination: true, includeTarget: true);
        //    Assert.IsTrue(toPath.Length >= 1);
        //    Assert.AreEqual(toPath.Length, fromPath.Length);
        //}

        //[Test]
        //public async Task GenerateDirectionMap_Async_And_Serialization_SyncAsync()
        //{
        //    var target = GridUtils.GetTile(rm, 4, 4, MajorOrder.ROW_MAJOR_ORDER);
        //    float lastProgress = 0f;
        //    var progress = new System.Progress<float>(p => lastProgress = p);
        //    using var cts = new CancellationTokenSource();
        //    var dm = await Pathfinding.GenerateDirectionMapAsync(rm, target, MajorOrder.ROW_MAJOR_ORDER, progress, cts.Token);
        //    Assert.GreaterOrEqual(lastProgress, 0f);
        //    var bytes = dm.ToByteArray();
        //    Assert.IsNotNull(bytes);
        //    var bytesAsync = await dm.ToByteArrayAsync(progress, cts.Token);
        //    Assert.IsNotNull(bytesAsync);
        //    Assert.Greater(bytesAsync.Length, 0);
        //    var dm2 = DirectionMap.FromByteArray(rm, bytes);
        //    var dm3 = await DirectionMap.FromByteArrayAsync(rm, bytesAsync, progress, cts.Token);
        //    var start = GridUtils.GetTile(rm, 0, 0, MajorOrder.ROW_MAJOR_ORDER);
        //    var p2 = dm2.GetPathToTarget(rm, start, includeStart: true, includeTarget: true);
        //    var p3 = dm3.GetPathToTarget(rm, start, includeStart: true, includeTarget: true);
        //    Assert.AreEqual(p2.Length, p3.Length);
        //}

        //[Test]
        //public void DirectionMap_Accessibility_Inaccessible_Start_When_WalledOff()
        //{
        //    var walled = GridFactory.Build(3, 3, MajorOrder.ROW_MAJOR_ORDER, (x, y) =>
        //    {
        //        if ((x, y) is (0, 1) or (1, 0) or (1, 1)) return false;
        //        return true;
        //    });
        //    var target = GridUtils.GetTile(walled, 2, 2, MajorOrder.ROW_MAJOR_ORDER);
        //    var dm = Pathfinding.GenerateDirectionMap(walled, target, MajorOrder.ROW_MAJOR_ORDER);
        //    var start = GridUtils.GetTile(walled, 0, 0, MajorOrder.ROW_MAJOR_ORDER);
        //    Assert.IsFalse(dm.IsTileAccessible(walled, start));
        //    Assert.IsTrue(dm.IsTileAccessible(walled, target));
        //}
    }
}
