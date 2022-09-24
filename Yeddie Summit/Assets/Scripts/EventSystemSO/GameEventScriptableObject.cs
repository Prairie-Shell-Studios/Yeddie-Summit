using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios.EventSystemSO
{
    /// <summary>
    /// Creates an event that other scripts may subscribe too.
    /// </summary>
    [CreateAssetMenu(fileName = "Event", menuName = "ScriptableObjects/Event", order = 2)]
    public class GameEventScriptableObject : ScriptableObject
    {
        #region fields

        private List<GameEventListener> listeners = new List<GameEventListener>();

        #endregion

        #region api

        public void Raise()
        {
            for(int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
        #endregion

    }
}
