using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ApiClient api;
    public string gameId;

    [Header("Prefabs & Spawning")]
    [SerializeField] private List<GameObject> playerPrefabs; 
    [SerializeField] private List<Transform> spawnPoints;   

    [Header("Multiplayer Settings")]
    public int localPlayerId = 0;
    [Tooltip("Si está activo, el id local se elige mirando si el slot 0 ya existe en el servidor (evita que dos clientes usen el mismo pingüino).")]
    [SerializeField] private bool autoAssignPlayerId = false;
    [SerializeField] private float pollRate = 0.1f;

    private List<PlayerController> players = new List<PlayerController>();
    private int remotePlayerId => localPlayerId == 0 ? 1 : 0;

    private bool playersSpawned = false;
    private bool bothPlayersConnected = false;
    private Vector3 lastRemotePosition;
    private bool remotePositionInitialized = false;


    private Vector3 lastSentPosition;
    private bool isPostRequestPending = false;
    private bool isGetRequestPending = false;

    public void Start()
    {
        api.OnDataReceived += OnDataReceived;


        autoAssignPlayerId = false;

        PlayerTransfer transfer = FindObjectOfType<PlayerTransfer>();
        if (transfer != null)
        {
            localPlayerId = transfer.SelectedPlayerId;
        }
        else
        {
            localPlayerId = PlayerSession.SelectedPlayerId;
        }

        StartGameAsPlayer(localPlayerId);
    }

    private IEnumerator BootstrapLocalPlayerId()
    {
        string safeGameId = string.IsNullOrEmpty(gameId) ? "partida1" : gameId;

        yield return new WaitForSeconds(Random.Range(0f, 0.08f));

        bool slot0Occupied = false;
        yield return StartCoroutine(api.IsPlayerSlotOccupied(safeGameId, "0", occupied => slot0Occupied = occupied));

        int assignedId = slot0Occupied ? 1 : 0;
        PlayerSession.SelectedPlayerId = assignedId;
        Debug.Log($"Multiplayer: slot 0 en servidor ocupado={slot0Occupied} → jugador local asignado {assignedId}");

        StartGameAsPlayer(assignedId);
    }

    public void StartGameAsPlayer(int id)
    {
        localPlayerId = id;
        SpawnPlayers();
        playersSpawned = true;
        StartCoroutine(ServerPollRoutine());
    }

    private void SpawnPlayers()
    {
        players.Clear();

        for (int i = 0; i < 2; i++)
        {
            if (i < playerPrefabs.Count && i < spawnPoints.Count)
            {

                Vector3 spawnRandomPos = spawnPoints[i].position + new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
                GameObject playerObj = Instantiate(playerPrefabs[i], spawnRandomPos, spawnPoints[i].rotation);
                PlayerController pc = playerObj.GetComponent<PlayerController>();
                
                if (pc != null)
                {
                    pc.isLocalPlayer = (i == localPlayerId);
                    players.Add(pc);
                }
            }
        }
    }

    private IEnumerator ServerPollRoutine()
    {

        if (localPlayerId >= 0 && localPlayerId < players.Count && players[localPlayerId] != null)
        {
            lastSentPosition = players[localPlayerId].GetPosition();
            SendPlayerPosition(localPlayerId);
        }

        while (true)
        {

            if (!isPostRequestPending && localPlayerId >= 0 && localPlayerId < players.Count && players[localPlayerId] != null)
            {
                Vector3 currentPos = players[localPlayerId].GetPosition();
                if (Vector3.Distance(currentPos, lastSentPosition) > 0.01f)
                {
                    lastSentPosition = currentPos;
                    SendPlayerPosition(localPlayerId);
                }
            }


            if (!isGetRequestPending && remotePlayerId >= 0 && remotePlayerId < players.Count && players[remotePlayerId] != null)
            {
                GetPlayerData(remotePlayerId);
            }

            yield return new WaitForSeconds(pollRate); 
        }
    }

    public void GetPlayerData(int playerId)
    {
        isGetRequestPending = true;
        string safeGameId = string.IsNullOrEmpty(gameId) ? "partida1" : gameId;
        StartCoroutine(api.GetPlayerData(safeGameId, playerId.ToString(), () => {
            isGetRequestPending = false;
        }));
    }

    public void OnDataReceived(int playerId, ServerData data)
    {
        Vector3 position = new Vector3(data.posX, data.posY, data.posZ);

   
        if (playerId == remotePlayerId)
        {
            if (!remotePositionInitialized)
            {
                lastRemotePosition = position;
                remotePositionInitialized = true;
            }
            else if (!bothPlayersConnected)
            {
                
                if (Vector3.Distance(position, lastRemotePosition) > 0.01f)
                {
                    bothPlayersConnected = true;
                    Debug.Log("¡El otro jugador se conectó! Comienza el movimiento.");
                    foreach (var p in players)
                    {
                        if (p != null) p.canMove = true;
                    }
                }
            }
        }

        if (playerId >= 0 && playerId < players.Count && players[playerId] != null)
        {
            if (!players[playerId].isLocalPlayer)
            {
                players[playerId].MovePlayer(position);
            }
        }
    }

    public void SendPlayerPosition(int playerId)
    {
        if (players.Count <= playerId || players[playerId] == null) return;
        
        isPostRequestPending = true;
        Vector3 position = players[playerId].GetPosition();

        ServerData data = new ServerData
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z
        };
        
        string safeGameId = string.IsNullOrEmpty(gameId) ? "partida1" : gameId;
        StartCoroutine(api.PostPlayerData(safeGameId, playerId.ToString(), data, () => {
            isPostRequestPending = false;
        }));
    }
}
