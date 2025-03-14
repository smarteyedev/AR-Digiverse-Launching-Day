using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Smarteye.AR
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private List<PanelItem> panels;

        public void ControllerShowPanel(int panelIndex)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].panelHandler.gameObject.activeSelf)
                {
                    panels[i].onPanelClose?.Invoke();
                }
            }

            foreach (var panel in panels)
            {
                panel.panelHandler.PanelVisibility(false);
            }

            if (panelIndex >= 0 && panelIndex < panels.Count)
            {
                panels[panelIndex].panelHandler.PanelVisibility(true);
                panels[panelIndex].onPanelOpen?.Invoke();
            }
        }

        public void HideStartPanel()
        {
            panels[3].panelHandler.PanelVisibility(false);
        }

        [Serializable]
        public class PanelItem
        {
            public PanelHandler panelHandler;

            [Space(10f)]
            [Header("Unity Event")]
            public UnityEvent onPanelOpen;
            [Space(2f)]
            public UnityEvent onPanelClose;
        }

        private void Start()
        {
            ControllerShowPanel(0);
        }
    }
}