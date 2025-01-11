
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

using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	public bool alive;
	public float speedMultiplier;
	public Sprite deadSprite;

	[SerializeField]
	private float horizontalSpeed;

	[SerializeField]
	private float verticalSpeed;

	[SerializeField]
	private Vector2 mousePosition;

	[SerializeField]
	private GameObject nearbyCar;

	[SerializeField]
	private new GameObject camera;

	[SerializeField]
	private AudioSource audioSource;

	private Rigidbody rb;



	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		camera = GameObject.Find("Main Camera");
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!alive)
			return;

		if(mousePosition != null) {
			Vector2 direction = (mousePosition - (Vector2) transform.position).normalized;
			float angle = Vector2.SignedAngle(Vector2.up, direction);
			transform.eulerAngles = new Vector3(0,0,angle);
		}
		camera.transform.up = Vector3.up;
		horizontalSpeed = Input.GetAxis("Horizontal");
		verticalSpeed = Input.GetAxis("Vertical");
		mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

		// Entering into car
		if (Input.GetKey(KeyCode.E) && nearbyCar != null) {
			CarController cc = nearbyCar.GetComponent<CarController>();

			if (cc.isDriving)
				cc.ExitVehicleNPC();

			cc.hasPlayer = true;
			cc.isDriving = true;
			cc.al.enabled = true;
			cc.kmh_text = GameObject.Find("Kmh").GetComponent<TMP_Text>();

			if (!cc.WasRobbed) {
				GameObject.Find("GameData").GetComponent<GameData>().RobbedVehicles++;
				cc.WasRobbed = true;
			}

			GameObject camera = GameObject.Find("Main Camera");
			camera.transform.parent = nearbyCar.transform;
			camera.transform.up = nearbyCar.transform.up;
			camera.transform.localPosition = new Vector3(0, 5, -10);
			camera.transform.rotation = new Quaternion(0,0,0,0);

			Input.ResetInputAxes();

			try {
				GameObject spawner = GameObject.Find("Spawner");
				spawner.transform.parent = nearbyCar.transform;
				spawner.transform.position = nearbyCar.transform.position;
			} catch {}

			try {
				GameObject.Find("ChaserWall").GetComponent<ChaserWallBehaviour>().PlayerLocation = nearbyCar;
			} catch {}

			GameObject.Find("GameData").GetComponent<GameData>().Player = nearbyCar;

			Destroy(gameObject);
		}
	}

	private void Die()
	{
		alive = false;
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.sprite = deadSprite;
		sr.sortingOrder = -1;

		AudioClip ac = Tools.GetRandomDeathClip();
		audioSource.PlayOneShot(ac);

		Invoke("LoseGame", 3);

		Destroy(rb);
		Destroy(GetComponent<BoxCollider>());
	}
	void FixedUpdate()
	{
		if(alive)
			rb.velocity = new Vector2(horizontalSpeed * speedMultiplier, verticalSpeed * speedMultiplier);
	}

	void LoseGame()
	{
		GameObject.Find("GameData").GetComponent<GameData>().LoseGame();
		Destroy(this);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (Tools.SumVelocityCrash(collision.relativeVelocity) > 6) {
			Die();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "VehicleEntrance") {
			nearbyCar = collider.gameObject.transform.parent.gameObject;
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "VehicleEntrance") {
			nearbyCar = collider.gameObject.transform.parent.gameObject;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		try {
			if (collider.gameObject.transform.parent.gameObject == nearbyCar) {
				nearbyCar = null;
			}
		} catch {}
	}
}
