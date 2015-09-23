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

	public Transform[] wheels;

	const float WHEEL_RADIUS = 0.10f;

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
		gears [2].MAX_SPEED = 14f;

		gears [3].MIN_SPEED = 11f;
		gears [3].MAX_SPEED = 20f;

		gears [4].MIN_SPEED = 17f;
		gears [4].MAX_SPEED = 26f;
	}

	bool wasAccelerating = false;
	public bool accelerating = false;
	
	// Update is called once per frame
	void Update () {
		//if ((Input.GetKeyDown (KeyCode.W) && left) || (Input.GetKeyDown (KeyCode.UpArrow) && !left)) {
		//	SetGear (Mathf.Min (4, gear + 1));
		//} else if ((Input.GetKeyDown (KeyCode.S) && left) || (Input.GetKeyDown (KeyCode.DownArrow) && !left)) {
		//	SetGear(Mathf.Max(0, gear - 1));
		//}

		accelerating = (left ? Input.GetKey (KeyCode.Space) : Input.GetKey (KeyCode.Return));

		if (accelerating) {
			if (gear == 0) {
				revolutions = Mathf.Min(1.5f, revolutions + 2f * Time.deltaTime);
			}
			else {
				velocity += GetVelocityGearFactor()*ACCELERATION*Time.deltaTime;
				revolutions = Mathf.Lerp(revolutions, GetRevolutionVelocityFactor(), Time.deltaTime*3f);

				if (revolutions > 1f) {
					//revolutions *= 0.93f;
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
					velocity += GetVelocityGearFactor()*SELF_ACCELERATION*Time.deltaTime;
				}
				else if (velocity > gears[gear].MAX_SPEED) {
					velocity += GetVelocityGearFactor()*ACCELERATION*Time.deltaTime;
				}
				else {
					velocity -= SELF_ACCELERATION*Time.deltaTime;
				}
				revolutions = Mathf.Lerp(revolutions, GetRevolutionVelocityFactor(), Time.deltaTime*3f);

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



		if (accelerating) {
			anim.SetFloat ("acc", revolutions);
		} else {
			anim.SetFloat ("acc", Mathf.Lerp(anim.GetFloat("acc"), 0f, Time.deltaTime));
		}
		transform.position += new Vector3 (0, 0, velocity * Time.deltaTime);
		accelerationImage.fillAmount = revolutions;
		audio.pitch = revolutions*3f + (left? 0.5f : 0.7f);

		if (gear > 0) wasAccelerating = accelerating;

		float angle = transform.position.z / (Mathf.PI * WHEEL_RADIUS);
		Quaternion wheelRot = Quaternion.AngleAxis (angle * Mathf.Rad2Deg, Vector3.right);

		for (int i = 0; i < wheels.Length; ++i) {
			wheels[i].localRotation = wheelRot;
		}
	}

	float GetVelocityGearFactor() {
		float minAcc = 0.15f;
		if (gear == 0) {
			return 0;
		}
		else if (velocity >= gears [gear].MIN_SPEED && velocity < gears[gear].MAX_SPEED) {
			float value = (velocity - gears [gear].MIN_SPEED) / (gears [gear].MAX_SPEED - gears [gear].MIN_SPEED);
			return minAcc + (1f - minAcc) * value;
		}
		else if (velocity > gears[gear].MAX_SPEED) {
			float value = -(velocity - gears[gear].MAX_SPEED)/(gears[gear].MAX_SPEED/2f);
			return -minAcc + value;
		}
		else {
			if (gear == 1) {
				return minAcc*2f;
			}
			else {
				float value = Mathf.Min(gears[gear].MIN_SPEED, 0.5f + velocity)/gears[gear].MIN_SPEED;
				return value*minAcc;
			}
		}
	}

	float GetRevolutionVelocityFactor() {
		return Mathf.Max(0f, (velocity - gears [gear].MIN_SPEED) / (gears [gear].MAX_SPEED - gears [gear].MIN_SPEED));
	}

	public void SetGear(int which) {
		gear = which;
		gearText.text = gear.ToString();
		if (gear > 0 && velocity > gears [gear].MAX_SPEED) {
			//revolutions = GetRevolutionVelocityFactor();
			//velocity = gears [gear].MAX_SPEED;
		}
	}
}
