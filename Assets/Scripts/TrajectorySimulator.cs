using UnityEngine;

public class TrajectorySimulator : MonoBehaviour {

    // Reference to the LineRenderer we will use to display the simulated path
    public LineRenderer sightLine;

    // Reference to a Component that holds information about fire strength, location of cannon, etc.
    public PlayerFire playerFire;

    // Number of segments to calculate - more gives a smoother line
    public int segmentCount = 20;

    // Length scale for each segment
    public float segmentScale = 1;

    // gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
    private Collider _hitObject;
    public Collider hitObject { get { return _hitObject; } }

    public Vector3 HitVector { get { return _hitVector; } }

    private Vector3 _hitVector;

    void FixedUpdate()
    {
        simulatePath();
    }

    /// <summary>
    /// Simulate the path of a launched ball.
    /// Slight errors are inherent in the numerical method used.
    /// </summary>
    void simulatePath()
    {
        Vector3[] segments = new Vector3[segmentCount];

        // The first line point is wherever the player's cannon, etc is
        segments[0] = playerFire.transform.position;

        // Create the angle we're rotating our launch direction by
        Quaternion angle = Quaternion.AngleAxis(65, playerFire.transform.right);
        // Create our new laucnh direction with our rotation
        Vector3 fireDirection = angle * playerFire.transform.up;

        // The initial velocity
        Vector3 segVelocity = fireDirection * playerFire.fireStrength * Time.deltaTime;

        // reset our hit object
        _hitObject = null;

        for (int i = 1; i < segmentCount; i++)
        {
            if (_hitObject != null)
            {
                segments[i] = _hitVector;
                continue;
            }

            // Time it takes to traverse one segment of length segScale (careful if velocity is zero)
            float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;

            // Add velocity from gravity for this segment's timestep
            segVelocity = segVelocity + Physics.gravity * segTime;

            // Check to see if we're going to hit a physics object
            RaycastHit hit;
            if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale))
            {
                // remember who we hit
                _hitObject = hit.collider;
                // set next position to the position where we hit the physics object
                segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
                // correct ending velocity, since we didn't actually travel an entire segment
                segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;
                // flip the velocity to simulate a bounce
                segVelocity = Vector3.Reflect(segVelocity, hit.normal);

                /*
				 * Here you could check if the object hit by the Raycast had some property - was 
				 * sticky, would cause the ball to explode, or was another ball in the air for 
				 * instance. You could then end the simulation by setting all further points to 
				 * this last point and then breaking this for loop.
				 */
                _hitVector = segments[i];
            }
            // If our raycast hit no objects, then set the next position to the last one plus v*t
            else
            {
                segments[i] = segments[i - 1] + segVelocity * segTime;
            }
        }

        // At the end, apply our simulations to the LineRenderer

        // Set the colour of our path to the colour of the next ball
        Color startColor = playerFire.nextColor;
        Color endColor = startColor;
        startColor.a = 1;
        endColor.a = 0;
        sightLine.SetColors(startColor, endColor);

        sightLine.SetVertexCount(segmentCount);
        for (int i = 0; i < segmentCount; i++)
            sightLine.SetPosition(i, segments[i]);
    }
}
