using Caskev.GridToolkit;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


namespace GridToolkitTests
{
    public class Pathfinding_Tests
    {
        TestTile[,] _gridRowMajorOrder;
        TestTile[,] _gridColMajorOrder;
        Dictionary<char, NextTileDirection> _dirs = new() { { '0', NextTileDirection.NONE }, { '.', NextTileDirection.SELF }, { '↑', NextTileDirection.DOWN }, { '↓', NextTileDirection.UP }, { '←', NextTileDirection.LEFT }, { '→', NextTileDirection.RIGHT }, { '↖', NextTileDirection.DOWN_LEFT }, { '↗', NextTileDirection.DOWN_RIGHT }, { '↙', NextTileDirection.UP_LEFT }, { '↘', NextTileDirection.UP_RIGHT } };
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
        [Test]
        public void GenerateDirectionMap()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile[,] grid = _gridRowMajorOrder;
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(grid, GridUtils.GetTile(grid, targetX, targetY, MajorOrder.ROW_MAJOR_ORDER), MajorOrder.ROW_MAJOR_ORDER);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'],
                _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'],
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'],
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['→'] , _dirs['.'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'],
                _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'],
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'],
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'],
                _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'],
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'],
            };
            for (int i = 0; i < dirMap._directionMap.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), i, MajorOrder.ROW_MAJOR_ORDER)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionMapColumnMajorOrder()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile[,] grid = _gridColMajorOrder;
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(grid, GridUtils.GetTile(grid, targetX, targetY, MajorOrder.COLUMN_MAJOR_ORDER), MajorOrder.COLUMN_MAJOR_ORDER);
            NextTileDirection[] expectedDirMap = new NextTileDirection[25 * 20]
            {
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['0'] , _dirs['↓'] , _dirs['.'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['→'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['0'] , _dirs['↓'] , _dirs['←'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] ,
            };
            for (int i = 0; i < dirMap._directionMap.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(1), grid.GetLength(0)), i, MajorOrder.COLUMN_MAJOR_ORDER)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        //[TestCase(MajorOrder.ROW_MAJOR_ORDER)]
        //[TestCase(MajorOrder.COLUMN_MAJOR_ORDER)]
        //public void GetTargetTile(MajorOrder majorOrder)
        //{
        //    int targetX = 6;
        //    int targetY = 10;
        //    int startX = 2;
        //    int startY = 10;
        //    TestTile[,] grid = majorOrder == MajorOrder.ROW_MAJOR_ORDER ? _gridRowMajorOrder : _gridColMajorOrder;
        //    DirectionMap dirMap = Pathfinding.GenerateDirectionMap(grid, GridUtils.GetTile(grid, targetX, targetY, majorOrder), majorOrder);
        //    //TestTile returnedTarget = dirMap.GetTargetTile(grid);
        //    //Assert.IsTrue(GridUtils.TileEquals(target, returnedTarget), $"Target is [{targetX}, {targetY}] but returned tile is {returnedTarget.X}, {returnedTarget.Y}");
        //}
    }
}
