//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;

namespace DungeonArchitect
{
    /// <summary>
    /// The main dungeon behavior that manages the creation and destruction of dungeons
    /// </summary>
	[ExecuteInEditMode]
	public class Dungeon : MonoBehaviour {
		DungeonConfig config;
        PooledDungeonSceneProvider sceneProvider;
        DungeonBuilder dungeonBuilder;
		DungeonModel dungeonModel;

        /// <summary>
        /// Active model used by the dungeon
        /// </summary>
		public DungeonModel ActiveModel {
			get {
				if (dungeonModel == null) {
					dungeonModel = GetComponent<DungeonModel> ();
				}
				return dungeonModel;
			}
		}

        /// <summary>
        /// Flag to check if the layout has been built.  
        /// This is used to quickly reapply the theme after the theme graph has been modified,
        /// without rebuilding the layout, if it has already been built
        /// </summary>
		public bool IsLayoutBuilt {
			get {
                if (dungeonBuilder == null)
                {
                    return false;
                }
                return dungeonBuilder.IsLayoutBuilt;
			}
		}

		DungeonToolData toolOverlayData = new DungeonToolData();

		public bool debugDraw = false;

        /// <summary>
        /// List of themes assigned to this dungeon
        /// </summary>
		public List<Graph> dungeonThemes;

        /// <summary>
        /// Flag to rebuild the dungeon. Set this to true if you want to rebuild it in the next update
        /// </summary>
		bool requestedRebuild = false;

		public DungeonConfig Config {
			get {
				if (config == null) {
					config = GetComponent<DungeonConfig> ();
				}
				return config;
			}
		}

        void Awake() {
            Initialize();
		}

		void Start () {
		}


		void Initialize() {
			if (config == null) {
				config = GetComponent<DungeonConfig> ();
			}
			
			if (sceneProvider == null) {
				sceneProvider = GetComponent<PooledDungeonSceneProvider> ();
			}
			
			if (dungeonBuilder == null) {
				dungeonBuilder = GetComponent<DungeonBuilder> ();
			}

			if (dungeonModel == null) {
				dungeonModel = GetComponent<DungeonModel> ();
			}
		}

        List<DungeonPropDataAsset> GetThemeAssets()
        {
            var themes = new List<DungeonPropDataAsset>();
            foreach (var themeGraph in dungeonThemes)
            {
                DungeonPropDataAsset theme = new DungeonPropDataAsset();
                theme.BuildFromGraph(themeGraph);
                themes.Add(theme);
            }
            return themes;
        }

        /// <summary>
        /// Builds the complete dungeon (layout and visual phase)
        /// </summary>
		public void Build() {
			Initialize();
			dungeonModel.ResetModel();
			dungeonModel.ToolData = toolOverlayData;

			dungeonBuilder.BuildDungeon(config, dungeonModel);

			NotifyPostLayoutBuild();

			ReapplyTheme();
		}

        /// <summary>
        /// Runs the theming engine over the existing layout to rebuild the game objects from the theme file.  
        /// The layout is not built in this stage
        /// </summary>
		public void ReapplyTheme() {
            // Emit markers defined by this builder
			dungeonBuilder.EmitMarkers();
			
            // Emit markers defined by the users (by attaching implementation of DungeonMarkerEmitter behaviors)
            dungeonBuilder.EmitCustomMarkers();

			var themes = GetThemeAssets();
			
			sceneProvider.OnDungeonBuildStart ();

			dungeonBuilder.ApplyTheme(themes, sceneProvider);

			sceneProvider.OnDungeonBuildStop ();

			NotifyPostBuild();
		}

		DungeonEventListener[] GetListeners() {
			var listeners = GetComponents<DungeonEventListener>();

			var enabledListeners = from listener in listeners
					where listener.enabled
					select listener;

			return enabledListeners.ToArray();
		}

		void NotifyPostLayoutBuild() {
			// Notify all listeners of the post build event
			foreach (var listener in GetListeners()) {
				listener.OnPostDungeonLayoutBuild(this, ActiveModel);
			}
		}

