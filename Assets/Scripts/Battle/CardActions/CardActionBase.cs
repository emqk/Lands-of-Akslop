using UnityEngine;

public abstract class CardActionBase
{
    protected Card card1 = null;
    protected Card card2 = null;

    protected GameObject interactiveObj = null;
    protected Vector3 targetPos;

    protected bool wasFinishInvoked = false;

    public abstract void OnInit();
    public abstract void Update();
    public abstract void OnFinish();

    protected void BaseInit()
    {
        wasFinishInvoked = false;
    }

    protected void InvokeOnFinishIfWasntFinished()
    {
        if (!wasFinishInvoked)
        {
            OnFinish();
            wasFinishInvoked = true;
        }
    }

    public bool IsActionFinished()
    {
        if (interactiveObj == null)
        {
            InvokeOnFinishIfWasntFinished();
            return true;
        }

        if (Mathf.Abs(interactiveObj.transform.position.x - targetPos.x) < 0.5f)
        {
            InvokeOnFinishIfWasntFinished();
            return true;
        }

        return false;
    }

    public bool IsTargetNull()
    {
        return interactiveObj == null;
    }
}