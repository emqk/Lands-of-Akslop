using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    [SerializeField] Canvas worldCanvas;
    [SerializeField] Battlefield battlefield;
    [SerializeField] BattleCamera battleCamera;
    [SerializeField] GameObject attackerColorObj;
    [SerializeField] GameObject defenderColorObj;
    [SerializeField] EndTurnButton endTurnButton;

    [SerializeField] GameObject attackBulletPrefab;

    public bool isBattleStarted = false;
    [SerializeField] TextMeshProUGUI currentPlayerTurnText;
    bool isAttackerTurn = true;

    CardActionBase currentCardAction;

    public static BattleController instance;


    private void Awake()
    {
        instance = this;
    }

    public Canvas GetWorldCanvas()
    {
        return worldCanvas;
    }

    public BattleCamera GetBattleCamera()
    {
        return battleCamera;
    }

    public Battlefield GetBattlefield()
    {
        return battlefield;
    }

    public EndTurnButton GetEndTurnButton()
    {
        return endTurnButton;
    }

    GameObject obj;
    private void Start()
    {
        TargetableObject attacker = BattleManager.battleInfo.GetAttacker();
        TargetableObject defender = BattleManager.battleInfo.GetDefender();
        Color noCountryColor = CountryManager.instance.NoCountryColor;

        attackerColorObj.GetComponent<Renderer>().material.SetColor("_Color", attacker.MyCountry != null ? attacker.MyCountry.GetCountryColor() : noCountryColor);
        defenderColorObj.GetComponent<Renderer>().material.SetColor("_Color", defender.MyCountry != null ? defender.MyCountry.GetCountryColor() : noCountryColor);

        obj = Instantiate(attackBulletPrefab, transform);
        obj.AddComponent<AudioSource>();
        obj.transform.localScale = new Vector3(10, 10, 10);
        currentCardAction = new CardActionMove(null, null, new Vector3());

        RefreshUI();
    }

    public void AfterStartBattleButtonClick()
    {
        TargetableObject attacker = BattleManager.battleInfo.GetAttacker();
        if (attacker.MyCountry.isPlayerCountry == false)
        {
            StartAITurn();
        }
    }

    private void Update()
    {
        if (currentCardAction != null)
        {
            if (currentCardAction.IsActionFinished() == false)
            {
                currentCardAction.Update();
            }
        }

        if (IsCardActionFinished() || !IsPlayerTurn())
        {
            endTurnButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            endTurnButton.GetComponent<Button>().interactable = false;
        }
    }


    public bool IsCardActionFinished()
    {
        return currentCardAction.IsActionFinished();
    }

    public bool IsCardActionNull()
    {
        if (currentCardAction == null || currentCardAction.IsTargetNull())
        {
            return true;
        }

        return false;
    }


    void WinBattle()
    {
        Debug.Log("Battle won!");
        BattleManager.ExitBattle(new BattleResult(true, true, true));
    }

    void LoseBattle()
    {
        Debug.Log("Battle lost!");
        BattleManager.ExitBattle(new BattleResult(false, true, true));
    }

    public void HighlightAvailableCardMovement(Card card)
    {
        battlefield.HighlightMovableRowsForCard(card);
    }


    public List<Card> GetPlayerCards()
    {
        return battlefield.GetPlayerCards();
    }

    public List<Card> GetAICards()
    {
        return BattleAI.instance.GetCards();
    }

    public int GetCardToRowDistance(Card card, int targetRowIndex)
    {
        return battlefield.GetCardToRowDistance(card, targetRowIndex);
    }

    public bool HaveCardEnoughStaminaToMove(Card card, int targetRowIndex)
    {
        return card.GetUnitInstance().currentStamina >= GetCardToRowDistance(card, targetRowIndex);
    }
    
    public bool CanAttack(Card card, int targetRowIndex)
    {
        int distance = GetCardToRowDistance(card, targetRowIndex);
        return card.GetUnitInstance().currentStamina > 0 && card.GetUnitInstance().GetUnit().attackRange >= distance;
    }

    public bool MoveCardToOtherRow(Card card, int targetRowIndex)
    {
        if (HaveCardEnoughStaminaToMove(card, targetRowIndex) && battlefield.IsRoadToRowClear(card, targetRowIndex))
        {
            BattleRow targetRow = battlefield.MoveCardToRow(card, targetRowIndex);
            currentCardAction = new CardActionMove(card, card.gameObject, new Vector3(targetRow.transform.position.x, card.transform.position.y, targetRow.transform.position.z));
            return true;
        }

        return false;
    }

    public void AttackCard(Card attackingCard, Card attackedCard)
    {
       currentCardAction = new CardActionAttack(attackingCard, attackedCard, obj, attackedCard.transform.position);
    }

    public BattleRow GetBattleRowByIndexWithOffsetForAI(int index, int offset)
    {
        //If AI is on right side, AI will move left
        if (BattleManager.battleInfo.GetAttacker().MyCountry.isPlayerCountry)
            offset = -Mathf.Abs(offset);

        return battlefield.GetBattleRowByIndex(index + offset);
    }

    public void OnBattleEnd()
    {
        battlefield.UnHighlightAllRows();

        BattleManager.RemoveAllUnitInstancesThatShouldBeDestroyed();

        BattleManager.ResetArmiesUnits();

        UIManager.instance.RefreshCurrentlySelectedWorldAgentArmyUI();
    }

    public bool IsPlayerTurn()
    {
        TargetableObject attacker = BattleManager.battleInfo.GetAttacker();
        TargetableObject defender = BattleManager.battleInfo.GetDefender();

        if (isAttackerTurn)
        {
            if (attacker.MyCountry != null)
            {
                if (attacker.MyCountry.isPlayerCountry)
                    return true;
            }
        }
        else
        {
            if (defender.MyCountry != null)
            {
                if (defender.MyCountry.isPlayerCountry)
                    return true;
            }
        }

        return false;
    }

    public void EndTurn()
    {
        if (ShouldBattleEnd())
        {
            EndBattle();
            return;
        }

        isAttackerTurn = !isAttackerTurn;

        if (isAttackerTurn)
            RefreshAllCardsFromCountry(BattleManager.battleInfo.GetAttacker().MyCountry);
        else
            RefreshAllCardsFromCountry(BattleManager.battleInfo.GetDefender().MyCountry);

        //AI
        if (IsPlayerTurn() == false)
        {
            StartAITurn();
        }
        ///

        BattleInteraction.instance.UnselectSelectedCard();
        battlefield.UnHighlightAllRows();
        RefreshUI();
    }
    void StartAITurn()
    {
        if (isBattleStarted)
        {
            BattleAI.instance.MakeTurn();
            EndTurn();
        }
    }

    void EndBattle()
    {
        TargetableObject winner = GetWinner();
        TargetableObject loser = GetLoser();

        Debug.Log("Winner: " + winner.MyCountry.GetCountryName());
        Debug.Log("Loser: " + loser.MyCountry.GetCountryName());

        if (GetAICards().Count <= 0 || GetPlayerCards().Count <= 0)
        {
            if (BattleManager.battleInfo.GetAttacker() == winner)
            {
                WinBattle();
            }
            else
            {
                LoseBattle();
            }
        }
    }
    TargetableObject GetWinner()
    {
        if (GetAICards().Count <= 0)
        {
            if (BattleManager.battleInfo.GetAttacker().MyCountry.isPlayerCountry)
                return BattleManager.battleInfo.GetAttacker();
            else
                return BattleManager.battleInfo.GetDefender();
        }
        else if (GetPlayerCards().Count <= 0)
        {
            if (BattleManager.battleInfo.GetAttacker().MyCountry.isPlayerCountry == false)
                return BattleManager.battleInfo.GetAttacker();
            else
                return BattleManager.battleInfo.GetDefender();
        }

        return null;
    }
    TargetableObject GetLoser()
    {
        TargetableObject winner = GetWinner();
        if (winner == BattleManager.battleInfo.GetAttacker())
            return BattleManager.battleInfo.GetDefender();
        else
            return BattleManager.battleInfo.GetAttacker();
    }
    bool ShouldBattleEnd()
    {
        return (GetPlayerCards().Count <= 0 || GetAICards().Count <= 0);
    }

    void RefreshAllCardsFromCountry(Country country)
    {
        List<Card> allCards = battlefield.GetAllCardsInRowsFromCountry(country);
        foreach (Card card in allCards)
        {
            card.OnTurnEnd();
        }
    }

    void RefreshUI()
    {
        bool isPlayerTurn = IsPlayerTurn();
        currentPlayerTurnText.text = isPlayerTurn == true ? "Your turn!" : "Enemy's turn";
    }

    public void StartBattle(GameObject startBattleUI)
    {
        isBattleStarted = true;
        startBattleUI.SetActive(false);
        AudioManager.instance.ClickButton();
    }
}