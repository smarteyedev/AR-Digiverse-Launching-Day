using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TapMechanism : MonoBehaviour, IPointerClickHandler
{
    private bool isCanTap = false;

    [Header("Slider Config")]
    public Slider slider;
    public float maxSliderValue = 15f;
    public float incrementValue = 1f;

    [Header("Difficulty Config")]
    [Range(1, 3)]
    public float difficulty = 1f;
    [Range(1, 3)]
    public float decreaseRate = 1f; // Kecepatan pengurangan nilai slider per detik
    public float decreaseDelay = .3f;

    public UnityAction OnTapStart;
    public UnityAction OnTapFinish;

    private float lastTapTime;

    private void Awake()
    {
        slider.maxValue = maxSliderValue;
    }

    void Start()
    {
        slider.value = 0f;
        lastTapTime = Time.time;

        //! StartTapping();
    }

    void Update()
    {
        if (!isCanTap) return;

        if (Time.time - lastTapTime >= decreaseDelay)
        {
            float decreaseAmount = decreaseRate * difficulty * Time.deltaTime;
            slider.value = Mathf.Max(slider.value - decreaseAmount, 0);
        }

        if (slider.value >= maxSliderValue)
        {
            OnTapFinish?.Invoke();

            Debug.Log($"on score reached {slider.value}");
            isCanTap = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isCanTap) return;

        if (slider.value < maxSliderValue)
        {
            slider.value += incrementValue;
        }

        lastTapTime = Time.time;
    }

    public void StartTapping()
    {
        isCanTap = true;
        OnTapStart?.Invoke();
    }
}
