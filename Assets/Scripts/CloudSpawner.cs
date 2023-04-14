using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    private Transform player;

    public GameObject cloudObject;
    public Vector2 startOffset;
    public float endPos;

    private void Start()
    {
        player = GameObject.Find("Player").transform;

        SpawnClouds();
    }

    private void SpawnClouds()
    {
        Vector2 spawnPos = new Vector2(player.position.x + startOffset.x, player.position.y + startOffset.y);
        Debug.Log("Spawned");
        Instantiate(cloudObject, spawnPos, Quaternion.identity);
    }
}
