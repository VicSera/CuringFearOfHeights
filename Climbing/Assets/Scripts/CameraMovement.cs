using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private bool _firstPerson;
    private float _cachedZ, _cachedX, _cachedY;
    
    public void ToFirstPerson()
    {
        _firstPerson = true;
        var position = transform.localPosition;
        _cachedZ = position.z;
        _cachedX = position.x;
        _cachedY = position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_firstPerson && _cachedZ < 1)
        {
            _cachedZ += 0.3f;
            transform.localPosition = new Vector3(_cachedX, _cachedY, _cachedZ);
        }
    }
}
