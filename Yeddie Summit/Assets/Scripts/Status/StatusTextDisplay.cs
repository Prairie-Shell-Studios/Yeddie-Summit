using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PraireShellStudios.Status
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
            UpdateText();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateText();
        }

        #endregion

        #region utility

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