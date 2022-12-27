using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Dish {
    public class DishRequester : MonoBehaviour {

        public List<DishItemSO> requestedDishes;

        public List<bool> completes { get; private set; }

        public Core.Words.WordObject wordObject;

        public UnityEvent OnDishesChanged;
        public UnityEvent OnRequestCompleted;

        public void Setup(List<DishItemSO> requestedDishes)
        {
            this.requestedDishes = requestedDishes;
        }

        private void Start()
        {
            completes = new();
            for (var i = 0; i < requestedDishes.Count; i++) completes.Add(false);
            OnDishesChanged?.Invoke();
        }

        public void AttemptToFill() {
            for (var i = 0; i < requestedDishes.Count; i++)
            {
                var dish = requestedDishes[i];
                if (!completes[i])
                {
                    if (Tray.I.RemoveDish(dish))
                    {
                        completes[i] = true;
                        OnDishesChanged?.Invoke();
                    }
                }
            }

            if (completes.All(complete => complete))
            {
                OnRequestCompleted?.Invoke();
            }
        }

        public static DishRequester Spawn(List<DishItemSO> requestedDishes, Core.Words.Generator.ITextGenerator generator, List<Core.Words.Modifier.ITextModifier> modifiers, Transform parent)
        {
            var result = GameObject.Instantiate(GlobalRef.I.DishRequesterPrefab, parent);
            result.Setup(requestedDishes);

            // eugh... maybe refactor nanti?
            var wordObject = result.wordObject;
            wordObject.TextGenerator = generator;
            if (modifiers != null) wordObject.TextModifiers = modifiers;

            return result;
        }

        public static (UniTask, DishRequester) SpawnAsync(List<DishItemSO> requestedDishes, Core.Words.Generator.ITextGenerator generator, List<Core.Words.Modifier.ITextModifier> modifiers, Transform parent, CancellationToken ct = default)
        {
            var result = Spawn(requestedDishes, generator, modifiers, parent);

            async UniTask task()
            {
                await result.OnRequestCompleted.ToUniTask(ct).SuppressCancellationThrow();
                // suppress cancellation so will always destroy this
                if (result) Destroy(result.gameObject);
            }

            return (task(), result);
        }

    }
}