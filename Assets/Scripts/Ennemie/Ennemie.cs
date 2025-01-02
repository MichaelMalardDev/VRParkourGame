using UnityEngine;

public class Ennemie : MonoBehaviour
{
    public float rotationSpeed = 5f; // Vitesse de rotation de l'ennemi
    public float distanceToDetect = 10f;
    public float fireRate = 1f; // Taux de tir en tirs par seconde
    public int damage = 1; // Dégâts infligés par l'ennemi
    SphereCollider sphereCollider;
    private Transform player; // Référence au transform du joueur

    public Ammo laserPrefab; // Préfabriqué du laser

    public Transform firePoint; // Point de tir du laser

    private float nextFireTime; // Temps du prochain tir


    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Vérifie si le joueur est entré dans le collider trigger
        {
            player = other.transform; // Obtient la référence du transform du joueur
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Vérifie si le joueur a quitté le collider trigger
        {
            player = null; // Réinitialise la référence du joueur
        }
    }

    void Update()
    {

        if (sphereCollider == null)
        {
            Debug.LogError("Le collider est manquant");
            return;
        }
        else
        {
            sphereCollider.radius = distanceToDetect;
        }


        // Vérifie si le joueur a été trouvé
        if (player != null)
        {
            // Calcul de la direction vers le joueur
            Vector3 direction = player.position - transform.position;
            direction.y = 0f; // Ne tournez pas sur l'axe Y

            // Rotation de l'ennemi pour faire face au joueur
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation *= Quaternion.Euler(0, 0, 90f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Lance le tir du laser si c'est le temps
            if (Time.time >= nextFireTime)
            {
                ShootLaser();
                nextFireTime = Time.time + 1f / fireRate; // Met à jour le temps du prochain tir
            }
        }



    }

    void ShootLaser()
    {
        if (laserPrefab != null && firePoint != null)
        {
            // Instancie le laser depuis le point de tir
            Ammo laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
            laser.damage = damage; // Applique les dégâts au laser
            // Assure que le laser est détruit après un certain temps
            Destroy(laser, 2f);
        }
    }
}
