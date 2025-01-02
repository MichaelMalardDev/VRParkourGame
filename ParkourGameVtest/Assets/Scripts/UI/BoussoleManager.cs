using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoussoleManager : MonoBehaviour
{
    public GameObject boussole;
    private SOPerso _playerData;
    private PhysicRig _physicRig;
    private bool _righthand;

    // Start is called before the first frame update
    void Start()
    {
        _physicRig = GetComponent<PhysicRig>();
        _playerData = _physicRig.playerData;
        boussole.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(_playerData.inTutorial)
        {
            boussole.SetActive(false);
            return;
        }
        _righthand = _physicRig.rightHandRotated;

        Vector3 target = GenerationTerrain.instance.positionToFolow;

        Vector3 direction = target - boussole.transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        boussole.transform.rotation = rotation;

        if (_righthand)
        {
            boussole.SetActive(true);
        }
        else
        {
            boussole.SetActive(false);
        }


    }

}

