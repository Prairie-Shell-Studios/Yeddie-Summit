using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PraireShellStudios.Status
{
    /// <summary>
    /// Handle the updates for each status in list.
    /// Also contains the status' in a dictionary for easy retrieval.
    /// </summary>
    public class StatusHandler : MonoBehaviour
    {
        #region fields

        [SerializeField]
        private List<StatusScriptableObject> statusScriptables = new List<StatusScriptableObject>();

        #endregion

        #region monobehaviour

        // Start is called before the first frame update
        void Start()
        {
            foreach (StatusScriptableObject status in statusScriptables)
            {
                status.Init();
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach(StatusScriptableObject status in statusScriptables)
            {
                status.HandleBehaviour();
            }
        }

        #endregion

        #region api

        public StatusScriptableObject GetStatus(string name)
        {
            foreach(StatusScriptableObject status in statusScriptables)
            {
                if (status.StatusName == name)
                {
                    return status;
                }
            }

            return null;
        }

        #endregion
    }

}