
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
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	static public GameObject LevelMenu;
	static public GameObject SettingsMenu;

	void Start()
	{
		LevelMenu = GameObject.Find("Levels");
		LevelMenu.SetActive(false);

		SettingsMenu = GameObject.Find("Settings");
		SettingsMenu.SetActive(false);
	}

	static public void CloseGame()
	{
		Application.Quit();
	}

	static public void GoToLevels()
	{
		GameObject.Find("MainMenu").SetActive(false);
		LevelMenu.SetActive(true);
	}

	static public void GoToSettings()
	{
		GameObject.Find("MainMenu").SetActive(false);
		SettingsMenu.SetActive(true);
	}

	static public void LoadLevel(string level)
	{
		SceneManager.LoadScene(level);
	}

	static public void LoadMainMenu()
	{
		SceneManager.LoadScene("Menu");
	}
}
