using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public LayerMask layerMask;
    [Header("Player Spawn Point")]
    [SerializeField] Transform playerSpawnPoint;

    public Transform currentPlayer;

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

        Instantiate(playerCrowPrefab, playerSpawnPoint.position, Quaternion.identity);
        currentPlayer = playerCrowPrefab.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
