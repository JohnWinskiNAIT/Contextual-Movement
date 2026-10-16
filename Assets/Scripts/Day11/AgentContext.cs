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

    public bool HasKey;

    public bool HasResponseTarget;

    public GameObject CurrentTarget;

    public Transform CurrentAgentTarget;

    public Transform VisiblePickup;

    public Transform VisibleAgent;

    public Transform VisibleTarget;

    public Transform ResponseTarget;

    public Transform LastPerceivedObject;

    public DetectionType CurrentDetection;

    public Vector3 AvoidanceDirection;

    public Vector3 LastPerceivedPosition;

    public float LastPerceivedTime;

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

        LastPerceivedObject = obj;
        LastPerceivedPosition = obj.position;
        LastPerceivedTime = Time.time;
    }

    #endregion
}
