using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapPieces
{
	public static class ForestPieces
	{
		static public List<Piece> allForestPiecesPieces;
		//---------------------------------------------------------------------------
		//---------- Piece Declerations ---------------------------------------------
		//---------------------------------------------------------------------------
		public static Piece forest_piece_for_test;
		static ForestPieces()
		{
			forest_piece_for_test = new Piece("forest_piece_for_test", "../Resources/RoomPieces//ForestPieces/forest_piece_for_test_Prefab.prefab", new Dictionary<string, List<Piece>>(){ 
				{"N", new List<Piece>(){}},
				{"NE", new List<Piece>(){}},
				{"E", new List<Piece>(){}},
				{"SE", new List<Piece>(){}},
				{"S", new List<Piece>(){}},
				{"SW", new List<Piece>(){}},
				{"W", new List<Piece>(){}},
				{"NW", new List<Piece>(){}}};
		}
	}

}