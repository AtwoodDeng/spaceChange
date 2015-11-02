using UnityEngine;
using System.Collections;

public class ObstacTan : MonoBehaviour {

	[SerializeField] float TanIntense = 1.0f;

	void OnCollisionEnter(Collision col )
	{
		Debug.Log("Collision");
		Character character = col.collider.gameObject.GetComponent<Character>();
		if ( character != null )
		{
			Debug.Log("Tan");
			Vector3 TanTo = col.contacts[0].normal;
			character.SetTanVelocity( TanTo * TanIntense );
		}
	}
	void OnTriggerEnter(Collider col )
	{
		Debug.Log("Trigger ");
		Character character = col.gameObject.GetComponent<Character>();
		if ( character != null )
		{
			Debug.Log("Tan");
			Vector3 TanTo = (col.transform.position - transform.position).normalized;
			character.SetTanVelocity( TanTo * TanIntense );
		}

	}
}
