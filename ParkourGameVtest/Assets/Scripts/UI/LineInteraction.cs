using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LineInteraction : MonoBehaviour
{
    public LineRenderer lr;
    public Transform preditionPoint;
    public Transform startLinePoint;
    public LayerMask uiLayer;
    

    [Space]
    [Header("Haptics")]
    public ActionBasedController xrController;
    [Range(0, 1)]
    public float amplitude = .1f;
    public float durationInSeconds = .3f;

    [Space]
    [Header("InputAction")]
    public InputActionReference inputActionReference;

    private bool _hasHit;
    private bool _action;
    private Collider _hitCollider;
    // Start is called before the first frame update
    void Start()
    {
        lr.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // if(inputActionReference.action.WasPerformedThisFrame())
        // {
        //     Debug.Log("ClickLine");
        // }

        _action = inputActionReference.action.WasPerformedThisFrame();

        if(_hasHit && _action)
        {
            Debug.Log("ClickLine");
            _hitCollider.GetComponent<Bouton>().OnClick();
        }

        GetLinePoint();
    }

    void TriggerHaptics()
    {
        xrController.SendHapticImpulse(amplitude, durationInSeconds);
    }



    void GetLinePoint()
    {
        RaycastHit hit;
        _hasHit = Physics.Raycast(startLinePoint.position, startLinePoint.forward, out hit, 10, uiLayer);
        if (!_hasHit)
        {
            lr.enabled = false;
            preditionPoint.gameObject.SetActive(false);
            if (_hitCollider != null)
            {
                _hitCollider.GetComponent<Animator>().SetBool("IsOn", false);
                _hitCollider = null;
            }
            return;
        }
        else
        {
            lr.enabled = true;
            preditionPoint.gameObject.SetActive(true);
            preditionPoint.position = hit.point;
            lr.SetPosition(0, startLinePoint.position);
            lr.SetPosition(1, hit.point);
            bool wasNull = _hitCollider == null;
            if (!wasNull && _hitCollider != hit.collider)
            {
                _hitCollider.GetComponent<Animator>().SetBool("IsOn", false);
            }
            _hitCollider = hit.collider;
            if (wasNull)
            {
                _hitCollider.GetComponent<Animator>().SetBool("IsOn", true);
                TriggerHaptics();
            }
        }
    }
}
