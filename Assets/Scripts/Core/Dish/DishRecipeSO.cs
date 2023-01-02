using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Core.Dish {
    [CreateAssetMenu(fileName = "New Dish Recipe", menuName = "Dish/Recipe", order = 0)]
    public class DishRecipeSO : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] private List<RecipeNode> nodes = new();
        private readonly Dictionary<RecipeNode, IEnumerable<RecipeNode>> _childrenLookup = new();

        #region Nodes

        private void OnValidate() {
            _childrenLookup.Clear();
            foreach (var node in nodes) {
                _childrenLookup[node] = nodes.Where(x => node.children.Contains(x.name));
            }
        }

        public IEnumerable<RecipeNode> GetAllNodes() {
            return nodes;
        }
        public IEnumerable<RecipeNode> GetAllChildren(RecipeNode parentNode) {
            return _childrenLookup.TryGetValue(parentNode, out var children) ? children : null;
        }
        public RecipeNode GetBaseNode(IngredientItemSO baseIngredient) {
            return GetAllNodes().FirstOrDefault(x => x.ancestors.Count == 0 && x.input == baseIngredient);
        }
        public void CreateNode(RecipeNode parentNode) {
            var newNode = CreateInstance<RecipeNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Recipe Node");
            
            if (parentNode) {
                parentNode.children.Add(newNode.name); 
                newNode.ancestors.AddRange(parentNode.ancestors);
                newNode.ancestors.Add(parentNode.name);

                var offset = new Vector2(newNode.rect.width * 1.5f, 0);
                newNode.rect.position = parentNode.rect.position + offset;
            }
            
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(RecipeNode nodeToDelete) {
            nodes.Remove(nodeToDelete);
            OnValidate();
            foreach (var node in GetAllNodes()) {
                node.children.Remove(nodeToDelete.name);
            }
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
        
        public void OnBeforeSerialize() {
            if (nodes.Count == 0) {
                CreateNode(null);
            }
            
            if (AssetDatabase.GetAssetPath(this) != "") {
                foreach (var node in GetAllNodes()) {
                    if (AssetDatabase.GetAssetPath(node) == "") {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
        }

        public void OnAfterDeserialize() {
            
        }

        #endregion
        public IEnumerable<IngredientItemSO> GetBaseIngredients() {
            return GetAllNodes().Where(x => x.ancestors.Count == 0).Select(x => x.input);
        }

    }
    
    public class RecipeNode: ScriptableObject {
        public IngredientItemSO input;
        public TrayItemSO output;
        [HideInInspector] public List<string> children = new ();
        [HideInInspector] public List<string> ancestors = new();
        
        [HideInInspector] public Rect rect = new Rect(0,0, 200,100);
    }

    [System.Serializable]
    public class Recipe {
        [SerializeField] private Disjunction[] and;
        public bool Check(int ingredientOrder, IngredientItemSO ingredientItem, out TrayItemSO finalOutput) {
            return and[ingredientOrder].Check(ingredientItem, out finalOutput);
        }

        public IEnumerable<IngredientItemSO> GetIngredientsAt(int ingredientOrder) {
            return and[ingredientOrder].GetIngredients();
        }

        public int GetRecipeStep() => and.Length;


        [System.Serializable]
        private class Disjunction {
            [SerializeField] private Predicate[] or;

            public bool Check(IngredientItemSO ingredientItem, out TrayItemSO finalOutput) {
                foreach (var predicate in or) {
                    if (predicate.Check(ingredientItem, out finalOutput)) return true;
                }
                finalOutput = null;
                return false;
            }

            public IEnumerable<IngredientItemSO> GetIngredients() {
                return or.Select(predicate => predicate.GetIngredient());
            }
        }
        [System.Serializable]
        private class Predicate {
            [SerializeField] private IngredientItemSO ingredient;
            [SerializeField] private TrayItemSO output;

            public bool Check(IngredientItemSO ingredientItem, out TrayItemSO finalOutput) {
                if (ingredientItem == ingredient) {
                    finalOutput = output;
                    return true;
                }

                finalOutput = null;
                return false;
                
            }

            public IngredientItemSO GetIngredient() => ingredient;
        }    
        
        
    }
}