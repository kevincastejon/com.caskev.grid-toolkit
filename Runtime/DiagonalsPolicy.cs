/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Represents the pathfinding diagonals permissiveness.<br/>
    /// When going diagonally from a tile <b>A</b> to tile <b>B</b> in 2D grid, there are two more tile involved, the ones that are both facing neighbours of the <b>A</b> and <b>B</b> tiles. You can allow diagonals movement depending on the walkable status of these tiles.<br/>
    /// <b>NONE :</b> no diagonal movement allowed<br/>
    /// <b>DIAGONAL_2FREE :</b> only diagonal movements, with two walkable facing neighbours common to the start and destination tiles, are allowed<br/>
    /// <b>DIAGONAL_1FREE :</b> only diagonal movements, with one or more walkable facing neighbour common to the start and destination tiles, are allowed<br/>
    /// <b>ALL_DIAGONALS :</b> all diagonal movements allowed<br/>
    /// \image html "./DiagonalsPolicySchematic.png" height=500
    /// </summary>
    public enum DiagonalsPolicy
    {
        NONE,
        DIAGONAL_2FREE,
        DIAGONAL_1FREE,
        ALL_DIAGONALS,
    }
}
