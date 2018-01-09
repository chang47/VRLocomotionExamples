using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    public float HeightOffset = 1; // adjustment for Gvr Editor Simulator
    public ScreenCoverRenderer ScreenCoverRenderer;

    private NavMeshAgent _navMeshAgent;
    private Camera _camera;
    private bool _startedMoving; // keeps track if we started moving so we can stop

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        _startedMoving = false;
    }

    void Update()
    {
        _camera.transform.Translate(new Vector3(0, HeightOffset, 0)); // Gvr Editor Simulator forces us to be at 0, 0, 0, we need to fix that adjustment
        
        if (Input.GetButton("Fire1"))
        {
            WalkTo();
        }

        if (_navMeshAgent.velocity.magnitude <= 0.1f && !ScreenCoverRenderer.IsAnimating() && _startedMoving)
        {
            _startedMoving = false;
            ScreenCoverRenderer.StartFadeOut();
        }
    }

    private void WalkTo()
    {
        // shoot a raycast from the center of our screen
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit; // output variable to get what we collided against
        if (Physics.Raycast(ray, out hit))
        {
            // If we hit something, set our nav mesh to go to it
            if (hit.transform != null)
            {
                // If we're already moving, we don't want to start another Fade In
                if (!_navMeshAgent.hasPath)
                {
                    ScreenCoverRenderer.StartFadeIn();
                    _startedMoving = true;
                }
                _navMeshAgent.SetDestination(hit.point);
            }
        }
    }
}