using UnityEngine;
using UnityEngine.Events;

namespace PrairieShellStudios.EventSystemSO
{
    public class GameEventListener : MonoBehaviour
    {
        #region fileds

        [SerializeField] private GameEventScriptableObject _event;
        [SerializeField] private UnityEvent _response;

        #endregion

        #region monobehaviour

        private void OnEnable()
        {
            _event.RegisterListener(this);
        }

        private void OnDisable()
        {
            _event.UnregisterListener(this);
        }

        #endregion

        #region api

        public void OnEventRaised()
        {
            _response.Invoke();
        }

        #endregion
    }
}
