
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
using Unity.VisualScripting;
using UnityEngine;

public class HintBox : MonoBehaviour
{
	public string[] Hints;
	public GameObject Body;
	public GameObject LeftButton;
	public GameObject RightButton;

	static public GameObject STBody;
	static public string[] STHints;
	static public int index = 0;

	void Awake()
	{
		STHints = Hints;
		STBody = Body;
		STBody.GetComponent<TMP_Text>().text = STHints[0];
	}

	static public void GoRight()
	{
		if (++index < STHints.Length){
			STBody.GetComponent<TMP_Text>().text = STHints[index];
		} else {
			STBody.GetComponent<TMP_Text>().text = STHints[0];
			index = 0;
		}
	}

	static public void GoLeft()
	{
		if (--index > -1){
			STBody.GetComponent<TMP_Text>().text = STHints[index];
		} else {
			index = STHints.Length - 1;
			STBody.GetComponent<TMP_Text>().text = STHints[index];
		}
	}
}
