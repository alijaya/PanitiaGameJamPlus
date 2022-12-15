using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class CustomerGroup : MonoBehaviour
{
    public static float delaySpawn = 0.3f; // seconds

    public CustomerTypeSO customerType;
    public int count = 1;

    public Transform spawnPoint;

    public List<Customer2> customers { get; private set; } = new List<Customer2>();

    public float countdown { get; private set; } = 0;
    public bool countdownEnabled = false;
    public bool isTimeout
    {
        get
        {
            return customerType && countdown <= 0;
        }
    }

    public UnityEvent OnTimeout;

    private CancellationTokenSource countdownCancel;
    private Seat seat;

    public void Setup(CustomerTypeSO customerType, Transform spawnPoint)
    {
        this.customerType = customerType;
        this.spawnPoint = spawnPoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!customerType) return;
        for (var i = 0; i < count; i++)
        {
            var customer = Instantiate(customerType.prefabs.GetRandom());
            customer.Setup(this);
            customer.SetToWorld(spawnPoint);
            customers.Add(customer);
        }

        // start the whole debacle of this algorithm
        countdownCancel = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        RunBehavior(countdownCancel.Token).Forget();
    }

    private void OnDestroy()
    {
        // destroy all customer in this group
        foreach (var customer in customers)
        {
            if (customer) Destroy(customer.gameObject);
        }
    }

    private async UniTask RunBehavior(CancellationToken ct)
    {
        ResetCountdown();
        bool cancelled = false;
        // First Search The Seat
        // hopefully found one, if not... don't know, should be error :')
        // let's handle that later

        seat = SeatManager.I.GetAvailableSeat(this);

        // Second, Go To The Seat
        // like all the customer in this group, find their seat
        // using the power of Async

        // booking the seat first, so the other doesn't take this seat
        seat.SeatCustomerGroup(this);
        // then move the customer
        var goToTasks = new List<UniTask>();
        for (var i = 0; i < customers.Count; i++)
        {
            var customer = customers[i];
            var position = seat.locations[i];
            goToTasks.Add(AsyncUtil.DelayTask(delaySpawn*i, () => customer.GoToPOI(position, ct), ct));
        }

        // await all until they arrived
        PauseCountdown();
        Debug.Log("entering");
        cancelled = await UniTask.WhenAll(goToTasks).SuppressCancellationThrow();
        if (cancelled || ct.IsCancellationRequested)
        {
            return; // umm something weird happened
        }

        // they all are arrived, then wait for ordering menu.
        // should play some animation and popup, but well later
        PauseCountdown();
        Debug.Log("thinking");
        await UniTask.Delay(TimeSpan.FromSeconds(customerType.OrderDuration), cancellationToken: ct);

        UniTask task;
        Core.Words.WordObject wordObject;

        // ordering
        // create fake order, and wait for interaction
        (task, wordObject) = Core.Words.WordObject.SpawnConstantAsync("order", transform, ct);
        PlayCountdown();
        Debug.Log("ordering");
        await task;

        // actually eating
        PauseCountdown();
        Debug.Log("eating");
        await UniTask.Delay(TimeSpan.FromSeconds(customerType.OrderDuration), cancellationToken: ct);

        // create fake paying, and wait for interaction
        (task, wordObject) = Core.Words.WordObject.SpawnConstantAsync("pay", transform, ct);
        PlayCountdown();
        Debug.Log("paying");
        await task;

        // done, leave, don't need to await
        Debug.Log("happy");
        LeaveBehavior(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask LeaveBehavior(CancellationToken ct)
    {
        PauseCountdown();
        Debug.Log("leaving");

        bool cancelled = false;

        // First, Go To Door

        // unseat the seat first, so the other could take this seat
        seat.UnseatCustomerGroup();
        // then move the customer
        var goToTasks = new List<UniTask>();
        for (var i = 0; i < customers.Count; i++)
        {
            var customer = customers[i];
            goToTasks.Add(customer.GoToWorld(spawnPoint, ct));
        }

        // await all until they arrived
        cancelled = await UniTask.WhenAll(goToTasks).SuppressCancellationThrow();
        if (cancelled || ct.IsCancellationRequested)
        {
            return; // umm something weird happened
        }

        // they all are leaved
        // DESTROYYZZ

        Destroy(gameObject);
    }

    private void OnTimeoutInternal()
    {
        countdownCancel?.Cancel();
        countdownCancel?.Dispose();
        countdownCancel = null;
        OnTimeout.Invoke();

        LeaveBehavior(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void Update()
    {
        if (countdownEnabled && !isTimeout)
        {
            countdown -= Time.deltaTime;
            if (isTimeout)
            {
                PauseCountdown();
                OnTimeoutInternal();
            }
        }
    }

    public void PlayCountdown()
    {
        // if it's more than it should be, then don't play
        if (isTimeout) return;
        countdownEnabled = true;
    }

    public void PauseCountdown()
    {
        countdownEnabled = false;
    }

    public void ResetCountdown()
    {
        if (customerType) countdown = customerType.WaitDuration;
    }
}
