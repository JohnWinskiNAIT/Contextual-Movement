using System.Collections.Generic;
using UnityEngine;

public class AgentContext : MonoBehaviour
{
    #region Public Variables

    public bool IsNearObstacle;

    public bool IsNearHazard;

    public bool IsNearTarget;

    public bool IsAvoiding;

    public bool HasAgentTarget;

    public GameObject CurrentTarget;

    public Transform CurrentAgentTarget;

    public DetectionType CurrentDetection;

    public Vector3 AvoidanceDirection;

    public List<Transform> PerceivedObjects = new List<Transform>();

    #endregion

    #region Public Methods

    /// <summary>
    /// Prints the current environmental and avoidance context.
    /// </summary>
    public void PrintCurrentState()
    {
        string targetName =
            CurrentTarget == null
                ? "None"
                : CurrentTarget.name;

        Debug.Log(
            $"Detection: {CurrentDetection} | " +
            $"Is Near Obstacle: {IsNearObstacle} | " +
            $"Is Near Hazard: {IsNearHazard} | " +
            $"Is Near Target: {IsNearTarget} | " +
            $"Is Avoiding: {IsAvoiding} | " +
            $"Avoidance Direction: {AvoidanceDirection} | " +
            $"Current Target: {targetName}",
            this);
    }

    public void ClearPerception()
    {
        PerceivedObjects.Clear();
    }

    public void AddPerceivedObject(Transform obj)
    {
        PerceivedObjects.Add(obj);
    }

    #endregion
}
