using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingGun : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private FPSController _movement;
    [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Transform player;
    [SerializeField] private Transform _camera;

    private Vector3 grapplePoint;
    //private Transform _camera;


    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float overshootYAxis;

    [Header("Cooldown")]
    [SerializeField] private float grapplingCoolDown;
    private float grapplingCoolDownTimer;

    [Header("Input")]
    [SerializeField] private KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        //_camera = player.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grapplingCoolDownTimer > 0)
            grapplingCoolDownTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling)
            _lineRenderer.SetPosition(0, _muzzle.position);
    }

    private void StartGrapple()
    {
        if (grapplingCoolDownTimer > 0) return;

        grappling = true;

        

        RaycastHit hit;
        if (Physics.Raycast(_camera.position, _camera.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            _movement.freeze = true;
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = _camera.position + _camera.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        _movement.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        _movement.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        _movement.freeze = false;

        grappling = false;

        grapplingCoolDownTimer = grapplingCoolDown;

        _lineRenderer.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

}
