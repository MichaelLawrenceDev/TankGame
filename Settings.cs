using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Keybinding
{
    KeyCode up;
    KeyCode down;
    KeyCode left;
    KeyCode right;
    KeyCode shoot;

    public Keybinding(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode shoot)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
        this.shoot = shoot;
    }
}
public class PlayerData
{
    int keybindingIndex;
    int colorIndex;
    string playerName;

    public void Save()
    {
        
    }
}
public class Settings : MonoBehaviour
{
    [SerializeField] List<Color> colors;
    List<Keybinding> keyPresets;

    private void Start()
    {
        // hardcoded default presets...
        // (wasd + tab), (ijkl + space), (arrowkeys + enter)
        keyPresets = new List<Keybinding>();
        keyPresets.Add(new Keybinding(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Tab)); 
        keyPresets.Add(new Keybinding(KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.Space)); 
        keyPresets.Add(new Keybinding(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.KeypadEnter));
    }
    public void SaveSettings()
    {
    }
    public void LoadSettings()
    {

    }
}
