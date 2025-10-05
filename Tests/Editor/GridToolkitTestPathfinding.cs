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

        #region DirectionGrid

        [Test]
        public void GenerateDirectionGrid_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.NONE);
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
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionGrid_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.ALL_DIAGONALS);
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
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionGrid_OneFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.DIAGONAL_1FREE);
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
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionGrid_TwoFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.DIAGONAL_2FREE);
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
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void DirectionGrid_GetTargetTile()
        {
            int targetX = 6;
            int targetY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target);
            TestTile returnedTarget = dirMap.GetTargetTile(_grid);
            Assert.IsTrue(GridUtils.TileEquals(target, returnedTarget), $"Target is [{target.X}, {target.Y}] but returned tile is {returnedTarget.X}, {returnedTarget.Y}");
        }
        [Test]
        public void DirectionGrid_IsTileAccessible()
        {
            int targetX = 6;
            int targetY = 10;
            int accessibleTileX = 2;
            int accessibleTileY = 1;
            int inaccessibleTileX = 1;
            int inaccessibleTileY = 3;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target);
            Assert.IsTrue(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, accessibleTileX, accessibleTileY)));
            Assert.IsFalse(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, inaccessibleTileX, inaccessibleTileY)));
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionGrid_GetPathToTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.NONE);
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
        public void DirectionGrid_GetPathToTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DirectionGrid_GetPathToTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DirectionGrid_GetPathToTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DirectionGrid_GetPathFromTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.NONE);
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
        public void DirectionGrid_GetPathFromTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DirectionGrid_GetPathFromTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DirectionGrid_GetPathFromTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DirectionGrid_GetNextTileFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.NONE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(4, 13);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionGrid_GetNextTileFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(6, 14);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionGrid_GetNextTileFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(2, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionGrid_GetNextTileFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(1, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionGrid_GetNextTileDirectionFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.NONE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.LEFT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionGrid_GetNextTileDirectionFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionGrid_GetNextTileDirectionFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionGrid_GetNextTileDirectionFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionGrid_Serialization()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 10;
            int startY = 17;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionGrid dirMap = Pathfinding.GenerateDirectionGrid(_grid, target);
            byte[] serializedDirMap = dirMap.ToByteArray();
            DirectionGrid deserializedDirMap = DirectionGrid.FromByteArray(_grid, serializedDirMap);
            Assert.AreEqual(dirMap._directionGrid.Length, deserializedDirMap._directionGrid.Length);
            for (int i = 0; i < deserializedDirMap._directionGrid.Length; i++)
            {
                Assert.AreEqual(dirMap._directionGrid[i], deserializedDirMap._directionGrid[i]);
            }
        }

        #endregion

        #region DirectionField

        [Test]
        public void GenerateDirectionField_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int maxDistance = 20;
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.NONE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['→'] , _dirs['.'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['→'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['↓'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['↓'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
                _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'],
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionField_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int maxDistance = 20;
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
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
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↑'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionField_OneFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int maxDistance = 20;
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↘'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['→'] , _dirs['.'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['→'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] ,
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void GenerateDirectionField_TwoFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int maxDistance = 20;
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['←'] , _dirs['→'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['↘'] , _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] ,
                _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↘'] , _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['→'] , _dirs['.'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['→'] , _dirs['→'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['0'] , _dirs['0'] , _dirs['↓'] , _dirs['↙'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['↙'] , _dirs['↙'] , _dirs['↙'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['→'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↘'] , _dirs['↓'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['→'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['←'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] ,
                _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↗'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['↑'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['↖'] , _dirs['0'] , _dirs['0'] ,
        };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
            }
        }
        [Test]
        public void DirectionField_GetTargetTile()
        {
            int targetX = 6;
            int targetY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance);
            TestTile returnedTarget = dirMap.GetTargetTile(_grid);
            Assert.IsTrue(GridUtils.TileEquals(target, returnedTarget), $"Target is [{target.X}, {target.Y}] but returned tile is {returnedTarget.X}, {returnedTarget.Y}");
        }
        [Test]
        public void DirectionField_IsTileAccessible()
        {
            int targetX = 6;
            int targetY = 10;
            int accessibleTileX = 2;
            int accessibleTileY = 1;
            int inaccessibleTileX = 1;
            int inaccessibleTileY = 3;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance);
            Assert.IsTrue(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, accessibleTileX, accessibleTileY)));
            Assert.IsFalse(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, inaccessibleTileX, inaccessibleTileY)));
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DirectionField_GetPathToTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
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
        public void DirectionField_GetPathToTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DirectionField_GetPathToTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DirectionField_GetPathToTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DirectionField_GetPathFromTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
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
        public void DirectionField_GetPathFromTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DirectionField_GetPathFromTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DirectionField_GetPathFromTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DirectionField_GetNextTileFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            int maxDistance = 20;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(4, 13);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionField_GetNextTileFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            int maxDistance = 14;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(6, 14);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionField_GetNextTileFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            int maxDistance = 14;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(2, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionField_GetNextTileFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            int maxDistance = 14;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(1, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DirectionField_GetNextTileDirectionFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            int maxDistance = 14;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionField_GetNextTileDirectionFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            int maxDistance = 9;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.DOWN_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionField_GetNextTileDirectionFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            int maxDistance = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DirectionField_GetNextTileDirectionFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            int maxDistance = 14;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DirectionField dirMap = Pathfinding.GenerateDirectionField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP;
            Assert.AreEqual(expectedDir, returnedDir);
        }

        #endregion

        #region DijkstraGrid

        [Test]
        public void GenerateDijkstraGrid_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.NONE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['←'],
                _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['←'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'],
                _dirs['↓'], _dirs['0'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'],
                _dirs['↓'], _dirs['0'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↓'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['←'],
                _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['0'], _dirs['↑'], _dirs['←'],
                _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['←'],
                _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↑'],
                _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↑'],
                _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['←'],
                _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↑'],
                _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['←'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'],
                _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['→'], _dirs['↑'], _dirs['↑'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'],
                _dirs['↑'], _dirs['↑'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                32f       , 0f        , 20f       , 19f       , 18f       , 17f       , 16f       , 15f       , 14f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 22f       , 23f       , 24f       , 25f       , 26f       , 27f       , 28f       ,
                31f       , 0f        , 19f       , 18f       , 17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 0f        , 21f       , 22f       , 23f       , 24f       , 25f       , 26f       , 27f       ,
                30f       , 0f        , 18f       , 17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 0f        , 0f        , 14f       , 15f       , 16f       , 17f       , 18f       , 0f        , 20f       , 21f       , 22f       , 23f       , 24f       , 25f       , 26f       ,
                29f       , 0f        , 17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 0f        , 0f        , 13f       , 14f       , 15f       , 16f       , 17f       , 0f        , 19f       , 20f       , 21f       , 22f       , 23f       , 24f       , 25f       ,
                28f       , 0f        , 18f       , 17f       , 16f       , 0f        , 12f       , 11f       , 10f       , 9f        , 0f        , 0f        , 12f       , 13f       , 14f       , 15f       , 16f       , 0f        , 18f       , 19f       , 20f       , 21f       , 22f       , 23f       , 24f       ,
                27f       , 0f        , 19f       , 18f       , 17f       , 0f        , 11f       , 10f       , 9f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 21f       , 22f       , 23f       ,
                26f       , 0f        , 20f       , 19f       , 18f       , 0f        , 10f       , 9f        , 8f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 21f       , 22f       ,
                25f       , 0f        , 21f       , 20f       , 19f       , 0f        , 9f        , 8f        , 7f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 21f       ,
                24f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 5f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 21f       , 22f       ,
                23f       , 22f       , 21f       , 22f       , 0f        , 2f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 0f        , 22f       , 23f       ,
                22f       , 21f       , 20f       , 21f       , 0f        , 1f        , 0f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 0f        , 23f       , 24f       ,
                21f       , 20f       , 19f       , 20f       , 0f        , 2f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 0f        , 0f        , 0f        , 0f        , 16f       , 0f        , 24f       , 25f       ,
                0f        , 0f        , 18f       , 19f       , 0f        , 0f        , 0f        , 3f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 10f       , 11f       , 12f       , 0f        , 20f       , 19f       , 18f       , 17f       , 0f        , 25f       , 26f       ,
                17f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 0f        , 11f       , 12f       , 13f       , 0f        , 21f       , 20f       , 19f       , 18f       , 0f        , 26f       , 27f       ,
                16f       , 15f       , 16f       , 17f       , 18f       , 0f        , 6f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 0f        , 12f       , 13f       , 14f       , 0f        , 22f       , 21f       , 20f       , 19f       , 0f        , 27f       , 28f       ,
                15f       , 14f       , 0f        , 0f        , 0f        , 0f        , 0f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 0f        , 13f       , 14f       , 15f       , 0f        , 23f       , 22f       , 21f       , 20f       , 0f        , 26f       , 27f       ,
                14f       , 13f       , 12f       , 11f       , 10f       , 9f        , 8f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 0f        , 14f       , 15f       , 16f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 25f       , 26f       ,
                15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 9f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 0f        , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 21f       , 22f       , 23f       , 24f       , 25f       ,
                16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 0f        , 16f       , 17f       , 18f       , 19f       , 20f       , 21f       , 22f       , 23f       , 24f       , 25f       , 26f       ,
                17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 0f        , 17f       , 18f       , 19f       , 20f       , 21f       , 22f       , 23f       , 24f       , 25f       , 26f       , 27f       ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Approximately(expectedDistanceMap[i], actualDistance), $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void GenerateDijkstraGrid_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↘'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↙'],
                _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↙'],
                _dirs['↗'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                21.8995f  , 0f        , 14.72793f , 14.31371f , 13.8995f  , 13.48528f , 13.07107f , 12.65686f , 12.24264f , 11.82843f , 12.24264f , 12.65686f , 13.07107f , 13.48528f , 13.8995f  , 14.31371f , 14.72793f , 0f        , 18.48528f , 18.8995f  , 19.31371f , 19.72792f , 20.14214f , 21.14214f , 22.14214f ,
                20.8995f  , 0f        , 14.31371f , 13.31371f , 12.8995f  , 12.48528f , 12.07107f , 11.65686f , 11.24264f , 10.82843f , 11.24264f , 12.24264f , 12.07107f , 12.48528f , 12.8995f  , 13.31371f , 13.72793f , 0f        , 17.48528f , 17.8995f  , 18.31371f , 18.72793f , 19.72792f , 20.72792f , 21.72792f ,
                19.8995f  , 0f        , 13.8995f  , 12.8995f  , 11.8995f  , 11.48528f , 11.07107f , 10.65686f , 10.24264f , 9.828428f , 0f        , 0f        , 11.07107f , 11.48528f , 11.8995f  , 12.31371f , 13.31371f , 0f        , 16.48528f , 16.8995f  , 17.31371f , 18.31371f , 19.31371f , 20.31371f , 21.31371f ,
                18.8995f  , 0f        , 13.48528f , 12.48528f , 11.48528f , 10.48528f , 10.07107f , 9.656857f , 9.242642f , 8.828428f , 0f        , 0f        , 10.07107f , 10.48528f , 10.8995f  , 11.8995f  , 12.8995f  , 0f        , 15.48528f , 15.8995f  , 16.8995f  , 17.8995f  , 18.8995f  , 19.8995f  , 20.8995f  ,
                17.8995f  , 0f        , 13.8995f  , 12.8995f  , 11.8995f  , 0f        , 9.071071f , 8.656857f , 8.242642f , 7.828428f , 0f        , 0f        , 9.071071f , 9.485285f , 10.48528f , 11.48528f , 12.48528f , 0f        , 14.48528f , 15.48528f , 16.48528f , 17.48528f , 18.48528f , 19.48528f , 20.48528f ,
                16.8995f  , 0f        , 14.31371f , 13.31371f , 12.8995f  , 0f        , 8.071071f , 7.656857f , 7.242642f , 6.828428f , 7.242642f , 7.656857f , 8.071071f , 9.071071f , 10.07107f , 11.07107f , 12.07107f , 13.07107f , 14.07107f , 15.07107f , 16.07107f , 17.07107f , 18.07107f , 19.07107f , 20.07107f ,
                15.8995f  , 0f        , 14.72793f , 14.31371f , 13.8995f  , 0f        , 7.656857f , 6.656857f , 6.242642f , 5.828428f , 6.242642f , 6.656857f , 7.656857f , 8.656857f , 9.656857f , 10.65686f , 11.65686f , 12.65686f , 13.65686f , 14.65686f , 15.65686f , 16.65686f , 17.65685f , 18.65685f , 19.65685f ,
                14.8995f  , 0f        , 15.72793f , 15.31371f , 14.8995f  , 0f        , 7.242642f , 6.242642f , 5.242642f , 4.828428f , 5.242642f , 6.242642f , 7.242642f , 8.242642f , 9.242642f , 10.24264f , 11.24264f , 12.24264f , 13.24264f , 14.24264f , 15.24264f , 16.24264f , 17.24264f , 18.24264f , 19.24264f ,
                13.8995f  , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 3.828428f , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 18.65685f , 19.65685f ,
                12.8995f  , 12.48528f , 12.07107f , 11.65686f , 0f        , 1.414214f , 1f        , 1.414214f , 2.414214f , 3.414214f , 4.414214f , 5.414214f , 6.414214f , 7.414214f , 8.414214f , 9.414214f , 10.41421f , 11.41421f , 12.41421f , 13.41421f , 14.41421f , 15.41421f , 0f        , 19.65685f , 20.07107f ,
                12.48528f , 11.48528f , 11.07107f , 10.65686f , 0f        , 1f        , 0f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 0f        , 20.65685f , 21.07107f ,
                12.07107f , 11.07107f , 10.07107f , 9.656857f , 0f        , 1.414214f , 1f        , 1.414214f , 2.414214f , 3.414214f , 4.414214f , 5.414214f , 6.414214f , 7.414214f , 8.414214f , 9.414214f , 10.41421f , 0f        , 0f        , 0f        , 0f        , 15.41421f , 0f        , 21.65685f , 22.07107f ,
                0f        , 0f        , 9.656857f , 8.656857f , 0f        , 0f        , 0f        , 2.414214f , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 8.828428f , 9.828428f , 10.82843f , 0f        , 18.82843f , 17.82843f , 16.82843f , 16.41422f , 0f        , 22.65685f , 23.07107f ,
                11.24264f , 10.24264f , 9.242642f , 8.242642f , 7.242642f , 6.242642f , 0f        , 3.414214f , 3.828428f , 4.828428f , 5.828428f , 6.828428f , 7.828428f , 0f        , 9.828428f , 10.24264f , 11.24264f , 0f        , 19.24264f , 18.24264f , 17.82843f , 17.41422f , 0f        , 23.65685f , 24.07107f ,
                11.65686f , 10.65686f , 9.656857f , 8.656857f , 7.656857f , 0f        , 4.828428f , 4.414214f , 4.828428f , 5.242642f , 6.242642f , 7.242642f , 8.242642f , 0f        , 10.82843f , 11.24264f , 11.65686f , 0f        , 19.65685f , 19.24264f , 18.82843f , 18.41422f , 0f        , 23.48528f , 23.8995f  ,
                12.07107f , 11.07107f , 0f        , 0f        , 0f        , 0f        , 0f        , 5.414214f , 5.828428f , 6.242642f , 6.656857f , 7.656857f , 8.656857f , 0f        , 11.82843f , 12.24264f , 12.65686f , 0f        , 20.65685f , 20.24264f , 19.82843f , 19.41422f , 0f        , 22.48528f , 22.8995f  ,
                12.48528f , 11.82843f , 10.82843f , 9.828428f , 8.828428f , 7.828428f , 6.828428f , 6.414214f , 6.828428f , 7.242642f , 7.656857f , 8.071071f , 9.071071f , 0f        , 12.82843f , 13.24264f , 13.65686f , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 21.48528f , 22.48528f ,
                13.24264f , 12.24264f , 11.24264f , 10.24264f , 9.242642f , 8.242642f , 7.828428f , 7.414214f , 7.828428f , 8.242642f , 8.656857f , 9.071071f , 9.485285f , 0f        , 13.82843f , 14.24264f , 14.65686f , 15.07107f , 16.07107f , 17.07107f , 18.07107f , 19.07107f , 20.07107f , 21.07107f , 22.07107f ,
                13.65686f , 12.65686f , 11.65686f , 10.65686f , 9.656857f , 9.242642f , 8.828428f , 8.414214f , 8.828428f , 9.242642f , 9.656857f , 10.07107f , 10.48528f , 0f        , 14.82843f , 15.24264f , 15.65686f , 16.07107f , 16.48528f , 17.48528f , 18.48528f , 19.48528f , 20.48528f , 21.48528f , 22.48528f ,
                14.07107f , 13.07107f , 12.07107f , 11.07107f , 10.65686f , 10.24264f , 9.828428f , 9.414214f , 9.828428f , 10.24264f , 10.65686f , 11.07107f , 11.48528f , 0f        , 15.82843f , 16.24264f , 16.65686f , 17.07107f , 17.48528f , 17.8995f  , 18.8995f  , 19.8995f  , 20.8995f  , 21.8995f  , 22.8995f  ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Abs(expectedDistanceMap[i] - actualDistance) < 0.01f, $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void GenerateDijkstraGrid_OneFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↘'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['→'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↙'],
                _dirs['↘'], _dirs['↘'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↙'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                28.48528000f, 0f        , 14.72793000f, 14.31371000f, 13.89950000f, 13.48528000f, 13.07107000f, 12.65686000f, 12.24264000f, 11.82843000f, 12.24264000f, 12.65686000f, 13.07107000f, 13.48528000f, 13.89950000f, 14.31371000f, 14.72793000f, 0f        , 18.48528000f, 18.89950000f, 19.31371000f, 19.72792000f, 20.14214000f, 21.14214000f, 22.14214000f,
                27.48528000f, 0f        , 14.31371000f, 13.31371000f, 12.89950000f, 12.48528000f, 12.07107000f, 11.65686000f, 11.24264000f, 10.82843000f, 11.24264000f, 12.24264000f, 12.07107000f, 12.48528000f, 12.89950000f, 13.31371000f, 13.72793000f, 0f        , 17.48528000f, 17.89950000f, 18.31371000f, 18.72793000f, 19.72792000f, 20.72792000f, 21.72792000f,
                26.48528000f, 0f        , 13.89950000f, 12.89950000f, 11.89950000f, 11.48528000f, 11.07107000f, 10.65686000f, 10.24264000f, 9.82842800f, 0f        , 0f        , 11.07107000f, 11.48528000f, 11.89950000f, 12.31371000f, 13.31371000f, 0f        , 16.48528000f, 16.89950000f, 17.31371000f, 18.31371000f, 19.31371000f, 20.31371000f, 21.31371000f,
                25.48528000f, 0f        , 13.48528000f, 12.48528000f, 11.48528000f, 10.48528000f, 10.07107000f, 9.65685700f, 9.24264200f, 8.82842800f, 0f        , 0f        , 10.07107000f, 10.48528000f, 10.89950000f, 11.89950000f, 12.89950000f, 0f        , 15.48528000f, 15.89950000f, 16.89950000f, 17.89950000f, 18.89950000f, 19.89950000f, 20.89950000f,
                24.48528000f, 0f        , 13.89950000f, 12.89950000f, 11.89950000f, 0f        , 9.07107100f, 8.65685700f, 8.24264200f, 7.82842800f, 0f        , 0f        , 9.07107100f, 9.48528500f, 10.48528000f, 11.48528000f, 12.48528000f, 0f        , 14.48528000f, 15.48528000f, 16.48528000f, 17.48528000f, 18.48528000f, 19.48528000f, 20.48528000f,
                23.48528000f, 0f        , 14.31371000f, 13.31371000f, 12.89950000f, 0f        , 8.07107100f, 7.65685700f, 7.24264200f, 6.82842800f, 7.24264200f, 7.65685700f, 8.07107100f, 9.07107100f, 10.07107000f, 11.07107000f, 12.07107000f, 13.07107000f, 14.07107000f, 15.07107000f, 16.07107000f, 17.07107000f, 18.07107000f, 19.07107000f, 20.07107000f,
                22.48528000f, 0f        , 14.72793000f, 14.31371000f, 13.89950000f, 0f        , 7.65685700f, 6.65685700f, 6.24264200f, 5.82842800f, 6.24264200f, 6.65685700f, 7.65685700f, 8.65685700f, 9.65685700f, 10.65686000f, 11.65686000f, 12.65686000f, 13.65686000f, 14.65686000f, 15.65686000f, 16.65686000f, 17.65685000f, 18.65685000f, 19.65685000f,
                21.48528000f, 0f        , 15.72793000f, 15.31371000f, 14.89950000f, 0f        , 7.24264200f, 6.24264200f, 5.24264200f, 4.82842800f, 5.24264200f, 6.24264200f, 7.24264200f, 8.24264200f, 9.24264200f, 10.24264000f, 11.24264000f, 12.24264000f, 13.24264000f, 14.24264000f, 15.24264000f, 16.24264000f, 17.24264000f, 18.24264000f, 19.24264000f,
                20.48528000f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 3.82842800f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 18.65685000f, 19.65685000f,
                19.48528000f, 19.07107000f, 18.65686000f, 19.07107000f, 0f        , 1.41421400f, 1.00000000f, 1.41421400f, 2.41421400f, 3.41421400f, 4.41421400f, 5.41421400f, 6.41421400f, 7.41421400f, 8.41421400f, 9.41421400f, 10.41421000f, 11.41421000f, 12.41421000f, 13.41421000f, 14.41421000f, 15.41421000f, 0f        , 19.65685000f, 20.07107000f,
                18.48528000f, 18.07107000f, 17.65686000f, 18.07107000f, 0f        , 1.00000000f, 0.00000000f, 1.00000000f, 2.00000000f, 3.00000000f, 4.00000000f, 5.00000000f, 6.00000000f, 7.00000000f, 8.00000000f, 9.00000000f, 10.00000000f, 11.00000000f, 12.00000000f, 13.00000000f, 14.00000000f, 15.00000000f, 0f        , 20.65685000f, 21.07107000f,
                18.07107000f, 17.07107000f, 16.65686000f, 17.07107000f, 0f        , 1.41421400f, 1.00000000f, 1.41421400f, 2.41421400f, 3.41421400f, 4.41421400f, 5.41421400f, 6.41421400f, 7.41421400f, 8.41421400f, 9.41421400f, 10.41421000f, 0f        , 0f        , 0f        , 0f        , 15.41421000f, 0f        , 21.65685000f, 22.07107000f,
                0f        , 0f        , 15.65686000f, 16.07107000f, 0f        , 0f        , 0f        , 2.41421400f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 8.82842800f, 9.82842800f, 10.82843000f, 0f        , 18.82843000f, 17.82843000f, 16.82843000f, 16.41422000f, 0f        , 22.65685000f, 23.07107000f,
                14.65686000f, 14.24264000f, 14.65686000f, 15.07107000f, 16.07107000f, 17.07107000f, 0f        , 3.41421400f, 3.82842800f, 4.82842800f, 5.82842800f, 6.82842800f, 7.82842800f, 0f        , 9.82842800f, 10.24264000f, 11.24264000f, 0f        , 19.24264000f, 18.24264000f, 17.82843000f, 17.41422000f, 0f        , 23.65685000f, 24.07107000f,
                13.65686000f, 13.24264000f, 13.65686000f, 14.65686000f, 15.65686000f, 0f        , 4.82842800f, 4.41421400f, 4.82842800f, 5.24264200f, 6.24264200f, 7.24264200f, 8.24264200f, 0f        , 10.82843000f, 11.24264000f, 11.65686000f, 0f        , 19.65685000f, 19.24264000f, 18.82843000f, 18.41422000f, 0f        , 23.48528000f, 23.89950000f,
                13.24264000f, 12.24264000f, 0f        , 0f        , 0f        , 0f        , 0f        , 5.41421400f, 5.82842800f, 6.24264200f, 6.65685700f, 7.65685700f, 8.65685700f, 0f        , 11.82843000f, 12.24264000f, 12.65686000f, 0f        , 20.65685000f, 20.24264000f, 19.82843000f, 19.41422000f, 0f        , 22.48528000f, 22.89950000f,
                12.82843000f, 11.82843000f, 10.82843000f, 9.82842800f, 8.82842800f, 7.82842800f, 6.82842800f, 6.41421400f, 6.82842800f, 7.24264200f, 7.65685700f, 8.07107100f, 9.07107100f, 0f        , 12.82843000f, 13.24264000f, 13.65686000f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 21.48528000f, 22.48528000f,
                13.24264000f, 12.24264000f, 11.24264000f, 10.24264000f, 9.24264200f, 8.24264200f, 7.82842800f, 7.41421400f, 7.82842800f, 8.24264200f, 8.65685700f, 9.07107100f, 9.48528500f, 0f        , 13.82843000f, 14.24264000f, 14.65686000f, 15.07107000f, 16.07107000f, 17.07107000f, 18.07107000f, 19.07107000f, 20.07107000f, 21.07107000f, 22.07107000f,
                13.65686000f, 12.65686000f, 11.65686000f, 10.65686000f, 9.65685700f, 9.24264200f, 8.82842800f, 8.41421400f, 8.82842800f, 9.24264200f, 9.65685700f, 10.07107000f, 10.48528000f, 0f        , 14.82843000f, 15.24264000f, 15.65686000f, 16.07107000f, 16.48528000f, 17.48528000f, 18.48528000f, 19.48528000f, 20.48528000f, 21.48528000f, 22.48528000f,
                14.07107000f, 13.07107000f, 12.07107000f, 11.07107000f, 10.65686000f, 10.24264000f, 9.82842800f, 9.41421400f, 9.82842800f, 10.24264000f, 10.65686000f, 11.07107000f, 11.48528000f, 0f        , 15.82843000f, 16.24264000f, 16.65686000f, 17.07107000f, 17.48528000f, 17.89950000f, 18.89950000f, 19.89950000f, 20.89950000f, 21.89950000f, 22.89950000f,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Abs(expectedDistanceMap[i] - actualDistance) < 0.01f, $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void GenerateDijkstraGrid_TwoFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, GridUtils.GetTile(_grid, targetX, targetY), DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↖'],
                _dirs['↘'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↙'],
                _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↙'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                29.65685000f, 0f        , 15.89950000f, 14.89950000f, 14.48528000f, 14.07107000f, 13.65686000f, 13.24264000f, 12.82843000f, 12.41421000f, 12.82843000f, 13.82843000f, 14.24264000f, 14.65686000f, 15.07107000f, 15.48528000f, 15.89950000f, 0f        , 20.24264000f, 20.65685000f, 21.07107000f, 21.48528000f, 21.89949000f, 22.31371000f, 23.31371000f,
                28.65685000f, 0f        , 15.48528000f, 14.48528000f, 13.48528000f, 13.07107000f, 12.65686000f, 12.24264000f, 11.82843000f, 11.41421000f, 12.41421000f, 13.41421000f, 13.24264000f, 13.65686000f, 14.07107000f, 14.48528000f, 14.89950000f, 0f        , 19.24264000f, 19.65685000f, 20.07107000f, 20.48528000f, 20.89950000f, 21.89949000f, 22.89949000f,
                27.65685000f, 0f        , 15.07107000f, 14.07107000f, 13.07107000f, 12.07107000f, 11.65686000f, 11.24264000f, 10.82843000f, 10.41421000f, 0f        , 0f        , 12.24264000f, 12.65686000f, 13.07107000f, 13.48528000f, 14.48528000f, 0f        , 18.24264000f, 18.65685000f, 19.07107000f, 19.48528000f, 20.48528000f, 21.48528000f, 22.48528000f,
                26.65685000f, 0f        , 14.65686000f, 13.65686000f, 12.65686000f, 11.65686000f, 10.65686000f, 10.24264000f, 9.82842800f, 9.41421400f, 0f        , 0f        , 11.24264000f, 11.65686000f, 12.07107000f, 13.07107000f, 14.07107000f, 0f        , 17.24264000f, 17.65685000f, 18.07107000f, 19.07107000f, 20.07107000f, 21.07107000f, 22.07107000f,
                25.65685000f, 0f        , 15.07107000f, 14.07107000f, 13.65686000f, 0f        , 9.65685700f, 9.24264200f, 8.82842800f, 8.41421400f, 0f        , 0f        , 10.24264000f, 10.65686000f, 11.65686000f, 12.65686000f, 13.65686000f, 0f        , 16.24264000f, 16.65686000f, 17.65685000f, 18.65685000f, 19.65685000f, 20.65685000f, 21.65685000f,
                24.65685000f, 0f        , 15.48528000f, 15.07107000f, 14.65686000f, 0f        , 9.24264200f, 8.24264200f, 7.82842800f, 7.41421400f, 7.82842800f, 8.24264200f, 9.24264200f, 10.24264000f, 11.24264000f, 12.24264000f, 13.24264000f, 14.24264000f, 15.24264000f, 16.24264000f, 17.24264000f, 18.24264000f, 19.24264000f, 20.24264000f, 21.24264000f,
                23.65685000f, 0f        , 16.48528000f, 16.07107000f, 15.65686000f, 0f        , 8.82842800f, 7.82842800f, 6.82842800f, 6.41421400f, 6.82842800f, 7.82842800f, 8.82842800f, 9.82842800f, 10.82843000f, 11.82843000f, 12.82843000f, 13.82843000f, 14.82843000f, 15.82843000f, 16.82843000f, 17.82843000f, 18.82843000f, 19.82843000f, 20.82843000f,
                22.65685000f, 0f        , 17.48528000f, 17.07107000f, 16.65686000f, 0f        , 8.41421400f, 7.41421400f, 6.41421400f, 5.41421400f, 6.41421400f, 7.41421400f, 8.41421400f, 9.41421400f, 10.41421000f, 11.41421000f, 12.41421000f, 13.41421000f, 14.41421000f, 15.41421000f, 16.41422000f, 17.41422000f, 18.41422000f, 19.41422000f, 20.41422000f,
                21.65685000f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 4.41421400f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 20.41422000f, 20.82843000f,
                20.65685000f, 20.24264000f, 19.82843000f, 20.24264000f, 0f        , 1.41421400f, 1.00000000f, 1.41421400f, 2.41421400f, 3.41421400f, 4.41421400f, 5.41421400f, 6.41421400f, 7.41421400f, 8.41421400f, 9.41421400f, 10.41421000f, 11.41421000f, 12.41421000f, 13.41421000f, 14.41421000f, 15.41421000f, 0f        , 21.41422000f, 21.82843000f,
                20.24264000f, 19.24264000f, 18.82843000f, 19.24264000f, 0f        , 1.00000000f, 0.00000000f, 1.00000000f, 2.00000000f, 3.00000000f, 4.00000000f, 5.00000000f, 6.00000000f, 7.00000000f, 8.00000000f, 9.00000000f, 10.00000000f, 11.00000000f, 12.00000000f, 13.00000000f, 14.00000000f, 15.00000000f, 0f        , 22.41422000f, 22.82843000f,
                19.82843000f, 18.82843000f, 17.82843000f, 18.24264000f, 0f        , 1.41421400f, 1.00000000f, 1.41421400f, 2.41421400f, 3.41421400f, 4.41421400f, 5.41421400f, 6.41421400f, 7.41421400f, 8.41421400f, 9.41421400f, 10.41421000f, 0f        , 0f        , 0f        , 0f        , 16.00000000f, 0f        , 23.41422000f, 23.82843000f,
                0f        , 0f        , 16.82843000f, 17.24264000f, 0f        , 0f        , 0f        , 2.41421400f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 9.41421400f, 9.82842800f, 10.82843000f, 0f        , 20.00000000f, 19.00000000f, 18.00000000f, 17.00000000f, 0f        , 24.41422000f, 24.82843000f,
                15.82843000f, 15.41421000f, 15.82843000f, 16.82843000f, 17.82843000f, 18.82843000f, 0f        , 3.41421400f, 4.41421400f, 5.41421400f, 6.41421400f, 7.41421400f, 8.41421400f, 0f        , 10.41421000f, 10.82843000f, 11.24264000f, 0f        , 20.41421000f, 19.41421000f, 18.41421000f, 18.00000000f, 0f        , 25.41422000f, 25.82843000f,
                14.82843000f, 14.41421000f, 15.41421000f, 16.41422000f, 17.41422000f, 0f        , 5.41421400f, 4.41421400f, 4.82842800f, 5.82842800f, 6.82842800f, 7.82842800f, 8.82842800f, 0f        , 11.41421000f, 11.82843000f, 12.24264000f, 0f        , 20.82843000f, 19.82843000f, 19.41421000f, 19.00000000f, 0f        , 25.24264000f, 25.65685000f,
                13.82843000f, 13.41421000f, 0f        , 0f        , 0f        , 0f        , 0f        , 5.41421400f, 5.82842800f, 6.24264200f, 7.24264200f, 8.24264200f, 9.24264200f, 0f        , 12.41421000f, 12.82843000f, 13.24264000f, 0f        , 21.24264000f, 20.82843000f, 20.41421000f, 20.00000000f, 0f        , 24.24264000f, 24.65685000f,
                13.41421000f, 12.41421000f, 11.41421000f, 10.41421000f, 9.41421400f, 8.41421400f, 7.41421400f, 6.41421400f, 6.82842800f, 7.24264200f, 7.65685700f, 8.65685700f, 9.65685700f, 0f        , 13.41421000f, 13.82843000f, 14.24264000f, 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 23.24264000f, 23.65685000f,
                13.82843000f, 12.82843000f, 11.82843000f, 10.82843000f, 9.82842800f, 8.82842800f, 7.82842800f, 7.41421400f, 7.82842800f, 8.24264200f, 8.65685700f, 9.07107100f, 10.07107000f, 0f        , 14.41421000f, 14.82843000f, 15.24264000f, 16.24264000f, 17.24264000f, 18.24264000f, 19.24264000f, 20.24264000f, 21.24264000f, 22.24264000f, 23.24264000f,
                14.24264000f, 13.24264000f, 12.24264000f, 11.24264000f, 10.24264000f, 9.24264200f, 8.82842800f, 8.41421400f, 8.82842800f, 9.24264200f, 9.65685700f, 10.07107000f, 10.48528000f, 0f        , 15.41421000f, 15.82843000f, 16.24264000f, 16.65686000f, 17.65685000f, 18.65685000f, 19.65685000f, 20.65685000f, 21.65685000f, 22.65685000f, 23.65685000f,
                14.65686000f, 13.65686000f, 12.65686000f, 11.65686000f, 10.65686000f, 10.24264000f, 9.82842800f, 9.41421400f, 9.82842800f, 10.24264000f, 10.65686000f, 11.07107000f, 11.48528000f, 0f        , 16.41422000f, 16.82843000f, 17.24264000f, 17.65685000f, 18.07107000f, 19.07107000f, 20.07107000f, 21.07107000f, 22.07107000f, 23.07107000f, 24.07107000f,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Abs(expectedDistanceMap[i] - actualDistance) < 0.01f, $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void DijkstraGrid_GetTargetTile()
        {
            int targetX = 6;
            int targetY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target);
            TestTile returnedTarget = dirMap.GetTargetTile(_grid);
            Assert.IsTrue(GridUtils.TileEquals(target, returnedTarget), $"Target is [{target.X}, {target.Y}] but returned tile is {returnedTarget.X}, {returnedTarget.Y}");
        }
        [Test]
        public void DijkstraGrid_IsTileAccessible()
        {
            int targetX = 6;
            int targetY = 10;
            int accessibleTileX = 2;
            int accessibleTileY = 1;
            int inaccessibleTileX = 1;
            int inaccessibleTileY = 3;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target);
            Assert.IsTrue(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, accessibleTileX, accessibleTileY)));
            Assert.IsFalse(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, inaccessibleTileX, inaccessibleTileY)));
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DijkstraGrid_GetPathToTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.NONE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathToTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                startTile, new(2,11), new(2,12), new(2,13), new(2,14), new(1,14), new(1,15), new(1,16), new(2,16), new(3,16), new(4,16), new(5,16), new(6,16), new(7,16), new(7,15), new(7,14), new(7,13), new(7,12), new(7,11), new(6,11), targetTile
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
        public void DijkstraGrid_GetPathToTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DijkstraGrid_GetPathToTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DijkstraGrid_GetPathToTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DijkstraGrid_GetPathFromTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.NONE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathFromTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                targetTile, new(6,11), new(7,11), new(7,12), new(7,13), new(7,14), new(7,15), new(7,16), new(6,16), new(5,16), new(4,16), new(3,16), new(2,16), new(1,16), new(1,15), new(1,14), new(2,14), new(2,13), new(2,12), new(2,11), startTile
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
        public void DijkstraGrid_GetPathFromTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DijkstraGrid_GetPathFromTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DijkstraGrid_GetPathFromTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DijkstraGrid_GetNextTileFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.NONE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(4, 13);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraGrid_GetNextTileFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(6, 14);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraGrid_GetNextTileFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(2, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraGrid_GetNextTileFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(1, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraGrid_GetNextTileDirectionFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.NONE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.LEFT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraGrid_GetNextTileDirectionFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraGrid_GetNextTileDirectionFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraGrid_GetNextTileDirectionFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target, DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraGrid_Serialization()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 10;
            int startY = 17;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target);
            byte[] serializedDirMap = dirMap.ToByteArray();
            DijkstraGrid deserializedDirMap = DijkstraGrid.FromByteArray(_grid, serializedDirMap);
            Assert.AreEqual(dirMap._directionGrid.Length, deserializedDirMap._directionGrid.Length);
            for (int i = 0; i < deserializedDirMap._directionGrid.Length; i++)
            {
                Assert.AreEqual(dirMap._directionGrid[i], deserializedDirMap._directionGrid[i]);
            }
        }
        [Test]
        public void DijkstraGrid_GetDistanceToTarget()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            DijkstraGrid dirMap = Pathfinding.GenerateDijkstraGrid(_grid, target);
            Assert.IsTrue(Mathf.Approximately(20f, dirMap.GetDistanceToTarget(_grid, start)));
        }

        #endregion

        #region DijkstraField

        [Test]
        public void GenerateDijkstraField_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.NONE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['0'], _dirs['0'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['0'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['↓'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['↓'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['→'], _dirs['↑'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['↑'], _dirs['↑'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↑'], _dirs['↑'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['←'], _dirs['↑'], _dirs['↑'], _dirs['↑'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                0f        , 0f        , 20f       , 19f       , 18f       , 17f       , 16f       , 15f       , 14f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
                0f        , 0f        , 19f       , 18f       , 17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
                0f        , 0f        , 18f       , 17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 0f        , 0f        , 14f       , 15f       , 16f       , 17f       , 18f       , 0f        , 20f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
                0f        , 0f        , 17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 0f        , 0f        , 13f       , 14f       , 15f       , 16f       , 17f       , 0f        , 19f       , 20f       , 0f        , 0f        , 0f        , 0f        , 0f        ,
                0f        , 0f        , 18f       , 17f       , 16f       , 0f        , 12f       , 11f       , 10f       , 9f        , 0f        , 0f        , 12f       , 13f       , 14f       , 15f       , 16f       , 0f        , 18f       , 19f       , 20f       , 0f        , 0f        , 0f        , 0f        ,
                0f        , 0f        , 19f       , 18f       , 17f       , 0f        , 11f       , 10f       , 9f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 0f        , 0f        ,
                0f        , 0f        , 20f       , 19f       , 18f       , 0f        , 10f       , 9f        , 8f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 0f        ,
                0f        , 0f        , 0f        , 20f       , 19f       , 0f        , 9f        , 8f        , 7f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        ,
                0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 5f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
                0f        , 0f        , 0f        , 0f        , 0f        , 2f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 16f       , 0f        , 0f        , 0f        ,
                0f        , 0f        , 20f       , 0f        , 0f        , 1f        , 0f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 0f        , 0f        , 0f        ,
                0f        , 20f       , 19f       , 20f       , 0f        , 2f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 0f        , 0f        , 0f        , 0f        , 16f       , 0f        , 0f        , 0f        ,
                0f        , 0f        , 18f       , 19f       , 0f        , 0f        , 0f        , 3f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 10f       , 11f       , 12f       , 0f        , 20f       , 19f       , 18f       , 17f       , 0f        , 0f        , 0f        ,
                17f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 0f        , 11f       , 12f       , 13f       , 0f        , 0f        , 20f       , 19f       , 18f       , 0f        , 0f        , 0f        ,
                16f       , 15f       , 16f       , 17f       , 18f       , 0f        , 6f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 0f        , 12f       , 13f       , 14f       , 0f        , 0f        , 0f        , 20f       , 19f       , 0f        , 0f        , 0f        ,
                15f       , 14f       , 0f        , 0f        , 0f        , 0f        , 0f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 0f        , 13f       , 14f       , 15f       , 0f        , 0f        , 0f        , 0f        , 20f       , 0f        , 0f        , 0f        ,
                14f       , 13f       , 12f       , 11f       , 10f       , 9f        , 8f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 0f        , 14f       , 15f       , 16f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
                15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 9f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 0f        , 15f       , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 0f        , 0f        , 0f        , 0f        ,
                16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 0f        , 16f       , 17f       , 18f       , 19f       , 20f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
                17f       , 16f       , 15f       , 14f       , 13f       , 12f       , 11f       , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 0f        , 17f       , 18f       , 19f       , 20f       , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Approximately(expectedDistanceMap[i], actualDistance), $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void GenerateDijkstraField_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'],
                _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'],
                _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['↘'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↑'], _dirs['0'],
                _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                0f        , 0f        , 14.72793f , 14.31371f , 13.8995f  , 13.48528f , 13.07107f , 12.65686f , 12.24264f , 11.82843f , 12.24264f , 12.65686f , 13.07107f , 13.48528f , 13.8995f  , 14.31371f , 14.72793f , 0f        , 18.48528f , 18.8995f  , 19.31371f , 19.72792f , 0f        , 0f        , 0f        ,
                0f        , 0f        , 14.31371f , 13.31371f , 12.8995f  , 12.48528f , 12.07107f , 11.65686f , 11.24264f , 10.82843f , 11.24264f , 12.24264f , 12.07107f , 12.48528f , 12.8995f  , 13.31371f , 13.72793f , 0f        , 17.48528f , 17.8995f  , 18.31371f , 18.72793f , 19.72792f , 0f        , 0f        ,
                19.8995f  , 0f        , 13.8995f  , 12.8995f  , 11.8995f  , 11.48528f , 11.07107f , 10.65686f , 10.24264f , 9.828428f , 0f        , 0f        , 11.07107f , 11.48528f , 11.8995f  , 12.31371f , 13.31371f , 0f        , 16.48528f , 16.8995f  , 17.31371f , 18.31371f , 19.31371f , 0f        , 0f        ,
                18.8995f  , 0f        , 13.48528f , 12.48528f , 11.48528f , 10.48528f , 10.07107f , 9.656857f , 9.242642f , 8.828428f , 0f        , 0f        , 10.07107f , 10.48528f , 10.8995f  , 11.8995f  , 12.8995f  , 0f        , 15.48528f , 15.8995f  , 16.8995f  , 17.8995f  , 18.8995f  , 19.8995f  , 0f        ,
                17.8995f  , 0f        , 13.8995f  , 12.8995f  , 11.8995f  , 0f        , 9.071071f , 8.656857f , 8.242642f , 7.828428f , 0f        , 0f        , 9.071071f , 9.485285f , 10.48528f , 11.48528f , 12.48528f , 0f        , 14.48528f , 15.48528f , 16.48528f , 17.48528f , 18.48528f , 19.48528f , 0f        ,
                16.8995f  , 0f        , 14.31371f , 13.31371f , 12.8995f  , 0f        , 8.071071f , 7.656857f , 7.242642f , 6.828428f , 7.242642f , 7.656857f , 8.071071f , 9.071071f , 10.07107f , 11.07107f , 12.07107f , 13.07107f , 14.07107f , 15.07107f , 16.07107f , 17.07107f , 18.07107f , 19.07107f , 0f        ,
                15.8995f  , 0f        , 14.72793f , 14.31371f , 13.8995f  , 0f        , 7.656857f , 6.656857f , 6.242642f , 5.828428f , 6.242642f , 6.656857f , 7.656857f , 8.656857f , 9.656857f , 10.65686f , 11.65686f , 12.65686f , 13.65686f , 14.65686f , 15.65686f , 16.65686f , 17.65685f , 18.65685f , 19.65685f ,
                14.8995f  , 0f        , 15.72793f , 15.31371f , 14.8995f  , 0f        , 7.242642f , 6.242642f , 5.242642f , 4.828428f , 5.242642f , 6.242642f , 7.242642f , 8.242642f , 9.242642f , 10.24264f , 11.24264f , 12.24264f , 13.24264f , 14.24264f , 15.24264f , 16.24264f , 17.24264f , 18.24264f , 19.24264f ,
                13.8995f  , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 3.828428f , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 18.65685f , 19.65685f ,
                12.8995f  , 12.48528f , 12.07107f , 11.65686f , 0f        , 1.414214f , 1f        , 1.414214f , 2.414214f , 3.414214f , 4.414214f , 5.414214f , 6.414214f , 7.414214f , 8.414214f , 9.414214f , 10.41421f , 11.41421f , 12.41421f , 13.41421f , 14.41421f , 15.41421f , 0f        , 19.65685f , 0f        ,
                12.48528f , 11.48528f , 11.07107f , 10.65686f , 0f        , 1f        , 0f        , 1f        , 2f        , 3f        , 4f        , 5f        , 6f        , 7f        , 8f        , 9f        , 10f       , 11f       , 12f       , 13f       , 14f       , 15f       , 0f        , 0f        , 0f        ,
                12.07107f , 11.07107f , 10.07107f , 9.656857f , 0f        , 1.414214f , 1f        , 1.414214f , 2.414214f , 3.414214f , 4.414214f , 5.414214f , 6.414214f , 7.414214f , 8.414214f , 9.414214f , 10.41421f , 0f        , 0f        , 0f        , 0f        , 15.41421f , 0f        , 0f        , 0f        ,
                0f        , 0f        , 9.656857f , 8.656857f , 0f        , 0f        , 0f        , 2.414214f , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 8.828428f , 9.828428f , 10.82843f , 0f        , 18.82843f , 17.82843f , 16.82843f , 16.41422f , 0f        , 0f        , 0f        ,
                11.24264f , 10.24264f , 9.242642f , 8.242642f , 7.242642f , 6.242642f , 0f        , 3.414214f , 3.828428f , 4.828428f , 5.828428f , 6.828428f , 7.828428f , 0f        , 9.828428f , 10.24264f , 11.24264f , 0f        , 19.24264f , 18.24264f , 17.82843f , 17.41422f , 0f        , 0f        , 0f        ,
                11.65686f , 10.65686f , 9.656857f , 8.656857f , 7.656857f , 0f        , 4.828428f , 4.414214f , 4.828428f , 5.242642f , 6.242642f , 7.242642f , 8.242642f , 0f        , 10.82843f , 11.24264f , 11.65686f , 0f        , 19.65685f , 19.24264f , 18.82843f , 18.41422f , 0f        , 0f        , 0f        ,
                12.07107f , 11.07107f , 0f        , 0f        , 0f        , 0f        , 0f        , 5.414214f , 5.828428f , 6.242642f , 6.656857f , 7.656857f , 8.656857f , 0f        , 11.82843f , 12.24264f , 12.65686f , 0f        , 0f        , 0f        , 19.82843f , 19.41422f , 0f        , 0f        , 0f        ,
                12.48528f , 11.82843f , 10.82843f , 9.828428f , 8.828428f , 7.828428f , 6.828428f , 6.414214f , 6.828428f , 7.242642f , 7.656857f , 8.071071f , 9.071071f , 0f        , 12.82843f , 13.24264f , 13.65686f , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        , 0f        ,
                13.24264f , 12.24264f , 11.24264f , 10.24264f , 9.242642f , 8.242642f , 7.828428f , 7.414214f , 7.828428f , 8.242642f , 8.656857f , 9.071071f , 9.485285f , 0f        , 13.82843f , 14.24264f , 14.65686f , 15.07107f , 16.07107f , 17.07107f , 18.07107f , 19.07107f , 0f        , 0f        , 0f        ,
                13.65686f , 12.65686f , 11.65686f , 10.65686f , 9.656857f , 9.242642f , 8.828428f , 8.414214f , 8.828428f , 9.242642f , 9.656857f , 10.07107f , 10.48528f , 0f        , 14.82843f , 15.24264f , 15.65686f , 16.07107f , 16.48528f , 17.48528f , 18.48528f , 19.48528f , 0f        , 0f        , 0f        ,
                14.07107f , 13.07107f , 12.07107f , 11.07107f , 10.65686f , 10.24264f , 9.828428f , 9.414214f , 9.828428f , 10.24264f , 10.65686f , 11.07107f , 11.48528f , 0f        , 15.82843f , 16.24264f , 16.65686f , 17.07107f , 17.48528f , 17.8995f  , 18.8995f  , 19.8995f  , 0f        , 0f        , 0f        ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Abs(expectedDistanceMap[i] - actualDistance) < 0.01f, $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void GenerateDijkstraField_OneFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'],
                _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'],
                _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↑'], _dirs['0'],
                _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↘'], _dirs['↘'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                0f          , 0f          , 14.72793000f, 14.31371000f, 13.89950000f, 13.48528000f, 13.07107000f, 12.65686000f, 12.24264000f, 11.82843000f, 12.24264000f, 12.65686000f, 13.07107000f, 13.48528000f, 13.89950000f, 14.31371000f, 14.72793000f, 0f          , 18.48528000f, 18.89950000f, 19.31371000f, 19.72792000f, 0f          , 0f          , 0f          ,
                0f          , 0f          , 14.31371000f, 13.31371000f, 12.89950000f, 12.48528000f, 12.07107000f, 11.65686000f, 11.24264000f, 10.82843000f, 11.24264000f, 12.24264000f, 12.07107000f, 12.48528000f, 12.89950000f, 13.31371000f, 13.72793000f, 0f          , 17.48528000f, 17.89950000f, 18.31371000f, 18.72793000f, 19.72792000f, 0f          , 0f          ,
                0f          , 0f          , 13.89950000f, 12.89950000f, 11.89950000f, 11.48528000f, 11.07107000f, 10.65686000f, 10.24264000f, 9.82842800f , 0f          , 0f          , 11.07107000f, 11.48528000f, 11.89950000f, 12.31371000f, 13.31371000f, 0f          , 16.48528000f, 16.89950000f, 17.31371000f, 18.31371000f, 19.31371000f, 0f          , 0f          ,
                0f          , 0f          , 13.48528000f, 12.48528000f, 11.48528000f, 10.48528000f, 10.07107000f, 9.65685700f , 9.24264200f , 8.82842800f , 0f          , 0f          , 10.07107000f, 10.48528000f, 10.89950000f, 11.89950000f, 12.89950000f, 0f          , 15.48528000f, 15.89950000f, 16.89950000f, 17.89950000f, 18.89950000f, 19.89950000f, 0f          ,
                0f          , 0f          , 13.89950000f, 12.89950000f, 11.89950000f, 0f          , 9.07107100f , 8.65685700f , 8.24264200f , 7.82842800f , 0f          , 0f          , 9.07107100f , 9.48528500f , 10.48528000f, 11.48528000f, 12.48528000f, 0f          , 14.48528000f, 15.48528000f, 16.48528000f, 17.48528000f, 18.48528000f, 19.48528000f, 0f          ,
                0f          , 0f          , 14.31371000f, 13.31371000f, 12.89950000f, 0f          , 8.07107100f , 7.65685700f , 7.24264200f , 6.82842800f , 7.24264200f , 7.65685700f , 8.07107100f , 9.07107100f , 10.07107000f, 11.07107000f, 12.07107000f, 13.07107000f, 14.07107000f, 15.07107000f, 16.07107000f, 17.07107000f, 18.07107000f, 19.07107000f, 0f          ,
                0f          , 0f          , 14.72793000f, 14.31371000f, 13.89950000f, 0f          , 7.65685700f , 6.65685700f , 6.24264200f , 5.82842800f , 6.24264200f , 6.65685700f , 7.65685700f , 8.65685700f , 9.65685700f , 10.65686000f, 11.65686000f, 12.65686000f, 13.65686000f, 14.65686000f, 15.65686000f, 16.65686000f, 17.65685000f, 18.65685000f, 19.65685000f,
                0f          , 0f          , 15.72793000f, 15.31371000f, 14.89950000f, 0f          , 7.24264200f , 6.24264200f , 5.24264200f , 4.82842800f , 5.24264200f , 6.24264200f , 7.24264200f , 8.24264200f , 9.24264200f , 10.24264000f, 11.24264000f, 12.24264000f, 13.24264000f, 14.24264000f, 15.24264000f, 16.24264000f, 17.24264000f, 18.24264000f, 19.24264000f,
                0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 3.82842800f , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 18.65685000f, 19.65685000f,
                19.48528000f, 19.07107000f, 18.65686000f, 19.07107000f, 0f          , 1.41421400f , 1.00000000f , 1.41421400f , 2.41421400f , 3.41421400f , 4.41421400f , 5.41421400f , 6.41421400f , 7.41421400f , 8.41421400f , 9.41421400f , 10.41421000f, 11.41421000f, 12.41421000f, 13.41421000f, 14.41421000f, 15.41421000f, 0f          , 19.65685000f, 0f          ,
                18.48528000f, 18.07107000f, 17.65686000f, 18.07107000f, 0f          , 1.00000000f , 0.00000000f , 1.00000000f , 2.00000000f , 3.00000000f , 4.00000000f , 5.00000000f , 6.00000000f , 7.00000000f , 8.00000000f , 9.00000000f , 10.00000000f, 11.00000000f, 12.00000000f, 13.00000000f, 14.00000000f, 15.00000000f, 0f          , 0f          , 0f          ,
                18.07107000f, 17.07107000f, 16.65686000f, 17.07107000f, 0f          , 1.41421400f , 1.00000000f , 1.41421400f , 2.41421400f , 3.41421400f , 4.41421400f , 5.41421400f , 6.41421400f , 7.41421400f , 8.41421400f , 9.41421400f , 10.41421000f, 0f          , 0f          , 0f          , 0f          , 15.41421000f, 0f          , 0f          , 0f          ,
                0f          , 0f          , 15.65686000f, 16.07107000f, 0f          , 0f          , 0f          , 2.41421400f , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 8.82842800f , 9.82842800f , 10.82843000f, 0f          , 18.82843000f, 17.82843000f, 16.82843000f, 16.41422000f, 0f          , 0f          , 0f          ,
                14.65686000f, 14.24264000f, 14.65686000f, 15.07107000f, 16.07107000f, 17.07107000f, 0f          , 3.41421400f , 3.82842800f , 4.82842800f , 5.82842800f , 6.82842800f , 7.82842800f , 0f          , 9.82842800f , 10.24264000f, 11.24264000f, 0f          , 19.24264000f, 18.24264000f, 17.82843000f, 17.41422000f, 0f          , 0f          , 0f          ,
                13.65686000f, 13.24264000f, 13.65686000f, 14.65686000f, 15.65686000f, 0f          , 4.82842800f , 4.41421400f , 4.82842800f , 5.24264200f , 6.24264200f , 7.24264200f , 8.24264200f , 0f          , 10.82843000f, 11.24264000f, 11.65686000f, 0f          , 19.65685000f, 19.24264000f, 18.82843000f, 18.41422000f, 0f          , 0f          , 0f          ,
                13.24264000f, 12.24264000f, 0f          , 0f          , 0f          , 0f          , 0f          , 5.41421400f , 5.82842800f , 6.24264200f , 6.65685700f , 7.65685700f , 8.65685700f , 0f          , 11.82843000f, 12.24264000f, 12.65686000f, 0f          , 0f          , 0f          , 19.82843000f, 19.41422000f, 0f          , 0f          , 0f          ,
                12.82843000f, 11.82843000f, 10.82843000f, 9.82842800f , 8.82842800f , 7.82842800f , 6.82842800f , 6.41421400f , 6.82842800f , 7.24264200f , 7.65685700f , 8.07107100f , 9.07107100f , 0f          , 12.82843000f, 13.24264000f, 13.65686000f, 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          ,
                13.24264000f, 12.24264000f, 11.24264000f, 10.24264000f, 9.24264200f , 8.24264200f , 7.82842800f , 7.41421400f , 7.82842800f , 8.24264200f , 8.65685700f , 9.07107100f , 9.48528500f , 0f          , 13.82843000f, 14.24264000f, 14.65686000f, 15.07107000f, 16.07107000f, 17.07107000f, 18.07107000f, 19.07107000f, 0f          , 0f          , 0f          ,
                13.65686000f, 12.65686000f, 11.65686000f, 10.65686000f, 9.65685700f , 9.24264200f , 8.82842800f , 8.41421400f , 8.82842800f , 9.24264200f , 9.65685700f , 10.07107000f, 10.48528000f, 0f          , 14.82843000f, 15.24264000f, 15.65686000f, 16.07107000f, 16.48528000f, 17.48528000f, 18.48528000f, 19.48528000f, 0f          , 0f          , 0f          ,
                14.07107000f, 13.07107000f, 12.07107000f, 11.07107000f, 10.65686000f, 10.24264000f, 9.82842800f , 9.41421400f , 9.82842800f , 10.24264000f, 10.65686000f, 11.07107000f, 11.48528000f, 0f          , 15.82843000f, 16.24264000f, 16.65686000f, 17.07107000f, 17.48528000f, 17.89950000f, 18.89950000f, 19.89950000f, 0f          , 0f          , 0f          ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Abs(expectedDistanceMap[i] - actualDistance) < 0.01f, $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void GenerateDijkstraField_TwoFreeDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, GridUtils.GetTile(_grid, targetX, targetY), maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection[] expectedDirMap = new NextTileDirection[20 * 25]
            {
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['↘'], _dirs['↘'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['→'], _dirs['.'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['→'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['0'], _dirs['0'], _dirs['↓'], _dirs['↙'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↘'], _dirs['↓'], _dirs['↙'], _dirs['↙'], _dirs['↙'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↘'], _dirs['↓'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['→'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↘'], _dirs['↓'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['↑'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['→'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['←'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
                _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↗'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['↑'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['↖'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'], _dirs['0'],
            };
            float[] expectedDistanceMap = new float[20 * 25]
            {
                0f          , 0f          , 15.89950000f, 14.89950000f, 14.48528000f, 14.07107000f, 13.65686000f, 13.24264000f, 12.82843000f, 12.41421000f, 12.82843000f, 13.82843000f, 14.24264000f, 14.65686000f, 15.07107000f, 15.48528000f, 15.89950000f, 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          ,
                0f          , 0f          , 15.48528000f, 14.48528000f, 13.48528000f, 13.07107000f, 12.65686000f, 12.24264000f, 11.82843000f, 11.41421000f, 12.41421000f, 13.41421000f, 13.24264000f, 13.65686000f, 14.07107000f, 14.48528000f, 14.89950000f, 0f          , 19.24264000f, 19.65685000f, 0f          , 0f          , 0f          , 0f          , 0f          ,
                0f          , 0f          , 15.07107000f, 14.07107000f, 13.07107000f, 12.07107000f, 11.65686000f, 11.24264000f, 10.82843000f, 10.41421000f, 0f          , 0f          , 12.24264000f, 12.65686000f, 13.07107000f, 13.48528000f, 14.48528000f, 0f          , 18.24264000f, 18.65685000f, 19.07107000f, 19.48528000f, 0f          , 0f          , 0f          ,
                0f          , 0f          , 14.65686000f, 13.65686000f, 12.65686000f, 11.65686000f, 10.65686000f, 10.24264000f, 9.82842800f , 9.41421400f , 0f          , 0f          , 11.24264000f, 11.65686000f, 12.07107000f, 13.07107000f, 14.07107000f, 0f          , 17.24264000f, 17.65685000f, 18.07107000f, 19.07107000f, 0f          , 0f          , 0f          ,
                0f          , 0f          , 15.07107000f, 14.07107000f, 13.65686000f, 0f          , 9.65685700f , 9.24264200f , 8.82842800f , 8.41421400f , 0f          , 0f          , 10.24264000f, 10.65686000f, 11.65686000f, 12.65686000f, 13.65686000f, 0f          , 16.24264000f, 16.65686000f, 17.65685000f, 18.65685000f, 19.65685000f, 0f          , 0f          ,
                0f          , 0f          , 15.48528000f, 15.07107000f, 14.65686000f, 0f          , 9.24264200f , 8.24264200f , 7.82842800f , 7.41421400f , 7.82842800f , 8.24264200f , 9.24264200f , 10.24264000f, 11.24264000f, 12.24264000f, 13.24264000f, 14.24264000f, 15.24264000f, 16.24264000f, 17.24264000f, 18.24264000f, 19.24264000f, 0f          , 0f          ,
                0f          , 0f          , 16.48528000f, 16.07107000f, 15.65686000f, 0f          , 8.82842800f , 7.82842800f , 6.82842800f , 6.41421400f , 6.82842800f , 7.82842800f , 8.82842800f , 9.82842800f , 10.82843000f, 11.82843000f, 12.82843000f, 13.82843000f, 14.82843000f, 15.82843000f, 16.82843000f, 17.82843000f, 18.82843000f, 19.82843000f, 0f          ,
                0f          , 0f          , 17.48528000f, 17.07107000f, 16.65686000f, 0f          , 8.41421400f , 7.41421400f , 6.41421400f , 5.41421400f , 6.41421400f , 7.41421400f , 8.41421400f , 9.41421400f , 10.41421000f, 11.41421000f, 12.41421000f, 13.41421000f, 14.41421000f, 15.41421000f, 16.41422000f, 17.41422000f, 18.41422000f, 19.41422000f, 0f          ,
                0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 4.41421400f , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          ,
                0f          , 0f          , 19.82843000f, 0f          , 0f          , 1.41421400f , 1.00000000f , 1.41421400f , 2.41421400f , 3.41421400f , 4.41421400f , 5.41421400f , 6.41421400f , 7.41421400f , 8.41421400f , 9.41421400f , 10.41421000f, 11.41421000f, 12.41421000f, 13.41421000f, 14.41421000f, 15.41421000f, 0f          , 0f          , 0f          ,
                0f          , 19.24264000f, 18.82843000f, 19.24264000f, 0f          , 1.00000000f , 0.00000000f , 1.00000000f , 2.00000000f , 3.00000000f , 4.00000000f , 5.00000000f , 6.00000000f , 7.00000000f , 8.00000000f , 9.00000000f , 10.00000000f, 11.00000000f, 12.00000000f, 13.00000000f, 14.00000000f, 15.00000000f, 0f          , 0f          , 0f          ,
                19.82843000f, 18.82843000f, 17.82843000f, 18.24264000f, 0f          , 1.41421400f , 1.00000000f , 1.41421400f , 2.41421400f , 3.41421400f , 4.41421400f , 5.41421400f , 6.41421400f , 7.41421400f , 8.41421400f , 9.41421400f , 10.41421000f, 0f          , 0f          , 0f          , 0f          , 16.00000000f, 0f          , 0f          , 0f          ,
                0f          , 0f          , 16.82843000f, 17.24264000f, 0f          , 0f          , 0f          , 2.41421400f , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 9.41421400f , 9.82842800f , 10.82843000f, 0f          , 20.00000000f, 19.00000000f, 18.00000000f, 17.00000000f, 0f          , 0f          , 0f          ,
                15.82843000f, 15.41421000f, 15.82843000f, 16.82843000f, 17.82843000f, 18.82843000f, 0f          , 3.41421400f , 4.41421400f , 5.41421400f , 6.41421400f , 7.41421400f , 8.41421400f , 0f          , 10.41421000f, 10.82843000f, 11.24264000f, 0f          , 0f          , 19.41421000f, 18.41421000f, 18.00000000f, 0f          , 0f          , 0f          ,
                14.82843000f, 14.41421000f, 15.41421000f, 16.41422000f, 17.41422000f, 0f          , 5.41421400f , 4.41421400f , 4.82842800f , 5.82842800f , 6.82842800f , 7.82842800f , 8.82842800f , 0f          , 11.41421000f, 11.82843000f, 12.24264000f, 0f          , 0f          , 19.82843000f, 19.41421000f, 19.00000000f, 0f          , 0f          , 0f          ,
                13.82843000f, 13.41421000f, 0f          , 0f          , 0f          , 0f          , 0f          , 5.41421400f , 5.82842800f , 6.24264200f , 7.24264200f , 8.24264200f , 9.24264200f , 0f          , 12.41421000f, 12.82843000f, 13.24264000f, 0f          , 0f          , 0f          , 0f          , 20.00000000f, 0f          , 0f          , 0f          ,
                13.41421000f, 12.41421000f, 11.41421000f, 10.41421000f, 9.41421400f , 8.41421400f , 7.41421400f , 6.41421400f , 6.82842800f , 7.24264200f , 7.65685700f , 8.65685700f , 9.65685700f , 0f          , 13.41421000f, 13.82843000f, 14.24264000f, 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          , 0f          ,
                13.82843000f, 12.82843000f, 11.82843000f, 10.82843000f, 9.82842800f , 8.82842800f , 7.82842800f , 7.41421400f , 7.82842800f , 8.24264200f , 8.65685700f , 9.07107100f , 10.07107000f, 0f          , 14.41421000f, 14.82843000f, 15.24264000f, 16.24264000f, 17.24264000f, 18.24264000f, 19.24264000f, 0f          , 0f          , 0f          , 0f          ,
                14.24264000f, 13.24264000f, 12.24264000f, 11.24264000f, 10.24264000f, 9.24264200f , 8.82842800f , 8.41421400f , 8.82842800f , 9.24264200f , 9.65685700f , 10.07107000f, 10.48528000f, 0f          , 15.41421000f, 15.82843000f, 16.24264000f, 16.65686000f, 17.65685000f, 18.65685000f, 19.65685000f, 0f          , 0f          , 0f          , 0f          ,
                14.65686000f, 13.65686000f, 12.65686000f, 11.65686000f, 10.65686000f, 10.24264000f, 9.82842800f , 9.41421400f , 9.82842800f , 10.24264000f, 10.65686000f, 11.07107000f, 11.48528000f, 0f          , 16.41422000f, 16.82843000f, 17.24264000f, 17.65685000f, 18.07107000f, 19.07107000f, 0f          , 0f          , 0f          , 0f          , 0f          ,
            };
            for (int i = 0; i < dirMap._directionGrid.Length; i++)
            {
                NextTileDirection actualDir = dirMap._directionGrid[i];
                float actualDistance = dirMap._distanceMap[i];
                Assert.AreEqual(expectedDirMap[i], actualDir, $"Direction for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDirMap[i]} but got {actualDir}");
                Assert.IsTrue(Mathf.Abs(expectedDistanceMap[i] - actualDistance) < 0.01f, $"Distance for tile {GridUtils.GetCoordinatesFromFlatIndex(new(_grid.GetLength(0), _grid.GetLength(1)), i)} should be {expectedDistanceMap[i]} but got {actualDistance}");
            }
        }
        [Test]
        public void DijkstraField_GetTargetTile()
        {
            int targetX = 6;
            int targetY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance);
            TestTile returnedTarget = dirMap.GetTargetTile(_grid);
            Assert.IsTrue(GridUtils.TileEquals(target, returnedTarget), $"Target is [{target.X}, {target.Y}] but returned tile is {returnedTarget.X}, {returnedTarget.Y}");
        }
        [Test]
        public void DijkstraField_IsTileAccessible()
        {
            int targetX = 6;
            int targetY = 10;
            int accessibleTileX = 2;
            int accessibleTileY = 1;
            int inaccessibleTileX = 1;
            int inaccessibleTileY = 3;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance);
            Assert.IsTrue(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, accessibleTileX, accessibleTileY)));
            Assert.IsFalse(dirMap.IsTileAccessible(_grid, GridUtils.GetTile(_grid, inaccessibleTileX, inaccessibleTileY)));
        }
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DijkstraField_GetPathToTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathToTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                startTile, new(2,11), new(2,12), new(2,13), new(2,14), new(1,14), new(1,15), new(1,16), new(2,16), new(3,16), new(4,16), new(5,16), new(6,16), new(7,16), new(7,15), new(7,14), new(7,13), new(7,12), new(7,11), new(6,11), targetTile
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
        public void DijkstraField_GetPathToTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DijkstraField_GetPathToTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DijkstraField_GetPathToTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DijkstraField_GetPathFromTarget_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
            TestTile targetTile = includeTarget ? target : null;
            TestTile startTile = includeStart ? start : null;
            TestTile[] returnedPath = dirMap.GetPathFromTarget(_grid, start, includeStart, includeTarget);
            List<TestTile> expectedPathList = new(){
                targetTile, new(6,11), new(7,11), new(7,12), new(7,13), new(7,14), new(7,15), new(7,16), new(6,16), new(5,16), new(4,16), new(3,16), new(2,16), new(1,16), new(1,15), new(1,14), new(2,14), new(2,13), new(2,12), new(2,11), startTile
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
        public void DijkstraField_GetPathFromTarget_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
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
        public void DijkstraField_GetPathFromTarget_Diagonals1Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
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
        public void DijkstraField_GetPathFromTarget_Diagonals2Free(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
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
        public void DijkstraField_GetNextTileFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(4, 13);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraField_GetNextTileFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(6, 14);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraField_GetNextTileFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(2, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraField_GetNextTileFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
            TestTile returnedTile = dirMap.GetNextTileFromTile(_grid, start);
            TestTile expectedTile = new(1, 16);
            Assert.IsTrue(GridUtils.TileEquals(expectedTile, returnedTile), $"Next tile should be tile [{expectedTile.X}, {expectedTile.Y}] but got [{returnedTile.X}, {returnedTile.Y}]");
        }
        [Test]
        public void DijkstraField_GetNextTileDirectionFromTile_NoDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.NONE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.LEFT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraField_GetNextTileDirectionFromTile_AllDiagonals()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 5;
            int startY = 13;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.ALL_DIAGONALS);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraField_GetNextTileDirectionFromTile_Diagonals1Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_1FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP_RIGHT;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraField_GetNextTileDirectionFromTile_Diagonals2Free()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 1;
            int startY = 15;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance, DiagonalsPolicy.DIAGONAL_2FREE);
            NextTileDirection returnedDir = dirMap.GetNextTileDirectionFromTile(_grid, start);
            NextTileDirection expectedDir = NextTileDirection.UP;
            Assert.AreEqual(expectedDir, returnedDir);
        }
        [Test]
        public void DijkstraField_GetDistanceToTarget()
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = GridUtils.GetTile(_grid, targetX, targetY);
            TestTile start = GridUtils.GetTile(_grid, startX, startY);
            float maxDistance = 20f;
            DijkstraField dirMap = Pathfinding.GenerateDijkstraField(_grid, target, maxDistance);
            Assert.IsTrue(Mathf.Approximately(20f, dirMap.GetDistanceToTarget(_grid, start)));
        }

        #endregion

        #region UniquePath

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void GenerateUniquePath_NoDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = includeTarget ? new(6, 10) : null;
            TestTile start = includeStart ? new(2, 10) : null;
            TestTile[] path = Pathfinding.GenerateUniquePath(_grid, GridUtils.GetTile(_grid, targetX, targetY), GridUtils.GetTile(_grid, startX, startY), DiagonalsPolicy.NONE, 1.414f, includeStart, includeTarget);
            for (int i = 0; i < path.Length; i++)
            {
                Debug.Log(path[i]);
            }
            List<TestTile> expectedPath = new() { start, new(2, 11), new(2, 12), new(2, 13), new(2, 14), new(1, 14), new(1, 15), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(7, 15), new(7, 14), new(7, 13), new(7, 12), new(7, 11), new(6, 11), target };
            expectedPath.RemoveAll(x => x == null);
            Assert.AreEqual(expectedPath.Count, path.Length, $"Path length should be {expectedPath.Count} but got {path.Length}");
            for (int i = 0; i < path.Length; i++)
            {
                Assert.IsTrue(expectedPath[i].X == path[i].X && expectedPath[i].Y == path[i].Y, $"Tile {i} on the path should be {expectedPath[i]} but got {path[i]}");
            }
        }
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void GenerateUniquePath_AllDiagonals(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = includeTarget ? new(6, 10) : null;
            TestTile start = includeStart ? new(2, 10) : null;
            TestTile[] path = Pathfinding.GenerateUniquePath(_grid, GridUtils.GetTile(_grid, targetX, targetY), GridUtils.GetTile(_grid, startX, startY), DiagonalsPolicy.ALL_DIAGONALS, 1.414f, includeStart, includeTarget);
            for (int i = 0; i < path.Length; i++)
            {
                Debug.Log(path[i]);
            }
            List<TestTile> expectedPath = new() { start, new(2, 11), new(3, 12), new(4, 13), new(5, 13), new(6, 14), new(7, 13), new(7, 12), new(6, 11), target };
            expectedPath.RemoveAll(x => x == null);
            Assert.AreEqual(expectedPath.Count, path.Length, $"Path length should be {expectedPath.Count} but got {path.Length}");
            for (int i = 0; i < path.Length; i++)
            {
                Assert.IsTrue(expectedPath[i].X == path[i].X && expectedPath[i].Y == path[i].Y, $"Tile {i} on the path should be {expectedPath[i]} but got {path[i]}");
            }
        }
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void GenerateUniquePath_DiagonalsOneFree(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = includeTarget ? new(6, 10) : null;
            TestTile start = includeStart ? new(2, 10) : null;
            TestTile[] path = Pathfinding.GenerateUniquePath(_grid, GridUtils.GetTile(_grid, targetX, targetY), GridUtils.GetTile(_grid, startX, startY), DiagonalsPolicy.DIAGONAL_1FREE, 1.414f, includeStart, includeTarget);
            for (int i = 0; i < path.Length; i++)
            {
                Debug.Log(path[i]);
            }
            List<TestTile> expectedPath = new() { start, new(2, 11), new(2, 12), new(2, 13), new(2, 14), new(1, 15), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 15), new(7, 14), new(7, 13), new(7, 12), new(6, 11), target };
            expectedPath.RemoveAll(x => x == null);
            Assert.AreEqual(expectedPath.Count, path.Length, $"Path length should be {expectedPath.Count} but got {path.Length}");
            for (int i = 0; i < path.Length; i++)
            {
                Assert.IsTrue(expectedPath[i].X == path[i].X && expectedPath[i].Y == path[i].Y, $"Tile {i} on the path should be {expectedPath[i]} but got {path[i]}");
            }
        }
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void GenerateUniquePath_DiagonalsTwoFree(bool includeStart, bool includeTarget)
        {
            int targetX = 6;
            int targetY = 10;
            int startX = 2;
            int startY = 10;
            TestTile target = includeTarget ? new(6, 10) : null;
            TestTile start = includeStart ? new(2, 10) : null;
            TestTile[] path = Pathfinding.GenerateUniquePath(_grid, GridUtils.GetTile(_grid, targetX, targetY), GridUtils.GetTile(_grid, startX, startY), DiagonalsPolicy.DIAGONAL_2FREE, 1.414f, includeStart, includeTarget);
            for (int i = 0; i < path.Length; i++)
            {
                Debug.Log(path[i]);
            }
            List<TestTile> expectedPath = new() { start, new(2, 11), new(2, 12), new(2, 13), new(1, 14), new(1, 15), new(1, 16), new(2, 16), new(3, 16), new(4, 16), new(5, 16), new(6, 16), new(7, 16), new(7, 15), new(7, 14), new(7, 13), new(7, 12), new(7, 11), target };
            expectedPath.RemoveAll(x => x == null);
            Assert.AreEqual(expectedPath.Count, path.Length, $"Path length should be {expectedPath.Count} but got {path.Length}");
            for (int i = 0; i < path.Length; i++)
            {
                Assert.IsTrue(expectedPath[i].X == path[i].X && expectedPath[i].Y == path[i].Y, $"Tile {i} on the path should be {expectedPath[i]} but got {path[i]}");
            }
        }
        #endregion
    }
}
