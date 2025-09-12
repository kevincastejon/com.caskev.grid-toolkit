using Caskev.GridToolkit;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace GridToolkitTests
{
    public class Extraction_Tests
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
        public void GetTilesInARectangle_InsideBounds_IncludeWalls(bool includeCenter)
        {
            int coordX = 10;
            int coordY = 11;
            int rectWidth = 6;
            int rectHeight = 4;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(4, 7) , new(5, 7) , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) ,
                new(4, 8) , new(5, 8) , new(6, 8) , new(7, 8) , new(8, 8) , new(9, 8) , new(10, 8) , new(11, 8) , new(12, 8) , new(13, 8) , new(14, 8) , new(15, 8) , new(16, 8) ,
                new(4, 9) , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) ,
                new(4, 10), new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10),
                new(4, 11), new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), center     , new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11),
                new(4, 12), new(5, 12), new(6, 12), new(7, 12), new(8, 12), new(9, 12), new(10, 12), new(11, 12), new(12, 12), new(13, 12), new(14, 12), new(15, 12), new(16, 12),
                new(4, 13), new(5, 13), new(6, 13), new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), new(13, 13), new(14, 13), new(15, 13), new(16, 13),
                new(4, 14), new(5, 14), new(6, 14), new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), new(13, 14), new(14, 14), new(15, 14), new(16, 14),
                new(4, 15), new(5, 15), new(6, 15), new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), new(13, 15), new(14, 15), new(15, 15), new(16, 15),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInARectangle(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), includeCenter, true);
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
        public void GetTilesInARectangle_InsideBounds_ExcludeWalls(bool includeCenter)
        {
            int coordX = 10;
            int coordY = 11;
            int rectWidth = 6;
            int rectHeight = 4;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(4, 7) , null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) ,
                null      , null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       , null       ,
                null      , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) ,
                null      , new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10),
                null      , new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), center     , new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11),
                null      , null      , null      , new(7, 12), null      , null      , null       , null       , null       , null       , new(14, 12), new(15, 12), new(16, 12),
                new(4, 13), new(5, 13), null      , new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), null       , new(14, 13), new(15, 13), new(16, 13),
                new(4, 14), new(5, 14), null      , new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), null       , new(14, 14), new(15, 14), new(16, 14),
                null      , null      , null      , new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), null       , new(14, 15), new(15, 15), new(16, 15),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInARectangle(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), includeCenter, false);
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
        public void GetTilesInARectangle_OutsideBounds_IncludeWalls(bool includeCenter)
        {
            int coordX = 2;
            int coordY = 1;
            int rectWidth = 40;
            int rectHeight = 40;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(0, 0) , new(1, 0) , new(2, 0) , new(3, 0) , new(4, 0) , new(5, 0) , new(6, 0) , new(7, 0) , new(8, 0) , new(9, 0) , new(10, 0) , new(11, 0) , new(12, 0) , new(13, 0) , new(14, 0) , new(15, 0) , new(16, 0) , new(17, 0) , new(18, 0) , new(19, 0) , new(20, 0) , new(21, 0) , new(22, 0) , new(23, 0) , new(24, 0) ,
                new(0, 1) , new(1, 1) , center    , new(3, 1) , new(4, 1) , new(5, 1) , new(6, 1) , new(7, 1) , new(8, 1) , new(9, 1) , new(10, 1) , new(11, 1) , new(12, 1) , new(13, 1) , new(14, 1) , new(15, 1) , new(16, 1) , new(17, 1) , new(18, 1) , new(19, 1) , new(20, 1) , new(21, 1) , new(22, 1) , new(23, 1) , new(24, 1) ,
                new(0, 2) , new(1, 2) , new(2, 2) , new(3, 2) , new(4, 2) , new(5, 2) , new(6, 2) , new(7, 2) , new(8, 2) , new(9, 2) , new(10, 2) , new(11, 2) , new(12, 2) , new(13, 2) , new(14, 2) , new(15, 2) , new(16, 2) , new(17, 2) , new(18, 2) , new(19, 2) , new(20, 2) , new(21, 2) , new(22, 2) , new(23, 2) , new(24, 2) ,
                new(0, 3) , new(1, 3) , new(2, 3) , new(3, 3) , new(4, 3) , new(5, 3) , new(6, 3) , new(7, 3) , new(8, 3) , new(9, 3) , new(10, 3) , new(11, 3) , new(12, 3) , new(13, 3) , new(14, 3) , new(15, 3) , new(16, 3) , new(17, 3) , new(18, 3) , new(19, 3) , new(20, 3) , new(21, 3) , new(22, 3) , new(23, 3) , new(24, 3) ,
                new(0, 4) , new(1, 4) , new(2, 4) , new(3, 4) , new(4, 4) , new(5, 4) , new(6, 4) , new(7, 4) , new(8, 4) , new(9, 4) , new(10, 4) , new(11, 4) , new(12, 4) , new(13, 4) , new(14, 4) , new(15, 4) , new(16, 4) , new(17, 4) , new(18, 4) , new(19, 4) , new(20, 4) , new(21, 4) , new(22, 4) , new(23, 4) , new(24, 4) ,
                new(0, 5) , new(1, 5) , new(2, 5) , new(3, 5) , new(4, 5) , new(5, 5) , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , new(15, 5) , new(16, 5) , new(17, 5) , new(18, 5) , new(19, 5) , new(20, 5) , new(21, 5) , new(22, 5) , new(23, 5) , new(24, 5) ,
                new(0, 6) , new(1, 6) , new(2, 6) , new(3, 6) , new(4, 6) , new(5, 6) , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) , new(16, 6) , new(17, 6) , new(18, 6) , new(19, 6) , new(20, 6) , new(21, 6) , new(22, 6) , new(23, 6) , new(24, 6) ,
                new(0, 7) , new(1, 7) , new(2, 7) , new(3, 7) , new(4, 7) , new(5, 7) , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) , new(17, 7) , new(18, 7) , new(19, 7) , new(20, 7) , new(21, 7) , new(22, 7) , new(23, 7) , new(24, 7) ,
                new(0, 8) , new(1, 8) , new(2, 8) , new(3, 8) , new(4, 8) , new(5, 8) , new(6, 8) , new(7, 8) , new(8, 8) , new(9, 8) , new(10, 8) , new(11, 8) , new(12, 8) , new(13, 8) , new(14, 8) , new(15, 8) , new(16, 8) , new(17, 8) , new(18, 8) , new(19, 8) , new(20, 8) , new(21, 8) , new(22, 8) , new(23, 8) , new(24, 8) ,
                new(0, 9) , new(1, 9) , new(2, 9) , new(3, 9) , new(4, 9) , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) , new(17, 9) , new(18, 9) , new(19, 9) , new(20, 9) , new(21, 9) , new(22, 9) , new(23, 9) , new(24, 9) ,
                new(0, 10), new(1, 10), new(2, 10), new(3, 10), new(4, 10), new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10), new(17, 10), new(18, 10), new(19, 10), new(20, 10), new(21, 10), new(22, 10), new(23, 10), new(24, 10),
                new(0, 11), new(1, 11), new(2, 11), new(3, 11), new(4, 11), new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), new(10, 11), new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), new(17, 11), new(18, 11), new(19, 11), new(20, 11), new(21, 11), new(22, 11), new(23, 11), new(24, 11),
                new(0, 12), new(1, 12), new(2, 12), new(3, 12), new(4, 12), new(5, 12), new(6, 12), new(7, 12), new(8, 12), new(9, 12), new(10, 12), new(11, 12), new(12, 12), new(13, 12), new(14, 12), new(15, 12), new(16, 12), new(17, 12), new(18, 12), new(19, 12), new(20, 12), new(21, 12), new(22, 12), new(23, 12), new(24, 12),
                new(0, 13), new(1, 13), new(2, 13), new(3, 13), new(4, 13), new(5, 13), new(6, 13), new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), new(13, 13), new(14, 13), new(15, 13), new(16, 13), new(17, 13), new(18, 13), new(19, 13), new(20, 13), new(21, 13), new(22, 13), new(23, 13), new(24, 13),
                new(0, 14), new(1, 14), new(2, 14), new(3, 14), new(4, 14), new(5, 14), new(6, 14), new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), new(13, 14), new(14, 14), new(15, 14), new(16, 14), new(17, 14), new(18, 14), new(19, 14), new(20, 14), new(21, 14), new(22, 14), new(23, 14), new(24, 14),
                new(0, 15), new(1, 15), new(2, 15), new(3, 15), new(4, 15), new(5, 15), new(6, 15), new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), new(13, 15), new(14, 15), new(15, 15), new(16, 15), new(17, 15), new(18, 15), new(19, 15), new(20, 15), new(21, 15), new(22, 15), new(23, 15), new(24, 15),
                new(0, 16), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), new(13, 16), new(14, 16), new(15, 16), new(16, 16), new(17, 16), new(18, 16), new(19, 16), new(20, 16), new(21, 16), new(22, 16), new(23, 16), new(24, 16),
                new(0, 17), new(1, 17), new(2, 17), new(3, 17), new(4, 17), new(5, 17), new(6, 17), new(7, 17), new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), new(13, 17), new(14, 17), new(15, 17), new(16, 17), new(17, 17), new(18, 17), new(19, 17), new(20, 17), new(21, 17), new(22, 17), new(23, 17), new(24, 17),
                new(0, 18), new(1, 18), new(2, 18), new(3, 18), new(4, 18), new(5, 18), new(6, 18), new(7, 18), new(8, 18), new(9, 18), new(10, 18), new(11, 18), new(12, 18), new(13, 18), new(14, 18), new(15, 18), new(16, 18), new(17, 18), new(18, 18), new(19, 18), new(20, 18), new(21, 18), new(22, 18), new(23, 18), new(24, 18),
                new(0, 19), new(1, 19), new(2, 19), new(3, 19), new(4, 19), new(5, 19), new(6, 19), new(7, 19), new(8, 19), new(9, 19), new(10, 19), new(11, 19), new(12, 19), new(13, 19), new(14, 19), new(15, 19), new(16, 19), new(17, 19), new(18, 19), new(19, 19), new(20, 19), new(21, 19), new(22, 19), new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInARectangle(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), includeCenter, true);
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
        public void GetTilesInARectangle_OutsideBounds_ExcludeWalls(bool includeCenter)
        {
            int coordX = 2;
            int coordY = 1;
            int rectWidth = 40;
            int rectHeight = 40;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(0, 0) , null      , new(2, 0) , new(3, 0) , new(4, 0) , new(5, 0) , new(6, 0) , new(7, 0) , new(8, 0) , new(9, 0) , new(10, 0) , new(11, 0) , new(12, 0) , new(13, 0) , new(14, 0) , new(15, 0) , new(16, 0) , null       , new(18, 0) , new(19, 0) , new(20, 0) , new(21, 0) , new(22, 0) , new(23, 0) , new(24, 0) ,
                new(0, 1) , null      , center    , new(3, 1) , new(4, 1) , new(5, 1) , new(6, 1) , new(7, 1) , new(8, 1) , new(9, 1) , new(10, 1) , new(11, 1) , new(12, 1) , new(13, 1) , new(14, 1) , new(15, 1) , new(16, 1) , null       , new(18, 1) , new(19, 1) , new(20, 1) , new(21, 1) , new(22, 1) , new(23, 1) , new(24, 1) ,
                new(0, 2) , null      , new(2, 2) , new(3, 2) , new(4, 2) , new(5, 2) , new(6, 2) , new(7, 2) , new(8, 2) , new(9, 2) , null       , null       , new(12, 2) , new(13, 2) , new(14, 2) , new(15, 2) , new(16, 2) , null       , new(18, 2) , new(19, 2) , new(20, 2) , new(21, 2) , new(22, 2) , new(23, 2) , new(24, 2) ,
                new(0, 3) , null      , new(2, 3) , new(3, 3) , new(4, 3) , new(5, 3) , new(6, 3) , new(7, 3) , new(8, 3) , new(9, 3) , null       , null       , new(12, 3) , new(13, 3) , new(14, 3) , new(15, 3) , new(16, 3) , null       , new(18, 3) , new(19, 3) , new(20, 3) , new(21, 3) , new(22, 3) , new(23, 3) , new(24, 3) ,
                new(0, 4) , null      , new(2, 4) , new(3, 4) , new(4, 4) , null      , new(6, 4) , new(7, 4) , new(8, 4) , new(9, 4) , null       , null       , new(12, 4) , new(13, 4) , new(14, 4) , new(15, 4) , new(16, 4) , null       , new(18, 4) , new(19, 4) , new(20, 4) , new(21, 4) , new(22, 4) , new(23, 4) , new(24, 4) ,
                new(0, 5) , null      , new(2, 5) , new(3, 5) , new(4, 5) , null      , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , new(15, 5) , new(16, 5) , new(17, 5) , new(18, 5) , new(19, 5) , new(20, 5) , new(21, 5) , new(22, 5) , new(23, 5) , new(24, 5) ,
                new(0, 6) , null      , new(2, 6) , new(3, 6) , new(4, 6) , null      , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) , new(16, 6) , new(17, 6) , new(18, 6) , new(19, 6) , new(20, 6) , new(21, 6) , new(22, 6) , new(23, 6) , new(24, 6) ,
                new(0, 7) , null      , new(2, 7) , new(3, 7) , new(4, 7) , null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) , new(17, 7) , new(18, 7) , new(19, 7) , new(20, 7) , new(21, 7) , new(22, 7) , new(23, 7) , new(24, 7) ,
                new(0, 8) , null      , null      , null      , null      , null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , new(23, 8) , new(24, 8) ,
                new(0, 9) , new(1, 9) , new(2, 9) , new(3, 9) , null      , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) , new(17, 9) , new(18, 9) , new(19, 9) , new(20, 9) , new(21, 9) , null       , new(23, 9) , new(24, 9) ,
                new(0, 10), new(1, 10), new(2, 10), new(3, 10), null      , new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10), new(17, 10), new(18, 10), new(19, 10), new(20, 10), new(21, 10), null       , new(23, 10), new(24, 10),
                new(0, 11), new(1, 11), new(2, 11), new(3, 11), null      , new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), new(10, 11), new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), null       , null       , null       , null       , new(21, 11), null       , new(23, 11), new(24, 11),
                null      , null      , new(2, 12), new(3, 12), null      , null      , null      , new(7, 12), null      , null      , null       , null       , null       , null       , new(14, 12), new(15, 12), new(16, 12), null       , new(18, 12), new(19, 12), new(20, 12), new(21, 12), null       , new(23, 12), new(24, 12),
                new(0, 13), new(1, 13), new(2, 13), new(3, 13), new(4, 13), new(5, 13), null      , new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), null       , new(14, 13), new(15, 13), new(16, 13), null       , new(18, 13), new(19, 13), new(20, 13), new(21, 13), null       , new(23, 13), new(24, 13),
                new(0, 14), new(1, 14), new(2, 14), new(3, 14), new(4, 14), new(5, 14), null      , new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), null       , new(14, 14), new(15, 14), new(16, 14), null       , new(18, 14), new(19, 14), new(20, 14), new(21, 14), null       , new(23, 14), new(24, 14),
                new(0, 15), new(1, 15), null      , null      , null      , null      , null      , new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), null       , new(14, 15), new(15, 15), new(16, 15), null       , new(18, 15), new(19, 15), new(20, 15), new(21, 15), null       , new(23, 15), new(24, 15),
                new(0, 16), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), null       , new(14, 16), new(15, 16), new(16, 16), null       , null       , null       , null       , null       , null       , new(23, 16), new(24, 16),
                new(0, 17), new(1, 17), new(2, 17), new(3, 17), new(4, 17), new(5, 17), new(6, 17), new(7, 17), new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), null       , new(14, 17), new(15, 17), new(16, 17), new(17, 17), new(18, 17), new(19, 17), new(20, 17), new(21, 17), new(22, 17), new(23, 17), new(24, 17),
                new(0, 18), new(1, 18), new(2, 18), new(3, 18), new(4, 18), new(5, 18), new(6, 18), new(7, 18), new(8, 18), new(9, 18), new(10, 18), new(11, 18), new(12, 18), null       , new(14, 18), new(15, 18), new(16, 18), new(17, 18), new(18, 18), new(19, 18), new(20, 18), new(21, 18), new(22, 18), new(23, 18), new(24, 18),
                new(0, 19), new(1, 19), new(2, 19), new(3, 19), new(4, 19), new(5, 19), new(6, 19), new(7, 19), new(8, 19), new(9, 19), new(10, 19), new(11, 19), new(12, 19), null       , new(14, 19), new(15, 19), new(16, 19), new(17, 19), new(18, 19), new(19, 19), new(20, 19), new(21, 19), new(22, 19), new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInARectangle(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), includeCenter, false);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y), $"Tile {expectedTiles[i]} has not been found into the extracted tiles");
            }
        }


        [Test]
        public void GetTilesInARectangleOutline_InsideBounds_IncludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int rectWidth = 6;
            int rectHeight = 4;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(4, 7) , new(5, 7) , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) ,
                new(4, 8) , null      , null      , null      , null      , null      , null      , null        , null       , null       , null       , null       , new(16, 8) ,
                new(4, 9) , null      , null      , null      , null      , null      , null      , null        , null       , null       , null       , null       , new(16, 9) ,
                new(4, 10), null      , null      , null      , null      , null      , null      , null        , null       , null       , null       , null       , new(16, 10),
                new(4, 11), null      , null      , null      , null      , null      , null      , null        , null       , null       , null       , null       , new(16, 11),
                new(4, 12), null      , null      , null      , null      , null      , null      , null        , null       , null       , null       , null       , new(16, 12),
                new(4, 13), null      , null      , null      , null      , null      , null      , null        , null       , null       , null       , null       , new(16, 13),
                new(4, 14), null      , null      , null      , null      , null      , null      , null        , null       , null       , null       , null       , new(16, 14),
                new(4, 15), new(5, 15), new(6, 15), new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), new(13, 15), new(14, 15), new(15, 15), new(16, 15),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnARectangleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), true);
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
        [Test]
        public void GetTilesInARectangleOutline_InsideBounds_ExcludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int rectWidth = 6;
            int rectHeight = 4;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(4, 7) , null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 9) ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 10),
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 11),
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 12),
                new(4, 13), null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 13),
                new(4, 14), null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 14),
                null      , null      , null      , new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), null       , new(14, 15), new(15, 15), new(16, 15),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnARectangleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), false);
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

        [Test]
        public void GetTilesInARectangleOutline_OutsideBounds_IncludeWalls()
        {
            int coordX = 2;
            int coordY = 1;
            int rectWidth = 40;
            int rectHeight = 40;
            Vector2Int?[] expectedTiles = new Vector2Int?[0];
            TestTile[] extractedTiles = Extraction.GetTilesOnARectangleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), true);
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
        [Test]
        public void GetTilesInARectangleOutline_OutsideBounds_ExcludeWalls()
        {
            int coordX = 2;
            int coordY = 1;
            int rectWidth = 40;
            int rectHeight = 40;
            Vector2Int?[] expectedTiles = new Vector2Int?[0];
            TestTile[] extractedTiles = Extraction.GetTilesOnARectangleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), new Vector2Int(rectWidth, rectHeight), false);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y), $"Tile {expectedTiles[i]} has not been found into the extracted tiles");
            }
        }



        [TestCase(true)]
        [TestCase(false)]
        public void GetTilesInACircle_InsideBounds_IncludeWalls(bool includeCenter)
        {
            int coordX = 10;
            int coordY = 11;
            int radius = 6;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , null      , null      , null      , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , null       , null       , null       , null       ,
                null      , null      , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , null       , null       ,
                null      , new(5, 7) , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , null       ,
                null      , new(5, 8) , new(6, 8) , new(7, 8) , new(8, 8) , new(9, 8) , new(10, 8) , new(11, 8) , new(12, 8) , new(13, 8) , new(14, 8) , new(15, 8) , null       ,
                new(4, 9) , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) ,
                new(4, 10), new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10),
                new(4, 11), new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), center     , new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11),
                new(4, 12), new(5, 12), new(6, 12), new(7, 12), new(8, 12), new(9, 12), new(10, 12), new(11, 12), new(12, 12), new(13, 12), new(14, 12), new(15, 12), new(16, 12),
                new(4, 13), new(5, 13), new(6, 13), new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), new(13, 13), new(14, 13), new(15, 13), new(16, 13),
                null      , new(5, 14), new(6, 14), new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), new(13, 14), new(14, 14), new(15, 14), null       ,
                null      , new(5, 15), new(6, 15), new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), new(13, 15), new(14, 15), new(15, 15), null       ,
                null      , null      , new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), new(13, 16), new(14, 16), null       , null       ,
                null      , null      , null      , null      , new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACircle(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, includeCenter, true);
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
        public void GetTilesInACircle_InsideBounds_ExcludeWalls(bool includeCenter)
        {
            int coordX = 10;
            int coordY = 11;
            int radius = 6;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , null      , null      , null      , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , null       , null       , null       , null       ,
                null      , null      , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , null       , null       ,
                null      , null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , null       ,
                null      , null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       , null       ,
                null      , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) ,
                null      , new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10),
                null      , new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), center     , new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11),
                null      , null      , null      , new(7, 12), null      , null      , null       , null       , null       , null       , new(14, 12), new(15, 12), new(16, 12),
                new(4, 13), new(5, 13), null      , new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), null       , new(14, 13), new(15, 13), new(16, 13),
                null      , new(5, 14), null      , new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), null       , new(14, 14), new(15, 14), null       ,
                null      , null      , null      , new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), null       , new(14, 15), new(15, 15), null       ,
                null      , null      , new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), null       , new(14, 16), null       , null       ,
                null      , null      , null      , null      , new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACircle(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, includeCenter, false);
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
        public void GetTilesInACircle_OutsideBounds_IncludeWalls(bool includeCenter)
        {
            int coordX = 2;
            int coordY = 1;
            int radius = 40;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(0, 0) , new(1, 0) , new(2, 0) , new(3, 0) , new(4, 0) , new(5, 0) , new(6, 0) , new(7, 0) , new(8, 0) , new(9, 0) , new(10, 0) , new(11, 0) , new(12, 0) , new(13, 0) , new(14, 0) , new(15, 0) , new(16, 0) , new(17, 0) , new(18, 0) , new(19, 0) , new(20, 0) , new(21, 0) , new(22, 0) , new(23, 0) , new(24, 0) ,
                new(0, 1) , new(1, 1) , center    , new(3, 1) , new(4, 1) , new(5, 1) , new(6, 1) , new(7, 1) , new(8, 1) , new(9, 1) , new(10, 1) , new(11, 1) , new(12, 1) , new(13, 1) , new(14, 1) , new(15, 1) , new(16, 1) , new(17, 1) , new(18, 1) , new(19, 1) , new(20, 1) , new(21, 1) , new(22, 1) , new(23, 1) , new(24, 1) ,
                new(0, 2) , new(1, 2) , new(2, 2) , new(3, 2) , new(4, 2) , new(5, 2) , new(6, 2) , new(7, 2) , new(8, 2) , new(9, 2) , new(10, 2) , new(11, 2) , new(12, 2) , new(13, 2) , new(14, 2) , new(15, 2) , new(16, 2) , new(17, 2) , new(18, 2) , new(19, 2) , new(20, 2) , new(21, 2) , new(22, 2) , new(23, 2) , new(24, 2) ,
                new(0, 3) , new(1, 3) , new(2, 3) , new(3, 3) , new(4, 3) , new(5, 3) , new(6, 3) , new(7, 3) , new(8, 3) , new(9, 3) , new(10, 3) , new(11, 3) , new(12, 3) , new(13, 3) , new(14, 3) , new(15, 3) , new(16, 3) , new(17, 3) , new(18, 3) , new(19, 3) , new(20, 3) , new(21, 3) , new(22, 3) , new(23, 3) , new(24, 3) ,
                new(0, 4) , new(1, 4) , new(2, 4) , new(3, 4) , new(4, 4) , new(5, 4) , new(6, 4) , new(7, 4) , new(8, 4) , new(9, 4) , new(10, 4) , new(11, 4) , new(12, 4) , new(13, 4) , new(14, 4) , new(15, 4) , new(16, 4) , new(17, 4) , new(18, 4) , new(19, 4) , new(20, 4) , new(21, 4) , new(22, 4) , new(23, 4) , new(24, 4) ,
                new(0, 5) , new(1, 5) , new(2, 5) , new(3, 5) , new(4, 5) , new(5, 5) , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , new(15, 5) , new(16, 5) , new(17, 5) , new(18, 5) , new(19, 5) , new(20, 5) , new(21, 5) , new(22, 5) , new(23, 5) , new(24, 5) ,
                new(0, 6) , new(1, 6) , new(2, 6) , new(3, 6) , new(4, 6) , new(5, 6) , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) , new(16, 6) , new(17, 6) , new(18, 6) , new(19, 6) , new(20, 6) , new(21, 6) , new(22, 6) , new(23, 6) , new(24, 6) ,
                new(0, 7) , new(1, 7) , new(2, 7) , new(3, 7) , new(4, 7) , new(5, 7) , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) , new(17, 7) , new(18, 7) , new(19, 7) , new(20, 7) , new(21, 7) , new(22, 7) , new(23, 7) , new(24, 7) ,
                new(0, 8) , new(1, 8) , new(2, 8) , new(3, 8) , new(4, 8) , new(5, 8) , new(6, 8) , new(7, 8) , new(8, 8) , new(9, 8) , new(10, 8) , new(11, 8) , new(12, 8) , new(13, 8) , new(14, 8) , new(15, 8) , new(16, 8) , new(17, 8) , new(18, 8) , new(19, 8) , new(20, 8) , new(21, 8) , new(22, 8) , new(23, 8) , new(24, 8) ,
                new(0, 9) , new(1, 9) , new(2, 9) , new(3, 9) , new(4, 9) , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) , new(17, 9) , new(18, 9) , new(19, 9) , new(20, 9) , new(21, 9) , new(22, 9) , new(23, 9) , new(24, 9) ,
                new(0, 10), new(1, 10), new(2, 10), new(3, 10), new(4, 10), new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10), new(17, 10), new(18, 10), new(19, 10), new(20, 10), new(21, 10), new(22, 10), new(23, 10), new(24, 10),
                new(0, 11), new(1, 11), new(2, 11), new(3, 11), new(4, 11), new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), new(10, 11), new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), new(17, 11), new(18, 11), new(19, 11), new(20, 11), new(21, 11), new(22, 11), new(23, 11), new(24, 11),
                new(0, 12), new(1, 12), new(2, 12), new(3, 12), new(4, 12), new(5, 12), new(6, 12), new(7, 12), new(8, 12), new(9, 12), new(10, 12), new(11, 12), new(12, 12), new(13, 12), new(14, 12), new(15, 12), new(16, 12), new(17, 12), new(18, 12), new(19, 12), new(20, 12), new(21, 12), new(22, 12), new(23, 12), new(24, 12),
                new(0, 13), new(1, 13), new(2, 13), new(3, 13), new(4, 13), new(5, 13), new(6, 13), new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), new(13, 13), new(14, 13), new(15, 13), new(16, 13), new(17, 13), new(18, 13), new(19, 13), new(20, 13), new(21, 13), new(22, 13), new(23, 13), new(24, 13),
                new(0, 14), new(1, 14), new(2, 14), new(3, 14), new(4, 14), new(5, 14), new(6, 14), new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), new(13, 14), new(14, 14), new(15, 14), new(16, 14), new(17, 14), new(18, 14), new(19, 14), new(20, 14), new(21, 14), new(22, 14), new(23, 14), new(24, 14),
                new(0, 15), new(1, 15), new(2, 15), new(3, 15), new(4, 15), new(5, 15), new(6, 15), new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), new(13, 15), new(14, 15), new(15, 15), new(16, 15), new(17, 15), new(18, 15), new(19, 15), new(20, 15), new(21, 15), new(22, 15), new(23, 15), new(24, 15),
                new(0, 16), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), new(13, 16), new(14, 16), new(15, 16), new(16, 16), new(17, 16), new(18, 16), new(19, 16), new(20, 16), new(21, 16), new(22, 16), new(23, 16), new(24, 16),
                new(0, 17), new(1, 17), new(2, 17), new(3, 17), new(4, 17), new(5, 17), new(6, 17), new(7, 17), new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), new(13, 17), new(14, 17), new(15, 17), new(16, 17), new(17, 17), new(18, 17), new(19, 17), new(20, 17), new(21, 17), new(22, 17), new(23, 17), new(24, 17),
                new(0, 18), new(1, 18), new(2, 18), new(3, 18), new(4, 18), new(5, 18), new(6, 18), new(7, 18), new(8, 18), new(9, 18), new(10, 18), new(11, 18), new(12, 18), new(13, 18), new(14, 18), new(15, 18), new(16, 18), new(17, 18), new(18, 18), new(19, 18), new(20, 18), new(21, 18), new(22, 18), new(23, 18), new(24, 18),
                new(0, 19), new(1, 19), new(2, 19), new(3, 19), new(4, 19), new(5, 19), new(6, 19), new(7, 19), new(8, 19), new(9, 19), new(10, 19), new(11, 19), new(12, 19), new(13, 19), new(14, 19), new(15, 19), new(16, 19), new(17, 19), new(18, 19), new(19, 19), new(20, 19), new(21, 19), new(22, 19), new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACircle(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, includeCenter, true);
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
        public void GetTilesInACircle_OutsideBounds_ExcludeWalls(bool includeCenter)
        {
            int coordX = 2;
            int coordY = 1;
            int radius = 40;
            Vector2Int? center = includeCenter ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(0, 0) , null      , new(2, 0) , new(3, 0) , new(4, 0) , new(5, 0) , new(6, 0) , new(7, 0) , new(8, 0) , new(9, 0) , new(10, 0) , new(11, 0) , new(12, 0) , new(13, 0) , new(14, 0) , new(15, 0) , new(16, 0) , null       , new(18, 0) , new(19, 0) , new(20, 0) , new(21, 0) , new(22, 0) , new(23, 0) , new(24, 0) ,
                new(0, 1) , null      , center    , new(3, 1) , new(4, 1) , new(5, 1) , new(6, 1) , new(7, 1) , new(8, 1) , new(9, 1) , new(10, 1) , new(11, 1) , new(12, 1) , new(13, 1) , new(14, 1) , new(15, 1) , new(16, 1) , null       , new(18, 1) , new(19, 1) , new(20, 1) , new(21, 1) , new(22, 1) , new(23, 1) , new(24, 1) ,
                new(0, 2) , null      , new(2, 2) , new(3, 2) , new(4, 2) , new(5, 2) , new(6, 2) , new(7, 2) , new(8, 2) , new(9, 2) , null       , null       , new(12, 2) , new(13, 2) , new(14, 2) , new(15, 2) , new(16, 2) , null       , new(18, 2) , new(19, 2) , new(20, 2) , new(21, 2) , new(22, 2) , new(23, 2) , new(24, 2) ,
                new(0, 3) , null      , new(2, 3) , new(3, 3) , new(4, 3) , new(5, 3) , new(6, 3) , new(7, 3) , new(8, 3) , new(9, 3) , null       , null       , new(12, 3) , new(13, 3) , new(14, 3) , new(15, 3) , new(16, 3) , null       , new(18, 3) , new(19, 3) , new(20, 3) , new(21, 3) , new(22, 3) , new(23, 3) , new(24, 3) ,
                new(0, 4) , null      , new(2, 4) , new(3, 4) , new(4, 4) , null      , new(6, 4) , new(7, 4) , new(8, 4) , new(9, 4) , null       , null       , new(12, 4) , new(13, 4) , new(14, 4) , new(15, 4) , new(16, 4) , null       , new(18, 4) , new(19, 4) , new(20, 4) , new(21, 4) , new(22, 4) , new(23, 4) , new(24, 4) ,
                new(0, 5) , null      , new(2, 5) , new(3, 5) , new(4, 5) , null      , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , new(15, 5) , new(16, 5) , new(17, 5) , new(18, 5) , new(19, 5) , new(20, 5) , new(21, 5) , new(22, 5) , new(23, 5) , new(24, 5) ,
                new(0, 6) , null      , new(2, 6) , new(3, 6) , new(4, 6) , null      , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) , new(16, 6) , new(17, 6) , new(18, 6) , new(19, 6) , new(20, 6) , new(21, 6) , new(22, 6) , new(23, 6) , new(24, 6) ,
                new(0, 7) , null      , new(2, 7) , new(3, 7) , new(4, 7) , null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) , new(17, 7) , new(18, 7) , new(19, 7) , new(20, 7) , new(21, 7) , new(22, 7) , new(23, 7) , new(24, 7) ,
                new(0, 8) , null      , null      , null      , null      , null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , new(23, 8) , new(24, 8) ,
                new(0, 9) , new(1, 9) , new(2, 9) , new(3, 9) , null      , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) , new(17, 9) , new(18, 9) , new(19, 9) , new(20, 9) , new(21, 9) , null       , new(23, 9) , new(24, 9) ,
                new(0, 10), new(1, 10), new(2, 10), new(3, 10), null      , new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10), new(17, 10), new(18, 10), new(19, 10), new(20, 10), new(21, 10), null       , new(23, 10), new(24, 10),
                new(0, 11), new(1, 11), new(2, 11), new(3, 11), null      , new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), new(10, 11), new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), null       , null       , null       , null       , new(21, 11), null       , new(23, 11), new(24, 11),
                null      , null      , new(2, 12), new(3, 12), null      , null      , null      , new(7, 12), null      , null      , null       , null       , null       , null       , new(14, 12), new(15, 12), new(16, 12), null       , new(18, 12), new(19, 12), new(20, 12), new(21, 12), null       , new(23, 12), new(24, 12),
                new(0, 13), new(1, 13), new(2, 13), new(3, 13), new(4, 13), new(5, 13), null      , new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), null       , new(14, 13), new(15, 13), new(16, 13), null       , new(18, 13), new(19, 13), new(20, 13), new(21, 13), null       , new(23, 13), new(24, 13),
                new(0, 14), new(1, 14), new(2, 14), new(3, 14), new(4, 14), new(5, 14), null      , new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), null       , new(14, 14), new(15, 14), new(16, 14), null       , new(18, 14), new(19, 14), new(20, 14), new(21, 14), null       , new(23, 14), new(24, 14),
                new(0, 15), new(1, 15), null      , null      , null      , null      , null      , new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), null       , new(14, 15), new(15, 15), new(16, 15), null       , new(18, 15), new(19, 15), new(20, 15), new(21, 15), null       , new(23, 15), new(24, 15),
                new(0, 16), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), null       , new(14, 16), new(15, 16), new(16, 16), null       , null       , null       , null       , null       , null       , new(23, 16), new(24, 16),
                new(0, 17), new(1, 17), new(2, 17), new(3, 17), new(4, 17), new(5, 17), new(6, 17), new(7, 17), new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), null       , new(14, 17), new(15, 17), new(16, 17), new(17, 17), new(18, 17), new(19, 17), new(20, 17), new(21, 17), new(22, 17), new(23, 17), new(24, 17),
                new(0, 18), new(1, 18), new(2, 18), new(3, 18), new(4, 18), new(5, 18), new(6, 18), new(7, 18), new(8, 18), new(9, 18), new(10, 18), new(11, 18), new(12, 18), null       , new(14, 18), new(15, 18), new(16, 18), new(17, 18), new(18, 18), new(19, 18), new(20, 18), new(21, 18), new(22, 18), new(23, 18), new(24, 18),
                new(0, 19), new(1, 19), new(2, 19), new(3, 19), new(4, 19), new(5, 19), new(6, 19), new(7, 19), new(8, 19), new(9, 19), new(10, 19), new(11, 19), new(12, 19), null       , new(14, 19), new(15, 19), new(16, 19), new(17, 19), new(18, 19), new(19, 19), new(20, 19), new(21, 19), new(22, 19), new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACircle(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, includeCenter, false);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y), $"Tile {expectedTiles[i]} has not been found into the extracted tiles");
            }
        }


        [Test]
        public void GetTilesOnACircleOutline_InsideBounds_IncludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int radius = 6;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , null      , null      , null      , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , null       , null       , null       , null       ,
                null      , null      , new(6, 6) , new(7, 6) , null      , null      , null       , null       , null       , new(13, 6) , new(14, 6) , null       , null       ,
                null      , new(5, 7) , null      , null      , null      , null      , null       , null       , null       , null       , null       , new(15, 7) , null       ,
                null      , new(5, 8) , null      , null      , null      , null      , null       , null       , null       , null       , null       , new(15, 8) , null       ,
                new(4, 9) , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 9) ,
                new(4, 10), null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 10),
                new(4, 11), null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 11),
                new(4, 12), null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 12),
                new(4, 13), null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 13),
                null      , new(5, 14), null      , null      , null      , null      , null       , null       , null       , null       , null       , new(15, 14), null       ,
                null      , new(5, 15), null      , null      , null      , null      , null       , null       , null       , null       , null       , new(15, 15), null       ,
                null      , null      , new(6, 16), new(7, 16), null      , null      , null       , null       , null       , new(13, 16), new(14, 16), null       , null       ,
                null      , null      , null      , null      , new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnACircleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, true);
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
        [Test]
        public void GetTilesOnACircleOutline_InsideBounds_ExcludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int radius = 6;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , null      , null      , null      , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , null       , null       , null       , null       ,
                null      , null      , new(6, 6) , new(7, 6) , null      , null      , null       , null       , null       , new(13, 6) , new(14, 6) , null       , null       ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , new(15, 7) , null       ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 9) ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 10),
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 11),
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 12),
                new(4, 13), null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , null       , new(16, 13),
                null      , new(5, 14), null      , null      , null      , null      , null       , null       , null       , null       , null       , new(15, 14), null       ,
                null      , null      , null      , null      , null      , null      , null       , null       , null       , null       , null       , new(15, 15), null       ,
                null      , null      , new(6, 16), new(7, 16), null      , null      , null       , null       , null       , null       , new(14, 16), null       , null       ,
                null      , null      , null      , null      , new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnACircleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, false);
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

        [Test]
        public void GetTilesOnACircleOutline_OutsideBounds_IncludeWalls()
        {
            int coordX = 2;
            int coordY = 1;
            int radius = 40;
            Vector2Int?[] expectedTiles = new Vector2Int?[0];
            TestTile[] extractedTiles = Extraction.GetTilesOnACircleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, true);
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
        [Test]
        public void GetTilesOnACircleOutline_OutsideBounds_ExcludeWalls()
        {
            int coordX = 2;
            int coordY = 1;
            int radius = 40;
            Vector2Int?[] expectedTiles = new Vector2Int?[0];
            TestTile[] extractedTiles = Extraction.GetTilesOnACircleOutline(_grid, GridUtils.GetTile(_grid, coordX, coordY), radius, false);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y), $"Tile {expectedTiles[i]} has not been found into the extracted tiles");
            }
        }



        [TestCase(true)]
        [TestCase(false)]
        public void GetTilesInACone_InsideBounds_IncludeWalls(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 270;
            int openingAngle = 90;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , null      , null      , new(8, 4) , new(9, 4) , new(10, 4) , new(11, 4) , new(12, 4) , null       , null       , null       ,
                null      , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , null       ,
                new(5, 6) , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) ,
                null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , null       ,
                null      , null      , new(7, 8) , new(8, 8) , new(9, 8) , new(10, 8) , new(11, 8) , new(12, 8) , new(13, 8) , null       , null       ,
                null      , null      , null      , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , null       , null       , null       ,
                null      , null      , null      , null      , new(9, 10), new(10, 10), new(11, 10), null       , null       , null       , null       ,
                null      , null      , null      , null      , null      , start      , null       , null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACone(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle, includeStart, true);
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
        public void GetTilesInACone_InsideBounds_ExcludeWalls(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 270;
            int openingAngle = 90;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null      , null      , null      , new(8, 4) , new(9, 4) , null       , null       , new(12, 4) , null       , null       , null       ,
                null      , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , null       ,
                null      , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) ,
                null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , null       ,
                null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       ,
                null      , null      , null      , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , null       , null       , null       ,
                null      , null      , null      , null      , new(9, 10), new(10, 10), new(11, 10), null       , null       , null       , null       ,
                null      , null      , null      , null      , null      , start      , null       , null       , null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACone(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle, includeStart, false);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y), $"The following tile has not been extracted {expectedTiles[i]}");
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GetTilesInACone_OutsideBounds_IncludeWalls(bool includeStart)
        {
            int coordX = 2;
            int coordY = 1;
            int length = 0;
            int directionAngle = 0;
            int openingAngle = 360;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(0, 0) , new(1, 0) , new(2, 0) , new(3, 0) , new(4, 0) , new(5, 0) , new(6, 0) , new(7, 0) , new(8, 0) , new(9, 0) , new(10, 0) , new(11, 0) , new(12, 0) , new(13, 0) , new(14, 0) , new(15, 0) , new(16, 0) , new(17, 0) , new(18, 0) , new(19, 0) , new(20, 0) , new(21, 0) , new(22, 0) , new(23, 0) , new(24, 0) ,
                new(0, 1) , new(1, 1) , start     , new(3, 1) , new(4, 1) , new(5, 1) , new(6, 1) , new(7, 1) , new(8, 1) , new(9, 1) , new(10, 1) , new(11, 1) , new(12, 1) , new(13, 1) , new(14, 1) , new(15, 1) , new(16, 1) , new(17, 1) , new(18, 1) , new(19, 1) , new(20, 1) , new(21, 1) , new(22, 1) , new(23, 1) , new(24, 1) ,
                new(0, 2) , new(1, 2) , new(2, 2) , new(3, 2) , new(4, 2) , new(5, 2) , new(6, 2) , new(7, 2) , new(8, 2) , new(9, 2) , new(10, 2) , new(11, 2) , new(12, 2) , new(13, 2) , new(14, 2) , new(15, 2) , new(16, 2) , new(17, 2) , new(18, 2) , new(19, 2) , new(20, 2) , new(21, 2) , new(22, 2) , new(23, 2) , new(24, 2) ,
                new(0, 3) , new(1, 3) , new(2, 3) , new(3, 3) , new(4, 3) , new(5, 3) , new(6, 3) , new(7, 3) , new(8, 3) , new(9, 3) , new(10, 3) , new(11, 3) , new(12, 3) , new(13, 3) , new(14, 3) , new(15, 3) , new(16, 3) , new(17, 3) , new(18, 3) , new(19, 3) , new(20, 3) , new(21, 3) , new(22, 3) , new(23, 3) , new(24, 3) ,
                new(0, 4) , new(1, 4) , new(2, 4) , new(3, 4) , new(4, 4) , new(5, 4) , new(6, 4) , new(7, 4) , new(8, 4) , new(9, 4) , new(10, 4) , new(11, 4) , new(12, 4) , new(13, 4) , new(14, 4) , new(15, 4) , new(16, 4) , new(17, 4) , new(18, 4) , new(19, 4) , new(20, 4) , new(21, 4) , new(22, 4) , new(23, 4) , new(24, 4) ,
                new(0, 5) , new(1, 5) , new(2, 5) , new(3, 5) , new(4, 5) , new(5, 5) , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , new(15, 5) , new(16, 5) , new(17, 5) , new(18, 5) , new(19, 5) , new(20, 5) , new(21, 5) , new(22, 5) , new(23, 5) , new(24, 5) ,
                new(0, 6) , new(1, 6) , new(2, 6) , new(3, 6) , new(4, 6) , new(5, 6) , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) , new(16, 6) , new(17, 6) , new(18, 6) , new(19, 6) , new(20, 6) , new(21, 6) , new(22, 6) , new(23, 6) , new(24, 6) ,
                new(0, 7) , new(1, 7) , new(2, 7) , new(3, 7) , new(4, 7) , new(5, 7) , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) , new(17, 7) , new(18, 7) , new(19, 7) , new(20, 7) , new(21, 7) , new(22, 7) , new(23, 7) , new(24, 7) ,
                new(0, 8) , new(1, 8) , new(2, 8) , new(3, 8) , new(4, 8) , new(5, 8) , new(6, 8) , new(7, 8) , new(8, 8) , new(9, 8) , new(10, 8) , new(11, 8) , new(12, 8) , new(13, 8) , new(14, 8) , new(15, 8) , new(16, 8) , new(17, 8) , new(18, 8) , new(19, 8) , new(20, 8) , new(21, 8) , new(22, 8) , new(23, 8) , new(24, 8) ,
                new(0, 9) , new(1, 9) , new(2, 9) , new(3, 9) , new(4, 9) , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) , new(17, 9) , new(18, 9) , new(19, 9) , new(20, 9) , new(21, 9) , new(22, 9) , new(23, 9) , new(24, 9) ,
                new(0, 10), new(1, 10), new(2, 10), new(3, 10), new(4, 10), new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10), new(17, 10), new(18, 10), new(19, 10), new(20, 10), new(21, 10), new(22, 10), new(23, 10), new(24, 10),
                new(0, 11), new(1, 11), new(2, 11), new(3, 11), new(4, 11), new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), new(10, 11), new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), new(17, 11), new(18, 11), new(19, 11), new(20, 11), new(21, 11), new(22, 11), new(23, 11), new(24, 11),
                new(0, 12), new(1, 12), new(2, 12), new(3, 12), new(4, 12), new(5, 12), new(6, 12), new(7, 12), new(8, 12), new(9, 12), new(10, 12), new(11, 12), new(12, 12), new(13, 12), new(14, 12), new(15, 12), new(16, 12), new(17, 12), new(18, 12), new(19, 12), new(20, 12), new(21, 12), new(22, 12), new(23, 12), new(24, 12),
                new(0, 13), new(1, 13), new(2, 13), new(3, 13), new(4, 13), new(5, 13), new(6, 13), new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), new(13, 13), new(14, 13), new(15, 13), new(16, 13), new(17, 13), new(18, 13), new(19, 13), new(20, 13), new(21, 13), new(22, 13), new(23, 13), new(24, 13),
                new(0, 14), new(1, 14), new(2, 14), new(3, 14), new(4, 14), new(5, 14), new(6, 14), new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), new(13, 14), new(14, 14), new(15, 14), new(16, 14), new(17, 14), new(18, 14), new(19, 14), new(20, 14), new(21, 14), new(22, 14), new(23, 14), new(24, 14),
                new(0, 15), new(1, 15), new(2, 15), new(3, 15), new(4, 15), new(5, 15), new(6, 15), new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), new(13, 15), new(14, 15), new(15, 15), new(16, 15), new(17, 15), new(18, 15), new(19, 15), new(20, 15), new(21, 15), new(22, 15), new(23, 15), new(24, 15),
                new(0, 16), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), new(13, 16), new(14, 16), new(15, 16), new(16, 16), new(17, 16), new(18, 16), new(19, 16), new(20, 16), new(21, 16), new(22, 16), new(23, 16), new(24, 16),
                new(0, 17), new(1, 17), new(2, 17), new(3, 17), new(4, 17), new(5, 17), new(6, 17), new(7, 17), new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), new(13, 17), new(14, 17), new(15, 17), new(16, 17), new(17, 17), new(18, 17), new(19, 17), new(20, 17), new(21, 17), new(22, 17), new(23, 17), new(24, 17),
                new(0, 18), new(1, 18), new(2, 18), new(3, 18), new(4, 18), new(5, 18), new(6, 18), new(7, 18), new(8, 18), new(9, 18), new(10, 18), new(11, 18), new(12, 18), new(13, 18), new(14, 18), new(15, 18), new(16, 18), new(17, 18), new(18, 18), new(19, 18), new(20, 18), new(21, 18), new(22, 18), new(23, 18), new(24, 18),
                new(0, 19), new(1, 19), new(2, 19), new(3, 19), new(4, 19), new(5, 19), new(6, 19), new(7, 19), new(8, 19), new(9, 19), new(10, 19), new(11, 19), new(12, 19), new(13, 19), new(14, 19), new(15, 19), new(16, 19), new(17, 19), new(18, 19), new(19, 19), new(20, 19), new(21, 19), new(22, 19), new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACone(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle, includeStart, true);
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
        public void GetTilesInACone_OutsideBounds_ExcludeWalls(bool includeStart)
        {
            int coordX = 2;
            int coordY = 1;
            int length = 0;
            int directionAngle = 0;
            int openingAngle = 360;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(0, 0) , null      , new(2, 0) , new(3, 0) , new(4, 0) , new(5, 0) , new(6, 0) , new(7, 0) , new(8, 0) , new(9, 0) , new(10, 0) , new(11, 0) , new(12, 0) , new(13, 0) , new(14, 0) , new(15, 0) , new(16, 0) , null       , new(18, 0) , new(19, 0) , new(20, 0) , new(21, 0) , new(22, 0) , new(23, 0) , new(24, 0) ,
                new(0, 1) , null      , start     , new(3, 1) , new(4, 1) , new(5, 1) , new(6, 1) , new(7, 1) , new(8, 1) , new(9, 1) , new(10, 1) , new(11, 1) , new(12, 1) , new(13, 1) , new(14, 1) , new(15, 1) , new(16, 1) , null       , new(18, 1) , new(19, 1) , new(20, 1) , new(21, 1) , new(22, 1) , new(23, 1) , new(24, 1) ,
                new(0, 2) , null      , new(2, 2) , new(3, 2) , new(4, 2) , new(5, 2) , new(6, 2) , new(7, 2) , new(8, 2) , new(9, 2) , null       , null       , new(12, 2) , new(13, 2) , new(14, 2) , new(15, 2) , new(16, 2) , null       , new(18, 2) , new(19, 2) , new(20, 2) , new(21, 2) , new(22, 2) , new(23, 2) , new(24, 2) ,
                new(0, 3) , null      , new(2, 3) , new(3, 3) , new(4, 3) , new(5, 3) , new(6, 3) , new(7, 3) , new(8, 3) , new(9, 3) , null       , null       , new(12, 3) , new(13, 3) , new(14, 3) , new(15, 3) , new(16, 3) , null       , new(18, 3) , new(19, 3) , new(20, 3) , new(21, 3) , new(22, 3) , new(23, 3) , new(24, 3) ,
                new(0, 4) , null      , new(2, 4) , new(3, 4) , new(4, 4) , null      , new(6, 4) , new(7, 4) , new(8, 4) , new(9, 4) , null       , null       , new(12, 4) , new(13, 4) , new(14, 4) , new(15, 4) , new(16, 4) , null       , new(18, 4) , new(19, 4) , new(20, 4) , new(21, 4) , new(22, 4) , new(23, 4) , new(24, 4) ,
                new(0, 5) , null      , new(2, 5) , new(3, 5) , new(4, 5) , null      , new(6, 5) , new(7, 5) , new(8, 5) , new(9, 5) , new(10, 5) , new(11, 5) , new(12, 5) , new(13, 5) , new(14, 5) , new(15, 5) , new(16, 5) , new(17, 5) , new(18, 5) , new(19, 5) , new(20, 5) , new(21, 5) , new(22, 5) , new(23, 5) , new(24, 5) ,
                new(0, 6) , null      , new(2, 6) , new(3, 6) , new(4, 6) , null      , new(6, 6) , new(7, 6) , new(8, 6) , new(9, 6) , new(10, 6) , new(11, 6) , new(12, 6) , new(13, 6) , new(14, 6) , new(15, 6) , new(16, 6) , new(17, 6) , new(18, 6) , new(19, 6) , new(20, 6) , new(21, 6) , new(22, 6) , new(23, 6) , new(24, 6) ,
                new(0, 7) , null      , new(2, 7) , new(3, 7) , new(4, 7) , null      , new(6, 7) , new(7, 7) , new(8, 7) , new(9, 7) , new(10, 7) , new(11, 7) , new(12, 7) , new(13, 7) , new(14, 7) , new(15, 7) , new(16, 7) , new(17, 7) , new(18, 7) , new(19, 7) , new(20, 7) , new(21, 7) , new(22, 7) , new(23, 7) , new(24, 7) ,
                new(0, 8) , null      , null      , null      , null      , null      , null      , null      , null      , new(9, 8) , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , null       , new(23, 8) , new(24, 8) ,
                new(0, 9) , new(1, 9) , new(2, 9) , new(3, 9) , null      , new(5, 9) , new(6, 9) , new(7, 9) , new(8, 9) , new(9, 9) , new(10, 9) , new(11, 9) , new(12, 9) , new(13, 9) , new(14, 9) , new(15, 9) , new(16, 9) , new(17, 9) , new(18, 9) , new(19, 9) , new(20, 9) , new(21, 9) , null       , new(23, 9) , new(24, 9) ,
                new(0, 10), new(1, 10), new(2, 10), new(3, 10), null      , new(5, 10), new(6, 10), new(7, 10), new(8, 10), new(9, 10), new(10, 10), new(11, 10), new(12, 10), new(13, 10), new(14, 10), new(15, 10), new(16, 10), new(17, 10), new(18, 10), new(19, 10), new(20, 10), new(21, 10), null       , new(23, 10), new(24, 10),
                new(0, 11), new(1, 11), new(2, 11), new(3, 11), null      , new(5, 11), new(6, 11), new(7, 11), new(8, 11), new(9, 11), new(10, 11), new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), null       , null       , null       , null       , new(21, 11), null       , new(23, 11), new(24, 11),
                null      , null      , new(2, 12), new(3, 12), null      , null      , null      , new(7, 12), null      , null      , null       , null       , null       , null       , new(14, 12), new(15, 12), new(16, 12), null       , new(18, 12), new(19, 12), new(20, 12), new(21, 12), null       , new(23, 12), new(24, 12),
                new(0, 13), new(1, 13), new(2, 13), new(3, 13), new(4, 13), new(5, 13), null      , new(7, 13), new(8, 13), new(9, 13), new(10, 13), new(11, 13), new(12, 13), null       , new(14, 13), new(15, 13), new(16, 13), null       , new(18, 13), new(19, 13), new(20, 13), new(21, 13), null       , new(23, 13), new(24, 13),
                new(0, 14), new(1, 14), new(2, 14), new(3, 14), new(4, 14), new(5, 14), null      , new(7, 14), new(8, 14), new(9, 14), new(10, 14), new(11, 14), new(12, 14), null       , new(14, 14), new(15, 14), new(16, 14), null       , new(18, 14), new(19, 14), new(20, 14), new(21, 14), null       , new(23, 14), new(24, 14),
                new(0, 15), new(1, 15), null      , null      , null      , null      , null      , new(7, 15), new(8, 15), new(9, 15), new(10, 15), new(11, 15), new(12, 15), null       , new(14, 15), new(15, 15), new(16, 15), null       , new(18, 15), new(19, 15), new(20, 15), new(21, 15), null       , new(23, 15), new(24, 15),
                new(0, 16), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(8, 16), new(9, 16), new(10, 16), new(11, 16), new(12, 16), null       , new(14, 16), new(15, 16), new(16, 16), null       , null       , null       , null       , null       , null       , new(23, 16), new(24, 16),
                new(0, 17), new(1, 17), new(2, 17), new(3, 17), new(4, 17), new(5, 17), new(6, 17), new(7, 17), new(8, 17), new(9, 17), new(10, 17), new(11, 17), new(12, 17), null       , new(14, 17), new(15, 17), new(16, 17), new(17, 17), new(18, 17), new(19, 17), new(20, 17), new(21, 17), new(22, 17), new(23, 17), new(24, 17),
                new(0, 18), new(1, 18), new(2, 18), new(3, 18), new(4, 18), new(5, 18), new(6, 18), new(7, 18), new(8, 18), new(9, 18), new(10, 18), new(11, 18), new(12, 18), null       , new(14, 18), new(15, 18), new(16, 18), new(17, 18), new(18, 18), new(19, 18), new(20, 18), new(21, 18), new(22, 18), new(23, 18), new(24, 18),
                new(0, 19), new(1, 19), new(2, 19), new(3, 19), new(4, 19), new(5, 19), new(6, 19), new(7, 19), new(8, 19), new(9, 19), new(10, 19), new(11, 19), new(12, 19), null       , new(14, 19), new(15, 19), new(16, 19), new(17, 19), new(18, 19), new(19, 19), new(20, 19), new(21, 19), new(22, 19), new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesInACone(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, directionAngle, includeStart, false);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y), $"Tile {expectedTiles[i]} has not been found into the extracted tiles");
            }
        }




        [TestCase(true)]
        [TestCase(false)]
        public void GetTilesOnALine_InsideBounds_IncludeWalls_AllowDiagonals(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       , null       ,
                null       , new(11, 12), null       , null       , null       , null       ,
                null       , null       , new(12, 13), null       , null       , null       ,
                null       , null       , null       , new(13, 14), null       , null       ,
                null       , null       , null       , null       , new(14, 15), null       ,
                null       , null       , null       , null       , null       , new(15, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false, includeStart, true);
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
        public void GetTilesOnALine_InsideBounds_IncludeWalls_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(11, 11), null       , null       , null       , null       ,
                null       , new(11, 12), new(12, 12), null       , null       , null       ,
                null       , null       , new(12, 13), new(13, 13), null       , null       ,
                null       , null       , null       , new(13, 14), new(14, 14), null       ,
                null       , null       , null       , null       , new(14, 15), new(15, 15),
                null       , null       , null       , null       , null       , new(15, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false, includeStart, true);
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
        public void GetTilesOnALine_InsideBounds_IncludeWalls_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       , null       ,
                new(10, 12), new(11, 12), null       , null       , null       , null       ,
                null       , new(11, 13), new(12, 13), null       , null       , null       ,
                null       , null       , new(12, 14), new(13, 14), null       , null       ,
                null       , null       , null       , new(13, 15), new(14, 15), null       ,
                null       , null       , null       , null       , new(14, 16), new(15, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true, includeStart, true);
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
        public void GetTilesOnALine_InsideBounds_ExcludeWalls_AllowDiagonals(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       , null       ,
                null       , null       , null       , null       , null       , null       ,
                null       , null       , new(12, 13), null       , null       , null       ,
                null       , null       , null       , null       , null       , null       ,
                null       , null       , null       , null       , new(14, 15), null       ,
                null       , null       , null       , null       , null       , new(15, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false, includeStart, false);
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
        public void GetTilesOnALine_InsideBounds_ExcludeWalls_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(11, 11), null       , null       , null       , null       ,
                null       , null       , null       , null       , null       , null       ,
                null       , null       , new(12, 13), null       , null       , null       ,
                null       , null       , null       , null       , new(14, 14), null       ,
                null       , null       , null       , null       , new(14, 15), new(15, 15),
                null       , null       , null       , null       , null       , new(15, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false, includeStart, false);
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
        public void GetTilesOnALine_InsideBounds_ExcludeWalls_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 10;
            int coordY = 11;
            int length = 7;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       , null       ,
                null       , null       , null       , null       , null       , null       ,
                null       , new(11, 13), new(12, 13), null       , null       , null       ,
                null       , null       , new(12, 14), null       , null       , null       ,
                null       , null       , null       , null       , new(14, 15), null       ,
                null       , null       , null       , null       , new(14, 16), new(15, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true, includeStart, false);
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
        public void GetTilesOnALine_OutsideBounds_IncludeWalls_AllowDiagonals(bool includeStart)
        {
            int coordX = 20;
            int coordY = 15;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       ,
                null       , new(21, 16), null       , null       , null       ,
                null       , null       , new(22, 17), null       , null       ,
                null       , null       , null       , new(23, 18), null       ,
                null       , null       , null       , null       , new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false, includeStart, true);
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
        public void GetTilesOnALine_OutsideBounds_IncludeWalls_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 20;
            int coordY = 15;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(21, 15), null       , null       , null       ,
                null       , new(21, 16), new(22, 16), null       , null       ,
                null       , null       , new(22, 17), new(23, 17), null       ,
                null       , null       , null       , new(23, 18), new(24, 18),
                null       , null       , null       , null       , new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false, includeStart, true);
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
        public void GetTilesOnALine_OutsideBounds_IncludeWalls_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 20;
            int coordY = 15;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       ,
                new(20, 16), new(21, 16), null       , null       , null       ,
                null       , new(21, 17), new(22, 17), null       , null       ,
                null       , null       , new(22, 18), new(23, 18), null       ,
                null       , null       , null       , new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true, includeStart, true);
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
        public void GetTilesOnALine_OutsideBounds_ExcludeWalls_AllowDiagonals(bool includeStart)
        {
            int coordX = 20;
            int coordY = 15;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       ,
                null       , null       , null       , null       , null       ,
                null       , null       , new(22, 17), null       , null       ,
                null       , null       , null       , new(23, 18), null       ,
                null       , null       , null       , null       , new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, true, false, includeStart, /*true,*/ false);
            Assert.AreEqual(expectedTiles.Count(x => x != null), extractedTiles.Length, "Number of extracted tiles does not match expected count.");
            for (int i = 0; i < expectedTiles.Length; i++)
            {
                if (expectedTiles[i] == null)
                {
                    continue;
                }
                Assert.IsTrue(extractedTiles.Any((x) => x.X == expectedTiles[i]?.x && x.Y == expectedTiles[i]?.y), $"Tile {expectedTiles[i]} has not been found into the extracted tiles");
            }
        }
        [TestCase(true)]
        [TestCase(false)]
        public void GetTilesOnALine_OutsideBounds_ExcludeWalls_NoDiagonals_FavorHorizontal(bool includeStart)
        {
            int coordX = 20;
            int coordY = 15;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , new(21, 15), null       , null       , null       ,
                null       , null       , null       , null       , null       ,
                null       , null       , new(22, 17), new(23, 17), null       ,
                null       , null       , null       , new(23, 18), new(24, 18),
                null       , null       , null       , null       , new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, false, includeStart, false);
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
        public void GetTilesOnALine_OutsideBounds_ExcludeWalls_NoDiagonals_FavorVertical(bool includeStart)
        {
            int coordX = 20;
            int coordY = 15;
            int length = 0;
            int directionAngle = 45;
            Vector2Int? start = includeStart ? new Vector2Int(coordX, coordY) : null;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                start      , null       , null       , null       , null       ,
                null       , null       , null       , null       , null       ,
                null       , new(21, 17), new(22, 17), null       , null       ,
                null       , null       , new(22, 18), new(23, 18), null       ,
                null       , null       , null       , new(23, 19), new(24, 19),
            };
            TestTile[] extractedTiles = Extraction.GetTilesOnALine(_grid, GridUtils.GetTile(_grid, coordX, coordY), length, directionAngle, false, true, includeStart, false);
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
        [Test]
        public void GetTileNeighbour_IncludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            int directionAngle = 45;
            Extraction.GetTileNeighbour(_grid, GridUtils.GetTile(_grid, coordX, coordY), directionAngle, out TestTile neighbour, true);
            Assert.IsTrue(neighbour.X == 21 && neighbour.Y == 16, $"Neighbour coordinates are wrong. Expected X:{21} Y:{16} but got X:{neighbour.X} Y:{neighbour.Y}.");
        }
        [Test]
        public void GetTileNeighbour_ExcludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            int directionAngle = 45;
            Assert.IsFalse(Extraction.GetTileNeighbour(_grid, GridUtils.GetTile(_grid, coordX, coordY), directionAngle, out TestTile neighbour, false));
        }
        [Test]
        public void GetTileNeighbours_IncludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(19, 14), new(20, 14), new(21, 14),
                new(19, 15), null       , new(21, 15),
                new(19, 16), new(20, 16), new(21, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTileNeighbours(_grid, GridUtils.GetTile(_grid, coordX, coordY), true);
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
        [Test]
        public void GetTileNeighbours_ExcludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(19, 14), new(20, 14), new(21, 14),
                new(19, 15), null       , new(21, 15),
                null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTileNeighbours(_grid, GridUtils.GetTile(_grid, coordX, coordY), false);
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
        [Test]
        public void GetTileOrthogonalNeighbours_IncludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null       , new(20, 14), null       ,
                new(19, 15), null       , new(21, 15),
                null       , new(20, 16), null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTileOrthogonalsNeighbours(_grid, GridUtils.GetTile(_grid, coordX, coordY), true);
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
        [Test]
        public void GetTileOrthogonalNeighbours_ExcludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                null       , new(20, 14), null       ,
                new(19, 15), null       , new(21, 15),
                null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTileOrthogonalsNeighbours(_grid, GridUtils.GetTile(_grid, coordX, coordY), false);
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
        [Test]
        public void GetTileDiagonalsNeighbours_IncludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(19, 14), null       , new(21, 14),
                null       , null       , null       ,
                new(19, 16), null       , new(21, 16),
            };
            TestTile[] extractedTiles = Extraction.GetTileDiagonalsNeighbours(_grid, GridUtils.GetTile(_grid, coordX, coordY), true);
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
        [Test]
        public void GetTileDiagonalsNeighbours_ExcludeWalls()
        {
            int coordX = 20;
            int coordY = 15;
            Vector2Int?[] expectedTiles = new Vector2Int?[] {
                new(19, 14), null       , new(21, 14),
                null       , null       , null       ,
                null       , null       , null       ,
            };
            TestTile[] extractedTiles = Extraction.GetTileDiagonalsNeighbours(_grid, GridUtils.GetTile(_grid, coordX, coordY), false);
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


        [Test]
        public void IsTileInARectangle()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 11;
            int trueY = 12;
            int falseX = 24;
            int falseY = 19;
            int width = 6;
            int height = 4;
            Assert.IsTrue(Extraction.IsTileInARectangle(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), new(width, height)));
            Assert.IsFalse(Extraction.IsTileInARectangle(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), new(width, height)));
        }
        [Test]
        public void IsTileInARectangleOutline()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 16;
            int trueY = 15;
            int falseX = 17;
            int falseY = 16;
            int width = 6;
            int height = 4;
            Assert.IsTrue(Extraction.IsTileOnARectangleOutline(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), new(width, height)));
            Assert.IsFalse(Extraction.IsTileOnARectangleOutline(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), new(width, height)));
        }


        [Test]
        public void IsTileInACircle()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 15;
            int trueY = 15;
            int falseX = 16;
            int falseY = 16;
            int radius = 6;
            Assert.IsTrue(Extraction.IsTileInACircle(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), radius));
            Assert.IsFalse(Extraction.IsTileInACircle(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), radius));
        }
        [Test]
        public void IsTileInACircleOutline()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 15;
            int trueY = 15;
            int falseX = 16;
            int falseY = 16;
            int radius = 6;
            Assert.IsTrue(Extraction.IsTileOnACircleOutline(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), radius));
            Assert.IsFalse(Extraction.IsTileOnACircleOutline(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), radius));
        }


        [Test]
        public void IsTileInACone()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 10;
            int trueY = 4;
            int falseX = 10;
            int falseY = 3;
            int length = 7;
            int openingAngle = 90;
            int destinationAngle = 270;
            Assert.IsTrue(Extraction.IsTileInACone(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, destinationAngle));
            Assert.IsFalse(Extraction.IsTileInACone(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), length, openingAngle, destinationAngle));
        }

        [Test]
        public void IsTileOnALine_AllowDiagonals()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 10;
            int trueY = 4;
            int falseX = 10;
            int falseY = 3;
            int length = 7;
            int destinationAngle = 270;
            Assert.IsTrue(Extraction.IsTileOnALine(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), length, destinationAngle, true, false));
            Assert.IsFalse(Extraction.IsTileOnALine(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), length, destinationAngle, true, false));
        }
        [Test]
        public void IsTileOnALine_NoDiagonals_FavorHorizontal()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 10;
            int trueY = 4;
            int falseX = 10;
            int falseY = 3;
            int length = 7;
            int destinationAngle = 270;
            Assert.IsTrue(Extraction.IsTileOnALine(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), length, destinationAngle, false, false));
            Assert.IsFalse(Extraction.IsTileOnALine(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), length, destinationAngle, false, false));
        }
        [Test]
        public void IsTileOnALine_NoDiagonals_FavorVertical()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 10;
            int trueY = 4;
            int falseX = 10;
            int falseY = 3;
            int length = 7;
            int destinationAngle = 270;
            Assert.IsTrue(Extraction.IsTileOnALine(_grid, GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), length, destinationAngle, false, true));
            Assert.IsFalse(Extraction.IsTileOnALine(_grid, GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), length, destinationAngle, false, true));
        }
        [Test]
        public void IsTileNeighbor_IncludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 11;
            int trueY = 12;
            int falseX = 12;
            int falseY = 13;
            int destinationAngle = 45;

            Assert.IsTrue(Extraction.IsTileNeighbor(GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), destinationAngle, true));
            Assert.IsFalse(Extraction.IsTileNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), destinationAngle, true));
        }
        [Test]
        public void IsTileNeighbor_ExcludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int falseX = 11;
            int falseY = 12;
            int destinationAngle = 315;

            Assert.IsFalse(Extraction.IsTileNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), destinationAngle, false));
        }
        [Test]
        public void IsTileAnyNeighbor_IncludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 11;
            int trueY = 12;
            int falseX = 12;
            int falseY = 13;

            Assert.IsTrue(Extraction.IsTileAnyNeighbor(GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), true));
            Assert.IsFalse(Extraction.IsTileAnyNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), true));
        }
        [Test]
        public void IsTileAnyNeighbor_ExcludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int falseX = 11;
            int falseY = 12;

            Assert.IsFalse(Extraction.IsTileAnyNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), false));
        }
        [Test]
        public void IsTileOrthogonalNeighbor_IncludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int trueX = 11;
            int trueY = 11;
            int falseX = 12;
            int falseY = 13;

            Assert.IsTrue(Extraction.IsTileOrthogonalNeighbor(GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), true));
            Assert.IsFalse(Extraction.IsTileOrthogonalNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), true));
        }
        [Test]
        public void IsTileOrthogonalNeighbor_ExcludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int falseX = 10;
            int falseY = 12;

            Assert.IsFalse(Extraction.IsTileOrthogonalNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), false));
        }
        [Test]
        public void IsTileDiagonalNeighbor_IncludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int falseX = 11;
            int falseY = 11;
            int trueX = 11;
            int trueY = 12;

            Assert.IsTrue(Extraction.IsTileDiagonalNeighbor(GridUtils.GetTile(_grid, trueX, trueY), GridUtils.GetTile(_grid, coordX, coordY), true));
            Assert.IsFalse(Extraction.IsTileDiagonalNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), true));
        }
        [Test]
        public void IsTileDiagonalNeighbor_ExcludeWalls()
        {
            int coordX = 10;
            int coordY = 11;
            int falseX = 10;
            int falseY = 12;

            Assert.IsFalse(Extraction.IsTileDiagonalNeighbor(GridUtils.GetTile(_grid, falseX, falseY), GridUtils.GetTile(_grid, coordX, coordY), false));
        }
    }
}
