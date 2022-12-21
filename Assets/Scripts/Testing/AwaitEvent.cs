using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class AwaitEvent : MonoBehaviour
{
    public ClickInvokeEvent invokeEvent;

    private CancellationTokenSource cts = new CancellationTokenSource();

    // Start is called before the first frame update
    void Start()
    {
        StartAsync().Forget();
    }

    async UniTask StartAsync()
    {
        Debug.Log("start");

        while (true)
        {
            await invokeEvent.OnClick.ToUniTask(cts.Token);
            Debug.Log("clicked");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            cts.Cancel();
        }
    }
}
