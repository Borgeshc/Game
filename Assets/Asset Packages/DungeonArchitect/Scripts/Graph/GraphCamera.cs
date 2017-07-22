//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect;

namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// A camera that manages the graph editor's viewport
    /// </summary>
    [Serializable]
    public class GraphCamera
    {
        [SerializeField]
        Vector2 position = Vector2.zero;
        /// <summary>
        /// Position of the camera
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        [SerializeField]
        Vector2 scale = Vector2.one;
        /// <summary>
        /// Zoom scale of the graph camera
        /// </summary>
        public Vector2 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        /// <summary>
        /// Pan the camera along the specified delta value
        /// </summary>
        /// <param name="x">Delta value to move along the X value</param>
        /// <param name="y">Delta value to move along the Y value</param>
        public void Pan(int x, int y)
        {
            Pan(new Vector2(x, y));
        }

        /// <summary>
        /// Pan the camera along the specified delta value
        /// </summary>
        /// <param name="delta">The delta offset to move the camera to</param>
        public void Pan(Vector2 delta)
        {
            position += delta;
        }

        /// <summary>
        /// Handles the user mouse and keyboard input 
        /// </summary>
        /// <param name="e"></param>
        public void HandleInput(Event e)
        {
            int dragButton = 1;
            if (e.type == EventType.MouseDrag && e.button == dragButton)
            {
                Pan(-e.delta);
            }
        }

        /// <summary>
        /// Converts world coordinates (in the graph view) into Screen coordinates (relative to the editor window)
        /// </summary>
        /// <param name="worldCoord">The world cooridnates of the graph view</param>
        /// <returns>The screen cooridnates relative to the editor window</returns>
        public Vector2 WorldToScreen(Vector2 worldCoord)
        {
            return worldCoord - position;
        }

        /// <summary>
        /// Converts the Screen coordinates (of the editor window) into the graph's world coordinate
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <returns>The world coordinates in the graph view</returns>
        public Vector2 ScreenToWorld(Vector2 screenCoord)
        {
            return screenCoord + position;
        }

        /// <summary>
        /// Moves the camera so most of the nodes are visible
        /// </summary>
        /// <param name="graph">The graph to query</param>
        /// <param name="editorBounds">The bounds of the editor window</param>
        public void FocusOnBestFit(Graph graph, Rect editorBounds)
        {
            Vector2 average = Vector2.zero;
            foreach (var node in graph.Nodes)
            {
                if (node == null) continue;
                average += node.Bounds.center;
            }
            average /= graph.Nodes.Count;
            position = average - editorBounds.size / 2.0f;
        }

        /// <summary>
        /// Moves the camera to the marker node
        /// </summary>
        /// <param name="graph">The graph to work on</param>
        /// <param name="editorBounds">The bounds of the editor window</param>
        /// <param name="markerName">The marker name to focus on</param>
        public void FocusOnMarker(Graph graph, Rect editorBounds, string markerName)
        {
            foreach (var node in graph.Nodes)
            {
                if (node is MarkerNode)
                {
                    var markerNode = node as MarkerNode;
                    if (markerNode.Caption == markerName)
                    {
                        var nodePosition = markerNode.Bounds.center;
                        position = nodePosition - editorBounds.size / 2.0f;
                        break;
                    }
                }
            }
        }
    }
}
