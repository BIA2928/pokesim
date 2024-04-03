using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggersLayer;

    public LayerMask FovLayer
    {
        get => fovLayer;
    }

    public LayerMask SolidLayer
    {
        get => solidObjectsLayer;
    }

    public LayerMask GrassLayer
    {
        get => grassLayer;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }

    public LayerMask PortalLayer
    {
        get => portalLayer;
    }

    public LayerMask TriggerableLayers
    {
        get => grassLayer | fovLayer | portalLayer | triggersLayer;
    }

    public LayerMask TriggersLayer
    {
        get => triggersLayer;
    }

    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }

}