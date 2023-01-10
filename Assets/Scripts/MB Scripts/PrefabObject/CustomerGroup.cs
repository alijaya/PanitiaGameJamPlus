using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Core.Dish;

public class CustomerGroup : MonoBehaviour
{
    public static class CustomerState
    {
        public static readonly string None = "None";
        public static readonly string GoToWait = "GoToWait";
        public static readonly string WaitForSeat = "WaitForSeat";
        public static readonly string GoToSeat = "GoToSeat";
        public static readonly string GoToQueue = "GoToQueue";
        public static readonly string WaitForCashier = "WaitForCashier";
        public static readonly string GoToCashier = "GoToCashier";
        public static readonly string Think = "Think";
        public static readonly string Order = "Order";
        public static readonly string Eat = "Eat";
        public static readonly string Pay = "Pay";
        public static readonly string Leave = "Leave";
    };

    // TODO: need to refactor supaya ga static
    public static float delaySpawn = 0.3f; // seconds
    public static float customerUIHeight = 2f; // unit

    public CustomerTypeSO customerType;
    public bool dineIn = false; // true: dine in, false: take out
    public int count = 1;

    public Transform spawnPoint;

    public string state { get; private set; } = CustomerState.None;

    public List<Customer> customers { get; private set; } = new List<Customer>();
    public List<DishItemSO> dishes { get; private set; } = new List<DishItemSO>();

    public int totalDishPrice
    {
        get
        {
            return dishes.Aggregate(0, (total, dish) => total + dish.price);
        }
    }

    public Customer firstCustomer
    {
        get
        {
            return customers[0];
        }
    }

    public float countdown { get; private set; } = 0;
    private bool countdownEnabled = false;
    public bool isTimeout
    {
        get
        {
            return customerType && countdown <= 0;
        }
    }

    public float waitDuration
    {
        get
        {
            return customerType?.WaitDuration ?? -1;
        }
    }

    public float patience
    {
        get
        {
            return waitDuration > 0 ? countdown / waitDuration : 0;
        }
    }

    public UnityEvent OnTimeout;

    private CancellationTokenSource countdownCancel;
    private Seat seat;
    private Seat waitingSeat;
    private Cashier cashier;
    private int queueNumber;

    private CustomerUI customerUI;

    static public CustomerGroup Spawn(CustomerTypeSO customerType, Transform spawnPoint)
    {
        return Spawn(customerType, false, 1, spawnPoint);
    }

    static public CustomerGroup Spawn(CustomerTypeSO customerType, int count, Transform spawnPoint)
    {
        return Spawn(customerType, true, count, spawnPoint);
    }

    static public CustomerGroup Spawn(CustomerTypeSO customerType, bool dineIn, int count, Transform spawnPoint)
    {
        var go = new GameObject(customerType.name);
        var customerGroup = go.AddComponent<CustomerGroup>();
        customerGroup.Setup(customerType, dineIn, count, spawnPoint);
        return customerGroup;
    }

    public void Setup(CustomerTypeSO customerType, Transform spawnPoint)
    {
        Setup(customerType, false, 1, spawnPoint);
    }

    public void Setup(CustomerTypeSO customerType, int count, Transform spawnPoint)
    {
        Setup(customerType, true, count, spawnPoint);
    }

    public void Setup(CustomerTypeSO customerType, bool dineIn, int count, Transform spawnPoint)
    {
        this.customerType = customerType;
        this.spawnPoint = spawnPoint;
        this.dineIn = dineIn;
        this.count = count > 0 ? count : 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!customerType)
        {
            // error :(
            Destroy(gameObject);
            return;
        }

        // Spawn CustomerUI
        customerUI = Instantiate(GlobalRef.I.CustomerUIPrefab, transform);

        var customerCount = dineIn ? count: 1; // force 1 customer if takeout
        for (var i = 0; i < customerCount; i++)
        {
            var customer = Instantiate(customerType.prefabs.GetRandom());
            customer.Setup(this);
            customer.SetToWorld(spawnPoint);
            customers.Add(customer);
        }

