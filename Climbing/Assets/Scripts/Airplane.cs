using UnityEngine;

public class Airplane : MonoBehaviour
{
    private Rigidbody _rigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void StartEngine()
    {
        var reference = Vector3.right;
        var speed = new Vector3(10, 0, 10);
        _rigidbody.velocity = speed;
        transform.rotation = Quaternion.FromToRotation(speed, reference);
    }
}
