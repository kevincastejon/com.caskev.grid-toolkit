using Caskev.GridToolkit;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GridToolkitTests
{
    public class Pathfinding_Tests
    {
        TestTile[,] _grid;
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
                { true , true , true , true , true , false, true , true , true , true , true , true , true , false, true , true , true , false, true , true , true , true , false, true , true  },
                { true , true , false, false, false, false, false, true , true , true , true , true , true , false, true , true , true , false, true , true , true , true , false, true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , false, false, false, false, false, false, true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true  },
                { true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , true , true , true , true , true , true , true , true , true , true  },
            };
            _grid = GridFactory.Build(map);
        }
        [Test]
        public void GenerateDirectionMap_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.NONE);
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
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'],
                _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'],
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'],
            };
            for (int i = 0; i < dirMap._directionMap.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionMap_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↘'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['→'] , _dirs['.'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['→'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↑'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
            };
            for (int i = 0; i < dirMap._directionMap.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionMap_OneFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↘'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['→'] , _dirs['.'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['→'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
            };
            for (int i = 0; i < dirMap._directionMap.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionMap_TwoFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['←'] , _dirs['→'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['→'] , _dirs['.'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
        };
            for (int i = 0; i < dirMap._directionMap.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void DirectionMap_GetTargetTile()
        {
            int targetX = 6;
            int targetY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target);
            TestTile returnedTarget = dirMap.GetTargetTile(_grid);
            Assert.IsTrue(GridUtils.TileEquals(target, returnedTarget), $"Target is [{target.X}, {target.Y}] but returned tile is {returnedTarget.X}, {returnedTarget.Y}");
        }
        [Test]
        public void DirectionMap_IsTileAccessible()
        {
            int targetX = 6;
            int targetY = 10;
            int accessibleTileX = 2;
            int accessibleTileY = 1;
            int inaccessibleTileX = 1;
            int inaccessibleTileY = 3;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target);
            Assert.IsTrue(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, accessibleTileX, accessibleTileY)));
            Assert.IsFalse(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, inaccessibleTileX, inaccessibleTileY)));
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathToTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.NONE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathToTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                startTile, new(2,11), new(2,12), new(2,13), new(2,14), new(1,14), new(1,15), new(1,16), new(2,16), new(3,16), new(4,16), new(5,16), new(6,16), new(7,16), new(7,15), new(7,14), new(7,13), new(7,12), new(7,11), new(7,10), targetTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathToTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathToTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                startTile, new(3,11), new(3,12), new(4,13), new(5,13), new(6,14), new(7,13), new(7,12), new(6,11), targetTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathToTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathToTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                startTile, new(2,11), new(2,12), new(1,13), new(1,14), new(1,15), new(2,16), new(3,16), new(4,16), new(5,16), new(6,16), new(7,15), new(7,14), new(7,13), new(7,12), new(6,11), targetTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathToTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathToTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                startTile, new(2,11), new(2,12), new(2,13), new(1,14), new(1,15), new(1,16), new(2,16), new(3,16), new(4,16), new(5,16), new(6,16), new(7,16), new(7,15), new(7,14), new(7,13), new(7,12), new(7,11), targetTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathFromTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.NONE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathFromTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                targetTile, new(7,10), new(7,11), new(7,12), new(7,13), new(7,14), new(7,15), new(7,16), new(6,16), new(5,16), new(4,16), new(3,16), new(2,16), new(1,16), new(1,15), new(1,14), new(2,14), new(2,13), new(2,12), new(2,11), startTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathFromTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathFromTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                targetTile, new(6,11), new(7,12), new(7,13), new(6,14), new(5,13), new(4,13), new(3,12), new(3,11), startTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathFromTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathFromTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                targetTile, new(6,11), new(7,12), new(7,13), new(7,14), new(7,15), new(6,16), new(5,16), new(4,16), new(3,16), new(2,16), new(1,15), new(1,14), new(1,13), new(2,12), new(2,11), startTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionMap_GetPathFromTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathFromTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                targetTile, new(7,11), new(7,12), new(7,13), new(7,14), new(7,15), new(7,16), new(6,16), new(5,16), new(4,16), new(3,16), new(2,16), new(1,16), new(1,15), new(1,14), new(2,13), new(2,12), new(2,11), startTile
            };
            TestTile[] expectedPath = expectedPathList.Where(x => x != null).ToArray();
            Assert.AreEqual(expectedPath.Length, returnedPath.Length);
            for (int i = 0; i < expectedPath.Length; i++)
            {
                TestTile actualTile = returnedPath[i];
                TestTile expectedTile = expectedPath[i];
                if (expectedTile == null)
                {
                    continue;
                }
                Assert.IsTrue(GridUtils.TileEquals(expectedTile, actualTile), $"Step {i} should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{actualTile.X}, {actualTile.Y}]");
            }
        }
        [Test]
        public void DirectionMap_GetNextTileFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.NONE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(4, 13);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionMap_GetNextTileFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(6, 14);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionMap_GetNextTileFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(2, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionMap_GetNextTileFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(1, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionMap_GetNextTileDirectionFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.NONE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.LEFT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionMap_GetNextTileDirectionFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionMap_GetNextTileDirectionFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionMap_GetNextTileDirectionFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionMap_Serialization()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 10;
            int startY = 17;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionMap dirMap = Pathfinding.GenerateDirectionMap(_grid, target);
            byte[] serializedDirMap = dirMap.ToByteArray();
            DirectionMap deserializedDirMap = DirectionMap.FromByteArray(_grid, serializedDirMap);
            Assert.AreEqual(dirMap._directionMap.Length, deserializedDirMap._directionMap.Length);
            for (int i = 0; i < deserializedDirMap._directionMap.Length; i++)
            {
                Assert.AreEqual(dirMap._directionMap[i], deserializedDirMap._directionMap[i]);
            }
        }
    }
}
