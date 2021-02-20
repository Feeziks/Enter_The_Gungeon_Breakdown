using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGeneration
{
	public static class Globals
	{
		public static readonly int NUM_FLOOR_TYPES = 3;
		public static readonly int[] NUM_PIECES_PER_FLOOR = {2, 58, 16};
	}

	//Direction enum
	public enum Direction : int
	{
		north = 0,
		northeast = 1,
		east = 2,
		southeast = 3,
		south = 4,
		southwest = 5,
		west = 6,
		northwest = 7,
		DIRECTION_LENGTH = 8
	}


	//Reverse Direction enum
	public enum Reverse_Direction : int
	{
		south = 0,
		southwest = 1,
		west = 2,
		northwest = 3,
		north = 4,
		northeast = 5,
		east = 6,
		southeast = 7,
		REVERSE_DIRECTION_LENGTH = 8
	}

}