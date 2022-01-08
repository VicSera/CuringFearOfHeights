using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private Rigidbody[] links;
    [SerializeField] private Rigidbody plane;
    
    // Start is called before the first frame update
    void Start()
    {
        links = GetComponentsInChildren<Rigidbody>();

        for (var i = 0; i < links.Length; ++i)
        {
            var currentLink = links[i];
            var nextLink = i + 1 < links.Length ? links[i + 1] : plane;

            var joint = currentLink.gameObject.AddComponent<ConfigurableJoint>();
            joint.anchor = new Vector3(0, 1.2f, 0);
            
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
        
            joint.connectedBody = nextLink;
        }
    }
}
