using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;//正規表現

public class othello : MonoBehaviour {
	// Start is called before the first frame update
	public GameObject disc;
	public GameObject discs;
	private GameObject discClone;
	private int discNumber = 0;
	public bool turnPlayer = true;
	public GameObject[] discArray = new GameObject[64];//DiscGameobjectの参照用
	private Vector3 [,] position = new Vector3 [8, 8];//座標指定用
	public bool? [,] discPosition = new bool? [8, 8];//処理用Disc白黒確認
	private GameObject [,] discObjPos = new GameObject [8, 8];//処理用DiscObject確認
	public bool [,] canFlipPos = new bool [8, 8];//裏返すことのできる座標の確認
	public bool setDiscPc;
	public ai ai;
	
	void Start () {
		ai AI = GetComponent<ai> ();
		discNumber = 0;
		discs = GameObject.Find ("discs");
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				float x = -0.437f + ((0.439f - (-0.437f)) / 7.0f * i);
				float y = -0.437f + ((0.439f - (-0.437f)) / 7.0f * j);
				float z = -0.5f;
				position [i, j] = new Vector3 (x, y, z);
				discPosition [i, j] = null;
			}
		}

		// プレハブを取得
		//GameObject prefab = (GameObject)Resources.Load ("Prefabs");//Prefabs/子フォルダ
		SetDisc (3, 3, true);
		SetDisc (3, 4, false);
		SetDisc (4, 3, false);
		SetDisc (4, 4, true);//特定位置にDiscをセット
		turnPlayer = false;//黒ターンからスタート
	}

	// Update is called once per frame
	/*
	void Update () {
		if (setDiscPc) {
			//他スクリプトからディスクを配置する。
			setDiscPc = !setDiscPc;
			Debug.Log ("Update!!play");
		}
	}
	*/

	//checkDiscのデータをdiscPositionに移行するプログラム
	public void setDiscScrip () {
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				bool? k = checkDisc (i, j);
				discPosition [i, j]= k;
			}
		}
	}
	//ターンプレイヤーが裏返せる場所があるか？確認
	public bool checkCanSet (bool t) {
		bool checkCanSet = false;
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				//場所が空いているか && 裏返せる場所であるか？
				if (discPosition [i, j] == null && flipDiscsPos (i, j, t)) {
					checkCanSet = true;
				}
			}
		}
		return checkCanSet;
	}
	#region setDisc
	//特定のXYに白黒指定でDiscを配置
	public void SetDisc (int x, int y, bool f) {
		discNumber += 1;
		//Instantiate (disc, position [x, y], Quaternion.identity);//インスタンス化のコード
		//Instance化した物の親を変更するために以下のコードに変化
		discClone = (GameObject)Instantiate (disc, this.transform.position, Quaternion.identity);
		discClone.transform.parent = discs.transform;
		discClone.transform.localPosition = position [x, y];
		discClone.transform.name = "disc" + discNumber;
		if (!f) {discClone.transform.Rotate (180, 0, 0);}//白黒反転
		//instantiateしたobjectの配置
		discClone.GetComponent<discPosR> ().posX = x;
		discClone.GetComponent<discPosR> ().posX = y;
		discClone.GetComponent<discPosR> ().Flip = f;
		discClone.GetComponent<discPosR> ().discNumber = discNumber;
		discPosition [x, y] = turnPlayer;//配置情報の白黒nullを管理
		discObjPos [x, y] = discClone;//配置情報のDiscNumberを管理
		discArray [discNumber-1] = discClone;//objectを参照できるようにPublicの配列にセット
	}
	//XYからDiscの白黒を明示する。白,黒,null
	public bool? checkDisc (int x,int y) {
		if (discObjPos [x, y] != null) {
			bool? flip = discObjPos [x, y].GetComponent<discPosR> ().Flip;
			return flip;
		} else {
			return null;
		}
	}
	//ディスクを裏返す
	public void flipDisc (int x, int y) {
		if (discObjPos [x, y]) {
			GameObject flipObj = discObjPos [x, y];
			bool? flip = checkDisc (x, y);
			if ((flip != null) && (bool)flip) {
				flipObj.transform.Rotate (180, 0, 0);
				discObjPos [x, y].GetComponent<discPosR> ().Flip = false; flip = false;//Objectの方の情報書き換え
				discPosition [x, y] = false;//scriptの方の情報書き換え
			} else {
				flipObj.transform.Rotate (180, 0, 0);
				discObjPos [x, y].GetComponent<discPosR> ().Flip = true; flip = true;//Objectの方の情報書き換え
				discPosition [x, y] = true;//scriptの方の情報書き換え
			}
		} else {
			Debug.Log ("can't flip, you have to choice object.");
		}
	}
	//パネルをクリックされた時
	public void onClickPanel (int panelNum) {
		int x, y;
		x = panelNum % 8;
		y = panelNum / 8;
		if (discPosition [x, y] == null && flipDiscsPos (x, y, turnPlayer)) {//もしdiscPositionに空きがあれば実行
			Debug.Log (x + " " + y);
			SetDisc (x, y, turnPlayer);
			flipDiscs (turnPlayer);
			turnPlayer = !turnPlayer;
			if (!checkCanSet(turnPlayer)) {//passの概念の追加
				Debug.Log ("pass : " + turnPlayer);
				turnPlayer = !turnPlayer;
				if(!checkCanSet (turnPlayer)) {
					GameOver ();
					turnPlayer = !turnPlayer;
				}
			}
		}
		if(turnPlayer == false) {
			(int posX, int posY) i = ai.Input (discPosition, false);
			//Debug.Log (i.posX+""+i.posY);
			onClickPanel (i.posX*8+i.posY);
			//SetDisc (i.posX,i.posY, false);
		}
	}
	//白黒tをxyにおいたときに反転させる．また，flipできたらtrueできなかったfalseを出力する．
	public bool flipDiscsPos (int x, int y,bool t) {
		bool canFlip = false;
		for(int i = 0; i < 8; i++) { for(int j = 0; j < 8; j++) {canFlipPos [i, j]=false;}}//canFlipPosの初期化
		//右側の駒をとる
		int X = x; int Y = y;//初期化
		for (int i = x + 1; i < 8; i++) {//反転箇所指定
			if (checkDisc (i, y) != null) {
				if (checkDisc (i, y) == t) {X = i;Y = y;break;}
			}else if(checkDisc(i,y) == null){X = x;Y = y;break;}
		}
		if ((X - x) > 1 && Y == y) { for (int i = x + 1; i < X; i++) { canFlipPos [i, y] = true; canFlip = true; } }//反転作業
		//右上の駒をとる
		X = x; Y = y;//初期化
		for (int i = x + 1, j = y + 1; i < 8 && j < 8; i++, j++) {//反転箇所指定
			if (checkDisc (i, j) != null) {
				if (checkDisc (i, j) == t) {X = i;Y = j;break;}
			} else if (checkDisc (i, j) == null) {X = x; Y = y; break;}
		}
		if ((X - x) > 1 && (Y - y) > 1) {for (int i = x + 1, j = y + 1; i < X && j < Y; i++, j++) { canFlipPos [i, j] = true; canFlip = true; } }//反転作業
		//右下の駒をとる
		X = x; Y = y;//初期化
		for (int i = x + 1, j = y - 1; i < 8 && -1 < j; i++, j--) {//反転箇所指定
			if (checkDisc (i, j) != null) {
				if (checkDisc (i, j) == t) {X = i;Y = j;break;}
			} else if (checkDisc (i, j) == null) {X = x; Y = y; break;}
		}
		if ((X - x) > 1 && (y - Y) > 1) { for (int i = x + 1, j = y - 1; (i < X) && (Y < j); i++, j--) { canFlipPos [i, j] = true; canFlip = true; } }//反転作業
		//左側の駒をとる
		X = x; Y = y;//初期化
		for (int i = x - 1; -1 < i; i--) {//反転箇所指定
			if (checkDisc (i, y) != null) {
				if (checkDisc (i, y) == t) {X = i;Y = y;break;}
			} else if (checkDisc (i, y) == null) {X = x; Y = y; break;}
		}
		if ((x - X) > 1 && Y == y) { for (int i = x - 1; X < i; i--) { canFlipPos [i, y] = true; canFlip = true; } }//反転作業
		//左上の駒をとる
		X = x; Y = y;//初期化
		for (int i = x - 1, j = y + 1; (-1 < i) && (j < 8); i--, j++) {//反転箇所指定
			if (checkDisc (i, j) != null) {
				if (checkDisc (i, j) == t) {X = i;Y = j;break;}
			} else if (checkDisc (i, j) == null) {X = x; Y = y; break;}
		}
		if ((x - X) > 1 && (Y - y) > 1) {for (int i = x - 1, j = y + 1; X < i && j < Y; i--, j++) { canFlipPos [i, j] = true; canFlip = true; } }//反転作業
		//左下の駒をとる
		X = x; Y = y;//初期化
		for (int i = x - 1, j = y - 1; -1 < i && -1 < j; i--, j--) {//反転箇所指定
			if (checkDisc (i, j) != null) {
				if (checkDisc (i, j) == t) {X = i;Y = j;break;}
			} else if (checkDisc (i, j) == null) {X = x; Y = y; break;}
		}
		if ((x - X) > 1 && (y - Y) > 1) { for (int i = x - 1, j = y - 1; (X < i) && (Y < j); i--, j--) { canFlipPos [i, j] = true; canFlip = true; } }//反転作業
		//上側の駒をとる
		X = x; Y = y;//初期化
		for (int j = y + 1; j < 8; j++) {//反転箇所指定
			if (checkDisc (x, j) != null) {
				if (checkDisc (x, j) == t) {X = x;Y = j;break;}
			} else if (checkDisc (x, j) == null) {X = x; Y = y; break;}
		}
		if (X == x && (Y - y) > 1) { for (int j = y + 1; j < Y; j++) { canFlipPos [x, j] = true; canFlip = true; } }//反転作業
		//上側の駒をとる
		X = x; Y = y;//初期化
		for (int j = y - 1; -1 < j; j--) {//反転箇所指定
			if (checkDisc (x, j) != null) {
				if (checkDisc (x, j) == t) {X = x;Y = j;break;}
			} else if (checkDisc (x, j) == null) {X = x; Y = y; break;}
		}
		if (X == x && (y - Y) > 1) { for (int j = y - 1; Y < j; j--) { canFlipPos [x, j] = true; canFlip = true; } }//反転作業

		return canFlip;
	}

	public void flipDiscs (bool t) {
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (canFlipPos [i, j]) {
					flipDisc (i, j);
				}
			} 
		 }//canFlipPosの初期化
	}

	public void GameOver () {
		int black = 0;
		int white = 0;
		string winner = "";
		for(int i = 0;i < 8; i++) { 
			for(int j = 0; j < 8; j++) {
				if (discPosition [i, j] != null) {
					if (discPosition [i, j] == false) { black++; }
					else if (discPosition [i, j] == true) { white++; }
				} else {}//there is NO object;
			}
		}
		if (black > white) { winner = "BLACK"; } 
		else if (black < white) { winner = "WHITE"; } 
		else if (black == white) { winner = "DRAW"; }
		Debug.Log ("GAME OVER " + winner + " win!!");
		Debug.Log ("score is --- black : " + black + " white : " + white);

	}

	public int panelX (GameObject discObj) {//数字(Gameobject)を入れると配置を出力するツール
		int x = discObj.GetComponent<discPosR> ().posX;
		return x;
	}
	public int panelY (GameObject discObj) {//数字(Gameobject)を入れると配置を出力するツール
		int y = discObj.GetComponent<discPosR> ().posY;
		return y;
	}

	#endregion
}