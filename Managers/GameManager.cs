using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // starting pos of tanks
    [Header("Initilization")]
    [SerializeField] GameObject escapeMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] List<GameObject> PlayerGUIs;
    [SerializeField] CameraEffects camEffects;
    [SerializeField] GameObject tankPrefab;
    [SerializeField] List<GameObject> spawnPointList;
    [SerializeField] List<Music> musicList;
    bool gameActive = false;
    bool endingRound = false;
    int songPlaying;
    float timeTillNextSong;

    [Header("Game Properties")]
    Game game;
    List<Player> players;
    [SerializeField] bool devMode;
    [SerializeField] int startingHP;
    [SerializeField] int rounds;
    [SerializeField] Gamemode mode;
    public int numPlayers = 2;

    private void Awake()
    {
        AudioManager.Initialize();
        players = new List<Player>();
    }

    private void Start()
    {
        PlayNextSong();
        if (devMode)
        {
            Debug.LogWarning("Starting Game in Dev Mode!");
            foreach (TankController tc in UnityEngine.Object.FindObjectsOfType<TankController>())
                tc.dealDamage(-1000000);
        }
    }
    private void Update()
    {
        if (Time.time > timeTillNextSong)
            PlayNextSong();
        if (gameActive)
        {
            // Check For Round Ending
            GameLogic();

            // Commands
            if (Input.GetKeyDown(KeyCode.R))
            {
                Clear();
                LoadPlayers();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                EscapeMenuToggle();
        }
    }
    private void GameLogic()
    {
        int playersAlive = 0;
        foreach (Player p in players)
            if (!p.isDead()) playersAlive++;

        if (!endingRound && playersAlive <= 1)
            StartCoroutine(NextRound());
            // End Game
    }
    IEnumerator NextRound()
    {
        // tick... tick... tick.
        endingRound = true;
        Debug.Log("Ending Round in 3 seconds...");
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1);
            AudioManager.PlaySound(Sound.Tick);
        }

        // Start new Round
        endingRound = false;
        if (game.NextRound())
        {
            // Reset Game
            Clear();
            LoadPlayers();
            foreach (Player p in players)
                p.ResetHP();
        }
        else
        {
            // No More Rounds
            players.Clear();
            game = null;
            ReturnToMainMenu();
        }
    }
    private void Clear()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("sound"))
            Destroy(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("projectile"))
            Destroy(obj);
        // Destroy players
        foreach (TankController obj in UnityEngine.Object.FindObjectsOfType<TankController>())
            Destroy(obj.gameObject);

    }
    private void EscapeMenuToggle()
    {
        Time.timeScale = !escapeMenu.activeInHierarchy ? 0 : 1;
        escapeMenu.SetActive(!escapeMenu.activeInHierarchy);
    }

    private void PlayNextSong()
    {
        if (timeTillNextSong == 0) // play random first song.
        {
            songPlaying = Random.Range(0, musicList.Count - 1);
        }
        else // play next song...
        {
            songPlaying++;
            if (songPlaying >= musicList.Count)
                songPlaying = 0;
        }
        timeTillNextSong = AudioManager.PlayMusic(musicList[songPlaying]) + Time.unscaledTime;
    }
    private void EndGame()
    {

    }
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        gameActive = false;
        Clear();

        // Enable menu effects
        camEffects.gameObject.SetActive(true);
        PlayerGUIs[numPlayers - 2].SetActive(false);
        mainMenu.SetActive(true);
    }
    public void PlayGame(int numberOfPlayers)
    {
        numPlayers = numberOfPlayers;
        LoadPlayers();
        game = new Game(mode, rounds, players);

        // Adjust Menus  
        mainMenu.SetActive(false);
        camEffects.StopEffects();
        camEffects.gameObject.SetActive(false);
        PlayerGUIs[numPlayers - 2].SetActive(true);

        gameActive = true;
    }
    public void QuitGame()
    {
        Debug.Log("Game cannot be exited when launched from editor");
        Application.Quit();
    }
    private void LoadPlayers()
    {
        switch (numPlayers)
        {
            // Note: Rect(position (x,y), size (width, height))

            case 2:
                InitilizePlayer(1, spawnPointList[0].transform.position, "player1",
                    new Rect(new Vector2(0, 0), new Vector2(.5f, 1)), false);
                InitilizePlayer(2, spawnPointList[1].transform.position, "player2",
                    new Rect(new Vector2(.5f, 0), new Vector2(.5f, 1)), false);
                break;

            case 3:
                InitilizePlayer(1, spawnPointList[0].transform.position, "player1",
                    new Rect(new Vector2(0, 0), new Vector2(1f / 3f, 1)), true);
                InitilizePlayer(2, spawnPointList[1].transform.position, "player2",
                    new Rect(new Vector2(1f / 3f, 0), new Vector2(1f / 3f, 1)), true);
                InitilizePlayer(3, spawnPointList[2].transform.position, "player3",
                    new Rect(new Vector2(2f / 3f, 0), new Vector2(1f / 3f, 1)), true);
                break;

            case 4:
                InitilizePlayer(1, spawnPointList[0].transform.position, "player1",
                    new Rect(new Vector2(0, .5f), new Vector2(.5f, .5f)), false);
                InitilizePlayer(2, spawnPointList[1].transform.position, "player2",
                    new Rect(new Vector2(.5f, .5f), new Vector2(.5f, 1)), false);
                InitilizePlayer(3, spawnPointList[2].transform.position, "player3",
                    new Rect(new Vector2(0, 0), new Vector2(.5f, .5f)), false);
                InitilizePlayer(4, spawnPointList[3].transform.position, "player4",
                    new Rect(new Vector2(.5f, 0), new Vector2(.5f, .5f)), false);
                break;

            default: // invalid number of players
                Debug.LogError("\"" + numPlayers + "\" is an invalid amount of players to start the game with.");
                break;
        }
    }
    private void InitilizePlayer(int playerNum, Vector3 position, string name, Rect camDimensions, bool zoomOut)
    {
        // Create Tank and Camera
        GameObject cam = new GameObject("Camera");
        Camera camera = cam.AddComponent<Camera>();
        if (playerNum == 1) cam.AddComponent<AudioListener>();
        cam.transform.position = position;

        GameObject tank = Instantiate(tankPrefab);
        tank.transform.position = position;

        // Position & Adjust Camera 
        cam.transform.parent = tank.transform;
        camera.rect = camDimensions;
        if (zoomOut)  cam.transform.localPosition = new Vector3(0f, 65f, 0f); // place camera further
        else          cam.transform.localPosition = new Vector3(0f, 40f, 0f);
        cam.transform.eulerAngles = new Vector3(0f, -90f, 0f);
        //camera.clearFlags = CameraClearFlags.SolidColor;
        //camera.backgroundColor = new Color(162f/255f, 198f/255f, 255f/255f); // sky blue

        // Create Player 
        if (gameActive)
        {
            players[playerNum - 1].tank = tank;
            tank.GetComponent<Animator>().SetInteger("color", playerNum); // color
            tank.GetComponent<TankController>().setPlayer(players[playerNum - 1]);
        }
        else
        {
            Player p = new Player(tank, name, startingHP);
            tank.GetComponent<TankController>().setPlayer(p);
            tank.GetComponent<Animator>().SetInteger("color", playerNum); // color
            players.Add(p);
        }
    }
    public void PlayPickupSong(float seconds)
    {
        AudioManager.PlayMusic(Music.PickupSong);
        timeTillNextSong = seconds + Time.time;
    }
}