using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GlobalRef : SingletonSO<GlobalRef>
{
    public FloatReference timeLeft;
    public IntReference totalCustomerServed;
    public IntReference totalSales;

    public GameObjectValueList Words;
    public GameObjectValueList PrevHighlightedWords;

    public SceneReference Scene_MainMenu;
    public SceneReference Scene_Gameplay;
    public SceneReference Scene_Report;

    public AudioClip BGM_MainMenu;
    public AudioClip BGM_Gameplay;
    public AudioClip SFX_Typing;
    public AudioClip SFX_TypingError;
    public AudioClip SFX_Served;
    public AudioClip SFX_CustomerEnter;
    public AudioClip SFX_CustomerHappy;
    public AudioClip SFX_CustomerAngry;

    public void CleanUpWords()
    {
        Words.Clear();
        PrevHighlightedWords.Clear();
    }

    public void GoToScene_MainMenu()
    {
        SceneManager.LoadScene(Scene_MainMenu.Name);
    }

    public void GoToScene_Gameplay()
    {
        SceneManager.LoadScene(Scene_Gameplay.Name);
    }

    public void GoToScene_Report()
    {
        SceneManager.LoadScene(Scene_Report.Name);
    }

    public void PlayBGM_MainMenu()
    {
        AudioManager.I.PlayBGM(BGM_MainMenu);
    }

    public void PlayBGM_Gameplay()
    {
        AudioManager.I.PlayBGM(BGM_Gameplay);
    }

    public void PlaySFX_Typing()
    {
        AudioManager.I.PlaySFX(SFX_Typing);
    }

    public void PlaySFX_TypingError()
    {
        AudioManager.I.PlaySFX(SFX_TypingError);
    }

    public void PlaySFX_Served()
    {
        AudioManager.I.PlaySFX(SFX_Served);
    }

    public void PlaySFX_CustomerEnter()
    {
        AudioManager.I.PlaySFX(SFX_CustomerEnter);
    }

    public void PlaySFX_CustomerHappy()
    {
        AudioManager.I.PlaySFX(SFX_CustomerHappy);
    }

    public void PlaySFX_CustomerAngry()
    {
        AudioManager.I.PlaySFX(SFX_CustomerAngry);
    }
}
