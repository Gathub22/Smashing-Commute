
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

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Spawner : MonoBehaviour
{
	public GameObject player;
	public int pedLimit;
	public int carLimit;
	public float spawnProbability;

	[SerializeField]
	private List<GameObject> waypoints;

	[SerializeField]
	private List<GameObject> pedwalks;

	[SerializeField]
	private List<GameObject> detectedCars;
	[SerializeField]
	private List<GameObject> detectedPeds;

	[SerializeField]
	private PlayerArea pa;

	private BoxCollider bc;

	private string[] ignoredTags = {
		"Waypoint",
		"Crosswalk",
		"Pedwalk",
		"PlayerArea",
		"Spawner"
	};

	private Object[] prefabsPeds;

	// Start is called before the first frame update
	void Start()
	{
		waypoints = new List<GameObject>();
		pedwalks = new List<GameObject>();
		detectedCars = new List<GameObject>();
		detectedPeds = new List<GameObject>();
		bc = GetComponent<BoxCollider>();
		pa = GameObject.Find("PlayerArea").GetComponent<PlayerArea>();
		prefabsPeds = Resources.LoadAll("Prefabs/Peds");
	}

	void FixedUpdate()
	{
		// if (spawnProbability >= Random.value) {
		// 	foreach (GameObject w in waypoints){
		// 		if (!pa.IsInArea(w) && bc.bounds.Contains(w.transform.position) && detectedCars.Count < carLimit && spawnProbability >= Random.value && ValidWaypointToUse(w.GetComponent<Waypoint>()))
		// 			SpawnCar(w.transform.position, w.GetComponent<Waypoint>());
		// 	}
		// }


		if (spawnProbability >= Random.value && waypoints.Count > 0 && detectedCars.Count < carLimit) {
			GameObject w = waypoints[Random.Range(0,waypoints.Count)];
			GameObject car = Tools.GetRandomVehicle();

			car = Instantiate(car);

			int tries = 0;
			do {
				Waypoint w_cc = w.GetComponent<Waypoint>();
				Quaternion orientation = GetOrientationByWaypoint(w_cc);

				if (
					!pa.IsInArea(w) &&
					bc.bounds.Contains(w.transform.position) &&
					ValidWaypointToUse(w.GetComponent<Waypoint>()) &&
					!ObjectOnSpawnPoint(w.transform.position, car.GetComponent<BoxCollider>().bounds.size / 2, orientation)
				) {
					ManageSpawnedCar(car, w.GetComponent<Waypoint>());
					tries = -1;
					break;
				}
				tries++;
			} while(tries < 2);

			if (tries != -1){
				Destroy(car);
			}
		}

		if (spawnProbability >= Random.value && pedwalks.Count > 0 && detectedPeds.Count < pedLimit) {
			GameObject p = pedwalks[Random.Range(0, pedwalks.Count)];
			int tries = 0;
			do {
				p = pedwalks[Random.Range(0, pedwalks.Count)];
			} while(!SpawnPed(p.GetComponent<BoxCollider>()) && ++tries < 5);
		}

		RemoveLostEntities();
	}

	private void ManageSpawnedCar(GameObject car, Waypoint w)
	{
		Quaternion orientation = GetOrientationByWaypoint(w);;

		car.transform.position = w.transform.position;
		car.transform.rotation = orientation;

		CarController car_cc = car.GetComponent<CarController>();
		car_cc.isDriving = true;

		bool speedAssigned = false;
		RaycastHit[] hits = Physics.RaycastAll(w.transform.position, car.transform.up, (car_cc.frontalDetectionLength + w.topSpeed) * 0.5f);
		foreach (RaycastHit h in hits) {
			CarController cc = h.collider.GetComponent<CarController>();
			if (cc != null) {
				car_cc.currentSpeed = cc.currentSpeed;
				speedAssigned = true;
				break;
			}
		}

		if (!speedAssigned)
			car_cc.currentSpeed = w.topSpeed;

		if (car_cc.CanBePainted)
			car.GetComponent<SpriteRenderer>().color = GetRandomColor();

		car.AddComponent<AIDriving>().nextWaypoint = w.nextWaypoint;
		detectedCars.Add(car);
	}

	private Quaternion GetOrientationByWaypoint(Waypoint w)
	{
		Vector3 direction = w.nextWaypoint.transform.position - w.transform.position;
		float angle = Vector3.SignedAngle(Vector3.up, direction, Vector3.forward);
		return Quaternion.Euler(0, 0, angle);
	}

	private bool ObjectOnSpawnPoint(Vector2 pos, Vector3 area, Quaternion orientation)
	{
		Collider[] colliders = Physics.OverlapBox(pos, area, orientation);

		foreach (Collider c in colliders)
		{
			if (c.tag == "Waypoint"){
				if (!c.gameObject.GetComponent<Waypoint>().passable)
					return true;
			} else if (!ignoredTags.Contains(c.tag))
				return true;
		}
		return false;
	}

	private bool SpawnPed(Collider pw)
	{
		Vector3 spawnPoint = GetRandomPointInBounds(pw.bounds);
		UnityEngine.Object prefab = prefabsPeds[(int) Random.Range(0,prefabsPeds.Length)];

		// If there is no object in spawn position and is not in Player area...
		if (Physics.OverlapBox(spawnPoint, new Vector3(1,1,1)).Length <=2 &&
			!pa.IsInArea(spawnPoint) &&
			bc.bounds.Contains(spawnPoint)
			) {

			GameObject ped = (GameObject) Instantiate(
				prefab,
				spawnPoint,
				Quaternion.identity
			);

			ped.GetComponent<PedAI>().walkTarget = pw.gameObject.GetComponent<PedWalk>().GetRandomWalkPath();
			detectedPeds.Add(ped);
			return true;
		}
		return false;
	}

	private Vector3 GetRandomPointInBounds(Bounds bounds)
	{
		float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
		float y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);

		return new Vector3(x, y, 0);
	}

	private Color GetRandomColor()
	{
		float r = Random.Range(0f, 1f);
		float g = Random.Range(0f, 1f);
		float b = Random.Range(0f, 1f);
		return new Color(r, g, b);
	}

	private void RemoveLostEntities()
	{
		ArrayList lostCars = new ArrayList();
		ArrayList lostPeds = new ArrayList();

		foreach (GameObject c in detectedCars) {
			if (c == null)
				lostCars.Add(c);
			else {
				if (!bc.bounds.Contains(c.transform.position))
					lostCars.Add(c);
			}
			// try {
			// 	if (!bc.bounds.Contains(c.transform.position))
			// 		lostCars.Add(c);
			// } catch {}
		}

		foreach (GameObject c in lostCars) {
			Destroy(c);
			detectedCars.Remove(c);
		}

		foreach (GameObject p in detectedPeds) {

			if(p == null) {
				lostPeds.Add(p);
			} else {
				if (!bc.bounds.Contains(p.transform.position))
					lostPeds.Add(p);
			}
			// try {
			// 	if (!bc.bounds.Contains(p.transform.position))
			// 		lostPeds.Add(p);
			// } catch {}
		}

		foreach (GameObject p in lostPeds) {
			Destroy(p);
			detectedPeds.Remove(p);
		}
	}

	private bool ValidWaypointToUse(Waypoint w)
	{
		return w.asSpawnPoint && w.passable && (w.nextWaypoint != null || w.rightWaypoint != null || w.leftWaypoint != null );
	}
	void OnTriggerEnter(Collider collider)
	{

		if (!pa.IsInArea(collider.gameObject)) {
			Waypoint w = collider.gameObject.GetComponent<Waypoint>();
			Vector2 pos = collider.gameObject.transform.position;

			if (w != null && w.nextWaypoint != null) {
				waypoints.Add(collider.gameObject);
			} else {
				PedWalk pw = collider.gameObject.GetComponent<PedWalk>();
				if (pw != null) {
					pedwalks.Add(collider.gameObject);
				}
			}
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "Waypoint") {
			// In case the waypoint is too close to the player...
			if (pa.IsInArea(collider.gameObject)) {
				waypoints.Remove(collider.gameObject);
			}
		} else if (collider.gameObject.tag == "Pedwalk") {
			if (detectedPeds.Count < pedLimit && spawnProbability >= Random.value){
				SpawnPed(collider);
			}

		}
	}

	void OnTriggerExit(Collider collider)
	{
		GameObject go = collider.gameObject;
		if (collider.tag == "Vehicle"){
			detectedCars.Remove(go);
			Destroy(go);
		}else if (go.tag == "Waypoint")
			waypoints.Remove(go);
		else if (go.tag == "PedWalk")
			pedwalks.Remove(go);
		else if (go.tag == "Ped") {
			detectedPeds.Remove(go);
			Destroy(go);
		}
	}
}
