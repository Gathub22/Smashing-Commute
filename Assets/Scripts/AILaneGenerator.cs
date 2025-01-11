
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
using Unity.VisualScripting;
using UnityEngine;

public class AILaneGenerator : MonoBehaviour
{
	void Awake()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		for (int i = 0; i < transform.childCount; i++)
		{
			try{
				GameObject w = transform.GetChild(i).gameObject;
				w.transform.position = new Vector3(w.transform.position.x, w.transform.position.y, 0);
				w.GetComponent<Waypoint>().nextWaypoint = transform.GetChild(i+1).gameObject;
			}catch{}
		}
	}
}
