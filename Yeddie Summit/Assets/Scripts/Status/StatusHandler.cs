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
        private Dictionary<string, StatusScriptableObject> statusScriptables = 
            new Dictionary<string, StatusScriptableObject>();

        #endregion

        #region monobehaviour

        // Start is called before the first frame update
        void Start()
        {
            foreach (KeyValuePair<string, StatusScriptableObject> status in statusScriptables)
            {
                status.Value.Init();
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach(KeyValuePair<string, StatusScriptableObject> status in statusScriptables)
            {
                status.Value.HandleBehaviour();
            }
        }

        #endregion

        #region api

        public StatusScriptableObject GetStatus(string name)
        {
            return statusScriptables[name];
        }

        #endregion
    }

}