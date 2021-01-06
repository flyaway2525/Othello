using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ai : MonoBehaviour {
	//private bool? [,] discPosition = new bool? [8, 8];//処理用Disc白黒確認
	//public bool [,] canFlipPos = new bool [8, 8];//裏返すことのできる座標の確認
	public othello othello;
	private static int [,] valuePos = {
	{ 5,-5, 3, 3, 3, 3,-5, 5 },
	{-5,-5, 4, 1, 1, 4,-5,-5 },
	{ 3, 4, 2, 1, 1, 2, 4, 3 },
	{ 3, 1, 1, 1, 1, 1, 1, 3 },
	{ 3, 1, 1, 1, 1, 1, 1, 3 },
	{ 3, 4, 2, 1, 1, 2, 4, 3 },
	{-5,-5, 4, 1, 1, 4,-5,-5 },
	{ 5,-5, 3, 3, 3, 3,-5, 5 }
	};
	public bool? [,] discPos = new bool? [8, 8];
	private int? [,] discEvaluation = new int? [8, 8];
	private int playerPoint = 0;
	public int [] aiPoint = { 0, 9, 9 };//ポイント，ｘ座標，y座標
	private bool firstStrike = false;

	public void aiflip() {
		int posX;
		int posY;
	
	
	
	
	}
	public void aiSetDisc () {
		othello.setDiscScrip ();
		Debug.Log ("OK");
		//discPos = othello.discPosition;

		Debug.Log (othello.discPosition[1,1]);
	}
	/*
	input ディスプポジションと次の石を入力することで，最適な次のポジションを提示するシステム．
	*/
	public (int posX,int posY) Input(bool? [,] discPosition,bool turnPlayer) {
		//評価する
		//置ける場所を列挙する
		//それぞれの場所において場所の評価をする．
		//それぞれに対して取れる石の数を評価する．
		//入力　ディスクポジションとターン
		//出力　ディスクポジション
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				if (discPosition [x, y] == null && othello.flipDiscsPos (x, y, turnPlayer)) {//もしdiscPositionに空きがあれば実行
					//Debug.Log (x + " " + y);
					//othello.SetDisc (x, y, turnPlayer);
					//othello.flipDiscs (turnPlayer);
					if(aiPoint[0] < valuePos [x, y]) {
						aiPoint [0] = valuePos [x, y];
						aiPoint [1] = x;
						aiPoint [2] = y;
					}/*
					else if(aiPoint [0] == valuePos [x, y] && (Random (0,10) > 5)) {
						aiPoint [0] = valuePos [x, y];
						aiPoint [1] = x;
						aiPoint [2] = y;
					}*/

					/*
					turnPlayer = !turnPlayer;
					if (!othello.checkCanSet (turnPlayer)) {//passの概念の追加
						Debug.Log ("pass : " + turnPlayer);
						turnPlayer = !turnPlayer;
						if (!othello.checkCanSet (turnPlayer)) {
							othello.GameOver ();
							turnPlayer = !turnPlayer;
						}
					}
					*/
				}  else {
					discEvaluation [x, y] = null;
				}
			}//for Y
		}//for X
		return (aiPoint[1],aiPoint[2]);
	}




	void output () { 
		
	
	}
}




