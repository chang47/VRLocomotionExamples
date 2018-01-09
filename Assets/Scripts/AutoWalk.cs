using UnityEngine;
using UnityEngine.AI;

public class AutoWalk : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Camera _camera;
    private bool _isWalking;

	void Start ()
	{
	    _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
	    _isWalking = false;
	}
	
	void Update ()
	{
	    _camera.transform.Translate(new Vector3(0, 1, 0)); // Gvr Editor Simulator forces us to be at 0, 0, 0, we need to fix that adjustment

        // switch the state we are in whenever we click
	    if (Input.GetButtonDown("Fire1"))
	    {
	        _isWalking = !_isWalking;
	    }

        // if we are walking, we want to set a direction for our Nav Mesh Agent
	    if (_isWalking)
	    {
            // set the direction to be our current location + whereever our camera is facing
	        _navMeshAgent.SetDestination(transform.position + _camera.transform.forward);
        }
	}
}
