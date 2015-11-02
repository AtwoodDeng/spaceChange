using UnityEngine;
using System.Collections;
using System;

public class Sensable : MonoBehaviour  {

	public StayAway stayAway
	{
		get{
			return _stayAway;
		}
		set{
			_stayAway = value;
		}
	}
	public string level="0";
	StayAway _stayAway=null;
	
//	[SerializeField] float diffcultyDragRate = 50f;
	
	void OnEnable()
	{
//		BEventManager.Instance.RegisterEvent(EventDefine.UpdateStayAway , UpdateStayAway );
		BEventManager.Instance.RegisterEvent(EventDefine.ExitStayAway , ExitStayAway );
	}
	
	void OnDisable()
	{
//		BEventManager.Instance.UnregisterEvent(EventDefine.UpdateStayAway , UpdateStayAway );
		BEventManager.Instance.UnregisterEvent(EventDefine.ExitStayAway , ExitStayAway );
	}
	
	protected void ExitStayAway(EventDefine eventName, object sender, EventArgs args)
	{
		stayAway = null;
	}
	
//	protected void UpdateStayAway(EventDefine eventName, object sender, EventArgs args)
//	{
//		if ( stayAway == null )
//			return;
////
//	}

	virtual public void UpdateInObstac( float resize )
	{

	}

	virtual public float GetRadius()
	{
		return 0;
	}
}
