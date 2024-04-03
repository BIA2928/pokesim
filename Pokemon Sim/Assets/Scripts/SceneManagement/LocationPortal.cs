using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destination;
    PlayerController player;
    Fader fader;

    public bool TriggerRepeatedly => false;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    public Transform SpawnPoint
    {
        get => spawnPoint;
    }

    public void OnPlayerTrigger(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport()
    {
        GameController.i.PauseGame(true);

        yield return fader.FadeIn(0.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destination == this.destination);
        player.Character.SetPosAndSnapToTile(destPortal.spawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.i.PauseGame(false);
    }
}
