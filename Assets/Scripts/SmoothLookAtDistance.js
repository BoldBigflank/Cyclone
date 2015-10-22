var target : Transform;
var damping = 6.0;
var smooth = true;
// The distance in the x-z plane to the target
var distance = 10.0;

var bigR = 0.75F;
var r = 1.0F;
private var t : float;

@script AddComponentMenu("Camera-Control/Smooth Look At")

function LateUpdate () {
	if (target) {
		if (smooth)
		{
			// Look at and dampen the rotation
			var lookPosition = Vector3(target.position.x * .75, target.position.y * .75, target.position.z);
			var rotation = Quaternion.LookRotation(lookPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
		}
		else
		{
			// Just lookat
//		    transform.LookAt(target);
			transform.rotation = Quaternion(0,0,0,0);
		}
		t += Time.deltaTime;
		transform.position.z = target.position.z - distance;
		transform.position.x = (bigR - r) * Mathf.Cos(  t ) + r * Mathf.Cos( (bigR - r)/r * t );
		transform.position.y = (bigR - r) * Mathf.Sin(  t ) + r * Mathf.Sin( (bigR - r)/r * t );		

	}
}

function Start () {
	// Make the rigid body not change rotation
   	if (GetComponent.<Rigidbody>())
		GetComponent.<Rigidbody>().freezeRotation = true;
	t = 0.0;
}