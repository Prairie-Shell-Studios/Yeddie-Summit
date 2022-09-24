using PrairieShellStudios.Status;
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
    public class ThirdPersonPlayerController : MonoBehaviour
    {
        #region fields

        private CharacterController controller;
        private Transform camTransform;

        [Header("Movement")]
        private Vector2 moveVal;
        private float sprintVal;
        [SerializeField] float turnSmoothTime = 0.1f;
        private float turnSmoothVelocity;
        [SerializeField] private float moveSpeed;
        [SerializeField]
        private Dictionary<SpeedState, float> speeds = new Dictionary<SpeedState, float>()
        {
            { SpeedState.Slow, 1f },
            { SpeedState.Normal, 3f},
            { SpeedState.Fast, 9f}
        };
        private bool isSprinting = false;
        private bool isExhausted = false;

        [Header("Jumping")]
        [SerializeField] private float jumpHeight = 10f;
        [SerializeField] private float maxFallingVelocity = 45f;
        private Vector3 playerVelocity;
        private bool isGrounded = false;
        private bool isJumping = false;
        private float gravityValue = -9.81f;

        [Header("Status")]
        [SerializeField] private StatusScriptableObject stamina;


        #endregion

        #region actions

        void OnJump()
        {
            isJumping = true;
        }

        void OnMove(InputValue value)
        {
            moveVal = value.Get<Vector2>();
        }

        void OnSprint(InputValue value)
        {
            sprintVal = value.Get<float>();
        }

        #endregion

        #region monobehaviour

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            camTransform = Camera.main.transform;
            moveSpeed = speeds[SpeedState.Normal];
        }

        void FixedUpdate()
        {
            HandlePlayerVelocity();
            
            HandleSprinting();

            HandleMovement();

            HandleJumping();
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

        /// <summary>
        /// Reset the player velocity if they are grounded.
        /// </summary>
        private void HandlePlayerVelocity()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
        }

        /// <summary>
        /// Handle player jumping when they are grounded.
        /// Current implementation has no clamped velocity when player is falling.
        /// </summary>
        private void HandleJumping()
        {
            if (isJumping && isGrounded)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            playerVelocity.y = Mathf.Clamp(playerVelocity.y, -maxFallingVelocity, maxFallingVelocity);
            controller.Move(playerVelocity * Time.deltaTime);
            isJumping = false;
        }

        /// <summary>
        /// Move the player based on the camera and player input.
        /// </summary>
        private void HandleMovement()
        {
            Vector3 direction = new Vector3(moveVal.x, 0f, moveVal.y);

            // handles player rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // handles player movement
            if (direction.magnitude >= 0.1f)
            {
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            }

        }

        /// <summary>
        /// Increase the player speed when the proper input is used.
        /// </summary>
        private void HandleSprinting()
        {
            isSprinting = sprintVal > 0f && isGrounded && !isExhausted && (moveVal.x > 0f || moveVal.y > 0f);

            // handles sprinting
            if (isSprinting && moveSpeed != speeds[SpeedState.Fast])
            {
                ChangeSpeed(speeds[SpeedState.Fast]);
                stamina.Behaviour = StatusBehaviour.Degrade; // use stamina 
            }
            else if (!isExhausted && !isSprinting && moveSpeed != speeds[SpeedState.Normal])
            {
                ChangeSpeed(speeds[SpeedState.Normal]);
                stamina.Behaviour = StatusBehaviour.Regen; // restore stamina
            }
        }

        #endregion

        #region api

        /// <summary>
        /// Called when the stamina status either reaches full or empty.
        /// Makes the player extra slow when they run out of stamina and remain that way until the stamina
        /// reaches max.
        /// </summary>
        /// <param name="exhausted">A bool that determines if the player is exhausted or not.</param>
        public void IsExhausted(bool exhausted)
        {
            isExhausted = exhausted;
            moveSpeed = isExhausted ? speeds[SpeedState.Slow] : speeds[SpeedState.Normal];

            // start regenerating stamina once exhausted
            if (isExhausted)
            {
                stamina.Behaviour = StatusBehaviour.Regen;
            }
        }

        #endregion
    }

    public enum SpeedState { Slow, Normal, Fast };
}
