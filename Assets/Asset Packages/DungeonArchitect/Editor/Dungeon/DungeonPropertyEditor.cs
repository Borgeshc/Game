//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for the dungeon game object
    /// </summary>
	[CustomEditor(typeof(Dungeon))]
	public class DungeonPropertyEditor : Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			if (GUILayout.Button ("Build Dungeon")) {
				BuildDungeon();
			}
			if (GUILayout.Button ("Destroy Dungeon")) {
				DestroyDungeon ();
			}
		}

		void BuildDungeon() {
			// Make sure we have a theme defined
			Dungeon dungeon = target as Dungeon;
			if (dungeon != null) {
				if (HasValidThemes(dungeon)) {
					// Build the dungeon
					dungeon.Build();
				} 
				else {
					Highlighter.Highlight ("Inspector", "Dungeon Themes");

					// Notify the user that atleast one theme needs to be set
					EditorUtility.DisplayDialog("Dungeon Architect", "Please assign atleast one Dungeon Theme before building", "Ok");
				}
			}

		}

		IEnumerator StopHighlighter() {
			yield return new WaitForSeconds(2);
			Highlighter.Stop();
		}

		void DestroyDungeon() {
			Dungeon dungeon = target as Dungeon;
			dungeon.DestroyDungeon();
		}

		bool HasValidThemes(Dungeon dungeon) {
			foreach (var theme in dungeon.dungeonThemes) {
				if (theme != null) {
					return true;
				}
			}
			return false;
		}

	}
}
