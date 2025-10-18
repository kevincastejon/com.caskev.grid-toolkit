using Caskev.GridToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GridToolkitWorkingProject.Samples.TowerDefense
{

    public class Character : MonoBehaviour
    {
        DirectionGrid _directionGrid;
        Floor _target;
        Floor[,] _map;
        public void Init(Floor startFloor, Floor[,] map, DirectionGrid directionGrid)
        {
            _map = map;
            _directionGrid = directionGrid;
            _target = _directionGrid.GetNextTile(_map, startFloor);
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_target.transform.position.x, 1.5f, _target.transform.position.z), 2.5f * Time.deltaTime);
            if (Vector3.Distance(transform.position, new Vector3(_target.transform.position.x, 1.5f, _target.transform.position.z)) < 0.1f)
            {
                Floor nextTile = _directionGrid.GetNextTile(_map, _target);
                if (_target == nextTile)
                {
                    Destroy(gameObject);
                }
                else
                {
                    _target = nextTile;
                }
            }
        }
    }

}