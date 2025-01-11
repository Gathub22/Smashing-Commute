
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerArea : MonoBehaviour
{

	public String[] detectableObjects;
	public ArrayList objects;

	private BoxCollider area;

	// Start is called before the first frame update
	void Start()
	{
		objects = new ArrayList();
		area = GetComponent<BoxCollider>();
	}

	public bool IsInArea(GameObject obj)
	{
		return objects.Contains(obj);
	}

	public bool IsInArea(Vector3 pos)
	{
		return area.bounds.Contains(pos);
	}

	void OnTriggerEnter(Collider collider)
	{
		String tag = collider.gameObject.tag;

		if (detectableObjects.Contains(tag)) {
			objects.Add(collider.gameObject);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		String tag = collider.gameObject.tag;

		if (detectableObjects.Contains(tag)) {
			objects.Remove(collider.gameObject);
		}
	}
}
