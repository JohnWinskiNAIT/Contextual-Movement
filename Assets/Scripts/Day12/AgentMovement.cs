using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AgentContext))]
public abstract class AgentMovement : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float turnSpeed = 8f;

    #endregion

    #region Protected Variables

    protected AgentContext context;
    protected Rigidbody rigidBody;

    #endregion

    #region Unity Methods

    protected virtual void Awake()
    {
        // TODO: Get the AgentContext and Rigidbody components.
        context = GetComponent<AgentContext>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // TODO: Configure the Rigidbody for top-down movement.
    }

    private void FixedUpdate()
    {
        // TODO: Return when there is no agent target.
        context.HasAgentTarget = context.CurrentAgentTarget != null;

        if (!context.HasAgentTarget)
        {
            return;
        }

        // TODO: Get the chase or flee direction from the derived class.
        Vector3 intentDirection = GetIntentDirection();
        intentDirection.y = 0;

        Vector3 movementDirection = context.IsAvoiding ? context.AvoidanceDirection : intentDirection;
        movementDirection.y = 0;

        if (movementDirection.sqrMagnitude <= 0)
        {
            return;
        }

        movementDirection.Normalize();

        // TODO: Let avoidance override the intent direction.
        // TODO: Rotate toward the selected movement direction.
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        Vector3 nextPosition = rigidBody.position + movementDirection * moveSpeed * Time.fixedDeltaTime;

        // TODO: Move with Rigidbody.MovePosition.

        rigidBody.MoveRotation(nextRotation);
        rigidBody.MovePosition(nextPosition);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Calculates the movement direction created by the agent's current intent.
    /// </summary>
    /// <returns>The unnormalized world-space movement direction.</returns>
    protected abstract Vector3 GetIntentDirection();

    #endregion
}
