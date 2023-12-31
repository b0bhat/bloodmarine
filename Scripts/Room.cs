﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ooparts.dungen;

namespace ooparts.dungen
{
	public class Room : MonoBehaviour
	{
		public Corridor CorridorPrefab;
		public IntVector2 Size;
		public IntVector2 Coordinates;
		public int Num;

		private GameObject _tilesObject;
		private GameObject _wallsObject;
		private GameObject _monstersObject;
		private GameObject _boxesObject;
		public Tile TilePrefab;
		private Tile[,] _tiles;
		public GameObject WallPrefab;
		public GameObject BoxPrefab;
		public RoomSetting Setting;

		public Dictionary<Room, Corridor> RoomCorridor = new Dictionary<Room, Corridor>();

		private Map _map;
		public GameObject RoomColliderPrefab;
		public GameObject RoomCollider;

		public GameObject PlayerPrefab;

		public GameObject MonsterPrefab;
		private GameObject[] Monsters;
		private GameObject[] Boxes;

		public bool startRoom = false;

		public void Init(Map map)
		{
			_map = map;
		}

		public void RoomColliderLoad() {
			RoomCollider = Instantiate(RoomColliderPrefab);
			RoomCollider.GetComponent<RoomCollider>().SetRoom(Size.x, Size.z, this.transform.position);
		}

		public IEnumerator Generate()
		{
			// Create parent object
			_tilesObject = new GameObject("Tiles");
			_tilesObject.transform.parent = transform;
			_tilesObject.transform.localPosition = Vector3.zero;

			_tiles = new Tile[Size.x, Size.z];
			for (int x = 0; x < Size.x; x++)
			{
				for (int z = 0; z < Size.z; z++)
				{
					_tiles[x, z] = CreateTile(new IntVector2((Coordinates.x + x), Coordinates.z + z));
				}
			}
			yield return null;
		}

		private Tile CreateTile(IntVector2 coordinates)
		{
			if (_map.GetTileType(coordinates) == TileType.Empty)
			{
				_map.SetTileType(coordinates, TileType.Room);
			}
			else
			{
				Debug.LogError("Tile Conflict!");
			}
			Tile newTile = Instantiate(TilePrefab);
			newTile.Coordinates = coordinates;
			newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
			newTile.transform.parent = _tilesObject.transform;
			newTile.transform.localPosition = RoomMapManager.TileSize * new Vector3(coordinates.x - Coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Coordinates.z - Size.z * 0.5f + 0.5f);
			newTile.transform.GetChild(0).GetComponent<Renderer>().material = Setting.floor;
			return newTile;
		}

		public Corridor CreateCorridor(Room otherRoom)
		{
			// Don't create if already connected
			if (RoomCorridor.ContainsKey(otherRoom))
			{
				return RoomCorridor[otherRoom];
			}
			//Debug.DrawLine(transform.position, otherRoom.transform.position, Color.red, 50f);

			Corridor newCorridor = Instantiate(CorridorPrefab);
			newCorridor.name = "Corridor (" + otherRoom.Num + ", " + Num + ")";
			newCorridor.transform.parent = transform.parent;
			newCorridor.Coordinates = new IntVector2(Coordinates.x + Size.x / 2, otherRoom.Coordinates.z + otherRoom.Size.z / 2);
			newCorridor.transform.localPosition = new Vector3(newCorridor.Coordinates.x - _map.MapSize.x / 2, 0, newCorridor.Coordinates.z - _map.MapSize.z / 2);
			newCorridor.Rooms[0] = otherRoom;
			newCorridor.Rooms[1] = this;
			newCorridor.Length = Mathf.Abs(otherRoom.transform.localPosition.x - transform.localPosition.x) + Mathf.Abs(otherRoom.transform.localPosition.z - transform.localPosition.z);
			newCorridor.Init(_map);
			otherRoom.RoomCorridor.Add(this, newCorridor);
			RoomCorridor.Add(otherRoom, newCorridor);

			return newCorridor;
		}

