using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

namespace Core.Words {
    public class WordStack : MonoBehaviour {
        [SerializeField] private Transform wordUIGameObject;
        [SerializeField] private float duration;

        private readonly Queue<Transform> _wordStack = new();
        
        // TODO, using Object Pool
        // private ObjectPool<Transform> _wordPool;
        
        private readonly Vector3 _offset = new(-.1f, .1f, 0);
        private const float MaxRotation = 30f;

        private void Start() {
            /*_wordPool = new ObjectPool<Transform>(
                () => Instantiate(wordUIGameObject, transform), 
                word => {
                    word.gameObject.SetActive(true);
                    word.name = wordUIGameObject.name;
                    word.SetLocalPositionAndRotation(new Vector3(0,0,0),quaternion.identity);
                    GetComponentInChildren<CanvasGroup>().alpha = 1;
                },word => word.gameObject.SetActive(false)
                ,word => Destroy(word.gameObject)
                ,false,10,20);*/
        }
        
        public void CreateStack() {
            var wordStack = _wordStack.ToArray();
            for (var i = 0; i < _wordStack.Count; i++) {
                wordStack[i].DOLocalMove( _offset * (_wordStack.Count - i), duration * 0.5f);
            }

            var wordUI = Instantiate(wordUIGameObject, transform);
            wordUI.name = wordUIGameObject.name;

            var sequence = DOTween.Sequence();
            sequence.Append(wordUIGameObject.DORotate(new Vector3(0, 0, -MaxRotation), duration));
            sequence.Join(wordUIGameObject.GetComponentInChildren<CanvasGroup>().DOFade(0.5f, duration));
            
            const float yOffset = 0.2f;
            sequence.Join(wordUIGameObject.DOLocalMoveY(yOffset, duration));
            sequence.Append(wordUIGameObject.DOLocalMove(_offset, duration * 0.5f));
            sequence.Join(wordUIGameObject.DORotate(new Vector3(0, 0, 0), duration * 0.5f));

            _wordStack.Enqueue(wordUIGameObject);
            wordUIGameObject = wordUI;
        }

        public void SetReceiveInput(bool value) {
            wordUIGameObject.GetComponent<WordObject>().ReceiveInput = value;
        }

        public void RemoveStack() {
            if (_wordStack.TryDequeue(out var stack)) {
                var sequence = DOTween.Sequence();
                sequence.Append(stack.DORotate(new Vector3(0, 0, -MaxRotation * 3), duration));
                sequence.Join(stack.GetComponentInChildren<CanvasGroup>().DOFade(0, duration * 2));
                sequence.OnComplete(() => {
                    //_wordPool.Release(stack);
                    Destroy(stack.gameObject);
                });
            }
        }
    }
}