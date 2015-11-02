using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Person : StayAway {
	
	[SerializeField] Obstac obstacle;
//	[SerializeField] float difficulty;
	[SerializeField] tk2dSprite sprite;
	[SerializeField] SphereCollider myCollider;
	[SerializeField] AnimationCurve obstacleResizeCurve;
	[SerializeField] AnimationCurve stayawayCurve;
	float oriRadius;

	// Use this for initialization
	void Awake () {
		base.Init();
		myCollider = GetComponent<SphereCollider>();
		oriRadius = myCollider.radius;
		obstacle.level = level; 
	}
	
	// Update is called once per frame

	
//	void OnTriggerEnter(Collider collider)
//	{
////		Debug.Log("TriggerEnter"+collider.name);
////		if ( checkSensable( collider.GetComponent<Sensable>() ))
////			OnSensableEnter(collider.GetComponent<Sensable>());
////		if ( collider.GetComponent<Sensable>() != null )
////		{
////			Sensable senserable = collider.GetComponent<Sensable>();
////			if ( senserable.stayAway == null )
////			{
////				OnSensableEnter( senserable );
////			}
////		}
//	}

//	bool checkSensable( Sensable s )
//	{
//		if( s == null )
//			return false;
//		if ( !(s.stayAway == null))
//			return false;
//		if ( !s.level.Equals(level))
//			return false;
//		return true;
//	}
	

//	void OnTriggerStay(Collider collider)
//	{
//		if ( checkSensable( collider.GetComponent<Sensable>() ))
//			OnSensableEnter(collider.GetComponent<Sensable>());
//	}

//	void OnTriggerExit(Collider collider)
//	{
////		Debug.Log("TriggerExit"+collider.name);
////		if ( collider.GetComponent<Sensable>() != null && collider.GetComponent<Sensable>().stayAway == this )
////		{
////			SensableOut();
////			collider.gameObject.transform.SetParent(null);
////		}
//
//	}

	
	virtual protected void OnTriggerEnter(Collider collider)
	{
		if ( checkSensable( collider.GetComponent<Sensable>() ) && enabled )
		{
			OnSensableEnter(collider.GetComponent<Sensable>());
		}
	}

	virtual protected void OnTriggerStay(Collider collider)
	{
		if ( checkSensable( collider.GetComponent<Sensable>() ) && enabled)
		{
			OnSensableEnter(collider.GetComponent<Sensable>());
		}
	}

	protected override void UpdateThisStayAway ( object sender , MessageEventArgs msg)
	{
		base.UpdateThisStayAway (sender , msg);
//		float distance = float.Parse( msg.GetMessage( "AwayDistance" ));
		float resizeObstacle = GetResizeObstacle();
		obstacle.UpdateObstacle( resizeObstacle );
		myCollider.radius = oriRadius * resizeObstacle;
		tempSenserable.UpdateInObstac(resizeObstacle );
		LogicManager.Instance.UpdateBGM( GetResizeObstacle() );

	}

	float GetResizeObstacle()
	{
		return GetAdjustByDifficulty( obstacleResizeCurve.Evaluate(GetEnterProcess()) );
	}

//	protected override float GetDifficultyDistance (float process)
//	{
//		return ( ( base.GetDifficultyDistance(process) - 1f ) * difficulty + 1f );
//	}

	protected override float GetEnterProcess ()
	{
		Vector3 toCenter = obstacle.transform.position - tempSenserable.transform.position ;
		return 1.01f - ( toCenter.magnitude - tempSenserable.GetRadius() ) /  myCollider.radius;
	}

	protected override void OnSensableEnter (Sensable _senserable)
	{
		base.OnSensableEnter (_senserable);
		InitEnter(_senserable);
	}

	void InitEnter(Sensable _senserable)
	{
		tempSenserable = _senserable;
		float resizeObstacle = GetResizeObstacle();
		obstacle.UpdateObstacle( resizeObstacle );
		myCollider.radius = oriRadius * resizeObstacle;
		obstacle.SetupObstacItems( sprite.scale.x  );
		_senserable.transform.parent = obstacle.transform;


	}

	protected override void SensableOut ()
	{
		base.SensableOut ();
		obstacle.End();
		LogicManager.Instance.UpdateBGM( 1f  );
	}

	public override float GetDistance ()
	{
		return (obstacle.transform.position - tempSenserable.transform.position ).magnitude - oriRadius;
	}

	protected override float GetAwayDistance (float process)
	{
		return GetAdjustByDifficulty( stayawayCurve.Evaluate(process) );
	}

	float GetAdjustByDifficulty( float value )
	{
		return ( value - 1 ) * ( LogicManager.Instance.difficulty + 0.3f) *.33f + 1f;
	}
}
