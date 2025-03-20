using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Smarteye.AR
{
    public class QuotesPanelHandler : PanelHandlerBase
    {
        [Header("Configuration")]
        [SerializeField] private int waitingDuration;
        [Header("Additional Reference")]
        [SerializeField] private Button resetButton;
        [SerializeField] private TextMeshProUGUI btnText;

        protected override void FirstOpenPanel()
        {
            base.FirstOpenPanel();

            StartCoroutine(WaitingBtnActivation());
        }

        private IEnumerator WaitingBtnActivation()
        {
            resetButton.interactable = false;

            for (int i = waitingDuration; i > 0; i--)
            {
                btnText.text = $"tunggu {i} detik lagi ...";
                yield return new WaitForSeconds(1f);
            }

            btnText.text = $"Main Lagi";
            resetButton.interactable = true;
        }
    }
}