        // start the whole debacle of this algorithm
        countdownCancel = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        if (dineIn)
        {
            DineInBehavior(countdownCancel.Token).Forget();
        } else
        {
            TakeOutBehavior(countdownCancel.Token).Forget();
        }
    }

    private void OnDestroy()
    {
        // destroy all customer in this group
        foreach (var customer in customers)
        {
            if (customer) Destroy(customer.gameObject);
        }
    }

    // not sure mending pake State Machine atau kagak... 👀
    private async UniTask DineInBehavior(CancellationToken ct)
    {
        ResetCountdown();

        // First Search The Seat

        await FindSeat(ct);

        // they all are arrived, then wait for ordering menu.
        // should play some animation and popup, but well later
        PauseCountdown();
        state = CustomerState.Think;
        customerUI.ShowThink();
        await UniTask.Delay(TimeSpan.FromSeconds(customerType.OrderDuration), cancellationToken: ct);
        customerUI.HideThink();

        // ordering
        await OrderBehavior(seat.serveLocation, ct);

        // actually eating
        PauseCountdown();
        state = CustomerState.Eat;
        customerUI.ShowThink();
        await UniTask.Delay(TimeSpan.FromSeconds(customerType.EatDuration), cancellationToken: ct);
        customerUI.HideThink();

        // finish eating, go to cashier
        await FindCashier(ct);

        // arriving at cashier
        await PayBehavior(ct);
    }

    private async UniTask FindSeat(CancellationToken ct)
    {
        // First Search The Seat
        // hopefully found one, if not, the customer need to wait

        seat = SeatManager.I.GetAvailableSeat(this);

        // If don't get any seat, well please wait to waiting point
        if (!seat)
        {
            waitingSeat = SeatManager.I.GetAvailableWaitingSeat(this);

            // just wait until get one
            while (!waitingSeat)
            {
                await UniTask.Yield(ct);
                waitingSeat = SeatManager.I.GetAvailableWaitingSeat(this);
            }

            // booking the waiting seat
            waitingSeat.WaitCustomerGroup(this);
            // then move the customer
            var goToWaitTasks = new List<UniTask>();
            for (var i = 0; i < customers.Count; i++)
            {
                var customer = customers[i];
                var position = waitingSeat.waitLocations[i];
                goToWaitTasks.Add(AsyncUtil.DelayTask(delaySpawn * i, () => customer.GoToPOI(position, ct), ct));
            }
            // await all until they arrived
            PauseCountdown();
            state = CustomerState.GoToWait;
            await UniTask.WhenAll(goToWaitTasks);

            // let's wait until there's a seat available
            // play timer
            PlayCountdown();
            state = CustomerState.WaitForSeat;
            seat = await WaitUntilGetAvailableSeat(ct);
        }

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
            goToTasks.Add(AsyncUtil.DelayTask(delaySpawn * i, () => customer.GoToPOI(position, ct), ct));
        }

        // await all until they arrived
        PauseCountdown();
        state = CustomerState.GoToSeat;
        await UniTask.WhenAll(goToTasks);
    }

    private async UniTask<Seat> WaitUntilGetAvailableSeat(CancellationToken ct)
    {
        while (true)
        {
            Seat availableSeat;
            // bisa jadi pas awal nunggu, mejanya langsung kosong
            // jadi ga perlu nunggu
            if (!waitingSeat.occupied)
            {
                availableSeat = waitingSeat;
            } else
            {
                // kalau enggak, mungkin ada meja lain yang langsung kosong?
                availableSeat = SeatManager.I.GetAvailableSeat(this);

                if (!availableSeat)
                {
                    // kalau bener2 nggak ada, baru nunggu
                    availableSeat = (await SeatManager.I.OnSeatUnoccupied.ToUniTask(ct)).Item1;
                }
            }

            if (availableSeat == waitingSeat || availableSeat.CouldSeat(this))
            {
                // if it's the same seat, then we go there first
                // or if it's empty seat without anything waiting, then we go there

                // unbook the waitingSeat
                waitingSeat.UnwaitCustomerGroup(this);
                waitingSeat = null;
                return availableSeat;
            }
        }
    }

    // not sure mending pake State Machine atau kagak... 👀
    private async UniTask TakeOutBehavior(CancellationToken ct)
    {
        ResetCountdown();

        // First Search The Cashier
        await FindCashier(ct);

        // has arrived, then wait for ordering menu.
        // should play some animation and popup, but well later
        PauseCountdown();
        state = CustomerState.Think;
        customerUI.ShowThink();
        await UniTask.Delay(TimeSpan.FromSeconds(customerType.OrderDuration), cancellationToken: ct);
        customerUI.HideThink();

        // ordering
        await OrderBehavior(cashier.serveLocation, ct);

        // no eating, directly paying
        await PayBehavior(ct);
    }

    private async UniTask FindCashier(CancellationToken ct)
    {
        PointOfInterest position;

        cashier = CashierManager.I.GetAvailableCashierQueue();

        while (!cashier)
        {
            await UniTask.Yield(ct);
            cashier = CashierManager.I.GetAvailableCashierQueue();
        }

        // booking the cashier queue
        queueNumber = cashier.QueueCustomerGroup(this);
        position = cashier.queueLocations[queueNumber];

        // then move the customer
        // await until arrived
        PauseCountdown();
        state = CustomerState.GoToQueue;
        await firstCustomer.GoToPOI(position, ct);

        // let's wait until there's a cashier available
        // play timer
        PlayCountdown();
        // hide patience display
        customerUI.HidePatience();
        state = CustomerState.WaitForCashier;
        cashier = await WaitUntilGetAvailableCashier(ct);

        // Second, Go To The Cashier

        // move the customer
        position = cashier.location;

        // await until arrived
        PauseCountdown();
        state = CustomerState.GoToCashier;
        await firstCustomer.GoToPOI(position, ct);
    }

    private async UniTask<Cashier> WaitUntilGetAvailableCashier(CancellationToken ct)
    {
        // check udah paling depan belum
        // check ada kasir lain kosong ga
        // check berubah ga nomor antriannya
        // kalau ga artinya blom ada yang berubah, well nunggu ada kasir kelar
        while (true)
        {
            var lastQueueNumber = queueNumber;
            queueNumber = cashier.QueueCustomerGroup(this);
            if (queueNumber == 0)
            {
                return cashier;
            }

            Cashier availableCashier;

            // mungkin ada cashier lain yang langsung kosong?
            availableCashier = CashierManager.I.GetAvailableCashier();

            if (availableCashier)
            {
                cashier.UnqueueCustomerGroup(this);
                cashier = availableCashier;
                queueNumber = cashier.QueueCustomerGroup(this);
                return availableCashier;
            }

            // kalau bisa maju
            if (lastQueueNumber != queueNumber)
            {
                var position = cashier.queueLocations[queueNumber];
                await firstCustomer.GoToPOI(position, ct);
                continue;
            }

            if (!availableCashier)
            {
                // kalau bener2 nggak ada, baru nunggu
                await CashierManager.I.OnCashierUnqueued.ToUniTask(ct);
                continue;
            }

        }
    }

    private async UniTask OrderBehavior(PointOfInterest serveLocation, CancellationToken ct)
    {
        dishes = RestaurantManager.I.GenerateDishes(count);
        Debug.Log(count);
        Debug.Log(String.Join(", ", dishes));
        var task = customerUI.RequestDish(this, serveLocation, ct);
        PlayCountdown();
        state = CustomerState.Order;
        await task;
    }

    private async UniTask PayBehavior(CancellationToken ct)
    {
        PlayCountdown();
        state = CustomerState.Pay;
        customerUI.ShowPay();
        await RestaurantManager.I.OnCashierTriggered.ToUniTask(ct);
        customerUI.HidePay();
        RestaurantManager.I.HandleCustomerPay(this);

        // done, leave, don't need to await
        //Debug.Log("happy");
        LeaveBehavior(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask LeaveBehavior(CancellationToken ct)
    {
        PauseCountdown();
        state = CustomerState.Leave;
        customerUI.HideAll();

        bool cancelled = false;

        // First, Go To Door

        // unseat the seat first, so the other could take this seat
        if (waitingSeat) waitingSeat.UnwaitCustomerGroup(this);
        if (seat) seat.UnseatCustomerGroup(this);

        // unoccupy the cashier, so the other could take this cashier
        if (cashier) cashier.UnqueueCustomerGroup(this);

        seat = null;
        waitingSeat = null;
        cashier = null;
        cashier = null;

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
        OnTimeout?.Invoke();

        LeaveBehavior(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void Update()
    {
        //var center = customers.Aggregate(Vector3.zero, (acc, customer) => acc + customer.transform.position) / customers.Count;
        var center = firstCustomer.transform.position;
        transform.position = center + new Vector3(0, customerUIHeight, 0);
        if (countdownEnabled && !isTimeout)
        {
            countdown -= Time.deltaTime;
            customerUI.RefreshPatience(this);
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
        customerUI.ShowPatience();
    }

    public void PauseCountdown()
    {
        countdownEnabled = false;
        customerUI.HidePatience();
    }

    public void ResetCountdown()
    {
        if (customerType) countdown = customerType.WaitDuration;
    }
}
