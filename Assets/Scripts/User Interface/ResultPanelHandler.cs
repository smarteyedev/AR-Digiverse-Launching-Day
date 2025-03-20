using System;
using TMPro;
using UnityEngine;

namespace Smarteye.AR
{
    public class ResultPanelHandler : PanelHandlerBase
    {
        [Range(3f, 10f)]
        [SerializeField] private float delayButtonNext = 7f;

        [Header("Additional Reference")]
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private GameObject screenshootMessage;
        [SerializeField] private GameObject buttonNext;

        public void ShowResultSuccess(string message)
        {
            resultText.text = message;
            screenshootMessage.SetActive(true);
            buttonNext.SetActive(false);

            Invoke(nameof(ShowNextButton), delayButtonNext);
        }

        public void ShowResultFail(string message)
        {
            resultText.text = message;
            buttonNext.SetActive(true);
        }

        private void ShowNextButton()
        {
            buttonNext.SetActive(true);
            screenshootMessage.SetActive(false);
        }
    }
}