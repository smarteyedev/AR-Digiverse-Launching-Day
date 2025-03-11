using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjectHandler : MonoBehaviour
{
    public List<Animator> characters;

    public void UpdateCharacterAnimation(float _arg)
    {
        characters[0].SetFloat("animationProgress", _arg);
        characters[1].SetFloat("animationProgress", _arg);
        characters[2].SetFloat("animationProgress", _arg);
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {

    }
}
