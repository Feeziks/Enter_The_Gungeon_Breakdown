using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestPieces
{
	private static GameObject TestRoomPiece_prefab = Resources.Load("Resources\\RoomPieces\\TestPieces\\TestRoomDoorPrefab") as GameObject;
	private static GameObject TestRoomPiece2_prefab = Resources.Load("Resources\\RoomPieces\\TestPieces\\TestRoomPiece2Prefab") as GameObject;
	private static GameObject TestRoomPieceCorner_prefab = Resources.Load("Resources\\RoomPieces\\TestPieces\\TestRoomPieceCornerPrefab") as GameObject;
	private static GameObject TestRoomPieceDoor_prefab = Resources.Load("Resources\\RoomPieces\\TestPieces\\TestRoomPiecePitPrefab") as GameObject;
	private static GameObject TestRoomPiecePit_prefab = Resources.Load("Resources\\RoomPieces\\TestPieces\\TestRoomPiecePrefab") as GameObject;
	private static GameObject TestRoomPieceWall_prefab = Resources.Load("Resources\\RoomPieces\\TestPieces\\TestRoomPieceWallPrefab") as GameObject;


	private static int[,] TestRoomPiece_valid_neighbors = new int[8, 6] { { -1, 1, -1, -1, 4, -1}, { -1, 1, -1, -1, 4, -1}, { -1, 1, -1, 3, 4, 5}, { -1, 1, -1, 3, 4, 5}, { -1, 1, -1, -1, 4, -1}, { -1, 1, 2, 3, 4, 5}, { -1, 1, 2, 3, 4, 5}, { -1, 1, -1, -1, 4, -1} };
	private static int[,] TestRoomPiece2_valid_neighbors = new int[8, 6] { { 0, -1, -1, -1, 4, -1}, { 0, -1, -1, -1, 4, -1}, { 0, -1, -1, 3, 4, 5}, { 0, -1, -1, 3, 4, 5}, { 0, -1, -1, -1, 4, -1}, { 0, -1, 2, 3, 4, 5}, { 0, -1, 2, 3, 4, 5}, { 0, -1, -1, -1, 4, -1} };
	private static int[,] TestRoomPieceCorner_valid_neighbors = new int[8, 6] { { -1, -1, -1, 3, -1, 5}, { 0, 1, -1, -1, 4, -1}, { 0, 1, -1, 3, 4, 5}, { -1, -1, -1, -1, -1, -1}, { -1, -1, -1, 3, -1, 5}, { -1, -1, -1, -1, -1, -1}, { -1, -1, -1, -1, -1, -1}, { -1, -1, -1, 3, -1, 5} };
	private static int[,] TestRoomPieceDoor_valid_neighbors = new int[8, 6] { { -1, -1, 2, -1, -1, 5}, { 0, 1, -1, -1, 4, -1}, { 0, 1, -1, -1, 4, 5}, { -1, -1, 2, -1, -1, -1}, { -1, -1, 2, -1, -1, 5}, { -1, -1, -1, -1, -1, -1}, { 0, 1, 2, -1, 4, 5}, { 0, 1, -1, -1, 4, -1} };
	private static int[,] TestRoomPiecePit_valid_neighbors = new int[8, 6] { { 0, 1, -1, -1, -1, -1}, { 0, 1, -1, -1, -1, -1}, { 0, 1, -1, 3, -1, 5}, { 0, 1, -1, 3, -1, 5}, { 0, 1, -1, -1, -1, -1}, { 0, 1, 2, 3, -1, 5}, { 0, 1, 2, 3, -1, 5}, { 0, 1, -1, -1, -1, -1} };
	private static int[,] TestRoomPieceWall_valid_neighbors = new int[8, 6] { { -1, -1, 2, 3, -1, -1}, { 0, 1, -1, -1, 4, -1}, { 0, 1, -1, 3, 4, -1}, { -1, -1, 2, -1, -1, -1}, { -1, -1, 2, 3, -1, -1}, { -1, -1, -1, -1, -1, -1}, { 0, 1, 2, 3, 4, -1}, { 0, 1, -1, -1, 4, -1} };


	public static Piece TestRoomPiece = new Piece("TestRoomPiece", TestRoomPiece_prefab, TestRoomPiece_valid_neighbors);
	public static Piece TestRoomPiece2 = new Piece("TestRoomPiece2", TestRoomPiece2_prefab, TestRoomPiece2_valid_neighbors);
	public static Piece TestRoomPieceCorner = new Piece("TestRoomPieceCorner", TestRoomPieceCorner_prefab, TestRoomPieceCorner_valid_neighbors);
	public static Piece TestRoomPieceDoor = new Piece("TestRoomPieceDoor", TestRoomPieceDoor_prefab, TestRoomPieceDoor_valid_neighbors);
	public static Piece TestRoomPiecePit = new Piece("TestRoomPiecePit", TestRoomPiecePit_prefab, TestRoomPiecePit_valid_neighbors);
	public static Piece TestRoomPieceWall = new Piece("TestRoomPieceWall", TestRoomPieceWall_prefab, TestRoomPieceWall_valid_neighbors);
}
