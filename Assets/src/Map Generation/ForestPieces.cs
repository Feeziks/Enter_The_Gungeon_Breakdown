using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ForestPieces
{
	private static string forest_piece_for_test_prefab_path = "RoomPieces/ForestPieces/forest_piece_prefab";


	private static int[,] forest_piece_for_test_valid_neighbors = new int[8, 1] { { -1}, { -1}, { -1}, { -1}, { -1}, { -1}, { -1}, { -1} };


	public static Piece forest_piece_for_test = new Piece("forest_piece_for_test", forest_piece_for_test_prefab_path, forest_piece_for_test_valid_neighbors);

	public static List<Piece> all_ForestPieces_pieces = new List<Piece> {forest_piece_for_test};

	public static bool Load()
	{
		bool success = true;

		for(int i = 0; i < all_ForestPieces_pieces.Count; i++)
		{
			success &= all_ForestPieces_pieces[i].LoadPrefab();
		}
		return success;
	}
}
