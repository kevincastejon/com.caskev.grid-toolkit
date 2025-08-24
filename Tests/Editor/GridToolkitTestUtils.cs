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
        [TestCase(6, 4, -1, -1, 0, 0, MajorOrder.ROW_MAJOR_ORDER, TestName = "DownLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, -1, 4, 0, 3, MajorOrder.ROW_MAJOR_ORDER, TestName = "UpLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, -1, 5, 0, MajorOrder.ROW_MAJOR_ORDER, TestName = "DownRight Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, 4, 5, 3, MajorOrder.ROW_MAJOR_ORDER, TestName = "UpRight Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, -1, -1, 0, 0, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "DownLeft Out Of Bounds ColumnMajorOrder")]
        [TestCase(6, 4, -1, 4, 0, 3, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "UpLeft Out Of Bounds ColumnMajorOrder")]
        [TestCase(6, 4, 6, -1, 5, 0, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "DownRight Out Of Bounds ColumnMajorOrder")]
        [TestCase(6, 4, 6, 4, 5, 3, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "UpRight Out Of Bounds ColumnMajorOrder")]
        public void ClampCoordsIntoGrid(int gridWidth, int gridHeight, int coordX, int coordY, int clampX, int clampY, MajorOrder majorOrder)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight, majorOrder);
            Assert.AreEqual(new Vector2Int(clampX, clampY), GridUtils.ClampCoordsIntoGrid(grid, coordX, coordY, majorOrder));
        }
        [TestCase(6, 4, 1, 1, true, MajorOrder.ROW_MAJOR_ORDER, TestName = "Into grid RowMajorOrder")]
        [TestCase(6, 4, -1, -1, false, MajorOrder.ROW_MAJOR_ORDER, TestName = "DownLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, -1, 4, false, MajorOrder.ROW_MAJOR_ORDER, TestName = "UpLeft Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, -1, false, MajorOrder.ROW_MAJOR_ORDER, TestName = "DownRight Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, 6, 4, false, MajorOrder.ROW_MAJOR_ORDER, TestName = "UpRight Out Of Bounds RowMajorOrder")]
        [TestCase(6, 4, -1, -1, false, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "DownLeft Out Of Bounds ColumnMajorOrder")]
        [TestCase(6, 4, -1, 4, false, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "UpLeft Out Of Bounds ColumnMajorOrder")]
        [TestCase(6, 4, 6, -1, false, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "DownRight Out Of Bounds ColumnMajorOrder")]
        [TestCase(6, 4, 6, 4, false, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "UpRight Out Of Bounds ColumnMajorOrder")]
        public void AreCoordsIntoGrid(int gridWidth, int gridHeight, int coordX, int coordY, bool expectedResult, MajorOrder majorOrder)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight, majorOrder);
            Assert.AreEqual(expectedResult, GridUtils.AreCoordsIntoGrid(grid, coordX, coordY, majorOrder));
        }
        [TestCase(6, 4, 2, 3, MajorOrder.ROW_MAJOR_ORDER, TestName = "RowMajorOrder")]
        [TestCase(6, 4, 2, 3, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "ColumMajorOrder")]
        public void GetTile(int gridWidth, int gridHeight, int coordX, int coordY, MajorOrder majorOrder)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight, majorOrder);
            TestTile tile = GridUtils.GetTile(grid, coordX, coordY, majorOrder);
            Vector2Int expectedCoords = new Vector2Int(coordX, coordY);
            Vector2Int actualCoords = new Vector2Int(tile.X, tile.Y);
            Assert.AreEqual(expectedCoords, actualCoords);
        }
        [TestCase(6, 4, 6, MajorOrder.ROW_MAJOR_ORDER, TestName = "RowMajorOrder")]
        [TestCase(6, 4, 6, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "ColumMajorOrder")]
        public void GetHorizontalLength(int gridWidth, int gridHeight, int expectedLength, MajorOrder majorOrder)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight, majorOrder);
            Assert.AreEqual(expectedLength, GridUtils.GetHorizontalLength(grid, majorOrder));
        }
        [TestCase(6, 4, 4, MajorOrder.ROW_MAJOR_ORDER, TestName = "RowMajorOrder")]
        [TestCase(6, 4, 4, MajorOrder.COLUMN_MAJOR_ORDER, TestName = "ColumMajorOrder")]
        public void GetVerticalLength(int gridWidth, int gridHeight, int expectedLength, MajorOrder majorOrder)
        {
            TestTile[,] grid = GridFactory.Build(gridWidth, gridHeight, majorOrder);
            Assert.AreEqual(expectedLength, GridUtils.GetVerticalLength(grid, majorOrder));
        }

        //[Test]
        //public void TileEquals_Works_On_SameCoords()
        //{
        //    var a = new TestTile(2, 3);
        //    var b = new TestTile(2, 3);
        //    var c = new TestTile(2, 4);
        //    Assert.IsTrue(GridUtils.TileEquals(a, b));
        //    Assert.IsFalse(GridUtils.TileEquals(a, c));
        //}

        //[Test]
        //public void Clamp_And_InBounds_BothOrders()
        //{
        //    var rm = GridFactory.Build(4, 3, MajorOrder.ROW_MAJOR_ORDER);
        //    var cm = GridFactory.Build(4, 3, MajorOrder.COLUMN_MAJOR_ORDER);

        //    var c1 = GridUtils.ClampCoordsIntoGrid(rm, -5, 99, MajorOrder.ROW_MAJOR_ORDER);
        //    Assert.AreEqual(new Vector2Int(0, 2), c1);
        //    Assert.IsTrue(GridUtils.AreCoordsIntoGrid(rm, 3, 2, MajorOrder.ROW_MAJOR_ORDER));
        //    Assert.IsFalse(GridUtils.AreCoordsIntoGrid(rm, 4, 0, MajorOrder.ROW_MAJOR_ORDER));

        //    var c2 = GridUtils.ClampCoordsIntoGrid(cm, 99, -5, MajorOrder.COLUMN_MAJOR_ORDER);
        //    Assert.AreEqual(new Vector2Int(3, 0), c2);
        //    Assert.IsTrue(GridUtils.AreCoordsIntoGrid(cm, 3, 2, MajorOrder.COLUMN_MAJOR_ORDER));
        //    Assert.IsFalse(GridUtils.AreCoordsIntoGrid(cm, 0, 3, MajorOrder.COLUMN_MAJOR_ORDER));
        //}

        //[Test]
        //public void GetTile_And_Dimensions_BothOrders()
        //{
        //    var rm = GridFactory.Build(5, 4, MajorOrder.ROW_MAJOR_ORDER);
        //    var cm = GridFactory.Build(5, 4, MajorOrder.COLUMN_MAJOR_ORDER);

        //    Assert.AreEqual(5, GridUtils.GetHorizontalLength(rm, MajorOrder.ROW_MAJOR_ORDER));
        //    Assert.AreEqual(4, GridUtils.GetVerticalLength(rm, MajorOrder.ROW_MAJOR_ORDER));
        //    Assert.AreEqual(5, GridUtils.GetHorizontalLength(cm, MajorOrder.COLUMN_MAJOR_ORDER));
        //    Assert.AreEqual(4, GridUtils.GetVerticalLength(cm, MajorOrder.COLUMN_MAJOR_ORDER));

        //    var t1 = GridUtils.GetTile(rm, 2, 1, MajorOrder.ROW_MAJOR_ORDER);
        //    var t2 = GridUtils.GetTile(cm, 2, 1, MajorOrder.COLUMN_MAJOR_ORDER);
        //    Assert.AreEqual((2, 1), (t1.X, t1.Y));
        //    Assert.AreEqual((2, 1), (t2.X, t2.Y));
        //}
    }
}
