using Caskev.GridToolkit;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace GridToolkitTests
{
    public class Raycasting_Tests
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
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetLineOfSight_InsideBounds_AllowDiagonals(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       ,
                null       , new(20, 14), null       ,
                null       , null       , new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, true, false, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetLineOfSight_InsideBounds_NoDiagonals_FavorHorizontal(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(20, 13), null       ,
                null       , new(20, 14), new(21, 14),
                null       , null       , new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, false, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetLineOfSight_InsideBounds_NoDiagonals_FavorVertical(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       ,
                new(19, 14), new(20, 14), null       ,
                null       , new(20, 15), new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, true, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetLineOfSight_OutsideBounds_AllowDiagonals(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(20, 14), new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, true, false, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetLineOfSight_OutsideBounds_NoDiagonals_FavorHorizontal(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(20, 13), null       ,
                null       , new(20, 14), new(21, 14),
                null       , null       , new(21, 15),
                null       , null       , null       ,
                null       , null       , null       ,
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, false, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetLineOfSight_OutsideBounds_NoDiagonals_FavorVertical(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       ,
                new(19, 14), new(20, 14), null       ,
                null       , new(20, 15), new(21, 15),
                null       , null       , null       ,
                null       , null       , null       ,
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, true, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }

        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetConeOfVision_InsideBounds(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 270;
            int openingAngle = 90;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , null      , null      , new(8, 4) , null      , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , new(8, 5) , null      , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , new(8, 6) , new(9, 6) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , null      , new(9, 7) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , null       , null       , null       ,
                null      , null      , null      , null      , new(9, 10), new(10, 10), new(11, 10), null       , null       , null       , null       ,
                null      , null      , null      , null      , null      , start      , null       , null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Raycasting.GetConeOfVision(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, openingAngle, directionAngle, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void GetConeOfVision_OutsideBounds(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 0;
            int directionAngle = 270;
            int openingAngle = 90;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , new(6, 0) , new(7, 0) , new(8, 0) , null      , null       , null       , null       , null       , null       , null       ,
                null      , new(6, 1) , new(7, 1) , new(8, 1) , null      , null       , null       , null       , null       , null       , null       ,
                null      , new(6, 2) , new(7, 2) , new(8, 2) , null      , null       , null       , null       , null       , null       , null       ,
                null      , null      , new(7, 3) , new(8, 3) , null      , null       , null       , null       , null       , null       , null       ,
                null      , null      , new(7, 4) , new(8, 4) , new(9, 4) , null       , null       , null       , null       , null       , null       ,
                null      , null      , new(7, 5) , new(8, 5) , new(9, 5) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , new(8, 6) , new(9, 6) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , new(8, 7) , new(9, 7) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , null       , null       , null       ,
                null      , null      , null      , null      , new(9, 10), new(10, 10), new(11, 10), null       , null       , null       , null       ,
                null      , null      , null      , null      , null      , start      , null       , null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Raycasting.GetConeOfVision(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, openingAngle, directionAngle, includeStart, majorOrder);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y));
            }
        }



        public void IsLineOfSightClear_InsideBounds_AllowDiagonals(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, true, false, majorOrder));
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void IsLineOfSightClear_InsideBounds_NoDiagonals_FavorHorizontal(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, false, majorOrder));
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void IsLineOfSightClear_InsideBounds_NoDiagonals_FavorVertical(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, true, majorOrder));
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void IsLineOfSightClear_OutsideBounds_AllowDiagonals(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, true, false, majorOrder));
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void IsLineOfSightClear_OutsideBounds_NoDiagonals_FavorHorizontal(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, false, majorOrder));
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void IsLineOfSightClear_OutsideBounds_NoDiagonals_FavorVertical(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, directionAngle, false, true, majorOrder));
        }

        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void IsConeOfVisionClear_InsideBounds(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 270;
            int openingAngle = 90;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsConeOfVisionClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, openingAngle, directionAngle, majorOrder));
        }
        [TestCase(true, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.ROW_MAJOR_ORDER)]
        [TestCase(true, MajorOrder.COLUMN_MAJOR_ORDER)]
        [TestCase(false, MajorOrder.COLUMN_MAJOR_ORDER)]
        public void IsConeOfVisionClear_OutsideBounds(bool includeStart, MajorOrder majorOrder)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 0;
            int directionAngle = 270;
            int openingAngle = 90;
            TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
            Assert.IsFalse(Raycasting.IsConeOfVisionClear(grid, GridUtils.GetTile(grid, coordX, coordY, majorOrder), length, openingAngle, directionAngle, majorOrder));
        }
    }
}
