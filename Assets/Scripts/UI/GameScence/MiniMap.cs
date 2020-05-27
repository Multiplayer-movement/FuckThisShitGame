﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [Header("小房间父节点")]
    public Transform miniRoomNode;

    [Header("迷你图标")]
    public GameObject miniRoomPrefab;
    public GameObject miniIconBoss;
    public GameObject miniIconTreasure;
    public GameObject miniIconShop;
    float width; float height;

    GameObject[,] miniRoomArray;
    GameObject currentRoom;
    Vector2 currentCoordinate;
    List<Vector2> hasBeenToList = new List<Vector2>();

    Level level;

    private void Start()
    {
        width = miniRoomPrefab.GetComponent<RectTransform>().rect.size.x * transform.localScale.x;
        height = miniRoomPrefab.GetComponent<RectTransform>().rect.size.y * transform.localScale.y ;
        level = UIManager.Instance.level;
    }

    public void CreatMiniMap()
    {
        Room[,] roomArray = level.roomArray;
        miniRoomArray = new GameObject[roomArray.GetLength(0), roomArray.GetLength(1)];

        foreach (Room room in roomArray)
        {
            if (room != null)
            {
                var cell = Instantiate(miniRoomPrefab, miniRoomNode);
                miniRoomArray[(int)room.coordinate.x, (int)room.coordinate.y] = cell;

                int x = (int)room.coordinate.y - roomArray.GetLength(0) / 2;
                int y = (int)room.coordinate.x - roomArray.GetLength(0) / 2;
                cell.GetComponent<RectTransform>().localPosition = new Vector2(x * width, y * height);

                switch (room.roomType)
                {
                    case Room.RoomType.Boss:
                        Instantiate(miniIconBoss, cell.transform);
                        break;
                    case Room.RoomType.Treasure:
                        Instantiate(miniIconTreasure, cell.transform);
                        break;
                    case Room.RoomType.Shop:
                        Instantiate(miniIconShop, cell.transform);
                        break;
                    default:
                        break;
                }
            }
        }
        currentCoordinate = new Vector2((int)level.currentRoom.coordinate.x, (int)level.currentRoom.coordinate.x);
        currentRoom = miniRoomArray[(int)currentCoordinate.x, (int)currentCoordinate.x];
    }

    public void UpdateMiniMap(Vector2 MoveDirection)
    {
        hasBeenToList.Add(currentCoordinate);

        currentRoom.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1);
        currentCoordinate.x += MoveDirection.y;
        currentCoordinate.y += MoveDirection.x;
        currentRoom = miniRoomArray[(int)currentCoordinate.x, (int)currentCoordinate.y];
        currentRoom.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);

        List<Vector2> neighboringCoordinate = new List<Vector2>()
        {
            currentCoordinate + Vector2.right,currentCoordinate + Vector2.left,
            currentCoordinate + Vector2.down,currentCoordinate + Vector2.up
        };
        foreach (var coordinate in neighboringCoordinate)
        {
            GameObject cell = miniRoomArray[(int)coordinate.x, (int)coordinate.y];
            if (cell != null && !hasBeenToList.Contains(coordinate))
            {
                cell.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
            }
        }

        miniRoomNode.transform.localPosition -= new Vector3(MoveDirection.x * width, MoveDirection.y * height, 0);
    }
}
