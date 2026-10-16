using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AgentContext))]
public abstract class AgentResponseController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float turnSpeed = 8f;

    [SerializeField]
    private float stopDistance = 0.75f;

    [SerializeField]
    private float memoryDuration = 2f;

    #endregion

    #region Protected Variables

    protected AgentContext context;

    protected Rigidbody rigidBody;

    #endregion

    #region Unity Methods

    protected virtual void Awake()
    {
        // TODO: Get AgentContext and Rigidbody, then configure top-down movement.
        context = GetComponent<AgentContext>();
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        // TODO: Clear the prior response target before evaluating this frame.
        context.ResponseTarget = null;
        context.HasResponseTarget = false;

        // TODO: Get the contextual response direction.
        Vector3 responseDirection = GetResponseDirection();

        // TODO: Use the avoidance direction while IsAvoiding is true.
        Vector3 movementDirection = context.IsAvoiding ? context.AvoidanceDirection : responseDirection;

        // TODO: Rotate and move with the Rigidbody.
        movementDirection.y = 0f;
        if (movementDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        if (!context.IsAvoiding && context.ResponseTarget != null)
        {
            Vector3 toTarget = context.ResponseTarget.position - rigidBody.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude <= stopDistance)
            {
                return;
            }
        }

        movementDirection.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        Vector3 nextPosition = rigidBody.position + movementDirection * moveSpeed * Time.fixedDeltaTime;
        rigidBody.MoveRotation(nextRotation);
        rigidBody.MovePosition(nextPosition);
    }

    private void Update()
    {
        // TODO: Draw a magenta line to ResponseTarget when it is not null.
        if (context.ResponseTarget != null)
        {
            Debug.DrawLine(transform.position, context.ResponseTarget.position, Color.magenta);
        }

    }
    #endregion

    #region Protected Methods

    protected abstract Vector3 GetResponseDirection();

    protected Vector3 GetDirectionTo(Transform target)
    {
        // TODO: Return the horizontal direction toward target.
        if (target == null)
        {
            return Vector3.zero;
        }

        Vector3 d = target.position - rigidBody.position;
        d.y = 0f;

        return d;
    }

    protected Vector3 GetDirectionAwayFrom(Transform target)
    {
        // TODO: Return the horizontal direction away from target.
        if (target == null)
        {
            return Vector3.zero;
        }

        Vector3 d = rigidBody.position - target.position;
        d.y = 0f;

        return d;
    }

    protected bool HasRecentPerception()
    {
        // TODO: Check LastPerceivedObject and memoryDuration.

        return context.LastPerceivedObject != null && Time.time - context.LastPerceivedTime <= memoryDuration;
    }

    protected Vector3 GetDirectionToLastPerceivedPosition()
    {
        // TODO: Return the horizontal direction to LastPerceivedPosition.
        Vector3 d = context.LastPerceivedPosition - rigidBody.position;
        d.y = 0f;

        return d;
    }

    protected void SetResponseTarget(Transform target)
    {
        // TODO: Store ResponseTarget and HasResponseTarget.
        context.ResponseTarget = target;

        context.HasResponseTarget = target != null;
    }
    #endregion
}
