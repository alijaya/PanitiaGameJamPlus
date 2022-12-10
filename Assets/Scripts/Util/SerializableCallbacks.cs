using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

[Serializable]
public class IntFunc : SerializableCallback<int> { }
[Serializable]
public class FloatFunc : SerializableCallback<float> { }
[Serializable]
public class StringFunc : SerializableCallback<string> { }
[Serializable]
public class BoolFunc : SerializableCallback<bool> { }
[Serializable]
public class UniTaskFunc : SerializableCallback<UniTask> { }