using Caskev.GridToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;
namespace GridToolkitWorkingProject.Samples.RealTimeShooter
{
    public class GridMap : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _progressLabel;
        [SerializeField] private Mob _mobPrefab;
        [SerializeField] private float _spawnDelay;
        [SerializeField] private Transform _mobs;
        private float _nextSpawnTime;
        private Tile[,] _map;
        private Tile[] _highlightedTiles = new Tile[0];
        private Tile[] _shootTiles = new Tile[0];
        private DirectionAtlas _directionAtlas;
        private PlayerController _player;
        private Camera _camera;
        private CancellationTokenSource _cts;
        public Tile[,] Map { get => _map; }
        private void Awake()
        {
            _player = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
            _camera = Camera.main;
        }
        private void Start()
        {
            RegisterTiles();
            if (!File.Exists(Application.persistentDataPath + "/Direction.atlas"))
            {
                GenerateAtlas();
            }
            else
            {
                Initialize();
            }
            
        }
        private async void Initialize()
        {
            byte[] savedAtlas = File.ReadAllBytes(Application.persistentDataPath + "/Direction.atlas");
            Debug.Log("Deserializing Atlas...");
            System.Progress<float> progressIndicator = new System.Progress<float>((progress) =>
            {
                _progressLabel.text = "Loading atlas...\n"+(progress*100).ToString("F0")+"%";
            });
            _cts = new CancellationTokenSource();
            _progressLabel.transform.parent.parent.gameObject.SetActive(true);
            try
            {
                _directionAtlas = await DirectionAtlas.FromByteArrayAsync(_map, savedAtlas, progressIndicator, _cts.Token);
            }
            catch (Exception)
            {
                throw;
            }
            _progressLabel.transform.parent.parent.gameObject.SetActive(false);
            Debug.Log("Atlas deserialized");
            _player.gameObject.SetActive(true);
            OnPlayerTileChange();
        }
        public void RegisterTiles()
        {
            Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
            int maxX = 0;
            int maxY = 0;
            foreach (Tile tile in tiles)
            {
                tile.X = Mathf.RoundToInt(tile.transform.position.x);
                tile.Y = Mathf.RoundToInt(tile.transform.position.z);
                if (tile.X > maxX)
                {
                    maxX = tile.X;
                }
                if (tile.Y > maxY)
                {
                    maxY = tile.Y;
                }
            }
            _map = new Tile[maxY + 1, maxX + 1];

            foreach (Tile tile in tiles)
            {
                _map[tile.Y, tile.X] = tile;
            }
        }
        public async void GenerateAtlas()
        {
            System.Progress<float> progressIndicator = new System.Progress<float>((progress) =>
            {
                _progressLabel.text = "Generating atlas...\n" + (progress * 100).ToString("F0")+"%";
            });
            _cts = new CancellationTokenSource();
            DirectionAtlas directionAtlas = null;
            _progressLabel.transform.parent.parent.gameObject.SetActive(true);
            try
            {
                directionAtlas = await Pathfinding.GenerateDirectionAtlasAsync(_map, DiagonalsPolicy.NONE, progressIndicator, _cts.Token);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Atlas generation was cancelled");
                return;
            }
            Debug.Log("Writing asset...");
            try
            {
                System.Progress<float> progressIndicator2 = new System.Progress<float>((progress) =>
                {
                    _progressLabel.text = "Saving atlas...\n" + (progress * 100).ToString("F0")+"%";
                });
                byte[] bytes = await directionAtlas.ToByteArrayAsync(progressIndicator2, _cts.Token);
                File.WriteAllBytes(Application.persistentDataPath+"/Direction.atlas", bytes);
                Debug.Log($"Serialized Atlas ({bytes.Length})");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
            _progressLabel.transform.parent.parent.gameObject.SetActive(false);
            Debug.Log("Atlas generation is done");
            Initialize();
        }
        private void SpawnMob()
        {
            bool done = false;
            while (!done)
            {
                Tile mobTile = _map[Random.Range(0, _map.GetLength(0)), Random.Range(0, _map.GetLength(1))];
                if (mobTile.IsWalkable && !GridUtils.TileEquals(mobTile, GetPlayerTile()) && _directionAtlas.HasPath(_map, mobTile, GetPlayerTile()))
                {
                    done = true;
                    Instantiate(_mobPrefab, new Vector3(mobTile.X, 0f, mobTile.Y), Quaternion.identity, _mobs);
                }
            }
        }
        private void Update()
        {
            if (_directionAtlas == null)
            {
                return;
            }
            if (Time.time >= _nextSpawnTime)
            {
                SpawnMob();
                _nextSpawnTime = Time.time + _spawnDelay;
            }
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) && Input.GetMouseButton(1))
            {
                _player.Laser.gameObject.SetActive(true);
                Vector3 direction = (hit.point - _player.Laser.position).normalized;
                Vector2 direction2D = new Vector2(direction.x, direction.z);
                ClearShootTiles();
                _shootTiles = Raycasting.GetLineOfSight(_map, GetPlayerTile(), 20, direction2D);
                foreach (Tile tile in _shootTiles)
                {
                    if (Input.GetMouseButton(0))
                    {
                        tile.IsShoot = true;
                        foreach (Transform mob in _mobs)
                        {
                            if (GridUtils.TileEquals(_map[Mathf.RoundToInt(mob.position.z), Mathf.RoundToInt(mob.position.x)], tile))
                            {
                                Destroy(mob.gameObject);
                            }
                        }
                    }
                    else
                    {
                        tile.IsAim = true;
                    }
                }
                _player.Laser.rotation = Quaternion.LookRotation(direction.normalized);
                if (_shootTiles.Length > 0)
                {
                    _player.Laser.localScale = new Vector3(_player.Laser.localScale.x, _player.Laser.localScale.y, Vector3.Distance(_player.transform.position, _shootTiles[_shootTiles.Length - 1].transform.position));
                }
            }
            else
            {
                ClearShootTiles();
                _player.Laser.gameObject.SetActive(false);
            }
        }
        public void OnPlayerTileChange()
        {
            ClearTiles();
            _highlightedTiles = Raycasting.GetConeOfVision(_map, GetPlayerTile(), 20, 360f, 0f);
            foreach (Tile tile in _highlightedTiles)
            {
                tile.IsHighlighted = true;
            }
        }
        private void ClearTiles()
        {
            foreach (Tile tile in _highlightedTiles)
            {
                tile.IsHighlighted = false;
            }
        }
        private void ClearShootTiles()
        {
            foreach (Tile tile in _shootTiles)
            {
                tile.IsAim = false;
                tile.IsShoot = false;
            }
        }
        private Tile GetPlayerTile()
        {
            return _map[_player.CurrentPosition.y, _player.CurrentPosition.x];
        }
        private void OnApplicationQuit()
        {
            CancelAtlasGeneration();
        }
        public Vector2Int GetNextPositionToPlayer(Vector2Int currentPosition)
        {
            Tile nextTile = _directionAtlas.GetNextTile(_map, _map[currentPosition.y, currentPosition.x], GetPlayerTile());
            return new Vector2Int(nextTile.X, nextTile.Y);
        }
        public void CancelAtlasGeneration()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
    }
}
