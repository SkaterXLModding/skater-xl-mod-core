using System.Collections;

using UnityEngine;


public class SXLRandomObstacle : MonoBehaviour
{
    public GameObject[] obstacles;
    private GameObject loadedObstacle;

    void Awake()
    {
        Debug.Log("Loading Obstacle");
        this.RefreshObstacle();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.RefreshObstacle();
        }
    }


    private void RefreshObstacle()
    {
        if (this.obstacles.Length > 0)
        {
            if (this.loadedObstacle != null)
            {
                Destroy(this.loadedObstacle);
            }
            this.loadedObstacle = Instantiate(this.obstacles[(int)Random.Range(0, this.obstacles.Length)], this.transform);
        }
    }
}
