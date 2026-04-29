using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class HookLineRenderer : MonoBehaviour
{
    public List<Transform> joints;
    public LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer.positionCount = joints.Count;
    }

    void LateUpdate()
    {
        for(int i = 0; i < joints.Count; ++i)
        {
            lineRenderer.SetPosition(i, joints[i].position);
        }
    }
}
