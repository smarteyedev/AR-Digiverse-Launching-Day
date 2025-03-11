using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TapMechanism tapMechanism;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tapMechanism.StartTapping();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            tapMechanism.ResetProgress();
        }
    }
}
