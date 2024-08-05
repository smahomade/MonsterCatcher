using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask fenceLayer;
    [SerializeField] LayerMask objectLayer;
    [SerializeField] LayerMask heroLayer;
    [SerializeField] LayerMask wildGrassLayer;

    
    public static GameLayers n { get; set; }

    private void Awake()
    {
        n = this;
    }
    public LayerMask ObjectLayer
    {
        get => objectLayer;
    }
    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }
    public LayerMask WildGrassLayer
    {
        get => wildGrassLayer;
    }
    public LayerMask HeroLayer
    {
        get => heroLayer;
    }
    public LayerMask FovLayer
    {
        get => fovLayer;
    }
    public LayerMask Fence
    {
        get => fenceLayer;
    }
}
