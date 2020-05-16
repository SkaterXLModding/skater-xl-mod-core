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
}


public class SXLTeleportVolume : SXLVolume
{
    private Transform _destination;

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
