using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Difficulty
{
    Easy,
    Normal
}

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    private bool _firstRound;

    [SerializeField] private Difficulty currentDifficulty = Difficulty.Easy;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectCardToPlay()
    {
        if (currentDifficulty == Difficulty.Easy) RandomSelection();
        else ChooseCard();
    }

    public void ResetEnemyFirstPlay()
    {
        _firstRound = false;
    }

    void RandomSelection()
    {
        List<Card> availableCards = CardManager.Instance.GetEnemyCards();
        int randomCardIndex = Random.Range(0, availableCards.Count);
        Card chosenCard = availableCards[randomCardIndex];

        //Just make sure to play the right card after an initial round so that
        //the player doesn't just win automatically without even getting to play
        if (!_firstRound)
        {
            _firstRound = true;
            int neededPointsToContinue = GameManager.Instance.PlayerPoints - GameManager.Instance.EnemyPoints;

            foreach (Card card in availableCards)
            {
                if (card.GetValue() >= neededPointsToContinue) chosenCard = card;
            }
        }
        
        CardManager.Instance.EnemyPlayCard(chosenCard);
    }

    void ChooseCard()
    {
        List<Card> availableCards = CardManager.Instance.EnemyCards;

        int neededPointsToContinue = GameManager.Instance.PlayerPoints - GameManager.Instance.EnemyPoints;
        int randomCardIndex = Random.Range(0, availableCards.Count);
        Card chosenCard = availableCards[randomCardIndex];
        
        foreach (Card card in availableCards)
        {
            if (card.GetValue() >= neededPointsToContinue) chosenCard = card;
        }
        Debug.Log(neededPointsToContinue);
        if (chosenCard == null || neededPointsToContinue >= 4)
            foreach (Card card in availableCards)
            {
                if (card.gameObject.GetComponent<MirrorCard>() != null) chosenCard = card;
                if (card.gameObject.GetComponent<BoltCard>() != null)
                {
                    if (CardManager.Instance.PlayerLastCardDisposed.GetValue() >= 3) chosenCard = card;
                }
                if (card.gameObject.GetComponent<DoubleCard>() != null)
                {
                    if (GameManager.Instance.EnemyPoints >= neededPointsToContinue/2) chosenCard = card;
                }
            }

        CardManager.Instance.EnemyPlayCard(chosenCard);
    }
}
