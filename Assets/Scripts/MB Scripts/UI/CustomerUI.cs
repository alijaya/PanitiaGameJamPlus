using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CustomerUI : MonoBehaviour
{
    public Core.Dish.DishRequester dishRequester;
    public Core.Words.WordObject wordObject;
    public PatienceIndicatorUI patienceIndicator;
    public GameObject thinkUI;
    public GameObject payUI;

    private void Start()
    {
        HideAll();
    }

    public void HideAll()
    {
        HideWordObject();
        HideDishRequester();
        HideThink();
        HidePay();
        HidePatience();
    }

    public void ShowWordObject()
    {
        wordObject.gameObject.SetActive(true);
    }

    public void HideWordObject()
    {
        wordObject.gameObject.SetActive(false);
    }

    public void ShowDishRequester()
    {
        dishRequester.gameObject.SetActive(true);
    }

    public void HideDishRequester()
    {
        dishRequester.gameObject.SetActive(false);
    }

    public void ShowPatience()
    {
        patienceIndicator.gameObject.SetActive(true);
    }

    public void HidePatience()
    {
        patienceIndicator.gameObject.SetActive(false);
    }

    public void ShowThink()
    {
        thinkUI.SetActive(true);
    }

    public void HideThink()
    {
        thinkUI.SetActive(false);
    }

    public void ShowPay()
    {
        payUI.SetActive(true);
    }

    public void HidePay()
    {
        payUI.SetActive(false);
    }

    public async UniTask RequestDish(CustomerGroup customerGroup, PointOfInterest servePOI, CancellationToken ct = default)
    {
        var count = customerGroup.count;
        var requestedDishes = customerGroup.dishes;
        var customerType = customerGroup.customerType;
        var generator = customerType.TextGenerator;
        var modifiers = customerType.TextModifiers;

        dishRequester.Setup(requestedDishes);

        wordObject.TextGenerator = generator;
        wordObject.TextModifiers = modifiers;
        wordObject.SetCompleteCheck((ct) => Chef.I.GoToPOI(servePOI, ct));
        wordObject.OnWordCompleted.AddListener(dishRequester.AttemptToFill);

        try
        {
            ShowDishRequester();
            ShowWordObject();
            await dishRequester.OnRequestCompleted.ToUniTask(ct);
        } finally
        {
            wordObject.OnWordCompleted.RemoveListener(dishRequester.AttemptToFill);
            HideDishRequester();
            HideWordObject();
        }
    }

    public async UniTask RequestCome(CustomerGroup customerGroup, PointOfInterest servePOI, CancellationToken ct = default)
    {
        var customerType = customerGroup.customerType;
        var generator = customerType.TextGenerator;
        var modifiers = customerType.TextModifiers;

        wordObject.TextGenerator = generator;
        wordObject.TextModifiers = modifiers;
        wordObject.SetCompleteCheck((ct) => Chef.I.GoToPOI(servePOI, ct));

        try
        {
            ShowWordObject();
            await wordObject.OnWordCompleted.ToUniTask(ct);
        }
        finally
        {
            HideWordObject();
        }
    }

    public void RefreshPatience(float value)
    {
        patienceIndicator.Value = value;
    }

    public void RefreshPatience(CustomerGroup customerGroup)
    {
        RefreshPatience(customerGroup.patience);
    }
}
