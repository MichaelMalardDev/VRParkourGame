using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaTrail : MonoBehaviour
{
    public Transform bladeTip;
    public Transform bladeBase;
    public GameObject trailMesh;
    public int trailFrameLength;
    public VelocityEstimator velocityEstimator;
    public float requiredVelocity = 5f;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;
    private int _frameCount;
    private Vector3 _previousTipPosition;
    private Vector3 _previousBasePosition;

    private const int NUM_VERTICES = 12;

    // Start is called before the first frame update
    void Start()
    {
        trailMesh.transform.position = Vector3.zero;
        _mesh = new Mesh();
        trailMesh.GetComponent<MeshFilter>().mesh = _mesh;

        _vertices = new Vector3[trailFrameLength * NUM_VERTICES];
        _triangles = new int[_vertices.Length];

        _previousTipPosition = bladeTip.position;
        _previousBasePosition = bladeBase.position;
    }

    void DrawTail()
    {
        //Reset the frame count one we reach the frame length
        if (_frameCount == (trailFrameLength * NUM_VERTICES))
        {
            _frameCount = 0;
        }

        //Draw first triangle vertices for back and front
        _vertices[_frameCount] = bladeBase.position;
        _vertices[_frameCount + 1] = bladeTip.position;
        _vertices[_frameCount + 2] = _previousTipPosition;
        _vertices[_frameCount + 3] = bladeBase.position;
        _vertices[_frameCount + 4] = _previousTipPosition;
        _vertices[_frameCount + 5] = bladeTip.position;

        //Draw fill in triangle vertices
        _vertices[_frameCount + 6] = _previousTipPosition;
        _vertices[_frameCount + 7] = bladeBase.position;
        _vertices[_frameCount + 8] = _previousBasePosition;
        _vertices[_frameCount + 9] = _previousTipPosition;
        _vertices[_frameCount + 10] = _previousBasePosition;
        _vertices[_frameCount + 11] = bladeBase.position;

        //Set triangles
        _triangles[_frameCount] = _frameCount;
        _triangles[_frameCount + 1] = _frameCount + 1;
        _triangles[_frameCount + 2] = _frameCount + 2;
        _triangles[_frameCount + 3] = _frameCount + 3;
        _triangles[_frameCount + 4] = _frameCount + 4;
        _triangles[_frameCount + 5] = _frameCount + 5;
        _triangles[_frameCount + 6] = _frameCount + 6;
        _triangles[_frameCount + 7] = _frameCount + 7;
        _triangles[_frameCount + 8] = _frameCount + 8;
        _triangles[_frameCount + 9] = _frameCount + 9;
        _triangles[_frameCount + 10] = _frameCount + 10;
        _triangles[_frameCount + 11] = _frameCount + 11;

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        //Track the previous base and tip positions for the next frame
        _previousTipPosition = bladeTip.position;
        _previousBasePosition = bladeBase.position;
        _frameCount += NUM_VERTICES;
    }
    // Update is called once per frame
    void LateUpdate()
    {

        Vector3 velocity = velocityEstimator.GetVelocityEstimate();

        if (velocity.magnitude >= requiredVelocity)
        {
            DrawTail();
        }

    }


}
