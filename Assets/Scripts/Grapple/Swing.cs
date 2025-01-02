using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// TOTEST: SwingGrapple
public class Swing : MonoBehaviour
{
    public SOPerso playerData;
    public Transform startSwingPoint;
    public float maxDistance = 35f;
    public LayerMask swingLayer;
    public Transform preditionPoint;

    // public InputActionProperty[] swingAction;
    public InputActionProperty swingAction;
    public InputActionProperty pullAction;

    public float pullForce = 500f;
    public float pullRate = 0.02f;

    // public Rigidbody playerRigidbody;
    public Rigidbody rightHandRigidbody;

    [Header("SpringJoint Settings")]
    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    [Space]
    [Header("Haptics")]
    public ActionBasedController xrController;
    [Range(0, 1)]
    public float amplitude = .1f;
    public float durationInSeconds = .3f;


    // public LineRenderer lineRenderer;

    private SpringJoint _joint;
    public SpringJoint joint { get => _joint; set => _joint = value; }
    private Vector3 _swingPoint;
    public Vector3 swingPoint { get => _swingPoint; set => _swingPoint = value; }

    private bool _hasHit;
    // Start is called before the first frame update
    void Start()
    {
        pullForce *= 1000;
        preditionPoint.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerData.newGame) return;
        GetSwingPoint();
        // if (swingAction[0].action.WasPerformedThisFrame() && swingAction[1].action.WasPerformedThisFrame())
        if (swingAction.action.WasPerformedThisFrame())
        {
            StartSwing();
        }
        // else if (swingAction[0].action.WasReleasedThisFrame() && swingAction[1].action.WasReleasedThisFrame())
        else if (swingAction.action.WasReleasedThisFrame())
        {
            StopSwing();
        }
        PullRope();

    }



    public void PullRope()
    {
        if (!joint) return;

        if (pullAction.action.IsPressed())
        {

            Vector3 direction = (_swingPoint - startSwingPoint.position).normalized;
            rightHandRigidbody.AddForce(direction * pullForce * Time.deltaTime);

            float distance = Vector3.Distance(rightHandRigidbody.position, _swingPoint);
            joint.maxDistance = distance;

        }
    }

    // IEnumerator PullRope()
    // {
    //     if (!_joint) yield break;

    //     while (true)
    //     {
    //         Vector3 pullDirection = (_swingPoint - startSwingPoint.position).normalized;
    //         playerRigidbody.AddForce(pullDirection * pullForce * Time.deltaTime);

    //         float distance = Vector3.Distance(playerRigidbody.position, _swingPoint);
    //         _joint.maxDistance = distance;

    //         yield return new WaitForSeconds(pullRate);
    //     }
    // }

    public void StartSwing()
    {
        if (_hasHit)
        {
            joint = rightHandRigidbody.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = _swingPoint;

            float distance = Vector3.Distance(rightHandRigidbody.position, _swingPoint);
            joint.maxDistance = distance;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            // StartCoroutine(PullRope());
        }
    }

    public void StopSwing()
    {
        Destroy(joint);
        // StopCoroutine(PullRope());
    }
    void TriggerHaptics()
    {
        xrController.SendHapticImpulse(amplitude, durationInSeconds);
    }

    public void GetSwingPoint()
    {
        if (joint)
        {
            preditionPoint.gameObject.SetActive(false);
            return;  //Si le player est deja en train de swing, on ne fait rien
        }

        RaycastHit hit;

        _hasHit = Physics.Raycast(startSwingPoint.position, startSwingPoint.forward, out hit, maxDistance, swingLayer);

        if (_hasHit)
        {
            TriggerHaptics();
            _swingPoint = hit.transform.position;
            Vector3 predictionDirection = hit.point;
            preditionPoint.gameObject.SetActive(true);
            preditionPoint.position = predictionDirection;
        }
        else
        {
            preditionPoint.gameObject.SetActive(false);
        }
    }


}
