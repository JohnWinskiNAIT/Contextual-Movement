using System.Threading;
using UnityEngine;

[RequireComponent(typeof(AgentContext))]
public class AvoidanceSensor : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float detectionDistance = 2f;

    [SerializeField]
    private float sensorHeight = 0.5f;

    [SerializeField]
    private float avoidanceDistance = 1.5f;

    #endregion

    #region Private Variables

    private AgentContext context;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // TODO: Get the AgentContext component.
        context = GetComponent<AgentContext>();
    }

    private void Update()
    {
        // TODO: Calculate the sensor origin and forward direction.
        Vector3 sensorOrigin = transform.position + Vector3.up * sensorHeight;
        Vector3 sensorDirection = transform.forward;

        // TODO: Draw the runtime debug ray.
        Debug.DrawRay(sensorOrigin, sensorDirection * detectionDistance, Color.red);
        bool hasHit = Physics.Raycast(sensorOrigin, sensorDirection, out RaycastHit hit, detectionDistance);

        if (!hasHit)
        {
            context.IsAvoiding = false;
            context.AvoidanceDirection = Vector3.zero;
            return;
        }

        DetectionSource source = hit.collider.GetComponent<DetectionSource>();

        if (!ShouldAvoid(source))
        {
            context.IsAvoiding = false;
            context.AvoidanceDirection = Vector3.zero;
            return;
        }

        context.IsAvoiding = true;
        context.AvoidanceDirection = -transform.right;
        // TODO: Raycast for hazards first.
        // TODO: Store IsAvoiding and AvoidanceDirection.
        // TODO: Extend the check to include obstacles.
    }

    private void OnDrawGizmos()
    {
        // TODO: Draw the red detection ray.
        // TODO: Draw the green avoidance direction while avoiding.
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns true when the detected source should be avoided.
    /// </summary>
    /// <param name="source">The source detected by the forward ray.</param>
    /// <returns>True when the source is a hazard or obstacle.</returns>
    private bool ShouldAvoid(
        DetectionSource source)
    {
        // TODO: Begin with hazards, then extend to obstacles.
        if (source == null)
        {
            return false;
        }
        return source.Type == DetectionType.Hazard |
            source.Type == DetectionType.Obstacle;
    }

    #endregion
}
