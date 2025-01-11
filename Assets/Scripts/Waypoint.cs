
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
using UnityEngine;

public class Waypoint : MonoBehaviour
{

	public int id;
	public GameObject nextWaypoint;
	public bool passable;
	public bool asSpawnPoint;
	public int topSpeed;
	public GameObject rightWaypoint;
	public GameObject leftWaypoint;

	void Awake()
	{
		Destroy(GetComponent<SpriteRenderer>());
	}

	// void OnTriggerEnter(Collider collider)
	// {
	// 	if (!Tools.IsGameDataRelated(collider.tag)) {
	// 		Colliders++;
	// 	}
	// }

	// void OnTriggerExit(Collider collider)
	// {
	// 	if (!Tools.IsGameDataRelated(collider.tag)) {
	// 		Colliders--;
	// 	}
	// }

	// public bool IsBusy()
	// {
	// 	return Colliders > 0;
	// }

}
