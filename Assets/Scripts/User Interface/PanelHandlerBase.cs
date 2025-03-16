using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Smarteye.AR
{
    public class PanelHandlerBase : MonoBehaviour
    {
        [SerializeField] protected float fadeInDuration = 0.5f;
        [SerializeField] protected float fadeOutDuration = 0.5f;
        [SerializeField] protected float StretchInDuration = 0.5f;
        [SerializeField] protected float StretchOutDuration = 0.5f;

        [Header("Component Reference")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected RectTransform popupTransform;

        public virtual void PanelVisibility(bool conn)
        {
            if (conn)
            {
                this.gameObject.SetActive(true);
                canvasGroup.alpha = 0;
                FirstOpenPanel();
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        protected void FirstOpenPanel()
        {
            if (popupTransform != null)
            {
                popupTransform.localScale = new Vector3(1, 0, 1);
            }
            FadeInPanel(StretchInPopup);
        }

        protected virtual void FadeInPanel(Action nextAction = null)
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeInDuration)
                    .SetEase(Ease.OutQuad)
                    .OnStart(() =>
                    {
                        canvasGroup.blocksRaycasts = true;
                    })
                    .OnComplete(() =>
                    {
                        nextAction?.Invoke();
                    });
        }

        protected virtual void FadeOutPanel()
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeOutDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => canvasGroup.blocksRaycasts = false);
        }

        protected virtual void StretchInPopup()
        {
            if (popupTransform == null) return;

            popupTransform.DOScaleY(1, StretchInDuration)
                .SetEase(Ease.OutBack);
        }

        protected virtual void StretchOutPopup()
        {
            if (popupTransform == null) return;

            popupTransform.DOScaleY(0, StretchOutDuration)
                .SetEase(Ease.InBack);
        }
    }
}