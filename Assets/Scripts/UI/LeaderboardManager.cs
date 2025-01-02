using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using Firebase.Database;

public class LeaderboardManager : MonoBehaviour
{
    static private LeaderboardManager _instance; //creation du singleton pour la classe niveau
    static public LeaderboardManager instance => _instance;

    //Firebase
    DatabaseReference databaseRef;

    // Nombre maximal de scores à afficher
    private const int maxScoresToShow = 10;

    // Liste des meilleurs scores
    [SerializeField] private List<float> topScores = new List<float>();

    public TextMeshProUGUI scoreText;

    public SOPerso playerData;
    public TextMeshProUGUI popUpDistanceText;
    public TextMeshProUGUI popUpTempsText;
    public TextMeshProUGUI popUpCouleurText;



    void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;

        // Charger les meilleurs scores depuis PlayerPrefs
        // LoadScores();

        // Initialiser Firebase
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        // Initialiser Firebase
        InitializeFirebase();

        // Charger les meilleurs scores depuis Firebase
        LoadScoresFromFirebase();

    }

    // public void SaveScore(float distance)
    // {
    //     int intDistance = Mathf.FloorToInt(distance);
    //     // Ajouter le score à la liste des meilleurs scores
    //     topScores.Add(intDistance);

    //     // Trier la liste des scores
    //     topScores.Sort((a, b) => b.CompareTo(a)); // Tri décroissant

    //     // Garder uniquement les meilleurs scores (jusqu'à maxScoresToShow)
    //     if (topScores.Count > maxScoresToShow)
    //     {
    //         topScores.RemoveAt(topScores.Count - 1);
    //     }

    //     // Enregistrer les scores dans PlayerPrefs
    //     SaveScores();
    // }

    public void SaveScore(float distance)
    {
        int intDistance = Mathf.FloorToInt(distance);
        topScores.Add(intDistance);
        topScores.Sort((a, b) => b.CompareTo(a));

        if (topScores.Count > maxScoresToShow)
        {
            topScores.RemoveAt(topScores.Count - 1);
        }

        SaveScoresToFirebase();
    }

    private void SaveScoresToFirebase()
    {
        float[] scoresArray = topScores.ToArray();

        databaseRef.Child("TopScores").SetValueAsync(scoresArray).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to save scores to Firebase: " + task.Exception);
            }
            else
            {
                Debug.Log("Scores saved to Firebase successfully.");
            }
        });
    }

    private void LoadScoresFromFirebase()
    {
        databaseRef.Child("TopScores").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load scores from Firebase: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            topScores.Clear();

            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                float score = Convert.ToSingle(childSnapshot.Value);
                topScores.Add(score);
            }

            UpdateScoreText();
        });
    }


    public List<float> GetTopScores()
    {
        return topScores;
    }

    private void LoadScores()
    {
        // Charger les scores depuis PlayerPrefs
        string scoresString = PlayerPrefs.GetString("TopScores", "");

        if (!string.IsNullOrEmpty(scoresString))
        {
            string[] scoreArray = scoresString.Split(',');

            foreach (string score in scoreArray)
            {
                float parsedScore;
                if (float.TryParse(score, out parsedScore))
                {
                    topScores.Add(parsedScore);
                }
            }
        }
    }

    [ContextMenu("Update Score Text")]
    private void LoadAndUpdateUI()
    {
        // Charger les meilleurs scores depuis PlayerPrefs
        // LoadScores();

        // Mettre à jour le texte des meilleurs scores
        // UpdateScoreText();

        // Charger les meilleurs scores depuis Firebase
        LoadScoresFromFirebase();

        // Mettre à jour le texte des meilleurs scores
        UpdateScoreText();
    }

    private void SaveScores()
    {
        // Convertir la liste des scores en une chaîne de caractères séparée par des virgules
        string scoresString = string.Join(",", topScores);

        // Enregistrer la chaîne des scores dans PlayerPrefs
        PlayerPrefs.SetString("TopScores", scoresString);
        PlayerPrefs.Save();
    }


    public void UpdateScoreText()
    {
        // Mettre à jour le texte des meilleurs scores
        string scoreString = "";
        if (topScores.Count == 0)
        {
            scoreString = "Pas encore de scores\n";
        }
        else
        {
            for (int i = 0; i < topScores.Count; i++)
            {
                scoreString += (i + 1) + ". " + topScores[i].ToString() + "m\n";
            }
        }

        scoreText.text = scoreString;

        if (playerData.inTutorial) return;

        string distanceString;
        // Distance
        if (playerData.distance > 999f)
        {
            float distanceInKilometers = playerData.distanceScore / 1000f;
            distanceString = distanceInKilometers.ToString("F0") + " km";
        }
        else
        {
            distanceString = playerData.distanceScore.ToString("F0") + "m";
        }
        popUpDistanceText.text = "Distance: " + distanceString;


        // Temps
        TimeSpan timeSpan = TimeSpan.FromSeconds(playerData.timeScore);
        string timeString = timeSpan.ToString("mm':'ss");
        popUpTempsText.text = "Temps: " + timeString;
    }

    [ContextMenu("Clear Scores")]
    public void ClearScores()
    {
        // Effacer les meilleurs scores
        topScores.Clear();

        // Enregistrer les scores dans PlayerPrefs
        // SaveScores();

        // Enregistrer les scores dans Firebase
        SaveScoresToFirebase();

        // Mettre à jour le texte des meilleurs scores
        UpdateScoreText();
    }

}
