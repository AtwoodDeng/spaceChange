using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class Level : MonoBehaviour {

	public Character levelCharacter;
	[SerializeField] string mylevel;
	public string level
	{
		get {return mylevel;}
	}
	public ObstacBackCircle backCircle;
	public Center[] centerList;

	public tk2dSprite showObj;
	
	void OnEnable()
	{
		BEventManager.Instance.RegisterEvent(EventDefine.NextLevelAfter , LevelAfter );
		BEventManager.Instance.RegisterEvent(EventDefine.LastLevelAfter , LevelAfter );
	}
	
	void OnDisable()
	{
		BEventManager.Instance.UnregisterEvent(EventDefine.NextLevelAfter , LevelAfter );
		BEventManager.Instance.UnregisterEvent(EventDefine.LastLevelAfter , LevelAfter );
	}
	
	public void LevelAfter(EventDefine eventName, object sender , EventArgs args )
	{
		Init();
	}

	public void Init()
	{
		foreach(Center c in centerList )
		{
			c.Init();

		}
		if ( LogicManager.Instance.tempLevel.level.Equals(level) && showObj != null )
		{
			
			DOTween.To( () => showObj.color ,
			           c => showObj.color = c ,
			           Global.getAlphaColor(showObj.color , 1f ),
			           2f);
		}
	}
}
