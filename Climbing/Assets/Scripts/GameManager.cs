using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Arm leftArm;
    [SerializeField] private Arm rightArm;
    [SerializeField] private Rigidbody leftTarget;
    [SerializeField] private Rigidbody rightTarget;
    [SerializeField] private new Camera camera;
    [SerializeField] private GameObject holdsParent;
    [SerializeField] private Transform marker;
    [SerializeField] private Airplane airplane;
    [SerializeField] private Rigidbody body;
    [SerializeField] private int goalY;
    [SerializeField] private TMPro.TMP_Text heightText;

    private Collider[] _holds;
    private float _cameraX, _cameraZ, _cameraY;
    private bool _isLastLevel;

    // Start is called before the first frame update
    void Start()
    {
        _holds = holdsParent.GetComponentsInChildren<Collider>();
        _CacheCameraPosition();

        _isLastLevel = !ReferenceEquals(airplane, null);
    }

    private void _CacheCameraPosition()
    {
        var cameraPos = camera.transform.position;
        _cameraX = cameraPos.x;
        _cameraY = cameraPos.y;
        _cameraZ = cameraPos.z;
    }

    // Update is called once per frame
    void Update()
    {
        _ArmMovement();
        _CameraMovement();
        _CheckHeightGoal();
    }

    private void _CheckHeightGoal()
    {
        if (goalY != 0)
        {
            var currentY = body.transform.position.y;
            heightText.text = "Height: " + (int) currentY + "/" + goalY;
            if (goalY != 0 && currentY >= goalY)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void _ArmMovement()
    {
        var mousePosition = Input.mousePosition;
        mousePosition = camera.ScreenToWorldPoint(new Vector3(
            mousePosition.x,
            mousePosition.y,
            camera.transform.localPosition.z * -1)
        );

        if (!_isLastLevel || leftArm.state != Arm.ArmState.holding)
            _CheckArm(leftArm, rightArm, mousePosition, leftTarget, KeyCode.A);
        if (!_isLastLevel || rightArm.state != Arm.ArmState.holding)
            _CheckArm(rightArm, leftArm, mousePosition, rightTarget, KeyCode.D);
    }

    private void _CameraMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _cameraY += 0.1f;
            camera.transform.position = new Vector3(_cameraX, _cameraY, _cameraZ - _cameraY / 2);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _cameraY -= 0.1f;
            camera.transform.position = new Vector3(_cameraX, _cameraY, _cameraZ - _cameraY / 2);
        }
    }

    private void _CheckArm(Arm arm, Arm oppositeArm, Vector3 mousePosition, Rigidbody target, KeyCode trigger)
    {
        var oppositeHold = oppositeArm.ConnectedTransform;
        if (ReferenceEquals(oppositeHold, null))
        {
            target.position = _CalculateTargetPosition(oppositeArm.EndPosition, mousePosition, 2.3f);
        }
        else
        {
            target.position = _CalculateTargetPosition(oppositeHold.position, mousePosition, 3f);
        }
        
        if (Input.GetKey(trigger))
        {
            if (arm.state != Arm.ArmState.following)
            {
                arm.FollowMouse(target);
            }
            else
            {
                var hold = _FindHold(arm.EndPosition, _holds, .5f);
                if (!ReferenceEquals(hold, null))
                {
                    marker.gameObject.SetActive(true);
                    marker.SetParent(hold.transform);
                    marker.localPosition = Vector3.zero;
                }
                else
                {
                    marker.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (arm.state == Arm.ArmState.following)
            {
                marker.gameObject.SetActive(false);
                var hold = _FindHold(arm.EndPosition, _holds, .5f);
                if (hold)
                {
                    arm.Connect(hold);
                    if (_isLastLevel && oppositeArm.state == Arm.ArmState.holding)
                    {
                        airplane.StartEngine();
                        body.constraints = RigidbodyConstraints.None;
                        camera.GetComponent<CameraMovement>().ToFirstPerson();
                    }
                }
                else
                    arm.Disconnect();
            }
        }
    }

    private Rigidbody _FindHold(Vector3 handPosition, IEnumerable<Collider> holds, float maxDistance)
    {
        var shortestDistance = maxDistance;
        Collider closestCollider = null;
        foreach (var hold in holds)
        {
            var handProjection = new Vector3(handPosition.x, handPosition.y, hold.transform.position.z);
            var distance = Vector3.Distance(hold.ClosestPoint(handProjection), handProjection);
            if (distance < shortestDistance)
            {
                closestCollider = hold;
                shortestDistance = distance;
            }
        }
        Debug.Log(shortestDistance);

        return closestCollider?.GetComponent<Rigidbody>();
    }

    private static Vector3 _CalculateTargetPosition(Vector3 start, Vector3 end, float limit)
    {
        var start2 = new Vector2(start.x, start.y);
        var end2 = new Vector2(end.x, end.y);
        var direction = end2 - start2;
        if (direction.magnitude > limit)
        {
            direction = direction.normalized * limit;
        }

        return start + new Vector3(direction.x, direction.y, 0);
    }
}
