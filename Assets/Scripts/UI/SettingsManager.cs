using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public SOPerso playerData;
    public GameObject[] tabButtons; // Array of tab buttons
    public GameObject[] tabContents; // Array of tab content panels
    public GameObject[] rotationOptionsButtons; // Array of rotation options buttons
    public GameObject[] handSpeedOptionsButtons; // Array of hand speed options buttons

    public GameObject checkBoxAButton;
    public GameObject checkBoxVigette;

    public TextMeshProUGUI textDistance;
    public TextMeshProUGUI textTime;

    private Color baseColor;
    public float timeAnimations;

    private void Start()
    {
        baseColor = tabButtons[0].GetComponent<MeshRenderer>().material.color;
        // Initialize UI by showing the content of the first tab and hiding others
        ShowTabContent(0);
        if (playerData.turnSpeed == 70) ShowRotationOptions(0);
        else if (playerData.turnSpeed == 100) ShowRotationOptions(1);
        else if (playerData.turnSpeed == 120) ShowRotationOptions(2);

        if (playerData.minJumpWithHandSpeed == 0.5f) ShowHandSpeedOptions(0);
        else if (playerData.minJumpWithHandSpeed == 1f) ShowHandSpeedOptions(1);
        else if (playerData.minJumpWithHandSpeed == 2f) ShowHandSpeedOptions(2);

        checkBoxAButton.GetComponent<MeshRenderer>().material.color = playerData.jumpWithHand ? baseColor : Color.red;
        checkBoxVigette.GetComponent<MeshRenderer>().material.color = playerData.vignette ? Color.red : baseColor;

    }

    void Update()
    {
        // Distance
        string distanceString;
        if (playerData.totalDistance > 999f)
        {
            float distanceInKilometers = playerData.totalDistance / 1000f;
            distanceString = distanceInKilometers.ToString("F0") + " km";
        }
        else
        {
            distanceString = playerData.totalDistance.ToString("F0") + "m";
        }
        textDistance.text = "Distance total: " + distanceString;
        // Temps
        TimeSpan timeSpan = TimeSpan.FromSeconds(playerData.totalTime);
        string timeString = timeSpan.ToString("hh':'mm':'ss");
        textTime.text = "Temps total: " + timeString;

        if (playerData.vignette)
        {
            checkBoxVigette.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            checkBoxVigette.GetComponent<MeshRenderer>().material.color = baseColor;
        }

        if (playerData.jumpWithHand)
        {
            checkBoxAButton.GetComponent<MeshRenderer>().material.color = baseColor;
        }
        else
        {
            checkBoxAButton.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }


    // Called when a tab button is clicked
    public void OnTabButtonClick(int tabIndex)
    {
        ShowTabContent(tabIndex);
    }

    public void OnRotationOptionsButtonClick(int index)
    {
        ShowRotationOptions(index);
    }

    public void OnHandSpeedOptionsButtonClick(int index)
    {
        ShowHandSpeedOptions(index);
    }

    // Show content of the specified tab index
    private void ShowTabContent(int indexToShow)
    {
        for (int i = 0; i < tabContents.Length; i++)
        {
            if (i == indexToShow)
            {
                // tabContents[i].SetActive(true);
                tabButtons[i].GetComponent<Animator>().SetBool("IsOn", true);
                tabButtons[i].GetComponent<MeshRenderer>().material.color = Color.red;
                StartCoroutine(AnimateContent(tabContents[i], true));
            }
            else
            {
                tabButtons[i].GetComponent<Animator>().SetBool("IsOn", false);
                tabButtons[i].GetComponent<MeshRenderer>().material.color = baseColor;
                StartCoroutine(AnimateContent(tabContents[i], false));
            }
        }
    }

    private void ShowRotationOptions(int indexToShow)
    {
        for (int i = 0; i < rotationOptionsButtons.Length; i++)
        {
            if (i == indexToShow)
            {
                rotationOptionsButtons[i].GetComponent<Animator>().SetBool("IsOn", true);
                rotationOptionsButtons[i].GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else
            {
                rotationOptionsButtons[i].GetComponent<Animator>().SetBool("IsOn", false);
                rotationOptionsButtons[i].GetComponent<MeshRenderer>().material.color = baseColor;
            }
        }
    }

    private void ShowHandSpeedOptions(int indexToShow)
    {
        for (int i = 0; i < handSpeedOptionsButtons.Length; i++)
        {
            if (i == indexToShow)
            {
                handSpeedOptionsButtons[i].GetComponent<Animator>().SetBool("IsOn", true);
                handSpeedOptionsButtons[i].GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else
            {
                handSpeedOptionsButtons[i].GetComponent<Animator>().SetBool("IsOn", false);
                handSpeedOptionsButtons[i].GetComponent<MeshRenderer>().material.color = baseColor;
            }
        }
    }


    private IEnumerator AnimateContent(GameObject content, bool show)
    {
        if (show)
        {
            content.SetActive(true);
            yield return new WaitForSeconds(timeAnimations);
            content.LeanMoveLocalZ(0, timeAnimations).setEaseInOutCubic();
        }
        else
        {
            content.LeanMoveLocalZ(0.06f, timeAnimations).setEaseInOutCubic();
            yield return new WaitForSeconds(timeAnimations);
            content.SetActive(false);
        }
    }
}
