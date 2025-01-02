using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public SOPerso playerData;
    public GameObject[] uiPanels;
    public GameObject popUpPanel;

    void Start()
    {
        MelangerTableau(uiPanels);
        AnimatePanels(false);
        if (playerData.firstGame || playerData.inTutorial)
        {
            popUpPanel.SetActive(false);
        }
    }
    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        if (sceneIndex == 1)
        {
            playerData.inTutorial = false;
        }
        if (sceneIndex == 0)
        {
            playerData.inTutorial = true;
        }
    }

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        StartCoroutine(CoroutStartGame());
        if (playerData.firstGame != false)
        {
            playerData.firstGame = false;
        }
        playerData.InitializeScore();
    }

    void Update()
    {
        foreach (GameObject panel in uiPanels)
        {
            if (panel != null)
            {
                if (!LeanTween.isTweening(panel) && panel.transform.localScale.y <= 0f)
                {
                    Destroy(panel);
                }

            }
        }

        if (popUpPanel != null)
        {
            if (!LeanTween.isTweening(popUpPanel) && popUpPanel.transform.localScale.y <= 0f)
            {
                Destroy(popUpPanel);
            }
        }

        if (playerData.isDead)
        {
            ReloadScene();
        }
    }

    public void ClosePopUp()
    {
        float random = Random.Range(.1f, .5f);

        popUpPanel.LeanScaleY(0f, random).setEaseInCubic();
    }

    IEnumerator CoroutStartGame()
    {
        if (popUpPanel != null)
        {
            ClosePopUp();
        }
        AnimatePanels(true);
        yield return new WaitForSeconds(1f);
        playerData.newGame = false;
    }

    void AnimatePanels(bool close)
    {
        foreach (GameObject panel in uiPanels)
        {
            float random = Random.Range(.1f, .5f);
            panel.LeanScaleY(close ? 0f : panel.transform.localScale.y, random).setEaseInCubic();
        }
    }

    void MelangerTableau(GameObject[] elements)
    {
        // Parcours du tableau à partir du dernier élément
        for (int i = elements.Length - 1; i > 0; i--)
        {
            // Génération d'un index aléatoire entre 0 et i (inclus)
            int randomIndex = Random.Range(0, i + 1);

            // Échange des éléments à l'index i et randomIndex
            GameObject temp = elements[i];
            elements[i] = elements[randomIndex];
            elements[randomIndex] = temp;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void OnApplicationQuit()
    {
        playerData.firstGame = true;
    }
}
