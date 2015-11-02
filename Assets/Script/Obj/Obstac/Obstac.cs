using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Obstac : MonoBehaviour {

	[SerializeField]protected  float IncreaseRate = 1.0f;
	[SerializeField] public string level;
	[SerializeField] List<GameObject> ObstacPrefab = new List<GameObject>();
	[SerializeField] int ObstacLevel= 3;
	[SerializeField] int ObstacNumMin = 3;
	[SerializeField] int ObstacNumMax = 6;
	[SerializeField] AnimationCurve scaleCurve;
	[SerializeField] float setUpInver = 1f;
	protected float oriRadius = 0.5f;
	[SerializeField]protected  float viewRadiusRate=2f;
	List<ObstacItem> obstacItems = new List<ObstacItem>();
	float setUpTimer = 0;
//	[SerializeField] float standardEmit = 2f;
	[SerializeField] ObstacBackCircle backCircle{
		get{
			if ( _backCircle == null && level.Equals(LogicManager.Instance.tempLevel.level) )
			{
				_backCircle = LogicManager.Instance.tempLevel.backCircle;
			}
			return _backCircle;
		}
	}
	ObstacBackCircle _backCircle = null;

	void Awake()
	{
		if ( backCircle != null )
			backCircle.UpdateInfo(Vector3.zero , 0 );
	}

	public void UpdateObstacle(float distance)
	{
		setUpTimer += Time.deltaTime;
		transform.localScale = Vector3.one * ( ( distance - 1f ) * IncreaseRate + 1f );

		foreach(ObstacItem obsItem in obstacItems )
		{
			obsItem.SetAlpha( getAlphaByDistance((obsItem.transform.position-transform.position).magnitude) );
		}

		if ( backCircle != null )
			backCircle.UpdateInfo(this.transform.position , transform.localScale.x - 1f );
	}

	protected virtual float getAlphaByDistance(float distance)
	{
		if ( distance < oriRadius)
			return 0.01f;
		if ( distance > oriRadius * viewRadiusRate )
			return 1f;
		return ( distance - oriRadius) / oriRadius * ( viewRadiusRate - 1f );
	}

	public void SetupObstacItems(float radius = 0.25f )
	{	
		oriRadius = radius;

		//level related to diff
		ObstacLevel = LogicManager.Instance.difficulty;

		if ( setUpTimer < setUpInver && obstacItems.Count > 0 )
			return;
		setUpTimer = 0;

		while(obstacItems.Count > 0 )
		{
			ObstacItem tempOItem = obstacItems[0];
			obstacItems.Remove(tempOItem);
			GameObject.Destroy(tempOItem.gameObject);
		}

		for ( int i = 1 ; i <= ObstacLevel ; ++ i )
		{
			GameObject prefab = ObstacPrefab[UnityEngine.Random.Range( 0 , ObstacPrefab.Count )];
			int obstaNum = UnityEngine.Random.Range(ObstacNumMin , ObstacNumMax+1);
			float r = radius / ObstacLevel * i * UnityEngine.Random.Range(0.9f,1.1f);
			float size = scaleCurve.Evaluate( 1.0f * i / ObstacLevel );
			SetUpOneLevel(prefab,obstaNum,r,size);
		}

	}

	public void SetUpOneLevel(GameObject prefab, int obsNumber, float radius , float size )
	{
		float initAngle = UnityEngine.Random.Range(0,2f * Mathf.PI  );
		for( int i = 0 ; i < obsNumber ; ++ i )
		{
			GameObject obj = Instantiate( prefab ) as GameObject;
			float angle = 2f * Mathf.PI * i / obsNumber + initAngle; 
			float x = Mathf.Sin(angle) * radius;
			float y = - Mathf.Cos(angle) * radius;
			obj.transform.parent = this.transform;
			obj.transform.localPosition = new  Vector3( x , y , 0 );
			obj.transform.Rotate( new Vector3( 0 , 0 , angle * 180f / Mathf.PI ) );
			obj.transform.localScale = Vector3.one * size;
			ObstacItem obsItem = obj.GetComponent<ObstacItem>();
			obsItem.Init();
			obsItem.SetAlpha( 0 );
			obstacItems.Add(obsItem);
		}
	}

	public void End()
	{
		transform.localScale = Vector3.one;
		foreach(ObstacItem item in obstacItems )
			item.SetAlpha(0);
	}

	public void setTrigger(bool isTrigger )
	{
		foreach(ObstacItem item in obstacItems )
			item.SetTrigger(isTrigger);
	}

}
