using UnityEngine;
using System.Collections.Generic;

public class Battlefield : MonoBehaviour
{
    [SerializeField] Transform battlefieldCenter;
    [SerializeField] BattleRow rowPrefab;
    [SerializeField] Card cardPrefab;
    BattleRow[] rows;

    BattleCardsContainer playerCards = new BattleCardsContainer();

    int rowCount;
    int rowWidth = 3;

    void Awake()
    {
        rowCount = Random.Range(6, 10);
        GenerateBattlefield(rowCount);
    }

    public List<Card> GetPlayerCards()
    {
        return playerCards.GetCards();
    }

    bool RemovePlayerCard(Card card)
    {
        return playerCards.RemoveCard(card);
    }


    void InitRows()
    {
        rows = new BattleRow[rowCount];
    }

    private void Start()
    {
        SpawnPlayerArmy();
        SpawnAIArmy();

        SpawnCityWall();
    }
    void SpawnCityWall()
    {
        if (BattleManager.battleInfo.GetDefender().HaveWalls)
        {
            Unit wallUnit = UnitsManager.instance.GetCityUnitByID(UnitID.WallCity);
            int rowIndex = 0;
            Card card = null;
            if (BattleManager.battleInfo.GetDefender().MyCountry.isPlayerCountry)
            {
                FitRowIndex(ref rowIndex, 2, true);
                card = SpawnUnitInRow(new UnitInstance(wallUnit, 1), rowIndex, BattleManager.GetPlayer().MyCountry);
            }
            else
            {
                FitRowIndex(ref rowIndex, 2, false);
                card = SpawnUnitInRow(new UnitInstance(wallUnit, 1), rowIndex, BattleManager.GetAI().MyCountry);
            }
            card.isThisCityWall = true;
        }
    }

    void GenerateBattlefield(int width)
    {
        InitRows();

        for (int i = 0; i < width; i++)
        {
            Vector3 targetPos = new Vector3((i * rowWidth) - (rowWidth * (width-1))/2f, -0.9f, 0) + battlefieldCenter.position;
            BattleRow battleRow = Instantiate(rowPrefab, targetPos, Quaternion.Euler(0, 0, 0));
            battleRow.transform.localScale = new Vector3(rowWidth, 1, 17.5f);
            battleRow.transform.SetParent(transform);
            battleRow.Setup(i);

            battleRow.GetComponent<BoxCollider>().enabled = false;
            battleRow.GetComponent<BoxCollider>().enabled = true;

            rows[i] = battleRow;
        }
    }

    private void SpawnPlayerArmy()
    {
        TargetableObject player = BattleManager.GetPlayer();

        if (player != null)
        {
            int rowIndex = 0;
            //Spawn on other side if blayer is defending
            FitRowIndex(ref rowIndex, 0, true);

            foreach (UnitInstance unit in player.GetArmy().GetUnits())
            {
                Card card = SpawnUnitInRow(unit, rowIndex, player.MyCountry);
                playerCards.AddCard(card);
            }
        }
    }

    private void SpawnAIArmy()
    {
        TargetableObject ai = BattleManager.GetAI();

        if (ai != null)
        {
            int rowIndex = 0;
            //Spawn on other side if player is defending
            FitRowIndex(ref rowIndex, 0, false);

            foreach (UnitInstance unit in ai.GetArmy().GetUnits())
            {
                Card card = SpawnUnitInRow(unit, rowIndex, ai.MyCountry);
                BattleAI.instance.AddCard(card);
            }
        }
    }

    void FitRowIndex(ref int index, int offset, bool isPlayer)
    {
        if (BattleManager.battleInfo.GetDefender().MyCountry != null)
        {
            if (BattleManager.battleInfo.GetDefender().MyCountry.isPlayerCountry == isPlayer)
                index = rowCount - (offset + 1);
        }
        else
        {
            index = offset;
        }
    }

    public Card SpawnUnitInRow(UnitInstance unitInstance, int rowIndex, Country unitCountry)
    {
        Card card = Instantiate(cardPrefab, BattleController.instance.GetWorldCanvas().transform);
        card.transform.position = rows[rowIndex].transform.position + new Vector3(0, 5, 0);
        card.transform.rotation = Quaternion.Euler(90, 0 , 0);
        card.Init(unitInstance, unitCountry);
        rows[rowIndex].AddCardToRow(card);

        return card;
    }

