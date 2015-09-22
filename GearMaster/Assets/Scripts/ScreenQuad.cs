using UnityEngine;
using System.Collections;

public class ScreenQuad : MonoBehaviour {

	public bool split = false;
	public enum Side {
		Left, Right
	};

	public Side side = Side.Left;

	// Use this for initialization
	void Start () {
		float ratio = (float)Screen.width / (float)Screen.height;

		if (!split) {
			transform.localScale = new Vector3 (1f * ratio, 1f, 1f);
		}
		else {
			float width = 1f * ratio / 2f;
			transform.localScale = new Vector3 (width, 1f, 1f);
			transform.localPosition = new Vector3((side == Side.Left? - width/2f : width/2f), 0f, transform.localPosition.z);
		}
	}
}
