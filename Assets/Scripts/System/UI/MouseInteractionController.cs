using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInteractionController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    // Cache
    private Dictionary<int, HexTileController> _hexTileColliderCache;

    // Mouse Selection memory
    HexTileSelectionCache _hexTileSelectionCache;

    [SerializeField] CameraController _cameraController;
    
    // Input
    [SerializeField] private PlayerInput _playerInput;
    private InputAction _selectionAction;

    private UIHexTilesNameDisplay _hexNameDisplay;

    // UI elements
    [SerializeField] TMP_InputField _radiousInputField;
    [SerializeField] TMP_Text UIHexTilesDisplayText;
    [SerializeField] TMP_Text UIPathFindErrorText;

    [SerializeField] GameObject UIresizeLoadingText;
    [SerializeField] GameObject UIinteractionPanel;


    public void EnableControllers()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        _hexNameDisplay = new UIHexTilesNameDisplay(UIHexTilesDisplayText);

        _hexTileColliderCache = new Dictionary<int, HexTileController>();
        _hexTileSelectionCache = new HexTileSelectionCache(10, _hexNameDisplay);

        if(_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        _selectionAction = _playerInput.actions["selection"];
        _selectionAction.performed += _selectionAction_performed;

        _cameraController.EnableControllers();
       
    }

    private void _selectionAction_performed(InputAction.CallbackContext obj)
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit))
        {
            if (hit.transform.CompareTag("HexTile"))
            {
                _hexTileSelectionCache.SelectHexTile(GetHexTile(hit.transform));
            }
        }
    }

    public void UIDrawPathDialogue()
    {
         ClearCache();
        _hexTileSelectionCache.maximumSelectable = 2;
    }
    public void FindPathInGrid()
    {
        if(_hexTileSelectionCache.selectedHexTileControllers.Count >= 2)
        {
            CubeCoordinate coordinateA = _hexTileSelectionCache.selectedHexTileControllers.Dequeue().cubeCoordinate;
            CubeCoordinate coordinateB = _hexTileSelectionCache.selectedHexTileControllers.Dequeue().cubeCoordinate;

            List<CubeCoordinate> coordinateList = CubeUtilities.AStarCubeNavigaction(coordinateA, coordinateB);

            _hexTileSelectionCache.ClearCache();

            _hexNameDisplay.DisplayHexTile(coordinateList);


            // Update UI
            if (coordinateList.Count > 0)
            {
                CubeUtilities.DrawHexTilePath(coordinateList);
                foreach (CubeCoordinate cubeCoordinate in coordinateList)
                {
                    _hexTileSelectionCache.outlinedTilesCache.Add(HexGridCubeLayout.Instance.GetTileFromCoordinate(cubeCoordinate));
                }

                // Deactivate UI
                UIPathFindErrorText.gameObject.SetActive(false);
            }
            else
            {
                UIPathFindErrorText.gameObject.SetActive(true);
            }
        }
    }

    public void RerollHexagon()
    {
        foreach (HexTileController selectedTile in _hexTileSelectionCache.selectedHexTileControllers)
        {
            HexGridCubeLayout.Instance.RerollHexagonTile(selectedTile.cubeCoordinate);
        }
    }
    private HexTileController GetHexTile(Transform raycastHitTransform)
    {
        int instanceId = raycastHitTransform.gameObject.GetInstanceID();
        if (!_hexTileColliderCache.ContainsKey(instanceId))
        {
            HexTileController hexTile = raycastHitTransform.gameObject.GetComponent<HexTileController>();
            _hexTileColliderCache.Add(instanceId, hexTile);
        }
        return _hexTileColliderCache[instanceId];
    }

    public void ChangeRadious()
    {
        StartCoroutine(ResizeCorutine());
    }
    private IEnumerator ResizeCorutine()
    {
        UIresizeLoadingText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        HexGridCubeLayout.Instance.UpdateGridSize(int.Parse(_radiousInputField.text));
        yield return new WaitForEndOfFrame();
        UIresizeLoadingText.SetActive(false);
    }

    public void ClearCache()
    {
        _hexTileSelectionCache.ClearCache();
        _hexNameDisplay.ClearDisplay();
    }

    public struct UIHexTilesNameDisplay
    {
        private TMP_Text _UItextAsset;
        private List<CubeCoordinate> _selectedCubeCoordinates;

        public UIHexTilesNameDisplay(TMP_Text textAsset)
        {
            _UItextAsset = textAsset;
            _selectedCubeCoordinates = new List<CubeCoordinate>();
        }
        public void DisplayHexTile(CubeCoordinate cubeCoordinate)
        {
            _selectedCubeCoordinates.Add(cubeCoordinate);
            RenderizeHexTilesNames();
        }
        public void DisplayHexTile(List<CubeCoordinate> cubeCoordinates)
        {
            _selectedCubeCoordinates.AddRange(cubeCoordinates);
            RenderizeHexTilesNames();
        }
        public void RemoveHexTile(CubeCoordinate cubeCoordinate)
        {
            _selectedCubeCoordinates.Remove(cubeCoordinate);
            RenderizeHexTilesNames();
        }
        public void ClearDisplay()
        {
            _UItextAsset.text = "";
            _selectedCubeCoordinates = new List<CubeCoordinate>();
        }
        private void RenderizeHexTilesNames()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CubeCoordinate cubeCoordinate in _selectedCubeCoordinates)
            {
                sb.Append("\t");
                sb.Append(string.Format("{0}", cubeCoordinate.ToString()));
                sb.Append(System.Environment.NewLine);
            }
            _UItextAsset.text = sb.ToString();
        }
    }

    private struct HexTileSelectionCache
    {
        public int maximumSelectable;
        public Queue<HexTileController> selectedHexTileControllers;
        public List<HexTileController> outlinedTilesCache;
        private UIHexTilesNameDisplay _hexTilesDisplayReference;

        public HexTileSelectionCache(int selectable, UIHexTilesNameDisplay hexDisplay)
        {
            selectedHexTileControllers = new Queue<HexTileController>();
            maximumSelectable = selectable; // Default
            _hexTilesDisplayReference = hexDisplay;
            outlinedTilesCache = new List<HexTileController>();
        }

        private bool ContainsHexTile(HexTileController hexTileController)
        {
            return selectedHexTileControllers.Contains(hexTileController);
        }
        public void SelectHexTile(HexTileController hexTileController)
        {

            // Add to cache for clear
            outlinedTilesCache.Add(hexTileController);


            // If already selected, deselect
            if (ContainsHexTile(hexTileController))
            {
                DeselectHexTile(hexTileController);
                return;
            }

            // Dispay hex tile name
            _hexTilesDisplayReference.DisplayHexTile(hexTileController.cubeCoordinate);

            // Restriction of selection cap, pop the oldest
            if (selectedHexTileControllers.Count >= maximumSelectable)
            {
                HexTileController oldestTile = selectedHexTileControllers.Dequeue();
                oldestTile.HexTileInteraction(CubeUtilities.HexTileStates.enabled);
                _hexTilesDisplayReference.RemoveHexTile(oldestTile.cubeCoordinate);
            }
            // Activate outline
            hexTileController.HexTileInteraction(CubeUtilities.HexTileStates.selected);
            selectedHexTileControllers.Enqueue(hexTileController);
        }

        public void DeselectHexTile(HexTileController hexTileController)
        {
            // Delete from display hex tile name
            _hexTilesDisplayReference.RemoveHexTile(hexTileController.cubeCoordinate);

            hexTileController.HexTileInteraction(CubeUtilities.HexTileStates.enabled);
            selectedHexTileControllers = new Queue<HexTileController>(selectedHexTileControllers.Where(s => s != hexTileController));
        }

        public void DisableHexTile(HexTileController hexTileController)
        {
            // Shift to deselect
            hexTileController.NavigableTile = !hexTileController.NavigableTile;
        }

        public void ClearCache()
        {
            foreach (HexTileController cacheHexTile in outlinedTilesCache)
            {
                cacheHexTile.HexTileInteraction(CubeUtilities.HexTileStates.enabled);
            }
            outlinedTilesCache = new List<HexTileController>();
            maximumSelectable = 10;
            selectedHexTileControllers.Clear();
            _hexTilesDisplayReference.ClearDisplay();
        }

    }
}
