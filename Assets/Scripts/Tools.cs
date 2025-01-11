
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

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Tools
{
  public static int SumVelocityCrash(Vector3 relativeVelocity)
	{
		return (int)System.Math.Abs(relativeVelocity.x + relativeVelocity.y + relativeVelocity.z);
	}

	public static bool IsPlayerColliding(Collision collision)
	{
		if (collision.collider.tag == "Player")
			return true;

		try{
			if (collision.collider.GetComponent<CarController>().hasPlayer)
				return true;
		} catch{}

		return false;
	}

	public static bool IsPlayerColliding(Collider collider)
	{
		if (collider.tag == "Player")
			return true;

		try{
			if (collider.GetComponent<CarController>().hasPlayer)
				return true;
		} catch{}

		return false;
	}

	public static bool IsGameDataRelated(string tag)
	{
		List<string> ignoredTags = new List<string> {
			"Waypoint",
			"Crosswalk",
			"Pedwalk",
			"PlayerArea",
			"Spawner"
		};
		return ignoredTags.Contains(tag);
	}

	public static bool IsHumanColliding(Collision collision)
	{
		return collision.collider.GetComponent<PedAI>() != null || collision.collider.GetComponent<PlayerController>() != null;
	}

	public static Object GetRandomPed()
	{
		Object[] peds = Resources.LoadAll("Prefabs/Peds/");

		return peds[Random.Range(0, peds.Length)];
	}

	public static GameObject GetRandomVehicle()
	{
		GameObject[] cars = Resources.LoadAll<GameObject>("Prefabs/Vehicles/");

		return cars[Random.Range(0, cars.Length)];
	}

	public static AudioClip GetRandomGlassClip()
	{
		AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/Effects/Glass/");
		return clips[Random.Range(0, clips.Length)];
	}

	public static AudioClip GetRandomDamageClip()
	{
		AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/Effects/Damage/");
		return clips[Random.Range(0, clips.Length)];
	}

	public static AudioClip GetRandomDeathClip()
	{
		AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/Effects/Death/");
		return clips[Random.Range(0, clips.Length)];
	}
}
