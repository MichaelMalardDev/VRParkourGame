using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabPhysics : MonoBehaviour
{
    public handEnum side;
    public SOPerso playerData;
    public InputActionProperty grabInputSource;
    public float radius = 0.1f;
    public LayerMask grabLayer;
    public AudioClip sonGrab;
    public AudioSource audioSource;
    public float pitchMax = 1.2f;
    public float pitchMin = 0.8f;
    public ActionBasedController xrController;
    [Range(0, 1)]
    public float amplitude = .1f;
    public float durationInSeconds = .3f;

    private FixedJoint _fixedJoint;
    private bool _isGrabbing = false;

    private bool _wasGrabbedLastFrame = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        bool isGrabButtonPressed = grabInputSource.action.ReadValue<float>() > 0.1f;

        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, radius, grabLayer, QueryTriggerInteraction.Ignore);

        if (nearbyColliders.Length > 0 && !_wasGrabbedLastFrame)
        {
            TriggerHaptics();
            _wasGrabbedLastFrame = true;
        }
        else if (nearbyColliders.Length == 0)
        {
            _wasGrabbedLastFrame = false;
        }

        if (isGrabButtonPressed && !_isGrabbing)
        {
            if (nearbyColliders.Length > 0)
            {
                GrabObject(nearbyColliders[0]);
            }
        }
        else if (!isGrabButtonPressed && _isGrabbing)
        {
            ReleaseObject();
        }
    }

    void TriggerHaptics()
    {
        xrController.SendHapticImpulse(amplitude, durationInSeconds);
    }

    void GrabObject(Collider collider)
    {
        Rigidbody nearbyRigidbody = collider.attachedRigidbody;

        _fixedJoint = gameObject.AddComponent<FixedJoint>();
        _fixedJoint.autoConfigureConnectedAnchor = false;
        // GestSonore.instance.JouerEffetSonore(sonGrab, 1, true);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(sonGrab);
        // Debug.Log("Je suis dans le grab");

        if (nearbyRigidbody)
        {
            _fixedJoint.connectedBody = nearbyRigidbody;
            _fixedJoint.connectedAnchor = nearbyRigidbody.transform.InverseTransformPoint(transform.position);
        }
        else
        {
            _fixedJoint.connectedAnchor = transform.position;
        }

        if (side == handEnum.left)
        {
            playerData.leftClimb = true;
        }
        else
        {
            playerData.rightClimb = true;
        }
        _isGrabbing = true;
    }

    void ReleaseObject()
    {
        _isGrabbing = false;

        if (side == handEnum.left)
        {
            playerData.leftClimb = false;
        }
        else
        {
            playerData.rightClimb = false;
        }

        if (_fixedJoint)
        {
            Destroy(_fixedJoint);
        }
    }
}

public enum handEnum
{
    left,
    right
}
