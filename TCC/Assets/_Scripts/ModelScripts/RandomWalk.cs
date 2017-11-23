using UnityEngine;
using UnityEngine.AI;

// Walk to a random position and repeat
[RequireComponent(typeof(NavMeshAgent))]
public class RandomWalk : MonoBehaviour
{
    public float m_Range = 25.0f;
    NavMeshAgent m_agent;
	Vector2 originalPos;
	Vector3 currentDest;

	bool pause;

    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
		originalPos = new Vector2 (transform.position.x, transform.position.z);
    }

    void Update()
    {
        if (pause || m_agent.pathPending || m_agent.remainingDistance > 0.1f)
            return;

		Vector2 circleRand = originalPos + (m_Range * Random.insideUnitCircle);
		Vector3 dest = new Vector3 (circleRand.x, 0, circleRand.y);
		circleRand = dest;

//		GameObject destinationSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere) as GameObject;
//		destinationSphere.transform.position = dest;
//		Destroy (destinationSphere, 2f);

		m_agent.destination = dest;
    }

	public void PauseWalk(bool doPause){
		pause = doPause;
	}
}
