using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Smarteye.AR.WebRequest;

namespace Smarteye.AR
{
    /// <summary>
    /// Berikut kegunaan dari class Game Manager :
    ///  -> mengatur game timer 
    ///  -> fungsi-fungsi untuk mengatur mekanisme game tapping
    ///  
    /// terdapat Unity Event digunakan untuk mengatur game flow
    /// </summary>
    /// 

    public class GameManager : MonoBehaviour
    {
        /* == Main field == */
        [HideInInspector] public bool playerDataHasSent = false;
        [HideInInspector] public string playerName;

        [Header("Timer Config")]
        [SerializeField] private float timeDuration = 60f;
        private float m_currentTime = 60f;
        private bool isTimerRun = false;
        private bool m_isTimerFinished = false;

        [Header("Component Reference")]
        [SerializeField] private UIController uIController;
        [SerializeField] private TapMechanism tapMechanism;
        [SerializeField] private HandlerPlayerCounter webRequestPlayerCounter;
        [Space(5f)]
        [SerializeField] private TMP_InputField playernameInput;
        [SerializeField] private GameObject notNullInputMessage;

        [Space(5f)]
        [SerializeField] private GameObject timerParent;
        [SerializeField] private TextMeshProUGUI countdownText;

        [Header("Unity Events")]
        [Tooltip("is called on game start")]
        public UnityEvent OnStart;

        [Space(10f)]
        [Tooltip("is called when timer is start")]
        public UnityEvent OnTimerStart;
        [Tooltip("is called when timer is finish")]
        public UnityEvent OnTimerFinish;

        [Space(10f)]
        public UnityEvent OnGameplayStart;
        public UnityEvent OnPlayerSuccess;
        public UnityEvent OnPlayerFail;
        public UnityEvent OnGameplayRestart;

        private void Start()
        {
            OnStart?.Invoke();
        }

        private void Update()
        {
            //! DebuggingFunction();

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

                if (IsTimerIsFinish() && !m_isTimerFinished)
                {
                    OnTimerFinish?.Invoke();
                    m_isTimerFinished = true;
                    // Debug.Log($"timer: {hours} {minutes} {minutes}");
                }
            }
            else
            {
                // Timer selesai
                countdownText.text = "00";
                isTimerRun = false;
            }
        }

        #region Player Data
        public void SetPlayerName()
        {
            if (!string.IsNullOrEmpty(playernameInput.text))
            {
                playerName = playernameInput.text;
                Debug.Log($"hello player: {playernameInput.text}");

                notNullInputMessage.SetActive(false);
                uIController.ControllerShowPanel(2);
            }
            else
            {
                notNullInputMessage.SetActive(true);
                Debug.Log($"player name is null");
            }
        }

        private bool IsTimerIsFinish()
        {
            int hours = Mathf.FloorToInt(m_currentTime / 3600);
            int minutes = Mathf.FloorToInt((m_currentTime % 3600) / 60);
            int seconds = Mathf.FloorToInt(m_currentTime % 60);

            return hours == 0 && minutes == 0 && seconds == 0;
        }

        public void FinishGameplay()
        {
            float totalTimePlayed = timeDuration - m_currentTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalTimePlayed);

            Debug.Log($"Game selesai! {playerName} melakukan {tapMechanism.TapCount} tapping dalam {timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3} detik.");

            PauseTimer();

            if (!playerDataHasSent)
            {
                webRequestPlayerCounter.SendPlayerData(() =>
                {
                    playerDataHasSent = true;
                });

                /* webRequestPlayerCounter.SendPlayerData(
                _playerName: playerName,
                _playerTimer: totalTimePlayed,
                _playerTapCount: tapMechanism.TapCount,
                () =>
                {
                    playerDataHasSent = true;
                }); */
            }


            if (!IsTimerIsFinish() && tapMechanism.IsFinishedTap)
            {
                OnPlayerSuccess?.Invoke();

                uIController.ShowResultPanel(
                    true,
                    $"{playerName} <br> ({tapMechanism.TapCount} ketukan dalam {timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3} detik.)"
                    // $"{playerName} <br> ({tapMechanism.TapCount} ketukan dalam {totalTimePlayed} detik.)"
                    );
            }
            else
            {
                OnPlayerFail?.Invoke();

                uIController.ShowResultPanel(
                    false,
                    $"{playerName} <br> ({tapMechanism.TapCount} ketukan dalam {timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3} detik.)"
                    // $"{playerName} <br> ({tapMechanism.TapCount} ketukan dalam {totalTimePlayed} detik.)"
                    );
            }

            ResetTimer();
        }
        #endregion

        #region Main Function
        /// <summary>
        /// fungsi-fungsi ini digunakan untuk mengatur mekanisme game tapping
        /// </summary>

        // gunakan fungsi ini ketika pertama kali memulai permainan, setelah object virtual muncul di layar
        public void StartTappingGame()
        {
            if (!isTimerRun)
            {
                tapMechanism.StartTapping(() =>
                {
                    StartTimer();
                    timerParent.gameObject.SetActive(true);
                    OnGameplayStart?.Invoke();
                });
            }
        }

        // called in quote panel
        public void ResetGameplay()
        {
            ResetTimer();
            tapMechanism.ResetDefault();

            OnGameplayRestart?.Invoke();
        }

        public void OnFullScreenSetup(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }

        public void OnMarkerFound()
        {
            if (!isTimerRun)
            {
                if (!tapMechanism.gameObject.activeSelf && m_currentTime == timeDuration)
                    uIController.ControllerShowPanel(3);
            }
            else
            {
                uIController.ControllerShowPanel(4);
                tapMechanism.SetTappingUIActive(true);
            }
        }

        public void OnMarkerLost()
        {
            tapMechanism.SetTappingUIActive(false);
            tapMechanism.ResetTappingProgress();

            if (!isTimerRun)
            {
                uIController.HideStartPanel();
                tapMechanism.ResetCountdown();
            }
        }
        #endregion

        #region Timer Behaviour
        private void StartTimer()
        {
            if (isTimerRun) return;

            OnTimerStart?.Invoke();
            isTimerRun = true;
            m_isTimerFinished = false;

            m_currentTime = timeDuration;
        }

        // this function is called in taptap finish event 
        public void PauseTimer()
        {
            isTimerRun = false;
            timerParent.SetActive(false);
        }

        private void ResetTimer()
        {
            m_currentTime = timeDuration;
            isTimerRun = false;
            m_isTimerFinished = false;
        }
        #endregion

        private void DebuggingFunction()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartTappingGame();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                tapMechanism.ResetTappingProgress();
                ResetTimer();
            }
        }
    }
}