using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SXLVolume : MonoBehaviour
{
    void Awake()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }
}

public class SXLBlockingVolume : SXLVolume
{

}

public class SXLTriggerVolume : SXLVolume
{
    

    void OnTriggerEnter(Collider other)
    {

    }

    void OnTriggerExit(Collider other)
    {

    }
}

public class SXLStreamingVolume : SXLVolume
{
    public string sceneName;
    private Scene baseScene = SceneManager.GetActiveScene();

    private IEnumerator AsyncLoader(bool unload)
    {
        AsyncOperation asyncOp;
        Scene scene = SceneManager.GetSceneByName(this.sceneName);

        if (unload)
        {
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            asyncOp = SceneManager.LoadSceneAsync(this.sceneName, LoadSceneMode.Additive);

            while (!asyncOp.isDone)
            {
                yield return null;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if (other.gameObject.name.Contains("Skater"))
        {
            this.StartCoroutine(AsyncLoader(false));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Skater"))
        {
            this.StartCoroutine(AsyncLoader(true));
        }
    }
}
