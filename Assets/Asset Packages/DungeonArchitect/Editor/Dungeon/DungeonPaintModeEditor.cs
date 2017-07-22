//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEditor;
using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for the paint mode object
    /// </summary>
    public class DungeonPaintModeEditor : Editor
    {
        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += SceneGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= SceneGUI;
        }

        protected virtual void SceneGUI(SceneView sceneview) { }
    }
}
