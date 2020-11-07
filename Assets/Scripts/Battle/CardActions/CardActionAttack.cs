using UnityEngine;

public class CardActionAttack : CardActionBase
{
    float flySpeed = 0;

    public CardActionAttack(Card _card1, Card _card2, GameObject _interactiveObj, Vector3 _targetPos)
    {
        card1 = _card1;
        card2 = _card2;
        interactiveObj = _interactiveObj;
        targetPos = _targetPos;

        OnInit();
    }

    public override void OnInit()
    {
        BaseInit();
        interactiveObj.transform.position = card1.transform.position;
        interactiveObj.transform.LookAt(card2.transform);

        Unit unit = card1.GetUnitInstance().GetUnit();
        if (unit.unitAttackType == UnitAttackType.Melee)
            flySpeed = 6;
        else if (unit.unitAttackType == UnitAttackType.Range)
            flySpeed = 12;

        CardActionManager.instance.PrepareShootObject(ref interactiveObj, unit.unitAttackType);
        interactiveObj.GetComponent<AudioSource>().Play();
        interactiveObj.SetActive(true);
    }

    public override void Update()
    {
        interactiveObj.transform.position = Vector3.MoveTowards(interactiveObj.transform.position, card2.transform.position, flySpeed * Time.unscaledDeltaTime);

        if(IsActionFinished())
            InvokeOnFinishIfWasntFinished();
    }

    public override void OnFinish()
    {
        BattleController.instance.GetBattlefield().AttackCard(card1, card2);

        if(card1.Country.isPlayerCountry)
            BattleController.instance.HighlightAvailableCardMovement(card1);

        interactiveObj.SetActive(false);
    }
}