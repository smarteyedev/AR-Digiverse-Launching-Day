using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        //! Main field
        [HideInInspector] public bool isGamePlaying = false;
        [HideInInspector] public bool playerHasLogged = false;
        public string playerName;
        public int playerTapCount;
        public float playerTime;

        [Header("Timer Config")]
        [SerializeField] private float timeDuration = 60f;
        private float m_currentTime = 60f;
        private bool isTimerRun = false;
        private bool m_isTimerFinished = false;

        [Header("Component Reference")]
        [SerializeField] private UIController uIController;
        [SerializeField] private TapMechanism tapMechanism;
        [Space(5f)]
        [SerializeField] private TMP_InputField playernameInput;
        [SerializeField] private GameObject notNullInputMessage;

        [Space(5f)]
        [SerializeField] private GameObject timerParent;
        [SerializeField] private TextMeshProUGUI countdownText;

        public UnityEvent OnStartGame;

        [Space(10f)]
        [Tooltip("is called when timer is start")]
        public UnityEvent OnTimerStart;
        [Tooltip("is called when timer is finish")]
        public UnityEvent OnTimerFinish;

        [Space(10f)]
        [Tooltip("is called when countdown in tappingMechanism is Finish")]
        public UnityEvent OnFirstTapping;
        [Tooltip("is called when player finish the tapping game and progress value is equal to max")]
        public UnityEvent OnTappingFinished;

        private void Start()
        {
            if (timerParent) { timerParent.gameObject.SetActive(false); }

            OnStartGame?.Invoke();

            Screen.fullScreen = !Screen.fullScreen;
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

                if (hours == 0 && minutes == 0 && seconds == 0 && !m_isTimerFinished)
                {
                    OnTimerFinish?.Invoke();
                    PlayingStatus(false);
                    m_isTimerFinished = true;
                    Debug.Log($"timer: {hours} {minutes} {minutes}");
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

        public void FinishGame()
        {
            float totalTimePlayed = timeDuration - m_currentTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalTimePlayed);

            Debug.Log($"Game selesai! {playerName} melakukan {playerTapCount} tapping dalam {timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3} detik.");
        }
        #endregion

        #region Main Function
        /// <summary>
        /// fungsi-fungsi ini digunakan untuk mengatur mekanisme game tapping
        /// </summary>

        // gunakan fungsi ini ketika pertama kali memulai permainan, setelah object virtual muncul di layar
        public void StartTappingGame()
        {
            if (!isGamePlaying)
            {
                tapMechanism.StartTapping(() =>
                {
                    StartTimer();
                    timerParent.gameObject.SetActive(true);
                    PlayingStatus(true);
                });
            }
        }

        public void PlayingStatus(bool state)
        {
            isGamePlaying = state;
        }

        public void OnMarkerFound()
        {
            if (!isGamePlaying)
            {
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

            if (!isGamePlaying)
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

            m_currentTime = timeDuration;
        }

        // this function is called in taptap finish event 
        public void PauseTimer()
        {
            isTimerRun = false;
        }

        private void ResetTimer()
        {
            m_currentTime = timeDuration;
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

        public void ResetScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}