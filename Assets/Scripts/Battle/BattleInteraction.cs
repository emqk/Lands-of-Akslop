using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInteraction : MonoBehaviour
{
    Card selectedCard = null;

    public static BattleInteraction instance;


    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (BattleController.instance.IsPlayerTurn() && BattleController.instance.isBattleStarted)
        {
            RaycastHit hit = new RaycastHit();
            if (Input.GetButtonDown("Fire1") && !BattleController.instance.GetEndTurnButton().IsCursorOverMe() && BattleController.instance.IsCardActionFinished())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    SelectCard(hit);

                    if (selectedCard && (BattleController.instance.IsCardActionFinished() || BattleController.instance.IsCardActionNull()))
                    {
                        if (hit.collider.GetComponent<BattleRow>())
                        {
                            List<Card> otherCards = hit.collider.GetComponent<BattleRow>().GetCardsFromCountryOtherThatArgument(selectedCard.Country);
                            if (otherCards.Count == 0)
                            {
                                BattleController.instance.MoveCardToOtherRow(selectedCard, hit.collider.GetComponent<BattleRow>().RowIndex);
                            }
                        }
                        else if (hit.collider.GetComponent<Card>())
                        {
                            Card hittedCard = hit.collider.GetComponent<Card>();
                            if (hittedCard.Country.isPlayerCountry == false)
                            {
                                if (BattleController.instance.CanAttack(selectedCard, hittedCard.currentRow.RowIndex))
                                {
                                    BattleController.instance.AttackCard(selectedCard, hittedCard);
                                }
                            }
                        }

                        //Highlight proper rows when mouse clicked
                        BattleController.instance.HighlightAvailableCardMovement(selectedCard);
                    }
                }
            }
        }
        else
        {
            CursorManager.instance.SetCursorTexture(CursorManager.CursorType.defaultCursor);
        }
    }

    void SelectCard(RaycastHit hit)
    {
        if (hit.collider.GetComponent<Card>())
        {
            if (hit.collider.GetComponent<Card>().Country.isPlayerCountry)
            {
                if (selectedCard)
                    selectedCard.Unselect();

                selectedCard = hit.collider.GetComponent<Card>();
                selectedCard.Select();
            }
        }
    }

    public void UnselectSelectedCard()
    {
        if (selectedCard)
        {
            selectedCard.Unselect();
            selectedCard = null;
        }
    }
}