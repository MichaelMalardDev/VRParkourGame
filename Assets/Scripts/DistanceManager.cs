using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class DistanceManager : MonoBehaviour
{
    public SOPerso playerData;
    public TextMeshProUGUI distanceText;

    private Vector3 startPosition;
    bool isRunning = false;

    IEnumerator UpdateDistance()
    {

        startPosition = transform.position;
        while (!playerData.newGame)
        {
            // Assurez-vous que la référence au transform du joueur et au texte sont définies
            if (transform != null && distanceText != null)
            {
                // Calcul de la distance parcourue en mètres
                playerData.distance = Vector3.Distance(startPosition, transform.position);

                // Vérification si la distance est supérieure à 99 mètres
                if (playerData.distance > 999f)
                {
                    // Convertir la distance en kilomètres
                    float distanceInKilometers = playerData.distance / 1000f;
                    distanceText.text = distanceInKilometers.ToString("F0") + " km";
                }
                else
                {
                    distanceText.text = playerData.distance.ToString("F0") + " m";
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void Update()
    {
        if (playerData.newGame) return;
        if (!isRunning && !playerData.newGame)
        {
            StartCoroutine(UpdateDistance());
            isRunning = true;
        }
        else if (playerData.newGame)
        {
            isRunning = false;
        }

        // Increase the time each second
        playerData.time += Time.deltaTime;

        if (!playerData.inTutorial)
        {
            playerData.timeScore = playerData.time;
            playerData.distanceScore = playerData.distance;
        }
    }

    void OnApplicationQuit()
    {
        playerData.totalDistance += playerData.distanceScore;
        playerData.totalTime += playerData.timeScore;
    }
}
