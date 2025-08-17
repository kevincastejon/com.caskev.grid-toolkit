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
        private DirectionMap<Tile> _pathMap;
        private List<Transform> _arrows = new();
        private Tile _targetTile;
        private Tile _startTile;
        private bool _walling;
        private bool _startWalkableValue;
        private System.Threading.CancellationTokenSource _cts = new();
        private void OnEnable()
        {
            if (_pathMap != null)
            {
                for (int i = 0; i < _pathMap._directionMap.Length; i++)
                {
                    NextNodeDirection nextDirection = _pathMap._directionMap[i];
                    Transform arrow = Instantiate(_arrowPrefab);
                    _arrows.Add(arrow);
                    Vector2Int pos = GridUtils.GetCoordinatesFromFlatIndex(new(_grid.Map.GetLength(0), _grid.Map.GetLength(1)), i, MajorOrder.ROW_MAJOR_ORDER);
                    arrow.position = new(pos.x, pos.y);
                    float angle = 0f;
                    bool hasNoDirection = false;
                    switch (nextDirection)
                    {
                        case NextNodeDirection.RIGHT: angle = 0f; break;
                        case NextNodeDirection.UP: angle = 90f; break;
                        case NextNodeDirection.LEFT: angle = 180f; break;
                        case NextNodeDirection.DOWN: angle = -90f; break;
                        case NextNodeDirection.UP_RIGHT: angle = 45f; break;
                        case NextNodeDirection.UP_LEFT: angle = 135f; break;
                        case NextNodeDirection.DOWN_LEFT: angle = -135f; break;
                        case NextNodeDirection.DOWN_RIGHT: angle = -45f; break;
                        case NextNodeDirection.NONE: break;
                        case NextNodeDirection.SELF: default: hasNoDirection = true; break;
                    }
                    Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                    if (hasNoDirection)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    else
                    {
                        arrow.rotation = rot;
                    }
                }
            }
        }
        private void OnDisable()
        {
            foreach (Transform arrow in _arrows)
            {
                if (arrow)
                {
                    Destroy(arrow.gameObject);
                }
            }
            _arrows.Clear();
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
            _pathMap = Pathfinding.GenerateDirectionMap(_grid.Map, _targetTile);
            foreach (Transform arrow in _arrows)
            {
                Destroy(arrow.gameObject);
            }
            _arrows.Clear();
            for (int i = 0; i < _pathMap._directionMap.Length; i++)
            {
                NextNodeDirection nextDirection = _pathMap._directionMap[i];
                Transform arrow = Instantiate(_arrowPrefab);
                _arrows.Add(arrow);
                Vector2Int pos = GridUtils.GetCoordinatesFromFlatIndex(new(_grid.Map.GetLength(0), _grid.Map.GetLength(1)), i, MajorOrder.ROW_MAJOR_ORDER);
                arrow.position = new(pos.x, pos.y);
                float angle = 0f;
                bool hasNoDirection = false;
                switch (nextDirection)
                {
                    case NextNodeDirection.RIGHT: angle = 0f; break;
                    case NextNodeDirection.UP: angle = 90f; break;
                    case NextNodeDirection.LEFT: angle = 180f; break;
                    case NextNodeDirection.DOWN: angle = -90f; break;
                    case NextNodeDirection.UP_RIGHT: angle = 45f; break;
                    case NextNodeDirection.UP_LEFT: angle = 135f; break;
                    case NextNodeDirection.DOWN_LEFT: angle = -135f; break;
                    case NextNodeDirection.DOWN_RIGHT: angle = -45f; break;
                    case NextNodeDirection.NONE: break;
                    case NextNodeDirection.SELF: default: hasNoDirection = true; break;
                }
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                if (hasNoDirection)
                {
                    arrow.gameObject.SetActive(false);
                }
                else
                {
                    arrow.rotation = rot;
                }
            }
            bool isAccessible = _pathMap.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _pathMap.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            _grid.Refresh(_targetTile, null, path, _startTile);
        }
        private void GetPathFromDirectionMap()
        {
            _grid.Refresh(null, null, _pathMap.IsTileAccessible(_grid.Map, _startTile) ? _pathMap.GetPathToTarget(_grid.Map, _startTile, false, false) : new Tile[0], _startTile);
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