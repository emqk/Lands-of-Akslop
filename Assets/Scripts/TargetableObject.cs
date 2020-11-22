using System.Collections;
using UnityEngine;

public abstract class TargetableObject : MonoBehaviour
{

    Country myCountry = null;
    public Country MyCountry
    {
        get
        {
            return myCountry;
        }
        set
        {
            Debug.Log("Now this object is in a new country!");
            myCountry = value;
        }
    }


    protected WorldAgent worldAgentDock;
    [SerializeField] protected Transform entrance;
    public Vector3 showArmyOffsetUI = new Vector3(0, 6f, 0);


    protected bool haveWalls = false;
    public bool HaveWalls { get => haveWalls; }


    public abstract void OnThisObjectReached(WorldAgent enteredAgent);
    public abstract void OnBattleWon(TargetableObject attacker);
    public abstract void OnBattleLost(TargetableObject attacker);
    public abstract void ChangeRelationsAfterBattle(Country attackerCountry);

    protected void OnBattleWonBase()
    {
        GameStatus.CheckGameStatus();
    }

    public abstract Army GetArmy();

    public Vector3 GetEntrancePos()
    {
        return entrance.position;
    }

    public WorldAgent GetWorldActorDockedInObject()
    {
        return worldAgentDock;
    }

    WorldAgent enteringWorldAgentGlobal = null;
    protected void EnterThisObject(WorldAgent enteringWorldAgent)
    {
        enteringWorldAgentGlobal = enteringWorldAgent;

        if (enteringWorldAgent.myCountry != myCountry)
        {
            if (BattleManager.isDuringBattle == false && !RTSCamera.IsZoomingOnBattleStart)
            {
                if (enteringWorldAgent.MyCountry.isPlayerCountry || myCountry.isPlayerCountry)
                {
                    RTSCamera.instance.LookAtTransform(transform.position);
                    StartingBattleUI.instance.Open();

                    StartCoroutine(StartBattleWhenCameraOnTarget());
                    RTSCamera.StartZoomBattleStart();
                }
                else
                {
                    StartBattle();
                }
            }
        }
        else
        {
            worldAgentDock = enteringWorldAgent;
            enteringWorldAgent.DockTo(this);
        }
    }

    IEnumerator StartBattleWhenCameraOnTarget()
    {
        yield return new WaitUntil( () => RTSCamera.instance.GetDistanceToTarget() < 1f );
        TimerManager.instance.AddTimer(new TimerUnscaled(StartBattle, 1f));
    }

    void StartBattle()
    {
        bool wasThisBattleAlreadyStarted = BattleManager.WasThisBattleAlreadyStarted(enteringWorldAgentGlobal, this);

        if (wasThisBattleAlreadyStarted == false)
        {
            if (enteringWorldAgentGlobal.MyCountry.isPlayerCountry || (MyCountry != null ? MyCountry.isPlayerCountry : false))
            {
                BattleManager.StartBattle(enteringWorldAgentGlobal, this);
            }
            else
            {
                BattleManager.QuickBattle(enteringWorldAgentGlobal, this);
            }
        }

        StartingBattleUI.instance.Close();
        RTSCamera.StopZoomBattleStart();
    }

    public void ExitThisObject(WorldAgent worldAgent)
    {
        if (worldAgentDock == worldAgent)
        {
            //worldAgent.transform.position = entrance.position;
            //worldAgent.GetComponent<NavMeshAgent>().enabled = true;
            worldAgent.Undock();
            worldAgentDock = null;
            Debug.Log("Agent exit this city", worldAgent);
        }
    }
}
