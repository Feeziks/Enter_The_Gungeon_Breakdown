using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForestPieces
{
	private static GameObject forest_piece_for_test_prefab = Resources.Load("RoomPieces/ForestPieces/forest_piece_prefab") as GameObject;


	private static int[,] forest_piece_for_test_valid_neighbors = new int[8, 1] { { -1}, { -1}, { -1}, { -1}, { -1}, { -1}, { -1}, { -1} };


	public static Piece forest_piece_for_test = new Piece("forest_piece_for_test", forest_piece_for_test_prefab, forest_piece_for_test_valid_neighbors);

	public static List<Piece> all_ForestPieces_pieces = new List<Piece> {forest_piece_for_test};
}
