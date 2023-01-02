using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using Cinemachine;
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
     *  enableSequence: Chain all next animation tags together.
     * 
     */
    
    [Title("Ink")]
    [SerializeField] private TextAsset inkAsset;

    [Title("Characters (Actor/Speaker)")]
    [SerializeField] private GameObject[] characters;

    // [Title("Text Animation Settings")]
    // [SerializeField] private float textSpeed = 15f;
    // [SerializeField] private Ease easeType = Ease.OutCubic;

    [Title("Camera")]
    [SerializeField] private Transform envCameraParent;
    [DisableInEditorMode]
    [SerializeField] private List<CinemachineVirtualCamera> cameras;

    [Title("Move Points")]
    [SerializeField] private Transform movePointParent;
    [DisableInEditorMode]
    [SerializeField] private List<Transform> movePoints;

    [Title("Screen Fade")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float screenFadeDuration = 1f;

    [Title("WordUI")]
    [SerializeField] private GameObject wordUI;
    
    [Title("Debugging")][Space]
    
    [Header("Person 1")]
    [SerializeField] public string person1NameText = "person1";
    [SerializeField] private GameObject person1Parent;
    [SerializeField] private MovementController person1Controller;
    [SerializeField] private GameObject person1Sprite;
    [SerializeField] private GameObject person1DialogueBox;
    [SerializeField] private CanvasGroup person1CanvasGroup;
    [SerializeField] private CinemachineVirtualCamera person1Camera;
    [SerializeField] private TextMeshProUGUI person1Name;
    [SerializeField] private TextMeshProUGUI person1Dialogue;
    
    [Header(("Person 2"))]
    [SerializeField] public string person2NameText = "person2";
    [SerializeField] private GameObject person2Parent;
    [SerializeField] private MovementController person2Controller;
    [SerializeField] private GameObject person2Sprite;
    [SerializeField] private GameObject person2DialogueBox;
    [SerializeField] private CanvasGroup person2CanvasGroup;
    [SerializeField] private CinemachineVirtualCamera person2Camera;
    [SerializeField] private TextMeshProUGUI person2Name;
    [SerializeField] private TextMeshProUGUI person2Dialogue;

    private Tween _typeWriterTween;
    private Tween _screenFadeTween;

    private Story _inkStory;

    private Sequence _bobSequence;
    
    private bool _enableSequence = false;

    private bool _isOverridingCamera = false;
    
    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);
        
        _inkStory.onError += (msg, type) => {
            if( type == Ink.ErrorType.Warning )
                Debug.LogWarning(msg);
            else
                Debug.LogError(msg);
        };
        
        _inkStory.BindExternalFunction ("SetPerson1", (string personName, string movePoint) => {
            SetPerson1(personName, movePoint);
        });
        
        _inkStory.BindExternalFunction ("SetPerson2", (string personName, string movePoint) => {
            SetPerson2(personName, movePoint);
        });
        
        _inkStory.BindExternalFunction("StartStory", StartStory);
        
        _inkStory.BindExternalFunction("Move", (string personName, string movePoint) =>
        {
            Move(personName == person1Parent.name ? person1Controller : person2Controller, movePoint);
        });
        
        _inkStory.BindExternalFunction ("SwitchCamera", (string cameraName) => {
            SwitchCamera(cameraName);
        });
        
        _inkStory.BindExternalFunction ("OverrideCamera", (bool trigger) => {
            OverrideCamera(trigger);
        });

        foreach (Transform child in envCameraParent)
        {
            cameras.Add(child.GetComponent<CinemachineVirtualCamera>());
        }
        
        foreach (Transform child in movePointParent)
        {
            movePoints.Add(child.GetComponent<Transform>());
        }
        
        FadeScreen(1f, 0f);
        
        ClickContinueStory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClickContinueStory();
        }
    }

    private void SetPerson1(string personName, string movePoint)
    {
        person1NameText = personName;

        foreach (GameObject character in characters)
        {
            if (character.name == personName)
            {
                person1Parent = character;
                person1Sprite = character.GetComponentInChildren<SpriteRenderer>().gameObject;
                person1Controller = character.GetComponent<MovementController>();
                person1DialogueBox = character.transform.Find("DialogueBox").gameObject;
                person1CanvasGroup = person1DialogueBox.GetComponent<CanvasGroup>();
                person1Camera = person1DialogueBox.transform.Find("VCam").GetComponent<CinemachineVirtualCamera>();
                person1Name = person1DialogueBox.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                person1Dialogue = person1DialogueBox.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();

                person1Camera.name = personName;
                cameras.Add(person1Camera);

                foreach (Transform point in movePoints)
                {
                    if (point.gameObject.name == movePoint)
                    {
                        person1Parent.transform.position = point.position;
                    }
                }
                
                return;
            }
        }
    }
    
    private void SetPerson2(string personName, string movePoint)
    {
        person2NameText = personName;

        foreach (GameObject character in characters)
        {
            if (character.name == personName)
            {
                person2Parent = character;
                person2Sprite = character.GetComponentInChildren<SpriteRenderer>().gameObject;
                person2Controller = character.GetComponent<MovementController>();
                person2DialogueBox = character.transform.Find("DialogueBox").gameObject;
                person2CanvasGroup = person2DialogueBox.GetComponent<CanvasGroup>();
                person2Camera = person2DialogueBox.transform.Find("VCam").GetComponent<CinemachineVirtualCamera>();
                person2Name = person2DialogueBox.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                person2Dialogue = person2DialogueBox.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();

                person2Camera.name = personName;
                cameras.Add(person2Camera);
                
                foreach (Transform point in movePoints)
                {
                    if (point.gameObject.name == movePoint)
                    {
                        person2Parent.transform.position = point.position;
                    }
                }
                
                return;
            }
        }
    }

    private void SwitchCamera(string cameraName)
    {
        foreach (CinemachineVirtualCamera vcam in cameras)
        {
            vcam.Priority = vcam.name == cameraName ? 40 : 20;
        }
    }

    private void OverrideCamera(bool trigger)
    {
        _isOverridingCamera = trigger;
    }

    private string[] ParseDialogue(string dialogue)
    {
        return dialogue.StartsWith("NA") ? new[] { "NA" } : dialogue.Split(": ");
    }

    private void StartStory()
    {
        
    }

    public void ClickContinueStory()
    {
        if (_inkStory.canContinue)
        {
            string text = _inkStory.Continue();
            OutputDialogue(text);
        }
        else
        {
            // Debug.Log("Story restarting.");
            // _inkStory.ResetState();
            // text = _inkStory.Continue();
            // UpdateDialogue(text);
            StartCoroutine(EndStory());
        }
    }

    private void OutputDialogue(string text)
    {
        person1CanvasGroup.alpha = 0f;
        person2CanvasGroup.alpha = 0f;
        
        string[] splitText = ParseDialogue(text);

        GameObject personGo = null;
        MovementController personController = null;
        CanvasGroup canvasGroup = null;
        TextMeshProUGUI personName = null;
        TextMeshProUGUI dialogue = null;

        if (splitText[0] == person1NameText)
        {
            personGo = person1Sprite;
            personController = person1Controller;
            canvasGroup = person1CanvasGroup;
            personName = person1Name;
            dialogue = person1Dialogue;
        }
        else if (splitText[0] == person2NameText)
        {
            personGo = person2Sprite;
            personController = person2Controller;
            canvasGroup = person2CanvasGroup;
            personName = person2Name;
            dialogue = person2Dialogue;
        }
        // else if (splitText[0] == "NA")
        // {
        //     StartCoroutine(RunTag(personGo, personController));
        // }
        else
        {
            Debug.Log($"Character {splitText[0]} not found.");
            return;
        }

        if (personName != null)
        {
            personName.text = splitText[0];
            if (_isOverridingCamera) SwitchCamera(personName.text);
        }
        
        if (dialogue != null) dialogue.richText = false;

        string[] words = splitText[1].Split();
        
        dialogue.richText = true;

        canvasGroup.alpha = 1f;

        StartCoroutine(RunTag(personGo, personController));
        
        personGo.transform.localEulerAngles = new Vector3(personGo.transform.localEulerAngles.x, personGo.transform.localEulerAngles.y, personGo.transform.localEulerAngles.z);
        dialogue.text = splitText[1];
        
        if (splitText[0] == "NA")
            ClickContinueStory();
    }

    private IEnumerator RunTag(GameObject personGo, MovementController personController)
    {
        foreach (string textTag in _inkStory.currentTags)
        {
            Debug.Log("tag: " + textTag);
            // if (textTag[..3] == "ena")
            // {
            //     _enableSequence = true;
            //     // _sequence = DOTween.Sequence();
            // }
            
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
                
                // Flip(personGo.transform.parent, isFacingLeft);
                // if (isFacingLeft)
                // {
                //     Debug.Log("Facing Left");
                //     personController.SetFaceLeft();
                // }
                // else
                // {
                //     Debug.Log("Facing Right");
                //     personController.SetFaceRight();
                // }

                Flip(personGo.transform, isFacingLeft);

            }
            else if (textTag[..3] == "mov")
            {
                string value = textTag.Remove(0, 6);
                Move(personController, value);
            }
        
            // if (textTag[..3] == "mov")
            // {
            //     float value = float.Parse(textTag.Remove(0, 7));
            //     MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 1);
            // }
        }

        yield break;
    }

    private IEnumerator RunTagsOnly()
    {
        bool ranAllTags = false;
        
        foreach (var tags in _inkStory.currentTags)
        {
            Debug.Log(tags);
        }

        ranAllTags = true;

        yield return new WaitUntil(() => ranAllTags == true);
    }

    // private void PlayAnimationOnly()
    // {
    //     person1Dialogue.text = "";
    //     person1Name.text = "";
    //     person2Dialogue.text = "";
    //     person2Name.text = "";
    //     
    //     List<Tween> list = new List<Tween>();
    //
    //     Sequence _sequence = null;
    //     bool isFirstSequence = true;
    //     _enableSequence = false;
    //     
    //     TextMeshProUGUI dialoguePosition = null;
    //     TextMeshProUGUI namePosition = null;
    //     GameObject speakingPerson = null;
    //
    //     foreach (var tags in _inkStory.currentTags)
    //     {
    //         Debug.Log(tags);
    //     }
    //
    //     foreach (string textTag in _inkStory.currentTags)
    //     {
    //         if (textTag[..3] == "dia")
    //         {
    //             switch (textTag.Remove(0, 10))
    //             {
    //                 case "left":
    //                     dialoguePosition = person1Dialogue;
    //                     namePosition = person1Name;
    //                     speakingPerson = person1Parent;
    //                     break;
    //                 case "right":
    //                     dialoguePosition = person2Dialogue;
    //                     namePosition = person2Name;
    //                     speakingPerson = person2Parent;
    //                     break;
    //             }
    //         }
    //
    //         if (textTag[..3] == "ena")
    //         {
    //             _enableSequence = true;
    //             // _sequence = DOTween.Sequence();
    //         }
    //         
    //         if (textTag[..3] == "fli")
    //         {
    //             bool isFacingLeft = true;
    //             
    //             switch (textTag.Remove(0, 6))
    //             {
    //                 case "left":
    //                     isFacingLeft = true;
    //                     break;
    //                 case "right":
    //                     isFacingLeft = false;
    //                     break;
    //             }
    //             
    //             if (_enableSequence)
    //             {
    //                 list.Add(Flip(speakingPerson.transform.parent, isFacingLeft));
    //             }
    //             else
    //             {
    //                 Flip(speakingPerson.transform.parent, isFacingLeft);
    //             }
    //             
    //             Debug.Log($"isfacingLeft: {isFacingLeft}");
    //         }
    //
    //         if (textTag[..3] == "mov")
    //         {
    //             float value = float.Parse(textTag.Remove(0, 7));
    //             
    //
    //             if (_enableSequence)
    //             {
    //                 // if (isFirstSequence)
    //                 // {
    //                 //     Debug.Log($"Append {value}");
    //                 //     _sequence.Append(MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 3));
    //                 //     isFirstSequence = false;
    //                 // }
    //                 // else
    //                 // {
    //                 //     Debug.Log($"Join {value}");
    //                 //     _sequence.Append(MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 3));
    //                 // }
    //                 
    //                 list.Add(MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 1));
    //             }
    //             else
    //             {
    //                 MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 1);
    //             }
    //             
    //         }
    //     }
    //
    //     _sequence = DOTween.Sequence();
    //
    //     foreach (Tween tween in list)
    //     {
    //         _sequence.Append(tween);
    //     }
    //
    //     _enableSequence = false;
    //     ClickContinueStory();
    // }

    // private void UpdateDialogue(string newText)
    // {
    //     person1Dialogue.text = "";
    //     person1Name.text = "";
    //     person2Dialogue.text = "";
    //     person2Name.text = "";
    //     
    //     Sequence _sequence = null;
    //     bool isFirstSequence = true;
    //     _enableSequence = false;
    //
    //     TextMeshProUGUI dialoguePosition = null;
    //     TextMeshProUGUI namePosition = null;
    //     GameObject speakingPerson = null;
    //
    //     foreach (string textTag in _inkStory.currentTags)
    //     {
    //         if (textTag[..3] == "dia")
    //         {
    //             switch (textTag.Remove(0, 10))
    //             {
    //                 case "left":
    //                     dialoguePosition = person1Dialogue;
    //                     namePosition = person1Name;
    //                     speakingPerson = person1Parent;
    //                     break;
    //                 case "right":
    //                     dialoguePosition = person2Dialogue;
    //                     namePosition = person2Name;
    //                     speakingPerson = person2Parent;
    //                     break;
    //             }
    //         }
    //         
    //         if (textTag[..3] == "per")
    //         {
    //             if (dialoguePosition == null || newText == "NA")
    //             {
    //                 
    //             };
    //             namePosition.text = textTag.Remove(0, 8);
    //         }
    //
    //         if (textTag[..3] == "fli")
    //         {
    //             bool isFacingLeft = true;
    //             
    //             switch (textTag.Remove(0, 6))
    //             {
    //                 case "left":
    //                     isFacingLeft = true;
    //                     break;
    //                 case "right":
    //                     isFacingLeft = false;
    //                     break;
    //             }
    //             
    //             if (_enableSequence)
    //             {
    //                 Flip(speakingPerson.transform.parent, isFacingLeft);
    //             }
    //             else
    //             {
    //                 Flip(speakingPerson.transform.parent, isFacingLeft);
    //             }
    //         }
    //
    //         if (textTag[..3] == "mov")
    //         {
    //             MoveX(speakingPerson.transform.parent, float.Parse(textTag.Remove(0, 7)), 3);
    //         }
    //     }
    //     
    //     if (newText != "NA") StartCoroutine(OutputDialogue(newText, dialoguePosition, speakingPerson));
    // }

    private Tween Flip(Transform objectTransform, bool isFacingLeft)
    {
        return DOTween.To(() => objectTransform.localEulerAngles.y, (value) =>
        {
            var rot = objectTransform.localEulerAngles;
            rot.y = value;
            objectTransform.localEulerAngles = rot;
        }, (isFacingLeft ? (objectTransform.localEulerAngles.y - 180f) : (objectTransform.localEulerAngles.y + 180f)), 0.3f);
    }

    private void Move(MovementController personController, string movePoint)
    {
        foreach (Transform point in movePoints)
        {
            if (point.gameObject.name == movePoint)
            {
                personController.GoTo(point);
            }
        }
    }

    private Tween MoveX(Transform objectTransform, float distance, float speed)
    {
        return objectTransform.DOMoveX(distance, speed);
    }

    // private IEnumerator OutputDialogue(string newText, TextMeshProUGUI dialogue, GameObject person)
    // {
    //     // ResetAnimation();
    //     person.transform.localEulerAngles = new Vector3(person.transform.localEulerAngles.x, person.transform.localEulerAngles.y, person.transform.localEulerAngles.z);
    //     // _bobSequence = DOTween.Sequence();
    //     // _bobSequence.Append(person.transform.DOLocalRotate(new Vector3(person.transform.localEulerAngles.x, person.transform.localEulerAngles.y, -10f), 0.1f, RotateMode.FastBeyond360)
    //     //     .SetLoops(20, LoopType.Yoyo));
    //     
    //     // dialogue.maxVisibleCharacters = 0;
    //     dialogue.text = newText;
    //
    //     // _typeWriterTween = DOVirtual.Int(0, newText.Length, newText.Length / textSpeed, v =>
    //     // {
    //     //     dialogue.maxVisibleCharacters = v;
    //     //
    //     // }).SetEase(easeType);
    //
    //     // yield return new WaitUntil(() => dialogue.maxVisibleCharacters == newText.Length);
    //     //
    //     // _bobSequence.Kill();
    //
    //     // person.transform.localEulerAngles = new Vector3(0f, person.transform.localEulerAngles.y, 0f);
    //     // person.transform.DOLocalRotate(new Vector3(person.transform.localEulerAngles.x, person.transform.localEulerAngles.y, 0f), 0.1f,
    //     //     RotateMode.Fast);
    //     
    //     // ResetAnimation();
    // }

    // private void ResetAnimation()
    // {
    //     _bobSequence.Kill();
    //     person1Parent.transform.localEulerAngles = Vector3.zero;
    //     person2Parent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    // }

    private void FadeScreen(float from, float to)
    {
        _screenFadeTween.Kill();
        
        _screenFadeTween = DOVirtual.Float(from, to, screenFadeDuration, v =>
        {
            blackScreen.alpha = v;
        });
    }

    private IEnumerator EndStory()
    {
        FadeScreen(0f, 1f);
        yield return new WaitForSeconds(screenFadeDuration);
        Debug.Log("End story and start gameplay");
    }
}
