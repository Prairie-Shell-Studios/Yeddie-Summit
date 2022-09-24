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
        [SerializeField] float turnSmoothTime = 0.1f;
        private float turnSmoothVelocity;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float sprintSpeed;
        private bool isSprinting = false;

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
            isSprinting = value.Get<float>() > 0f && isGrounded;
        }

        #endregion

        #region monobehaviour

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            camTransform = Camera.main.transform;
            moveSpeed = walkSpeed;
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
