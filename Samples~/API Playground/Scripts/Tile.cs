using Caskev.GridToolkit;

namespace GridToolkitWorkingProject.Demos.APIPlayground
{
    public class Tile : ITile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsWalkable { get; set; }

        public Tile(int x, int y, bool isWalkable = true)
        {
            IsWalkable = isWalkable;
            X = x;
            Y = y;
        }
    }
}