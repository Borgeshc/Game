//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Graphs;


namespace DungeonArchitect.Editors
{

	/// <summary>
	/// The main editor window for the Theme graph editor.  This hosts the graph editor for managing the theme graph
	/// </summary>
	public class DungeonArchitectGraphEditor : EditorWindow
	{
		[SerializeField]
		GraphEditor graphEditor;

		public GraphEditor GraphEditor {
			get { return graphEditor; }
		}

		Timer timer = new Timer();

		[MenuItem( "Window/Dungeon Architect" )]
		static void ShowEditor()
		{
			EditorWindow.GetWindow<DungeonArchitectGraphEditor>();
		}

		public void Init(Graph graph)
		{
			if( graphEditor != null ) {
				graphEditor.Init( graph, position );
				Repaint();
			}
		}

		void OnEnable()
		{
			if( graphEditor == null ) {
				graphEditor = CreateInstance<GraphEditor>();
			}
			this.wantsMouseMove = true;
			timer.Hertz = 4;
			timer.Tick += OnTimerTick;
		}

		void OnDisable()
		{
			timer.Tick -= OnTimerTick;
		}

		void OnTimerTick(float elapsedTime)
		{
			Repaint();
		}

		void OnDestroy()
		{
			graphEditor.OnDestroy();
			graphEditor = null;

		}

		void Update()
		{
			var frameTime = 1 / 100.0f;
			timer.Update( frameTime );
			if( graphEditor != null ) {
				graphEditor.Update();
			}
		}

		string[] GetMarkerNames()
		{
			var markerNames = new List<string>();
			if( graphEditor != null && graphEditor.Graph != null ) {
				var graph = graphEditor.Graph;
				foreach( var node in graph.Nodes ) {
					if( node is MarkerNode ) {
						var markerNode = node as MarkerNode;
						markerNames.Add( markerNode.Caption );
					}
				}
			}
			var markerArray = markerNames.ToArray();
			System.Array.Sort( markerArray );
			return markerArray;
		}

		void DrawToolbar()
		{
			var graphValid = ( graphEditor != null && graphEditor.Graph != null );

			EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
			if( graphValid ) {
				if( GUILayout.Button( "Navigate To", EditorStyles.toolbarDropDown ) ) {
					GenericMenu markerMenu = new GenericMenu();
					var markerNames = GetMarkerNames();
					if( markerNames.Length > 0 ) {
						foreach( var markerName in markerNames ) {
							markerMenu.AddItem( new GUIContent( markerName ), false, OnJumpTo_MarkerName, markerName );
						}

						markerMenu.AddSeparator( "" );
					}
					markerMenu.AddItem( new GUIContent( "Center On Graph" ), false, OnJumpTo_CenterGraph );

					// Offset menu from right of editor window
					markerMenu.DropDown( new Rect( 0, 0, 0, 20 ) );
					EditorGUIUtility.ExitGUI();
				}

				if( GUILayout.Button( "Tools", EditorStyles.toolbarDropDown ) ) {
					GenericMenu toolsMenu = new GenericMenu();
					toolsMenu.AddItem( new GUIContent( "Refresh Thumbnails" ), false, OnRefreshThumbnail );
					
					toolsMenu.AddSeparator( "" );
					toolsMenu.AddItem( new GUIContent( "Advanced/Recreate Node Ids" ), false, Advanced_OnCreateNodeIds );

					// Offset menu from right of editor window
					toolsMenu.DropDown( new Rect( 80, 0, 0, 20 ) );
					EditorGUIUtility.ExitGUI();
				}
                
				GUILayout.FlexibleSpace();
				graphEditor.RealtimeUpdate = GUILayout.Toggle( graphEditor.RealtimeUpdate, "Realtime Update", EditorStyles.toolbarButton );
			}

			EditorGUILayout.EndHorizontal();
		}

		void Advanced_OnCreateNodeIds()
		{
			var confirm = EditorUtility.DisplayDialog( "Recreate Node Ids?",
				              "Are you sure you want to recreate node Ids?  You should do this after cloning a theme file", "Yes", "Cancel" );
			if( confirm ) {
				DungeonEditorHelper._Advanced_RecreateGraphNodeIds();
			}
		}

		void OnRefreshThumbnail()
		{
			AssetThumbnailCache.Instance.Reset();
		}

		void OnJumpTo_MarkerName(object userdata)
		{
			var markerName = userdata as string;
			if( markerName != null && graphEditor != null ) {
				graphEditor.FocusCameraOnMarker( markerName, position );
			}
		}

		void OnJumpTo_CenterGraph()
		{
			if( graphEditor != null ) {
				graphEditor.FocusCameraOnBestFit( position );
			}
		}

		public void  OnGUI()
		{
			var originalColor = GUI.backgroundColor;

			graphEditor.Draw( position );

			Event e = Event.current;
			switch( e.type ) {
				case EventType.DragUpdated:
				case EventType.DragPerform:
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					if( e.type == EventType.DragPerform ) {
						DragAndDrop.AcceptDrag();

						foreach( var draggedObject in DragAndDrop.objectReferences ) {
							if( draggedObject is GameObject ) {
								var node = graphEditor.CreateNode<GameObjectNode>( e.mousePosition );
								node.Template = draggedObject as GameObject;

								var originalTransform = node.Template.transform;
								node.offset = Matrix4x4.TRS( Vector3.zero, originalTransform.rotation, originalTransform.localScale );

								graphEditor.SelectNode( node );
							} else if( draggedObject is Sprite ) {
								var node = graphEditor.CreateNode<SpriteNode>( e.mousePosition );
								node.sprite = draggedObject as Sprite;
								graphEditor.SelectNode( node );
							}
						}
					}
				break;
			}

			GUI.backgroundColor = originalColor;
			DrawToolbar();

			HandleInput( Event.current );
		}

		void HandleInput(Event e)
		{
			graphEditor.HandleInput( e );

			switch( e.type ) {
				case EventType.MouseMove:
				case EventType.MouseDrag:
				case EventType.MouseDown:
				case EventType.MouseUp:
				case EventType.KeyDown:
					Repaint();
				break;
			}
		}
	}
}
