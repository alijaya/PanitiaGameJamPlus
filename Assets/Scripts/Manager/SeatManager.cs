using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

public class SeatManager : SingletonSceneMB<SeatManager>
{
    private HashSet<Seat> seats = new ();
    private Dictionary<Seat, UnityAction<CustomerGroup>> seatOccupiedHandlers = new ();
    private Dictionary<Seat, UnityAction<CustomerGroup>> seatUnoccupiedHandlers = new ();

    public UnityEvent<Seat, CustomerGroup> OnSeatOccupied = new ();
    public UnityEvent<Seat, CustomerGroup> OnSeatUnoccupied = new ();

    public void RegisterSeat(Seat seat)
    {
        if (seats.Add(seat))
        {
            UnityAction<CustomerGroup> occupiedHandler = customerGroup =>
            {
                OnSeatOccupied.Invoke(seat, customerGroup);
            };
            UnityAction<CustomerGroup> unoccupiedHandler = customerGroup =>
            {
                OnSeatUnoccupied.Invoke(seat, customerGroup);
            };

            seat.OnSeatOccupied.AddListener(occupiedHandler);
            seat.OnSeatUnoccupied.AddListener(unoccupiedHandler);

            seatOccupiedHandlers[seat] = occupiedHandler;
            seatUnoccupiedHandlers[seat] = unoccupiedHandler;
        }
    }

    public void UnregisterSeat(Seat seat)
    {
        if (seats.Remove(seat))
        {
            if (seatOccupiedHandlers.TryGetValue(seat, out var occupiedHandler))
            {
                seat.OnSeatOccupied.RemoveListener(occupiedHandler);
                seatOccupiedHandlers.Remove(seat);
            }

            if (seatUnoccupiedHandlers.TryGetValue(seat, out var unoccupiedHandler))
            {
                seat.OnSeatUnoccupied.RemoveListener(unoccupiedHandler);
                seatUnoccupiedHandlers.Remove(seat);
            }
        }
    }

    public Seat GetAvailableSeat()
    {
        return seats.Where(seat => !seat.occupied && !seat.waitOccupied).GetRandom();
    }

    public Seat GetAvailableWaitingSeat()
    {
        return seats.Where(seat => !seat.waitOccupied).GetRandom();
    }

    public Seat GetAvailableSeat(CustomerGroup customerGroup)
    {
        return GetAvailableSeat(customerGroup.count);
    }

    public Seat GetAvailableSeat(int customerCount)
    {
        var results = seats.Where(seat => seat.CouldSeat(customerCount));

        if (results.Count() > 0) return results.Aggregate((seatA, seatB) => seatA.count <= seatB.count ? seatA : seatB);
        return null;
    }

    public Seat GetAvailableWaitingSeat(CustomerGroup customerGroup)
    {
        return GetAvailableWaitingSeat(customerGroup.count);
    }

    public Seat GetAvailableWaitingSeat(int customerCount)
    {
        var results = seats.Where(seat => seat.CouldWait(customerCount));

        if (results.Count() > 0) return results.Aggregate((seatA, seatB) => seatA.count <= seatB.count ? seatA : seatB);
        return null;
    }
}
