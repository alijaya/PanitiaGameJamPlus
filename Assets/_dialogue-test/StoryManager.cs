using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Ink.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [Title("Ink")]
    [SerializeField] private TextAsset inkAsset;

    [Title("Characters")]
    [SerializeField] private GameObject leftPerson;
    [SerializeField] private GameObject rightPerson;

    [Title("UI")]
    [SerializeField] private TextMeshProUGUI leftDialogue;
    [SerializeField] private TextMeshProUGUI rightDialogue;

    [Title("Animation")]
    [SerializeField] private float textSpeed = 15f;
    [SerializeField] private Ease easeType = Ease.OutCubic;

    private Tween _typeWriterTween;

    private Story _inkStory;

    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);
        
        _inkStory.onError += (msg, type) => {
            if( type == Ink.ErrorType.Warning )
                Debug.LogWarning(msg);
            else
                Debug.LogError(msg);
        };
        
        UpdateDialogue(_inkStory.Continue());
    }

    public void ClickContinueStory()
    {
        if (_inkStory.canContinue)
        {
            UpdateDialogue(_inkStory.Continue());
        }
        else
        {
            Debug.Log("Story already ended.");
        }
    }

    private void UpdateDialogue(string newText)
    {
        Debug.Log(_inkStory.currentTags[0]);
        
        leftDialogue.text = "";
        rightDialogue.text = "";

        switch (_inkStory.currentTags[1])
        {
            case "left":
                StartCoroutine(OutputDialogue(newText, leftDialogue, leftPerson));
                break;
            case "right":
                StartCoroutine(OutputDialogue(newText, rightDialogue, rightPerson));
                break;
        }
    }

    private IEnumerator OutputDialogue(string newText, TextMeshProUGUI dialogue, GameObject person)
    {
        person.transform.localEulerAngles = new Vector3(0f, person.transform.localEulerAngles.y, 0f);
        Sequence tweenSequence = DOTween.Sequence();
        tweenSequence.Append(person.transform.DOLocalRotate(new Vector3(person.transform.localEulerAngles.x, person.transform.localEulerAngles.y, -10f), 0.1f, RotateMode.FastBeyond360)
            .SetLoops(20, LoopType.Yoyo));
        
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = newText;

        _typeWriterTween = DOVirtual.Int(0, newText.Length, newText.Length / textSpeed, v =>
        {
            dialogue.maxVisibleCharacters = v;

        }).SetEase(easeType);

        yield return new WaitUntil(() => dialogue.maxVisibleCharacters == newText.Length);

        tweenSequence.Kill();
    }
}
