using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Card : MonoBehaviour
{
    protected UnitInstance unitInstance;
    [SerializeField] protected Image unitIcon;
    [SerializeField] protected Image unitCountryBackground;
    [SerializeField] protected Image selectionBackground;
    [SerializeField] protected TextMeshProUGUI currentUnitAmount;
    [SerializeField] protected TextMeshProUGUI currentUnitArmorAmount;

    public BattleRow currentRow;
    public bool isThisCityWall;

    Color defaultColor;

    public static readonly int attackStaminaCost = 1;

    protected Country country;
    public Country Country { get => country; }


    public virtual void Init(UnitInstance instance, Country _country)
    {
        unitInstance = instance;
        country = _country;
        unitIcon.sprite = unitInstance.GetUnit().icon;
        unitCountryBackground.color = country.GetCountryColor();
        defaultColor = unitIcon.color;
        RefreshOnCardUI();
    }

    public bool ShouldBeDestroyed()
    {
        return unitInstance.amount <= 0;
    }

    void RefreshOnCardUI()
    {
        currentUnitAmount.text = unitInstance.amount.ToString();
        currentUnitArmorAmount.text = unitInstance.currentArmor.ToString();
    }

    public UnitInstance GetUnitInstance()
    {
        return unitInstance;
    }

    public void Select()
    {
        selectionBackground.enabled = true;
        BattleController.instance.HighlightAvailableCardMovement(this);
        AudioManager.instance.ClickButton();
    }
    public virtual void Unselect()
    {
        selectionBackground.enabled = false;
    }

    public void OnCardMove(int moveDistance)
    {
        unitInstance.currentStamina -= moveDistance;
    }

    public void OnCardAttack()
    {
        unitInstance.currentStamina -= attackStaminaCost;
    }

    public void OnCardAttacked(Card attackingCard)
    {
        TakeDamage(attackingCard);
        RefreshOnCardUI();
        AudioManager.instance.TakeDamage();
    }

    IEnumerator GoBackToDefaultColor()
    {
        for (int i = 0; i < 3; i++)
            yield return new WaitForSeconds(0.02f);

        unitIcon.color = defaultColor;
        unitCountryBackground.color = country.GetCountryColor();
    }
    void TakeDamage(Card attackingCard)
    {
        Unit attackingUnit = attackingCard.GetUnitInstance().GetUnit();
        int unitSumHp = ((unitInstance.amount - 1) * unitInstance.GetUnit().hp) + unitInstance.currentHP;
        int damageToDeal = Mathf.Clamp(Random.Range(attackingUnit.damageRange.x, attackingUnit.damageRange.y + 1), attackingUnit.damageRange.x, attackingUnit.damageRange.y) * attackingCard.GetUnitInstance().amount;
        int armorLeft = unitInstance.currentArmor - damageToDeal;
        //Calculate armor
        if (armorLeft < 0)
        {
            damageToDeal -= unitInstance.currentArmor;
            if (damageToDeal < 0)
                damageToDeal = 0;

            armorLeft = 0;
        }
        else
        {
            damageToDeal = 0;
        }

        VisualInfoManager.instance.CreateVisualInfo("- " + damageToDeal + " HP!", transform.position + new Vector3(0, 1, 0), 2f, BattleController.instance.GetWorldCanvas());

        int hpLeft = unitSumHp - damageToDeal;
        int unitsLeft = Mathf.CeilToInt((float)hpLeft / (float)unitInstance.GetUnit().hp);
        int lastUnitHp = hpLeft % unitInstance.GetUnit().hp;

        if (lastUnitHp <= 0)
        {
            lastUnitHp = unitInstance.GetUnit().hp;
        }

        //Debug.Log("------Begin Attack------");
        //Debug.Log("maxHP: " + unitSumHp);
        //Debug.Log("dmgToDeal: " + damageToDeal);
        //Debug.Log("hpLeft: " + hpLeft);
        //Debug.Log("unitsLeft: " + unitsLeft);
        //Debug.Log("lastUnitHP: " + lastUnitHp);
        //Debug.Log("------End Attack------");

        unitInstance.amount = unitsLeft;
        unitInstance.currentHP = lastUnitHp;
        unitInstance.currentArmor = armorLeft;

        //Effect
        unitIcon.color = Color.red;
        unitCountryBackground.color = Color.red;
        StartCoroutine(GoBackToDefaultColor());
    }

    public void OnTurnEnd()
    {
        unitInstance.currentStamina = unitInstance.GetUnit().stamina;
    }

    private void OnDestroy()
    {
        if (isThisCityWall)
        {
            TargetableObject attacker = BattleManager.battleInfo.GetAttacker();
            TargetableObject defender = BattleManager.battleInfo.GetDefender();

            if (attacker.MyCountry == Country)
            {
                attacker.GetComponent<City>().DestroyWalls();
            }
            else if (defender.MyCountry == Country)
            {
                defender.GetComponent<City>().DestroyWalls();
            }
        }
    }
}