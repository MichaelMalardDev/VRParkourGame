using UnityEngine;
using EzySlice;
using UnityEngine.XR.Interaction.Toolkit;


public class SliceObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public float requiredVelocity = 5f;
    public LayerMask sliceLayer;
    public LayerMask otherLayer;
    
    public Material crossSectionMaterial;
    public ParticleSystem explosionEffect;
    public SwordHandler swordHandler;

    [Space]
    [Header("XR Controller")]
    public ActionBasedController xrController;

    public float amplitudeHover = .1f;
    public float amplitudeSlice = .5f;


    private float _cutForce;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!swordHandler._grabbingSword) return;

        bool hasHitSpecific = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceLayer);
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hitInfo, otherLayer);
        if (hasHit)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, hitInfo.point, Quaternion.identity);
            Destroy(explosion.gameObject, explosion.main.duration + 1f);
            TriggerHaptics(amplitudeHover);
        }

        if (hasHitSpecific)
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Debug.Log("Velocity: " + velocity.magnitude);
    }

    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        _cutForce = velocity.magnitude * 100f;

        if (velocity.magnitude >= requiredVelocity)
        {
            if (hull != null)
            {
                TriggerHaptics(amplitudeSlice);
                GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
                SetupSliceComponents(upperHull);
                GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
                SetupSliceComponents(lowerHull);
                Destroy(target);
            }
            else
            {
                Debug.Log("Slice failed");
            }
        }
    }

    void TriggerHaptics(float amplitude, float durationInSeconds = .3f)
    {
        xrController.SendHapticImpulse(amplitude, durationInSeconds);
    }

    public void SetupSliceComponents(GameObject sliceObject)
    {
        Rigidbody rb = sliceObject.AddComponent<Rigidbody>();
        MeshCollider collider = sliceObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(_cutForce, sliceObject.transform.position, 1f);
        sliceObject.layer = LayerMask.NameToLayer("Sliceable");
    }
}
