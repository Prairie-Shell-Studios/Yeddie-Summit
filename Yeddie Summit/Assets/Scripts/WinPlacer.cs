using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios
{
    public class WinPlacer : MonoBehaviour
    {
        #region fields
        public LayerMask spawnMask;
        public float rayCastLength = 0f;
        public float delay = 0.5f;
        #endregion

        #region monobehaviour

        void Start()
        {
            StartCoroutine(LateStart());
        }

        // Start is called before the first frame update
        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(delay);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayCastLength, spawnMask))
            {
                transform.position = hit.point;
            }
        }
        #endregion

    }
}
