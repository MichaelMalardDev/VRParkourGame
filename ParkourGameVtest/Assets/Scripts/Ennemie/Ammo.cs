using System.Collections;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public float speed = 10f; // Vitesse de déplacement du laser
    SOPerso playerData;
    private int _damage = 1; // Dégâts infligés par le laser
    public int damage
    {
        get { return _damage; }
        set { _damage = value; }
    }
    private Rigidbody rb; // Référence au Rigidbody du laser

    void Start()
    {
        // Obtenez la référence au Rigidbody du laser
        rb = GetComponent<Rigidbody>();
        // Appliquez une vitesse de déplacement au Rigidbody du laser dans sa direction vers l'avant
        rb.velocity = transform.forward * speed;
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            return;
        }


        if (other.gameObject.tag == "Player")
        {
            // Récupérer le script PlayerHealth de l'objet Player
            playerData = other.gameObject.GetComponentInParent<ContinuousMovementPhysics>().playerData;
            // Si le script PlayerHealth est récupéré
            if (playerData != null)
            {
                // Appliquer des dégâts au joueur
                playerData.TakeDamage(_damage);
                // StartCoroutine(playerData.AnimateDamage());
                StartCoroutine(AnimateInDamage());
                Debug.Log("Player Health: " + playerData.health);
            }
        }

        // Détruire le laser lorsqu'il entre en collision avec un objet
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<TrailRenderer>().enabled = false;
        Destroy(gameObject, 2f);
    }

    IEnumerator AnimateInDamage()
    {
        Debug.Log("Animate Damage");
        float duration = 0.5f; // Duration of the animation in seconds
        float elapsedTime = 0.0f; // Elapsed time since the animation started

        Color startColor = playerData.vignetteProfile.color.value; // Get the current color
        Color targetColor = Color.red; // Set the target color

        float startIntensity = playerData.vignetteProfile.intensity.value; // Get the current intensity
        float targetIntensity = 0.5f; // Set the target intensity

        while (elapsedTime < duration)
        {
            // Calculate the interpolation value between 0 and 1 based on the elapsed time and duration
            float t = elapsedTime / duration;

            // Interpolate the color between the start color and the target color
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            if (playerData.vignette == false)
            {
                float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                playerData.vignetteProfile.intensity.value = currentIntensity;
            }
            // Set the interpolated color to the vignette profile
            playerData.vignetteProfile.color.value = currentColor;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Set the final color to the vignette profile
        playerData.vignetteProfile.color.value = targetColor;

        if (playerData.vignette == false)
        {
            // Set the final intensity to the vignette profile
            playerData.vignetteProfile.intensity.value = targetIntensity;
        }

        StartCoroutine(AnimateOutDamage());
    }

    IEnumerator AnimateOutDamage()
    {
        Debug.Log("Animate Damage");
        float duration = 0.5f; // Duration of the animation in seconds
        float elapsedTime = 0.0f; // Elapsed time since the animation started

        Color startColor = playerData.vignetteProfile.color.value; // Get the current color
        Color targetColor = Color.black; // Set the target color

        float startIntensity = playerData.vignetteProfile.intensity.value; // Get the current intensity
        float targetIntensity = 0.0f; // Set the target intensity


        while (elapsedTime < duration)
        {
            // Calculate the interpolation value between 0 and 1 based on the elapsed time and duration
            float t = elapsedTime / duration;

            // Interpolate the color between the start color and the target color
            Color currentColor = Color.Lerp(startColor, targetColor, t);

            if (playerData.vignette == false)
            {
                float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                playerData.vignetteProfile.intensity.value = currentIntensity;
            }

            // Set the interpolated color to the vignette profile
            playerData.vignetteProfile.color.value = currentColor;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Set the final color to the vignette profile
        playerData.vignetteProfile.color.value = targetColor;

        if (playerData.vignette == false)
        {
            playerData.vignetteProfile.intensity.value = targetIntensity;
        }
    }
}
