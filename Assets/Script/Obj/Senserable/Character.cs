using UnityEngine;
using System.Collections;

public class Character : Sensable {
	
	[SerializeField] Rigidbody rigidBody;
	[SerializeField] tk2dSprite sprite;
	
	[SerializeField] float forceIntense = 0.05f;
	[SerializeField] SphereCollider myCollider;
	public Vector3 extraVelocity=Vector3.zero;

	float resize = 1.0f;
	[SerializeField] float TanDrag = 0.1f;
	// Use this for initializatio

	Vector3 velocity = Vector3.zero;
	Vector3 TanVelocity= Vector3.zero;

	void Awake()
	{
		if ( myCollider == null )
			myCollider = GetComponent<SphereCollider>();
	}

	// Update is called once per frame
	void Update () {
		
		//set scale 1
		if ( transform.parent != null && stayAway != null )
		{

			float enlargeSize = 1 / transform.parent.localScale.x;
			transform.localScale = Vector3.one * enlargeSize;
		}

		if ( LogicManager.Instance != null && !LogicManager.Instance.tempLevel.level.Equals(level))
			return;
		//decide the direction
		float x_direction = 0;
		float y_direction = 0;
		
		if ( Input.GetKey(KeyCode.UpArrow))
			y_direction = 1f;
		if ( Input.GetKey(KeyCode.DownArrow))
			y_direction = -1f;
		if ( Input.GetKey(KeyCode.RightArrow))
			x_direction = 1f;
		if ( Input.GetKey(KeyCode.LeftArrow))
			x_direction = -1f;
		
		//force the obj

		velocity = new Vector3(x_direction,y_direction,0) * forceIntense + extraVelocity + TanVelocity;
		TanVelocity = TanVelocity * ( 1 - TanDrag );
		extraVelocity = Vector3.zero;

		if ( stayAway == null )
		{
			MoveCharacterNormal(velocity);
		}
		else
		{
			MoveCharacterInObsta( velocity , resize );
		}

		if ( level.Equals("99") && Input.GetKeyDown(KeyCode.Space))
			Application.LoadLevel(1);
	}

	public void MoveCharacterNormal(Vector3 velocity )
	{
		transform.position += velocity/60f;
	}

	public void MoveCharacterInObsta(Vector3 velocity , float rate)
	{
		transform.position += rate * velocity/60f;
	}

		
	public override void UpdateInObstac (float _resize)
	{
		resize = ( _resize - 1 ) / 5f + 1;
	}

	public override float GetRadius ()
	{
		if ( myCollider != null )
			return myCollider.radius;
		return 0;
	}

	public void SetTanVelocity(Vector3 v )
	{
		TanVelocity = v;
	}
}
