//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Renders the graph node in the graph editor
    /// </summary>
    public class GraphNodeRenderer
    {

        protected virtual Color getBackgroundColor(GraphNode node)
        {
            return node.Selected ? GraphEditorConstants.NODE_COLOR_SELECTED : GraphEditorConstants.NODE_COLOR;
        }

        public virtual void Draw(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            // Draw the pins
            foreach (var pin in node.InputPins)
            {
                GraphPinRenderer.Draw(rendererContext, pin, camera);
            }
            foreach (var pin in node.OutputPins)
            {
                GraphPinRenderer.Draw(rendererContext, pin, camera);
            }
        }

        protected void DrawBackgroundBox(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var screenPosition = camera.WorldToScreen(node.Bounds.position);
            var screenBounds = new Rect(screenPosition.x, screenPosition.y, node.Bounds.width, node.Bounds.height);

            GUI.backgroundColor = getBackgroundColor(node);
            GUI.Box(screenBounds, "");
        }

        protected void DrawNodeTexture(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera, string textureName)
        {
            var shadowTexture = rendererContext.Resources.GetResource<Texture2D>(textureName);
            if (shadowTexture != null)
            {
                var center = camera.WorldToScreen(node.Bounds.center);
                var position = center;
                position.x -= shadowTexture.width / 2.0f;
                position.y -= shadowTexture.height / 2.0f;
                var size = new Vector2(shadowTexture.width, shadowTexture.height);
                var rect = new Rect(position.x, position.y, size.x, size.y);
                GUI.DrawTexture(rect, shadowTexture);
            }
        }
    }

    public class GraphNodeRendererFactory
    {
        GraphNodeRenderer defaultRenderer = new GraphNodeRenderer();
        Dictionary<Type, GraphNodeRenderer> renderers = new Dictionary<Type, GraphNodeRenderer>();


        public void RegisterNodeRenderer(Type nodeType, GraphNodeRenderer renderer)
        {
            if (!renderers.ContainsKey(nodeType))
            {
                renderers.Add(nodeType, renderer);
            }
        }

        public GraphNodeRenderer GetRenderer(Type nodeType)
        {
            if (renderers.ContainsKey(nodeType))
            {
                return renderers[nodeType];
            }
            return defaultRenderer;
        }
    }
}
