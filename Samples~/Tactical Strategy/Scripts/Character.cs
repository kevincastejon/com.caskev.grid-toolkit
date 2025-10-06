using Caskev.GridToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GridToolkitWorkingProject.Samples.TacticalStrategy
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private Transform _bulletPrefab;
        private Tile[,] _map;
        private DijkstraField _dijksraField;
        private Tile[] _currentPath = null;
        private int _currentPathIndex;
        private bool _isAttacking;
        private Transform _bullet;

        public DijkstraField DijkstraField { get => _dijksraField; set => _dijksraField = value; }
        public Tile CurrentTile { get => _dijksraField.GetTargetTile(_map); }
        public bool IsMoving { get => _currentPath != null; }
        public string Name { get => _name; }
        public void Init(Tile[,] map)
        {
            _map = map;
            _currentPath = null;
        }
        private void Update()
        {
            if (_currentPath != null)
            {
                if (!_isAttacking)
                {
                    if (Vector3.Distance(ToFixedY(_currentPath[_currentPathIndex].transform.position), ToFixedY(transform.position)) <= 0.1f)
                    {
                        if (_currentPathIndex == _currentPath.Length - 1)
                        {
                            _currentPath = null;
                            _currentPathIndex = 0;
                        }
                        else
                        {
                            _currentPathIndex++;
                        }
                    }
                    else
                    {
                        transform.Translate((ToFixedY(_currentPath[_currentPathIndex].transform.position) - ToFixedY(transform.position)).normalized * Time.deltaTime * 4f);
                    }
                }
                else
                {
                    if (Vector3.Distance(ToFixedY(_currentPath[_currentPathIndex].transform.position), ToFixedY(_bullet.position)) <= 0.1f)
                    {
                        if (_currentPathIndex == _currentPath.Length - 1)
                        {
                            _currentPath = null;
                            _currentPathIndex = 0;
                            Destroy(_bullet.gameObject);
                        }
                        else
                        {
                            _currentPathIndex++;
                        }
                    }
                    else
                    {
                        _bullet.Translate((ToFixedY(_currentPath[_currentPathIndex].transform.position) - ToFixedY(_bullet.position)).normalized * Time.deltaTime * 8f);
                    }
                }
            }
        }

        public void Move(Tile[] tiles)
        {
            _currentPath = tiles;
            _isAttacking = false;
        }
        public void Attack(Tile tile)
        {
            _currentPath = new Tile[] { tile };
            _isAttacking = true;
            _bullet = Instantiate(_bulletPrefab, ToFixedY(transform.position, 1f), Quaternion.identity);
        }

        private Vector3 ToFixedY(Vector3 vector, float yValue = 0f)
        {
            return new Vector3(vector.x, yValue, vector.z);
        }
    }
}