    public void AttackCard(Card attackingCard, Card attackedCard)
    {
        BattleRow attackedCardRow = attackedCard.currentRow;
        attackedCard.OnCardAttacked(attackingCard);
        attackingCard.OnCardAttack();

        if (attackedCard.ShouldBeDestroyed())
        {
            UnitAttackType attackingUnitAttackType = attackingCard.GetUnitInstance().GetUnit().unitAttackType;
            if (attackingUnitAttackType == UnitAttackType.Melee)
            {
                if (attackedCardRow.GetCardsFromCountryOtherThatArgument(attackingCard.Country).Count <= 0)
                    MoveCardToRow(attackingCard, attackedCardRow.RowIndex);
            }

            attackedCard.currentRow.RemoveCardFromRow(attackedCard);

            //Remove card from proper controller (AI or Player)
            if (attackedCard.Country.isPlayerCountry)
                RemovePlayerCard(attackedCard);
            else
                BattleAI.instance.RemoveCard(attackedCard);

            Destroy(attackedCard.gameObject);
        }
    }

    public BattleRow MoveCardToRow(Card card, int targetRowIndex)
    {
        int moveDistance = GetCardToRowDistance(card, targetRowIndex);
        card.OnCardMove(moveDistance);
        rows[card.currentRow.RowIndex].RemoveCardFromRow(card);
        rows[targetRowIndex].AddCardToRow(card);

        return rows[targetRowIndex];
    }

    public int GetCardToRowDistance(Card card, int targetRowIndex)
    {
        return Mathf.Abs(card.currentRow.RowIndex - targetRowIndex);
    }

    public bool IsRoadToRowClear(Card card, int targetRowIndex)
    {
        int cardRowIndex = card.currentRow.RowIndex;
        int smallerRowIndex;
        int biggerRowIndex;

        if (cardRowIndex < targetRowIndex)
        {
            smallerRowIndex = cardRowIndex;
            biggerRowIndex = targetRowIndex;
        }
        else
        {
            smallerRowIndex = targetRowIndex;
            biggerRowIndex = cardRowIndex;
        }

        for (int i = smallerRowIndex; i < biggerRowIndex; i++)
        {
            if (rows[i].GetCardsFromCountryOtherThatArgument(card.Country).Count > 0)
                return false;
        }

        return true;
    }

    public void HighlightMovableRowsForCard(Card card)
    {
        UnHighlightAllRows();

        int sourceIndex = card.currentRow.RowIndex;
        int maxMoveDist = card.GetUnitInstance().currentStamina;
        int maxAttackDist = card.GetUnitInstance().GetUnit().attackRange;
        if (maxMoveDist <= 0)
            maxAttackDist = 0;

        int moveStartIndex = Mathf.Clamp(sourceIndex - maxMoveDist, 0, rows.Length - 1);
        int moveEndIndex = Mathf.Clamp(sourceIndex + maxMoveDist, 0, rows.Length - 1);
        int attackEndIndex = Mathf.Clamp(sourceIndex + maxAttackDist, 0, rows.Length - 1);
        int attackStartIndex = Mathf.Clamp(sourceIndex - maxAttackDist, 0, rows.Length - 1);

        bool foundEnemyCard = false;
        //Left side
        for (int i = sourceIndex; i >= Mathf.Min(moveStartIndex, attackStartIndex); i--)
        {
            HighlightRowOnProperColor(i, card, maxMoveDist, ref foundEnemyCard);
        }

        foundEnemyCard = false;
        //Right side
        for (int i = sourceIndex; i <= Mathf.Max(moveEndIndex, attackEndIndex); i++)
        {
            HighlightRowOnProperColor(i, card, maxMoveDist, ref foundEnemyCard);
        }
    }
    void HighlightRowOnProperColor(int i, Card card, int maxMoveDistance, ref bool foundEnemyCard)
    {
        BattleRow targetRow = GetBattleRowByIndex(i);
        if (i != card.currentRow.RowIndex)
        {
            if (targetRow.GetCardsFromCountryOtherThatArgument(card.Country).Count > 0)
            {
                foundEnemyCard = true;
                rows[i].SetHighlightActive(true, BattleRow.HighlightType.Attack);
            }
            else if (!foundEnemyCard && Mathf.Abs(i - card.currentRow.RowIndex) <= maxMoveDistance)
                rows[i].SetHighlightActive(true, BattleRow.HighlightType.Normal);
        }
    }

    public void UnHighlightAllRows()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].SetHighlightActive(false, BattleRow.HighlightType.Normal);
        }
    }

    public List<Card> GetAllCardsInRows()
    {
        List<Card> result = new List<Card>();

        foreach (BattleRow battleRow in rows)
        {
            result.AddRange(battleRow.GetAllCardsInThisRow());
        }

        return result;
    }

    public List<Card> GetAllCardsInRowsFromCountry(Country country)
    {
        List<Card> result = new List<Card>();

        foreach (BattleRow battleRow in rows)
        {
            List<Card> cardsInRow = battleRow.GetAllCardsInThisRow();
            foreach (Card card in cardsInRow)
            {
                if (card.Country == country)
                    result.Add(card);
            }
        }

        return result;
    }

    public BattleRow GetBattleRowByIndex(int index)
    {
        if (index < 0 || index >= rows.Length)
        {
            Debug.LogError("You are trying to get element which is out of array range!");
            return null;
        }

        return rows[index];
    }
}