
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Smarteye.AR
{
    public class VirtualObjectHandler : MonoBehaviour
    {
        [Header("Component Reference")]
        [SerializeField] private List<Animator> characters;
        [SerializeField] private ParticleSystem vfx;
        [SerializeField] private GameObject messageParent;
        [SerializeField] private TextMeshProUGUI messageText; //! ganti dengan textmeshpro

        public bool isShowFinalAnimation;

        private void OnEnable()
        {
            if (isShowFinalAnimation)
            {
                ShowFinalAnimation();
            }
        }

        public void UpdateCharacterAnimation(float _arg)
        {
            foreach (Animator item in characters)
            {
                item.SetFloat("animationProgress", _arg);
            }
        }

        void Start()
        {
            ResetDefault();
        }

        public void ShowFinalAnimation()
        {
            UpdateCharacterAnimation(1f);
            ShowVFX();
        }

        public void ResetDefault()
        {
            vfx.Stop();
            messageParent.gameObject.SetActive(false);
            isShowFinalAnimation = false;
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