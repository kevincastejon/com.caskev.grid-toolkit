/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// An enumeration representing the possible directions to move to reach a neighboring tile
    /// </summary>
    public enum NextTileDirection : byte
    {
        NONE,
        SELF,
        LEFT,
        RIGHT,
        DOWN,
        UP,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT,
    }
}
