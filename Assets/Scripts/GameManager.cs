using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TapMechanism tapMechanism;
    public VirtualObjectHandler virtualObjectPrefab;
    public Transform spawnLocation;

    private void Start()
    {
        VirtualObjectHandler newObj = Instantiate(virtualObjectPrefab, spawnLocation.position, spawnLocation.rotation);
        tapMechanism.currentObject = newObj;
    }

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
