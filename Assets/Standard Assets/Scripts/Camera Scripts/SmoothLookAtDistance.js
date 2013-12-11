var target : Transform;
var damping = 6.0;
var smooth = true;
// The distance in the x-z plane to the target
var distance = 10.0;
var radius = .75;
var period = 8;

@script AddComponentMenu("Camera-Control/Smooth Look At")

function LateUpdate () {
	if (target) {
		if (smooth)
		{
			// Look at and dampen the rotation
			var rotation = Quaternion.LookRotation(target.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
		}
		else
		{
			// Just lookat
//		    transform.LookAt(target);
			transform.rotation = Quaternion(0,0,0,0);
		}
		
		transform.position.z = target.position.z - distance;
		transform.position.x = radius * Mathf.Sin( transform.position.z /( Mathf.PI * period) );
		transform.position.y = radius * Mathf.Cos( transform.position.z /( Mathf.PI * period) );
		

	}
}

function Start () {
	// Make the rigid body not change rotation
   	if (rigidbody)
		rigidbody.freezeRotation = true;
}