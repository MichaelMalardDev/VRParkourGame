using UnityEngine;

public class ShowCollidersInEditMode : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            Gizmos.color = Color.blue; // Set the color of the gizmo
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size); // Draw the collider bounds as a wireframe cube
        }
    }
}