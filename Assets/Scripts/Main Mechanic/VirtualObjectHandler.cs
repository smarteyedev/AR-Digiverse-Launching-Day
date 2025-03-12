using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualObjectHandler : MonoBehaviour
{
    [SerializeField] private List<Animator> characters;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] Text messageText;

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

    public void ShowVFX()
    {
        vfx.Play();
    }


    public void ShowMessage(string txt)
    {
        messageText.text = txt;
    }
}
