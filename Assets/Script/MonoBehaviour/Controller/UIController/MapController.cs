using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public bool isChamber;
    public Tilemap obstacleMap;
    public Tilemap stoneMap;
    public Sprite mapImage;

    private void OnEnable()
    {
        GameManager.InChamber(isChamber);
        PathFinder.UpdateMap(obstacleMap);
        AudioManager.stoneMap = stoneMap;
        GameManager.instance.confiner.m_BoundingShape2D = GetComponent<PolygonCollider2D>();
        GameManager.instance.currentMapImage.sprite = mapImage;
    }
}
