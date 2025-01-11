
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
using UnityEngine.Timeline;

public class TrafficSignal : MonoBehaviour
{

	public GameObject[] firstPhase;
	public GameObject[] secondPhase;
	public float phaseDuration;

	[SerializeField]
	private float currentSeconds;

	[SerializeField]
	private float currentPhase;

	void Awake()
	{
		currentPhase = 0;
		currentSeconds = 0;
		activatePhase(firstPhase);
		deactivatePhase(secondPhase);
	}

	// Start is called before the first frame update
	void Start()
	{
		currentPhase = 0;
		currentSeconds = 0;
	}

	// Update is called once per frame
	void Update()
	{
		currentSeconds += Time.deltaTime;

		if (currentSeconds >= phaseDuration) {
			if (currentPhase == 0) {
				activatePhase(secondPhase);
				deactivatePhase(firstPhase);
				currentPhase++;
			} else {
				activatePhase(firstPhase);
				deactivatePhase(secondPhase);
				currentPhase--;
			}
			currentSeconds = 0;
		}
	}

	private void deactivatePhase(GameObject[] phase)
	{
		foreach (GameObject g in phase) {
			Waypoint w = g.GetComponent<Waypoint>();
			if (w != null) {
				w.passable = false;
			} else {
				PedSideCrosswalk psc = g.GetComponent<PedSideCrosswalk>();
				psc.passable = false;
			}
		}
	}

	private void activatePhase(GameObject[] phase)
	{
		foreach (GameObject g in phase) {
			Waypoint w = g.GetComponent<Waypoint>();
			if (w != null) {
				w.passable = true;
			} else {
				PedSideCrosswalk psc = g.GetComponent<PedSideCrosswalk>();
				psc.passable = true;
			}
		}
	}
}
