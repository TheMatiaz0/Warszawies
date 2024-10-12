using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestObstacle : MonoBehaviour
{
    [SerializeField]
    private GameObject Trees;
    // Start is called before the first frame update
    void Start()
    {
        Trees.transform.rotation = Quaternion.Euler(0, 45 * (int)Random.Range(0, 8), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
