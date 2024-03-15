using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{

    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
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
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        var destPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.Character.SetPosAndSnapToTile(destPortal.spawnPoint.position);
        Destroy(gameObject);
    }
}
