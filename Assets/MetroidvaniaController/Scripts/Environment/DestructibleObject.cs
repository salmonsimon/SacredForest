using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
	public float life = 3;

	// Desired duration of the shake effect
	private float shakeDuration = 0f;

	// A measure of magnitude for the shake. Tweak based on your preference
	private float shakeMagnitude = 0.25f;

	// A measure of how quickly the shake effect should evaporate
	private float dampingSpeed = 1f;

	// The initial position of the GameObject
	Vector3 initialPosition;
	// Start is called before the first frame update

	void Awake()
	{
		initialPosition = transform.position;
	}

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (life <= 0)
		{
			Destroy(gameObject);
		}
		else if (shakeDuration > 0)
		{
			transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

			shakeDuration -= Time.deltaTime * dampingSpeed;
		}
		else
		{
			shakeDuration = 0f;
			transform.localPosition = initialPosition;
		}
	}

	public void ApplyDamage(float damage)
	{
		life -= 1;
		shakeDuration = 0.1f;
	}
}
