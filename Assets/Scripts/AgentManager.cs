using UnityEngine;
using System.Collections.Generic;

public class AgentManager : MonoBehaviour
{
    [SerializeField] WorldAgent troopPrefab;
    [SerializeField] EnemyAgent enemyAgentPrefab;
    List<WorldAgent> worldAgents = new List<WorldAgent>();

    [SerializeField] BuyRequirement[] worldAgentBuyRequirements;


    public static AgentManager instance;

    AgentManager()
    {
        instance = this;
    }

    public BuyRequirement[] GetWorldAgentBuyRequirements()
    {
        return worldAgentBuyRequirements;
    }

    public List<WorldAgent> GetEnemyWorldAgentsForCountry(Country country)
    {
        List<WorldAgent> result = new List<WorldAgent>();

        foreach (WorldAgent currWorldAgent in worldAgents)
        {
            if (currWorldAgent.MyCountry == country)
                continue;

            CountryRelation currCountryRelation = CountryManager.instance.GetRelationBetweenCountries(country, currWorldAgent.MyCountry);
            if (currCountryRelation.IsEnemy())
            {
                result.Add(currWorldAgent);
            }
        }

        return result;
    }

    public WorldAgent SpawnAllyAgent(Vector3 pos, Quaternion quaternion, Country country)
    {
        WorldAgent agent = Instantiate(troopPrefab, pos, quaternion);
        SceneParent.ParentGameObjectToMe(agent.transform);
        agent.Setup(country);
        worldAgents.Add(agent);

        return agent;
    }

    public WorldAgent SpawnEnemyAgent(Vector3 pos, Quaternion quaternion, Country country)
    {
        WorldAgent agent = Instantiate(enemyAgentPrefab, pos, quaternion);
        SceneParent.ParentGameObjectToMe(agent.transform);
        agent.Setup(country);
        worldAgents.Add(agent);

        return agent;
    }

    public void RemoveFromList(WorldAgent agent)
    {
        worldAgents.Remove(agent);
    }
}