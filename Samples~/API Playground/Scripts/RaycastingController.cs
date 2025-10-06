using Caskev.GridToolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GridToolkitWorkingProject.Samples.APIPlayground
{
    public class RaycastingController : MonoBehaviour
    {
        public enum DemoType
        {
            LINE_OF_SIGHT,
            CONE_OF_VISION,
        }
        [SerializeField] private GameObject _uiContent;
        [SerializeField] private Image _lineClearLED;
        [SerializeField] private Image _coneClearLED;
        [SerializeField] private TMP_Dropdown _demoTypeDropDown;
        [SerializeField] private Toggle _allowDiagonalsToggle;
        [SerializeField] private Toggle _favorVerticalToggle;
        [SerializeField] private Slider _lengthSlider;
        [SerializeField] private Slider _directionSlider;
        [SerializeField] private Slider _angleSlider;
        [SerializeField] private TextMeshProUGUI _hoveredTileLabel;
        [SerializeField] private bool _allowDiagonals = true;
        [SerializeField] private bool _favorVertical = false;
        [SerializeField] private int _length = 5;
        [SerializeField] private float _direction = 0f;
        [SerializeField] private float _angle = 90f;
        private GridController _grid;
        private Image[] _allLeds;
        private DemoType _demoType;
        private Tile _centerTile;
        private bool _walling;
        private bool _startWalkableValue;
        private bool _isQuitting = false;

        private void OnEnable()
        {
            _uiContent.SetActive(true);
            if (_centerTile != null)
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
        }
        private void Awake()
        {
            _grid = FindAnyObjectByType<GridController>();
            _allLeds = new Image[] { _lineClearLED, _coneClearLED };
            _allowDiagonalsToggle.isOn = _allowDiagonals;
            _favorVerticalToggle.isOn = _favorVertical;
            _lengthSlider.value = _length;
            _directionSlider.value = _direction;
            _angleSlider.value = _angle;
            _demoTypeDropDown.value = (int)_demoType;
            _centerTile = _grid.CenterTile;

            _allowDiagonalsToggle.onValueChanged.AddListener((x) =>
            {
                _allowDiagonals = x; Raycast();
            });
            _favorVerticalToggle.onValueChanged.AddListener((x) =>
            {
                _favorVertical = x; Raycast();
            });
            _lengthSlider.onValueChanged.AddListener((x) =>
            {
                _length = Mathf.FloorToInt(x); Raycast();
            });
            _directionSlider.onValueChanged.AddListener((x) =>
            {
                _direction = x; Raycast();
            });
            _angleSlider.onValueChanged.AddListener((x) =>
            {
                _angle = x; Raycast();
            });
            _demoTypeDropDown.onValueChanged.AddListener((x) =>
            {
                SetCurrentDemoType((DemoType)x); Raycast();
            });
        }
        private void Start()
        {
            _grid.StartTileEnabled = false;
            Raycast();
        }
        private void Update()
        {
            _hoveredTileLabel.text = _grid.ClampedHoveredTile == null ? "" : ("X:" + _grid.ClampedHoveredTile.X + " Y:" + _grid.ClampedHoveredTile.Y);
            if ((_grid.JustEnteredTile && Input.GetMouseButton(0)) || (Input.GetMouseButtonDown(0) && _grid.HoveredTile != null && _grid.HoveredTile != _centerTile))
            {
                if (!_walling)
                {
                    _walling = true;
                    _startWalkableValue = !_grid.HoveredTile.IsWalkable;
                }
                if (_grid.HoveredTile.IsWalkable != _startWalkableValue)
                {
                    _grid.SetWalkable(_grid.HoveredTile, _startWalkableValue);
                    Raycast();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _walling = false;
            }
            else if ((_grid.JustEnteredClampedTile && _grid.ClampedHoveredTile.IsWalkable && Input.GetMouseButton(1)) || (Input.GetMouseButtonDown(1) && _grid.ClampedHoveredTile.IsWalkable && _grid.ClampedHoveredTile != _centerTile))
            {
                _centerTile = _grid.ClampedHoveredTile;
                Raycast();
            }
        }
        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
        private void SetCurrentDemoType(DemoType value)
        {
            if (_demoType == value)
            {
                return;
            }
            _demoType = value;
            HideAllLeds();
            switch (_demoType)
            {
                case DemoType.LINE_OF_SIGHT:
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(true);
                    _favorVerticalToggle.gameObject.SetActive(true);
                    _lineClearLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.CONE_OF_VISION:
                    _angleSlider.transform.parent.gameObject.SetActive(true);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _coneClearLED.transform.parent.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        private void Raycast()
        {
            switch (_demoType)
            {
                case DemoType.LINE_OF_SIGHT:
                    RaycastLine();
                    break;
                case DemoType.CONE_OF_VISION:
                    RaycastCone();
                    break;
                default:
                    break;
            }
        }
        private void RaycastLine()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Raycasting.GetLineOfSight(_grid.Map, out bool isClear, _centerTile, _length, _direction, _allowDiagonals, _favorVertical, false));
            _lineClearLED.color = isClear ? Color.green : Color.red;
        }
        private void RaycastCone()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Raycasting.GetConeOfVision(_grid.Map, out bool isClear, _centerTile, _length, _angle, _direction, false));
            _coneClearLED.color = isClear ? Color.green : Color.red;
        }
        private void HideAllLeds()
        {
            foreach (Image led in _allLeds)
            {
                led.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}