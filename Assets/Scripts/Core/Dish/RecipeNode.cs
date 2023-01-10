using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Dish {
    public class RecipeNode: ScriptableObject {
        [SerializeField] private IngredientItemSO input;
        [SerializeField] private TrayItemSO output;
        [HideInInspector] [SerializeField] private DishItemSO finalDish;
        [FormerlySerializedAs("_children")] [SerializeField] private List<string> children = new ();
        [FormerlySerializedAs("_ancestors")] [SerializeField] private List<string> ancestors = new();
        
        [HideInInspector] [FormerlySerializedAs("_rect")] [SerializeField] private Rect rect = new (0,0, 200,100);

        public IngredientItemSO GetInput() => input;
        public TrayItemSO GetOutput() => output;
        public DishItemSO GetFinalDish() => finalDish;
        public Rect GetRect() => rect;

        public bool IsOutputNode() {
            return children.Count == 0;
        }

        public List<string> GetChildren() => children;
        public List<string> GetAncestors() => ancestors;

#if UNITY_EDITOR
        public void AddChild(string childID) {
            Undo.RecordObject(this, "Add Recipe Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void ClearAncestor() {
            ancestors.Clear();
            EditorUtility.SetDirty(this);
        }

        public void AddAncestor(string ancestorID) {
            if (ancestors.Contains(ancestorID)) return;
            Undo.RecordObject(this, "Add Recipe Link");
            ancestors.Add(ancestorID);
            EditorUtility.SetDirty(this);
        }

        public void AddAncestor(IEnumerable<string> ancestorIDs) {
            foreach (var ancestorID in ancestorIDs) {
                AddAncestor(ancestorID);
            }
        }

        public void RemoveChild(string childID) {
            Undo.RecordObject(this, "Remove Recipe Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveAncestor(string ancestorID) {
            Undo.RecordObject(this, "Remove Recipe Link");
            ancestors.Remove(ancestorID);
            EditorUtility.SetDirty(this);
        }

        public void SetInput(IngredientItemSO newInput) {
            if (input == newInput) return;
            Undo.RecordObject(this, "Update Input Ingredient");
            input = newInput;
            EditorUtility.SetDirty(this);
        }

        public void SetOutput(TrayItemSO newOutput) {
            if (output == newOutput) return;
            Undo.RecordObject(this, "Update Output");
            output = newOutput;

            if (output is DishItemSO dishItem) {
                finalDish = dishItem;
            }
            
            EditorUtility.SetDirty(this);
        }

        public void SetFinalDish(DishItemSO newFinalDish) {
            if (finalDish == newFinalDish) return;
            Undo.RecordObject(this, "Update Final Dish");
            finalDish = newFinalDish;
            EditorUtility.SetDirty(this);
        }

        public void SetPosition(Vector2 newPosition) {
            Undo.RecordObject(this, "Move Recipe Link");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}