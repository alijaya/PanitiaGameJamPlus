using System.Collections;
using System.Collections.Generic;
using Core.LevelManagement;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class GlobalRef : SingletonSO<GlobalRef>
{
    public FloatReference timeLeft;
    public IntReference totalCustomerServed;
    public IntReference totalSales;

    public SceneReference Scene_MainMenu;
    public SceneReference Scene_LevelSelection;
    public SceneReference Scene_Gameplay;
    public SceneReference Scene_GameplayTesting;
    public SceneReference Scene_Report;

    public LevelProgression LevelProgression;

    public AudioClip BGM_MainMenu;
    public AudioClip BGM_Gameplay;
    public AudioClip BGM_Report;
    public AudioClip SFX_Typing;
    public AudioClip SFX_TypingError;
    public AudioClip SFX_FoodReady;
    public AudioClip SFX_FoodServed;
    public AudioClip SFX_CustomerEnter;
    public AudioClip SFX_CustomerOrder;
    public AudioClip SFX_CustomerHappy;
    public AudioClip SFX_CustomerAngry;
    public AudioClip SFX_Shot;
    
    // popup-test
    public GameObject scorePopup;

    public WordBankSO defaultWordBank;

    [AssetSelector]
    public Core.Words.WordObject WordObjectPrefab;

    [AssetSelector]
    public CustomerUI CustomerUIPrefab;

    public void GoToScene_MainMenu()
    {
        SceneManager.LoadScene(Scene_MainMenu.Name);
    }

    public void GoToScene_LevelSelection() {
        SceneManager.LoadScene(Scene_LevelSelection.Name);
    }

    public void GoToScene_Gameplay()
    {
        SceneManager.LoadScene(Scene_Gameplay.Name);
    }

    public void GoToScene_Gameplay(int level) {
        LevelProgression.SetLevel(level);
        SceneManager.LoadScene(Scene_GameplayTesting.Name);
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

    public void PlayBGM_Report()
    {
        AudioManager.I.PlayBGM(BGM_Report, true, false);
    }

    public void PlaySFX_Typing()
    {
        AudioManager.I.PlaySFX(SFX_Typing);
    }

    public void PlaySFX_TypingError()
    {
        AudioManager.I.PlaySFX(SFX_TypingError);
    }

    public void PlaySFX_FoodReady()
    {
        AudioManager.I.PlaySFX(SFX_FoodReady, 0.3f);
    }

    public void PlaySFX_FoodServed()
    {
        AudioManager.I.PlaySFX(SFX_FoodServed);
    }

    public void PlaySFX_CustomerEnter()
    {
        AudioManager.I.PlaySFX(SFX_CustomerEnter);
    }

    public void PlaySFX_CustomerOrder()
    {
        AudioManager.I.PlaySFX(SFX_CustomerOrder, 0.3f);
    }

    public void PlaySFX_CustomerHappy()
    {
        AudioManager.I.PlaySFX(SFX_CustomerHappy);
    }

    public void PlaySFX_CustomerAngry()
    {
        AudioManager.I.PlaySFX(SFX_CustomerAngry);
    }

    public void PlaySFX_Shot()
    {
        AudioManager.I.PlaySFX(SFX_Shot);
    }
}
