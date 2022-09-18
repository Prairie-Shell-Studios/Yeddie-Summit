using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrairieShellStudios.Status
{
    /// <summary>
    /// Display the values of a StatusScriptableObject in a Bar Slider element.
    /// </summary>
    public class StatusSliderDisplay : MonoBehaviour
    {
        #region fields

        private Slider slider;
        public StatusScriptableObject status;
        [SerializeField]
        private float delay = 2f;

        #endregion

        #region monobehaviour

        public void Awake()
        {
            slider = GetComponent<Slider>();
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSlider();
        }

        #endregion

        #region utility

        private void Init()
        {
            if (slider != null && status != null)
            {
                slider.maxValue = status.Max;
                slider.minValue = status.Min;
                slider.value = status.Current;
            }
            else
            {
                Debug.LogWarning("StatusSliderDisplay: Either slider or status have not been assigned.");
            }
        }

        private void UpdateSlider()
        {
            if (slider != null && status != null)
            {
                slider.maxValue = status.Max;
                slider.minValue = status.Min;
                slider.value = Mathf.Lerp(slider.value, status.Current, Time.deltaTime * delay);
            }
        }

        #endregion
    }

}