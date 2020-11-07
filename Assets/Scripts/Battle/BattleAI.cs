using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BattleAI : MonoBehaviour
{

    BattleCardsContainer myCards = new BattleCardsContainer();

    public static BattleAI instance;

    void Awake()
    {
        instance = this;
    }

    public void AddCard(Card card)
    {
        myCards.AddCard(card);
    }

    public List<Card> GetCards()
    {
        return myCards.GetCards();
    }

    public bool RemoveCard(Card card)
    {
        return myCards.RemoveCard(card);
    }

    public void MakeTurn()
    {
        StartCoroutine(MakeTurnWait());
    }

    bool attackFinished = true;
    IEnumerator MakeTurnWait()
    {
        List<Card> cards = myCards.GetCards();
        for (int i = 0; i < cards.Count; i++)
        {
            Card currCard = cards[i];
            BattleRow nextRow = GetRowWithOffsetFromCard(currCard, 1);
            if (nextRow)
            {
                //Move(currCard);
                int maxRowsAvailableToMove = 0;
                for (int j = 0; j <= currCard.GetUnitInstance().currentStamina; j++)
                {
                    StartCoroutine(AttackIfCan(currCard));
                    yield return new WaitUntil(() => attackFinished);
                    attackFinished = false;

                    if (nextRow)
                    {
                        if (nextRow.GetCardsFromCountryOtherThatArgument(currCard.Country).Count <= 0)
                            maxRowsAvailableToMove++;
                        else
                            break;
                    }

                    BattleRow destinationRow = GetRowWithOffsetFromCard(currCard, 1);
                    BattleController.instance.MoveCardToOtherRow(currCard, destinationRow.RowIndex);

                    yield return new WaitUntil(() => BattleController.instance.IsCardActionFinished());
                }
            }

            yield return new WaitUntil(() => BattleController.instance.IsCardActionFinished());
        }
    }

    IEnumerator AttackIfCan(Card controlledCard)
    {
        int rowOffset = 1;
        BattleRow nextRow = GetRowWithOffsetFromCard(controlledCard, rowOffset);
        while (nextRow && controlledCard.GetUnitInstance().currentStamina >= 1 && controlledCard.GetUnitInstance().GetUnit().attackRange >= rowOffset)
        {
            List<Card> cardsToAttack = nextRow.GetCardsFromCountryOtherThatArgument(controlledCard.Country);
            Card cardToAttack = null;
            if (cardsToAttack.Count > 0)
            {
                cardToAttack = cardsToAttack[0];
                BattleController.instance.AttackCard(controlledCard, cardToAttack);
                yield return new WaitUntil(() => BattleController.instance.IsCardActionFinished());
            }
            else
            {
                rowOffset++;
                nextRow = GetRowWithOffsetFromCard(controlledCard, rowOffset);
            }
        }

        attackFinished = true;
    }

    BattleRow GetRowWithOffsetFromCard(Card card, int offset)
    {
        return BattleController.instance.GetBattleRowByIndexWithOffsetForAI(card.currentRow.RowIndex, offset);
    }
}
