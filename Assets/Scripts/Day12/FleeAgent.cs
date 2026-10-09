using UnityEngine;

public class FleeAgent : AgentMovement
{
    #region Protected Methods

    /// <summary>
    /// Calculates a direction from the current agent target away from this agent.
    /// </summary>
    /// <returns>The unnormalized flee direction.</returns>
    protected override Vector3 GetIntentDirection()
    {
        // TODO: Subtract the target position from this agent's position.
        return rigidBody.position - context.CurrentAgentTarget.position;
    }

    #endregion

    #region Unity Methods

    private void OnDrawGizmosSelected()
    {
        // TODO: Draw the flee direction in cyan.
    }

    #endregion
}
