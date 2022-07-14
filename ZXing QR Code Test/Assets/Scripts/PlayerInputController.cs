using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private GameplayManager _gameplayManager;

    private bool placeObjectEditor;

    private void Update()
    {
        if (placeObjectEditor)
        {
            _gameplayManager.PlaceObjectEditor();
        }
    }

    public void OnPlaceObjectEditor(InputValue inputValue)
    {
        placeObjectEditor = inputValue.isPressed;
    }
}
