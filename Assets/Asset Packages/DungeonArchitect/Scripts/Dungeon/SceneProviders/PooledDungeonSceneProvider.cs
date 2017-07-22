//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    /// <summary>
    /// Implementation of the Scene provider that adds object pooling over the existing functionality.
    /// This is useful for quick rebuilding and better performance, as object in the scene are reused
    /// while rebuilding, instead of destroying everything and rebuilding
    /// </summary>
	public class PooledDungeonSceneProvider : DungeonSceneProvider {
		// Pools list of game objects by their node ids
		Dictionary<string, Queue<GameObject>> pooledObjects = new Dictionary<string, Queue<GameObject>>();

		public override void OnDungeonBuildStart() {
            base.OnDungeonBuildStart();
			pooledObjects.Clear ();
			var items = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
			foreach (var item in items) {
                if (item == null) continue;
                if (item.dungeon != this.dungeon) continue;

				if (!pooledObjects.ContainsKey(item.NodeId)) {
					pooledObjects.Add(item.NodeId, new Queue<GameObject>());
				}
				pooledObjects[item.NodeId].Enqueue(item.gameObject);
			}
		}

		public override void OnDungeonBuildStop() {
			// Destroy all unused objects from the pool
			foreach (var objects in pooledObjects.Values) {
				foreach (var obj in objects) {
					if (Application.isPlaying) {
						Destroy(obj);
					} else {
						DestroyImmediate(obj);
					}
				}
			}

			pooledObjects.Clear ();
		}

		public override void AddSprite(SpritePropTypeData spriteProp, Matrix4x4 transform) {
			if (spriteProp == null) return;
			string NodeId = spriteProp.NodeId;
			
			if (spriteProp.sprite == null) {
				return;
			}

			FlipSpriteTransform(ref transform, spriteProp.sprite);

			GameObject item = null;
			// Try to reuse an object from the pool
			if (pooledObjects.ContainsKey (NodeId) && pooledObjects [NodeId].Count > 0) {
				item = pooledObjects [NodeId].Dequeue ();
				SetTransform (item.transform, transform);
			} else {
				// Pool is exhausted for this object
				item = BuildSpriteObject(spriteProp, transform, NodeId);
			}
			item.isStatic = spriteProp.IsStaticObject;
		}

		public override void AddGameObject(GameObjectPropTypeData gameObjectProp, Matrix4x4 transform)
        {
			if (gameObjectProp == null) return;
			var MeshTemplate = gameObjectProp.Template;
			string NodeId = gameObjectProp.NodeId;

			if (MeshTemplate == null) {
				return;
			}
			
			// If we are in 2D mode, then flip the YZ axis
			{
				var mode2D = false;
				if (config is GridDungeonConfig) {
					var gridConfig = config as GridDungeonConfig;
					mode2D = gridConfig.Mode2D;
				}
				if (mode2D) {
					var position = Matrix.GetTranslation(ref transform);
					FlipSpritePosition(ref position);
					Matrix.SetTranslation(ref transform, position);
				}
			}

			GameObject item = null;
			// Try to reuse an object from the pool
			if (pooledObjects.ContainsKey (NodeId) && pooledObjects [NodeId].Count > 0) {
				item = pooledObjects [NodeId].Dequeue ();
				SetTransform (item.transform, transform);
			} else {
				// Pool is exhausted for this object
				item = BuildGameObject(gameObjectProp, transform);


            }
			item.isStatic = gameObjectProp.IsStaticObject;
		}


	}
}
