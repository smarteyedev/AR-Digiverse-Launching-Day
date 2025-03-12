using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        [Header("Timer Config")]
        [SerializeField] private float timeDuration = 60f;
        private float m_currentTime = 60f;
        private bool isTimerRun = false;

        [Header("Object Prefab")]
        public VirtualObjectHandler virtualObjectPrefab;
        [SerializeField] private Transform spawnLocation;
        private VirtualObjectHandler m_currentObject;

        [Header("Component Reference")]
        [SerializeField] private TapMechanism tapMechanism;

        [Space(5f)]
        [SerializeField] private GameObject timerParent;
        [SerializeField] private Text countdownText; //! ganti dengan komponen textmeshpro

        [Header("Unity Event")]
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
            }
            else
            {
                // Timer selesai
                countdownText.text = "00";
                isTimerRun = false;
                OnTimerFinish?.Invoke();
            }
        }

        #region Main Function
        /// <summary>
        /// fungsi-fungsi ini digunakan untuk mengatur mekanisme game tapping
        /// </summary>

        // gunakan fungsi ini untuk memasukkan virutal object sebagai object reverensi dari tap mechanism
        // ketika marker terdeteksi dan object muncul di layar
        public void AssignObjectToTapMechanism()
        {
            tapMechanism.SetupVirtualObject(m_currentObject);

            tapMechanism.OnTapStart.AddListener(() => OnFirstTapping.Invoke());
            tapMechanism.OnTapFinish.AddListener(() => OnTappingFinished.Invoke());
        }

        // gunakan fungsi ini ketika pertama kali memulai permainan, setelah object virtual muncul di layar
        public void StartTappingGame()
        {
            if (m_currentObject)
            {
                tapMechanism.StartTapping(() =>
                {
                    StartTimer();
                    timerParent.gameObject.SetActive(true);
                });
            }
        }

        // panggil fungsi ini ketika marker atau object tidak terdeteksi
        public void OnMarkerDisappear()
        {
            if (m_currentObject)
            {
                Destroy(m_currentObject.gameObject);

                tapMechanism.SetTappingUIActive(false);
                tapMechanism.ResetTappingProgress();
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

        private void PauseTimer()
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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!m_currentObject)
                {
                    m_currentObject = Instantiate(virtualObjectPrefab, spawnLocation.position, spawnLocation.rotation).GetComponent<VirtualObjectHandler>();
                    AssignObjectToTapMechanism();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnMarkerDisappear();
            }

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