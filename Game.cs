using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

public enum Gamemode
{
    Deathmatch,     // each kill counts as score
    LastManStanding // last person alive counts as score
}
public class Game : MonoBehaviour
{
    // Desc: Holds stats needed for game logic
    List<Player> players;
    List<int> scores;
    public int rounds { get; }
    int currentRound;
    public Gamemode mode { get; }

    public Game(Gamemode gameMode, int rounds, List<Player> players) 
    {
        scores = new List<int>();
        for (int i = 0; i <= players.Count; i++)
            scores.Add(0);

        mode = gameMode;
        this.rounds = rounds;
        currentRound = 1;
        this.players = players;
    }
    public float GetScore(Player player) 
    {
        return scores[players.IndexOf(player)];
    }
    public void lastPlayerAlive(Player player) { }
    public void addScore(Player player)
    {
        scores[players.IndexOf(player)]++;
        Debug.Log(player.name + " scores! total points: " + scores[players.IndexOf(player)]);
    }
    public void addScore(GameObject playerOBJ) 
    {
        foreach (Player p in players)
            if (p.tank == playerOBJ) 
                addScore(p);
    }
    public bool NextRound()
    {
        // returns true if rounds are avaliable;
        currentRound++;
        return currentRound <= rounds;
    }
}
public class Player
{
    // Desc: Holds stats needed for player logic
    public string name { get; set; }
    int hp;
    public GameObject tank { get; set; }
    int startingHP;

    public Player(GameObject tank, string name, int hp)
    {
        this.startingHP = hp;
        this.hp = hp;
        this.tank = tank;
        this.name = name;
    }
    public bool isDead() { return hp <= 0; }
    public void AddHP(int value) { this.hp += value; }
    public int GetHP() { return hp; }
    public void ResetHP() { hp = startingHP; }
}
