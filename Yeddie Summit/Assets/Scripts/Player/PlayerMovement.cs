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
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        #region fields

        private CharacterController controller;
        public Transform cam;

        [Header("Movement")]
        private Vector2 moveVal;
        [SerializeField] float turnSmoothTime = 0.1f;
        private float turnSmoothVelocity;
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

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void Start()
        {
            ChangeSpeed(walkSpeed);
        }

        void FixedUpdate()
        {
            HandleSprinting();

            HandleMovement();
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

        private void HandleJumping()
        {

        }

        private void HandleMovement()
        {
            Vector3 direction = new Vector3(moveVal.x, 0f, moveVal.y);

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            }
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
