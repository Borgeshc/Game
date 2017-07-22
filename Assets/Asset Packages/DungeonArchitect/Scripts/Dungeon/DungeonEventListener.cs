//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using DungeonArchitect;
using System.Collections;

namespace DungeonArchitect
{

	/// <summary>
	/// Listen to various dungeon events during the build and destroy phase
	/// </summary>
	public class DungeonEventListener : MonoBehaviour {
		/// <summary>
        /// Called after the layout is built in memory, but before the markers are emitted
		/// </summary>
		/// <param name="model">The dungeon model</param>
		public virtual void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model) {}

		/// <summary>
        /// Called after the dungeon is completely built
        /// </summary>
        /// <param name="model">The dungeon model</param>
		public virtual void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model) {}

		/// <summary>
        /// Called after the dungeon is destroyed
        /// </summary>
        /// <param name="model">The dungeon model</param>
		public virtual void OnDungeonDestroyed(Dungeon dungeon) {}
	}
}