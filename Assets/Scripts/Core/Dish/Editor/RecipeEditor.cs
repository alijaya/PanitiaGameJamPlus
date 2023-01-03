using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Core.Dish.Editor {
    public class RecipeEditor : EditorWindow {
        private DishRecipeSO _selectedRecipe;

        [NonSerialized] private RecipeNode _createdNode;
        [NonSerialized] private RecipeNode _deletedNode;
        [NonSerialized] private RecipeNode _draggingNode;
        [NonSerialized] private RecipeNode _linkingParentNode;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private GUIStyle _nodeStyle;

        [NonSerialized] private bool _draggingCanvas;
        [NonSerialized] private Vector2 _draggingCanvasOffset;

        private Vector2 _scrollPosition;

        private const float CanvasSize = 4000;
        private const float BackgroundWidth = 50;
        private const float BackgroundHeight = 50;

        [MenuItem("Window/Recipe Editor")]
        private static void ShowWindow() {
            GetWindow(typeof(RecipeEditor), false, "Recipe Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line) {
            var obj = EditorUtility.InstanceIDToObject(instanceId) as DishRecipeSO;
            if (obj == null) return false;
            ShowWindow();
            return true;
        }

        private void OnEnable() {
            Selection.selectionChanged += SelectionChanged;
            
            _nodeStyle = new GUIStyle {
                normal = {
                    background = Texture2D.grayTexture,
                    textColor = Color.white
                },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };
        }

        private void OnDisable() {
            Selection.selectionChanged -= SelectionChanged;
        }

        private void SelectionChanged() {
            var newRecipe = Selection.activeObject as DishRecipeSO;
            if (newRecipe) {
                _selectedRecipe = newRecipe;
                Repaint();
            }
        }

        private void OnGUI() {
            if (_selectedRecipe == null) {
                EditorGUILayout.LabelField("No Recipe Selected");
            }
            else {
                ProcessEvents();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                var canvasRect = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
                var backgroundTexture = Resources.Load("background") as Texture2D;
                var textCoords = new Rect(0, 0, CanvasSize / BackgroundWidth, CanvasSize / BackgroundHeight);
                GUI.DrawTextureWithTexCoords(canvasRect, backgroundTexture, textCoords);

                foreach (var node in _selectedRecipe.GetAllNodes()) {
                    DrawConnections(node);
                }
                foreach (var node in _selectedRecipe.GetAllNodes()) {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();
                
                if (_createdNode != null) {
                    _selectedRecipe.CreateNode(_createdNode);
                    _createdNode = null;
                }

                if (_deletedNode != null) {
                    _selectedRecipe.DeleteNode(_deletedNode);
                    _deletedNode = null;
                }
            }
        }

        private void DrawConnections(RecipeNode node) {
            var startPosition = new Vector3(node.GetRect().xMax, node.GetRect().center.y);
            foreach (var childNode in _selectedRecipe.GetAllChildren(node)) {
                var endPosition = new Vector3(childNode.GetRect().xMin, childNode.GetRect().center.y);
                var controlOffset = endPosition - startPosition;
                controlOffset.y = 0;
                controlOffset.x *= 0.8f;
                
                Handles.DrawBezier(startPosition, endPosition, 
                    startPosition + controlOffset,
                    endPosition - controlOffset, 
                    Color.white, null, 3.5f);
            }
        }

        private void ProcessEvents() {
            var e = Event.current;
            switch (e.type) {
                case EventType.MouseDown when !_draggingNode:
                    _draggingNode = GetNodeAtPoint(e.mousePosition);
                    if (_draggingNode) {
                        _draggingOffset = _draggingNode.GetRect().position - e.mousePosition;
                        Selection.activeObject = _draggingNode;
                    }
                    else {
                        _draggingCanvas = true;
                        _draggingCanvasOffset = e.mousePosition + _scrollPosition;
                        Selection.activeObject = _selectedRecipe;
                    }
                    break;
                case EventType.MouseDrag when _draggingNode:
                    _draggingNode.SetPosition(e.mousePosition + _draggingOffset);
                    GUI.changed = true;
                    break;
                case EventType.MouseDrag when _draggingCanvas:
                    _scrollPosition = _draggingCanvasOffset - e.mousePosition;
                    GUI.changed = true;
                    break;
                case EventType.MouseUp when _draggingNode:
                    _draggingNode = null;
                    break;
                case EventType.MouseUp when _draggingCanvas:
                    _draggingCanvas = false;
                    break;
            }

            RecipeNode GetNodeAtPoint(Vector2 point) {
                RecipeNode foundNode = null;
                foreach (var node in _selectedRecipe.GetAllNodes()) {
                    if (node.GetRect().Contains(_scrollPosition + point)) {
                        foundNode = node;    
                    }
                }

                return foundNode;
            }
        }

        private void DrawNode(RecipeNode node) {
            GUILayout.BeginArea(node.GetRect(), _nodeStyle);
            EditorGUI.BeginChangeCheck();

            node.SetInput(EditorGUILayout.ObjectField(node.GetInput(), typeof(IngredientItemSO), false) as IngredientItemSO);
            if (node.IsOutputNode()) {
                node.SetOutput(EditorGUILayout.ObjectField(node.GetOutput(), typeof(TrayItemSO), false) as TrayItemSO);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x")) {
                _deletedNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("+")) {
                _createdNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(RecipeNode node) {
            if (!_linkingParentNode) {
                if (GUILayout.Button("link")) {
                    _linkingParentNode = node;
                }    
            } else if (_linkingParentNode == node) {
                if (GUILayout.Button("cancel")) {
                    _linkingParentNode = null;
                }
            } else if (_linkingParentNode.GetChildren().Contains(node.name)) {
                if (GUILayout.Button("unlink")) {
                    node.RemoveAncestor(_linkingParentNode.name);
                    _linkingParentNode.RemoveChild(node.name);
                    _linkingParentNode = null;
                }
            }
            else {
                if (node.GetChildren().Contains(_linkingParentNode.name)) return;
                if (_linkingParentNode.GetAncestors().Contains(node.name)) return;
                if (GUILayout.Button("child")) {
                    foreach (var ancestor in _linkingParentNode.GetAncestors().Where(ancestor => !node.GetAncestors().Contains(ancestor))) {
                        node.AddAncestor(ancestor);
                    }
                    node.AddAncestor(_linkingParentNode.name);
                    _linkingParentNode.AddChild(node.name);
                    _linkingParentNode = null;
                }
            }
        }
    }
}