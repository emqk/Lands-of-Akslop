using System.Collections.Generic;
using UnityEngine;

public class BattleCardsContainer
{
    List<Card> cards = new List<Card>();

    public List<Card> GetCards()
    {
        return cards;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public bool RemoveCard(Card card)
    {
        return cards.Remove(card);
    }
}