using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Timer Config")]
    [SerializeField] private float timeDuration = 60f;
    private float m_currentTime = 60f;
    [SerializeField] private bool isTimerRun = false;

    [Header("Component Reference")]
    [SerializeField] private TapMechanism tapMechanism;
    [SerializeField] private Text countdownText;

    [Header("Object Reference")]
    public VirtualObjectHandler virtualObjectPrefab;
    [SerializeField] private Transform spawnLocation;

    [Header("Unity Event")]
    public UnityEvent OnTimerStart;
    public UnityEvent OnTimerFinish;


    private void Start()
    {
        VirtualObjectHandler newObj = Instantiate(virtualObjectPrefab, spawnLocation.position, spawnLocation.rotation);
        tapMechanism.SetupVirtualObject(newObj);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tapMechanism.StartTapping(() => StartTimer());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            tapMechanism.ResetTappingProgress();
            ResetTimer();
        }

        if (m_currentTime > 0 && isTimerRun)
        {
            m_currentTime -= Time.deltaTime;

            int hours = Mathf.FloorToInt(m_currentTime / 3600);
            int minutes = Mathf.FloorToInt((m_currentTime % 3600) / 60);
            int seconds = Mathf.FloorToInt(m_currentTime % 60);

            if (countdownText != null)
            {
                if (hours > 0)
                    countdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
                else if (minutes > 0)
                    countdownText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
                else
                    countdownText.text = string.Format("{0:D2}", seconds);
            }
            else
            {
                Debug.Log($"text component is empty, duration is : {m_currentTime}");
            }
        }
        else
        {
            // Timer selesai
            countdownText.text = "00";
            isTimerRun = false;
            OnTimerFinish?.Invoke();
        }
    }

    public void StartTimer()
    {
        OnTimerStart?.Invoke();
        isTimerRun = true;

        m_currentTime = timeDuration;
    }

    public void PauseTimer()
    {
        isTimerRun = false;
    }

    public void ResetTimer()
    {
        m_currentTime = timeDuration;
    }
}
