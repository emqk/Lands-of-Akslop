using UnityEngine;

public class CardActionMove : CardActionBase
{
    public CardActionMove(Card _card1, GameObject _interactiveObj, Vector3 _targetPos) 
    {
        card1 = _card1;
        card2 = null;
        interactiveObj = _interactiveObj;
        targetPos = _targetPos;

        OnInit();
        wasFinishInvoked = false;
    }

    public override void OnInit()
    {
        BaseInit();
    }

    public override void Update()
    {       
        if(IsActionFinished())
            InvokeOnFinishIfWasntFinished();
    }

    public override void OnFinish()
    {
    }
}