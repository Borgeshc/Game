//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for visual nodes
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VisualNode))]
    public class VisualNodeEditor : PlaceableNodeEditor
	{
		SerializedProperty IsStatic;
		SerializedProperty affectsNavigation;
        InstanceCache instanceCache = new InstanceCache();

        public override void OnEnable()
        {
            base.OnEnable();
            drawOffset = true;
			drawAttachments = true;
			IsStatic = sobject.FindProperty("IsStatic");
			affectsNavigation = sobject.FindProperty("affectsNavigation");
        }

        protected override void DrawPreInspectorGUI()
		{
			EditorGUILayout.PropertyField(IsStatic);

			// affectsNavigation flag is only valid if the object is static.  So disable it if not static
			GUI.enabled = IsStatic.boolValue;
			EditorGUILayout.PropertyField(affectsNavigation);
			GUI.enabled = true;

            GUILayout.Space(CATEGORY_SPACING);
        }
        protected override void DrawPostInspectorGUI()
        {
            GUILayout.Label("Rules", EditorStyles.boldLabel);

            var meshNode = target as VisualNode;
            DrawRule<SelectorRule>(" Selection Rule", ref meshNode.selectionRuleEnabled, ref meshNode.selectionRuleClassName);
            DrawRule<TransformationRule>(" Transform Rule", ref meshNode.transformRuleEnabled, ref meshNode.transformRuleClassName);

            GUI.enabled = true;


            GUILayout.Space(CATEGORY_SPACING);
        }

        void DrawRule<T>(string caption, ref bool ruleEnabled, ref string ruleClassName) where T : ScriptableObject
        {
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(CATEGORY_SPACING);
            ruleEnabled = EditorGUILayout.ToggleLeft(caption, ruleEnabled);
            GUI.enabled = ruleEnabled;
            MonoScript script = null;
            if (ruleClassName != null)
            {
                var rule = instanceCache.GetInstance(ruleClassName) as ScriptableObject;
                if (rule != null)
                {
                    script = MonoScript.FromScriptableObject(rule);
                }
            }
            var oldScript = script;
            script = EditorGUILayout.ObjectField(script, typeof(MonoScript), false) as MonoScript;
            if (oldScript != script && script != null)
            {
                ruleClassName = script.GetClass().FullName;
            }
            else if (script == null)
            {
                ruleClassName = null;
            }

            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }

    /// <summary>
    /// Renders a visual node
    /// </summary>
    public abstract class VisualNodeRenderer : GraphNodeRenderer
    {
        public override void Draw(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_GO_NODE_BG);

            var thumbObject = GetThumbObject(node);
            var visualNode = node as VisualNode;
            if (thumbObject != null)
            {
                Texture texture = AssetThumbnailCache.Instance.GetThumb(thumbObject);
                if (texture != null)
                {
                    var positionWorld = new Vector2(12, 12) + visualNode.Position;
                    var positionScreen = camera.WorldToScreen(positionWorld);
                    GUI.DrawTexture(new Rect(positionScreen.x, positionScreen.y, 96, 96), texture);
                }
            }
            else
            {
                var style = GUI.skin.GetStyle("Label");
                style.alignment = TextAnchor.MiddleCenter;

                var positionScreen = camera.WorldToScreen(visualNode.Position);
                var labelBounds = new Rect(positionScreen.x, positionScreen.y, visualNode.Bounds.width, visualNode.Bounds.height);
                style.normal.textColor = visualNode.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;
                GUI.Label(labelBounds, "None", style);
            }

            DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_GO_NODE_FRAME);

            base.Draw(rendererContext, node, camera);

            if (node.Selected)
            {
                DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_GO_NODE_SELECTION);
            }
        }

        protected abstract Object GetThumbObject(GraphNode node);
    }
}
