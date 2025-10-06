using Caskev.GridToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GridToolkitWorkingProject.Samples.APIPlayground
{
    public class PathfindingController : MonoBehaviour
    {
        public enum PathfindingType
        {
            DIRECTION_GRID,
            DIJKSTRA_GRID,
            DIRECTION_FIELD,
            DIJKSTRA_FIELD,
            PATH
        }
        [SerializeField] private GameObject _uiContent;
        [SerializeField] private Transform _directionPrefab;
        [SerializeField] private Transform _distancePrefab;
        [SerializeField] private TextMeshProUGUI _progressWindow;
        [SerializeField] private TMP_Dropdown _pathfindingType;
        [SerializeField] private TMP_Dropdown _diagonalsPolicy;
        [SerializeField] private Slider _diagonalsWeight;
        [SerializeField] private Slider _maxDistance;
        [SerializeField] private TextMeshProUGUI _hoveredTileLabel;
        private Transform[,] _directionSprites;
        private Transform[,] _distanceSprites;
        private CanvasGroup _canvasGroup;
        private GridController _grid;
        private DirectionGrid _directionGrid;
        private DirectionField _directionField;
        private DijkstraGrid _dijkstraGrid;
        private DijkstraField _dijkstraField;
        private Tile[] _uniquePath;
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
            HideDirectionSprites();
            HideDistanceSprites();
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
            _maxDistance.onValueChanged.AddListener((x) => UpdateParameters());
            GenerateDirectionSprites();
            GenerateDistanceSprites();
            HideDirectionSprites();
            HideDistanceSprites();
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
            _diagonalsWeight.transform.parent.gameObject.SetActive((((PathfindingType)_pathfindingType.value) == PathfindingType.DIJKSTRA_GRID || ((PathfindingType)_pathfindingType.value) == PathfindingType.DIJKSTRA_FIELD || ((PathfindingType)_pathfindingType.value) == PathfindingType.PATH) && ((DiagonalsPolicy)_diagonalsPolicy.value) != DiagonalsPolicy.NONE);
            _maxDistance.transform.parent.gameObject.SetActive(((PathfindingType)_pathfindingType.value) == PathfindingType.DIRECTION_FIELD || ((PathfindingType)_pathfindingType.value) == PathfindingType.DIJKSTRA_FIELD);
            GenerateMap();
        }
        private void GenerateMap()
        {
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIRECTION_GRID:
                    GenerateDirectionGrid();
                    break;
                case PathfindingType.DIJKSTRA_GRID:
                    GenerateDijkstraGrid();
                    break;
                case PathfindingType.DIRECTION_FIELD:
                    GenerateDirectionField();
                    break;
                case PathfindingType.DIJKSTRA_FIELD:
                    GenerateDijkstraField();
                    break;
                case PathfindingType.PATH:
                    GenerateUniquePath();
                    break;
                default:
                    break;
            }
        }
        private async void GenerateDirectionGrid()
        {
            _canvasGroup.interactable = false;
            _progressWindow.transform.parent.gameObject.SetActive(true);
            _cts = new System.Threading.CancellationTokenSource();
            try
            {
                _directionGrid = await Pathfinding.GenerateDirectionGridAsync(_grid.Map, _targetTile, (DiagonalsPolicy)_diagonalsPolicy.value, new Progress<float>((x) => _progressWindow.text = (x * 100).ToString("F0") + "%"), _cts.Token);
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
            UpdateDirectionSprites();
            UpdateDistanceSprites();
            bool isAccessible = _directionGrid.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _directionGrid.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            _grid.TintCenter(_targetTile);
            _grid.TintPathTiles(path);
            _grid.TintStart(_startTile);
            _grid.TintHighlightedTiles(new Tile[0]);
        }
        private async void GenerateDirectionField()
        {
            _canvasGroup.interactable = false;
            _progressWindow.transform.parent.gameObject.SetActive(true);
            _cts = new System.Threading.CancellationTokenSource();
            DirectionField directionField;
            try
            {
                directionField = await Pathfinding.GenerateDirectionFieldAsync(_grid.Map, _targetTile, Mathf.FloorToInt(_maxDistance.value), (DiagonalsPolicy)_diagonalsPolicy.value, new Progress<float>((x) => _progressWindow.text = (x * 100).ToString("F0") + "%"), _cts.Token);
            }
            catch (Exception e)
            {
                _progressWindow.transform.parent.gameObject.SetActive(false);
                _canvasGroup.interactable = true;
                _cts = null;
                throw e;
            }
            _directionField = directionField;
            _progressWindow.transform.parent.gameObject.SetActive(false);
            _canvasGroup.interactable = true;
            _cts = null;
            UpdateDirectionSprites();
            UpdateDistanceSprites();
            bool isAccessible = _directionField.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _directionField.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            Tile[] allAccessibleTiles = new Tile[_directionField.AccessibleTilesCount];
            for (int i = 0; i < _directionField.AccessibleTilesCount; i++)
            {
                allAccessibleTiles[i] = _directionField.GetAccessibleTile(_grid.Map, i);
            }
            _grid.TintCenter(_targetTile);
            _grid.TintStart(_startTile);
            if (path.Length == 0)
            {
                _grid.ClearPathTiles();
            }
            else
            {
                _grid.TintPathTiles(path);
            }
            _grid.TintHighlightedTiles(allAccessibleTiles);
        }
        private async void GenerateDijkstraGrid()
        {
            _canvasGroup.interactable = false;
            _progressWindow.transform.parent.gameObject.SetActive(true);
            _cts = new System.Threading.CancellationTokenSource();
            try
            {
                _dijkstraGrid = await Pathfinding.GenerateDijkstraGridAsync(_grid.Map, _targetTile, (DiagonalsPolicy)_diagonalsPolicy.value, _diagonalsWeight.value, new Progress<float>((x) => _progressWindow.text = (x * 100).ToString("F0") + "%"), _cts.Token);
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
            UpdateDirectionSprites();
            UpdateDistanceSprites();
            bool isAccessible = _dijkstraGrid.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _dijkstraGrid.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            _grid.TintCenter(_targetTile);
            _grid.TintPathTiles(path);
            _grid.TintStart(_startTile);
            _grid.TintHighlightedTiles(new Tile[0]);
        }
        private async void GenerateDijkstraField()
        {
            _canvasGroup.interactable = false;
            _progressWindow.transform.parent.gameObject.SetActive(true);
            _cts = new System.Threading.CancellationTokenSource();
            try
            {
                _dijkstraField = await Pathfinding.GenerateDijkstraFieldAsync(_grid.Map, _targetTile, _maxDistance.value, (DiagonalsPolicy)_diagonalsPolicy.value, _diagonalsWeight.value, new Progress<float>((x) => _progressWindow.text = (x * 100).ToString("F0") + "%"), _cts.Token);
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
            UpdateDirectionSprites();
            UpdateDistanceSprites();
            bool isAccessible = _dijkstraField.IsTileAccessible(_grid.Map, _startTile);
            Tile[] path = new Tile[0];
            if (isAccessible)
            {
                path = _dijkstraField.GetPathToTarget(_grid.Map, _startTile, false, false);
            }
            Tile[] allAccessibleTiles = new Tile[_dijkstraField.AccessibleTilesCount];
            for (int i = 0; i < _dijkstraField.AccessibleTilesCount; i++)
            {
                allAccessibleTiles[i] = _dijkstraField.GetAccessibleTile(_grid.Map, i);
            }
            _grid.TintCenter(_targetTile);
            _grid.TintStart(_startTile);
            if (path.Length == 0)
            {
                _grid.ClearPathTiles();
            }
            else
            {
                _grid.TintPathTiles(path);
            }
            _grid.TintHighlightedTiles(allAccessibleTiles);
        }
        private async void GenerateUniquePath()
        {
            _canvasGroup.interactable = false;
            _progressWindow.transform.parent.gameObject.SetActive(true);
            _cts = new System.Threading.CancellationTokenSource();
            try
            {
                _uniquePath = await Pathfinding.GenerateUniquePathAsync(_grid.Map, _targetTile, _startTile, (DiagonalsPolicy)_diagonalsPolicy.value, _diagonalsWeight.value, true, true, new Progress<float>((x) => _progressWindow.text = (x * 100).ToString("F0") + "%"), _cts.Token);
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
            UpdateDirectionSprites();
            UpdateDistanceSprites();
            if (_uniquePath == null)
            {
                _grid.TintPathTiles(new Tile[0]);
                HideDirectionSprites();
            }
            else
            {
                _grid.TintPathTiles(_uniquePath);
            }
            _grid.TintCenter(_targetTile);
            _grid.TintStart(_startTile);
            _grid.TintHighlightedTiles(new Tile[0]);
        }
        private void HideDirectionSprites()
        {
            foreach (Transform arrow in _directionSprites)
            {
                if (arrow != null)
                {
                    arrow.gameObject.SetActive(false);
                }
            }
        }
        private void HideDistanceSprites()
        {
            foreach (Transform distanceSprite in _distanceSprites)
            {
                if (distanceSprite != null)
                {
                    distanceSprite.gameObject.SetActive(false);
                }
            }
        }
        private void UpdateDirectionSprites()
        {
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIRECTION_GRID:
                    UpdateDirectionSpritesFromDirectionGrid();
                    break;
                case PathfindingType.DIJKSTRA_GRID:
                    UpdateDirectionSpritesFromDijkstraGrid();
                    break;
                case PathfindingType.DIRECTION_FIELD:
                    UpdateDirectionSpritesFromDirectionField();
                    break;
                case PathfindingType.DIJKSTRA_FIELD:
                    UpdateDirectionSpritesFromDijkstraField();
                    break;
                case PathfindingType.PATH:
                    UpdateDirectionSpritesFromUniquePath();
                    break;
                default:
                    break;
            }
        }
        private void UpdateDistanceSprites()
        {
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIJKSTRA_GRID:
                    UpdateDistanceSpritesFromDijkstraGrid();
                    break;
                case PathfindingType.DIJKSTRA_FIELD:
                    UpdateDistanceSpritesFromDijkstraField();
                    break;
                default:
                    HideDistanceSprites();
                    break;
            }
        }
        private void GenerateDirectionSprites()
        {
            _directionSprites = new Transform[_grid.Map.GetLength(0), _grid.Map.GetLength(1)];
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = Instantiate(_directionPrefab);
                    _directionSprites[i, j] = arrow;
                    arrow.position = _grid.TileMap.CellToWorld(new Vector3Int(j, i)) + new Vector3(_grid.TileMap.transform.localScale.x / 2, _grid.TileMap.transform.localScale.y / 2) + (_grid.TileMap.transform.parent.localScale.y < 0 ? Vector3.down * _grid.TileMap.transform.localScale.y : Vector3.zero); ;
                    arrow.localScale = _grid.transform.localScale;
                }
            }
        }
        private void UpdateDirectionSpritesFromDijkstraGrid()
        {
            if (_dijkstraGrid == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = _directionSprites[i, j];
                    float angle = 0f;
                    bool hasDirection = _dijkstraGrid.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (hasDirection)
                    {
                        NextTileDirection nextDirection = _dijkstraGrid.GetNextTileDirectionFromTile(_grid.Map, _grid.Map[i, j]);
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
                        arrow.gameObject.SetActive(true);
                        arrow.rotation = rot;
                    }
                    else
                    {
                        arrow.gameObject.SetActive(false);
                    }
                }
            }
        }
        private void UpdateDirectionSpritesFromDijkstraField()
        {
            if (_dijkstraField == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = _directionSprites[i, j];
                    float angle = 0f;
                    bool hasDirection = _dijkstraField.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (hasDirection)
                    {
                        NextTileDirection nextDirection = _dijkstraField.GetNextTileDirectionFromTile(_grid.Map, _grid.Map[i, j]);
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
                        arrow.gameObject.SetActive(true);
                        arrow.rotation = rot;
                    }
                    else
                    {
                        arrow.gameObject.SetActive(false);
                    }
                }
            }
        }
        private void UpdateDirectionSpritesFromUniquePath()
        {
            if (_uniquePath == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = _directionSprites[i, j];
                    arrow.gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < _uniquePath.Length; i++)
            {
                float angle = 0f;
                if (i < _uniquePath.Length - 1)
                {
                    NextTileDirection direction = GridUtils.GetDirectionBetweenAdjacentTiles(_uniquePath[i], _uniquePath[i + 1]);
                    switch (direction)
                    {
                        case NextTileDirection.RIGHT: angle = 0f; break;
                        case NextTileDirection.UP: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -90f : 90f; break;
                        case NextTileDirection.LEFT: angle = 180f; break;
                        case NextTileDirection.DOWN: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 90f : -90f; break;
                        case NextTileDirection.UP_RIGHT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -45f : 45f; break;
                        case NextTileDirection.UP_LEFT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? -135f : 135f; break;
                        case NextTileDirection.DOWN_LEFT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 135f : -135f; break;
                        case NextTileDirection.DOWN_RIGHT: angle = _grid.TileMap.transform.parent.localScale.y < 0 ? 45f : -45f; break;
                    }
                    Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                    Transform arrow = _directionSprites[_uniquePath[i].Y, _uniquePath[i].X];
                    arrow.gameObject.SetActive(true);
                    arrow.rotation = rot;
                }
            }
        }

        private void UpdateDirectionSpritesFromDirectionGrid()
        {
            if (_directionGrid == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = _directionSprites[i, j];
                    float angle = 0f;
                    bool hasDirection = _directionGrid.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (hasDirection)
                    {
                        NextTileDirection nextDirection = _directionGrid.GetNextTileDirectionFromTile(_grid.Map, _grid.Map[i, j]);
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
                        arrow.gameObject.SetActive(true);
                        arrow.rotation = rot;
                    }
                    else
                    {
                        arrow.gameObject.SetActive(false);
                    }
                }
            }
        }
        private void UpdateDirectionSpritesFromDirectionField()
        {
            if (_directionField == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform arrow = _directionSprites[i, j];
                    float angle = 0f;
                    bool hasDirection = _directionField.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (hasDirection)
                    {
                        NextTileDirection nextDirection = _directionField.GetNextTileDirectionFromTile(_grid.Map, _grid.Map[i, j]);
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
                        arrow.gameObject.SetActive(true);
                        arrow.rotation = rot;
                    }
                    else
                    {
                        arrow.gameObject.SetActive(false);
                    }
                }
            }
        }
        private void GenerateDistanceSprites()
        {
            _distanceSprites = new Transform[_grid.Map.GetLength(0), _grid.Map.GetLength(1)];
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform distanceSprite = Instantiate(_distancePrefab);
                    _distanceSprites[i, j] = distanceSprite;
                    distanceSprite.position = _grid.TileMap.CellToWorld(new Vector3Int(j, i)) + new Vector3(_grid.TileMap.transform.localScale.x / 2, _grid.TileMap.transform.localScale.y / 2) + (_grid.TileMap.transform.parent.localScale.y < 0 ? Vector3.down * _grid.TileMap.transform.localScale.y : Vector3.zero); ;
                    distanceSprite.localScale = _grid.transform.localScale;
                }
            }
        }
        private void UpdateDistanceSpritesFromDijkstraGrid()
        {
            if (_dijkstraGrid == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform distanceSprite = _distanceSprites[i, j];
                    bool isWall = !_dijkstraGrid.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (isWall)
                    {
                        distanceSprite.gameObject.SetActive(false);
                    }
                    else
                    {
                        distanceSprite.gameObject.SetActive(true);
                        distanceSprite.GetComponent<TextMeshPro>().text = _dijkstraGrid.GetDistanceToTarget(_grid.Map, _grid.Map[i, j]).ToString("F0");
                    }
                }
            }
        }
        private void UpdateDistanceSpritesFromDijkstraField()
        {
            if (_dijkstraField == null)
            {
                return;
            }
            for (int i = 0; i < _grid.Map.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.Map.GetLength(1); j++)
                {
                    Transform distanceSprite = _distanceSprites[i, j];
                    bool isWall = !_dijkstraField.IsTileAccessible(_grid.Map, _grid.Map[i, j]);
                    if (isWall)
                    {
                        distanceSprite.gameObject.SetActive(false);
                    }
                    else
                    {
                        distanceSprite.gameObject.SetActive(true);
                        distanceSprite.GetComponent<TextMeshPro>().text = _dijkstraField.GetDistanceToTarget(_grid.Map, _grid.Map[i, j]).ToString("F0");
                    }
                }
            }
        }
        private void GetPathFromMap()
        {
            _grid.TintStart(_startTile);
            switch ((PathfindingType)_pathfindingType.value)
            {
                case PathfindingType.DIRECTION_GRID:
                    if (_directionGrid.IsTileAccessible(_grid.Map, _startTile))
                    {
                        _grid.TintPathTiles(_directionGrid.GetPathToTarget(_grid.Map, _startTile, false, false));
                    }
                    else
                    {
                        _grid.ClearPathTiles();
                    }
                    break;
                case PathfindingType.DIJKSTRA_GRID:
                    if (_dijkstraGrid.IsTileAccessible(_grid.Map, _startTile))
                    {
                        _grid.TintPathTiles(_dijkstraGrid.GetPathToTarget(_grid.Map, _startTile, false, false));
                    }
                    else
                    {
                        _grid.ClearPathTiles();
                    }
                    break;
                case PathfindingType.DIRECTION_FIELD:
                    if (_directionField.IsTileAccessible(_grid.Map, _startTile))
                    {
                        _grid.TintPathTiles(_directionField.GetPathToTarget(_grid.Map, _startTile, false, false));
                    }
                    else
                    {
                        _grid.ClearPathTiles();
                    }
                    break;
                case PathfindingType.DIJKSTRA_FIELD:
                    if (_dijkstraField.IsTileAccessible(_grid.Map, _startTile))
                    {
                        _grid.TintPathTiles(_dijkstraField.GetPathToTarget(_grid.Map, _startTile, false, false));
                    }
                    else
                    {
                        _grid.ClearPathTiles();
                    }
                    break;
                case PathfindingType.PATH:
                    GenerateUniquePath();
                    break;
                default:
                    break;
            }
        }
    }
}