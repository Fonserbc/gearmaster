using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarMovement : MonoBehaviour {

	public TrailLight[] lights;

	public bool left = true;

	public Animator anim;

	public Image accelerationImage;

	public Text gearText;

	public AudioSource audio;

	const float ACCELERATION = 5f;
	const float SELF_ACCELERATION = 0.5f;

	float velocity = 0f;

	float revolutions = 0;

	int gear = 0;

	struct Gear {
		public float MIN_SPEED;
		public float MAX_SPEED;
	}

	Gear[] gears;

	// Use this for initialization
	void Start () {
		gears = new Gear[5];

		gears [0].MIN_SPEED = 0f;
		gears [0].MAX_SPEED = 0f;

		gears [1].MIN_SPEED = 1f;
		gears [1].MAX_SPEED = 5f;

		gears [2].MIN_SPEED = 4f;
		gears [2].MAX_SPEED = 10f;

		gears [3].MIN_SPEED = 8f;
		gears [3].MAX_SPEED = 18f;

		gears [4].MIN_SPEED = 15f;
		gears [4].MAX_SPEED = 30f;
	}

	bool wasAccelerating = false;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.W)) {
			SetGear (Mathf.Min (4, gear + 1));
		} else if (Input.GetKeyDown (KeyCode.S)) {
			SetGear(Mathf.Max(0, gear - 1));
		}

		bool accelerating = (left ? Input.GetKey (KeyCode.Space) : Input.GetKey (KeyCode.Return));

		if (accelerating) {
			if (gear == 0) {
				revolutions = Mathf.Min(1.5f, revolutions + 2f * Time.deltaTime);
			}
			else {
				velocity += Easing.Quadratic.Out(GetVelocityGearFactor())*ACCELERATION*Time.deltaTime;
				revolutions = Mathf.Lerp(revolutions, GetVelocityGearFactor(), Time.deltaTime*3f);

				if (revolutions > 1f) {
					velocity *= 0.93f;
					// BSSMM down
				}

				if (!wasAccelerating) {
					for (int i = 0; i < lights.Length; ++i) {
						lights[i].SetTrail(true);
					}
				}
			}
		}
		else {
			if (gear == 0) {
				revolutions = Mathf.Max(0f, revolutions - 0.5f * Time.deltaTime);
			}
			else {
				if (velocity < gears[gear].MIN_SPEED) {
					velocity += Easing.Quadratic.Out(GetVelocityGearFactor())*SELF_ACCELERATION*Time.deltaTime;
				}
				else {
					velocity *= 1f - (0.1f * Time.deltaTime);
				}
				revolutions = Mathf.Lerp(revolutions, GetVelocityGearFactor(), Time.deltaTime*3f);

				if (revolutions < 0f) {
					revolutions = 0f;
				}

				if (wasAccelerating) {
					for (int i = 0; i < lights.Length; ++i) {
						lights[i].SetTrail(false);
					}
				}
			}
		}


		for (int i = 0; i < lights.Length; ++i) {
			lights [i].SetIntensity (Mathf.Min(1f, revolutions));
		}



		if (accelerating && gear > 0) {
			anim.SetFloat ("acc", revolutions);
		} else {
			anim.SetFloat ("acc", Mathf.Lerp(anim.GetFloat("acc"), 0f, Time.deltaTime));
		}
		transform.position += new Vector3 (0, 0, velocity * Time.deltaTime);
		accelerationImage.fillAmount = revolutions;
		audio.pitch = revolutions*2f + 0.5f;

		if (gear > 0) wasAccelerating = accelerating;
	}

	float GetVelocityGearFactor() {
		float minAcc = 0.15f;
		if (gear == 0) {
			return 0;
		}
		else if (velocity >= gears [gear].MIN_SPEED) {
			float value = (velocity - gears [gear].MIN_SPEED) / (gears [gear].MAX_SPEED - gears [gear].MIN_SPEED);
			return minAcc + (1f - minAcc) * value;
		}
		else {
			return Easing.Quadratic.In(Mathf.Min(gears[gear].MIN_SPEED, 0.5f + velocity)/gears[gear].MIN_SPEED)*minAcc;
		}
	}

	void SetGear(int which) {
		gear = which;
		gearText.text = gear.ToString();
		if (gear > 0 && velocity > gears [gear].MAX_SPEED) {
			revolutions = GetVelocityGearFactor();
			velocity = gears [gear].MAX_SPEED;
		}
	}
}
