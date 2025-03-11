using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TapMechanism : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool m_isCanTapping = false;
    private bool m_isTapping = false;
    private float m_currentProgressValue = 0f;

    [Header("Configuration")]
    [Range(0f, 1f)]
    public float increaseSpeed = 0.3f;
    [Range(0f, 1f)]
    public float decreaseSpeed = 0.1f;
    private float m_maxProgressValue = 1f;

    [Header("Component Reference")]
    public Slider slider;
    public Animator animator;

    [Header("Unity Event")]
    [Space(5)]
    public UnityEvent OnTapStart;
    public UnityEvent<float> progressValue;
    public UnityEvent OnTapFinish;

    void Start()
    {
        slider.value = m_currentProgressValue;
    }

    void Update()
    {
        if (m_isCanTapping == false) return;

        if (m_isTapping)
        {
            m_currentProgressValue = Mathf.MoveTowards(m_currentProgressValue, m_maxProgressValue, increaseSpeed * Time.deltaTime);

            if (slider.value >= m_maxProgressValue)
            {
                OnTapFinish?.Invoke();
                m_isCanTapping = false;

                Debug.Log($"player reached max value");
            }
        }
        else
        {
            m_currentProgressValue = Mathf.MoveTowards(m_currentProgressValue, 0f, decreaseSpeed * Time.deltaTime);
        }

        if (slider)
        { slider.value = m_currentProgressValue; }

        if (animator)
        { animator.SetFloat("animationProgress", m_currentProgressValue); }

        progressValue?.Invoke(m_currentProgressValue);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isTapping = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isTapping = false;
    }

    public void StartTapping()
    {
        m_isCanTapping = true;
        OnTapStart?.Invoke();
    }
}
