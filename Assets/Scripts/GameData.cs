
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
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{

	public float DamagesCost
	{
		get {
			return _damagesCost;
		}
		set {
			if (!HasFinished) {
				_damagesCost = value;
				DamagesCostText.text = (int) (value) + "€";
			}
		}
	}
	public int RobbedVehicles {
		get {
			return _robbedVehicles;
		}
		set {
			if (!HasFinished) {
				_robbedVehicles = value;
			}
		}
	}
	public int KilledPeds {
		get {
			return _killedPeds;
		}
		set {
			if (!HasFinished) {
				_killedPeds = value;
			}
		}
	}

	public bool HasFinished = false;
	public float TotalTime;
	public int Grade;
	public TMP_Text DamagesCostText;
	public GameObject PauseMenu;
	public GameObject Player;

	private float _damagesCost;
	private int _robbedVehicles;
	public int _killedPeds;

	void Start()
	{
		if(PlayerPrefs.GetInt("NoTime") == 1) {

			GameObject c = GameObject.Find("Checkpoints");

			for (int i = 0; i < c.transform.childCount; i++) {
				GameObject check = c.transform.GetChild(i).gameObject;

				if (!check.GetComponent<Checkpoint>().IsFinishLine) {
					Destroy(check);
				}
			}

			Destroy(GameObject.Find("Timer"));
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) && GameObject.Find("GameMenu(Clone)") == null) {
			PauseGame();
		}
	}

	public void PauseGame()
	{
		GameObject m = Instantiate(Resources.Load<GameObject>("Prefabs/UI/GameMenu"), transform.position, Quaternion.identity);
		GameMenu gm = m.GetComponent<GameMenu>();
		gm.Reason = 0;

		Time.timeScale = 0f;
		AudioListener.pause = true;
	}

	public void LoseGame()
	{
		GameObject m = Instantiate(Resources.Load<GameObject>("Prefabs/UI/GameMenu"), transform.position, Quaternion.identity);
		GameMenu gm = m.GetComponent<GameMenu>();
		gm.Reason = 1;

		GameObject camera = GameObject.Find("Main Camera");
		camera.transform.SetParent(null);
		camera.GetComponent<AudioListener>().enabled = true;
		GameObject.Find("Spawner").transform.SetParent(null);

		CarController cc = Player.GetComponent<CarController>();
		if (cc != null){
			cc.hasPlayer = false;
			cc.isDriving = false;
		} else {
			PlayerController pc = Player.GetComponent<PlayerController>();
			if (pc != null){
				pc.gameObject.GetComponent<AudioListener>().enabled = false;
				pc.gameObject.AddComponent<PedAI>().mood = 0;
				Destroy(pc);
			}
		}

		Destroy(GameObject.Find("GameUI"));
	}

	public void EndGame()
	{
		HasFinished = true;

		GameObject m = Instantiate(Resources.Load<GameObject>("Prefabs/UI/GameMenu"), transform.position, Quaternion.identity);
		GameMenu gm = m.GetComponent<GameMenu>();
		gm.Reason = 2;

		if(PlayerPrefs.GetInt("NoTime") == 0){
			PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "-d", (int) DamagesCost);
		}

		GameObject camera = GameObject.Find("Main Camera");
		camera.transform.SetParent(null);
		camera.GetComponent<AudioListener>().enabled = true;
		GameObject.Find("Spawner").transform.SetParent(null);
		Destroy(GameObject.Find("ChaserWall"));
		Destroy(GameObject.Find("GameUI"));
	}

}
