using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using DG.Tweening;
using Ink.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    /*  Tags List:
     *  
     *  person: name of character that is speaking.
     *  dialogue: where to display dialogue (left, right).
     *  flip: flip the character's sprite 180 degrees to the y axis.
     *  moveX: move character at x axis.
     *  enableSequence: Chain animation tags together.
     * 
     */
    
    [Title("Ink")]
    [SerializeField] private TextAsset inkAsset;

    [Title("Characters")]
    [SerializeField] private GameObject leftPerson;
    [SerializeField] private GameObject rightPerson;

    [Title("UI")]
    [SerializeField] private TextMeshProUGUI leftDialogue;
    [SerializeField] private TextMeshProUGUI leftName;
    [SerializeField] private TextMeshProUGUI rightDialogue;
    [SerializeField] private TextMeshProUGUI rightName;

    [Title("Animation")]
    [SerializeField] private float textSpeed = 15f;
    [SerializeField] private Ease easeType = Ease.OutCubic;

    private Tween _typeWriterTween;

    private Story _inkStory;

    private Sequence _bobSequence;
    
    //private Sequence _sequence;
    private bool _enableSequence = false;
    
    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);
        
        _inkStory.onError += (msg, type) => {
            if( type == Ink.ErrorType.Warning )
                Debug.LogWarning(msg);
            else
                Debug.LogError(msg);
        };
        
        // UpdateDialogue(_inkStory.Continue());
        ClickContinueStory();
    }

    public void ClickContinueStory()
    {
        if (_inkStory.canContinue)
        {
            string text = _inkStory.Continue();
            if (text.StartsWith("NA"))
            {
                PlayAnimationOnly();
            }
            else
            {
                UpdateDialogue(text);
            }
            
        }
        else
        {
            // Debug.Log("Story restarting.");
            // _inkStory.ResetState();
            // text = _inkStory.Continue();
            // UpdateDialogue(text);
            Debug.Log("Story already ended.");
        }
    }

    private void PlayAnimationOnly()
    {
        leftDialogue.text = "";
        leftName.text = "";
        rightDialogue.text = "";
        rightName.text = "";
        
        List<Tween> list = new List<Tween>();

        Sequence _sequence = null;
        bool isFirstSequence = true;
        _enableSequence = false;
        
        TextMeshProUGUI dialoguePosition = null;
        TextMeshProUGUI namePosition = null;
        GameObject speakingPerson = null;

        foreach (var tags in _inkStory.currentTags)
        {
            Debug.Log(tags);
        }

        foreach (string textTag in _inkStory.currentTags)
        {
            if (textTag[..3] == "dia")
            {
                switch (textTag.Remove(0, 10))
                {
                    case "left":
                        dialoguePosition = leftDialogue;
                        namePosition = leftName;
                        speakingPerson = leftPerson;
                        break;
                    case "right":
                        dialoguePosition = rightDialogue;
                        namePosition = rightName;
                        speakingPerson = rightPerson;
                        break;
                }
            }

            if (textTag[..3] == "ena")
            {
                _enableSequence = true;
                // _sequence = DOTween.Sequence();
            }
            
            if (textTag[..3] == "fli")
            {
                bool isFacingLeft = true;
                
                switch (textTag.Remove(0, 6))
                {
                    case "left":
                        isFacingLeft = true;
                        break;
                    case "right":
                        isFacingLeft = false;
                        break;
                }
                
                if (_enableSequence)
                {
                    list.Add(Flip(speakingPerson.transform.parent, isFacingLeft));
                }
                else
                {
                    Flip(speakingPerson.transform.parent, isFacingLeft);
                }
                
                Debug.Log($"isfacingLeft: {isFacingLeft}");
            }

            if (textTag[..3] == "mov")
            {
                float value = float.Parse(textTag.Remove(0, 7));
                

                if (_enableSequence)
                {
                    // if (isFirstSequence)
                    // {
                    //     Debug.Log($"Append {value}");
                    //     _sequence.Append(MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 3));
                    //     isFirstSequence = false;
                    // }
                    // else
                    // {
                    //     Debug.Log($"Join {value}");
                    //     _sequence.Append(MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 3));
                    // }
                    
                    list.Add(MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 1));
                }
                else
                {
                    MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 1);
                }
                
            }
        }

        _sequence = DOTween.Sequence();

        foreach (Tween tween in list)
        {
            Debug.Log("Hey");
            _sequence.Append(tween);
        }

        _enableSequence = false;
        ClickContinueStory();
    }

    private void UpdateDialogue(string newText)
    {
        leftDialogue.text = "";
        leftName.text = "";
        rightDialogue.text = "";
        rightName.text = "";
        
        Sequence _sequence = null;
        bool isFirstSequence = true;
        _enableSequence = false;

        TextMeshProUGUI dialoguePosition = null;
        TextMeshProUGUI namePosition = null;
        GameObject speakingPerson = null;

        foreach (string textTag in _inkStory.currentTags)
        {
            if (textTag[..3] == "dia")
            {
                switch (textTag.Remove(0, 10))
                {
                    case "left":
                        dialoguePosition = leftDialogue;
                        namePosition = leftName;
                        speakingPerson = leftPerson;
                        break;
                    case "right":
                        dialoguePosition = rightDialogue;
                        namePosition = rightName;
                        speakingPerson = rightPerson;
                        break;
                }
            }
            
            if (textTag[..3] == "per")
            {
                if (dialoguePosition == null || newText == "NA")
                {
                    
                };
                namePosition.text = textTag.Remove(0, 8);
            }

            if (textTag[..3] == "fli")
            {
                bool isFacingLeft = true;
                
                switch (textTag.Remove(0, 6))
                {
                    case "left":
                        isFacingLeft = true;
                        break;
                    case "right":
                        isFacingLeft = false;
                        break;
                }
                
                if (_enableSequence)
                {
                    Flip(speakingPerson.transform.parent, isFacingLeft);
                }
                else
                {
                    Flip(speakingPerson.transform.parent, isFacingLeft);
                }
            }

            if (textTag[..3] == "mov")
            {
                MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 3);
            }
        }
        
        if (newText != "NA") StartCoroutine(OutputDialogue(newText, dialoguePosition, speakingPerson));
    }

    private Tween Flip(Transform objectTransform, bool isFacingLeft)
    {
        return DOTween.To(() => objectTransform.localEulerAngles.y, (value) =>
        {
            var rot = objectTransform.localEulerAngles;
            rot.y = value;
            objectTransform.localEulerAngles = rot;
        }, (isFacingLeft ? (objectTransform.localEulerAngles.y - 180f) : (objectTransform.localEulerAngles.y + 180f)), 0.3f);
    }

    private Tween MoveX(Transform objectTransform, float distance, float speed)
    {
        return objectTransform.DOMoveX(distance, speed);
    }

    private IEnumerator OutputDialogue(string newText, TextMeshProUGUI dialogue, GameObject person)
    {
        ResetAnimation();
        person.transform.localEulerAngles = new Vector3(person.transform.localEulerAngles.x, person.transform.localEulerAngles.y, person.transform.localEulerAngles.z);
        _bobSequence = DOTween.Sequence();
        _bobSequence.Append(person.transform.DOLocalRotate(new Vector3(person.transform.localEulerAngles.x, person.transform.localEulerAngles.y, -10f), 0.1f, RotateMode.FastBeyond360)
            .SetLoops(20, LoopType.Yoyo));
        
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = newText;

        _typeWriterTween = DOVirtual.Int(0, newText.Length, newText.Length / textSpeed, v =>
        {
            dialogue.maxVisibleCharacters = v;

        }).SetEase(easeType);

        yield return new WaitUntil(() => dialogue.maxVisibleCharacters == newText.Length);
        
        _bobSequence.Kill();

        // person.transform.localEulerAngles = new Vector3(0f, person.transform.localEulerAngles.y, 0f);
        person.transform.DOLocalRotate(new Vector3(person.transform.localEulerAngles.x, person.transform.localEulerAngles.y, 0f), 0.1f,
            RotateMode.Fast);
        
        // ResetAnimation();
    }

    private void ResetAnimation()
    {
        _bobSequence.Kill();
        leftPerson.transform.localEulerAngles = Vector3.zero;
        rightPerson.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
    }
}
