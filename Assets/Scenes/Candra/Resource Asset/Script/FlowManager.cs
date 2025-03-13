using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlowGameManager : MonoBehaviour
{
    // ==== PANEL NAVIGASI ====
    [Header("🔹 Panel UI References")]
    public GameObject PanelAwal;
    public GameObject PanelTutorial;
    public GameObject PanelScan;

    // ==== AR ELEMENTS ====
    [Header("🟠 AR Elements")]
    public GameObject GrupMarkerStiker;

    // ==== UI ELEMENTS ====
    [Header("🟢 UI Elements")]
    public TextMeshProUGUI TxtCountdown;

    // ==== MINI GAME ELEMENTS ====
    [Header("🎮 Mini Game Elements")]
    public GameObject CanvasMiniGame;
    private GameObject panelTapMulai;
    private GameObject panelSelesai;
    private GameObject panelGagal;
    private GameObject popUpFinish;
    public GameObject panelMiniGame;

    // ==== MINI GAME VARIABLES ====
    [Header("🕐 Timer & Skor UI")]
    public TextMeshProUGUI TxtTimer;
    public TextMeshProUGUI TxtScore;
    public Button ButtonTap;

    private int score;
    private float timeRemaining = 30f;
    private bool isGameRunning = false;

    public static Action OnMiniGameSuccess;
    public static Action OnMiniGameFail;

    private void Start()
    {
        ResetGame();
        Debug.Log("🚀 FlowGameManager Started!");
    }

    private void OnEnable()
    {
        OnMiniGameSuccess += MiniGameBerhasil;
        OnMiniGameFail += MiniGameGagal;
    }

    private void OnDisable()
    {
        OnMiniGameSuccess -= MiniGameBerhasil;
        OnMiniGameFail -= MiniGameGagal;
    }

    // ==== NAVIGASI PANEL ====
    public void MasukTutorial()
    {
        PanelAwal.SetActive(false);
        PanelTutorial.SetActive(true);
    }

    public void MasukPanelScan()
    {
        PanelTutorial.SetActive(false);
        PanelScan.SetActive(true);
    }

    // ==== SCAN BERHASIL ====
    public void ScanBerhasil()
    {
        if (GrupMarkerStiker == null)
        {
            Debug.LogError("❌ ERROR: GrupMarkerStiker belum di-assign!");
            return;
        }

        GrupMarkerStiker.SetActive(true);
        CariPanelDalamMarkerStiker();

        if (panelTapMulai != null)
        {
            panelTapMulai.SetActive(true);
        }
    }

    // ==== CARI PANEL DI DALAM MARKER STIKER / CANVAS MINI GAME ====
    private void CariPanelDalamMarkerStiker()
    {
        Transform canvasUI = GrupMarkerStiker != null ? GrupMarkerStiker.transform.Find("CanvasUI") : null;
        if (canvasUI != null)
        {
            panelTapMulai = CariPanel(canvasUI, "PanelTapMulai");
            panelSelesai = CariPanel(canvasUI, "PanelSelesai");
            panelGagal = CariPanel(canvasUI, "PanelGagal");
            popUpFinish = CariPanel(canvasUI, "PopUpFinish");
        }

        Transform miniGameCanvas = GrupMarkerStiker != null ? GrupMarkerStiker.transform.Find("CanvasMiniGame") : null;
        if (miniGameCanvas == null && CanvasMiniGame != null)
        {
            miniGameCanvas = CanvasMiniGame.transform;
        }

        if (miniGameCanvas != null)
        {
            panelMiniGame = miniGameCanvas.Find("PanelMiniGame")?.gameObject;

            if (TxtTimer == null)
                TxtTimer = miniGameCanvas.Find("PanelMiniGame/TxtTimer")?.GetComponent<TextMeshProUGUI>();
            if (TxtScore == null)
                TxtScore = miniGameCanvas.Find("PanelMiniGame/TxtScore")?.GetComponent<TextMeshProUGUI>();
            if (ButtonTap == null)
                ButtonTap = miniGameCanvas.Find("PanelMiniGame/ButtonTap")?.GetComponent<Button>();

            if (ButtonTap != null)
            {
                ButtonTap.onClick.RemoveAllListeners();
                ButtonTap.onClick.AddListener(TambahScore);
            }
        }

        Debug.Log(panelMiniGame != null ? "✅ PanelMiniGame ditemukan!" : "❌ PanelMiniGame TIDAK ditemukan!");
    }

    private GameObject CariPanel(Transform parent, string panelName)
    {
        Transform panel = parent.Find(panelName);
        return panel != null ? panel.gameObject : null;
    }

    // ==== MULAI MINI GAME ====
    public void MulaiGame()
    {
        if (panelTapMulai != null) panelTapMulai.SetActive(false);
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        TxtCountdown.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            TxtCountdown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        TxtCountdown.gameObject.SetActive(false);
        StartMiniGame();
    }

    private void StartMiniGame()
    {
        if (CanvasMiniGame != null) CanvasMiniGame.SetActive(true);
        if (panelMiniGame != null)
        {
            panelMiniGame.SetActive(true);
            Debug.Log("✅ Panel Mini Game Aktif: " + panelMiniGame.activeSelf);
        }
        else
        {
            Debug.LogError("❌ Panel Mini Game tidak ditemukan!");
        }

        score = 0;
        timeRemaining = 30f;
        isGameRunning = true;

        if (TxtScore != null) TxtScore.text = "0";
        if (TxtTimer != null) TxtTimer.text = "30";

        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        while (timeRemaining > 0 && isGameRunning)
        {
            timeRemaining -= Time.deltaTime;
            if (TxtTimer != null) TxtTimer.text = Mathf.CeilToInt(timeRemaining).ToString();
            yield return null;
        }

        if (isGameRunning)
        {
            MiniGameGagal();
        }
    }

    public void TambahScore()
    {
        if (!isGameRunning) return;

        score++;
        if (TxtScore != null) TxtScore.text = score.ToString();

        if (score >= 10)
        {
            MiniGameBerhasil();
        }
    }

    public void MiniGameBerhasil()
    {
        if (!isGameRunning) return;

        isGameRunning = false;
        SelesaikanMiniGame(true);
    }

    public void MiniGameGagal()
    {
        if (!isGameRunning) return;

        isGameRunning = false;
        SelesaikanMiniGame(false);
    }

    private void SelesaikanMiniGame(bool sukses)
    {
        if (panelMiniGame != null) panelMiniGame.SetActive(false);

        if (sukses)
        {
            if (panelSelesai != null) panelSelesai.SetActive(true);
            StartCoroutine(TampilkanPopUpFinish());
        }
        else
        {
            if (panelGagal != null) panelGagal.SetActive(true);
        }
    }

    private IEnumerator TampilkanPopUpFinish()
    {
        yield return new WaitForSeconds(2f);
        if (popUpFinish != null) popUpFinish.SetActive(true);
    }

    public void CobaLagi() { ResetGame(); }

    private void ResetGame()
    {
        PanelAwal.SetActive(true);
        PanelTutorial.SetActive(false);
        PanelScan.SetActive(false);
        if (GrupMarkerStiker != null) GrupMarkerStiker.SetActive(false);
        if (CanvasMiniGame != null) CanvasMiniGame.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            panelMiniGame.SetActive(!panelMiniGame.activeSelf);
            Debug.Log("Toggle PanelMiniGame: " + panelMiniGame.activeSelf);
        }
    }
}
