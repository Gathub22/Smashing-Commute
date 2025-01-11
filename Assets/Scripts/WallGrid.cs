
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

public class WallGrid : MonoBehaviour
{
	public GameObject Wall;
	public BoxCollider Trigger;

	[SerializeField]
	private BoxCollider bc;

	void Awake()
	{
		Destroy(GetComponent<SpriteRenderer>());
		bc = GetComponent<BoxCollider>();
		Vector3 s = bc.size;

		Vector3 w_s = Wall.GetComponent<SpriteRenderer>().bounds.size;
		int q = Mathf.FloorToInt(s.x / w_s.x);

		float width = q * w_s.x;
		float x = -width / 2 + w_s.x / 2;

		for (int i = 1; i <= q; i++) {
			Vector3 worldPosition = transform.TransformPoint(new Vector3(x, 0));
			Instantiate(Wall, worldPosition, transform.rotation).transform.parent = transform;
			x += w_s.x;
		}

		Trigger.size = new Vector3(bc.size.x + 1, bc.size.y + 1);
	}

	void OnTriggerEnter(Collider collider)
	{
		if (!Tools.IsPlayerColliding(collider)) {
			Physics.IgnoreCollision(bc, collider);
		}
	}
}
