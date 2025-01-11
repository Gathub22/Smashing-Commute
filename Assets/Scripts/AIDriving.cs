
/*

	Copyright (C) 2025 Raúl Gutiérrez raulgbeltran23@proton.me

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <https://www.gnu.org/licenses/>.

*/

using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AIDriving : MonoBehaviour
{
	public bool alive;
	public GameObject nextWaypoint;

	private Rigidbody rb;

	private CarController cc;

	[SerializeField]
	private float steerStrength;

	[SerializeField]
	private float throttleStrength;

	[SerializeField]
	private float rayLength;

	[SerializeField]
	private Vector3 rayOrigin;

	[SerializeField]
	private Vector3 rayDirection;

	private String[] ignoredTags = {
		"Waypoint",
		"Crosswalk",
		"PlayerArea",
		"Spawner",
		"Checkpoint",
		"Wall"
	};

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		cc = GetComponent<CarController>();
		rayLength = cc.frontalDetectionLength;
		rayDirection = transform.up;
	}

	private bool ObjectOnTheWay()
	{
		foreach (GameObject d in cc.detectors)
		{
			rayOrigin = d.transform.position;
			rayDirection = transform.up;
			float length = (rayLength + cc.currentSpeed) * 0.5f;
			Debug.DrawRay(rayOrigin, rayDirection * length, Color.red);

			RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, length);
			for (int i = 0; i < hits.Length; i++)	{
				if (hits[i].collider.gameObject.tag == "Waypoint") {
					if (!hits[i].collider.gameObject.GetComponent<Waypoint>().passable)
						return true;
				}else	if (!ignoredTags.Contains(hits[i].collider.gameObject.tag) && !hits[i].collider.isTrigger)
					return true;
			}
		}
		return false;
	}

	public void ReactToCrash(float intensity, GameObject car)
	{
		if (intensity < 10)
			cc.ExitVehicleNPC(1, car);
		else if (intensity < 15)
			cc.ExitVehicleNPC(2, car);
		else
			Die();
	}

	private void Die()
	{
		alive = false;
		Destroy(this);
	}

	void FixedUpdate()
	{
		if (!ObjectOnTheWay()) {
			Vector2 direction = (nextWaypoint.transform.position - transform.position).normalized;
			steerStrength = Vector2.SignedAngle(transform.up, direction);
			steerStrength /= 90f;
			cc.Steer(steerStrength);

			if ((steerStrength <= 0.5f && steerStrength >= -0.5f && nextWaypoint.GetComponent<Waypoint>().topSpeed > cc.speedInKPH)
				|| cc.currentSpeed < 3){
				cc.Throttle();
				throttleStrength = 1;
			}
		} else {
			cc.Brake();
			throttleStrength = 0;
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Waypoint") {

			Waypoint w = collider.gameObject.GetComponent<Waypoint>();

			if(!w.passable) {
				cc.Brake();
			} else if (collider.gameObject == nextWaypoint || nextWaypoint == null) {
				if (w.nextWaypoint != null) {

					nextWaypoint = (w.rightWaypoint != null && UnityEngine.Random.value <= 0.5f) ? w.rightWaypoint : null;
					nextWaypoint = (w.leftWaypoint != null && UnityEngine.Random.value <= 0.5f) ? w.leftWaypoint : null;
					nextWaypoint = nextWaypoint == null ? w.nextWaypoint : null;

					if (nextWaypoint == null)
						Destroy(gameObject);

				} else {
					nextWaypoint = w.rightWaypoint != null ? w.rightWaypoint : null;
					nextWaypoint = w.leftWaypoint != null ? w.leftWaypoint : null;

					if (nextWaypoint == null)
						Destroy(gameObject);
				}
			}
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.tag == "Waypoint") {

			Waypoint w = collider.gameObject.GetComponent<Waypoint>();

			if(!w.passable) {
				cc.Brake();
			}
		}
	}
}
