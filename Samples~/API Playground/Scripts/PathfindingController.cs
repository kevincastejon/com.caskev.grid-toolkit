using CasKev.GridToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GridToolkitWorkingProject.Demos.APIPlayground
{
    public class PathfindingController : MonoBehaviour
    {
        public enum DemoType
        {
            DIRECT_PATHFINDING,
            DIRECT_PATHFINDING_ASYNC,
            PATHMAP,
            PATHMAP_ASYNC,
            PATHGRID_ASYNC,
        }
        [SerializeField] private Transform _arrowPrefab;
        [SerializeField] private TextMeshProUGUI _progressWindow;
        [SerializeField] private TextMeshProUGUI _hoveredTileLabel;
        [SerializeField] private GridController _grid;
        private DirectionMap<Tile> _directionMap;
        private List<Transform> _arrows = new();
        private Tile _targetTile;
        private Tile _startTile;
        private bool _walling;
        private bool _startWalkableValue;
        private System.Threading.CancellationTokenSource _cts = new();
        private void OnEnable()
        {
            if (_directionMap != null)
            {
                GenerateArrowsFromDirectionMap();
            }
        }
        private void OnDisable()
        {
            DestroyArrows();
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
        private void Start()
        {
            _targetTile = _grid.CenterTile;
            _startTile = _grid.StartTile;
            GenerateDirectionMap();
        }
        private void Update()
        {
            _hoveredTileLabel.text = _grid.ClampedHoveredTile == null ? "" : ("X:" + _grid.ClampedHoveredTile.X + " Y:" + _grid.ClampedHoveredTile.Y);
            if ((_grid.JustEnteredTile && Input.GetMouseButton(0)) || (Input.GetMouseButtonDown(0) && _grid.HoveredTile != null && _grid.HoveredTile != _targetTile))
            {
                if (!_walling)
                {
                    _walling = true;
                    _startWalkableValue = !_grid.HoveredTile.IsWalkable;
                }
                if (_grid.HoveredTile.IsWalkable != _startWalkableValue)
                {
                    _grid.SetWalkable(_grid.HoveredTile, _startWalkableValue);
                    _grid.Refresh(null, new Tile[0], new Tile[0], null);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_walling)
                {
                    _walling = false;
                    GenerateDirectionMap();
                }
            }
            else if (Input.GetMouseButtonDown(1) && (_grid.HoveredTile != null && _grid.HoveredTile.IsWalkable && _grid.HoveredTile != _targetTile && _grid.HoveredTile != _startTile))
            {
                _targetTile = _grid.HoveredTile;
                GenerateDirectionMap();
            }
            else if (Input.GetMouseButtonDown(2) && (_grid.HoveredTile != null && _grid.HoveredTile.IsWalkable && _grid.HoveredTile != _targetTile && _grid.HoveredTile != _startTile && _grid.ClampedHoveredTile != _targetTile))
            {
                _startTile = _grid.HoveredTile;
                GetPathFromDirectionMap();
            }
        }
        private void GenerateDirectionMap()
        {
            _directionMap = Pathfinding.GenerateDirectionMap(_grid.Map, _targetTile);
            DestroyArrows();
            GenerateArrowsFromDirectionMap();
            bool isAccessible = _directionMap.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _directionMap.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            _grid.Refresh(_targetTile, null, path, _startTile);
        }

        private void DestroyArrows()
        {
            foreach (Transform arrow in _arrows)
            {
                if (arrow != null)
                {
                    Destroy(arrow.gameObject);
                }
            }
            _arrows.Clear();
        }

        private void GenerateArrowsFromDirectionMap()
        {
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = Instantiate(_arrowPrefab);
                    _arrows.Add(arrow);
                    arrow.position = new(j, i);
                    float angle = 0f;
                    bool hasDirection = _directionMap.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (hasDirection)
                    {
                        NextTileDirection nextDirection = _directionMap.GetNextTileDirectionFromTile(_grid.Map, _grid.Map[i, j]);
                        switch (nextDirection)
                        {
                            case NextTileDirection.RIGHT: angle = 0f; break;
                            case NextTileDirection.UP: angle = 90f; break;
                            case NextTileDirection.LEFT: angle = 180f; break;
                            case NextTileDirection.DOWN: angle = -90f; break;
                            case NextTileDirection.UP_RIGHT: angle = 45f; break;
                            case NextTileDirection.UP_LEFT: angle = 135f; break;
                            case NextTileDirection.DOWN_LEFT: angle = -135f; break;
                            case NextTileDirection.DOWN_RIGHT: angle = -45f; break;
                            case NextTileDirection.NONE: break;
                            case NextTileDirection.SELF: default: hasDirection = false; break;
                        }
                    }
                    Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                    if (hasDirection)
                    {
                        arrow.rotation = rot;
                    }
                    else
                    {
                        arrow.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void GetPathFromDirectionMap()
        {
            _grid.Refresh(null, null, _directionMap.IsTileAccessible(_grid.Map, _startTile) ? _directionMap.GetPathToTarget(_grid.Map, _startTile, false, false) : new Tile[0], _startTile);
        }
        private void OnApplicationQuit()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
    }
}