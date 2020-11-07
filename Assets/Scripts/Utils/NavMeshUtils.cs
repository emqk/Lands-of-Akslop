using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static float GetAgentCurrentDestinationPathDistance(NavMeshAgent agent)
    {
        return GetNavMeshAgentPathDistance(agent);
    }

    public static float GetDistanceBetweenPointsOnNavMeshPath(NavMeshAgent agent, Vector3 source, Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(source, destination, agent.areaMask, path);

        return GetNavMeshPathDistance(path);
    }
    
    static float GetNavMeshAgentPathDistance(NavMeshAgent agent)
    {
        float result = 0;

        NavMeshPath path = agent.path;

        int startIndex = GetCurrentPathCornerIndex(agent, path.corners);
        int endIndex = path.corners.Length - 1;
        if (path.corners.Length < 2 || startIndex < 0)
        {
            Debug.LogError("NavMeshPath distance is invalid. Return!");
            return -1;
        }

        result += Vector3.Distance(agent.transform.position, path.corners[startIndex]);
        for (int i = startIndex; i < endIndex; i++)
        {
            result += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return result;
    }

    static float GetNavMeshPathDistance(NavMeshPath path)
    {
        float result = 0;

        int startIndex = 0;
        int endIndex = path.corners.Length - 1;
        if (path.corners.Length < 2 || startIndex < 0)
        {
            Debug.LogError("NavMeshPath distance is invalid. Return!");
            return -1;
        }

        result += Vector3.Distance(path.corners[0], path.corners[startIndex]);
        for (int i = startIndex; i < endIndex; i++)
        {
            result += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return result;
    }

    public static int GetCurrentPathCornerIndex(NavMeshAgent agent, Vector3[] corners)
    {
        for (int i = 0; i < corners.Length; i++)
        {
            if (agent.steeringTarget == corners[i])
                return i;
        }

        return -1;
    }
}
