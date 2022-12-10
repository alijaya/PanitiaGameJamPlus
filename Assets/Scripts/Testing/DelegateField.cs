using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class DelegateField : MonoBehaviour
{
    public BoolFunc fun;

    [SerializeReference]
    public ITest inter;

    public List<BoolFunc> funs;

    [SerializeReference]
    public List<ITest> inters;
}