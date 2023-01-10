using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private IntVariable totalSales;
    [SerializeField] private TextPopupPreset preset;

    private TextMeshProUGUI _scoreText;
    private CanvasGroup _cg;
    private Transform _pivot;
    
    private void Awake()
    {
        if (preset == null)
        {
            Debug.LogError($"Popup preset for {gameObject.name} not found.");    
        }
        
        switch (preset.moveCurveType)
        {
            case TextPopupPreset.CurveType.Custom:
                break;
            case TextPopupPreset.CurveType.DOTween:
                break;
        }
        
        switch (preset.scaleCurveType)
        {
            case TextPopupPreset.CurveType.Custom:
                break;
            case TextPopupPreset.CurveType.DOTween:
                break;
        }
        
        _scoreText = GetComponent<TextMeshProUGUI>();
        _pivot = transform.parent;
        _cg = _pivot.GetComponentInParent<CanvasGroup>();

        _cg.alpha = 0f;
    }

    private IEnumerator Start()
    {
        _scoreText.text = (totalSales.Value - totalSales.OldValue).ToString();
        _scoreText.fontSize = preset.textSize;

        if (preset.scaleCurveType == TextPopupPreset.CurveType.Custom)
        {
            _pivot.DOScaleY(_pivot.localScale.y + preset.scaleOffset, preset.scaleFrequency).SetEase(preset.customScaleCurve).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _pivot.DOScaleY(_pivot.localScale.y + preset.scaleOffset, preset.scaleFrequency).SetEase(preset.presetScaleCurve).SetLoops(-1, LoopType.Yoyo);
        }

        if (preset.moveCurveType == TextPopupPreset.CurveType.Custom)
        {
            transform.DOMoveY(transform.position.y + preset.moveOffset, preset.moveDuration).SetEase(preset.customMoveCurve);
        }
        else
        {
            transform.DOMoveY(transform.position.y + preset.moveOffset, preset.moveDuration).SetEase(preset.presetMoveCurve);
        }
        
        DOVirtual.Float(0, 1f, preset.fadeInDuration, v => _cg.alpha = v);
        
        yield return new WaitForSeconds(1f);
        
        DOVirtual.Float(1, 0f, preset.fadeOutDuration, v => _cg.alpha = v);
        Destroy(_pivot.parent.gameObject, 1f + preset.fadeOutDuration);
    }
}
