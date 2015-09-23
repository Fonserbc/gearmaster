using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GearSquare : MonoBehaviour {

	public Image gearChange;

	public Image[] horizontals;
	public Image[] verticals;

	public Text topGear, bottomGear, leftGear, rightGear;

	public bool left;

	public CarMovement movement;

	public AudioClip gearFail;

	public AudioSource audioSource;

	public enum Direction {
		Up = 0,
		Down = 1,
		Right = 2, 
		Left = 3
	};

	const float ANIMATION_TIME = 0.07f;

	bool animating = false;
	float animationTime = 0f;

	Vector2 lastPos = new Vector2 (2f, 2f);
	Vector2 currentPos = new Vector2(2f, 2f);
	Vector2 wantedPos = new Vector2(2f, 2f);

	int[,] gearMap;

	// Update is called once per frame
	void Update () {
		if (left) {
			if (Input.GetKeyDown(KeyCode.W)) {
				MoveGear(Direction.Up);
			}
			else if (Input.GetKeyDown(KeyCode.S)) {
				MoveGear(Direction.Down);
			}

			if (Input.GetKeyDown(KeyCode.A)) {
				MoveGear(Direction.Left);
			}
			else if (Input.GetKeyDown(KeyCode.D)) {
				MoveGear(Direction.Right);
			}
		}
		else {
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				MoveGear(Direction.Up);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				MoveGear(Direction.Down);
			}
			
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				MoveGear(Direction.Left);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow)) {
				MoveGear(Direction.Right);
			}
		}

		if (animating) {
			animationTime += Time.deltaTime;

			if (animationTime < ANIMATION_TIME) {
				currentPos = Vector3.Lerp(lastPos, wantedPos, Easing.Quadratic.Out(animationTime/(ANIMATION_TIME*3f)));
			}
			else {
				FinishAnimation();
			}
		}

		// Graphics

		gearChange.rectTransform.anchoredPosition = new Vector2 (- 9f - 9f*currentPos.x, 9f + 9f*currentPos.y);
	}

	void FinishAnimation() {
		if (animating) {
			if (CanMoveTo (wantedPos)) {
				currentPos = wantedPos;
				lastPos = wantedPos;

				int gear = IsGear(wantedPos);
				if (gear > 0) {
					movement.SetGear(gear);
				}
				else {
					movement.SetGear(0);
				}
			}
			else {
				currentPos = lastPos;
			}

			animating = false;
		}
	}

	public void MoveGear (Direction dir) {
		FinishAnimation ();

		wantedPos = GetPosFromDirection (lastPos, dir);
		animating = true;
		animationTime = 0f;
	}

	bool CanMoveTo(Vector2 pos) {
		if (!Logic.ins.playing) {
			return false;
		}
		else if (movement.accelerating && (IsGear (lastPos) > 0)) {
			audioSource.PlayOneShot(gearFail, 1f);
			return false;
		}
		return pos.x <= 4f && pos.x >= 0f && pos.y <= 4f && pos.y >= 0f && gearMap[(int)pos.x, (int)pos.y] != 0;
	}

	Vector2 GetPosFromDirection (Vector2 from, Direction towards) {
		switch (towards) {
		case Direction.Down:
			return from + Vector2.down;
		case Direction.Up:
			return from + Vector2.up;
		case Direction.Left:
			return from + Vector2.right;
		case Direction.Right:
			return from + Vector2.left;
		}

		return from;
	}

	int IsGear(Vector2 pos) {
		if (pos.x == 2 && pos.y == 2) {
			return 0;
		}
		else {
			return gearMap[(int)pos.x, (int)pos.y];
		}
	}

	public void SetGearMap(int[,] newGearMap) {
		if (!left) {
			gearMap = new int[5,5];

			for (int i = 0; i < 5; ++i) {
				for (int j = 0; j < 5; ++j) {
					gearMap[i,j] = newGearMap[4 - i,j];
				}
			}
		}
		else {
			gearMap = newGearMap;
		}

		DisplayGearMap ();
	}

	void DisplayGearMap() {
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 4; ++j) {
				if (gearMap[i,j] == 0 || gearMap[i,j + 1] == 0) {
					verticals[i*4 + j].enabled = false;
				}
			}
		}
		for (int i = 0; i < 4; ++i) {
			for (int j = 0; j < 5; ++j) {
				if (gearMap[i,j] == 0 || gearMap[i + 1,j] == 0) {
					horizontals[i + j*4].enabled = false;
				}
			}
		}
	}

	public void SetGearNumber(Direction dir, int x, int gear) {
		if (!left) {
			int d = (int)dir;
			if (d/2 == 1) {
				dir = (Direction)(((d + 1)%2) + (d / 2) * 2);
			}
			else {
				x = 4 - x;
			}
		}

		switch (dir) {
		case GearSquare.Direction.Down:
			bottomGear.rectTransform.anchoredPosition = new Vector2(- 9f - 9f*x, bottomGear.rectTransform.anchoredPosition.y);
			bottomGear.text = gear.ToString();
			break;
		case GearSquare.Direction.Left:
			leftGear.rectTransform.anchoredPosition = new Vector2(leftGear.rectTransform.anchoredPosition.x, 9f + 9f*x);
			leftGear.text = gear.ToString();
			break;
		case GearSquare.Direction.Right:
			rightGear.rectTransform.anchoredPosition = new Vector2(rightGear.rectTransform.anchoredPosition.x, 9f + 9f*x);
			rightGear.text = gear.ToString();
			break;
		case GearSquare.Direction.Up:
			topGear.rectTransform.anchoredPosition = new Vector2(- 9f - 9f*x, topGear.rectTransform.anchoredPosition.y);
			topGear.text = gear.ToString();
			break;
		}
	}
}
