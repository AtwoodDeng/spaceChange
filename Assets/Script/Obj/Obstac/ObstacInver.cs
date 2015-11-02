using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacInver : Obstac {

	
	override protected float getAlphaByDistance(float distance)
	{
		if ( distance < oriRadius)
			return 0.01f;
		if ( distance > oriRadius * viewRadiusRate )
			return 1f;
		return ( distance - oriRadius) / oriRadius * ( viewRadiusRate - 1f );
	}


	public float GetRange()
	{
		return oriRadius * transform.localScale.x;
	}


}