		void NotifyPostBuild() {
			// Notify all listeners of the post build event
			foreach (var listener in GetListeners()) {
				listener.OnPostDungeonBuild(this, ActiveModel);
			}
		}

		void NotifyDungeonDestroyed() {
			// Notify all listeners that the dungeon is destroyed
			foreach (var listener in GetListeners()) {
				listener.OnDungeonDestroyed(this);
			}
		}

		/*
		/// <summary>
		/// Builds the custom navigation managed by the dungeon architect plugin
		/// This requires a DungeonNavigation prefab present in the level
		/// </summary>
		public void BuildNavigation() {
			var navGameObject = GameObject.FindGameObjectWithTag(DungeonConstants.TAG_DUNGEON_NAVIGATION);
			if (navGameObject == null) {
				Debug.LogWarning("Cannot build navigation. No dungeon navigation object found in the scene. Drop in the DungeonNavigation prefab into the scene");
				return;
			}

			//var navMesh = navGameObject.GetComponent<DungeonNavMesh>();
			//navMesh.Build();
		}
		*/

        /// <summary>
        /// Destroys the dungeon
        /// </summary>
		public void DestroyDungeon() {
            var itemList = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
            var dungeonItems = new List<GameObject>();
            foreach (var item in itemList)
            {
                if (item == null) continue;
                if (item.dungeon == this)
                {
                    dungeonItems.Add(item.gameObject);
                }
            }
			foreach(var item in dungeonItems) {
				if (Application.isPlaying) {
					Destroy(item);
				} else {
					DestroyImmediate(item);
				}
			}
            
			if (dungeonModel != null) {
				dungeonModel.ResetModel();
			}

			if (dungeonBuilder != null) {
				dungeonBuilder.OnDestroyed();
			}
			NotifyDungeonDestroyed();
		}

		/// <summary>
		/// Requests the dungeon to be rebuilt in the next update phase
		/// </summary>
		public void RequestRebuild() {
			requestedRebuild = true;
		}


		void Update() {
			if (dungeonModel == null) return;
			if (debugDraw) {
				DrawModel();
			}
			
			if (requestedRebuild) {
				requestedRebuild = false;
				Build();
            }
		}

		void OnGUI() {
            /*
			var model = dungeonModel as GridDungeonModel;
			if (debugDraw && model != null) {
				
				foreach (var cell in model.Cells) {
					DebugDrawer.DrawCellId(cell, model.Config.GridCellSize);
				}
			}
            */
		}

		void DrawModel() {
			var model = dungeonModel as GridDungeonModel;
			foreach (var cell in model.Cells) {
                DebugDrawer.DrawCell(cell, Color.white, config.GridCellSize);
				DebugDrawer.DrawAdjacentCells(cell, model, Color.green);
            }

			foreach (var door in model.DoorManager.Doors) {
                var start = door.AdjacentTiles[0];
                var end = door.AdjacentTiles[1];
                var boundsStart = new Rectangle(start.x, start.z, 1, 1);
                var boundsEnd = new Rectangle(end.x, end.z, 1, 1);
                IntVector location = boundsStart.Location;
                location.y = start.y;
                boundsStart.Location = location;

                location = boundsEnd.Location;
                location.y = end.y;
                boundsEnd.Location = location;

                DebugDrawer.DrawBounds(boundsStart, Color.yellow, config.GridCellSize);
                DebugDrawer.DrawBounds(boundsEnd, Color.yellow, config.GridCellSize);
            }
		}

        /// <summary>
        /// Registers a painted cell
        /// </summary>
        /// <param name="location">the location of the painted cell, in grid cooridnates</param>
        /// <param name="automaticRebuild">if true, the dungeon would be rebuilt, if the data model has changed due to this request</param>
		public void AddPaintCell(IntVector location, bool automaticRebuild) {
			bool overlappingCell = false;
			IntVector overlappingCellValue = new IntVector();
			foreach (var cellData in toolOverlayData.PaintedCells) {
				if (cellData.x == location.x && cellData.z == location.z) {
					if (cellData.y != location.y) {
						overlappingCell = true;
						overlappingCellValue = cellData;
						break;
					}
					else {
						// Cell with this data already exists.  Ignore the request
						return;
					}
				}
			}
			if (overlappingCell) {
				toolOverlayData.PaintedCells.Remove(overlappingCellValue);
			}

			toolOverlayData.PaintedCells.Add(location);
			if (automaticRebuild) {
				Build();
			}
		}

