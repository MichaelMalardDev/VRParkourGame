using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float dissolveRate = 0.0125f;
    public float refrechRate = 0.025f;
    private Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        if (skinnedMeshRenderer != null)
        {
            materials = skinnedMeshRenderer.materials;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DissolveEffect());
        }
    }

    IEnumerator DissolveEffect()
    {
        if(materials.Length > 0)
        {
            float counter = 0;
            while (materials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refrechRate);
            }
        }
    }
}
