using UnityEngine;
using System.Collections;
using System;

public class StayAway : MonoBehaviour {

//	[SerializeField] SphereCollider myCollider;
//	[SerializeField] AnimationCurve stayawayCurve;
                                                  //	[SerializeField] AnimationCurve diffcultyCurve;
	[SerializeField] float enterTime = 0.3f;
	Vector3 oriPos;

	public string level = "0";
//	float oriRadius;

	bool _ifStayAway;
	public bool ifStayAway{
		get{
			return _ifStayAway;
		}
		set{
			_ifStayAway = value;
		}
	}

	void Awake()
	{
		Init();
	}

	protected void Init()
	{
//		if ( myCollider == null )
//			myCollider = GetComponent<SphereCollider>();
		oriPos = transform.localPosition;
//		oriRadius = myCollider.radius;
	}


	void OnEnable()
	{
//		BEventManager.Instance.RegisterEvent(EventDefine.EnterStayAway , EnterStayAway );
		BEventManager.Instance.RegisterEvent(EventDefine.UpdateStayAway , UpdateStayAway );
//		BEventManager.Instance.RegisterEvent(EventDefine.ExitStayAway , ExitStayAway );
		BEventManager.Instance.RegisterEvent(EventDefine.NextLevel , NextLevel );
		BEventManager.Instance.RegisterEvent(EventDefine.LastLevel , LastLevel );
	}

	void OnDisable()
	{
//		BEventManager.Instance.UnregisterEvent(EventDefine.EnterStayAway , EnterStayAway );
		BEventManager.Instance.UnregisterEvent(EventDefine.UpdateStayAway , UpdateStayAway );
//		BEventManager.Instance.UnregisterEvent(EventDefine.ExitStayAway , ExitStayAway );
		BEventManager.Instance.UnregisterEvent(EventDefine.NextLevel , NextLevel );
		BEventManager.Instance.UnregisterEvent(EventDefine.LastLevel , LastLevel );
	}

	
	virtual public void NextLevel(EventDefine eventName, object sender , EventArgs args )
	{
		if ( tempSenserable != null )
			SensableOut();
	}
	
	virtual public void LastLevel(EventDefine eventName, object sender , EventArgs args )
	{
		if  (tempSenserable != null )
			SensableOut();
	}

//	void EnterStayAway(EventDefine eventName, object sender, EventArgs args)
//	{}
//
//	void ExitStayAway(EventDefine eventName, object sender, EventArgs args)
//	{}

	void UpdateStayAway(EventDefine eventName, object sender, EventArgs args)
	{
		MessageEventArgs msg = (MessageEventArgs) args;
		if ( tempSenserable == null ) //other stayAway
		{
			UpdateOtherStayAway(sender, msg);
		}
		else{							//In the stayAway
			UpdateThisStayAway(sender ,msg);
		}
	}

	virtual protected void UpdateOtherStayAway(  object sender , MessageEventArgs msg )
	{
		if ( !msg.GetMessage("Level").Equals(level) )
			return;
		float distance = float.Parse( msg.GetMessage( "AwayDistance" ));
		Vector3 towardCenter = ((GameObject)sender).transform.localPosition - oriPos;
		transform.localPosition = - towardCenter * ( distance - 1 ) + oriPos;
	}

	virtual protected void UpdateThisStayAway(  object sender , MessageEventArgs msg )
	{
		if ( !msg.GetMessage("Level").Equals(level) )
			return;
	}
	
	// Update is called once per frame

	protected Sensable tempSenserable{
		get{
			return _tempSenserable;
		}
		set{
			if ( _tempSenserable != null )
				_tempSenserable.stayAway = null;

			_tempSenserable = value;
			
			if ( _tempSenserable != null )
				_tempSenserable.stayAway = this;
		
		}
	}
	Sensable _tempSenserable;

	float enterProcess
	{
		get{
			return _enterProcess;
		}
		set{
			_enterProcess = value;
		}
	}
	float _enterProcess = 0;

	IEnumerator UpdateEnterProcess()
	{
		yield return new WaitForSeconds(enterTime);
		while( true )
		{
			if ( tempSenserable == null )
				yield break;

//			Vector3 deltaPosition = tempSenserable.transform.position - lastSensablePosition;
//			lastSensablePosition = tempSenserable.transform.position;

			// get the enterProcess
//			enterProcess += GetDeltaEnterProcess( deltaPosition );
			enterProcess = GetEnterProcess();

			MessageEventArgs msg  = new MessageEventArgs();
			msg.AddMessage("AwayDistance" , GetAwayDistance(enterProcess).ToString() );
			msg.AddMessage("CenterPosition" , transform.position.ToString() );
			msg.AddMessage("Level" , level );
			BEventManager.Instance.PostEvent(EventDefine.UpdateStayAway , this.gameObject
			                                 , msg );
//			Debug.Log( "From " + name +  " EnterProcess" + enterProcess.ToString());
			if ( ifEndProcess(enterProcess) ) 
			{
				SensableOut();
				yield break;
			}
			yield return null;
		}
	}

	virtual protected bool ifEndProcess(float enterProcess)
	{
		return enterProcess < 0 ;
	}

	virtual protected float GetAwayDistance(float process )
	{
		return 9999f;
	}

	virtual protected float GetDifficultyDistance(float process)
	{
		return 0;
	}

	public float GetDifficulty()
	{
		return GetAwayDistance(enterProcess);
	}

	virtual public float GetDistance()
	{
		return tempSenserable.transform.localPosition.magnitude;
	}

	virtual protected void SensableOut()
	{
		tempSenserable.transform.parent = LogicManager.Instance.tempLevel.transform;
		tempSenserable = null;
		
		MessageEventArgs msg  = new MessageEventArgs();
		msg.AddMessage("Level" , level );
		BEventManager.Instance.PostEvent(EventDefine.ExitStayAway);
//		enterProcess = -0.01f;
		StopCoroutine( UpdateEnterProcess());
	}

	virtual protected float GetEnterProcess()
	{
		Vector3 toCenter = transform.position - tempSenserable.transform.position;
		return 1f - toCenter.magnitude/2f;
	}


	virtual protected void OnSensableEnter(Sensable _senserable )
	{
		Debug.Log("On Enter");
		tempSenserable = _senserable;
		StartCoroutine( UpdateEnterProcess());
				
		MessageEventArgs msg  = new MessageEventArgs();
		msg.AddMessage("Level" , level );
		BEventManager.Instance.PostEvent(EventDefine.EnterStayAway , this.gameObject , msg );
	}

	virtual protected bool checkSensable( Sensable s )
	{
		if( s == null )
			return false;
		if ( s.stayAway != null )
			return false;
		if ( !s.level.Equals(level))
			return false;
		return true;
	}

}
