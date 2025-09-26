using Caskev.GridToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GridToolkitWorkingProject.Demos.APIPlayground
{
    public class PathfindingController : MonoBehaviour
    {
        public enum PathfindingType
        {
            DIRECTION_MAP,
            DIJKSTRA_MAP
        }
        [SerializeField] private GameObject _uiContent;
        [SerializeField] private Transform _arrowPrefab;
        [SerializeField] private Transform _distancePrefab;
        [SerializeField] private TextMeshProUGUI _progressWindow;
        [SerializeField] private TMP_Dropdown _pathfindingType;
        [SerializeField] private TMP_Dropdown _diagonalsPolicy;
        [SerializeField] private Slider _diagonalsWeight;
        [SerializeField] private TextMeshProUGUI _hoveredTileLabel;
        private CanvasGroup _canvasGroup;
        private GridController _grid;
        private DirectionMap _directionMap;
        private DijkstraMap _dijkstraMap;
        private List<Transform> _arrowsSprites = new();
        private List<Transform> _distanceSprites = new();
        private Tile _targetTile;
        private Tile _startTile;
        private bool _walling;
        private bool _startWalkableValue;
        private System.Threading.CancellationTokenSource _cts = new();
        private bool _isQuitting = false;
        private void OnEnable()
        {
            _uiContent.SetActive(true);
            if (_targetTile != null)
            {
                Start();
            }
        }
        private void OnDisable()
        {
            if (_isQuitting)
            {
                return;
            }
            _uiContent.SetActive(false);
            _grid.ClearWalkables();
            DestroyArrows();
            DestroyDistanceSprites();
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
        private void Awake()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            _grid = FindAnyObjectByType<GridController>();
            _targetTile = _grid.CenterTile;
            _startTile = _grid.StartTile;
            _diagonalsPolicy.onValueChanged.AddListener((x) => UpdateParameters());
            _pathfindingType.onValueChanged.AddListener((x) => UpdateParameters());
            _diagonalsWeight.onValueChanged.AddListener((x) => UpdateParameters());
        }
        private void Start()
        {
            _grid.StartTileEnabled = true;
            UpdateParameters();
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
                    _grid.ClearWalkables();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_walling)
                {
                    _walling = false;
                    GenerateMap();
                }
            }
            else if (Input.GetMouseButtonDown(1) && (_grid.HoveredTile != null && _grid.HoveredTile.IsWalkable && _grid.HoveredTile != _targetTile && _grid.HoveredTile != _startTile))
            {
                _targetTile = _grid.HoveredTile;
                GenerateMap();
            }
            else if (Input.GetMouseButtonDown(2) && (_grid.HoveredTile != null && _grid.HoveredTile.IsWalkable && _grid.HoveredTile != _targetTile && _grid.HoveredTile != _startTile && _grid.ClampedHoveredTile != _targetTile))
            {
                _startTile = _grid.HoveredTile;
                GetPathFromMap();
            }
        }
        private void OnApplicationQuit()
        {
            _isQuitting = true;
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
        public void CancelOperation()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
        private void UpdateParameters()
        {
            _diagonalsWeight.transform.parent.gameObject.SetActive(((PathfindingType)_pathfindingType.value) != PathfindingType.DIRECTION_MAP && ((DiagonalsPolicy)_diagonalsPolicy.value) != DiagonalsPolicy.NONE);
            GenerateMap();
        }
        private void GenerateMap()
        {
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIRECTION_MAP:
                    GenerateDirectionMap();
                    break;
                case PathfindingType.DIJKSTRA_MAP:
                    GenerateDijkstraMap();
                    break;
                default:
                    break;
            }
        }
        private async void GenerateDijkstraMap()
        {
            _canvasGroup.interactable = false;
            _progressWindow.transform.parent.gameObject.SetActive(true);
            _cts = new System.Threading.CancellationTokenSource();
            try
            {
                _dijkstraMap = await Pathfinding.GenerateDijkstraMapAsync(_grid.Map, _targetTile, (DiagonalsPolicy)_diagonalsPolicy.value, _diagonalsWeight.value, new Progress<float>((x) => _progressWindow.text = (x * 100).ToString("F0") + "%"), _cts.Token);
            }
            catch (Exception e)
            {
                _progressWindow.transform.parent.gameObject.SetActive(false);
                _canvasGroup.interactable = true;
                _cts = null;
                throw e;
            }
            _progressWindow.transform.parent.gameObject.SetActive(false);
            _canvasGroup.interactable = true;
            _cts = null;
            DestroyArrows();
            DestroyDistanceSprites();
            GenerateArrowsSprites();
            GenerateDistanceSprites();
            bool isAccessible = _dijkstraMap.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _dijkstraMap.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            _grid.TintCenter(_targetTile);
            _grid.TintPathTiles(path);
            _grid.TintStart(_startTile);
        }
        private async void GenerateDirectionMap()
        {
            _canvasGroup.interactable = false;
            _progressWindow.transform.parent.gameObject.SetActive(true);
            _cts = new System.Threading.CancellationTokenSource();
            try
            {
                _directionMap = await Pathfinding.GenerateDirectionMapAsync(_grid.Map, _targetTile, (DiagonalsPolicy)_diagonalsPolicy.value, new Progress<float>((x) => _progressWindow.text = (x * 100).ToString("F0") + "%"), _cts.Token);
            }
            catch (Exception e)
            {
                _progressWindow.transform.parent.gameObject.SetActive(false);
                _canvasGroup.interactable = true;
                _cts = null;
                throw e;
            }
            _progressWindow.transform.parent.gameObject.SetActive(false);
            _canvasGroup.interactable = true;
            _cts = null;
            DestroyArrows();
            DestroyDistanceSprites();
            GenerateArrowsSprites();
            GenerateDistanceSprites();
            bool isAccessible = _directionMap.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _directionMap.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            _grid.TintCenter(_targetTile);
            _grid.TintPathTiles(path);
            _grid.TintStart(_startTile);
        }
        private void DestroyArrows()
        {
            foreach (Transform arrow in _arrowsSprites)
            {
                if (arrow != null)
                {
                    Destroy(arrow.gameObject);
                }
            }
            _arrowsSprites.Clear();
        }
        private void DestroyDistanceSprites()
        {
            foreach (Transform distanceSprite in _distanceSprites)
            {
                if (distanceSprite != null)
                {
                    Destroy(distanceSprite.gameObject);
                }
            }
            _arrowsSprites.Clear();
        }
        private void GenerateArrowsSprites()
        {
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIRECTION_MAP:
                    GenerateArrowsFromDirectionMap();
                    break;
                case PathfindingType.DIJKSTRA_MAP:
                    GenerateArrowsFromDijkstraMap();
                    break;
                default:
                    break;
            }
        }
        private void GenerateDistanceSprites()
        {
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIJKSTRA_MAP:
                    GenerateDistanceFromDijkstraMap();
                    break;
                default:
                    break;
            }
        }
        private void GenerateArrowsFromDijkstraMap()
        {
            if (_dijkstraMap == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = Instantiate(_arrowPrefab);
                    _arrowsSprites.Add(arrow);
                    arrow.position = _grid.TileMap.CellToWorld(new Vector3Int(j, i)) + new Vector3(_grid.TileMap.transform.localScale.x / 2, _grid.TileMap.transform.localScale.y / 2) + (_grid.TileMap.transform.parent.localScale.y < 0 ? Vector3.down * _grid.TileMap.transform.localScale.y : Vector3.zero); ;
                    arrow.localScale = _grid.transform.localScale;
                    float angle = 0f;
                    bool hasDirection = _dijkstraMap.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (hasDirection)
                    {
                        NextTileDirection nextDirection = _dijkstraMap.GetNextTileDirectionFromTile(_grid.Map, _grid.Map[i, j]);
                        switch (nextDirection)
                        {
                            case NextTileDirection.RIGHT: angle = 0f; break;
                            case NextTileDirection.UP: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -90f : 90f; break;
                            case NextTileDirection.LEFT: angle = 180f; break;
                            case NextTileDirection.DOWN: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 90f : -90f; break;
                            case NextTileDirection.UP_RIGHT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -45f : 45f; break;
                            case NextTileDirection.UP_LEFT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -135f : 135f; break;
                            case NextTileDirection.DOWN_LEFT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 135f : -135f; break;
                            case NextTileDirection.DOWN_RIGHT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 45f : -45f; break;
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
        private void GenerateArrowsFromDirectionMap()
        {
            if (_directionMap == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = Instantiate(_arrowPrefab);
                    _arrowsSprites.Add(arrow);
                    arrow.position = _grid.TileMap.CellToWorld(new Vector3Int(j, i)) + new Vector3(_grid.TileMap.transform.localScale.x / 2, _grid.TileMap.transform.localScale.y / 2) + (_grid.TileMap.transform.parent.localScale.y < 0 ? Vector3.down * _grid.TileMap.transform.localScale.y : Vector3.zero); ;
                    arrow.localScale = _grid.transform.localScale;
                    float angle = 0f;
                    bool hasDirection = _directionMap.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (hasDirection)
                    {
                        NextTileDirection nextDirection = _directionMap.GetNextTileDirectionFromTile(_grid.Map, _grid.Map[i, j]);
                        switch (nextDirection)
                        {
                            case NextTileDirection.RIGHT: angle = 0f; break;
                            case NextTileDirection.UP: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -90f : 90f; break;
                            case NextTileDirection.LEFT: angle = 180f; break;
                            case NextTileDirection.DOWN: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 90f : -90f; break;
                            case NextTileDirection.UP_RIGHT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -45f : 45f; break;
                            case NextTileDirection.UP_LEFT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -135f : 135f; break;
                            case NextTileDirection.DOWN_LEFT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 135f : -135f; break;
                            case NextTileDirection.DOWN_RIGHT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 45f : -45f; break;
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
        private void GenerateDistanceFromDijkstraMap()
        {
            if (_dijkstraMap == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform distanceSprite = Instantiate(_distancePrefab);
                    _distanceSprites.Add(distanceSprite);
                    distanceSprite.position = _grid.TileMap.CellToWorld(new Vector3Int(j, i)) + new Vector3(_grid.TileMap.transform.localScale.x / 2, _grid.TileMap.transform.localScale.y / 2) + (_grid.TileMap.transform.parent.localScale.y < 0 ? Vector3.down * _grid.TileMap.transform.localScale.y : Vector3.zero); ;
                    distanceSprite.localScale = _grid.transform.localScale;
                    bool isWall = !_dijkstraMap.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (isWall)
                    {
                        distanceSprite.gameObject.SetActive(false);
                    }
                    else
                    {
                        distanceSprite.GetComponent<TextMeshPro>().text = _dijkstraMap.GetDistanceToTarget(_grid.Map, _grid.Map[i, j]).ToString("F0");
                    }
                }
            }
        }
        private void GetPathFromMap()
        {
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIRECTION_MAP:
                    _grid.TintStart(_startTile);
                    if (_directionMap.IsTileAccessible(_grid.Map, _startTile))
                    {
                        _grid.TintPathTiles(_directionMap.GetPathToTarget(_grid.Map, _startTile, false, false));
                    }
                    break;
                case PathfindingType.DIJKSTRA_MAP:
                    _grid.TintStart(_startTile);
                    if (_directionMap.IsTileAccessible(_grid.Map, _startTile))
                    {
                        _grid.TintPathTiles(_dijkstraMap.GetPathToTarget(_grid.Map, _startTile, false, false));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}