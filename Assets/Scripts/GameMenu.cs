
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
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{

	public int Reason;
	public GameObject Title;
	public GameObject Damages;
	public GameObject Restart;
	public GameObject Exit;


	void Start()
	{
		if (Reason == 0) {
			Title.GetComponent<TMP_Text>().text = "Pause Menu";
			Damages.SetActive(false);
		} else if (Reason == 1) {
			Title.GetComponent<TMP_Text>().text = "Wasted";
			Damages.SetActive(false);
		} else if (Reason == 2) {
			Title.GetComponent<TMP_Text>().text = "Winner!";
			Damages.GetComponent<TMP_Text>().text = ( (int) (GameObject.Find("GameData").GetComponent<GameData>().DamagesCost) ) + "€";
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape)) {
			Time.timeScale = 1f;
			AudioListener.pause = false;

			Destroy(gameObject);
		}
	}

	static public void RestartGame()
	{
		Time.timeScale = 1f;
		AudioListener.pause = false;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	static public void QuitGame()
	{
		Time.timeScale = 1f;
		AudioListener.pause = false;
		SceneManager.LoadScene("Menu");
	}
}
