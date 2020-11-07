using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Country
{
    [SerializeField] string countryName;
    Color countryColor;
    public bool isPlayerCountry = false;
    public bool isDefaultCountry = false;
    [SerializeField] List<Building> buildings = new List<Building>();
    [SerializeField] List<WorldAgent> worldAgents = new List<WorldAgent>();

    Inventory inventory = new Inventory(300);
    public Inventory Inventory
    {
        get => inventory;
    }

    public Country(string _countryName, bool _isPlayerCountry, bool _isDefaultCountry, Color _color)
    {
        countryName = _countryName;
        isPlayerCountry = _isPlayerCountry;
        isDefaultCountry = _isDefaultCountry;
        countryColor = _color;

        Inventory.AddItem(new Item(ItemType.Gold, 1000));
        Inventory.AddItem(new Item(ItemType.Wood, 10));
        Inventory.AddItem(new Item(ItemType.Iron, 10));
        Inventory.AddItem(new Item(ItemType.Stone, 0));
        Inventory.AddItem(new Item(ItemType.Human, 10));
        Inventory.AddItem(new Item(ItemType.Food, 5));

        SetFirstCity();
    }

    void SetFirstCity()
    {
        if (isDefaultCountry == false)
        {
            City emptyCity = CaptureEmptyCity();
            for (int i = 0; i < 1; i++)
            {
                if (emptyCity)
                {
                    emptyCity.SpawnWorldAgentInThisCity();
                }
            }
        }
    }
    City CaptureEmptyCity()
    {
        City cityToCapture = CountryManager.instance.GetRandomNotCapturedCity();
        if (cityToCapture)
        {
            CaptureNewBuilding(cityToCapture);
            return cityToCapture;
        }

        return null;
    }

    public City GetNearestUndockedCity(WorldAgent worldAgent)
    {
        City nearestCity = null;
        float minDistance = float.MaxValue;

        foreach (Building currBuilding in buildings)
        {
            City currCity = currBuilding.GetComponent<City>();
            if (currCity)
            {
                if (currCity.GetWorldActorDockedInObject() == null)
                {
                    float navMeshDistance = NavMeshUtils.GetDistanceBetweenPointsOnNavMeshPath(worldAgent.GetNavMeshAgent(), worldAgent.transform.position, currCity.GetEntrancePos());
                    if (navMeshDistance < minDistance && navMeshDistance >= 0)
                    {
                        minDistance = navMeshDistance;
                        nearestCity = currCity;
                    }
                }
            }
        }

        return nearestCity;
    }

    City GetUndockedCity()
    {
        foreach (Building currBuilding in buildings)
        {
            City currCity = currBuilding.GetComponent<City>();
            if (currCity)
            {
                if (currCity.GetWorldActorDockedInObject() == null)
                {
                    return currCity;
                }
            }
        }

        return null;
    }

    public bool IsTherePlaceForNextWorldAgent()
    {
        return worldAgents.Count < GetCitiesCount();
    }

    public int GetArmyStrength()
    {
        int result = 0;

        foreach (WorldAgent worldAgent in worldAgents)
        {
            result += worldAgent.GetArmy().GetArmyStrength();
        }

        foreach (Building building in buildings)
        {
            result += building.GetArmy().GetArmyStrength();
        }

        return result;
    }

    public bool ShoulBeDestroyed()
    {
        return buildings.Count <= 0 && worldAgents.Count <= 0;
    }

    public void Update()
    {
        if (isPlayerCountry == false && isDefaultCountry == false)
        {
            Debug.Log("Updating non-player and non-default country");
            ControlStationaryAI();
            ControlAIWorldAgents();
        }
    }

    bool controlRelationsInvoked = true;
    void ControlStationaryAI()
    {
        if (IsTherePlaceForNextWorldAgent())
        {
            BuyRequirement[] WorldAgentBuyRequirements = AgentManager.instance.GetWorldAgentBuyRequirements();
            if (Inventory.HaveEnoughToBuy(WorldAgentBuyRequirements, 1))
                GetUndockedCity().SpawnWorldAgentInThisCity();
        }
        else
        {
            City weakestCity = FindCityWithWeakestArmy();
            if (weakestCity)
            {
                Unit[] availableUnits = UnitsManager.instance.GetAllAvailableUnits();
                for (int i = 0; i < availableUnits.Length; i++)
                {
                    Unit unitToBuy = availableUnits[i];
                    if (Inventory.HaveEnoughToBuy(unitToBuy.buyRequirements, 1))
                    {
                        Inventory.PayRequirements(unitToBuy.buyRequirements, 1);
                        weakestCity.Army.AddUnit(new UnitInstance(unitToBuy, 1));
                    }
                }
            }
        }

        ControlBuildings();

        //Control Country realtions once per random time
        if (controlRelationsInvoked == true)
        {
            TimerManager.instance.AddTimer(new Timer(ControlRelations, Random.Range(5f, 10f)));
            controlRelationsInvoked = false;
        }
    }
    void ControlBuildings()
    {
        foreach (Building building in buildings)
        {
            City city = building.GetComponent<City>();
            if (city)
            {
                if (!city.HaveWalls && city.CanBuyWalls())
                {
                    city.BuildWalls();
                }
            }
        }
    }
    void ControlRelations()
    {
        controlRelationsInvoked = true;
        Debug.Log("Control relations Invoke");

        int random = Random.Range(0, 100);
        if(random <= 49)
            ControlPeace();
        else 
            ControlWars();
    }
    void ControlWars()
    {
        List<Country> enemyCountries = CountryManager.instance.GetEnemyCountriesForCountry(this);
        enemyCountries.Remove(CountryManager.instance.DefaultCountry);
        if (enemyCountries.Count > 0)
            return;

        List<Country> friendlyCountry = CountryManager.instance.GetFriendlyCountriesWithWorstRelationsWithCountryExceptDefaultCountry(this);

        foreach (Country item in friendlyCountry)
        {
            //Is 110% of enemy army strength weaker than my army
            Debug.Log(countryName + " - > " + item.countryName + " Trying to make war: my: " + GetArmyStrength() + " other: " + item.GetArmyStrength());
            if (item.GetArmyStrength() * 1.1f < GetArmyStrength())
            {
                CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(this, item);
                countryRelation.MakeWar();
                return;
            }
        }
    }
    void ControlPeace()
    {
        List<Country> enemyCountries = CountryManager.instance.GetEnemyCountriesForCountry(this);
        enemyCountries.Remove(CountryManager.instance.DefaultCountry);
        if (enemyCountries.Count <= 0)
            return;

        foreach (Country item in enemyCountries)
        {
            //Is enemy army strength stronger than my army
            Debug.LogError(countryName + " - > " + item.countryName + " Trying to make peace: my: " + GetArmyStrength() + " other: " + item.GetArmyStrength());
            if (item.GetArmyStrength() > GetArmyStrength() && item.GetArmyStrength() < GetArmyStrength() * 1.5f)
            {
                CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(this, item);
                countryRelation.MakePeace();
                return;
            }
        }

    }
    City FindCityWithWeakestArmy()
    {
        int weakestArmyStrength = int.MaxValue;
        City weakestCity = null;

        foreach (Building currentBuilding in buildings)
        {
            City currentCity = currentBuilding.GetComponent<City>();
            if (currentCity)
            {
                int currentCityArmyStrength = currentCity.Army.GetArmyStrength();
                if (currentCityArmyStrength <= weakestArmyStrength)
                {
                    weakestArmyStrength = currentCityArmyStrength;
                    weakestCity = currentCity;
                }
            }
        }

        return weakestCity;
    }

    void GetOnlyWeakerArmiesFilter<T>(ref List<T> targetableObjects, Army army) where T : TargetableObject
    {
        for (int i = targetableObjects.Count - 1; i >= 0; i--)
        {
            if (!army.IsThisArmyStrongerThan(targetableObjects[i].GetArmy()))
            {
                targetableObjects.RemoveAt(i);
            }
        }
    }

    void ControlAIWorldAgents()
    {
        //Prepare
        List<WorldAgent> notFriendlyWorldAgents = AgentManager.instance.GetEnemyWorldAgentsForCountry(this);
        List<City> defaultCountryCities = CountryManager.instance.GetAllCitiesWithDefaultCountry();
        List<UtilityBuilding> defaultUtilityBuildings = CountryManager.instance.GetAllUtilityBuildingsWithDefaultCountry();
        List<City> notFriendlyCities = CountryManager.instance.GetEnemyCitiesForCountry(this);
        List<UtilityBuilding> notFriendlyUtilityBuildings = CountryManager.instance.GetEnemyUtilityBuildingsForCountry(this);
        List<WorldItem> worldItems = WorldItem.GetAllWorldItems();

        List<WorldAgent> worldAgentsNotUsed = new List<WorldAgent>(worldAgents);

        //Do behaviour
        ConquerPlotCityWithWeakerArmy(ref worldAgentsNotUsed);
        ConquerClosestBuildingWithWeakerArmy<City>(defaultCountryCities, ref worldAgentsNotUsed);
        ConquerClosestBuildingWithWeakerArmy<UtilityBuilding>(defaultUtilityBuildings, ref worldAgentsNotUsed);
        ChaseClosestEnemyWorldAgent(notFriendlyWorldAgents, ref worldAgentsNotUsed);
        CaptureClosestEnemyBuildingWithWeakerArmy(notFriendlyUtilityBuildings, ref worldAgentsNotUsed);
        CaptureClosestEnemyBuildingWithWeakerArmy(notFriendlyCities, ref worldAgentsNotUsed);
        ConquerClosestBuildingWithWeakerArmy<WorldItem>(worldItems, ref worldAgentsNotUsed);

        //Go to IdleState for all not used WorldAgents
        for (int i = 0; i < worldAgentsNotUsed.Count; i++)
        {
            worldAgentsNotUsed[i].GetComponent<EnemyAgent>().blackboard.targetableObject = null;
            worldAgentsNotUsed[i].GetComponent<EnemyAgent>().stateMachine.ChangeState(new IdleState());
        }   
    }
    void ConquerPlotCityWithWeakerArmy(ref List<WorldAgent> worldAgentsNotUsed)
    {
        for (int i = worldAgentsNotUsed.Count - 1; i >= 0; i--)
        {
            WorldAgent currAgent = worldAgentsNotUsed[i];
            PlotCity plotCity = PlotCity.instance;

            if (plotCity)
            {
                if (currAgent.GetArmy().IsThisArmyStrongerThan(plotCity.GetArmy()))
                {
                    CaptureEnemyTargetableObject(currAgent, plotCity);
                    worldAgentsNotUsed.RemoveAt(i);
                }
            }
        }
    }
    void ChaseClosestEnemyWorldAgent(List<WorldAgent> notFriendlyWorldAgents, ref List<WorldAgent> worldAgentsNotUsed)
    {
        for (int i = worldAgentsNotUsed.Count-1; i >= 0; i--)
        {
            //Make copy of target list for every AI
            List<WorldAgent> notFriendlyWorldAgentsLocal = new List<WorldAgent>(notFriendlyWorldAgents);
            EnemyAgent AIAgent = worldAgentsNotUsed[i].GetComponent<EnemyAgent>();
            if (AIAgent)
            {
                GetOnlyWeakerArmiesFilter(ref notFriendlyWorldAgentsLocal, AIAgent.GetArmy());
                WorldAgent targetAgent = GetClosestTargetableObjectFromList(notFriendlyWorldAgentsLocal, AIAgent.GetNavMeshAgent(), AIAgent.blackboard.sightDistance);
                if (targetAgent)
                {
                    AIAgent.ChaseWorldAgent(targetAgent);
                    worldAgentsNotUsed.RemoveAt(i);
                }
            }
        }
    }
    void CaptureClosestEnemyBuildingWithWeakerArmy<T>(List<T> notFriendlyBuildings, ref List<WorldAgent> worldAgentsNotUsed) where T : TargetableObject
    {
        for (int i = worldAgentsNotUsed.Count-1; i >= 0 ; i--)
        {
            //Make copy of target list for every AI
            List<T> notFriendlyBuildingsLocal = new List<T>(notFriendlyBuildings);
            WorldAgent worldAgent = worldAgentsNotUsed[i];
            GetOnlyWeakerArmiesFilter(ref notFriendlyBuildingsLocal, worldAgent.GetArmy());
            T closestBuilding = GetClosestTargetableObjectFromList<T>(notFriendlyBuildingsLocal, worldAgent.GetNavMeshAgent(), worldAgent.GetComponent<EnemyAgent>().blackboard.sightDistance);

            if (closestBuilding)
            {
                CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(this, closestBuilding.MyCountry);
                if (countryRelation.IsEnemy() && closestBuilding.GetWorldActorDockedInObject() == null)
                {
                    CaptureEnemyTargetableObject(worldAgent, closestBuilding);
                    worldAgentsNotUsed.RemoveAt(i);
                }
            }
        }
    }
    void ConquerClosestBuildingWithWeakerArmy<T>(List<T> buildingsList, ref List<WorldAgent> worldAgentsNotUsed) where T : TargetableObject
    {
        for (int i = worldAgentsNotUsed.Count-1; i >= 0; i--)
        {
            //Make copy of target list for every AI
            List<T> buildingsListLocal = new List<T>(buildingsList);
            WorldAgent currAgent = worldAgentsNotUsed[i];
            GetOnlyWeakerArmiesFilter(ref buildingsListLocal, currAgent.GetArmy());
            T closestTargetBuildingToConquer = GetClosestTargetableObjectFromList(buildingsListLocal, currAgent.GetNavMeshAgent(), currAgent.GetComponent<EnemyAgent>().blackboard.sightDistance);

            if (closestTargetBuildingToConquer)
            {
                CaptureEnemyTargetableObject(currAgent, closestTargetBuildingToConquer);
                worldAgentsNotUsed.RemoveAt(i);
            }
        }   
    }
    void CaptureEnemyTargetableObject(WorldAgent worldAgent, TargetableObject targetableObject)
    {
        worldAgent.GetComponent<EnemyAgent>().blackboard.targetableObject = targetableObject;
        worldAgent.GetComponent<EnemyAgent>().stateMachine.ChangeState(new CaptureTargetState());
    }

    T GetClosestTargetableObjectFromList<T>(List<T> objectList, UnityEngine.AI.NavMeshAgent agent, float maxDistance) where T : TargetableObject
    {
        T closestObject = null;
        float minDistance = float.MaxValue;

        foreach (T currentObject in objectList)
        {
            float navmeshDistance = NavMeshUtils.GetDistanceBetweenPointsOnNavMeshPath(agent, agent.transform.position, currentObject.GetEntrancePos());
            if (navmeshDistance < minDistance && navmeshDistance >= 0 && navmeshDistance <= maxDistance)
            {
                minDistance = navmeshDistance;
                closestObject = currentObject;
            }
        }

        return closestObject;
    }

    public Color GetCountryColor()
    {
        return countryColor;
    }

    public string GetCountryName()
    {
        return countryName;
    }

    public List<UtilityBuilding> GetUtilityBuildingOfType(ItemType itemType)
    {
        List<UtilityBuilding> result = new List<UtilityBuilding>();
         for (int i = 0; i < buildings.Count; i++)
         {
             UtilityBuilding utilityBuilding = null;
             if (utilityBuilding = buildings[i].GetComponent<UtilityBuilding>())
             {
                 if (utilityBuilding.GetProducedItemType() == itemType)
                 {
                     result.Add(utilityBuilding);
                 }
             }
         }

        return result;
    }

    public int GetCitiesCount()
    {
        int result = 0;

        foreach (Building building in buildings)
        {
            if (building.GetComponent<City>())
            {
                result++;
            }
        }

        return result;
    }

    public WorldAgent SpawnWorldAgent(Vector3 pos, bool isPlayer)
    {
        WorldAgent agentInstance = null;
        if (isPlayer)
            agentInstance = AgentManager.instance.SpawnAllyAgent(pos, Quaternion.Euler(0, 0, 0), this);
        else
            agentInstance = AgentManager.instance.SpawnEnemyAgent(pos, Quaternion.Euler(0, 0, 0), this);

        worldAgents.Add(agentInstance);

        return agentInstance;
    }

    public void CaptureNewBuilding(Building building)
    {
        if (building == null)
        {
            Debug.LogError("building is null!");
        }

        //Remove from old country
        if (building.MyCountry != null)
        {
            building.MyCountry.RemoveBuildingFromList(building);
        }
        
        //Add to new country
        building.MyCountry = this;
        building.SetMaterialBaseColorToCountryColor(this);
        buildings.Add(building);
    }

    public void AddItem(Item item)
    {
        Inventory.AddItem(item);
    }

    public bool CanTradeItem(Item item)
    {
        return Inventory.GetItemAmount(item.itemType) - item.amount >= 0;
    }

    public void RemoveBuildingFromList(Building building)
    {
        buildings.Remove(building);
    }

    public void RemoveAgentFromList(WorldAgent agent)
    {
        worldAgents.Remove(agent);
    }
}