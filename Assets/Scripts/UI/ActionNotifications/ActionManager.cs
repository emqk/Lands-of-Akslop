using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public class ActionData
    {
        public Color color;
        public string content;
        public GameObject actionObj;

        public ActionData(Color _color, string _content, GameObject _actionObj)
        {
            color = _color;
            content = _content;
            actionObj = _actionObj;
        }
    }

    public enum ActionInformationContent
    {
        PeaceDeclared, WarDeclared,
        PlayerBuildingCaptured, PlayerWorldAgentDestroyed
    }
    public enum ActionType
    {
        BadInfo, DefaultInfo, GoodInfo
    }

    [SerializeField] ActionNotificationUI actionUIPrefab;
    [SerializeField] Transform actionUIParent;

    [SerializeField] Color badColor;
    [SerializeField] Color defaultColor;
    [SerializeField] Color goodColor;

    readonly float UILifeTime = 7f;

    public static ActionManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void CreateAction(ActionInformationContent actionInformationContent, GameObject actionObj = null)
    {
        ActionNotificationUI instance = Instantiate(actionUIPrefab, actionUIParent);
        instance.Refresh(GetActionDataBasedOnActionInformation(actionInformationContent, actionObj), UILifeTime);

        Debug.Log("Created action: " + actionInformationContent, actionObj);
    }

    ActionData GetActionDataBasedOnActionInformation(ActionInformationContent actionInformationContent, GameObject actionObj)
    {
        switch (actionInformationContent)
        {
            case ActionInformationContent.PeaceDeclared:
                return new ActionData(GetColorBasedOnActionType(ActionType.GoodInfo), "Made peace!", actionObj);
            case ActionInformationContent.WarDeclared:
                return new ActionData(GetColorBasedOnActionType(ActionType.BadInfo), "War has been declared!", actionObj);
            case ActionInformationContent.PlayerBuildingCaptured:
                return new ActionData(GetColorBasedOnActionType(ActionType.BadInfo), "Your building has been captured!", actionObj);
            case ActionInformationContent.PlayerWorldAgentDestroyed:
                return new ActionData(GetColorBasedOnActionType(ActionType.BadInfo), "Your general has been destroyed!", actionObj);
            default:
                return new ActionData(Color.white, "", null);
        }
    }
    Color GetColorBasedOnActionType(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.BadInfo:
                return badColor;
            case ActionType.DefaultInfo:
                return defaultColor;
            case ActionType.GoodInfo:
                return goodColor;
            default:
                return Color.white;
        }
    }
}
