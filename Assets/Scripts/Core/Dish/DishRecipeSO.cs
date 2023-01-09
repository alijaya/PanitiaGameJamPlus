using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Dish {
    [CreateAssetMenu(fileName = "New Dish Recipe", menuName = "Dish/Recipe", order = 0)]
    public class DishRecipeSO : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] private List<RecipeNode> nodes = new();
        private readonly Dictionary<string, RecipeNode> _nodeLookup = new();
        private readonly Dictionary<DishItemSO, HashSet<IngredientItemSO>> _finalDishIngredientsLookup = new();

        public IEnumerable<IngredientItemSO> GetBaseIngredients() {
            return GetAllNodes().Where(x => x.GetAncestors().Count == 0).Select(x => x.GetInput());
        }

        public IEnumerable<IngredientItemSO> GetAllIngredients() {
            return GetAllNodes().Select(node => node.GetInput());
        }

        private IEnumerable<IngredientItemSO> GetIngredients(RecipeNode finalNode) {
            return finalNode.GetAncestors().Where(ancestor => _nodeLookup.ContainsKey(ancestor)).Select(ancestor => _nodeLookup[ancestor].GetInput());
        }

        public IEnumerable<IngredientItemSO> GetPossibleIngredients(IEnumerable<DishItemSO> possibleDishes) {
            var possibleIngredients = new HashSet<IngredientItemSO>();
            foreach (var possibleDish in possibleDishes) {
                if (!_finalDishIngredientsLookup.ContainsKey(possibleDish)) continue;
                possibleIngredients.UnionWith(_finalDishIngredientsLookup[possibleDish]);
            }

            return possibleIngredients;
        }
        
        #region Nodes

        private void OnValidate() {
            _nodeLookup.Clear();
            _finalDishIngredientsLookup.Clear();
            foreach (var node in GetAllNodes()) {
                _nodeLookup[node.name] = node;

                if (node.GetFinalDish()) {
                    var ingredients = GetIngredients(node).ToHashSet();
                    ingredients.Add(node.GetInput());
                    _finalDishIngredientsLookup[node.GetFinalDish()] = ingredients;
                }
            }
            
        }

        public IEnumerable<RecipeNode> GetAllNodes() {
            return nodes;
        }
        public IEnumerable<RecipeNode> GetAllChildren(RecipeNode parentNode) {
            return from childID in parentNode.GetChildren() where _nodeLookup.ContainsKey(childID) select _nodeLookup[childID];
        }

        public IEnumerable<RecipeNode> GetAllChildren(string parentID) {
            return _nodeLookup.ContainsKey(parentID) ? GetAllChildren(_nodeLookup[parentID]) : null;
        }

        public IEnumerable<RecipeNode> GetAllAncestors(string childID) {
            var ancestors = new HashSet<RecipeNode>();
            foreach (var node in GetAllNodes()) {
                if (node.GetChildren().Contains(childID)) {
                    foreach (var nodeAncestor in node.GetAncestors()) {
                        ancestors.Add(_nodeLookup[nodeAncestor]);
                    }
                }
            }

            return ancestors;
        }

        public RecipeNode GetBaseNode(IngredientItemSO baseIngredient) {
            return GetAllNodes().FirstOrDefault(x => x.GetAncestors().Count == 0 && x.GetInput() == baseIngredient);
        }
        
        
        
#if UNITY_EDITOR
        public void CreateNode(RecipeNode parentNode) {
            var newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Recipe Node");
            Undo.RecordObject(this, "Added Recipe Node");
            AddNode(newNode);
        }

        public void DeleteNode(RecipeNode nodeToDelete) {
            Undo.RecordObject(this, "Deleted Recipe Node");
            CleanAncestor(nodeToDelete);
            nodes.Remove(nodeToDelete);
            
            OnValidate();
            CleanChildren(nodeToDelete);
            
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private RecipeNode MakeNode(RecipeNode parent) {
            var newNode = CreateInstance<RecipeNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent) {
                parent.AddChild(newNode.name);
                newNode.GetAncestors().AddRange(parent.GetAncestors());
                newNode.GetAncestors().Add(parent.name);
                
                var offset = new Vector2(newNode.GetRect().width * 1.5f, 0);
                newNode.SetPosition(parent.GetRect().position + offset);
            }

            return newNode;
        }

        private void AddNode(RecipeNode newNode) {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanChildren(RecipeNode deletedNode) {
            foreach (var node in GetAllNodes()) {
                node.RemoveChild(deletedNode.name);
                node.ClearAncestor();
            }

            // rebuild Ancestor
            foreach (var node in GetAllNodes()) {
                if (node == deletedNode) continue;
                foreach (var child in GetAllChildren(node)) {
                    if (child == deletedNode) continue;
                    child.AddAncestor(node.name);
                }
            }
        }

        private void CleanAncestor(RecipeNode deletedNode) {
            var ancestors = new List<string>();
            ancestors.AddRange(deletedNode.GetAncestors());

            foreach (var node in GetAllNodes()) {
                var remainingAncestors = GetAllAncestors(deletedNode.name).Select(x => x.name).ToList();
                foreach (var ancestorID in ancestors) {
                    if (remainingAncestors.Contains(ancestorID)) continue;
                    node.RemoveAncestor(ancestorID);
                }
                
                node.RemoveAncestor(deletedNode.name);
            }
        }
        
#endif
        
        
        public void OnBeforeSerialize() {
            #if UNITY_EDITOR
            if (nodes.Count == 0) {
                var newNode = MakeNode(null);
                AddNode(newNode);
            }
            
            if (AssetDatabase.GetAssetPath(this) != "") {
                foreach (var node in GetAllNodes()) {
                    if (AssetDatabase.GetAssetPath(node) == "") {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
            #endif
        }

        public void OnAfterDeserialize() {
            
        }

        #endregion
    }
}