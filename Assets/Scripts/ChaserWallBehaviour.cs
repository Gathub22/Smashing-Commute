
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

public class ChaserWallBehaviour : MonoBehaviour
{
	public GameObject PlayerLocation;
	public float Margin;

	void Update()
	{
		if (PlayerLocation == null)
			Destroy(gameObject);

		Vector3 pos = PlayerLocation.transform.position;

		if (pos.y > (transform.position.y + Margin)) {
			transform.position = new Vector3(0, pos.y - Margin, 0);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!Tools.IsPlayerColliding(collision.collider)) {
			Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.collider);
		}
	}
}
