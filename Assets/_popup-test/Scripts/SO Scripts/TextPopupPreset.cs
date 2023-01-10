using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTextPopupPreset", menuName = "Presets/Text Popup", order = 0)]
[HideMonoScript]
public class TextPopupPreset : ScriptableObject
{
    [TitleGroup("General Settings")]
    [VerticalGroup("General Settings/Split")]
    [PreviewField]
    public AudioClip popupSound;
    
    [TitleGroup("Text Settings")]
    [VerticalGroup("Text Settings/Split")]
    [Range(0.1f, 2f)]
    public float textSize = 0.8f;
    
    [VerticalGroup("Text Settings/Split")]
    public Color32 textColor = new Color32(255, 255, 255, 255);
    
    [TitleGroup("Animation Settings")]
    [VerticalGroup("Animation Settings/Split1")]
    public float fadeInDuration = 0.2f;
    
    [VerticalGroup("Animation Settings/Split1")]
    public float fadeOutDuration = 0.2f;
    
    [HorizontalGroup("Animation Settings/Split")]
    [VerticalGroup("Animation Settings/Split/Left")]
    [Title("Move")]
    public float moveOffset = -0.2f;
    
    [VerticalGroup("Animation Settings/Split/Left")]
    public float moveDuration = 0.2f;
    
    [VerticalGroup("Animation Settings/Split/Left")]
    [HideLabel]
    [EnumToggleButtons]
    public CurveType moveCurveType;

    [VerticalGroup("Animation Settings/Split/Left")]
    [ShowIf("moveCurveType", CurveType.Custom)]
    [HideIf("moveCurveType", CurveType.DOTween)]
    [HideLabel]
    public AnimationCurve customMoveCurve;
    
    [VerticalGroup("Animation Settings/Split/Left")]
    [ShowIf("moveCurveType", CurveType.DOTween)]
    [HideIf("moveCurveType", CurveType.Custom)]
    [HideLabel]
    public Ease presetMoveCurve;

    [HorizontalGroup("Animation Settings/Split")]
    
    [VerticalGroup("Animation Settings/Split/Right")]
    [Title("Scale")]
    public float scaleOffset = 0.5f;
    
    [VerticalGroup("Animation Settings/Split/Right")]
    public float scaleFrequency = 0.2f;
    
    [VerticalGroup("Animation Settings/Split/Right")]
    [HideLabel]
    [EnumToggleButtons]
    public CurveType scaleCurveType;

    [VerticalGroup("Animation Settings/Split/Right")]
    [ShowIf("scaleCurveType", CurveType.Custom)]
    [HideIf("scaleCurveType", CurveType.DOTween)]
    [HideLabel]
    public AnimationCurve customScaleCurve;
    
    [VerticalGroup("Animation Settings/Split/Right")]
    [ShowIf("scaleCurveType", CurveType.DOTween)]
    [HideIf("scaleCurveType", CurveType.Custom)]
    [HideLabel]
    public Ease presetScaleCurve;

    public enum CurveType
    {
        DOTween,
        Custom
    }

    public static IEnumerable CurveTypes = new ValueDropdownList<string>()
    {
        { "Use Preset Curves", "Use Preset Curves" },
        { "Use Custom Curves", "Use Custom Curves" }
    };
}
    
