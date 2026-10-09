using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AgentContext))]
public class SimpleAgentController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float turnSpeed = 100f;

    [SerializeField]
    private float stopDistance = 0.75f;

    #endregion

    #region Private Variables

    private AgentContext context;

    private Rigidbody rigidBody;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        context = GetComponent<AgentContext>();

        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezePositionY |                      RigidbodyConstraints.FreezeRotationX |
          RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (context.CurrentTarget == null)
        {
            return;
        }

        Vector3 targetPosition = context.CurrentTarget.transform.position;

        Vector3 targetDirection = targetPosition - rigidBody.position;

        targetDirection.y = 0f;

        if (!context.IsAvoiding && targetDirection.magnitude <= stopDistance)
        {
            return;
        }

        Vector3 movementDirection = context.IsAvoiding ? context.AvoidanceDirection : targetDirection;

        movementDirection.y = 0;

        if (movementDirection.sqrMagnitude <= 0)
        {
            return;
        }

        movementDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

        Quaternion nextRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

        Vector3 nextPosition = rigidBody.position + movementDirection * moveSpeed * Time.fixedDeltaTime;

        rigidBody.MoveRotation(nextRotation);

        rigidBody.MovePosition(nextPosition);
    }

    #endregion
}
