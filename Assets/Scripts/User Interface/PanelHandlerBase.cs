using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Smarteye.AR
{
    public class PanelHandlerBase : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;
        public float fadeDuration = 0.5f;

        public virtual void PanelVisibility(bool conn)
        {
            this.gameObject.SetActive(conn);
        }

        public virtual void FadeIn()
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeDuration)
                    .SetEase(Ease.OutQuad)
                    .OnStart(() => canvasGroup.blocksRaycasts = true);
        }

        public virtual void FadeOut()
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => canvasGroup.blocksRaycasts = false);
        }
    }
}