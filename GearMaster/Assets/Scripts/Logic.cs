using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Logic : MonoBehaviour {
	
	public static Logic ins;

	public bool playing = false;

	public GearSquare leftChange, rightChange;

	public GameObject leftCar, rightCar;
	public GameObject mainCanvas;

	public Transform[] cameraPos;

	public Camera mainCamera;
	public GameObject mainCameraQuad;

	public Text readyText;
	public Text timeText;

	public Transform endPos;

	public Transform leftWinPos;
	public Transform rightWinPos;

	public GameObject leftWinCanvas;
	public GameObject rightWinCanvas;

	public AudioClip boob, beeb;
	public AudioSource audioSource;

	int [,] gearMap;

	// Use this for initialization
	void Awake () {
		if (ins != null) {
			DestroyImmediate(ins);
		}
		ins = this;

		gearMap = new int[5, 5];
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 5; ++j) {
				gearMap[i,j] = 0;
			}
		}
		gearMap[2,2] = -100;

		GearSquare.Direction dir;
		bool[] usedDir = new bool[4];
		for (int i = 0; i < 4; ++i) {
			usedDir[i] = false;
		}

		for (int g = 1; g <= 4; ++g) {
			dir = (GearSquare.Direction)Random.Range(0,4);

			while (usedDir[(int)dir]) {
				dir = (GearSquare.Direction)(((int)dir + 1)% 4);
			}

			usedDir[(int)dir] = true;
			
			int posx = 0, posy = 0;
			int pos = 0;
			bool canUse = false;

			while (!canUse) {
				pos = Random.Range(0, 5);
				switch (dir) {
				case GearSquare.Direction.Down:
					posx = pos;
					posy = 0;
					break;
				case GearSquare.Direction.Left:
					posx = 4;
					posy = pos;
					break;
				case GearSquare.Direction.Right:
					posx = 0;
					posy = pos;
					break;
				case GearSquare.Direction.Up:
					posx = pos;
					posy = 4;
					break;
				}

				if (gearMap[posx,posy] <= 0) {
					gearMap[posx,posy] = g;
					canUse = true;
				}
			}

			leftChange.SetGearNumber(dir, pos, g);
			rightChange.SetGearNumber(dir, pos, g);
			GeneratePathFrom(posx,posy, g);
		}

		/*for (int j = 4; j >= 0; --j) {
			Debug.Log(gearMap[4,j]+"\t"+gearMap[3,j] +"\t"+gearMap[2,j] +"\t"+gearMap[1,j] +"\t"+gearMap[0,j]);
		}*/

		leftChange.SetGearMap (gearMap);
		rightChange.SetGearMap (gearMap);
	}

	bool GeneratePathFrom(int x, int y, int gear) {
		if (CheckIsConnected (x, y, gear)) {
			return true;
		}

		GearSquare.Direction dir;
		bool[] usedDir = new bool[4];
		for (int i = 0; i < 4; ++i) {
			usedDir[i] = false;
		}

		int triedCount = 0;
		while (triedCount < 4) {
			dir = (GearSquare.Direction)Random.Range(0,4);
			while (usedDir[(int)dir]) {
				dir = (GearSquare.Direction)(((int)dir + 1)% 4);
			}

			int posx = x, posy = y;
			switch (dir) {
			case GearSquare.Direction.Down:
				if (y - 1 >= 0) {
					posy -= 1;
				}
				break;
			case GearSquare.Direction.Left:
				if (x + 1 <= 4) {
					posx += 1;
				}
				break;
			case GearSquare.Direction.Right:
				if (x - 1 >= 0) {
					posx -= 1;
				}
				break;
			case GearSquare.Direction.Up:
				if (y + 1 <= 4) {
					posy += 1;
				}
				break;
			}

			if (posx != x || posy != y) {
				if (gearMap[posx, posy] < 0 && gearMap[posx, posy] != -gear) {
					return true;
				}
				else if (gearMap[posx, posy] == 0) {
					gearMap[posx, posy] = -gear;
					if (GeneratePathFrom(posx, posy, gear)) {
						return true;
					}
					else {
						gearMap[posx, posy] = 0;
					}
				}
			}

			usedDir[(int)dir] = true;
			triedCount++;
		}

		return false;
	}

	bool CheckIsConnected(int x, int y, int gear) {
		GearSquare.Direction dir;
		
		for (int i = 0; i < 4; ++i) {
			dir = (GearSquare.Direction)i;

			int value = 0;

			switch (dir) {
			case GearSquare.Direction.Down:
				if (y - 1 >= 0) {
					value = gearMap[x, y - 1];
			    }
		    	break;
		    case GearSquare.Direction.Left:
			    if (x + 1 <= 4) {
					value = gearMap[x + 1, y];
				}
				break;
			case GearSquare.Direction.Right:
				if (x - 1 >= 0) {
					value = gearMap[x - 1, y];
				}
				break;
			case GearSquare.Direction.Up:
				if (y + 1 <= 4) {
					value = gearMap[x, y + 1];
				}
				break;
			}

			if (value < 0 && value != -gear) return true;
		}

		return false;
	}

	bool gameStarted = false;
	float animTime = 9f;
	int cameraCount = 0;
	int textCount = 0;
	int soundCount;
	float timeRunning = 30f;

	void Update() {
		if (!playing) {
			if (!gameStarted) {
				animTime -= Time.deltaTime;

				if (animTime <= 0f) {
					animTime = 2f;
					readyText.text = "GO!";

					playing = true;
					gameStarted = true;
					audioSource.PlayOneShot(beeb);
				}
				else if (animTime <= 6f && textCount == 0) {
					readyText.text = "Ready?";
					textCount ++;
				}
				else if (animTime <= 4f && textCount == 1) {
					readyText.text = "Set";
					textCount ++;
				}

				if (animTime <= 2f && soundCount == 0) {
					soundCount++;

					audioSource.PlayOneShot(boob);
				}
				else if (animTime <= 1f && soundCount == 1) {
					soundCount++;

					audioSource.PlayOneShot(boob);
				}

				if (animTime <= 7f && cameraCount == 0) {
					cameraCount = 1;
					mainCamera.transform.position = cameraPos[cameraCount].position;
					mainCamera.transform.rotation = cameraPos[cameraCount].rotation;
				}
				else if (animTime <= 5f && cameraCount == 1) {
					cameraCount = 2;
					mainCamera.transform.position = cameraPos[cameraCount].position;
					mainCamera.transform.rotation = cameraPos[cameraCount].rotation;
				}
				else if (animTime <= 3f && cameraCount == 2) {
					cameraCount = 3;

					mainCamera.enabled = false;
					mainCameraQuad.SetActive(false);
				}
			}
			else {
				animTime -= Time.deltaTime;

				if (animTime <= 0f) {
					animTime = 5f;
					Application.LoadLevel(0);
				}
			}
		}
		else {
			timeRunning -= Time.deltaTime;
			if (timeRunning <= 0f) {
				timeRunning = 0f;

				readyText.text = "Noone wins!";
				animTime = 7f;
				playing = false;
			}
			timeText.text = Mathf.CeilToInt(timeRunning).ToString();

			if (animTime > 0f) {
				animTime -= Time.deltaTime;

				if (animTime <= 0f) {
					readyText.text = "";
				}
			}

			if (leftCar.transform.position.z > endPos.position.z || rightCar.transform.position.z > endPos.position.z) {
				if (leftCar.transform.position.z > rightCar.transform.position.z) {
					mainCamera.transform.parent = leftWinPos;
					mainCamera.transform.localPosition = Vector3.zero;
					mainCamera.transform.localRotation = Quaternion.identity;

					leftWinCanvas.SetActive(true);
				} else {
					mainCamera.transform.parent = rightWinPos;
					mainCamera.transform.localPosition = Vector3.zero;
					mainCamera.transform.localRotation = Quaternion.identity;
					
					rightWinCanvas.SetActive(true);
				}
				playing = false;
				animTime = 7f;
				mainCamera.enabled = true;
				mainCameraQuad.SetActive(true);
				mainCanvas.SetActive(false);
			}
		}
	}
}
