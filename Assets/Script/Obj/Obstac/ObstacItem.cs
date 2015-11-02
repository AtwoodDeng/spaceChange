using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacItem : MonoBehaviour {

	[SerializeField] bool spin = false;
	[SerializeField] AnimationCurve spinCurve;
	[SerializeField] float spinFixedRate = 0;
	float oriAngle;
	[SerializeField] bool move = false;
	[SerializeField] AnimationCurve moveCurve;
	[SerializeField] float moveFixedRate = 0;
	[SerializeField] bool shape = false;
	[SerializeField] AnimationCurve shapeCurve;
	[SerializeField] List<tk2dSprite> spriteList = new List<tk2dSprite>();
	[SerializeField] List<Material> materialList = new List<Material>();

	[SerializeField] float T;
	Vector3 oriPosition;
	[SerializeField] MeshCollider myCollider;

	public void Init()
	{
		oriPosition = transform.localPosition;
		oriAngle = transform.rotation.eulerAngles.z;
	}

	
	// Update is called once per frame
	void Update () {
		if ( spin )
		{

			float r = spinCurve.Evaluate(Time.time / T) + spinFixedRate * Time.time ;
			transform.rotation = Quaternion.Euler( 0 , 0 , oriAngle + 180f / Mathf.PI * r);
		}
		if ( move )
		{
			float r = moveCurve.Evaluate(Time.time / T) + moveFixedRate * Time.time;
//			transform.localPosition = oriPosition + new Vector3 ( r * Mathf.Sin( transform.rotation.eulerAngles.z ) 
//			                               , r * Mathf.Cos( transform.rotation.eulerAngles.z ) , 0 );
			transform.localPosition = angle2Position( position2Angle(oriPosition) + r );
		}

		if ( shape )
		{
			//TODO shape the item
			float r = shapeCurve.Evaluate(Time.time / T);
			transform.localScale = Vector3.one * r;
		}

	}

	float position2Angle( Vector3 position )
	{
		float angle =  Vector3.Angle( position , Vector3.right ) / 180f * Mathf.PI;
		if ( position.y > 0 )
			return angle;
		else
			return Mathf.PI * 2 - angle;
	}

	Vector3 angle2Position( float angle )
	{
		float r = oriPosition.magnitude;
		return new Vector3( Mathf.Cos(angle) , Mathf.Sin(angle)) * r;
	}

	public void SetAlpha(float alpha)
	{
		foreach( tk2dSprite sprite in spriteList )
			sprite.color = Global.getAlphaColor(sprite.color , alpha );
		foreach( Material m in materialList )
		{
			m.SetFloat("_Alpha" , alpha );
		}

	}
	public void SetTrigger(bool isTrigger )
	{
		myCollider.convex = true;
		myCollider.isTrigger = isTrigger;
	}
}
