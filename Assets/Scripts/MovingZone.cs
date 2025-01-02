using System.Linq;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder;
using UnityEngine;


public class MovingZone : MonoBehaviour
{
    public SOPerso playerData;
    public float speed = 5.0f; // Vitesse de déplacement de la zone

    public TypeMur typeMur; // Type de mur de la zone
    public Vector3 direction = Vector3.forward; // Direction dans laquelle la zone se déplace
    // private Vector3 initialScale; // Taille initiale de la zone
    ProBuilderMesh _zoneMesh; // Mesh de la zone
    // public Transform center; // Centre de la zone

    float zMax;

    Vertex[] _vertices; // Vertices du mesh de la zone
    Vertex[] _goodVertices; // Vertices du mesh de la zone



    // Start is called before the first frame update
    void Start()
    {
        // Calculer le centre du rectangle en utilisant l'échelle initiale
        // initialScale = gameObject.GetComponent<Renderer>().bounds.size;


        // Récupérer le mesh de la zone
        _zoneMesh = GetComponent<ProBuilderMesh>();
        _vertices = _zoneMesh.GetVertices();

        zMax = _vertices.Max(v => v.position.z);
        _goodVertices = _vertices.Where(v => Mathf.Round(v.position.z * 100f) / 100f >= zMax).ToArray();
        _vertices = _vertices.Where(v => !_goodVertices.Contains(v)).ToArray();

    }




    void Update()
    {
        if(playerData.newGame) return;
        if (typeMur == TypeMur.Petit)
        {
            // Déplacer la zone dans la direction spécifiée à la vitesse spécifiée
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else if (typeMur == TypeMur.Large)
        {
            // Déplacer la zone
            MoveZone();
        }
        // // Créer une boîte de collision à la position actuelle de la zone
        // Collider[] colliders = Physics.OverlapBox(center.position, initialScale, Quaternion.identity);

        // // Parcourir tous les objets avec lesquels la boîte de collision est en collision
        // foreach (Collider collider in colliders)
        // {
        //     // Faire quelque chose avec l'objet en collision, par exemple :
        //     Debug.Log("Collision avec : " + collider.name);
        // }



    }

    // Dessiner la boîte de collision dans l'éditeur Unity (gizmos)
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Vector3 boxSize = Vector3.Scale(transform.localScale, initialScale);
    //     Gizmos.DrawWireCube(center.position, initialScale);
    // }


    void MoveZone()
    {
        // Grossir la zone
        foreach (Vertex vertex in _goodVertices)
        {
            vertex.position += direction * speed * Time.deltaTime;
        }
        // _vertices = _vertices.Concat(_goodVertices).ToArray();
        RebuildZone();
    }

    void RebuildZone()
    {
        _zoneMesh.SetVertices(_vertices.Concat(_goodVertices).ToArray());
        _zoneMesh.ToMesh();

        _zoneMesh.RebuildColliders();
        _zoneMesh.Refresh();
    }

    void OnTriggerEnter(Collider other)
    {
        if (typeMur == TypeMur.Large)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player entered the zone");
                ZoneManager zoneManager = other.GetComponent<ZoneManager>();
                zoneManager.Dissolve(true);
                
            }
            if (other.gameObject.CompareTag("TuileBase"))
            {
                
                TuileBase tuileBase = other.GetComponent<TuileBase>();
                tuileBase.StartDestroy();
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (typeMur == TypeMur.Large)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                
                ZoneManager zoneManager = other.GetComponent<ZoneManager>();
                // SOPerso playerData = other.gameObject.GetComponent<ZoneManager>().playerData;
                zoneManager.Dissolve(false);
            }
        }

    }

    public enum TypeMur
    {
        Petit,
        Large,
    }
}

