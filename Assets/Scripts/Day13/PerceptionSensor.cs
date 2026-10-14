using UnityEngine;

[RequireComponent(typeof(AgentContext))]
public class PerceptionSensor : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float perceptionRadius = 6f;

    [SerializeField]
    private float fieldOfViewAngle = 90f;

    [SerializeField]
    private float eyeHeight = 0.5f;

    [SerializeField]
    private LayerMask perceptionMask;

    [SerializeField]
    private LayerMask obstructionMask;

    #endregion

    #region Private Variables

    private AgentContext context;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // TODO: Get AgentContext.
        context = GetComponent<AgentContext>();
    }

    private void Update()
    {
        // TODO: Clear the previous perception results.
        context.ClearPerception();

        // TODO: Find nearby colliders with Physics.OverlapSphere.
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, perceptionRadius, perceptionMask, QueryTriggerInteraction.Collide);

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            Transform perceivedTransform = GetTaggedTransform(nearbyCollider.transform);

            if (perceivedTransform == null || perceivedTransform.root == transform.root)
            {
                continue;
            }

            if (!IsInsideFieldOfView(perceivedTransform) || !HasLineOfSight(perceivedTransform))
            {
                continue;
            }

            context.AddPerceivedObject(perceivedTransform);
        }
        

        // TODO: Filter candidates by field of view.
        // TODO: Confirm line of sight with Physics.Raycast.
        // TODO: Resolve each candidate to its tagged root object.
        // TODO: Add visible objects to AgentContext.
    }

    private void OnDrawGizmosSelected()
    {
        // TODO: Draw the perception radius.
        // TODO: Draw the left and right field-of-view boundaries.
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns true when a candidate is inside the sensor's field of view.
    /// </summary>
    /// <param name="candidate">The candidate transform.</param>
    /// <returns>True when the candidate is inside the configured field of view.</returns>
    private bool IsInsideFieldOfView(Transform candidate)
    {
        // TODO: Use Vector3.Dot to compare the forward direction with the direction to the candidate.
        Vector3 directionToCandidate = candidate.position - transform.position;
        directionToCandidate.y = 0f;

        if (directionToCandidate.sqrMagnitude <= 0)
        {
            return true;
        }

        directionToCandidate.Normalize();
        float minimumDot = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
        float forwardDot = Vector3.Dot(transform.forward, directionToCandidate);

        return forwardDot >= minimumDot;
    }

    /// <summary>
    /// Returns true when no obstruction blocks the candidate.
    /// </summary>
    /// <param name="candidate">The candidate transform.</param>
    /// <returns>True when the candidate has a clear line of sight.</returns>
    private bool HasLineOfSight(Transform candidate)
    {
        // TODO: Raycast from eye height toward the candidate.
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 destination = candidate.position + Vector3.up * eyeHeight;
        Vector3 direction = destination - origin;
        float distance = direction.magnitude;

        if (distance <= 0f)
        {
            return true;
        }

        direction.Normalize();
        Debug.DrawRay(origin, direction * distance, Color.cyan);

        if (!Physics.Raycast(origin, direction, out RaycastHit hit, distance, perceptionMask | obstructionMask, QueryTriggerInteraction.Collide))
        {
            return false;
        }

        return hit.transform.root == candidate.root;
    }

    /// <summary>
    /// Finds the tagged transform that represents a perceived object.
    /// </summary>
    /// <param name="candidate">The transform returned by the physics query.</param>
    /// <returns>The tagged root or parent transform, or null if no supported tag is found.</returns>
    private Transform GetTaggedTransform(Transform candidate)
    {
        // TODO: Search the candidate and its parents for a supported perception tag.
        Transform current = candidate;

        while (current != null)
        {
            if (current.CompareTag("Player") ||
                current.CompareTag("AIAgent") ||
                current.CompareTag("Key") ||
                current.CompareTag("Powerup") ||
                current.CompareTag("Chest") ||
                current.CompareTag("Hazard"))
            {
                return current;
            }
            current = current.parent;
        }
        return null;
    }

    #endregion
}
