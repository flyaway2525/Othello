using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//othelloで指定した座標をもとに動作をエフェクトを追加する。
//ひっくり返る時にくるくる回るなど

public class discPosR : MonoBehaviour{
	public int posX = 0;
	public int posY = 0;
	public bool Flip = true;//false is black, true is white
	public int discNumber = 0;
}
