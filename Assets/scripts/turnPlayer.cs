using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnPlayer : MonoBehaviour {
	public GameObject discs;
	public bool player;
	void Start () { }
	void Update () {
		bool nextPlayer = discs.gameObject.GetComponent<othello> ().turnPlayer;
		if (player != nextPlayer) {
			this.gameObject.transform.Rotate (180, 0, 0);
			player = nextPlayer;
		}
	}
}
