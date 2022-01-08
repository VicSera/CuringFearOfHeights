using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private float cachedX, cachedZ;
    // Start is called before the first frame update
    void Start()
    {
        var position = _player.position;
        cachedX = position.x;
        cachedZ = position.z - 10;
    }

    // Update is called once per frame
    void Update()
    {
        var y = _player.position.y;
        transform.position = new Vector3(cachedX, y, cachedZ - y / 2);
    }
}
