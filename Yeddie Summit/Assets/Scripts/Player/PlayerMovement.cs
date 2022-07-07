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
        
        }

        void FixedUpdate()
        {
            transform.Translate(new Vector3(moveVal.x, 0f, moveVal.y) * moveSpeed);
        }

        #endregion
    }
}
