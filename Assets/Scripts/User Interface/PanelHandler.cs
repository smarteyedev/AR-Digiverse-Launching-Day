using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smarteye.AR
{
    public class PanelHandler : MonoBehaviour
    {
        public void PanelVisibility(bool conn)
        {
            this.gameObject.SetActive(conn);
        }
    }
}