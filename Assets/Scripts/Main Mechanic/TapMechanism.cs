using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

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
    [SerializeField] private float increaseSpeed = 0.3f;
    [Range(0f, 1f)]
    [SerializeField] private float decreaseSpeed = 0.1f;
    private float m_maxProgressValue = 1f;

    [Space(5f)]
    [SerializeField] bool isUsingOverlayCanvasMessage = false;

    [Header("Component Reference")]
    [SerializeField] private Slider progressSlider;
    private float m_lowerSliderValue = 0.1f;
    // [SerializeField] private Animator characterAnimator;
    [SerializeField] private Text countdownText; //! should change to TMPro
    [SerializeField] private Text instructionText; //! should change to TMPro
    private Text ctaText; //! should change to TMPro
    [SerializeField] private GameObject ctaPlank;
    private VirtualObjectHandler currentObject;

    [Header("Unity Event")]
    [Space(5)]
    public UnityEvent OnTapStart;
    public UnityEvent OnTapFinish;
    public UnityEvent<float> progressValue;

    [Space(15f)]
    [Header("Progress Message")]
    [SerializeField] private List<ProgressMessage> progressMessages;

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

    private float m_timeSinceLastTap = 0f;
    private float m_idleThreshold = 3f;
    private float m_holdCooldown = .1f; // Cooldown duration 
    private float m_holdTime = 0f;  // Track time holding the tap

    void Start()
    {
        if (progressSlider) { progressSlider.value = m_currentProgressValue > m_lowerSliderValue ? m_currentProgressValue : m_lowerSliderValue; }

        if (isUsingOverlayCanvasMessage)
        {
            ctaText = ctaPlank.GetComponentInChildren<Text>();
        }
    }

    public void SetupVirtualObject(VirtualObjectHandler vObj)
    {
        currentObject = vObj;

        OnTapFinish.AddListener(vObj.ShowVFX);
    }

    void Update()
    {
        if (m_isCanTapping == false) return;

        if (m_isTapping)
        {
            m_holdTime += Time.deltaTime;
            // Debug.Log($"hold time: {m_holdTime}");

            if (m_holdTime >= m_holdCooldown)
            {
                m_isTapping = false;
                return;
            }

            m_currentProgressValue = Mathf.MoveTowards(m_currentProgressValue, m_maxProgressValue, increaseSpeed * Time.deltaTime);

            CheckProgressMessages(ProgressMessage.Condition.OnIncrease);

            if (progressSlider.value == m_maxProgressValue)
            {
                OnTapFinish?.Invoke();
                m_isCanTapping = false;

                SetTappingUIActive(false);

                m_isCanTapping = false;
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

            // Debug.Log(m_timeSinceLastTap);
        }

        if (progressSlider) { progressSlider.value = m_currentProgressValue > m_lowerSliderValue ? m_currentProgressValue : m_lowerSliderValue; }

        if (currentObject)
        { currentObject.UpdateCharacterAnimation(m_currentProgressValue); }

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
                if (isUsingOverlayCanvasMessage && ctaText)
                {
                    ctaText.text = $"{progressMessage.message}";
                }
                currentObject.ShowMessage(progressMessage.message);
                progressMessage.onProgressReached?.Invoke();
            }
        }
    }

    // Fungsi untuk memulai tapping dengan countdown
    public void StartTapping(Action onFinishCountdownAction)
    {
        if (m_isCanTapping) return;

        StartCoroutine(CountdownAndStartTapping(onFinishCountdownAction));
    }

    // Coroutine untuk menampilkan countdown 3, 2, 1 sebelum memulai tapping
    private IEnumerator CountdownAndStartTapping(Action finishAction)
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

        OnTapStart?.Invoke();

        if (countdownText)
            countdownText.gameObject.SetActive(false);

        SetTappingUIActive(true);

        finishAction.Invoke();
    }

    public void SetTappingUIActive(bool isActive)
    {
        if (instructionText)
        {
            instructionText.gameObject.SetActive(isActive);
        }

        if (progressSlider)
        {
            progressSlider.gameObject.SetActive(isActive);
        }

        if (ctaText && isUsingOverlayCanvasMessage)
        {
            ctaPlank.gameObject.SetActive(isActive);
        }

        m_isCanTapping = isActive;
    }

    public void ResetTappingProgress()
    {
        // m_isCanTapping = false; //! reset default, if needed

        m_currentProgressValue = 0;
        currentObject.UpdateCharacterAnimation(m_currentProgressValue);
        progressSlider.value = m_currentProgressValue;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_holdTime = 0f;

        m_isTapping = true;
        m_timeSinceLastTap = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isTapping = false;

        m_holdTime = 0f;
    }
}
