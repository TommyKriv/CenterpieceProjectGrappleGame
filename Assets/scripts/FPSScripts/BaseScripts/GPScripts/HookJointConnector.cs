using System.Collections.Generic;
using UnityEngine;

public class HookJointConnector : MonoBehaviour
{
    public List<Joint> joints;
    public Rigidbody rootRigidbody;

    public void Hit()
    {
        foreach(var joint in joints)
        {
            joint.connectedBody = rootRigidbody;
        }
    }
}
