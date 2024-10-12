using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> ObjectsToClear;
    // Start is called before the first frame update
    void Start()
    {
        RandomRotate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RandomRotate()
    {
        foreach (GameObject obj in ObjectsToClear)
        {
            obj.transform.rotation = Quaternion.Euler(0, 45 * (int)Random.Range(0, 8), 0);
        }
    }
    public void ClearTile()
    {
        foreach (GameObject obj in ObjectsToClear)
        {
            Destroy(obj);
        }
    }
}
