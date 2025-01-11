
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
using UnityEngine;

public class PedAI : MonoBehaviour
{
	public bool isAlive = true;

	public Vector3 walkTarget;
	public int mood = 0;
	public GameObject killTarget;
	public float speed;
	public Sprite deadSprite;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private AudioSource audioSource;


	// Start is called before the first frame update
	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	void FixedUpdate()
	{
		switch (mood)
		{
			case 0:
			if (walkTarget != new Vector3(0,0,-1)) {
				Walk();
				if (Vector3.Distance(transform.position, walkTarget) < 0.2) {
					walkTarget = new Vector3(0,0,-1);
				}
			}
			break;

			case 1:
			Watch();
			break;

			case 2:
			Chase();
			break;
		}

	}

	void OnTriggerEnter(Collider collider)
	{
		if (mood != 2) {
			if (collider.tag == "Crosswalk" && mood != 2) {
				PedSideCrosswalk psc = collider.gameObject.GetComponent<PedSideCrosswalk>();
				if (!collider.bounds.Contains(walkTarget)) {
					walkTarget = psc.oppositeSide.transform.position;
					if (psc.passable)
						mood = 0;
					else{
						mood = 1;
						killTarget = psc.oppositeSide.gameObject;
					}
				}
			}
		} else { // If he was chasing someone...
			if (collider.tag == "Player") {
				collider.gameObject.GetComponent<PlayerController>().alive = false;
			} else if (collider.tag == "VehicleEntrance" &&
				collider.gameObject.transform.parent.gameObject == killTarget) {
				CarController cc = collider.gameObject.transform.parent.gameObject.GetComponent<CarController>();
				if (cc.hasPlayer)
					cc.ExitVehicle(collider.gameObject);
				else
					cc.ExitVehicleNPC(collider.gameObject);
			}
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if (mood == 1 && collider.tag == "Crosswalk") {
			PedSideCrosswalk psc = collider.gameObject.GetComponent<PedSideCrosswalk>();
			if (psc.passable)
				mood = 0;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Crosswalk") {
			mood = 0;
		}
	}

	private void Walk()
	{
		Vector3 direction = (walkTarget - transform.position).normalized;
		float angle = Vector2.SignedAngle(Vector2.up, direction);
		transform.rotation = Quaternion.Euler(0, 0, angle);
		transform.position += transform.up * speed;
	}

	private void Chase()
	{
		walkTarget = killTarget.transform.position;
		Walk();
	}

	private void Watch()
	{
		try {
			Vector3 direction = (killTarget.transform.position - transform.position).normalized;
			float angle = Vector2.SignedAngle(Vector2.up, direction);
			transform.rotation = Quaternion.Euler(0, 0, angle);
		} catch {}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (Tools.SumVelocityCrash(collision.relativeVelocity) > 4){

			if (collision.collider.tag == "Player") {
				GameObject.Find("GameData").GetComponent<GameData>().KilledPeds++;
			}
			CarController cc = collision.collider.GetComponent<CarController>();
			if (cc != null) {
				if (cc.hasPlayer){
					GameObject.Find("GameData").GetComponent<GameData>().KilledPeds++;
				}
			}

			Die();
		}
	}

	public void Die()
	{
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.sprite = deadSprite;
		sr.sortingOrder = -1;
		isAlive = false;

		AudioClip ac = Tools.GetRandomDeathClip();
		audioSource.PlayOneShot(ac);
		Destroy(rb);
		Destroy(GetComponent<BoxCollider>());
		Destroy(this);
	}
}
