using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Smarteye.AR
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private List<PanelItem> panels;

        public void ShowPanel(int panelIndex)
        {
            foreach (var panel in panels)
            {
                panel.panelHandler.PanelVisibility(false);
            }

            if (panelIndex >= 0 && panelIndex < panels.Count)
            {
                panels[panelIndex].panelHandler.PanelVisibility(true);
            }
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
    }
}