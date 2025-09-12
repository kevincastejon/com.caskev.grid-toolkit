using Caskev.GridToolkit;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


namespace GridToolkitTests
{
    public class Pathfinding_Tests
    {
        TestTile[,] grid;
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

            grid = GridFactory.Build(map);
        }
        [Test]
        public void GenerateDirectionMap()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(grid, GridUtils.GetTile(grid, targetX, targetY));
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
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }

        [Test]
        public void GetTargetTile()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(grid, targetX, targetY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(grid, target);
            TestTile returnedTarget = dirMap.GetTargetTile(grid);
            Assert.IsTrue(GridUtils.TileEquals(target, returnedTarget), $"Target is [{target.X}, {target.Y}] but returned tile is {returnedTarget.X}, {returnedTarget.Y}");
        }
    }
}
