using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Swing swing;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve effectCurve;

    private Spring _spring;

    // Start is called before the first frame update
    void Start()
    {
        _spring = new Spring();
        _spring.SetTarget(0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        DrawRope();
    }

    public void DrawRope()
    {
        if (!swing.joint)
        {
            _spring.Reset();
            if (lineRenderer.positionCount > 0) lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
            if (lineRenderer.positionCount == 0)
            {
                _spring.SetVelocity(velocity);
                lineRenderer.positionCount = quality + 1;
            }

            _spring.SetDamper(damper);
            _spring.SetStrength(strength);
            _spring.Update(Time.deltaTime);

            Vector3 swingPoint = swing.swingPoint;
            Vector3 startSwingPoint = swing.startSwingPoint.position;

            Vector3 up = Quaternion.LookRotation(swingPoint - startSwingPoint) * Vector3.up;



            for (int i = 0; i < quality + 1; i++)
            {
                float delta = i / (float)quality;
                Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * _spring.Value *
                         effectCurve.Evaluate(delta);

                lineRenderer.SetPosition(i, Vector3.Lerp(startSwingPoint, swingPoint, delta) + offset);
            }
            // lineRenderer.positionCount = 2;
            // lineRenderer.SetPosition(0, swing.startSwingPoint.position);
            // lineRenderer.SetPosition(1, swing.swingPoint);
        }
    }
}
