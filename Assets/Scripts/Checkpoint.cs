
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
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public bool IsFinishLine = false;
	public int AddedSeconds;
	public Sprite StreetSprite;
	public GameObject RelatedCheck;

	void OnTriggerEnter(Collider collider){
		try {
			if (collider.tag == "Player" || collider.GetComponent<CarController>().hasPlayer) {

				if (IsFinishLine) {
					CarController cc = collider.GetComponent<CarController>();
					if (cc != null){
						collider.GetComponent<CarController>().hasPlayer = false;
						Destroy(collider.GetComponent<AudioListener>());
					} else {
						PlayerController pc = collider.GetComponent<PlayerController>();
						pc.AddComponent<PedAI>().walkTarget = new Vector3(pc.transform.position.x, pc.transform.position.y + 1000);
						Destroy(pc);
					}

					GameObject.Find("GameData").GetComponent<GameData>().EndGame();
				} else {
					GameObject.Find("Timer").GetComponent<Timer>().CurrentTime += AddedSeconds;
					//TODO: Spawn street sprites
				}

				if (RelatedCheck != null)
					Destroy(RelatedCheck);

				AudioClip ac = Resources.Load<AudioClip>("Audio/Effects/UI/Checkpoint");
				GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(ac);

				Destroy(gameObject);
			}
		} catch {}
	}
}
