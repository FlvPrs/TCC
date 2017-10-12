using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFly : MonoBehaviour {

	public float m_Range = 25.0f;
	Transform m_npc;
	Vector3 originalPos;
	Vector3 currentDest;

	float remainingDistance = 0f;


	//Placeholder pra animação de voo
	float maxHeight = 7f;
	Rigidbody rb;


	void Start()
	{
		m_npc = GetComponent<Transform>();
		originalPos = m_npc.position;
		currentDest = Vector3.zero;
		rb = GetComponent < Rigidbody> ();
		Vector3 initialHeight = rb.velocity;
		initialHeight.y = Random.Range (-maxHeight, maxHeight);
		rb.velocity = initialHeight;

	}

	void Update()
	{
		if(currentDest != Vector3.zero)
			remainingDistance = Vector3.Distance (currentDest, m_npc.position);


		//Placeholder para a animação de voo
		if(rb.velocity.y < -maxHeight)
			rb.velocity = -rb.velocity;
		

		if (remainingDistance > 0.1f) {
			Vector3 pos = Vector3.MoveTowards(m_npc.position, currentDest, 2 * Time.deltaTime);
			m_npc.position = pos;
			return;
		}

		Vector3 dest = originalPos + (m_Range * Random.insideUnitSphere);

//		GameObject destinationSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere) as GameObject;
//		destinationSphere.transform.position = dest;
//		Destroy (destinationSphere, 2f);

		currentDest = dest;
	}
}
