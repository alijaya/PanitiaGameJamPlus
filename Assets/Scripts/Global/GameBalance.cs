using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;

public class GameBalance : SingletonSO<GameBalance>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        // auto create asset
        LoadOrCreateInstance<GameBalance>();
    }

    public FloatReference difficulty;

    public FloatReference walkSpeed;
    public FloatReference waitDuration;
    public FloatReference orderDuration;
    public FloatReference eatDuration;
    public FloatReference bonusTip;
    public FloatReference foodDifficulty;
    public FloatReference wordDifficulty;
}
