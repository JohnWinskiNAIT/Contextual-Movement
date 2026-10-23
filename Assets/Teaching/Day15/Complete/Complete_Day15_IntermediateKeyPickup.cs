using UnityEngine;

public class Complete_Day15_IntermediateKeyPickup : MonoBehaviour
{
    #region Unity Methods

    /// <summary>
    /// Gives the Key to an intermediate agent entering the trigger.
    /// </summary>
    /// <param name="other">The collider entering the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        Complete_Day15_IntermediateAgentContext context = other.GetComponentInParent<Complete_Day15_IntermediateAgentContext>();

        if (context == null)
        {
            return;
        }

        context.HasKey = true;
        gameObject.SetActive(false);
    }

    #endregion
}