		public IEnumerator CreateWalls()
		{
			_wallsObject = new GameObject("Walls");
			_wallsObject.transform.parent = transform;
			_wallsObject.transform.localPosition = Vector3.zero;

			IntVector2 leftBottom = new IntVector2(Coordinates.x - 1, Coordinates.z - 1);
			IntVector2 rightTop = new IntVector2(Coordinates.x + Size.x, Coordinates.z + Size.z);
			for (int x = leftBottom.x; x <= rightTop.x; x++)
			{
				for (int z = leftBottom.z; z <= rightTop.z; z++)
				{
					// If it's center or corner or not wall
					if ((x != leftBottom.x && x != rightTop.x && z != leftBottom.z && z != rightTop.z) ||
						((x == leftBottom.x || x == rightTop.x) && (z == leftBottom.z || z == rightTop.z)) ||
						(_map.GetTileType(new IntVector2(x, z)) != TileType.Wall))
					{
						continue;
					}
					Quaternion rotation = Quaternion.identity;
					if (x == leftBottom.x)
					{
						rotation = MapDirection.West.ToRotation();
					}
					else if (x == rightTop.x)
					{
						rotation = MapDirection.East.ToRotation();
					}
					else if (z == leftBottom.z)
					{
						rotation = MapDirection.South.ToRotation();
					}
					else if (z == rightTop.z)
					{
						rotation = MapDirection.North.ToRotation();
					}
					else
					{
						Debug.LogError("Wall is not on appropriate location!!");
					}

					GameObject newWall = Instantiate(WallPrefab);
					newWall.name = "Wall (" + x + ", " + z + ")";
					newWall.transform.parent = _wallsObject.transform;
					newWall.transform.localPosition = RoomMapManager.TileSize * new Vector3(x - Coordinates.x - Size.x * 0.5f + 0.5f, 0f, z - Coordinates.z - Size.z * 0.5f + 0.5f);
					newWall.transform.localRotation = rotation;
					newWall.transform.localScale *= RoomMapManager.TileSize;
					newWall.transform.GetChild(0).GetComponent<Renderer>().material = Setting.wall;
				}
			}
			yield return null;
		}

		public IEnumerator CreateMonsters(int MonsterCount)
		{
			if (startRoom) {
				yield return null;
			} else {
				//_monstersObject = new GameObject("Monsters");
				//_monstersObject.transform.parent = transform;
				//_monstersObject.transform.localPosition = Vector3.zero;

				//Monsters = new GameObject[MonsterCount];

				for (int i = 0; i < MonsterCount; i++)
				{
					//newMonster.transform.localPosition = new Vector3(i / 2f, 0.5f, i % 2f);
					EnemySpawner.instance.Spawn(new Vector2(Size.x, Size.z),transform.position);
					//newMonster.transform.localPosition = new Vector3(Random.Range(-Size.x+2,Size.x-2),0f,Random.Range(-Size.z+2,Size.z-2));
					//Monsters[i] = newMonster;
				}
				yield return null;
			}
		}

		public IEnumerator CreatePlayer()
		{
			GameObject player = Instantiate((PlayerPrefab));
			player.name = "Player";
			//player.transform.parent = transform.parent;
			player.transform.localPosition = transform.localPosition;
			startRoom = true;
			yield return null;
		}

		public IEnumerator CreateBoxes(int count)
		{
			_boxesObject = new GameObject("Boxes");
			_boxesObject.transform.parent = transform;
			_boxesObject.transform.localPosition = Vector3.zero;

			Boxes = new GameObject[count];

			for (int i = 0; i < count; i++)
			{
				GameObject newBox = Instantiate(BoxPrefab);
				newBox.name = "Box " + (i + 1);
				newBox.transform.parent = _boxesObject.transform;
				float scale = Random.Range(1f,2f);
				newBox.transform.localScale = new Vector3(scale,scale,scale);
				newBox.transform.localPosition = new Vector3(Random.Range(-Size.x+1,Size.x-1),1+scale/2,Random.Range(-Size.z+1,Size.z-1));
				Boxes[i] = newBox;
			}
			yield return null;
		}
	}
}
