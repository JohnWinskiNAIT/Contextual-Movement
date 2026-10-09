using UnityEngine;

public class ChaseAgent : AgentMovement
{
    #region Protected Methods

    /// <summary>
    /// Calculates a direction from this agent toward its current agent target.
    /// </summary>
    /// <returns>The unnormalized chase direction.</returns>
    protected override Vector3 GetIntentDirection()
    {
        // TODO: Subtract this agent's position from the target position.
        return context.CurrentAgentTarget.position - rigidBody.position;
    }

    #endregion

    #region Unity Methods

    private void OnDrawGizmosSelected()
    {
        // TODO: Draw the chase direction in blue.
    }

    #endregion
}
