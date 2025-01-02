using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicRig : MonoBehaviour
{
    public SOPerso playerData;
    public Transform playerHead;

    public Transform leftController;
    public Transform rightController;

    public ConfigurableJoint headJoint;
    public ConfigurableJoint leftHandJoint;
    public ConfigurableJoint rightHandJoint;
    public CapsuleCollider bodyCollider;

    public LayerMask groundLayer; // Layer du sol
    public float lerpSpeed = 0.000001f; // Vitesse de transition
    public float maxDistance = 0.1f; // Distance maximale pour le raycast

    public bool rightHandRotated = false;
    
    void FixedUpdate()
    {

        // Debug.Log("leftController rotation : " + leftController.localRotation.eulerAngles);
        // Debug.Log("rightController rotation : " + rightController.localRotation.eulerAngles);
        // Debug.Log("FixedUpdate :"+ bodyCollider.height);
        // Effectue un raycast vers le bas depuis la tête pour détecter le sol
        RaycastHit hit;
        bool isGrounded = Physics.Raycast(playerHead.position, Vector3.down, out hit, maxDistance, groundLayer);

        // Dessine le raycast dans l'éditeur Unity
        Debug.DrawRay(playerHead.position, Vector3.down * maxDistance, isGrounded ? Color.green : Color.red);

        // Si le personnage touche le sol, laisse les jambes dans leur position normale
        if (isGrounded)
        {
            float targetHeight = Mathf.Clamp(playerHead.localPosition.y, playerData.bodyHeightMin, playerData.bodyHeightMax);
            float newHeight = Mathf.Lerp(bodyCollider.height, targetHeight, lerpSpeed * Time.fixedDeltaTime);
            float newCenter = Mathf.Lerp(bodyCollider.center.y, targetHeight / 2, lerpSpeed * Time.fixedDeltaTime);
            bodyCollider.height = newHeight;
            bodyCollider.center = new Vector3(playerHead.localPosition.x, newCenter, playerHead.localPosition.z);

        }
        else
        {
            // Si le personnage ne touche pas le sol, lève les jambes
            bodyCollider.height = playerData.bodyHeightMin; // Réduit la hauteur du collider pour lever les jambes
            bodyCollider.center = new Vector3(playerHead.localPosition.x, bodyCollider.height * 3, playerHead.localPosition.z); // Centre le collider à mi-hauteur
        }

        leftHandJoint.targetPosition = leftController.localPosition;
        leftHandJoint.targetRotation = leftController.localRotation;

        rightHandJoint.targetPosition = rightController.localPosition;
        rightHandJoint.targetRotation = rightController.localRotation;

        headJoint.targetPosition = playerHead.localPosition;



        if (!playerData.isDead)
        {
            // Définir les valeurs communes
            float springValue = playerData.leftClimb || playerData.rightClimb ? 1000f : 5000f;

            // Modifier les joints en fonction de leftClimb
            leftHandJoint.xDrive = new JointDrive { positionSpring = springValue, positionDamper = leftHandJoint.xDrive.positionDamper, maximumForce = leftHandJoint.xDrive.maximumForce };
            leftHandJoint.yDrive = new JointDrive { positionSpring = springValue, positionDamper = leftHandJoint.yDrive.positionDamper, maximumForce = leftHandJoint.yDrive.maximumForce };
            leftHandJoint.zDrive = new JointDrive { positionSpring = springValue, positionDamper = leftHandJoint.zDrive.positionDamper, maximumForce = leftHandJoint.zDrive.maximumForce };

            // Modifier les joints en fonction de rightClimb
            rightHandJoint.xDrive = new JointDrive { positionSpring = springValue, positionDamper = rightHandJoint.xDrive.positionDamper, maximumForce = rightHandJoint.xDrive.maximumForce };
            rightHandJoint.yDrive = new JointDrive { positionSpring = springValue, positionDamper = rightHandJoint.yDrive.positionDamper, maximumForce = rightHandJoint.yDrive.maximumForce };
            rightHandJoint.zDrive = new JointDrive { positionSpring = springValue, positionDamper = rightHandJoint.zDrive.positionDamper, maximumForce = rightHandJoint.zDrive.maximumForce };
        }
        else
        {
            Invoke(nameof(DesactivateHands), 1f);
        }


    }

    void Update()
    {
        rightHandRotated = CheckIfFaceUp(leftController);
    }
 
    bool CheckIfFaceUp(Transform hand)
    {
        Vector3 handUpDirection = hand.TransformDirection(Vector3.left);
        // Debug.Log("handUpDirection : " + handUpDirection);
        return handUpDirection.y < -.9f;
    }

    void DesactivateHands()
    {
        leftHandJoint.xDrive = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };
        leftHandJoint.yDrive = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };
        leftHandJoint.zDrive = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };

        rightHandJoint.xDrive = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };
        rightHandJoint.yDrive = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };
        rightHandJoint.zDrive = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };
    }
}