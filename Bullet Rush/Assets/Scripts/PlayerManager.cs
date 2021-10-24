using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public LayerMask layerMask;
    [Header("Player Spawn Point")]
    [SerializeField] Transform playerSpawnPoint;
    public GameObject bulletReference;
    public GameObject currentPlayer;

    [Header("Players")]
    [SerializeField] GameObject playerCrowPrefab;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }

        currentPlayer = Instantiate(playerCrowPrefab, playerSpawnPoint.position, Quaternion.identity);
    }
}
