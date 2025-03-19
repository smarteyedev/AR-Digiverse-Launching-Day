using DG.Tweening;
using UnityEngine;

namespace Smarteye.AR
{
    public class StartPanelHandler : PanelHandlerBase
    {
        [Header("UI Animation")]
        // Object pertama (UI)
        [SerializeField] private RectTransform objectOne;
        [SerializeField] private Vector2 startPositionObjectOne;
        [SerializeField] private RectTransform targetPositionOne;
        [SerializeField] private float durationOne = 1f;

        // Object kedua (UI)
        [SerializeField] private RectTransform objectTwo;
        [SerializeField] private float durationTwo = 1f;

        // Object ketiga (UI)
        [SerializeField] private RectTransform objectThree;
        [SerializeField] private float durationThree = 1f;

        private Sequence sequence;

        [Space(10f)]
        [SerializeField] private RectTransform targetArea;
        [SerializeField] private RectTransform iconParent;
        [SerializeField] private int spawnCount = 5;

        void OnEnable()
        {
            PlayAnimation(true);
        }

        void OnDisable()
        {
            // Hentikan semua animasi jika objek dinonaktifkan
            if (sequence != null)
            {
                sequence.Kill();
            }
        }

        public void PlayAnimation(bool isLoop)
        {
            if (sequence != null)
            {
                sequence.Kill();
            }

            sequence = DOTween.Sequence();

            objectOne.anchoredPosition = startPositionObjectOne;
            objectOne.localScale = Vector3.one;

            sequence.Append(objectOne.DOAnchorPos(targetPositionOne.anchoredPosition, durationOne).SetEase(Ease.OutBack));
            sequence.Append(objectOne.DOScale(0.5f, durationOne * 0.5f).SetEase(Ease.InOutQuad));
            sequence.Append(objectOne.DOScale(1f, durationOne * 0.5f).SetEase(Ease.InOutQuad));

            sequence.AppendCallback(() =>
            {
                objectTwo.localScale = Vector3.zero;
            });
            sequence.Append(objectTwo.DOScale(1f, durationTwo).SetEase(Ease.OutBack));

            sequence.AppendCallback(() =>
            {
                objectThree.localScale = Vector3.zero;
            });
            sequence.Append(objectThree.DOScale(1f, durationThree).SetEase(Ease.OutBack));

            if (isLoop)
            {
                sequence.AppendInterval(1f);
                sequence.AppendCallback(() =>
                {
                    ResetObjects();

                    Vector2 randomPosition = GetRandomPositionInArea();
                    iconParent.anchoredPosition = randomPosition;
                });

                sequence.SetLoops(-1, LoopType.Restart);
            }
        }

        void ResetObjects()
        {
            // Kembalikan semua objek ke posisi & skala awal
            objectOne.anchoredPosition = startPositionObjectOne;
            objectOne.localScale = Vector3.one;

            objectTwo.localScale = Vector3.zero;
            objectThree.localScale = Vector3.zero;
        }

        private Vector2 GetRandomPositionInArea()
        {
            float width = targetArea.rect.width;
            float height = targetArea.rect.height;

            float randomX = Random.Range(-width / 2, width / 2);
            float randomY = Random.Range(-height / 2, height / 2);

            return new Vector2(randomX, randomY);
        }
    }
}