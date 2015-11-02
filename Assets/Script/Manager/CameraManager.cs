using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour {

	[SerializeField] protected Character mainCharacter;
	[SerializeField] protected Camera myCamera;
	[SerializeField] float HightSense=1.0f;
	[SerializeField] float closeRate = 0.1f;
	[SerializeField] bool ifMove=true;
	[SerializeField] bool ifAdjustSize=true;

	void Awake()
	{
		Init();
	}

	virtual protected void Init()
	{
		myCamera = GetComponent<Camera>();

	}

	// Update is called once per frame
	void Update () {
		if ( ifMove )
		{
		Vector3 toPos = (mainCharacter!=null)? getDesPosition():transform.position;
		Vector3 myPos = transform.position;

		Vector3 pos = Vector3.zero;
		pos.z = -10f;
		pos.x = Mathf.Lerp( myPos.x , toPos.x , closeRate );
		pos.y = Mathf.Lerp( myPos.y , toPos.y , closeRate );
		transform.position = pos;
		}

		if ( ifAdjustSize &&  mainCharacter != null && mainCharacter.stayAway != null)
//			myCamera.orthographicSize = 5f * ( 1f + ( mainCharacter.stayAway.GetDifficulty() - 1f ) * HightSense );  //HightSense = 0.1f
			myCamera.orthographicSize = 5f * ( 1f + ( mainCharacter.stayAway.GetDistance() ) * HightSense );
	}

	virtual protected Vector3 getDesPosition()
	{
		return mainCharacter.transform.position;
	}

	void OnEnable()
	{
		BEventManager.Instance.RegisterEvent(EventDefine.NextLevelAfter , NextLevelAfter );
		BEventManager.Instance.RegisterEvent(EventDefine.LastLevelAfter , LastLevelAfter );
	}
	
	void OnDisable()
	{
		BEventManager.Instance.UnregisterEvent(EventDefine.NextLevelAfter , NextLevelAfter );
		BEventManager.Instance.UnregisterEvent(EventDefine.LastLevelAfter , LastLevelAfter );
	}
	
	public void NextLevelAfter(EventDefine eventName, object sender , EventArgs args )
	{
		mainCharacter = null;
		StartCoroutine( setMainCharacterAfterSecond(LogicManager.Instance.levelChangeTime));
	}
	
	public void LastLevelAfter(EventDefine eventName, object sender , EventArgs args )
	{
		mainCharacter = null;
		StartCoroutine( setMainCharacterAfterSecond(LogicManager.Instance.levelChangeTime));
	}

	IEnumerator setMainCharacterAfterSecond(float second)
	{
		yield return new  WaitForSeconds(second);
		mainCharacter = LogicManager.Instance.tempLevel.levelCharacter;
		yield break;
	}

}
