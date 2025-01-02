using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordHandler : MonoBehaviour
{
    public GameObject sword;
    public BoxCollider swordTriggerZone;

    public InputActionProperty GrabInputSource;

    private bool _isGrabbing = false;
    public bool _grabbingSword = false;

    void Start()
    {
        sword.SetActive(false);
        swordTriggerZone = GetComponent<BoxCollider>();
    }

    void FixedUpdate()
    {
        _isGrabbing = GrabInputSource.action.ReadValue<float>() > 0.1f;

        if (_isGrabbing == false)
        {
            _grabbingSword = false;
        }

        if (_grabbingSword == true)
        {
            sword.SetActive(true);
        }
        else
        {
            sword.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Main") && _isGrabbing)
        {
            SOPerso playerData = other.gameObject.GetComponentInParent<GrabPhysics>().playerData;

            if (playerData != null)
            {
                if (playerData.rightClimb == false)
                {
                    _grabbingSword = true;
                }
            }
        }
    }


}
