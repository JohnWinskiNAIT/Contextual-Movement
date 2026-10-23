using UnityEngine;

public class Complete_Day15_CoinPickup : MonoBehaviour
{
    #region Unity Methods

    /// <summary>
    /// Disables the Coin when an intermediate agent enters its trigger.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        Complete_Day15_IntermediateAgentContext context = other.GetComponentInParent<Complete_Day15_IntermediateAgentContext>();

        if (context == null)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    #endregion
}
