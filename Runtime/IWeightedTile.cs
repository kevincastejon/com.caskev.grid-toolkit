/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// A tile with coordinates, walkable status and weight.
    /// </summary>
    public interface IWeightedTile : ITile
    {
        /// <summary>
        /// The movement cost to enter this tile.
        /// </summary>
        public float Weight
        {
            get;
        }
    }
}
