using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Room_Piece")]
public class RoomPiece_SO : ScriptableObject
{
    public Sprite image;

    public PolygonCollider2D collider;
}
