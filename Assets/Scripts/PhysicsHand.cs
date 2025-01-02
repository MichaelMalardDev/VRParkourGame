using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    [Header("PID")]
    [SerializeField] Rigidbody playerRigidbody;
    // [SerializeField] CharacterController playerCharacter;
    [SerializeField] float frequency = 50f;
    [SerializeField] float damping = 1f;
    [SerializeField] Transform target;
    [SerializeField] float rotFrequency = 100f;
    [SerializeField] float rotDamping = 0.9f;
    Rigidbody _rigidbody;

    [Space]
    [Header("Springs")]
    [SerializeField] float climbForce = 1000f;
    [SerializeField] float climbDrag = 500f;

    [Space]
    [Header("Climb")]
    [SerializeField] float sphereRadius = 0.5f;
    [SerializeField] float maxDistance = 2f;
    [SerializeField] LayerMask layerMask;



    float _drag;
    bool _isColliding;
    bool _isInClimbing;


    Vector3 _previousPosition;

    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxAngularVelocity = float.PositiveInfinity;
        _previousPosition = transform.position;
    }
    void Update()
    {
        // SphereCast();
    }

    // void OnDrawGizmos()
    // {
    //     // Obtenez la position et la direction de votre spherecast
    //     Vector3 origin = transform.position;
    //     Vector3 direction = transform.forward; // Par exemple, vous pouvez utiliser la direction vers l'avant de votre objet

    //     // Afficher le spherecast dans l'éditeur
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(origin, sphereRadius);

    //     // Déclarer une variable pour stocker les informations de collision
    //     RaycastHit hit;

    //     // Effectuer le spherecast
    //     if (Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, layerMask))
    //     {
    //         // Si le spherecast heurte quelque chose, vous pouvez obtenir les informations de collision à partir de la variable 'hit'
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawLine(origin, hit.point);
    //         Gizmos.DrawWireSphere(hit.point, 0.1f); // Afficher un point de collision
    //     }
    //     else
    //     {
    //         // Si le spherecast ne heurte rien, vous pouvez effectuer des actions en conséquence
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawLine(origin, origin + direction * maxDistance);
    //     }
    // }


    void SphereCast()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit hit;

        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, layerMask))
        {

            Debug.Log("Spherecast hit: " + hit.collider.name);

            // Vous pouvez également effectuer d'autres actions en fonction de la collision détectée
            // Par exemple, vous pouvez déclencher des mécanismes d'escalade ici
        }
        else
        {
            Debug.Log("Spherecast didn't hit anything");
        }
    }

    void FixedUpdate()
    {
        PIDMovement();
        PIDRotation();
        if (_isColliding) HookesLaw();
    }


    void PIDMovement()
    {
        float kp = (6f * frequency) * (6f * frequency) * 0.25f;
        float kd = 4.5f * frequency * damping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        // Vector3 force = (target.position - transform.position) * ksg + (playerRigidbody.velocity - _rigidbody.velocity) * kdg;
        Vector3 force = (target.position - transform.position) * ksg + (playerRigidbody.velocity - _rigidbody.velocity) * kdg;
        _rigidbody.AddForce(force, ForceMode.Acceleration);
    }



    void PIDRotation()
    {
        float kp = (6f * rotFrequency) * (6f * rotFrequency) * 0.25f;
        float kd = 4.5f * rotFrequency * rotDamping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Quaternion q = target.rotation * Quaternion.Inverse(transform.rotation);
        if (q.w < 0)
        {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        Vector3 torque = ksg * axis * angle + -_rigidbody.angularVelocity * kdg;
        _rigidbody.AddTorque(torque, ForceMode.Acceleration);
    }
    // playerCharacter.Move(force * Time.fixedDeltaTime);
    // playerCharacter.Move(_drag * -playerCharacter.velocity * climbDrag * Time.fixedDeltaTime);

    void HookesLaw()
    {
        Vector3 displacementFromResting = transform.position - target.position;
        Vector3 force = displacementFromResting * climbForce;
        _drag = GetDrag();

        playerRigidbody.AddForce(force, ForceMode.Acceleration);
        playerRigidbody.AddForce(_drag * -playerRigidbody.velocity * climbDrag, ForceMode.Acceleration);

    }


    float GetDrag()
    {
        Vector3 handVelocity = (target.localPosition - _previousPosition) / Time.fixedDeltaTime;
        float drag = 1 / handVelocity.magnitude + 0.01f;
        drag = drag > 1 ? 1 : drag;
        drag = drag < 0.03f ? 0.03f : drag;
        _previousPosition = transform.position;
        return drag;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _isColliding = true;
        if(collision.gameObject.tag == "Climb")
        {
            
        }
        else
        {
            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isColliding = false;
    }
}