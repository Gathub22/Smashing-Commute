
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

public class PedWalk : MonoBehaviour
{

	private BoxCollider walkArea;

	// Start is called before the first frame update
	void Awake()
	{
		walkArea = GetComponent<BoxCollider>();
		Destroy(GetComponent<SpriteRenderer>());
	}

	public Vector3 GetRandomWalkPath()
	{
		Vector3 newTarget = new Vector3(
				Random.Range(-0.5f, 0.5f),
				Random.Range(-0.5f, 0.5f),
				0
			);
		newTarget = walkArea.transform.TransformPoint(walkArea.center + Vector3.Scale(newTarget, walkArea.size));
		return newTarget;
	}

	void OnTriggerStay(Collider collider)
	{
		PedAI p = collider.gameObject.GetComponent<PedAI>();

		if (p == null)
			return;

		if (p.mood == 0 && p.walkTarget == new Vector3(0,0,-1)) {
			p.walkTarget = GetRandomWalkPath();
		}
	}
}
