using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance;
    
    private GameInput _input;
    private Camera mainCamera;

    [SerializeField] private LayerMask cardMask;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        _input = new GameInput();
        ToggleInputAvailability(true);
        
        mainCamera = Camera.main;

        _input.Mouse.Click.started += ctx => Click();
    }

    public void ToggleInputAvailability(bool newState)
    {
        if (newState) _input.Enable();
        else _input.Disable();
    }

    void Click()
    {
        DetectCard();
    }
    
    void DetectCard()
    {
        Vector2 mousePos = _input.Mouse.Position.ReadValue<Vector2>();
        
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(
                new Vector3(mousePos.x, mousePos.y, 0)), 
            Vector2.zero, 100, cardMask);

        if (hit.collider != null)
        {
            Card cardDetected = hit.collider.gameObject.GetComponent<Card>();
            Debug.Log(cardDetected.name);

            if (!cardDetected.Detectable) return;
            
            if (GameManager.Instance.GetState() == CurrentTurn.PlayerTurn)
                CardManager.Instance.PlayerPlayCard(cardDetected);
        }
    }
}
