using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Smarteye.AR
{
    public class VirtualObjectHandler : MonoBehaviour
    {
        [Header("Component Reference")]
        [SerializeField] private List<Animator> characters;
        [SerializeField] private ParticleSystem vfx;
        [SerializeField] private GameObject messageParent;
        [SerializeField] private Text messageText; //! ganti dengan textmeshpro

        public void UpdateCharacterAnimation(float _arg)
        {
            foreach (Animator item in characters)
            {
                item.SetFloat("animationProgress", _arg);
            }
        }

        private void Start()
        {
            messageParent.gameObject.SetActive(false);
            vfx.Stop();
        }

        private void OnDestroy()
        {

        }

        public void ShowVFX()
        {
            vfx.Play();
        }

        public void ShowMessage(string txt)
        {
            messageParent.gameObject.SetActive(true);
            messageText.text = txt;
        }
    }
}