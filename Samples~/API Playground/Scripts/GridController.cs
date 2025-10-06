using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Caskev.GridToolkit;

namespace GridToolkitWorkingProject.Samples.APIPlayground
{
    [DefaultExecutionOrder(-100)]
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Transform _selectionOutline;
        [SerializeField] private Transform _clampedSelectionOutline;
        private readonly Color _floorColor = Color.white;
        private readonly Color _centerColor = Color.red;
        private readonly Color _wallColor = Color.black;
        private readonly Color _highlightedColor = Color.cyan;
        private readonly Color _pathColor = Color.green;
        private readonly Color _startColor = Color.magenta;
        private Camera _camera;
        private Tilemap _tileMap;
        private Tile[,] _map;
        private Tile _centerTile;
        private Tile _startTile;
        private Tile[] _highlightedTiles = new Tile[0];
        private Tile[] _pathTiles = new Tile[0];
        private Tile _hoveredTile;
        private Tile _clampedHoveredTile;
        private bool _justEnteredTile;
        private bool _justEnteredClampedTile;
        private bool _mouseEnabled = true;
        private bool _startTileEnabled = true;

        public Tile[,] Map { get => _map; }
        public Tile HoveredTile { get => _hoveredTile; }
        public bool JustEnteredTile { get => _justEnteredTile; }
        public bool JustEnteredClampedTile { get => _justEnteredClampedTile; }
        public Tile CenterTile { get => _centerTile; }
        public Tile StartTile { get => _startTile; }
        public Tile ClampedHoveredTile { get => _clampedHoveredTile; }
        public bool MouseEnabled { get => _mouseEnabled; set => _mouseEnabled = value; }
        public Tilemap TileMap => _tileMap;
        public bool StartTileEnabled
        {
            get => _startTileEnabled;
            set
            {
                _startTileEnabled = value;
                if (!_startTileEnabled && _startTile != null)
                {
                    if (_highlightedTiles.Contains(_startTile))
                    {
                        _tileMap.SetColor(new Vector3Int(_startTile.X, _startTile.Y), _highlightedColor);
                    }
                    else
                    {
                        _tileMap.SetColor(new Vector3Int(_startTile.X, _startTile.Y), _floorColor);
                    }
                }
            }
        }

