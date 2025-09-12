using Caskev.GridToolkit;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace GridToolkitTests
{
    public class Raycasting_Tests
    {
        TestTile[,] _grid;

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

            _grid = GridFactory.Build(map);
        }
        [TestCase(true)]
        [TestCase(false)]
        public void GetLineOfSight_InsideBounds_AllowDiagonals(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       ,
                null       , new(20, 14), null       ,
                null       , null       , new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false, includeStart);
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
        [TestCase(true)]
        [TestCase(false)]
        public void GetLineOfSight_InsideBounds_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(20, 13), null       ,
                null       , new(20, 14), new(21, 14),
                null       , null       , new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false, includeStart);
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
        [TestCase(true)]
        [TestCase(false)]
        public void GetLineOfSight_InsideBounds_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       ,
                new(19, 14), new(20, 14), null       ,
                null       , new(20, 15), new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true, includeStart);
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
        [TestCase(true)]
        [TestCase(false)]
        public void GetLineOfSight_OutsideBounds_AllowDiagonals(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(20, 14), new(21, 15),
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false, includeStart);
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
        [TestCase(true)]
        [TestCase(false)]
        public void GetLineOfSight_OutsideBounds_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(20, 13), null       ,
                null       , new(20, 14), new(21, 14),
                null       , null       , new(21, 15),
                null       , null       , null       ,
                null       , null       , null       ,
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false, includeStart);
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
        [TestCase(true)]
        [TestCase(false)]
        public void GetLineOfSight_OutsideBounds_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       ,
                new(19, 14), new(20, 14), null       ,
                null       , new(20, 15), new(21, 15),
                null       , null       , null       ,
                null       , null       , null       ,
            };
            TestTile[] extractedTiles = Raycasting.GetLineOfSight(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true, includeStart);
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

        [TestCase(true)]
        [TestCase(false)]
        public void GetConeOfVision_InsideBounds(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 270;
            int openingAngle = 90;
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
            TestTile[] extractedTiles = Raycasting.GetConeOfVision(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle, includeStart);
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
        [TestCase(true)]
        [TestCase(false)]
        public void GetConeOfVision_OutsideBounds(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 0;
            int directionAngle = 270;
            int openingAngle = 90;
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
            TestTile[] extractedTiles = Raycasting.GetConeOfVision(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle, includeStart);
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



        public void IsLineOfSightClear_InsideBounds_AllowDiagonals(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false));
        }
        [TestCase(true)]
        [TestCase(false)]
        public void IsLineOfSightClear_InsideBounds_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false));
        }
        [TestCase(true)]
        [TestCase(false)]
        public void IsLineOfSightClear_InsideBounds_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 7;
            int directionAngle = 45;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true));
        }
        [TestCase(true)]
        [TestCase(false)]
        public void IsLineOfSightClear_OutsideBounds_AllowDiagonals(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false));
        }
        [TestCase(true)]
        [TestCase(false)]
        public void IsLineOfSightClear_OutsideBounds_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false));
        }
        [TestCase(true)]
        [TestCase(false)]
        public void IsLineOfSightClear_OutsideBounds_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 19;
            int coordY = 13;
            int length = 0;
            int directionAngle = 45;
            Assert.IsFalse(Raycasting.IsLineOfSightClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsConeOfVisionClear_InsideBounds(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 270;
            int openingAngle = 90;
            Assert.IsFalse(Raycasting.IsConeOfVisionClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle));
        }
        [TestCase(true)]
        [TestCase(false)]
        public void IsConeOfVisionClear_OutsideBounds(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 0;
            int directionAngle = 270;
            int openingAngle = 90;
            Assert.IsFalse(Raycasting.IsConeOfVisionClear(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle));
        }
    }
}
