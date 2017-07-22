//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editors for MarkerNode
    /// </summary>
    [CustomEditor(typeof(MarkerNode))]
    public class MarkerNodeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty caption;

        public void OnEnable()
        {
            sobject = new SerializedObject(target);
            caption = sobject.FindProperty("caption");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            GUILayout.Label("Marker Node", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(caption, new GUIContent("Name"));
            sobject.ApplyModifiedProperties();
        }
    }


    /// <summary>
    /// Renders a marker node
    /// </summary>
    public class MarkerNodeRenderer : GraphNodeRenderer
    {
        public override void Draw(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            // Draw the background base texture
            DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_MARKER_NODE_BG);

            var style = GUI.skin.GetStyle("Label");
            style.alignment = TextAnchor.MiddleCenter;

            var positionScreen = camera.WorldToScreen(node.Position);
            var pinHeight = node.OutputPins[0].BoundsOffset.height;
            var labelBounds = new Rect(positionScreen.x, positionScreen.y, node.Bounds.width, node.Bounds.height - pinHeight / 2);
            style.normal.textColor = node.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;
            GUI.Label(labelBounds, node.Caption, style);

            // Draw the foreground frame textures
            DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_MARKER_NODE_FRAME);

            if (node.Selected)
            {
                DrawNodeTexture(rendererContext, node, camera, DungeonEditorResources.TEXTURE_MARKER_NODE_SELECTION);
            }

            // Draw the pins
            base.Draw(rendererContext, node, camera);

        }
    }
}
