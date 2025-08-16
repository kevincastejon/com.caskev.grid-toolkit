using UnityEngine;
namespace GridToolkitWorkingProject.Demos.Shared
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private bool _enableFreeRotation = true;
        [SerializeField] private float _lookSensitivity = 1;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _initialRotation = transform.rotation;
        }

        void Update()
        {
            bool leftClick = Input.GetMouseButton(0);
            bool rightClick = Input.GetMouseButton(1);

            if (leftClick && rightClick)
            {
                transform.rotation = _initialRotation;
            }
            else if (_enableFreeRotation && rightClick)
            {
                FreeRotate();
            }
            else if (Input.GetMouseButton(2))
            {
                RotateY();
            }
        }

        void FreeRotate()
        {
            Vector2 mouseDelta = _lookSensitivity * new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            Quaternion rotation = transform.rotation;
            Quaternion horiz = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);
            transform.rotation = horiz * rotation * vert;
        }
        void RotateY()
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Input.GetAxis("Mouse X"), transform.eulerAngles.z);
        }
    }
}