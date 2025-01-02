using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject[] tGrapplingPoints;
    public GameObject[] tGripPoints;
    public GameObject[] tEnnemies;
    public TypeBuilding typeBuilding;
    private float chanceToDeactivateAll = 0.1f; // La probabilité que tous les objets soient désactivés

    void Start()
    {
        if (typeBuilding == TypeBuilding.Small)
        {
            foreach (GameObject obj in tGripPoints)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
        else if (typeBuilding == TypeBuilding.Medium)
        {

            foreach (GameObject obj in tGripPoints)
            {
                int randomIndex = Random.Range(0, tGripPoints.Length);
                obj.SetActive(randomIndex < tGripPoints.Length / 2);
            }

        }
        else if (typeBuilding == TypeBuilding.Large)
        {
            foreach (GameObject obj in tGripPoints)
            {
                obj.SetActive(true);
            }
        }
        // Retire les objets null du tableau
        tGrapplingPoints = RemoveNullObjects(tGrapplingPoints);

        if (tGrapplingPoints.Length > 0)
        {
            if (Random.value <= chanceToDeactivateAll)
            {
                DeactivateAllObjects(tGrapplingPoints);
            }
            else
            {
                ActivateRandomObject(tGrapplingPoints);
            }
        }
        else
        {
            Debug.LogWarning("Aucun objet à activer. Assurez-vous d'assigner des objets dans le tableau objectsToActivate.");
        }


        int desactivateEnnemie = Random.Range(0, 10);
        if (desactivateEnnemie == 5)
        {
            int randomIndexEnnemie = Random.Range(0, tEnnemies.Length);
            GameObject ennemieToActivate = tEnnemies[randomIndexEnnemie];
            ennemieToActivate.SetActive(true);
        }

        if (typeBuilding == TypeBuilding.Small)
        {
            chanceToDeactivateAll = 0.7f;
        }
        else if (typeBuilding == TypeBuilding.Medium)
        {
            chanceToDeactivateAll = 0.5f;
        }
        else if (typeBuilding == TypeBuilding.Large)
        {
            chanceToDeactivateAll = 0.3f;
        }
    }

    void ActivateRandomObject(GameObject[] objects)
    {
        int randomIndex = Random.Range(0, objects.Length);
        GameObject objectToActivate = objects[randomIndex];


        // Désactive tous les objets sauf celui à activer
        foreach (GameObject obj in objects)
        {
            bool shouldActivate = obj == objectToActivate;
            if (shouldActivate)
            {
                obj.SetActive(true);
            }
            else
            {
                Destroy(obj);
            }
        }
    }

    void DeactivateAllObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    // Retire les objets null du tableau
    GameObject[] RemoveNullObjects(GameObject[] array)
    {
        int nullCount = 0;

        // Compte le nombre d'objets null dans le tableau
        foreach (GameObject obj in array)
        {
            if (obj == null)
            {
                nullCount++;
            }
        }

        // Crée un nouveau tableau sans les objets null
        GameObject[] newArray = new GameObject[array.Length - nullCount];
        int index = 0;

        foreach (GameObject obj in array)
        {
            if (obj != null)
            {
                newArray[index++] = obj;
            }
        }

        return newArray;
    }
}

public enum TypeBuilding
{
    Small,
    Medium,
    Large
}
