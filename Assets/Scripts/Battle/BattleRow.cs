using System.Collections.Generic;
using UnityEngine;

public class BattleRow : MonoBehaviour
{
    public enum HighlightType
    {
        Normal, Attack
    }

    List<Card> cardsInRow = new List<Card>();
    [SerializeField] GameObject highlight;
    [SerializeField] GameObject baseHighlight;

    [SerializeField] Color normalColor;
    [SerializeField] Color attackColor;

    readonly float cardMovementSpeed = 8;
    static float cardsOffset = 2.25f;

    int rowIndex = -1;
    public int RowIndex { get => rowIndex; }

    public void Setup(int index)
    {
        rowIndex = index;
    }

    private void Update()
    {
        UpdateCardsPosition();
    }


    public List<Card> GetAllCardsInThisRow()
    {
        return cardsInRow;
    }

    public List<Card> GetCardsFromCountryOtherThatArgument(Country country)
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < cardsInRow.Count; i++)
        {
            if (cardsInRow[i].Country != country)
                if(!cardsInRow[i].ShouldBeDestroyed())
                    cards.Add(cardsInRow[i]);
        }

        return cards;
    }

    void UpdateCardsPosition()
    {
        for (int i = 0; i < cardsInRow.Count; i++)
        {
            Vector3 targetPos = transform.position + new Vector3(0, 1, (cardsOffset * i) - (cardsOffset * (cardsInRow.Count - 1)) / 2f);
            Vector3 lerpedPos = Vector3.Lerp(cardsInRow[i].transform.position, targetPos, cardMovementSpeed * Time.unscaledDeltaTime);
            cardsInRow[i].transform.position = lerpedPos;

            //Refresh collider bounds
            cardsInRow[i].GetComponent<BoxCollider>().enabled = false;
            cardsInRow[i].GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void AddCardToRow(Card card)
    {
        card.currentRow = this;
        cardsInRow.Add(card);
    }

    public void RemoveCardFromRow(Card card)
    {
        card.currentRow = null;
        cardsInRow.Remove(card);
    }

    public void SetHighlightActive(bool value, HighlightType highlightType)
    {
        if (value)
        {
            ChangeHighlightColor(highlightType);
            highlight.SetActive(true);
            baseHighlight.SetActive(false);
        }
        else
        {
            highlight.SetActive(false);
            baseHighlight.SetActive(true);
        }
    }
    void ChangeHighlightColor(HighlightType highlightType)
    {
        switch (highlightType)
        {
            case HighlightType.Normal:
                highlight.GetComponent<Renderer>().material.color = normalColor;
                break;
            case HighlightType.Attack:
                highlight.GetComponent<Renderer>().material.color = attackColor;
                break;
            default:
                break;
        }
    }
}