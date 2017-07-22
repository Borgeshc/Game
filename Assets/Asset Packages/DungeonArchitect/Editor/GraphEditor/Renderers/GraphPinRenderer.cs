//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Renders a graph pin hosted inside a node
    /// </summary>
    public class GraphPinRenderer
    {

        public static void Draw(GraphRendererContext rendererContext, GraphPin pin, GraphCamera camera)
        {
            var bounds = new Rect(pin.GetBounds());
            var positionWorld = pin.Node.Position + bounds.position;
            var positionScreen = camera.WorldToScreen(positionWorld);
            bounds.position = positionScreen;

            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = GetPinColor(pin);
            GUI.Box(bounds, "");
            GUI.backgroundColor = originalColor;

            // Draw the pin glow

            var glowTexture = rendererContext.Resources.GetResource<Texture2D>(DungeonEditorResources.TEXTURE_PIN_GLOW);
            if (glowTexture != null)
            {
                GUI.DrawTexture(bounds, glowTexture);
            }
        }

        static Color GetPinColor(GraphPin pin)
        {
            Color color;
            if (pin.ClickState == GraphPinMouseState.Clicked)
            {
                color = GraphEditorConstants.PIN_COLOR_CLICK;
            }
            else if (pin.ClickState == GraphPinMouseState.Hover)
            {
                color = GraphEditorConstants.PIN_COLOR_HOVER;
            }
            else
            {
                color = GraphEditorConstants.PIN_COLOR;
            }
            return color;
        }

    }
}
