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
        public float mass = 1.0f;
        public float damageModifier = 30f;
        public float knockbackModifier = 25f;
        [SerializeField] private float knockbackThreshold = 0.2f;
        [SerializeField] private float knockbackDeteriorationRate = 2.5f;
        private Vector3 impact = Vector3.zero;
        private CharacterController controller;
        [SerializeField] private LayerMask damageMask;
        #endregion

        #region monobehaviour

        private void Awake() 
        {
            controller = gameObject.GetComponent<CharacterController>();
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

        private void Update() 
        {
            // handles knockback deterioration
            if (impact.magnitude > knockbackThreshold) 
            {
                controller.Move(impact * Time.deltaTime);
            }
            impact = Vector3.Lerp(impact, Vector3.zero, knockbackDeteriorationRate * Time.deltaTime);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (IsInLayerMask(other.gameObject, damageMask))
            {
                float otherScale = other.transform.localScale.x;
                int damage = (int) -Mathf.Round(otherScale * damageModifier);
                float force = otherScale * knockbackModifier; 
                health.ChangeCurrent(damage);
                Vector3 hitVector = transform.position - other.transform.position;
                AddImpact(hitVector.normalized, force);
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

        private void AddImpact(Vector3 direction, float force)
        {
            // add knockback
            if (direction.y < 0f)
            {
                direction.y = -direction.y;
            }
            impact += (direction * force / mass);
        }

        #endregion
    }
}