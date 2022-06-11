using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ShuffleExtension
{
    public static void ShuffleCards<T>(this List<T> list, int shuffleAccuracy)
    {
        for (int i = 0; i < shuffleAccuracy; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            
            T temp = list[randomIndex];
            list[randomIndex] = list[0];
            list[0] = temp;
        }
    }
}

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    
    [Header("Main Attributes")]
    [SerializeField] private ScriptableCardDeck _deck;
    [SerializeField] private int maxCardsPerPlayer;
    [SerializeField] private ParticleSystem _boltExplosionEffect;

    [Header("Card Spawning")]
    [SerializeField] private float _distanceBetweenCards;
    [SerializeField] private Transform _playerCardPosTransform;
    [SerializeField] private Transform _playerDisposePosTransform;
    [SerializeField] private Transform _enemyCardPosTransform;
    [SerializeField] private Transform _enemyDisposePosTransform;

    public Transform PlayerDisposePosTransform => _playerDisposePosTransform;
    public Transform EnemyDisposePosTransform => _enemyDisposePosTransform;

    [Header("Card Lists")]
    [SerializeField] private List<Card> _playerCards;
    public List<Card> PlayerCards => _playerCards;
    
    [SerializeField] private List<Card> _enemyCards;
    public List<Card> EnemyCards => _enemyCards;

    [SerializeField] private List<Card> _cardsToDeal;

    private Card _playerLastCardDisposed;
    private Card _enemyLastCardDisposed;

    public Card PlayerLastCardDisposed => _playerLastCardDisposed;
    public Card EnemyLastCardDisposed => _enemyLastCardDisposed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public List<Card> GetEnemyCards()
    {
        return _enemyCards;
    }

    public void DealCards()
    {
        _cardsToDeal = new List<Card>();
        
        foreach (DeckCard card in _deck.deckCards)
        {
            for (int i = 0; i < card.amount; i++)
            {
                _cardsToDeal.Add(card.card);
            }
        }

        SpawnCards();
    }

    void SpawnCards()
    {
        SpawnPlayerCards();
    }

    void SpawnPlayerCards()
    {
        GameObject playerCardParent = new GameObject("Player Cards");
        playerCardParent.transform.SetParent(transform);
        
        List<Card> cards = _cardsToDeal;
        cards.ShuffleCards(10);
     
        SpawnPlayerCardsTween(cards, playerCardParent.transform, 0);
    }

    void SpawnPlayerCardsTween(List<Card> cards, Transform playerCardParent, int iteration)
    {
        List<Card> cardList = cards;
        Transform cardParent = playerCardParent;
        int currentIteration = iteration;
        
        Card newCard = Instantiate(cards[iteration], _playerCardPosTransform.position - 
             (Vector3.right * 10), Quaternion.identity, playerCardParent.transform);
        newCard.ToggleDetectableState(true);
        _playerCards.Add(newCard);

        float cardPosOffset = iteration * _distanceBetweenCards;
        newCard.transform.DOMove(_playerCardPosTransform.position + (Vector3.right * cardPosOffset),
            0.2f).OnComplete(() =>
        {
            if(currentIteration < maxCardsPerPlayer-1) SpawnPlayerCardsTween(cardList, cardParent, currentIteration + 1);
            else SpawnEnemyCards();
        });
    }

    void SpawnEnemyCards()
    {
        GameObject enemyCardParent = new GameObject("Enemy Cards");
        enemyCardParent.transform.SetParent(transform);
        
        List<Card> cards = _cardsToDeal;
        cards.ShuffleCards(10);
        
        SpawnEnemyCardsTween(cards, enemyCardParent.transform, 0);
    }
    
    void SpawnEnemyCardsTween(List<Card> cards, Transform enemyCardParent, int iteration)
    {
        List<Card> cardList = cards;
        Transform cardParent = enemyCardParent;
        int currentIteration = iteration;
        
        Card newCard = Instantiate(cards[iteration], _playerCardPosTransform.position + 
             (Vector3.right * 10), Quaternion.identity, enemyCardParent.transform);
        
        newCard.FlipCard();
        _enemyCards.Add(newCard);

        float cardPosOffset = iteration * _distanceBetweenCards;
        newCard.transform.DOMove(_enemyCardPosTransform.position + (Vector3.right * cardPosOffset),
            0.2f).OnComplete(() =>
        {
            if(currentIteration < maxCardsPerPlayer-1) SpawnEnemyCardsTween(cardList, cardParent, currentIteration + 1);
            else SpawnInitialCards();
        });
    }

    public void SpawnInitialCards()
    {
        InterfaceManager.Instance.ShowDrawingText();
        
        int randomPlayerCard = Random.Range(0, _cardsToDeal.Count);
        
        Card newPlayerCard = Instantiate(_cardsToDeal[randomPlayerCard], _playerDisposePosTransform.position + 
             (Vector3.right * 10), Quaternion.identity, transform);
        newPlayerCard.ChangeSortingOrder();
        newPlayerCard.transform.DOMove(_playerDisposePosTransform.position, 0.5f);
        
        int randomEnemyCard = Random.Range(0, _cardsToDeal.Count);
        
        Card newEnemyCard = Instantiate(_cardsToDeal[randomEnemyCard], _enemyDisposePosTransform.position - 
             (Vector3.right * 10), Quaternion.identity, transform);
        
        AIManager.Instance.ResetEnemyFirstPlay();
        newEnemyCard.ChangeSortingOrder();
        
        newEnemyCard.transform.DOMove(_enemyDisposePosTransform.position, 0.5f).OnComplete(() =>
        {
            InterfaceManager.Instance.ShowPoints();
            
            _playerLastCardDisposed = newPlayerCard;
            _enemyLastCardDisposed = newEnemyCard;
        
            //cards with a value of zero are worth 1 point on the drawing phase
            int playerInitialCardValue = newPlayerCard.GetValue() != 0 ? newPlayerCard.GetValue() : 1;
            int enemyInitialCardValue = newEnemyCard.GetValue() != 0 ? newEnemyCard.GetValue() : 1;
        
            GameManager.Instance.ChangePlayerPoints(playerInitialCardValue);
            GameManager.Instance.ChangeEnemyPoints(enemyInitialCardValue);
            
            //wait a second for better gameplay flow
            newEnemyCard.transform.DOMove(newEnemyCard.transform.position, 1).OnComplete(() =>
            {
                //Start with enemy turn if initial player card value is higher
                //or vice-versa if player card value is lower
                CurrentTurn turnToEnter = playerInitialCardValue > enemyInitialCardValue ? CurrentTurn.EnemyTurn
                    : CurrentTurn.PlayerTurn;
                GameManager.Instance.EnterTurn(turnToEnter);
            });
        });
    }

    public void EnemyPlayCard(Card chosenCard)
    {
        chosenCard.PlayCard();
        chosenCard.ChangeSortingOrder();

        _enemyLastCardDisposed = chosenCard;
        _enemyCards.Remove(chosenCard);
    }

    public void PlayerPlayCard(Card chosenCard)
    {
        chosenCard.PlayCard();
        chosenCard.ChangeSortingOrder();

        chosenCard.ToggleDetectableState(false);
        _playerLastCardDisposed = chosenCard;
        _playerCards.Remove(chosenCard);
    }

    public void PlayBoltEffect(Vector3 targetCardPos)
    {
        _boltExplosionEffect.transform.position = targetCardPos;
        _boltExplosionEffect.Play();
    }
}
