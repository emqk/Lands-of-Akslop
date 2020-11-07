using UnityEngine;
using System.Collections.Generic;

public class WorldEvent : WorldItem
{
    [System.Serializable]
    public struct WorldEventData
    {
        [TextArea] public string description;
        public WorldEventButtonData[] buttonsData;
    }
    [System.Serializable]
    public struct WorldEventButtonData
    {
        public string buttonsText;
        public bool shouldStartBattle;
    }

    [SerializeField] WorldEventData worldEventData;

    WorldAgent enteredWorldAgent = null;

    BuyRequirement[] chosenItemsOption;

    public WorldEventData GetWorldEventData()
    {
        return worldEventData;
    }

    public WorldAgent GetEnteredWorldAgent()
    {
        return enteredWorldAgent;
    }

    public override void OnThisObjectReached(WorldAgent enteredAgent)
    {
        if (!enteredWorldAgent)
        {
            enteredWorldAgent = enteredAgent;
            if (enteredWorldAgent.GetComponent<EnemyAgent>())
                HandleAIEntrance();
            else
                HandlePlayerEntrance();
        }
    }
    void HandlePlayerEntrance()
    {
        WorldEventUIManager.instance.ShowEvent(WorldEventUIPanel.instance, this);
    }
    void HandleAIEntrance()
    {
        List<int> availableChoices = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            if(enteredWorldAgent.MyCountry.Inventory.WillEveryItemWillBePositiveOrZeroAfterAdding(items[i].Get()))
                availableChoices.Add(i);
        }

        int choice = availableChoices[Random.Range(0, availableChoices.Count)];
        Option(choice);
    }

    public override void OnBattleLost(TargetableObject attacker)
    {
        enteredWorldAgent = null;
        Destroy(attacker.gameObject);
    }

    public override void OnBattleWon(TargetableObject attacker)
    {
        attacker.MyCountry.Inventory.AddItem(chosenItemsOption);
        enteredWorldAgent = null;
        DestroyMe();
    }

    void StartBattleIfShouldOrGiveItemsIfNot(bool shouldStartBattle)
    {
        if (shouldStartBattle)
        {
            EnterThisObject(enteredWorldAgent);
        }
        else
        {
            enteredWorldAgent.MyCountry.Inventory.AddItem(chosenItemsOption);
            DestroyMe();
        }
    }

    public void Option(int optionIndex)
    {
        Debug.Log("AI chose option " + optionIndex);
        chosenItemsOption = items[optionIndex].Get();
        StartBattleIfShouldOrGiveItemsIfNot(worldEventData.buttonsData[optionIndex].shouldStartBattle);
    }
}