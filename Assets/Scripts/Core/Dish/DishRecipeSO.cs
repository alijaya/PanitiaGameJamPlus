using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Dish {
    [CreateAssetMenu(fileName = "New Dish Recipe", menuName = "Dish/Recipe", order = 0)]
    public class DishRecipeSO : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] private List<RecipeNode> nodes = new();
        //private readonly Dictionary<RecipeNode, IEnumerable<RecipeNode>> _childrenLookup = new();
        private readonly Dictionary<string, RecipeNode> _nodeLookup = new();

        public IEnumerable<IngredientItemSO> GetBaseIngredients() {
            return GetAllNodes().Where(x => x.GetAncestors().Count == 0).Select(x => x.GetInput());
        }

        #region Nodes

        private void OnValidate() {
            _nodeLookup.Clear();
            foreach (var node in GetAllNodes()) {
                _nodeLookup[node.name] = node;
                //_childrenLookup[node] = nodes.Where(x => node.GetChildren().Contains(x.name));
            }
        }

        public IEnumerable<RecipeNode> GetAllNodes() {
            return nodes;
        }
        public IEnumerable<RecipeNode> GetAllChildren(RecipeNode parentNode) {
            return from childID in parentNode.GetChildren() where _nodeLookup.ContainsKey(childID) select _nodeLookup[childID];
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