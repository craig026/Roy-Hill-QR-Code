using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class GameplayManager : MonoBehaviour
{
    [SerializeField]
    public GameObject _placedPrefab;
    private GameObject _placedObject;
    public GameObject PlacedObject { get { return _placedObject; } set { _placedObject = value; } }

    public GameObject placementIndicator;
    public TextMeshProUGUI _instructionText;

    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _arPlaneManager;

    private int _numberOfObjectsAllowed = 1;
    private int _currentNumberOfObjects = 0;

    private Vector3 newPos;
    private Quaternion newRot;
    private Ease m_TweenEase = Ease.OutBounce;
    const float k_TweenTime = 0.8f;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    public List<ARRaycastHit> Hits { get { return _hits; } set { _hits = value; } }

    private bool ReadyToPlaceObject = false;
    private bool ObjectIsPlaced = false;

    private GameplayState _gameplayState;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        _arRaycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
        _arPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();

        SwitchGameplayState(GameplayState.DeviceScanning);
    }

    // Update is called once per frame
    void Update()
    {
        

#if UNITY_EDITOR
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceObjectEditor();
        }*/
#endif

        if (!ObjectIsPlaced)
        {
            UpdatePlacementPose();
        }

        // If user touches screen
        if (EventSystem.current.currentSelectedGameObject == null && ReadyToPlaceObject)
        {
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

            if (activeTouches.Count > 0 && activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                // Instantiate prefab
                //PlaceObject();
                MoveObject();

                // Deactivate placement indicator
                //placementIndicator.SetActive(false);
            }
        }
    }

    /*private void PlaceObject()
    {
        if (_placedPrefab != null && _currentNumberOfObjects < _numberOfObjectsAllowed)
        {
            // Instantiate prefab at user touch position and rotation
            _placedObject = Instantiate(_placedPrefab, _hits[0].pose.position, _hits[0].pose.rotation);

            _currentNumberOfObjects++;
            SwitchGameplayState(GameplayState.AdjustPosition);
        }
    }*/

    private void UpdatePlacementPose()
    {
        Vector2 screenCentre = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));           // Raycast origin from device screen centre

        TrackableType trackableType = TrackableType.Planes;                                             // Track flat surface planes

        if (_arRaycastManager.Raycast(screenCentre, _hits, trackableType))                                // Check if raycast hits a surface
        {
            if (placementIndicator != null)                                                              // If placement indicator has not been destroyed
            {
                placementIndicator.SetActive(true);                                                                     // Show placement indicator
                placementIndicator.transform.SetPositionAndRotation(_hits[0].pose.position, _hits[0].pose.rotation);      // Show placement indicator at user touch position and rotation
                if (!ObjectIsPlaced)
                {
                    SwitchGameplayState(GameplayState.TapToPlace);
                }
            }
        }
    }

    private void MoveObject()
    {
        if (_currentNumberOfObjects < _numberOfObjectsAllowed)
        {
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            // If user touches screen
            if (activeTouches.Count > 0 && activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                //_arSessionOrigin.MakeContentAppearAt(PlacedPrefab.transform, _hits[0].pose.position);
                newPos = _placedPrefab.transform.position;
                newPos = _hits[0].pose.position;
                _placedPrefab.transform.position = newPos;
                newRot = _hits[0].pose.rotation;
                _placedPrefab.transform.rotation = newRot;
                ShowARTerrain();
                //_placedObjectRenderer = _placedObject.GetComponentInChildren<MeshRenderer>();
                //placementIndicator.SetActive(false);
                //placementIndicator.GetComponent<MeshRenderer>().enabled = false;
                //_arPlaneManager.enabled = false;
                SwitchGameplayState(GameplayState.AdjustPosition);
                _currentNumberOfObjects++;
            }
        }
    }

    public void ShowARTerrain()
    {
        _placedPrefab.transform.localScale = Vector3.zero;
        _placedPrefab.SetActive(true);
        _placedPrefab.transform.DOScale(Vector3.one, k_TweenTime).SetEase(m_TweenEase);
        //Invoke("EnableARScaleInteractable", 0.9f);
    }

    public void PlaceObjectEditor()
    {
        Vector3 newPos = _placedPrefab.transform.position;
        newPos = new Vector3(0, 0, 0);
        _placedPrefab.transform.position = newPos;
        ShowARTerrain();
        SwitchGameplayState(GameplayState.AdjustPosition);
    }

    public enum GameplayState
    {
        DeviceScanning,
        TapToPlace,
        AdjustPosition
    }

    private void GameplayStateChanged()
    {
        switch (_gameplayState)
        {
            case GameplayState.DeviceScanning:
                _instructionText.text = "Point your device at the floor and move it slowly in a circle.";
                _instructionText.gameObject.SetActive(true);
                break;
            case GameplayState.TapToPlace:
                ReadyToPlaceObject = true;
                _instructionText.text = "Align the crosshair with the marker on the ground and then tap to place the drain.";
                break;
            case GameplayState.AdjustPosition:
                ObjectIsPlaced = true;
                placementIndicator.SetActive(false);
                _instructionText.text = "Move or rotate to adjust positioning as required.";
                break;
        } 
    }

    public void SwitchGameplayState(GameplayState toSwitch)
    {
        _gameplayState = toSwitch;

        GameplayStateChanged();
    }
    
}