        /// <summary>
        /// Remove a previous painted cell
        /// </summary>
        /// <param name="location">the location of the painted cell to remove, in grid cooridnates</param>
        /// <param name="automaticRebuild">if true, the dungeon would be rebuilt, if the data model has changed due to this request</param>
		public void RemovePaintCell(IntVector location, bool automaticRebuild) {
			if (toolOverlayData.PaintedCells.Contains(location)) {
				toolOverlayData.PaintedCells.Remove(location);
				if (automaticRebuild) {
					Build ();
				}
			}
		}

        /// <summary>
        /// Clears all overlay data
        /// </summary>
        /// <param name="automaticRebuild"></param>
		public void ClearToolOverlayData(bool automaticRebuild) {
			toolOverlayData.PaintedCells.Clear();
			if (automaticRebuild) {
				Build ();
			}
		}
	}
	
    /// <summary>
    /// Helper functions to draw debug information of the dungeon layout in the scene view 
    /// </summary>
	class DebugDrawer {
		public static void DrawCell(Cell cell, Color color, Vector3 gridScale) {
			DrawBounds(cell.Bounds, color, gridScale);
		}
		public static void DrawBounds(Rectangle bounds, Color color, Vector3 gridScale) {
			var x0 = bounds.Left * gridScale.x;
			var x1 = bounds.Right * gridScale.x;
			var z0 = bounds.Front * gridScale.z;
			var z1 = bounds.Back * gridScale.z;
			var y = bounds.Location.y * gridScale.y;
			Debug.DrawLine(new Vector3(x0, y, z0), new Vector3(x1, y, z0), color);
			Debug.DrawLine(new Vector3(x1, y, z0), new Vector3(x1, y, z1), color);
			Debug.DrawLine(new Vector3(x1, y, z1), new Vector3(x0, y, z1), color);
			Debug.DrawLine(new Vector3(x0, y, z1), new Vector3(x0, y, z0), color);
		}

		public static void DrawCellId(Cell cell, Vector3 gridScale) {
			var center = Vector3.Scale(cell.Bounds.CenterF(), gridScale); // + new Vector3(0, .2f, 0);
			var screenCoord = Camera.main.WorldToScreenPoint(center);
			if (screenCoord.z > 0) {
				GUI.Label(new Rect(screenCoord.x, Screen.height - screenCoord.y, 100, 50), "" + cell.Id);
			}
		}

		public static void DrawMarker(PropSocket marker, Color color) {
			var start = Matrix.GetTranslation(ref marker.Transform);
			var end = start + new Vector3(0, 0.2f, 0);
			Debug.DrawLine(start, end, color);

		}

		public static void DrawAdjacentCells(Cell cell, GridDungeonModel model, Color color) {
			foreach (var adjacentId in cell.AdjacentCells) {
				var adjacentCell = model.GetCell(adjacentId);
				if (adjacentCell == null) return;
				var centerA = Vector3.Scale(cell.Bounds.CenterF(), model.Config.GridCellSize);
				var centerB = Vector3.Scale(adjacentCell.Bounds.CenterF(), model.Config.GridCellSize);
				Debug.DrawLine(centerA, centerB, color, 0, false);
			}

			foreach (var adjacentId in cell.FixedRoomConnections) {
				var adjacentCell = model.GetCell(adjacentId);
				if (adjacentCell == null) return;
				var centerA = Vector3.Scale(cell.Bounds.CenterF(), model.Config.GridCellSize) + new Vector3(0, 0.2f, 0);
				var centerB = Vector3.Scale(adjacentCell.Bounds.CenterF(), model.Config.GridCellSize) + new Vector3(0, 0.2f, 0);
				Debug.DrawLine(centerA, centerB, Color.red, 0, false);
			}

		}
	}
}
