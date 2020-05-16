using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SXLVolume : MonoBehaviour
{
    public bool visibleInGame = false;
    private MeshRenderer renderer;
    void Awake()
    {
        renderer = this.GetComponent<MeshRenderer>();
        renderer.enabled = this.visibleInGame;
    }

    public void SetVisibility(bool visible)
    {
        this.visibleInGame = visible;
        renderer.enabled = visible;
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


public class SXLTeleportVolume : SXLVolume
{
    private Transform _destination;
    private Vector3 _pinPos;
    private Quaternion _pinQuat;

    void Start()
    {
        this._destination = this.transform.GetChild(0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Skater"))
        {
            Respawn r = PlayerController.Instance.respawn;
            // Break the pointer reference
            Vector3 playerOffset = r.getSpawn[0].position - r.getSpawn[1].position - new Vector3(0.0f, 0.08f, 0.0f);
            this._pinPos = new Vector3(r.pin.position.x, r.pin.position.y, r.pin.position.z);  
            this._pinQuat = new Quaternion(r.pin.rotation.x, r.pin.rotation.y, r.pin.rotation.z, r.pin.rotation.w);
            r.SetSpawnPos(this._destination.position - playerOffset, this._destination.rotation);
            r.DoRespawn();
            r.SetSpawnPos(this._pinPos - playerOffset, this._pinQuat);
        }
    }

    void OnDrawGizmos()
    {
        if (this.gameObject.transform.childCount > 0)
        {
            this._destination = this.transform.GetChild(0);
        }
        else
        {
            GameObject endpoint = new GameObject("Teleport_Endpoint");
            endpoint.transform.position = this.transform.position;
            endpoint.transform.parent = this.transform;
            this._destination = endpoint.transform;
        }
        Gizmos.DrawWireSphere(this._destination.position, 1.0f);
        Gizmos.DrawLine(this.transform.position, this._destination.position);
    }
}
