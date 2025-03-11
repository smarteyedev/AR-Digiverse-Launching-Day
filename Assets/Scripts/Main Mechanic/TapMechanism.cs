using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Berikut kegunaan dari class Tap Mechanism :
///  -> fungsi tapping dengan membaca pointer down & up , sekaligus menambahkan value progress yang akan menambahkan slider value
///  -> fungsi untuk menggerakkan animasi character sesuai dengan nilai progress,
///  -> time countdown pada saat pertama kali memulai melakukan aksi
/// </summary>

public class TapMechanism : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool m_isCanTapping = false;
    private bool m_isTapping = false;
    private float m_currentProgressValue = 0f;
    private float m_countdownTime = 0f;
    private float m_countdownDuration = 3f;

    [Header("Configuration")]
    [Range(0f, 1f)]
    public float increaseSpeed = 0.3f;
    [Range(0f, 1f)]
    public float decreaseSpeed = 0.1f;
    private float m_maxProgressValue = 1f;

    [Header("Component Reference")]
    public Slider progressSlider;
    public Animator characterAnimator;
    public Text countdownText; //! should change to TMPro
    public Text instructionText; //! should change to TMPro
    public Text ctaText; //! should change to TMPro

    [Header("Unity Event")]
    [Space(5)]
    public UnityEvent OnTapStart;
    public UnityEvent OnTapFinish;
    public UnityEvent<float> progressValue;

    [Space(15f)]
    [Header("Progress Message")]
    public List<ProgressMessage> progressMessages;

    [System.Serializable]
    public class ProgressMessage
    {
        [TextArea]
        public string message;
        public Condition condition;
        public enum Condition
        {
            OnIncrease, OnDecrease
        }
        public float targetProgressValue;
        public UnityEvent onProgressReached;
    }

    void Start()
    {
        if (progressSlider)
        { progressSlider.value = m_currentProgressValue; }
    }

    private float m_timeSinceLastTap = 0f;
    private float m_idleThreshold = 3f;

    void Update()
    {
        if (m_isCanTapping == false) return;

        if (m_isTapping)
        {
            m_currentProgressValue = Mathf.MoveTowards(m_currentProgressValue, m_maxProgressValue, increaseSpeed * Time.deltaTime);

            CheckProgressMessages(ProgressMessage.Condition.OnIncrease);

            if (progressSlider.value >= m_maxProgressValue)
            {
                OnTapFinish?.Invoke();
                m_isCanTapping = false;

                SetUIActive(false);

                Debug.Log($"player reached max value");
            }
        }
        else
        {
            m_currentProgressValue = Mathf.MoveTowards(m_currentProgressValue, 0f, decreaseSpeed * Time.deltaTime);

            // Timer untuk cek waktu idle
            m_timeSinceLastTap += Time.deltaTime;

            if (m_timeSinceLastTap >= m_idleThreshold)
            {
                CheckProgressMessages(ProgressMessage.Condition.OnDecrease);
            }

            Debug.Log(m_timeSinceLastTap);
        }

        if (progressSlider)
        { progressSlider.value = m_currentProgressValue; }

        if (characterAnimator)
        { characterAnimator.SetFloat("animationProgress", m_currentProgressValue); }

        if (Mathf.Abs(m_currentProgressValue - progressSlider.value) > 0.01f)
        {
            progressValue?.Invoke(m_currentProgressValue);
        }
    }

    private void CheckProgressMessages(ProgressMessage.Condition condition)
    {
        foreach (var progressMessage in progressMessages)
        {
            if (progressMessage.condition == condition && Mathf.Abs(m_currentProgressValue - progressMessage.targetProgressValue) <= 0.01f)
            {
                // Debug.Log(progressMessage.message);
                ctaText.text = $"{progressMessage.message}";
                progressMessage.onProgressReached?.Invoke();
            }
        }
    }

    // Fungsi untuk memulai tapping dengan countdown
    public void StartTapping()
    {
        if (m_isCanTapping) return;

        StartCoroutine(CountdownAndStartTapping());
    }

    // Coroutine untuk menampilkan countdown 3, 2, 1 sebelum memulai tapping
    private IEnumerator CountdownAndStartTapping()
    {
        if (countdownText)
            countdownText.gameObject.SetActive(true);

        m_countdownTime = m_countdownDuration;

        for (int i = (int)m_countdownTime; i > 0; i--)
        {
            // Debug.Log($"countdown : {i}");

            if (countdownText)
            { countdownText.text = $"{i}"; }
            yield return new WaitForSeconds(1f);
        }

        m_isCanTapping = true;
        OnTapStart?.Invoke();

        if (countdownText)
            countdownText.gameObject.SetActive(false);

        SetUIActive(true);
    }

    private void SetUIActive(bool isActive)
    {
        if (instructionText)
        {
            instructionText.gameObject.SetActive(isActive);  // Hanya aktifkan messageText saat dibutuhkan
        }

        if (progressSlider)
        {
            progressSlider.gameObject.SetActive(isActive);  // Hanya aktifkan progressSlider saat dibutuhkan
        }
    }

    public void ResetProgress()
    {
        // m_isCanTapping = false; //! reset default, if needed

        m_currentProgressValue = 0;
        characterAnimator.SetFloat("animationProgress", m_currentProgressValue);
        progressSlider.value = m_currentProgressValue;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_timeSinceLastTap = 0f;

        m_isTapping = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isTapping = false;
    }
}
