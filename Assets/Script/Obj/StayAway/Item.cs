using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Item : StayAway {
	
	[SerializeField] ObstacInver obstacle;
//	[SerializeField] float difficulty;
	[SerializeField] tk2dSprite sprite;
	[SerializeField] SphereCollider myCollider;
	[SerializeField] AnimationCurve obstacleResizeCurve;
	[SerializeField] float initEnterProcess = 0.3f;
	[SerializeField] float initEnterTime = 2f;
	[SerializeField] float gettingInForceIntense = 0.1f;
	[SerializeField] AnimationCurve stayawayCurve;
//	bool ifIn = false;
	float oriRadius;

	// Use this for initialization
	void Awake () {
		base.Init();
		myCollider = GetComponent<SphereCollider>();
		oriRadius = myCollider.radius;
		obstacle.level = level;
	}
	
	// Update is called once per frame
	

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
//	
//	
//	void OnTriggerStay(Collider collider)
//	{
//		if ( checkSensable( collider.GetComponent<Sensable>() ))
//			OnSensableEnter(collider.GetComponent<Sensable>());
//	}

	
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
		float resizeObstacle = GetResizeObstacle();
		obstacle.UpdateObstacle( resizeObstacle );
		tempSenserable.UpdateInObstac(resizeObstacle );
		myCollider.radius = resizeObstacle * oriRadius;
		LogicManager.Instance.UpdateBGM( 1f / GetResizeObstacle() );
//		myCollider.radius = 0;
		
	}
	
	float GetResizeObstacle()
	{
		return GetAdjustByDifficulty ( obstacleResizeCurve.Evaluate(GetEnterProcess()));
	}


	void InitEnter(Sensable _senserable)
	{
		obstacle.SetupObstacItems( sprite.scale.x );
		if ( _senserable == null )
			Debug.Log("null _sens");
		if ( tempSenserable == null )
			Debug.Log("null temp ");
		if ( obstacle == null )
			Debug.Log("null obstac");
		tempSenserable = _senserable;
		float resizeObstacle = GetResizeObstacle();
		obstacle.UpdateObstacle( resizeObstacle );
		myCollider.radius = oriRadius * resizeObstacle;
		tempSenserable.UpdateInObstac( resizeObstacle );
		_senserable.transform.parent = obstacle.transform;
//		myCollider.radius = 0;

		StartCoroutine(GettingIn());

	}
	
//	protected override float GetDifficultyDistance (float process)
//	{
//		return ( ( base.GetDifficultyDistance(process) - 1f ) * difficulty + 1f );
//	}
	
	protected override float GetEnterProcess ()
	{
		Vector3 toCenter = obstacle.transform.position - tempSenserable.transform.position ;
//		return (toCenter.magnitude - tempSenserable.GetRadius()) / obstacle.GetRange() ;
		Debug.Log("toCenter"+toCenter.ToString());
		Debug.Log("ToCenterM"+toCenter.magnitude.ToString()+" tempSR"+tempSenserable.GetRadius().ToString() + " myCoR" +myCollider.radius.ToString());
		return ( toCenter.magnitude - tempSenserable.GetRadius()) / myCollider.radius;
	}

	protected override bool ifEndProcess (float enterProcess)
	{
		return GetEnterProcess() > 1.2f;
	}
	
	protected override void OnSensableEnter (Sensable _senserable)
	{
		base.OnSensableEnter (_senserable);
		InitEnter(_senserable);
	}
	
	protected override void SensableOut ()
	{
		base.SensableOut ();
		obstacle.End();
		LogicManager.Instance.UpdateBGM(1f);
//		DOTween.To( r => myCollider.radius = r , 0 , oriRadius , 3f ).SetDelay(3f);
	}

	IEnumerator GettingIn()
	{
		float timer = 0;
		obstacle.setTrigger(false);
		while(true)
		{
			timer += Time.deltaTime;
			if ( tempSenserable == null )
			{
				break;
			}

			if ( GetEnterProcess() < initEnterProcess )
			{
//				ifIn = true;
				break;
			}
			if ( timer > initEnterTime )
			{
				break;
			}
			Character character = (Character)tempSenserable;
			character.extraVelocity = - character.transform.localPosition * gettingInForceIntense;
//			Debug.Log("Getting in " + character.extraVelocity.ToString());
			yield return null;
		}
		obstacle.setTrigger(true);
		yield break;
	}
	
	protected override float GetAwayDistance (float process)
	{
		return GetAdjustByDifficulty( stayawayCurve.Evaluate(process));
	}

	
	float GetAdjustByDifficulty( float value )
	{
		return ( value - 1 ) * ( LogicManager.Instance.difficulty + 0.3f) *.33f + 1f;
	}
}
