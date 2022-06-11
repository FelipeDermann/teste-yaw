using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private Animator _anim;
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int Instructions = Animator.StringToHash("Instructions");
    private static readonly int Back = Animator.StringToHash("Back");

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void StartButton()
    {
        _anim.SetTrigger(Start);
    }
    
    public void InstructionsButton()
    {
        _anim.SetTrigger(Instructions);
    }

    public void BackButton()
    {
        _anim.SetTrigger(Back);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void NormalButton()
    {
        SceneManager.LoadScene(1);
    }
    
    public void HardButton()
    {
        SceneManager.LoadScene(2);
    }
}
