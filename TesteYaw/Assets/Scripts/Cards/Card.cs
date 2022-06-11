using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum CardSide
{
    FaceUp,
    FaceDown
}

[SelectionBase]
public class Card : MonoBehaviour
{
    [SerializeField] private int _cardValue; 
    private CardSide _cardSide = CardSide.FaceUp;
    
    private bool _detectable = false;
    public bool Detectable => _detectable;

    [SerializeField] protected bool isASpecialCard = false;
    public bool IsASpecialCard => isASpecialCard;

    private bool bolted = false;
    public bool Bolted => bolted;

    [Header("Sprites")] 
    [SerializeField] private SpriteRenderer _backSprite;
    [SerializeField] private SpriteRenderer _frontSprite;
    [SerializeField] private SpriteRenderer _infoSprite;
    private SpriteRenderer _symbolSprite;
    
    public int GetValue()
    {
        return _cardValue;
    }

    public void PlayCard()
    {
        Debug.Log("Base card class");
        CurrentTurn currentTurn = GameManager.Instance.GetState();
        Vector3 destination;

        if (currentTurn == CurrentTurn.PlayerTurn) destination = CardManager.Instance.PlayerDisposePosTransform.position;
        else
        {
            FlipCard();
            destination = CardManager.Instance.EnemyDisposePosTransform.position;
        }
        
        gameObject.transform.DOMove(destination, 1).OnComplete(() =>
        {
            CardEffect(currentTurn);
        });
    }

    public virtual void CardEffect(CurrentTurn currentTurn)
    {
        if(currentTurn == CurrentTurn.PlayerTurn) GameManager.Instance.ChangePlayerPoints(_cardValue);
        else GameManager.Instance.ChangeEnemyPoints(_cardValue);

        ChangeTurn(currentTurn);
    }

    public void ChangeTurn(CurrentTurn currentTurn)
    {
        //Signals game manager to change the turn 
        CurrentTurn nextTurn = currentTurn == CurrentTurn.PlayerTurn ? 
            CurrentTurn.EnemyTurn : CurrentTurn.PlayerTurn;

        GameManager.Instance.EnterTurn(nextTurn);
    }

    public void FlipCard()
    {
        if (_symbolSprite == null) _symbolSprite = GetComponentInChildren<CardSymbol>().GetSymbolSprite();
        
        if (_cardSide == CardSide.FaceUp) FlipDown(); 
        else FlipUp();
    }

    void FlipUp()
    {
        transform.DORotate(Vector3.zero, 0.5f);
        _symbolSprite.sortingOrder = 1;
        _cardSide = CardSide.FaceUp;
    }

    void FlipDown()
    {
        transform.eulerAngles = new Vector3(0,180,0);
        _symbolSprite.sortingOrder = 0;
        _cardSide = CardSide.FaceDown;
    }

    public void ChangeSortingOrder()
    {
        if (_symbolSprite == null) _symbolSprite = GetComponentInChildren<CardSymbol>().GetSymbolSprite();

        int currentRound = GameManager.Instance.GetRoundNumber();
        
        _backSprite.sortingOrder += 3 + currentRound;
        _frontSprite.sortingOrder += 3 + currentRound;
        _infoSprite.sortingOrder += 3 + currentRound;
        _symbolSprite.sortingOrder += 3 + currentRound;
    }

    public bool CheckIfDetectable()
    {
        return _detectable;
    }

    public void ToggleDetectableState(bool newState)
    {
        _detectable = newState;
    }
}
