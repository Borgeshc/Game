//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;

namespace DungeonArchitect
{
    /// <summary>
    /// The dungeon configuration for the Grid builder
    /// </summary>
    public class GridDungeonConfig : DungeonConfig
    {
        /// <summary>
        /// Changing this number would completely change the layout of the dungeon. 
        /// This is the base random number seed that is used to build the dungeon. 
        /// There is a convenience function to randomize this value (button labeled R)
        /// </summary>
        public int NumCells = 150;

        /// <summary>
        /// This is how small a cell size can be. While generation, a cell is either converted
        /// to a room, corridor or is discarded completely. The Cell width / height is randomly 
        /// chosen within this range
        /// </summary>
        public int MinCellSize = 2;

        /// <summary>
        /// This is how big a cell size can be. While generation, a cell is either 
        /// converted to a room, corridor or is discarded completely.
        /// The Cell width / height is randomly chosen within this range
        /// </summary>
        public int MaxCellSize = 5;

        /// <summary>
        ///  If a cell size exceeds past this limit, it is converted into a room. After cells are
        ///  promoted to rooms, all rooms are connected to each other through corridors
        ///  (either directly or indirectly. See spanning tree later)
        /// </summary>
        public int RoomAreaThreshold = 15;

        /// <summary>
        /// The aspect ratio of the cells (width to height ratio). Keeping this value near 0 would 
        /// create square rooms. Bringing this close to 1 would create elongated / stretched rooms
        /// with a high width to height ratio
        /// </summary>
        public float RoomAspectDelta = 0.4f;

        /// <summary>
        /// The extra width to apply to one side of a corridor
        /// </summary>
        public int CorridorPadding = 1;

        /// <summary>
        /// Flag to apply the padding on both sides of the corridor
        /// </summary>
        public bool CorridorPaddingDoubleSided = false;

        /// <summary>
        /// Tweak this value to increase / reduce the height variations (and stairs)
        /// in your dungeon. A value close to 0 reduces the height variation and increases
        /// as you approach 1. Increasing this value to a higher level might create dungeons 
        /// with no place for proper stair placement since there is too much height variation.
        /// A value of 0.2 to 0.4 seems good
        /// </summary>
        public float HeightVariationProbability = 0.2f;

        /// <summary>
        /// The number of logical floor units the dungeon height can vary. This determines how 
        /// high the dungeon's height can vary (e.g. max 2 floors high). Set this value depending 
        /// on the stair meshes you designer has created. In the sample demo, there are two stair
        /// meshes, one 200 units high (1 floor) and another 400 units high (2 floors).
        /// So the default is set to 2
        /// </summary>
        public int MaxAllowedStairHeight = 2;

        /// <summary>
        /// Determines how many loops you would like to have in your dungeon. A value near 0 will create
        /// fewer loops creating linear dungeons. A value near 1 would create lots of loops, which would look unoriginal.
        /// Its good to allow a few loops so a value close to zero (like 0.2 should be good)
        /// </summary>
        public float SpanningTreeLoopProbability = 0.15f;

        /// <summary>
        /// The generator would add stairs to make different areas of the dungeon accessible. However, we do not want too
        /// many stairs. For e.g., before adding a stair in a particular elevated area, the generator would check if this
        /// area is already accessible from a nearby stair. If so, it would not add it. This tolerance parameter determines
        /// how far to look for an existing path before we can add a stair. Play with this parameter if you see too many
        /// stairs close to each other, or too few
        /// </summary>
        public float StairConnectionTollerance = 3;

        /// <summary>
        /// The random number generator used in the dungeon generator does not use a uniform distribution.
        /// Instead it uses a normal distribution to get higher frequency of lower values and fewer higher values
        /// (and hence fewer room cells and a lot more corridor cells). Play with these parameters for different results
        /// </summary>
        public float NormalMean = 0;

        /// <summary>
        /// The random number generator used in the dungeon generator does not use a uniform distribution.
        /// Instead it uses a normal distribution to get higher frequency of lower values and fewer higher values
        /// (and hence fewer room cells and a lot more corridor cells). Play with these parameters for different results
        /// </summary>
        public float NormalStd = 0.3f;

		public bool Mode2D = false;

        /// <summary>
        /// The radius within which to spawn the initial cells before they are separated.
        /// Keep to a low value like 10-15
        /// </summary>
        public float InitialRoomRadius = 15;

        /// <summary>
        /// __Internal
        /// </summary>
        public int FloorHeight = 0;

    }
}
