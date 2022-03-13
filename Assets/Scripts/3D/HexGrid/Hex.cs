using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Hex : MonoBehaviour
{
    private HexCoordinates hexCoordinates;
    [SerializeField]
    private GlowHighlight highlight;

    [SerializeField]
    private HexType hexType;
    
    public Vector3Int HexCoords => hexCoordinates.GetHexCoords();

    public int GetCost()
    {
        int cost = 0;
        switch (hexType)
        {
            case HexType.Difficult: 
                cost = 20;
                break;
            case HexType.Default:
                cost = 10;
                break;
            case HexType.Road:
                cost = 5;
                break;
            case HexType.Water:
                cost = 15;
                break;
            default:
                throw new System.Exception($"Hex of type {hexType} not supported");
        }
        return cost;
    }

    public bool IsObstacle()
    {
        return this.hexType == HexType.Obstacle;
    }

    private void Awake() 
    {
        hexCoordinates = GetComponent<HexCoordinates>();
        highlight = GetComponent<GlowHighlight>();
    }

    public void EnableHighlight()
    {
        highlight.ToggleGlow(true);
    }

    public void DisableHighlight()
    {
        highlight.ToggleGlow(false);
    }

    internal void ResetHightLight()
    {
        highlight.ResetGlowHighlight();
    }

    internal void HighlightPath()
    {
        highlight.HighlightValidPath();
    }
}

public enum HexType
{
    None,
    Default,
    Difficult,
    Road,
    Water,
    Obstacle
}