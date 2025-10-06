using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Caskev.GridToolkit;

namespace GridToolkitWorkingProject.Samples.APIPlayground
{
    public class ExtractionController : MonoBehaviour
    {
        public enum DemoType
        {
            EXTRACT_CIRCLE,
            EXTRACT_CIRCLE_OUTLINE,
            EXTRACT_RECTANGLE,
            EXTRACT_RECTANGLE_OUTLINE,
            EXTRACT_CONE,
            EXTRACT_LINE,
            NEIGHBOR,
            NEIGHBORS_ORTHO,
            NEIGHBORS_DIAGONALS,
            NEIGHBORS_ALL,
        }
        [SerializeField] private GameObject _uiContent;
        [SerializeField] private Image _circleLED;
        [SerializeField] private Image _circleOutlineLED;
        [SerializeField] private Image _rectangleLED;
        [SerializeField] private Image _rectangleOutlineLED;
        [SerializeField] private Image _coneLED;
        [SerializeField] private Image _lineLED;
        [SerializeField] private Image _neiLED;
        [SerializeField] private Image _neiOrthoLED;
        [SerializeField] private Image _neiDiagoLED;
        [SerializeField] private Image _neiAnyLED;
        [SerializeField] private TMP_Dropdown _demoTypeDropDown;
        [SerializeField] private Toggle _allowDiagonalsToggle;
        [SerializeField] private Toggle _favorVerticalToggle;
        [SerializeField] private Slider _radiusSlider;
        [SerializeField] private TextMeshProUGUI _radiusLabel;
        [SerializeField] private Slider _directionSlider;
        [SerializeField] private Slider _angleSlider;
        [SerializeField] private Slider _rectangleSizeXSlider;
        [SerializeField] private Slider _rectangleSizeYSlider;
        [SerializeField] private TextMeshProUGUI _hoveredTileLabel;
        [SerializeField] private bool _allowDiagonals = true;
        [SerializeField] private bool _favorVertical = false;
        [SerializeField][Min(0)] private int _radius = 5;
        [SerializeField] private float _direction = 0f;
        [SerializeField] private float _angle = 90f;
        [SerializeField] private Vector2Int _size = Vector2Int.one * 5;
        private GridController _grid;
        private Image[] _allLeds;
        private DemoType _demoType;
        private Tile _centerTile;
        private Tile _ledTile;
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
            _allLeds = new Image[] { _circleLED, _circleOutlineLED, _rectangleLED, _rectangleOutlineLED, _coneLED, _lineLED, _neiLED, _neiOrthoLED, _neiDiagoLED, _neiAnyLED };
            _allowDiagonalsToggle.SetIsOnWithoutNotify(_allowDiagonals);
            _favorVerticalToggle.SetIsOnWithoutNotify(_favorVertical);
            _radiusSlider.SetValueWithoutNotify(_radius);
            _directionSlider.SetValueWithoutNotify(_direction);
            _angleSlider.SetValueWithoutNotify(_angle);
            _rectangleSizeXSlider.SetValueWithoutNotify(_size.x);
            _rectangleSizeYSlider.SetValueWithoutNotify(_size.y);
            _demoTypeDropDown.SetValueWithoutNotify((int)_demoType);
            _centerTile = _grid.CenterTile;
            _allowDiagonalsToggle.onValueChanged.AddListener((x) =>
            {
                _allowDiagonals = x; Extract();
            });
            _favorVerticalToggle.onValueChanged.AddListener((x) =>
            {
                _favorVertical = x; Extract();
            });
            _radiusSlider.onValueChanged.AddListener((x) =>
            {
                _radius = Mathf.FloorToInt(x); Extract();
            });
            _directionSlider.onValueChanged.AddListener((x) =>
            {
                _direction = x; Extract();
            });
            _angleSlider.onValueChanged.AddListener((x) =>
            {
                _angle = x; Extract();
            });
            _rectangleSizeXSlider.onValueChanged.AddListener((x) =>
            {
                _size.x = Mathf.FloorToInt(x); Extract();
            });
            _rectangleSizeYSlider.onValueChanged.AddListener((x) =>
            {
                _size.y = Mathf.FloorToInt(x); Extract();
            });
            _demoTypeDropDown.onValueChanged.AddListener((x) =>
            {
                SetCurrentDemoType((DemoType)x); Extract();
            });
        }
        private void Start()
        {
            _grid.StartTileEnabled = false;
            Extract();
        }
        private void Update()
        {
            _hoveredTileLabel.text = _grid.ClampedHoveredTile == null ? "" : ("X:" + _grid.ClampedHoveredTile.X + " Y:" + _grid.ClampedHoveredTile.Y);
            if ((_grid.JustEnteredClampedTile && Input.GetMouseButton(1)) || (Input.GetMouseButtonDown(1) && _grid.ClampedHoveredTile != _centerTile))
            {
                _centerTile = _grid.ClampedHoveredTile;
                Extract();
            }
            else if ((_grid.JustEnteredClampedTile && Input.GetMouseButton(2)) || (Input.GetMouseButtonDown(2) && _grid.ClampedHoveredTile != _ledTile))
            {
                _ledTile = _grid.ClampedHoveredTile;
                RefreshCurrentLED();
            }
            else if (Input.GetMouseButtonUp(2) || _grid.ClampedHoveredTile == null)
            {
                _ledTile = null;
                TurnOffCurrentLed();
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
                case DemoType.EXTRACT_CIRCLE:
                    _radiusSlider.transform.parent.gameObject.SetActive(true);
                    _radiusSlider.minValue = 1;
                    _radiusSlider.SetValueWithoutNotify(Mathf.Max(_radiusSlider.value, 1));
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(false);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _radiusLabel.text = "Radius";
                    _circleLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.EXTRACT_CIRCLE_OUTLINE:
                    _radiusSlider.transform.parent.gameObject.SetActive(true);
                    _radiusSlider.minValue = 1;
                    _radiusSlider.SetValueWithoutNotify(Mathf.Max(_radiusSlider.value, 1));
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(false);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _radiusLabel.text = "Radius";
                    _circleOutlineLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.EXTRACT_RECTANGLE:
                    _radiusSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(true);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(true);
                    _directionSlider.transform.parent.gameObject.SetActive(false);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _rectangleLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.EXTRACT_RECTANGLE_OUTLINE:
                    _radiusSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(true);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(true);
                    _directionSlider.transform.parent.gameObject.SetActive(false);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _rectangleOutlineLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.EXTRACT_CONE:
                    _radiusSlider.transform.parent.gameObject.SetActive(true);
                    _radiusSlider.minValue = 0;
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(true);
                    _angleSlider.transform.parent.gameObject.SetActive(true);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _radiusLabel.text = "Length";
                    _coneLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.EXTRACT_LINE:
                    _radiusSlider.transform.parent.gameObject.SetActive(true);
                    _radiusSlider.minValue = 0;
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(true);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(true);
                    _favorVerticalToggle.gameObject.SetActive(true);
                    _radiusLabel.text = "Length";
                    _lineLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.NEIGHBOR:
                    _radiusSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(true);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _neiLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.NEIGHBORS_ORTHO:
                    _radiusSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(false);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _neiOrthoLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.NEIGHBORS_DIAGONALS:
                    _radiusSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(false);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _neiDiagoLED.transform.parent.gameObject.SetActive(true);
                    break;
                case DemoType.NEIGHBORS_ALL:
                    _radiusSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeXSlider.transform.parent.gameObject.SetActive(false);
                    _rectangleSizeYSlider.transform.parent.gameObject.SetActive(false);
                    _directionSlider.transform.parent.gameObject.SetActive(false);
                    _angleSlider.transform.parent.gameObject.SetActive(false);
                    _allowDiagonalsToggle.gameObject.SetActive(false);
                    _favorVerticalToggle.gameObject.SetActive(false);
                    _neiAnyLED.transform.parent.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        private void Extract()
        {
            switch (_demoType)
            {
                case DemoType.EXTRACT_CIRCLE:
                    ExtractCircle();
                    break;
                case DemoType.EXTRACT_CIRCLE_OUTLINE:
                    ExtractCircleOutline();
                    break;
                case DemoType.EXTRACT_RECTANGLE:
                    ExtractRectangle();
                    break;
                case DemoType.EXTRACT_RECTANGLE_OUTLINE:
                    ExtractRectangleOutline();
                    break;
                case DemoType.EXTRACT_CONE:
                    ExtractCone();
                    break;
                case DemoType.EXTRACT_LINE:
                    ExtractLine();
                    break;
                case DemoType.NEIGHBOR:
                    ExtractNeighbor();
                    break;
                case DemoType.NEIGHBORS_ORTHO:
                    ExtractOrthoNeighbors();
                    break;
                case DemoType.NEIGHBORS_DIAGONALS:
                    ExtractDiagonalsNeighbors();
                    break;
                case DemoType.NEIGHBORS_ALL:
                    ExtractNeighbors();
                    break;
                default:
                    break;
            }
        }
        private void RefreshCurrentLED()
        {
            switch (_demoType)
            {
                case DemoType.EXTRACT_CIRCLE:
                    _circleLED.color = Extraction.IsTileInACircle(_grid.Map, _ledTile, _centerTile, _radius) ? Color.green : Color.red;
                    break;
                case DemoType.EXTRACT_CIRCLE_OUTLINE:
                    _circleOutlineLED.color = Extraction.IsTileOnACircleOutline(_grid.Map, _ledTile, _centerTile, _radius) ? Color.green : Color.red;
                    break;
                case DemoType.EXTRACT_RECTANGLE:
                    _rectangleLED.color = Extraction.IsTileInARectangle(_grid.Map, _ledTile, _centerTile, new Vector2Int(_size.x, _size.y)) ? Color.green : Color.red;
                    break;
                case DemoType.EXTRACT_RECTANGLE_OUTLINE:
                    _rectangleOutlineLED.color = Extraction.IsTileOnARectangleOutline(_grid.Map, _ledTile, _centerTile, new Vector2Int(_size.x, _size.y)) ? Color.green : Color.red;
                    break;
                case DemoType.EXTRACT_CONE:
                    _coneLED.color = Extraction.IsTileInACone(_grid.Map, _ledTile, _centerTile, _radius, _angle, _direction) ? Color.green : Color.red;
                    break;
                case DemoType.EXTRACT_LINE:
                    _lineLED.color = Extraction.IsTileOnALine(_grid.Map, _ledTile, _centerTile, _radius, _direction, _allowDiagonals, _favorVertical) ? Color.green : Color.red;
                    break;
                case DemoType.NEIGHBOR:
                    _neiLED.color = Extraction.IsTileNeighbor(_ledTile, _centerTile, _direction, false) ? Color.green : Color.red;
                    break;
                case DemoType.NEIGHBORS_ORTHO:
                    _neiOrthoLED.color = Extraction.IsTileOrthogonalNeighbor(_ledTile, _centerTile) ? Color.green : Color.red;
                    break;
                case DemoType.NEIGHBORS_DIAGONALS:
                    _neiDiagoLED.color = Extraction.IsTileDiagonalNeighbor(_ledTile, _centerTile) ? Color.green : Color.red;
                    break;
                case DemoType.NEIGHBORS_ALL:
                    _neiAnyLED.color = Extraction.IsTileAnyNeighbor(_ledTile, _centerTile) ? Color.green : Color.red;
                    break;
                default:
                    break;
            }
        }
        private void ExtractCircle()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTilesInACircle(_grid.Map, _centerTile, _radius, false, false));
        }
        private void ExtractCircleOutline()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTilesOnACircleOutline(_grid.Map, _centerTile, _radius, false));
        }
        private void ExtractRectangle()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTilesInARectangle(_grid.Map, _centerTile, _size, false, false));
        }
        private void ExtractRectangleOutline()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTilesOnARectangleOutline(_grid.Map, _centerTile, _size, false));
        }
        private void ExtractCone()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTilesInACone(_grid.Map, _centerTile, _radius, _angle, _direction, false, false));
        }
        private void ExtractLine()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTilesOnALine(_grid.Map, _centerTile, _radius, _direction, _allowDiagonals, _favorVertical, false, false/*, false*/));
        }
        private void ExtractNeighbor()
        {
            Extraction.GetTileNeighbour(_grid.Map, _centerTile, _direction, out Tile neighbor, false);
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(new Tile[] { neighbor });
        }
        private void ExtractOrthoNeighbors()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTileOrthogonalsNeighbours(_grid.Map, _centerTile, false));
        }
        private void ExtractDiagonalsNeighbors()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTileDiagonalsNeighbours(_grid.Map, _centerTile, false));
        }
        private void ExtractNeighbors()
        {
            _grid.TintCenter(_centerTile);
            _grid.TintHighlightedTiles(Extraction.GetTileNeighbours(_grid.Map, _centerTile, false));
        }
        private void TurnOffCurrentLed()
        {
            switch (_demoType)
            {
                case DemoType.EXTRACT_CIRCLE:
                    _circleLED.color = Color.grey;
                    break;
                case DemoType.EXTRACT_CIRCLE_OUTLINE:
                    _circleOutlineLED.color = Color.grey;
                    break;
                case DemoType.EXTRACT_RECTANGLE:
                    _rectangleLED.color = Color.grey;
                    break;
                case DemoType.EXTRACT_RECTANGLE_OUTLINE:
                    _rectangleOutlineLED.color = Color.grey;
                    break;
                case DemoType.EXTRACT_CONE:
                    _coneLED.color = Color.grey;
                    break;
                case DemoType.EXTRACT_LINE:
                    _lineLED.color = Color.grey;
                    break;
                case DemoType.NEIGHBOR:
                    _neiLED.color = Color.grey;
                    break;
                case DemoType.NEIGHBORS_ORTHO:
                    _neiOrthoLED.color = Color.grey;
                    break;
                case DemoType.NEIGHBORS_DIAGONALS:
                    _neiDiagoLED.color = Color.grey;
                    break;
                case DemoType.NEIGHBORS_ALL:
                    _neiAnyLED.color = Color.grey;
                    break;
                default:
                    break;
            }
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