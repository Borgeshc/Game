//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;

namespace DungeonArchitect
{
    /// <summary>
    /// A scene provider instantiates game objects into the scene.  
    /// Implementations can customize the instantiation process if needed (e.g. object pooling etc)
    /// </summary>
	public class DungeonSceneProvider : MonoBehaviour {
        /// <summary>
        /// Called when build is started
        /// </summary>
        public virtual void OnDungeonBuildStart()
        {
            Initialize();
        }

        /// <summary>
        /// Called after build has ended
        /// </summary>
		public virtual void OnDungeonBuildStop() {}

        /// <summary>
        /// Request the creation of a game object
        /// </summary>
        /// <param name="gameObjectProp">The template to use for instantiation</param>
        /// <param name="transform">The transform of the instantiated game object</param>
		public virtual void AddGameObject(GameObjectPropTypeData gameObjectProp, Matrix4x4 transform) {}

        /// <summary>
        /// Request the creation of a sprite object
        /// </summary>
        /// <param name="spriteProp">The sprite game object template reference</param>
        /// <param name="transform">The transform of the prop</param>
		public virtual void AddSprite(SpritePropTypeData spriteProp, Matrix4x4 transform) {}
		//public virtual void AddLight(GameObject Template, Matrix4x4 transform, string NodeId) {}

        /// <summary>
        /// Dungeon config used by the builder
        /// </summary>
		protected DungeonConfig config;

        /// <summary>
        /// The owning dungeon actor reference
        /// </summary>
        protected Dungeon dungeon;

        /// <summary>
        /// The parent for all spawned game objects.  Assign this to create all spawned objects
        /// underneath it to avoid cluttering up the hierarchy
        /// </summary>
		public GameObject itemParent;

		void Awake() {
            Initialize();
		}

        protected void Initialize()
        {
            config = GetComponent<DungeonConfig>();
            dungeon = GetComponent<Dungeon>();
        }

		protected GameObject BuildGameObject(GameObjectPropTypeData gameObjectProp, Matrix4x4 transform) {
			Matrix.DecomposeMatrix(ref transform, out _position, out _rotation, out _scale);
			
			var MeshTemplate = gameObjectProp.Template;
			string NodeId = gameObjectProp.NodeId;
			var gameObj = Instantiate(MeshTemplate, _position, _rotation) as GameObject;
			gameObj.transform.localScale = _scale;
			gameObj.layer = 0;
			
			if (itemParent != null) {
				string parentName = gameObjectProp.AttachToSocket;
				var subTypeParent = itemParent.transform.Find(parentName);
				if(subTypeParent == null)
				{
					subTypeParent = new GameObject(parentName).transform;
					subTypeParent.SetParent(itemParent.transform);
				}
				gameObj.transform.parent = subTypeParent;
			}
			
			var data = gameObj.AddComponent<DungeonSceneProviderData> ();
			data.NodeId = NodeId;
			data.dungeon = dungeon;
			data.affectsNavigation = gameObjectProp.affectsNavigation;
			
			return gameObj;
		}

		protected void FlipSpriteTransform(ref Matrix4x4 transform, Sprite sprite) {
			Matrix.DecomposeMatrix(ref transform, out _position, out _rotation, out _scale);

			FlipSpritePosition(ref _position);

			var basePixelScale = 1.0f / sprite.pixelsPerUnit;	// TODO: Verify this
			var spriteScale = new Vector3(sprite.rect.width * basePixelScale, sprite.rect.height * basePixelScale, 1);
			_scale = Vector3.Scale(_scale, spriteScale);

			// flip the rotation
			var angles = _rotation.eulerAngles;
			var t = angles.z;
			angles.z = -angles.y;
			angles.y = t;
			_rotation = Quaternion.Euler(angles);

			transform.SetTRS(_position, _rotation, _scale);
		}
		
		protected void FlipSpritePosition(ref Matrix4x4 transform) {
			var position = Matrix.GetTranslation(ref transform);
			
			FlipSpritePosition(ref _position);
			
			Matrix.SetTranslation(ref transform, position);
		}

		protected void FlipSpritePosition(ref Vector3 position) {
			var z = position.z;
			position.z = position.y;
			position.y = z;
		}
		
		protected GameObject BuildSpriteObject(SpritePropTypeData spriteData, Matrix4x4 transform, string NodeId) {
			if (spriteData.sprite == null) return null;
			var gameObj = new GameObject(spriteData.sprite.name);
			
			// Setup the sprite
			var spriteRenderer = gameObj.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = spriteData.sprite;
			spriteRenderer.color = spriteData.color;
			spriteRenderer.sortingOrder = spriteData.orderInLayer;
			
			if (spriteData.materialOverride != null) {
				spriteRenderer.material = spriteData.materialOverride;
			}
			if (spriteData.sortingLayerName != null && spriteData.sortingLayerName.Length > 0) {
				spriteRenderer.sortingLayerName = spriteData.sortingLayerName;
			}

			// Setup the sprite collision
			var collisionType = spriteData.collisionType;
			if (collisionType != DungeonSpriteCollisionType.None) {
				Vector2 baseScaleMultiplier = Vector2.one;
				var sprite = spriteData.sprite;
				var pixelsPerUnit = sprite.pixelsPerUnit;
				baseScaleMultiplier.x = sprite.rect.width / pixelsPerUnit;
				baseScaleMultiplier.y = sprite.rect.height / pixelsPerUnit;

				Collider2D collider = null;
				if (collisionType == DungeonSpriteCollisionType.Box) {
					var boxCollider = gameObj.AddComponent<BoxCollider2D>();
					boxCollider.size = Vector3.Scale(spriteData.physicsSize, baseScaleMultiplier);
					collider = boxCollider;
				}
				else if (collisionType == DungeonSpriteCollisionType.Circle) {
					var circleCollider = gameObj.AddComponent<CircleCollider2D>();
					circleCollider.radius = spriteData.physicsRadius * baseScaleMultiplier.x;
					collider = circleCollider;
				}
				else if (collisionType == DungeonSpriteCollisionType.Polygon) {
					collider = gameObj.AddComponent<PolygonCollider2D>();
				}

				if (collider != null) {
					collider.sharedMaterial = spriteData.physicsMaterial;
					collider.offset = Vector3.Scale(spriteData.physicsOffset, baseScaleMultiplier);
				}
			}


			// Set the transform
			Matrix.DecomposeMatrix(ref transform, out _position, out _rotation, out _scale);
			gameObj.transform.position = _position;
			gameObj.transform.rotation = _rotation;
			gameObj.transform.localScale = _scale;
			
			// Setup dungeon related parameters
			if (itemParent != null) {
				gameObj.transform.parent = itemParent.transform;
			}
			
			var data = gameObj.AddComponent<DungeonSceneProviderData> ();
			data.NodeId = NodeId;
			data.dungeon = dungeon;
			
			return gameObj;
		}
		
		protected Vector3 _position = new Vector3();
		protected Quaternion _rotation = new Quaternion();
		protected Vector3 _scale = new Vector3();
		protected void SetTransform(Transform transform, Matrix4x4 matrix) {
			Matrix.DecomposeMatrix(ref matrix, out _position, out _rotation, out _scale);
			transform.position = _position;
			transform.rotation = _rotation;
			transform.localScale = _scale;
		}
	}
}