        private void Awake()
        {
            _camera = Camera.main;
            _tileMap = GetComponent<Tilemap>();
            _map = new Tile[_tileMap.size.y, _tileMap.size.x];
            for (int y = 0; y < _tileMap.size.y; y++)
            {
                for (int x = 0; x < _tileMap.size.x; x++)
                {
                    Color tile = _tileMap.GetColor(new Vector3Int(x, y));
                    if (tile == _centerColor)
                    {
                        _map[y, x] = new Tile(x, y, true);
                        _centerTile = _map[y, x];
                    }
                    else if (tile == _startColor)
                    {
                        _map[y, x] = new Tile(x, y, true);
                        _startTile = _map[y, x];
                    }
                    else
                    {
                        _map[y, x] = new Tile(x, y, tile == _floorColor);
                    }
                }
            }
        }
        private void Update()
        {
            _justEnteredTile = false; ;
            _justEnteredClampedTile = false;
            if (!_mouseEnabled)
            {
                _hoveredTile = null;
                _clampedHoveredTile = null;
                return;
            }
            Vector3Int hoveredCoordinates = _tileMap.WorldToCell(_camera.ScreenToWorldPoint(Input.mousePosition));
            Vector3Int clampedHoveredCoordinates = new Vector3Int(Mathf.Clamp(hoveredCoordinates.x, 0, _tileMap.size.x - 1), Mathf.Clamp(hoveredCoordinates.y, 0, _tileMap.size.y - 1));
            if (hoveredCoordinates.x >= 0 && hoveredCoordinates.x < _tileMap.size.x && hoveredCoordinates.y >= 0 && hoveredCoordinates.y < _tileMap.size.y)
            {
                Tile tile = _map[Mathf.FloorToInt(hoveredCoordinates.y), Mathf.FloorToInt(hoveredCoordinates.x)];
                if (_hoveredTile == null || !GridUtils.TileEquals(tile, _hoveredTile))
                {
                    _hoveredTile = tile;
                    _justEnteredTile = true;
                    if (_selectionOutline)
                    {
                        _selectionOutline.localScale = _tileMap.transform.localScale;
                        _selectionOutline.position = _tileMap.CellToWorld(hoveredCoordinates) + new Vector3(_tileMap.transform.localScale.x / 2, _tileMap.transform.localScale.y / 2);
                    }
                }
            }
            else
            {
                _hoveredTile = null;
            }
            Tile clampedTile = _map[Mathf.FloorToInt(clampedHoveredCoordinates.y), Mathf.FloorToInt(clampedHoveredCoordinates.x)];
            if (_clampedHoveredTile == null || !GridUtils.TileEquals(clampedTile, _clampedHoveredTile))
            {
                _clampedHoveredTile = clampedTile;
                _justEnteredClampedTile = true;
                if (_clampedSelectionOutline)
                {
                    _clampedSelectionOutline.localScale = _tileMap.transform.localScale;
                    _clampedSelectionOutline.position = _tileMap.CellToWorld(clampedHoveredCoordinates) + new Vector3(_tileMap.transform.localScale.x / 2, _tileMap.transform.localScale.y / 2) + (_tileMap.transform.parent.localScale.y < 0 ? Vector3.down * _tileMap.transform.localScale.y : Vector3.zero);
                }
            }
        }
        public void SetWalkable(Tile tile, bool isWalkable)
        {
            ClearPathTiles();
            ClearHighlightedTiles();
            tile.IsWalkable = isWalkable;
            _tileMap.SetColor(new Vector3Int(tile.X, tile.Y), isWalkable ? _floorColor : _wallColor);
        }
        public void ClearWalkables()
        {
            ClearHighlightedTiles();
            ClearPathTiles();
            ClearCenter();
            ClearStart();
        }
        public void TintCenter(Tile tile)
        {
            if (tile == null)
            {
                return;
            }
            ClearCenter();
            _centerTile = tile;
            _tileMap.SetColor(new Vector3Int(_centerTile.X, _centerTile.Y), _centerColor);
        }
        public void TintStart(Tile tile)
        {
            if (tile == null)
            {
                return;
            }
            ClearStart();
            _startTile = tile;
            if (_startTileEnabled)
            {
                _tileMap.SetColor(new Vector3Int(_startTile.X, _startTile.Y), _startColor);
            }
        }
        public void TintHighlightedTiles(Tile[] highlightedTiles)
        {
            if (highlightedTiles == null)
            {
                return;
            }
            ClearHighlightedTiles();
            _highlightedTiles = highlightedTiles;
            for (int i = 0; i < _highlightedTiles.Length; i++)
            {
                if ((!_startTileEnabled || _highlightedTiles[i] != _startTile) && (_highlightedTiles[i] != _centerTile) && (!_pathTiles.Contains(_highlightedTiles[i])))
                {
                    _tileMap.SetColor(new Vector3Int(_highlightedTiles[i].X, _highlightedTiles[i].Y), _highlightedColor);
                }
            }
        }
        public void TintPathTiles(Tile[] pathTiles)
        {
            if (pathTiles == null)
            {
                return;
            }
            ClearPathTiles();
            _pathTiles = pathTiles;
            for (int i = 0; i < _pathTiles.Length; i++)
            {
                _tileMap.SetColor(new Vector3Int(_pathTiles[i].X, _pathTiles[i].Y), _pathColor);
            }
        }
        public void ClearCenter()
        {
            if (_centerTile == null)
            {
                return;
            }
            if (_pathTiles.Contains(_centerTile))
            {
                _tileMap.SetColor(new Vector3Int(_centerTile.X, _centerTile.Y), _pathColor);
            }
            else if (_highlightedTiles.Contains(_centerTile))
            {
                _tileMap.SetColor(new Vector3Int(_centerTile.X, _centerTile.Y), _highlightedColor);
            }
            else
            {
                if (_centerTile.IsWalkable)
                {
                    _tileMap.SetColor(new Vector3Int(_centerTile.X, _centerTile.Y), _floorColor);
                }
                else
                {
                    _tileMap.SetColor(new Vector3Int(_centerTile.X, _centerTile.Y), _wallColor);
                }
            }
            _centerTile = null;
        }
        public void ClearStart()
        {
            if (_startTile == null)
            {
                return;
            }
            if (_pathTiles.Contains(_centerTile))
            {
                _tileMap.SetColor(new Vector3Int(_startTile.X, _startTile.Y), _pathColor);
            }
            else if (_highlightedTiles.Contains(_startTile))
            {
                _tileMap.SetColor(new Vector3Int(_startTile.X, _startTile.Y), _highlightedColor);
            }
            else
            {
                if (_startTile.IsWalkable)
                {
                    _tileMap.SetColor(new Vector3Int(_startTile.X, _startTile.Y), _floorColor);
                }
                else
                {
                    _tileMap.SetColor(new Vector3Int(_startTile.X, _startTile.Y), _wallColor);
                }
            }
            _startTile = null;
        }
        public void ClearHighlightedTiles()
        {
            for (int i = 0; i < _highlightedTiles.Length; i++)
            {
                if (!_pathTiles.Contains(_highlightedTiles[i]) && _highlightedTiles[i] != _centerTile && (!_startTileEnabled || _highlightedTiles[i] != _startTile))
                {
                    _tileMap.SetColor(new Vector3Int(_highlightedTiles[i].X, _highlightedTiles[i].Y), _floorColor);
                }
            }
            _highlightedTiles = new Tile[0];
        }
        public void ClearPathTiles()
        {
                for (int i = 0; i < _pathTiles.Length; i++)
                {
                    if (_pathTiles[i] != _centerTile && _pathTiles[i] != _startTile)
                    {
                        if (_highlightedTiles.Contains(_pathTiles[i]))
                        {
                            _tileMap.SetColor(new Vector3Int(_pathTiles[i].X, _pathTiles[i].Y), _highlightedColor);
                        }
                        else
                        {
                            _tileMap.SetColor(new Vector3Int(_pathTiles[i].X, _pathTiles[i].Y), _floorColor);
                        }
                    }
                }
            _pathTiles = new Tile[0];
        }
    }
}