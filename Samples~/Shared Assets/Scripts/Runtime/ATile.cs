using UnityEngine;
using CasKev.GridToolkit;
namespace GridToolkitWorkingProject.Demos.Shared
{
public abstract class ATile : MonoBehaviour, ITile
{
    private bool _isWalkable;
    private int _x;
    private int _y;

    public bool IsWalkable { get => _isWalkable; set => _isWalkable = value; }
    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }
}
}