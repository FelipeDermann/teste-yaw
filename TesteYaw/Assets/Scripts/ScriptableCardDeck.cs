using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DeckCard
{
    public Card card;
    public int amount;
}

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/Deck", order = 1)]
public class ScriptableCardDeck : ScriptableObject
{
    public List<DeckCard> deckCards;
}
