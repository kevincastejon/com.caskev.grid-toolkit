using Caskev.GridToolkit;

namespace GridToolkitWorkingProject.Samples.APIPlayground
{
    public class Tile : IWeightedTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsWalkable { get; set; }
        public float Weight => 1f;

        public Tile(int x, int y, bool isWalkable = true)
        {
            IsWalkable = isWalkable;
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"[X:{X} Y:{Y} Walkable:{IsWalkable} Weight:{Weight}]";
        }
    }
}