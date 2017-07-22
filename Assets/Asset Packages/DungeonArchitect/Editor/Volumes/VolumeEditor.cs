//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections;
using DungeonArchitect;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for volumes game objects
    /// </summary>
    [ExecuteInEditMode]
    public class VolumeEditor : Editor
    {
        IntVector positionOnGrid;
        IntVector sizeOnGrid;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Rebuild Dungeon"))
            {
                RequestRebuild(true);
            }
        }


        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += OnUpdate;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnUpdate;
        }

        public virtual void OnUpdate(SceneView sceneView)
        {
        }

        void RequestRebuild(bool force)
        {
            var volume = target as Volume;
            if (volume != null && volume.dungeon != null)
            {
                if (force)
                {
                    volume.dungeon.Build();
                }
                else
                {
                    volume.dungeon.RequestRebuild();
                }
            }
        }

        protected void OnTransformModified(Volume volume)
        {
            if (volume == null || volume.dungeon == null)
            {
                return;
            }
            IntVector newPositionOnGrid, newSizeOnGrid;
            volume.GetGridTransform(out newPositionOnGrid, out newSizeOnGrid);
            if (!positionOnGrid.Equals(newPositionOnGrid) || !sizeOnGrid.Equals(newSizeOnGrid))
            {
                positionOnGrid = newPositionOnGrid;
                sizeOnGrid = newSizeOnGrid;
                OnGridTransformModified();
            }

        }

        void OnGridTransformModified()
        {
            RequestRebuild(false);
        }
    }
}
