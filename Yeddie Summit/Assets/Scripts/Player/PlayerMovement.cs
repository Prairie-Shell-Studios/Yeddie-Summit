using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrairieShellStudios.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        #region fields

        [Header("Movement")]
        private Vector2 moveVal;
        [SerializeField] private float moveSpeed;
        [SerializeField] private InputActionReference moveAction;

        #endregion

        #region movement

        private void OnMove(InputValue value)
        {
            moveVal = value.Get<Vector2>();
        }

        #endregion

        #region monobehaviour
        
        void Start()
        {
            ChangeSpeed(moveSpeed);
        }

        void FixedUpdate()
        {
            transform.Translate(new Vector3(moveVal.x, 0f, moveVal.y) * moveSpeed);
        }

        #endregion

        #region utility

        /// <summary>
        /// Change the speed of the player by modifying the Scale Processor for the Move action.
        /// If a negative value is provided, it will be inverted.
        /// </summary>
        /// <param name="newSpeed">A new float value for the players' movement speed.</param>
        public void ChangeSpeed(float newSpeed)
        {
            moveSpeed = (newSpeed < 0) ? -newSpeed : newSpeed;
            string newFactor = "scale(factor=" + moveSpeed + ")";
            moveAction.action.ApplyBindingOverride(new InputBinding { overrideProcessors = newFactor });
        }

        #endregion
    }
}
