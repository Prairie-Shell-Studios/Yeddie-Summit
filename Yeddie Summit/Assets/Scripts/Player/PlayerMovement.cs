using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrairieShellStudios.Player
{
    /// <summary>
    /// Uses the new Unity Input System to control the players movement.
    /// Current implementation allows the player to move, jump, and sprint.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        #region fields

        [Header("Movement")]
        private Vector2 moveVal;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float moveSpeed;

        [Header("Sprinting")]
        [SerializeField] private float sprintSpeed;
        private bool isSprinting = false;

        [Header("Jumping")]
        private bool isGrounded = false;

        #endregion

        #region actions

        void OnJump(InputValue value)
        {

        }

        void OnMove(InputValue value)
        {
            moveVal = value.Get<Vector2>();
        }

        void OnSprint(InputValue value)
        {
            isSprinting = value.Get<float>() > 0f;
        }

        #endregion

        #region monobehaviour
        
        void Start()
        {
            ChangeSpeed(walkSpeed);
        }

        void FixedUpdate()
        {
            HandleSprinting();

            transform.Translate(new Vector3(moveVal.x, 0f, moveVal.y) * moveSpeed);
        }

        #endregion

        #region utility

        /// <summary>
        /// Change the speed of the player.
        /// If a negative value is provided, it will be inverted.
        /// </summary>
        /// <param name="newSpeed">A new float value for the players' movement speed.</param>
        public void ChangeSpeed(float newSpeed)
        { 
            moveSpeed = (newSpeed < 0) ? -newSpeed : newSpeed;
        }

        private void HandleSprinting()
        {
            // handles sprinting
            if (isSprinting && moveSpeed != sprintSpeed)
            {
                ChangeSpeed(sprintSpeed);
            }
            else if (!isSprinting && moveSpeed != walkSpeed)
            {
                ChangeSpeed(walkSpeed);
            }
        }

        #endregion
    }
}
