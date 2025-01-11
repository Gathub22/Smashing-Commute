
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

public class TimeSwitch : MonoBehaviour
{

	static private int isInactive;
	static private TMP_Text tmp_text;

	void Start()
	{
		tmp_text = GetComponent<TMP_Text>();

		isInactive = PlayerPrefs.GetInt("NoTime");

		if (isInactive == 0) {
			tmp_text.text = "Turn off time";
		} else {
			tmp_text.text = "Turn on time";
		}
	}

	static public void SetTime()
	{

		if (isInactive == 0) {
			isInactive = 1;
			tmp_text.text = "Turn on time";
		} else {
			isInactive = 0;
			tmp_text.text = "Turn off time";
		}

		PlayerPrefs.SetInt("NoTime", isInactive);
	}
}
