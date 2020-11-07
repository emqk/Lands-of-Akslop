using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] GameObject selectionPrefab;
    GameObject currentSelector;

    WorldAgent selectedAgent;
    public WorldAgent SelectedAgent { get => selectedAgent; }

    Building selectedBuilding;

    public static Interaction instance;


    private Interaction()
    {
        instance = this;
    }

    private void Start()
    {
        SpawnSelector();
        MoveSelectorToDefaultPosition();
    }

    void SpawnSelector()
    {
        currentSelector = Instantiate(selectionPrefab);
        SceneParent.ParentGameObjectToMe(currentSelector.transform);
    }
    
    void MoveSelectorToDefaultPosition()
    {
        currentSelector.transform.transform.position = new Vector3(0, -10000, 0);
    }

    void Update()
    {
        Vector2 pos = InputManager.instance.GetCursorPosition();
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit = new RaycastHit();

        if (!UIManager.IsMouseOverUI())
        {
            if (InputManager.instance.IsRightPressedDown())
            {
                if (Physics.Raycast(ray, out hit))
                {
                    WorldAgent clickedAgent = hit.transform.GetComponent<WorldAgent>();
                    if (clickedAgent != null)
                    {
                        SelectWorldAgentIfItsPlayer(clickedAgent);
                        if (!clickedAgent.MyCountry.isPlayerCountry)
                        {
                            OpenCountryInfoUI(clickedAgent.MyCountry);
                        }
                    }
                    else
                    {
                        Building clickedBuilding = hit.transform.GetComponent<Building>();
                        if (clickedBuilding != null)
                        {
                            WorldAgent worldAgentInBulding = clickedBuilding.GetWorldActorDockedInObject();
                            if (worldAgentInBulding != null)
                            {
                                SelectWorldAgentIfItsPlayer(worldAgentInBulding);
                            }

                            if (clickedBuilding.MyCountry != null)
                            {
                                if (clickedBuilding.MyCountry.isPlayerCountry)
                                {
                                    SelectBuilding(clickedBuilding);
                                }
                                else if (!clickedBuilding.MyCountry.isDefaultCountry)
                                {
                                    OpenCountryInfoUI(clickedBuilding.MyCountry);
                                }
                            }
                        }
                    }
                }
            }
            else if (InputManager.instance.IsLeftPressedDown())
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (selectedAgent)
                    {
                        if (hit.transform.GetComponent<TargetableObject>())
                        {
                            if (hit.transform.GetComponent<TargetableObject>() != selectedAgent)
                            {
                                Debug.Log("clicked on TargetableObject");
                                selectedAgent.MoveToTargetObject(hit.transform.GetComponent<TargetableObject>());
                            }
                            else
                            {
                                Debug.LogError("Selected WorldAgent can not be targeted/docked to itself!");
                            }
                        }
                        else
                        {
                            selectedAgent.MoveToPosition(hit.point);
                        }
                    }
                }
            }
        }

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetComponent<TargetableObject>())
            {
                WorldArmyUI.instance.Open(hit.collider.GetComponent<TargetableObject>());
                if(!UIManager.IsMouseOverUI())
                    CursorManager.instance.SetCursorTexture(CursorManager.CursorType.interactionCursor);
                else
                    CursorManager.instance.SetCursorTexture(CursorManager.CursorType.defaultCursor);
            }
            else
            {
                WorldArmyUI.instance.Close();
                CursorManager.instance.SetCursorTexture(CursorManager.CursorType.defaultCursor);
            }
        }
    }

    private void LateUpdate()
    {
        if (SelectedAgent)
        {
            currentSelector.transform.position = SelectedAgent.transform.position;
        }
        else
        {
            MoveSelectorToDefaultPosition();
        }
    }

    void SelectWorldAgentIfItsPlayer(WorldAgent worldAgent)
    {
        if (worldAgent.MyCountry.isPlayerCountry)
        {
            if (selectedAgent)
            {
                UnselectCurrentAgent();
            }

            selectedAgent = worldAgent;
            worldAgent.OnSelect();
        }
    }
    void UnselectCurrentAgent()
    {
        selectedAgent.OnUnselect();
        MoveSelectorToDefaultPosition();
        selectedAgent = null;
    }

    void SelectBuilding(Building building)
    {
        if (selectedBuilding)
        {
            UnselectCurrentCity();
        }

        selectedBuilding = building;
        building.OnSelect();
    }

    void UnselectCurrentCity()
    {
        selectedBuilding.OnUnselect();
        selectedBuilding = null;
    }

    void OpenCountryInfoUI(Country country)
    {
        UIManager.instance.CountryInfoPanel.Open(country);
    }
}