using UnityEngine;

public class Arm : MonoBehaviour
{
    public enum ArmState
    {
        dangling,
        holding,
        following
    };
    
    private ConfigurableJoint _joint;
    private Rigidbody _rigidbody;
    private Transform _end;
    public Transform ConnectedTransform { get; private set; }
    public ArmState state { get; private set; } = ArmState.dangling;

    public Vector3 EndPosition => _end.position;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _end = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FollowMouse(Rigidbody mouse)
    {
        _Disconnect();
        
        _joint = gameObject.AddComponent<ConfigurableJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.anchor = new Vector3(0, -.5f, 0);
        _joint.connectedAnchor = new Vector3(0, 0, 0);

        _joint.xMotion = ConfigurableJointMotion.Locked;
        _joint.yMotion = ConfigurableJointMotion.Locked;
        _joint.zMotion = ConfigurableJointMotion.Locked;
        
        _joint.connectedBody = mouse;
        ConnectedTransform = mouse.transform;
        state = ArmState.following;
    }

    public void Connect(Rigidbody obj)
    {
        _Disconnect();
        
        _joint = gameObject.AddComponent<ConfigurableJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.anchor = new Vector3(0, -.5f, 0);
        _joint.connectedAnchor = new Vector3(0, 0, 0);

        _joint.xMotion = ConfigurableJointMotion.Locked;
        _joint.yMotion = ConfigurableJointMotion.Locked;
        _joint.zMotion = ConfigurableJointMotion.Locked;
        
        _joint.connectedBody = obj;
        ConnectedTransform = obj.transform;
        state = ArmState.holding;
    }

    private void _Disconnect()
    {
        if (!ReferenceEquals(_joint, null))
        {
            Destroy(_joint);
            _joint = null;
        }
    }
    
    public void Disconnect()
    {
        _Disconnect();

        state = ArmState.dangling;
        ConnectedTransform = null;
    }
}
