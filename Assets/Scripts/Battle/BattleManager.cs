using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleResult battleResult = new BattleResult(false, false, true);
    public static BattleInfo battleInfo = new BattleInfo();

    static List<BattleInfo> thisFrameBattles = new List<BattleInfo>();

    public static bool isDuringBattle = false;

    private void Awake()
    {
        //Reset because battleResult is static, so there can be data from previous playthrough
        battleResult.Reset();
    }

    private void LateUpdate()
    {
        thisFrameBattles.Clear();
    }

    public static void StartBattle(TargetableObject attacker, TargetableObject defender)
    {
        Debug.Log("Starting battle!");
        battleInfo.Setup(attacker, defender);

        TacticalMode.instance.Disable();
        //TimeManager.SetPauseTime();
        SceneUtils.instance.LoadBattleSceneAdditive();
        SceneUtils.instance.EnterBattleScene();

        Debug.LogError("Changing fog start/end by code!");
        RenderSettings.fogStartDistance = 25;
        RenderSettings.fogEndDistance = 40;

        battleResult.isStarted = true;
        isDuringBattle = true;
    }

    public static void EndBattleImmediate()
    {
        TargetableObject attacker = battleInfo.GetAttacker();
        TargetableObject defender = battleInfo.GetDefender();
        if (battleResult.victory)
        {
            if (attacker.MyCountry != defender.MyCountry)
            {
                defender.OnBattleWon(attacker);
            }
        }
        else
        {
            defender.OnBattleLost(attacker);
        }

        //FinishBattle();
    }

    public static void ExitBattle(BattleResult endBattleResult)
    {

        battleResult = endBattleResult;
        EndBattleImmediate();

        SceneUtils.instance.UnloadBattleScene();
        SceneUtils.instance.ExitBattleScene();

        BattleController.instance.OnBattleEnd();

        Debug.LogError("Changing fog start/end by code!");
        RenderSettings.fogStartDistance = 55;
        RenderSettings.fogEndDistance = 350;

        battleInfo.Clear();
        FinishBattle();
    }

    public static bool WasThisBattleAlreadyStarted(TargetableObject attacker, TargetableObject defender)
    {
        foreach (BattleInfo item in thisFrameBattles)
        {
            if (item != null)
            {
                if (item.GetAttacker().GetInstanceID() == attacker.GetInstanceID() && item.GetDefender().GetInstanceID() == defender.GetInstanceID()
                    || item.GetAttacker().GetInstanceID() == defender.GetInstanceID() && item.GetDefender().GetInstanceID() == attacker.GetInstanceID())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void QuickBattle(TargetableObject attacker, TargetableObject defender)
    {
        Debug.Log("Starting quick battle by " + attacker.name, attacker.gameObject);

        BattleInfo bInfo = new BattleInfo();
        bInfo.Setup(attacker, defender);
        thisFrameBattles.Add(bInfo);

        battleInfo.Setup(attacker, defender);

        battleResult.isStarted = true;

        ///////////// BATTLE END /////////////

        bool attackerWin = false;

        float attackerStrength = attacker.GetArmy().GetArmyStrength();
        float defenderStrength = defender.GetArmy().GetArmyStrength();
        float result = (Mathf.Max(attackerStrength, defenderStrength) - Mathf.Min(defenderStrength, attackerStrength)) / Mathf.Max(attackerStrength, defenderStrength);

        if (defenderStrength <= 0f || attackerStrength <= 0f)
            result = 1f;

        Debug.Log("Battle result for Astrength " + attackerStrength + " and Dstrength " + defenderStrength + " result: " + result);

        if (attacker.GetArmy().IsThisArmyStrongerThan(defender.GetArmy()))
        {
            attackerWin = true;
            defender.GetArmy().RemoveAllUnits();
            attacker.GetArmy().ChangeUnitsAmountByPercentage(result);
        }
        else
        {
            attackerWin = false;
            attacker.GetArmy().RemoveAllUnits();
            defender.GetArmy().ChangeUnitsAmountByPercentage(result);
        }

        BattleManager.RemoveAllUnitInstancesThatShouldBeDestroyed();
        battleResult = new BattleResult(attackerWin, false, true);

        EndBattleImmediate();
        FinishBattle();
    }

    public static TargetableObject GetPlayer()
    {
        TargetableObject player = null;

        if (battleInfo.GetAttacker().MyCountry.isPlayerCountry)
            player = battleInfo.GetAttacker();
        else if (battleInfo.GetDefender().MyCountry.isPlayerCountry)
            player = battleInfo.GetDefender();

        return player;
    }

    public static TargetableObject GetAI()
    {
        TargetableObject ai = null;

        if (battleInfo.GetAttacker().MyCountry.isPlayerCountry == false || battleInfo.GetAttacker().MyCountry == null)
            ai = battleInfo.GetAttacker();
        else if (battleInfo.GetDefender().MyCountry.isPlayerCountry == false || battleInfo.GetDefender().MyCountry == null)
            ai = battleInfo.GetDefender();

        return ai;
    }

    public static void FinishBattle()
    {
        Debug.Log("FinishBattle");     
        //TimeManager.SetDefaultTime();     
        battleResult.Reset();
        isDuringBattle = false;
    }

    public static void ResetArmiesUnits()
    {
        battleInfo.GetAttacker().GetArmy().ResetUnits();
        battleInfo.GetDefender().GetArmy().ResetUnits();
    }

    //Remove all units which has amount <= 0 from both armies (attacker, defender)
    public static void RemoveAllUnitInstancesThatShouldBeDestroyed()
    {
        battleInfo.GetAttacker().GetArmy().RemoveDestroyedUnits();
        battleInfo.GetDefender().GetArmy().RemoveDestroyedUnits();
    }
}