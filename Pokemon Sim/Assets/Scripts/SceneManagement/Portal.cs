using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{

    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destination;
    PlayerController player;
    public Transform SpawnPoint
    {
        get => spawnPoint;
    }

    public void OnPlayerTrigger(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        GameController.i.PauseGame(true);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destination == this.destination);
        player.Character.SetPosAndSnapToTile(destPortal.spawnPoint.position);
        GameController.i.PauseGame(false);
        Destroy(gameObject);
    }

    
}

public enum DestinationIdentifier
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I
}
