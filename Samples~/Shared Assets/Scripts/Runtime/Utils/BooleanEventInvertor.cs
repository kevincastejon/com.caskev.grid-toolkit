using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace GridToolkitWorkingProject.Samples.Shared
{
    public class BooleanEventInvertor : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> _onChange;

        public void OnChange(bool value)
        {
            _onChange.Invoke(!value);
        }
    }
}
