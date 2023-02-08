using PrairieShellStudios.Status;
using UnityEngine;

namespace PrairieShellStudios.Player
{
    /// <summary>
    /// Controls the player health.
    /// </summary>
    public class PlayerHealthController : MonoBehaviour
    {
        #region fields
        private StatusHandler statusHandler;
        private StatusScriptableObject health;
        [SerializeField] private LayerMask damageMask;
        #endregion

        #region monobehaviour

        private void Awake() 
        {
            statusHandler = gameObject.GetComponent<StatusHandler>();
            if (statusHandler != null) 
            {
                health = statusHandler.GetStatus("PlayerHealth");
            }
            else if (health == null)
            {
                Debug.Log("Could not retreive PlayerHealth, please attach a StatusHandler to the player.");
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("collided");
            if (IsInLayerMask(other.gameObject, damageMask))
            {
                // player takes damage
                // get damage value from collision
                Debug.Log("damage dealt");
                health.ChangeCurrent(-10);
            }
        }

        #endregion

        #region utils

        private bool IsInLayerMask(GameObject checkGO, LayerMask mask)
        {
            // TODO: Fix to check a LayerMask with multiple layers
            // TODO: Move to a utility class.
            return (mask & (1 << checkGO.layer)) != 0;
        }

        #endregion
    }
}