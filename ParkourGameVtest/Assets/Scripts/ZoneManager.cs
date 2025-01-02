using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZoneManager : MonoBehaviour
{
    public SOPerso playerData;
    public Camera mainCamera;
    public TMP_Text countdownText;

    public int timeBeforeDead = 5;
    private int _timerBeforeDead = 0;

    [Space]
    [Header("Desolve Effect")]
    public SkinnedMeshRenderer skinnedMeshRendererLeft;
    public SkinnedMeshRenderer skinnedMeshRendererRight;
    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;
    private Material[] materialsLeft;
    private Material[] materialsRight;


    void Awake()
    {
        if (playerData != null)
        {
            playerData.Initialize();
        }
    }

    void Start()
    {
        countdownText.text = "";
        if (skinnedMeshRendererLeft != null)
        {
            materialsLeft = skinnedMeshRendererLeft.materials;
        }
        if (skinnedMeshRendererRight != null)
        {
            materialsRight = skinnedMeshRendererRight.materials;
        }

    }

    public void Dissolve(bool dissolve = true)
    {
        // StartCoroutine(DissolveEffect(dissolve, materialsLeft));
        // StartCoroutine(DissolveEffect(dissolve, materialsRight));
        if (dissolve)
        {
            Debug.Log("TuileBase entered the zone");
            playerData.inZone = true;
            StartCoroutine(TimerBeforeDead());
        }
        else if (!dissolve)
        {
            Debug.Log("Player exited the zone");
            playerData.inZone = false;
            _timerBeforeDead = 0;
            // countdownText.text = "";
            StartCoroutine(DissolveEffect(dissolve, materialsLeft));
            StartCoroutine(DissolveEffect(dissolve, materialsRight));
        }
    }

    IEnumerator TimerBeforeDead(bool dissolve = true)
    {
        if (playerData.inZone == false)
        {
            yield break;
        }
        while (_timerBeforeDead < timeBeforeDead && !playerData.isDead && playerData.inZone)
        {
            yield return new WaitForSeconds(1f);
            if (playerData.inZone == false)
            {
                countdownText.text = "";
            }
            else
            {
                countdownText.text = (timeBeforeDead - _timerBeforeDead).ToString();
                _timerBeforeDead++;
            }
        }
        Debug.Log("Starting Dissolve timer Finished");
        if (_timerBeforeDead == timeBeforeDead)
        {
            StartCoroutine(DissolveEffect(dissolve, materialsLeft));
            StartCoroutine(DissolveEffect(dissolve, materialsRight));
        }
    }



    IEnumerator DissolveEffect(bool dissolve, Material[] materials)
    {
        // Debug.Log("DissolveEffect: " + dissolve);
        if (materials.Length > 0)
        {
            float targetAmount = dissolve ? 1f : 0f;

            while (!Mathf.Approximately(materials[2].GetFloat("_DissolveAmount"), targetAmount))
            {
                float counter = Mathf.MoveTowards(materials[2].GetFloat("_DissolveAmount"), targetAmount, dissolveRate);

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
            if (dissolve && playerData.isDead == false)
            {
                Debug.Log("Dead");
                playerData.totalDistance += playerData.distance;
                playerData.totalTime += playerData.time;
                playerData.isDead = true;
                LeaderboardManager.instance.SaveScore(playerData.distance);
            }
        }
    }
}
