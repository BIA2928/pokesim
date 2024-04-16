using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    [SerializeField] AudioClip sceneMusic;

    public AudioClip SceneMusic => sceneMusic;
    public bool IsLoaded { get; private set; }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Entered {gameObject.name}");

            LoadScene();
            GameController.i.SetCurrentScene(this);
            if (sceneMusic != null)
                AudioManager.i.PlayMusic(sceneMusic, fade: true);
            foreach (SceneDetails scene in connectedScenes)
            {
                scene.LoadScene();
            } 

            if (GameController.i.PrevScene != null)
            {
                var previousLoadedScenes = GameController.i.PrevScene.connectedScenes;
                foreach (var scene in previousLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            SceneManager.LoadSceneAsync($"{gameObject.name}", LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            Debug.Log($"unloading {gameObject.name}");
            SceneManager.UnloadSceneAsync($"{gameObject.name}");
            IsLoaded = false;
        }
    }

}
