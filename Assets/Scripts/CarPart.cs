
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
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CarPart : MonoBehaviour
{

	public ArrayList touchingColliders;
	public Sprite[] damageSprites;
	public Light2D Light;
	public int Health;
	public bool IsGlass;

	public float currentDamage;
	[SerializeField]
	private float damageLimit;
	[SerializeField]
	private int activeSpriteIndex;
	[SerializeField]
	private AudioSource audioSource;

	private SpriteRenderer sr;

	// Start is called before the first frame update
	void Start()
	{
		touchingColliders = new ArrayList();
		currentDamage = 0;
		if (Light == null){
			sr = GetComponent<SpriteRenderer>();
			damageLimit = Health / damageSprites.Length;
			activeSpriteIndex = -1;
		}
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.spatialBlend = 1;
	}

	void OnTriggerEnter(Collider collider)
	{
		try {
			touchingColliders.Add(collider);
		} catch {}
	}

	void OnTriggerExit(Collider collider)
	{
		touchingColliders.Remove(collider);
	}

	public bool IsTouching(Collider collider)
	{
		foreach (Collider c in touchingColliders) {
			if (c != null){
				if (c == collider)
					return true;
			}
		}
		return false;
	}

	public void ApplyDamage(float damage)
	{
		currentDamage += damage;

		// In case it's a light...
		if (Light != null && currentDamage >= Health){

			AudioClip ac = Tools.GetRandomGlassClip();
			audioSource.PlayOneShot(ac);

			GameObject p = Resources.Load<GameObject>("Prefabs/Particles/Glass");
			Instantiate(p, transform.position, Quaternion.identity);

			Destroy(Light);
		}else {
			// In case it's just a normal part...
			if (activeSpriteIndex == -1) {
				if (currentDamage > damageLimit) {

					if (++activeSpriteIndex < damageSprites.Length)
						sr.sprite = damageSprites[activeSpriteIndex];

					AudioClip ac = IsGlass ? Tools.GetRandomGlassClip() : Tools.GetRandomDamageClip();
					audioSource.PlayOneShot(ac);

					GameObject p = Resources.Load<GameObject>("Prefabs/Particles/Crash");
					Instantiate(p, transform.position, Quaternion.identity);
				}
			} else {
				if (currentDamage > damageLimit * (activeSpriteIndex + 1)){
					if (++activeSpriteIndex < damageSprites.Length)
						sr.sprite = damageSprites[activeSpriteIndex];

					AudioClip ac = IsGlass ? Tools.GetRandomGlassClip() : Tools.GetRandomDamageClip();
					audioSource.PlayOneShot(ac);

					GameObject p = Resources.Load<GameObject>("Prefabs/Particles/Crash");
					Instantiate(p, transform.position, Quaternion.identity);
				}
			}
		}
	}
}
