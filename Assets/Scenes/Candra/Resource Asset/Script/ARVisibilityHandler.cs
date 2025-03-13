using System;
using UnityEngine;
using Imagine.WebAR;

public class ARVisibilityHandler : MonoBehaviour
{
    public static ARVisibilityHandler instance;

    [Header("References")]
    public GameObject canvasScan;
    public ImageTracker imageTracker;
    public string targetID;

    private bool isTracking = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // Mencegah duplikasi instance
    }

    void Start()
    {
        if (canvasScan == null)
        {
            Debug.LogError("❌ canvasScan belum di-assign di Inspector!");
            return;
        }

        canvasScan.SetActive(true);

        if (imageTracker != null)
        {
            imageTracker.OnImageFound.AddListener(OnMarkerFound);
            imageTracker.OnImageLost.AddListener(OnMarkerLost);
        }
        else
        {
            Debug.LogError("❌ imageTracker tidak terhubung di Inspector!");
        }
    }

    void OnDestroy()
    {
        if (imageTracker != null)
        {
            imageTracker.OnImageFound.RemoveListener(OnMarkerFound);
            imageTracker.OnImageLost.RemoveListener(OnMarkerLost);
        }
    }

    void OnMarkerFound(string foundTargetID)
    {
        if (foundTargetID == targetID && !isTracking)
        {
            isTracking = true;
            canvasScan.SetActive(false); // Sembunyikan canvasScan
            FlowGameManager flowGame = FindObjectOfType<FlowGameManager>();
            if (flowGame != null)
            {
                flowGame.ScanBerhasil();
            }
        }
    }

    void OnMarkerLost(string lostTargetID)
    {
        if (lostTargetID == targetID && isTracking)
        {
            isTracking = false;
            canvasScan.SetActive(true); // Tampilkan kembali canvasScan
        }
    }
}