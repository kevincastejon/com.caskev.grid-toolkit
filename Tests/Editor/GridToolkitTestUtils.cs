using NUnit.Framework;
using UnityEngine;
using Caskev.GridToolkit;

namespace GridToolkitTests
{
    public class GridUtils_Tests
    {
        /// <summary>
        /// Tests the equality comparison logic for tile objects depending on their coordinates.
        /// </summary>
        /// <remarks>This test verifies that the equality operation correctly identifies whether two tile
        /// objects are considered equal or not based on their X and Y properties.</remarks>
        [TestCase(2, 3, 2, 3, true)]
        [TestCase(2, 3, 5, 5, false)]
        public void TileEquals_SameCoords(int aX, int aY, int bX, int bY, bool expectedResult)
        {
            TestTile a = new(aX, aY);
            TestTile b = new(bX, bY);
            Assert.AreEqual(expectedResult, GridUtils.TileEquals(a, b), "Tiles with same coordinates should be equal.");
        }
        [TestCase(6, 4, -1, -1, 0, 0, TestName = "DownLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, -1, 4, 0, 3, TestName = "UpLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, -1, 5, 0, TestName = "DownRight Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, 4, 5, 3, TestName = "UpRight Out Of Bounds RowMajorOrder")]
        public void ClampCoordsIntoGrid(int gridWidth, int gridHeight, int coordX, int coordY, int clampX, int clampY)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight);
            Assert.AreEqual(new Vector2Int(clampX, clampY), GridUtils.ClampCoordsIntoGrid(grid, coordX, coordY));
        }
        [TestCase(6, 4, 1, 1, true, TestName = "Into grid RowMajorOrder")]
        [TestCase(6, 4, -1, -1, false, TestName = "DownLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, -1, 4, false, TestName = "UpLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, -1, false, TestName = "DownRight Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, 4, false, TestName = "UpRight Out Of Bounds RowMajorOrder")]
        public void AreCoordsIntoGrid(int gridWidth, int gridHeight, int coordX, int coordY, bool expectedResult)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight);
            Assert.AreEqual(expectedResult, GridUtils.AreCoordsIntoGrid(grid, coordX, coordY));
        }
        [TestCase(6, 4, 2, 3, TestName = "RowMajorOrder")]
        public void GetTile(int gridWidth, int gridHeight, int coordX, int coordY)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight);
            TestTile tile = GridUtils.GetTile(grid, coordX, coordY);
            Vector2Int expectedCoords = new Vector2Int(coordX, coordY);
            Vector2Int actualCoords = new Vector2Int(tile.X, tile.Y);
            Assert.AreEqual(expectedCoords, actualCoords);
        }
        [TestCase(6, 4, 6, TestName = "RowMajorOrder")]
        public void GetHorizontalLength(int gridWidth, int gridHeight, int expectedLength)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight);
            Assert.AreEqual(expectedLength, GridUtils.GetHorizontalLength(grid));
        }
        [TestCase(6, 4, 4, TestName = "RowMajorOrder")]
        public void GetVerticalLength(int gridWidth, int gridHeight, int expectedLength)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight);
            Assert.AreEqual(expectedLength, GridUtils.GetVerticalLength(grid));
        }
    }
}
