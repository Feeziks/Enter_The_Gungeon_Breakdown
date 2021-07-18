using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TestPieces
{
	private static string TestRoomPiece_prefab_path = "RoomPieces/TestPieces/TestRoomPiece2";
	private static string TestRoomPiece2_prefab_path = "RoomPieces/TestPieces/TestRoomPiece2Walls";
	private static string TestRoomPiece2walls_prefab_path = "RoomPieces/TestPieces/TestRoomPiece2Walls_f";
	private static string TestRoomPiece2walls_f_prefab_path = "RoomPieces/TestPieces/TestRoomPiece3Walls";
	private static string TestRoomPiece2walls_f_F_prefab_path = "RoomPieces/TestPieces/TestRoomPiece3Walls_f";
	private static string TestRoomPiece2_F_prefab_path = "RoomPieces/TestPieces/TestRoomPieceCorner";
	private static string TestRoomPiece3Walls_prefab_path = "RoomPieces/TestPieces/TestRoomPieceCorner_f";
	private static string TestRoomPiece3Walls_f_prefab_path = "RoomPieces/TestPieces/TestRoomPieceCorner_ff";
	private static string TestRoomPiece3Walls_f_F_prefab_path = "RoomPieces/TestPieces/TestRoomPieceCorner_fff";
	private static string TestRoomPieceCorner_prefab_path = "RoomPieces/TestPieces/TestRoomPieceDoor";
	private static string TestRoomPieceCorner_f_prefab_path = "RoomPieces/TestPieces/TestRoomPiecePit";
	private static string TestRoomPieceCorner_ff_prefab_path = "RoomPieces/TestPieces/TestRoomPieceWall";
	private static string TestRoomPieceCorner_fff_prefab_path = "RoomPieces/TestPieces/TestRoomPieceWall_f";
	private static string TestRoomPieceCorner_fff_F_prefab_path = "RoomPieces/TestPieces/TestRoomPieceWall_ff";
	private static string TestRoomPieceCorner_ff_F_prefab_path = "RoomPieces/TestPieces/TestRoomPieceWall_fff";
	private static string TestRoomPieceCorner_f_F_prefab_path = "RoomPieces/TestPieces/Test_Inactive_Piece";


	private static int[,] TestRoomPiece_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, 9, -1, -1, -1, -1, -1, 15}, { 0, 1, -1, -1, -1, 5, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 12, -1, 14, -1} };
	private static int[,] TestRoomPiece2_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, 9, -1, -1, -1, -1, -1, 15}, { 0, 1, -1, -1, -1, 5, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 12, -1, 14, -1} };
	private static int[,] TestRoomPiece2walls_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPiece2walls_f_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPiece2walls_f_F_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPiece2_F_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, 9, -1, -1, -1, -1, -1, 15}, { 0, 1, -1, -1, -1, 5, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 12, -1, 14, -1} };
	private static int[,] TestRoomPiece3Walls_valid_neighbors = new int[8, 16] { { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPiece3Walls_f_valid_neighbors = new int[8, 16] { { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPiece3Walls_f_F_valid_neighbors = new int[8, 16] { { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPieceCorner_valid_neighbors = new int[8, 16] { { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPieceCorner_f_valid_neighbors = new int[8, 16] { { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 12, -1, 14, -1} };
	private static int[,] TestRoomPieceCorner_ff_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, 9, -1, -1, -1, -1, -1, 15}, { 0, 1, -1, -1, -1, 5, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPieceCorner_fff_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPieceCorner_fff_F_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, 9, -1, -1, -1, -1, -1, 15}, { 0, 1, -1, -1, -1, 5, 6, -1, 8, 9, -1, -1, 12, -1, 14, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPieceCorner_ff_F_valid_neighbors = new int[8, 16] { { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, -1, 12, -1, 14, 15}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1}, { 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };
	private static int[,] TestRoomPieceCorner_f_F_valid_neighbors = new int[8, 16] { { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { 0, 1, -1, -1, -1, 5, -1, -1, -1, -1, -1, 11, -1, 13, -1, -1}, { 0, 1, -1, -1, -1, 5, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, -1, 11, 12, 13, 14, 15}, { -1, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10, -1, -1, -1, -1, 15}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, -1, 10, 11, 12, 13, 14, -1}, { -1, -1, 2, 3, 4, -1, -1, 7, -1, -1, 10, 11, -1, 13, -1, -1}, { -1, -1, 2, 3, 4, -1, 6, 7, 8, 9, 10, 11, -1, 13, -1, 15} };


	public static Piece TestRoomPiece = new Piece("TestRoomPiece", TestRoomPiece_prefab_path, TestRoomPiece_valid_neighbors);
	public static Piece TestRoomPiece2 = new Piece("TestRoomPiece2", TestRoomPiece2_prefab_path, TestRoomPiece2_valid_neighbors);
	public static Piece TestRoomPiece2walls = new Piece("TestRoomPiece2walls", TestRoomPiece2walls_prefab_path, TestRoomPiece2walls_valid_neighbors);
	public static Piece TestRoomPiece2walls_f = new Piece("TestRoomPiece2walls_f", TestRoomPiece2walls_f_prefab_path, TestRoomPiece2walls_f_valid_neighbors);
	public static Piece TestRoomPiece2walls_f_F = new Piece("TestRoomPiece2walls_f_F", TestRoomPiece2walls_f_F_prefab_path, TestRoomPiece2walls_f_F_valid_neighbors);
	public static Piece TestRoomPiece2_F = new Piece("TestRoomPiece2_F", TestRoomPiece2_F_prefab_path, TestRoomPiece2_F_valid_neighbors);
	public static Piece TestRoomPiece3Walls = new Piece("TestRoomPiece3Walls", TestRoomPiece3Walls_prefab_path, TestRoomPiece3Walls_valid_neighbors);
	public static Piece TestRoomPiece3Walls_f = new Piece("TestRoomPiece3Walls_f", TestRoomPiece3Walls_f_prefab_path, TestRoomPiece3Walls_f_valid_neighbors);
	public static Piece TestRoomPiece3Walls_f_F = new Piece("TestRoomPiece3Walls_f_F", TestRoomPiece3Walls_f_F_prefab_path, TestRoomPiece3Walls_f_F_valid_neighbors);
	public static Piece TestRoomPieceCorner = new Piece("TestRoomPieceCorner", TestRoomPieceCorner_prefab_path, TestRoomPieceCorner_valid_neighbors);
	public static Piece TestRoomPieceCorner_f = new Piece("TestRoomPieceCorner_f", TestRoomPieceCorner_f_prefab_path, TestRoomPieceCorner_f_valid_neighbors);
	public static Piece TestRoomPieceCorner_ff = new Piece("TestRoomPieceCorner_ff", TestRoomPieceCorner_ff_prefab_path, TestRoomPieceCorner_ff_valid_neighbors);
	public static Piece TestRoomPieceCorner_fff = new Piece("TestRoomPieceCorner_fff", TestRoomPieceCorner_fff_prefab_path, TestRoomPieceCorner_fff_valid_neighbors);
	public static Piece TestRoomPieceCorner_fff_F = new Piece("TestRoomPieceCorner_fff_F", TestRoomPieceCorner_fff_F_prefab_path, TestRoomPieceCorner_fff_F_valid_neighbors);
	public static Piece TestRoomPieceCorner_ff_F = new Piece("TestRoomPieceCorner_ff_F", TestRoomPieceCorner_ff_F_prefab_path, TestRoomPieceCorner_ff_F_valid_neighbors);
	public static Piece TestRoomPieceCorner_f_F = new Piece("TestRoomPieceCorner_f_F", TestRoomPieceCorner_f_F_prefab_path, TestRoomPieceCorner_f_F_valid_neighbors);

	public static List<Piece> all_TestPieces_pieces = new List<Piece> {TestRoomPiece, TestRoomPiece2, TestRoomPiece2walls, TestRoomPiece2walls_f, TestRoomPiece2walls_f_F, TestRoomPiece2_F, TestRoomPiece3Walls, TestRoomPiece3Walls_f, TestRoomPiece3Walls_f_F, TestRoomPieceCorner, TestRoomPieceCorner_f, TestRoomPieceCorner_ff, TestRoomPieceCorner_fff, TestRoomPieceCorner_fff_F, TestRoomPieceCorner_ff_F, TestRoomPieceCorner_f_F};

	public static bool Load()
	{
		bool success = true;

		for(int i = 0; i < all_TestPieces_pieces.Count; i++)
		{
			success &= all_TestPieces_pieces[i].LoadPrefab();
		}
		return success;
	}
}
