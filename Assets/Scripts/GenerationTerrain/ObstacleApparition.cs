using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleApparition : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _probabiliteApparition;

    void Start()
    {
        float aleatoire = Random.Range(0f, 1f);
        bool estPresent = (aleatoire <= _probabiliteApparition);

        if (estPresent)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

   
}
