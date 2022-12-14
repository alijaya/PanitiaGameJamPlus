using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SeatManager : SingletonMB<SeatManager>
{
    private HashSet<Seat> seats = new ();

    public void RegisterSeat(Seat seat)
    {
        seats.Add(seat);
    }

    public void UnregisterSeat(Seat seat)
    {
        seats.Remove(seat);
    }

    public Seat GetAvailableSeat()
    {
        return seats.Where(seat => !seat.occupied).GetRandom();
    }

    public Seat GetAvailableSeat(CustomerGroup customerGroup)
    {
        return GetAvailableSeat(customerGroup.count);
    }

    public Seat GetAvailableSeat(int customerCount)
    {
        return seats.Where(seat => !seat.occupied && seat.count >= customerCount).Aggregate((seatA, seatB) => seatA.count <= seatB.count ? seatA : seatB);
    }
}
