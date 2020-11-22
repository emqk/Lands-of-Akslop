using UnityEngine;
using UnityEngine.AI;

public class WorldAgent : TargetableObject
{
    [SerializeField] LineRenderer lineRenderer;

    Army army = new Army(10);

    [SerializeField] Animator animator;

    protected NavMeshAgent agent;
    public TargetableObject destinationObject;
    TargetableObject dockedIn;
    NavMeshPath currPath;
    protected readonly float interactionDistance = 2f;


    public override void OnThisObjectReached(WorldAgent enteredAgent)
    {
        if (MyCountry == enteredAgent.MyCountry)
        {
            UIManager.instance.ExchangePanel.Open(enteredAgent, this);
        }
        else
        {
            EnterThisObject(enteredAgent);
        }

        enteredAgent.MoveToTargetObject(null);
    }

    public override void OnBattleWon(TargetableObject attacker)
    {
        Debug.Log("Battle won! Attacker: " + attacker);
        ChangeRelationsAfterBattle(attacker.MyCountry);
        Destroy(gameObject);
    }

    public override void OnBattleLost(TargetableObject attacker)
    {
        Debug.Log("Battle lost! Attacker: " + attacker);
        if (attacker != null)
        {
            ChangeRelationsAfterBattle(attacker.MyCountry);
            Destroy(attacker.gameObject);
        }
    }

    public override void ChangeRelationsAfterBattle(Country attackerCountry)
    {
        if (!attackerCountry.isDefaultCountry)
        {
            Debug.Log("Changing relations because WorldAgent has been conquered");
            CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(MyCountry, attackerCountry);
            countryRelation.ChangeAmount(-5);
        }
    }

    public override Army GetArmy()
    {
        return army;
    }

    public float GetMaxSpeed()
    {
        return agent.speed;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return agent;
    }

    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currPath = new NavMeshPath();
    }

    public void Setup(Country _country)
    {
        MyCountry = _country;
        //Set base agent mesh and its line renderer color to country color
        animator.transform.GetChild(4).GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", MyCountry.GetCountryColor());
        lineRenderer.GetComponent<Renderer>().material.SetColor("_Color", MyCountry.GetCountryColor());
        lineRenderer.GetComponent<Renderer>().material.SetColor("_EmissionColor", MyCountry.GetCountryColor());
        Debug.Log("setup");
    }

    public void OnSelect()
    {
        Debug.Log("World agent selected");
        UIManager.instance.WorldAgentPanel.Open(army);
        WorldAgentWorldInfoPanelUI.instance.Open(this);
    }

    public void OnUnselect()
    {
        Debug.Log("World agent unselected");
        UIManager.instance.WorldAgentPanel.Close();
        WorldAgentWorldInfoPanelUI.instance.Close();
    }

    public void AddUnitToMyArmy(Unit unit, int amount)
    {
        army.AddUnit(new UnitInstance(unit, amount));
        if (MyCountry.isPlayerCountry)
            UIManager.instance.WorldAgentPanel.GetArmyUI().Refresh(army);
    }

    protected void Update()
    {
        RefreshPath();

        if (currPath != null)
        {
            DrawPath(currPath.corners);
        }

        if (IsDestinationObjectReached())
        {
            if (destinationObject.GetWorldActorDockedInObject() == null)
            {
                Debug.Log("Object reached", destinationObject.gameObject);
                destinationObject.OnThisObjectReached(this);
            }

            //Move all my items to destination City even if City is docking other agent
            if (destinationObject.GetComponent<City>())
            {
                destinationObject.GetComponent<City>().OnThisObjectEntrancePosition(this);
            }

            Debug.Log("I am on my destination pos! : " + name, gameObject);
        }

        ControlAnimations();
    }

    void ControlAnimations()
    {
        if (IsOnDestinationPosition())
        {
            animator.SetBool("IsMoving", false);
        }
        else
        {
            animator.SetBool("IsMoving", true);
        }
    }

    public bool IsDocked()
    {
        return dockedIn != null;
    }

    public TargetableObject GetDockedIn()
    {
        return dockedIn;
    }

    public void DockTo(TargetableObject target)
    {
        dockedIn = target;
    }

    public void Undock()
    {
        dockedIn = null;
    }

    bool IsOnDestinationPosition()
    {
        if (destinationObject)
        {
            if ((dockedIn == destinationObject && IsDocked()) || Vector3.Distance(transform.position, destinationObject.GetEntrancePos()) <= interactionDistance)
            {
                return true;
            }
            return false;
        }

        return Vector3.Distance(transform.position, agent.destination) <= interactionDistance;
    }

    public bool MoveToTargetObject(TargetableObject targetObject)
    {
        if (destinationObject && targetObject)
        {
            if (destinationObject.GetInstanceID() == targetObject.GetInstanceID() && IsDocked())
            {
                if(dockedIn.GetInstanceID() == targetObject.GetInstanceID())
                    return true;
            }
        }

        Debug.Log("Moving to target obj");
        if (targetObject == null)
        {
            destinationObject = null;
            MoveToPosition(transform.position);
            Debug.LogError("TargetObject == null! Was that intended?");
            return false;
        }

        MoveToPosition(targetObject.GetEntrancePos());
        destinationObject = targetObject;

        if (destinationObject)
        {
            if(IsOnDestinationPosition())
                return true;
        }



        return false;
    }

    bool IsDestinationObjectReached()
    {
        if (destinationObject != null)
        {
            if (IsOnDestinationPosition())
                return true;
        }

        return false;
    }

    public void MoveToPosition(Vector3 pos)
    {
        ExitFromCurrentObject();

        NavMeshPath tempPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, pos, agent.areaMask, tempPath);
        if (tempPath.status == NavMeshPathStatus.PathComplete)
        {
            currPath = tempPath;
            agent.SetPath(currPath);
        }
        else
        {
            Debug.LogError("Path is invalid!");
        }
    }
    void ExitFromCurrentObject()
    {
        if (dockedIn)
        {
            if (destinationObject == dockedIn)
            {
                destinationObject = null;
            }
            dockedIn.ExitThisObject(this);
            //Undock();
        }

        if (destinationObject)
            destinationObject = null;
    }

    void RefreshPath()
    {
        if (destinationObject)
        {
            NavMeshPath tempPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, destinationObject.GetEntrancePos(), agent.areaMask, tempPath);
            if (tempPath.status == NavMeshPathStatus.PathComplete)
            {
                currPath = tempPath;
                agent.SetPath(currPath);
            }
        }
    }

    //Refresh path if target is an object (because some object can move)
    void DrawPath(Vector3[] corners)
    {
        if (currPath != null && currPath.corners.Length > 1)
        {
            lineRenderer.positionCount = corners.Length;
            lineRenderer.SetPositions(corners);

            int currCornerIndex = NavMeshUtils.GetCurrentPathCornerIndex(agent, corners);
            for (int i = 0; i < currCornerIndex; i++)
            {
                lineRenderer.SetPosition(i, transform.position);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void OnDestroy()
    {
        MyCountry.RemoveAgentFromList(this);
        AgentManager.instance.RemoveFromList(this);

        if (MyCountry.isPlayerCountry)
        {
            ActionManager.instance.CreateAction(ActionManager.ActionInformationContent.PlayerWorldAgentDestroyed);
            UIManager.instance.WorldAgentPanel.Close();
            WorldAgentWorldInfoPanelUI.instance.Close();
        }
    }
}