
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

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CarController : MonoBehaviour
{

	public bool hasPlayer;
	public bool isDriving;
	public bool WasRobbed = false;
	public bool CanBePainted;
	public float acceleration;
	public float maximumSpeed;
	public float speedInKPH;
	public int health;
	public int gears;
	public float value;

	public float handlingPower;
	public float brakingPower;
	public float drag;

	public float crashVelocityThreshold;

	public float frontalDetectionLength;

	public CarPart[] carParts;
	public GameObject[] BrakingLights;
	public GameObject leftEntrance;
	public GameObject rightEntrance;
	public GameObject SmokePoint;
	public List<GameObject> detectors;

	public float currentSpeed;

	[SerializeField]
	private int currentGear;

	[SerializeField]
	private float playingThrottleProgress;

	[SerializeField]
	private bool WasSlowingDown;

	[SerializeField]
	private float gearLimit;

	[SerializeField]
	private Quaternion currentRotation;

	[SerializeField]
	private int currentContacts;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private BoxCollider bc;

	public TMP_Text kmh_text;

	public AudioListener al;
	public AudioSource Idling;
	public AudioSource Throttling;
	public AudioSource ReverseThrottling;
	public AudioSource Horn;



	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		bc = GetComponent<BoxCollider>();
		al = GetComponent<AudioListener>();
		currentGear = 1;
		gearLimit = maximumSpeed / gears;
		currentContacts = 0;
		playingThrottleProgress = 0;
		WasSlowingDown = true;

		if(hasPlayer) {
			GameObject.Find("GameData").GetComponent<GameData>().Player = gameObject;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (hasPlayer) {
			if (Input.GetKey(KeyCode.F) && !Horn.isPlaying) {
				Horn.Play();
			}else if(!Input.GetKey(KeyCode.F) && Horn.isPlaying) {
				Horn.Stop();
			}

			if (Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.F)) {
				ExitVehicle();
				return;
			}
		}
	}

	public void ExitVehicle()
	{
		hasPlayer = false;
		isDriving = false;
		Vector3 pos = GetExitPosition();

		Object p = Resources.Load("Player/Player");
		GameObject player = (GameObject) Instantiate(
			p,
			pos,
			Quaternion.identity
		);
		player.transform.up = Vector3.up;

		Camera c = Camera.main;
		c.transform.parent = player.transform;
		c.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);

		try{
			GameObject.Find("Spawner").transform.parent = player.transform;
		} catch {}

		al.enabled = false;

		GameObject.Find("GameData").GetComponent<GameData>().Player = player;

		Input.ResetInputAxes();
	}

	public void ExitVehicle(GameObject entrance)
	{
		hasPlayer = false;
		isDriving = false;
		al.enabled = false;
		Vector3 pos = entrance.transform.position;
		Object p = Resources.Load("Player/Player");
		GameObject player = (GameObject) Instantiate(
			p,
			pos,
			Quaternion.identity
		);
		player.transform.up = Vector3.up;

		Camera c = Camera.main;
		c.transform.parent = player.transform;
		c.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
		GameObject.Find("Spawner").transform.parent = player.transform;

		GameObject.Find("ChaserWall").GetComponent<ChaserWallBehaviour>().PlayerLocation = player;


		Input.ResetInputAxes();
	}

	public void ExitVehicleNPC(int mood, GameObject car)
	{
		isDriving = false;
		Vector3 pos = GetExitPosition();

		Object p = Tools.GetRandomPed();

		GameObject ped = (GameObject) Instantiate(
			p,
			pos,
			Quaternion.identity
		);

		ped.transform.up = Vector3.up;
		PedAI ai = ped.GetComponent<PedAI>();
		ai.killTarget = car;
		ai.mood = mood;

		isDriving = false;
		Destroy(GetComponent<AIDriving>());
	}

	public void ExitVehicleNPC(GameObject entrance)
	{
		isDriving = false;
		Vector3 pos = entrance.transform.position;

		Object p = Tools.GetRandomPed();
		GameObject ped = (GameObject) Instantiate(
			p,
			pos,
			Quaternion.identity
		);

		ped.transform.up = Vector3.up;
		PedAI ai = ped.GetComponent<PedAI>();
		ai.mood = 1;

		Destroy(GetComponent<AIDriving>());
	}

	public void ExitVehicleNPC()
	{
		isDriving = false;
		Vector3 pos = GetExitPosition();

		Object p = Tools.GetRandomPed();
		GameObject ped = (GameObject) Instantiate(
			p,
			pos,
			Quaternion.identity
		);

		ped.transform.up = Vector3.up;
		PedAI ai = ped.GetComponent<PedAI>();
		ai.mood = 1;
		ai.walkTarget = transform.position;

		if (GetComponent<AIDriving>() == null)
			ai.Die();
		else
			Destroy(GetComponent<AIDriving>());
	}

	public void Steer(float angle)
	{
		if ((speedInKPH > 3 || speedInKPH < -3) && health > 0){
			rb.MoveRotation( Quaternion.Euler(0,0, transform.rotation.eulerAngles.z + angle * handlingPower * Time.fixedDeltaTime));
		}
	}

	public void Throttle()
	{
		if( speedInKPH < maximumSpeed ){
			currentSpeed += acceleration;
			RevUp();
		} else {
			RevDown();
		}
	}

	public void Throttle(float intensity)
	{
		if (intensity > 0)
			RevUp();

		if( speedInKPH < maximumSpeed ){
			currentSpeed += acceleration * intensity;
		} else {
			RevDown();
		}
	}

	public void Brake() {
		currentSpeed -= brakingPower;

		TurnOnBrakingLights();
		RevDown();
	}

	private void TurnOnBrakingLights()
	{
		foreach (GameObject l in BrakingLights)
		{
			CarPart cp = l.GetComponent<CarPart>();
			if (cp.currentDamage < cp.Health) {
				l.GetComponent<Light2D>().intensity = 1;
			}
		}
	}

	private void UpdateSpeed()
	{
		if (currentSpeed > 0)
			currentSpeed -= drag;
		else if (currentSpeed + drag < 0)
			currentSpeed += drag;

		if (currentContacts == 0) {
			rb.velocity = transform.up * currentSpeed;
		} else {
			rb.AddForce(transform.up * currentSpeed, ForceMode.Acceleration);
		}

		speedInKPH = currentSpeed * 3.6f;
		if (hasPlayer)
			DisplaySpeed();
	}

	private void CheckGear()
	{
		if (hasPlayer) {
			if(speedInKPH >= gearLimit*currentGear && IsMoving() > 0f && currentGear < gears) {
				currentGear++;
				RestoreThrottlingAudio();
				WasSlowingDown = true;
			} else if (speedInKPH < gearLimit*currentGear && currentSpeed >= 0 && IsMoving() <= 0f && currentGear > 1){
				currentGear--;
				RestoreThrottlingAudio();
				WasSlowingDown = false;
			}
		}
	}

	private void DisplaySpeed()
	{
		if (speedInKPH >= 0){
			kmh_text.text = ((int) speedInKPH) + " kmh";
		} else {
			kmh_text.text = ((int) -speedInKPH) + " kmh";
		}
	}

	private void RestoreThrottlingAudio()
	{
		Throttling.Stop();
		ReverseThrottling.Stop();
		playingThrottleProgress = 0; // TODO: No te mantieness en 0. Sad
	}

	private float IsMoving()
	{
		if (hasPlayer) {
			return Input.GetAxis("Vertical");
		}

		return 0;
	}

	private float IsTurning()
	{
		if (hasPlayer) {
			return Input.GetAxis("Horizontal");
		}

		return 0;
	}

	private Vector3 GetExitPosition()
	{
		Vector3 pos = transform.up + transform.position + new Vector3(0,3,0);

		if (!leftEntrance.GetComponent<CarEntrance>().IsBlocked())
			pos = leftEntrance.transform.position;
		else if (!rightEntrance.GetComponent<CarEntrance>().IsBlocked())
			pos = rightEntrance.transform.position;
		return pos;
	}

	// private int GetCrashIntensity(Vector3 force)
	// {
	// 	float impactX = Mathf.Abs(force.x) / impactSensitivityThreshold;
	// 	float impactY = Mathf.Abs(force.y) / impactSensitivityThreshold;
	// 	float impactZ = Mathf.Abs(force.z) / impactSensitivityThreshold;

	// 	int impactRating = Mathf.RoundToInt(impactX + impactY + impactZ);
	// 	return impactRating;
	// }

	void FixedUpdate()
	{
		float impulse = IsMoving();
		float steer = IsTurning();

		if (health > 0) {
			if(hasPlayer) {
				if (impulse >= 0f && speedInKPH >= 0) {

					if (impulse > 0f) {
						Throttle(impulse);
					} else {
						TurnOffBrakingLights();
						if (speedInKPH > 0)
							RevDown();
					}
					Steer(-steer);
				} else {
					if (speedInKPH > 0) {
						Brake();
					} else if (speedInKPH > -10) {
						Throttle(impulse);
						Steer(steer);
					}
				}
			}
		}else{
			AIDriving aid = GetComponent<AIDriving>();
			if (aid != null) {
				ExitVehicleNPC(1, gameObject);
			}

			if (impulse < 0f && speedInKPH > 0) {
				Brake();
			} else {
				TurnOffBrakingLights();
			}
			Steer(steer);
		}

		if (health > 0){
			UpdateSpeed();
			CheckGear();
		}
	}

	void RevUp()
	{
		if(!Throttling.isPlaying){
			ReverseThrottling.Stop();
			Throttling.Play();

			//playingThrottleProgress = ReverseThrottling.time;
			//Throttling.time = playingThrottleProgress;
			Throttling.pitch = Idling.pitch + currentGear + 0.5f;
			WasSlowingDown = false;
		}
	}

	void RevDown()
	{
		if(!ReverseThrottling.isPlaying) {
			ReverseThrottling.Play();

			// playingThrottleProgress = Throttling.clip.length - Throttling.time;
			Throttling.Stop();

			//ReverseThrottling.time = playingThrottleProgress;
			Throttling.pitch = Idling.pitch + currentGear;
			WasSlowingDown = true;
		}
	}

	private float getThrottlingPitch()
	{
		float ms = (1/Time.fixedDeltaTime) * acceleration;
		float secondsNeeded = gearLimit / ((ms / 1000f) * 3600);
		float pitch = secondsNeeded / Throttling.clip.length;
		return pitch;
	}

	private float getReverseThrottlingPitch()
	{
		return (getThrottlingPitch() * drag) / acceleration;
	}

	private void TurnOffBrakingLights()
	{
		foreach (GameObject l in BrakingLights)
		{
			CarPart cp = l.GetComponent<CarPart>();
			if (cp.currentDamage < cp.Health) {
				l.GetComponent<Light2D>().intensity = 0.5f;
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Wall" && !hasPlayer)
			return;

		if (collision.collider.tag != "Ped" && collision.collider.tag != "Prop" && collision.collider.tag != "Player"){
			currentContacts++;
			CheckResultantSpeed(collision);
		}

		int intensity = Tools.SumVelocityCrash(collision.relativeVelocity);

		if ((intensity >= crashVelocityThreshold && collision.collider.tag != "Ped") || (collision.collider.tag == "Ped" && intensity >= crashVelocityThreshold*2)) {

			int lostHealth = (int)(intensity*1.2f);
			CarController cc = collision.collider.GetComponent<CarController>();
			bool crasherIsPlayer = false;
			if (cc != null) {
				if (cc.hasPlayer) {
					crasherIsPlayer = true;
				}
			}

			if (crasherIsPlayer || hasPlayer && value > 0){

				GameData gd = GameObject.Find("GameData").GetComponent<GameData>();

				float remainingValue = ((health - lostHealth) * value) / health;
				if (remainingValue > 1){
					float lostValue = value - remainingValue;
					value = remainingValue;
					gd.DamagesCost += lostValue;
				} else {
					gd.DamagesCost += value;
					value = 0;
				}
			}

			health -= lostHealth;

			foreach(CarPart cp in carParts) {
				if (cp.IsTouching(collision.collider) && cp.Health > cp.currentDamage) {
					cp.ApplyDamage(intensity);
				}
			}

			if (health <= 0 && !SmokePoint.activeSelf) {
				SmokePoint.SetActive(true);
			}

			if (health <= 0) {
				RestoreThrottlingAudio();
			}

			AIDriving ai = GetComponent<AIDriving>();
			if (ai != null)
				ai.ReactToCrash(intensity, collision.gameObject);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if ((collision.collider.tag == "Wall" && !hasPlayer) || collision.collider.tag == "Ped" || collision.collider.tag == "Prop" || collision.collider.tag == "Player")
			return;

		currentContacts--;
	}

	private void CheckResultantSpeed(Collision collision) // TODO: Document this
	{
		if (rb == null) {
			rb = GetComponent<Rigidbody>();
		}

		Vector3 collisionDirection = collision.contacts[0].point - transform.position;
		collisionDirection.Normalize();
		float dot = Vector3.Dot(transform.up, collisionDirection);

		float impactForce = Vector3.Dot(collision.relativeVelocity, collision.contacts[0].normal);

		if (dot < 0) {
			currentSpeed += Mathf.Abs(impactForce) * 0.5f;
		} else if (dot > 0) {
			currentSpeed -= Mathf.Abs(impactForce) * 0.5f;
		}

		rb.AddForce(collision.contacts[0].normal * impactForce, ForceMode.Impulse);

		float sideDot = Vector3.Dot(transform.right, collisionDirection);
		if (Mathf.Abs(sideDot) > 0.1f)
		{
			float spinForce = sideDot * impactForce;
			spinForce = Mathf.Clamp(spinForce, -20, 20);

			rb.AddTorque(Vector3.forward * -spinForce, ForceMode.Impulse);
		}
	}
}
