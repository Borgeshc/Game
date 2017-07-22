//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;

namespace DungeonArchitect
{
    /// <summary>
    /// Base dungeon configuration.  Create your own implementation of this configuration based on your dungeon builder's needs
    /// </summary>
	public class DungeonConfig : MonoBehaviour {
        public uint Seed = 0;
        public Vector3 GridCellSize = new Vector3(1, 1, 1);
	}
}
