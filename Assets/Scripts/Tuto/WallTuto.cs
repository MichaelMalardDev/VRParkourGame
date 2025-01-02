using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTuto : MonoBehaviour
{
    public GameObject wall;
    void Start()
    {
        wall.SetActive(true);
    }

    void Update()
    {
        if(wall.transform.localScale.y == 0)
        {
            Destroy(wall);
            // wall.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            AnimateWall();
        }
    }

    void AnimateWall()
    {
        wall.LeanScaleY(0, 0.5f).setEaseOutCubic();
    }


}
