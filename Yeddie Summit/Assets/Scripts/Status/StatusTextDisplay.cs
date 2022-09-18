using TMPro;
using UnityEngine;

namespace PrairieShellStudios.Status
{
    /// <summary>
    /// Display the current value of a status in a TMPro element.
    /// Connects a StatusScriptableObject with a TMPro element.
    /// </summary>
    public class StatusTextDisplay : MonoBehaviour
    {
        #region fields

        private TextMeshProUGUI textMesh;
        public StatusScriptableObject status;

        #endregion

        #region monobehaviour

        public void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        // Start is called before the first frame update
        void Start()
        {
            CheckDependencies();
            UpdateText();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateText();
        }

        #endregion

        #region utility

        private void CheckDependencies()
        {
            if (textMesh == null || status == null)
            {
                Debug.LogWarning("StatusTextDisplay: Either text or status have not been assigned.");
            }
        }

        private void UpdateText()
        {
            if (textMesh != null && status != null)
            {
                textMesh.text = status.Current.ToString();
            }
        }

        #endregion
    }

}