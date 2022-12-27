using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

[Serializable]
public class IntFunc : SerializableCallback<int> { }
[Serializable]
public class FloatFunc : SerializableCallback<float> { }
[Serializable]
public class StringFunc : SerializableCallback<string> { }
[Serializable]
public class BoolFunc : SerializableCallback<bool> { }
[Serializable]
public class UniTaskFunc : SerializableCallback<CancellationToken, UniTask> { }

//public static class SerializableCallbackUtil
//{
//    public static void SetCallback<TResult>(this SerializableCallback<TResult> sc, Func<TResult> fun)
//    {
//        var ic = new InvokableCallback<TResult>(null, null);
//        ic.func = fun;
//        sc.func = ic;
//    }

//    public static void SetCallback<T0, TResult>(this SerializableCallback<T0, TResult> sc, Func<T0, TResult> fun)
//    {
//        var ic = new InvokableCallback<T0, TResult>(null, null);
//        ic.func = fun;
//        sc.func = ic;
//    }
//}