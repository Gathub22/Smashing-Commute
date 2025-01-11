
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
using UnityEngine;

public class Prop : MonoBehaviour
{
	public Sprite DestroyedSprite;
	public float Value;
	public float CrashVelocitySensibility;
	public Collider Collider;
	public bool IsTurnable;

	void OnCollisionEnter(Collision collision)
	{
		if (Tools.SumVelocityCrash(collision.relativeVelocity) >= CrashVelocitySensibility && !Tools.IsHumanColliding(collision)) {
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			sr.sprite = DestroyedSprite;
			sr.sortingOrder = -1;
			if (IsTurnable)
				transform.up = collision.contacts[0].normal;

			if (Tools.IsPlayerColliding(collision))
				GameObject.Find("GameData").GetComponent<GameData>().DamagesCost += Value;

			Destroy(Collider);

			try {
				TrafficLight tl = GetComponent<TrafficLight>();
				Destroy(tl.Light);
				Destroy(tl);
			} catch (Exception) { }

			AudioSource audioSource = GetComponent<AudioSource>();
			if (audioSource.clip == null){
				audioSource.clip = Tools.GetRandomDamageClip();
			}
			audioSource.Play();

			Destroy(this);
		}
	}
}
