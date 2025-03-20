using DG.Tweening;
using UnityEngine;

namespace Smarteye.AR
{
    public class TutorPanelHandler : PanelHandlerBase
    {
        [Header("UI Animation")]
        // Object pertama (UI)
        [SerializeField] private RectTransform objectOne;
        [SerializeField] private RectTransform targetPositionOne;
        [SerializeField] private Vector2 startPositionObjectOne;
        [SerializeField] private float durationOne = 1f;

        [Space(5f)]
        // Object kedua (UI)
        [SerializeField] private RectTransform objectTwo;
        [SerializeField] private RectTransform targetPositionTwo;
        [SerializeField] private Vector2 startPositionObjectTwo;
        [SerializeField] private float durationTwo = 1.5f;

        private Sequence sequence;

        void OnEnable()
        {
            PlayAnimation(true);
        }

        void OnDisable()
        {
            // Hentikan semua animasi pada objectOne dan objectTwo
            if (sequence != null)
            {
                sequence.Kill();
            }
        }

        public void PlayAnimation(bool isLoop)
        {
            objectOne.anchoredPosition = startPositionObjectOne;
            objectOne.localScale = Vector3.one * 0.5f;

            sequence = DOTween.Sequence();

            sequence.Append(objectOne.DOAnchorPos(targetPositionOne.anchoredPosition, durationOne).SetEase(Ease.OutBack));
            sequence.Join(objectOne.DOScale(Vector3.one, durationOne).SetEase(Ease.OutBack));

            sequence.AppendCallback(() =>
            {
                objectTwo.anchoredPosition = startPositionObjectTwo;

                objectTwo.DOAnchorPos(targetPositionTwo.anchoredPosition, durationTwo).SetEase(Ease.OutQuad);
            });

            if (isLoop)
            {
                sequence.AppendInterval(2f);
                sequence.AppendCallback(() =>
                {
                    objectOne.anchoredPosition = startPositionObjectOne;
                    objectOne.localScale = Vector3.one * 0.5f;
                    objectTwo.anchoredPosition = startPositionObjectTwo;
                });

                sequence.SetLoops(-1, LoopType.Restart);
            }
        }
    }
}