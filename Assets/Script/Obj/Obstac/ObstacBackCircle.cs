using UnityEngine;
using System.Collections;
using System;

public class ObstacBackCircle : MonoBehaviour {

	[SerializeField] Material material;
	[SerializeField] float ratePos = 1f;
	[SerializeField] float rateScale = 1f;
	[SerializeField] Renderer render;
	[SerializeField] GameObject parItem;
	[SerializeField] GameObject parPers;
	[SerializeField] string level;
	[SerializeField] float fadeTime = 2f;

	// Use this for initialization
	void Awake () {
		if ( material == null )
			material = GetComponent<Material>();
		if ( render == null )
			render = GetComponent<Renderer>();
	}

	void OnEnable()
	{
		BEventManager.Instance.RegisterEvent(EventDefine.EnterStayAway , EnterStayAway );
		BEventManager.Instance.RegisterEvent(EventDefine.ExitStayAway , ExitStayAway );
		BEventManager.Instance.RegisterEvent(EventDefine.LastLevelAfter , LevelAfter );
		BEventManager.Instance.RegisterEvent(EventDefine.NextLevelAfter , LevelAfter );

	}
	
	void OnDisable()
	{
		BEventManager.Instance.UnregisterEvent(EventDefine.EnterStayAway , EnterStayAway );
		BEventManager.Instance.UnregisterEvent(EventDefine.ExitStayAway , ExitStayAway );
		BEventManager.Instance.UnregisterEvent(EventDefine.LastLevelAfter , LevelAfter );
		BEventManager.Instance.UnregisterEvent(EventDefine.NextLevelAfter , LevelAfter );
	}

	
	void LevelAfter(EventDefine eventName, object sender , EventArgs args )
	{
		if ( LogicManager.Instance.tempLevel.level.Equals(level))
		{
			parItem.SetActive(false);
			parPers.SetActive(false);
			StartCoroutine(fadeTo(1f));
		}else
		{
			StartCoroutine( fadeTo( 0 ));
		}
	}

	IEnumerator fadeTo( float alpha)
	{
		float timer = 0;
		float tempAlpha = material.GetFloat("_Alpha");
		while(true)
		{
			timer += Time.deltaTime;
			material.SetFloat("_Alpha" , Mathf.Lerp(tempAlpha,alpha,timer/fadeTime));
			if ( timer > fadeTime )
				yield break;
			yield return null;
		}
	}

	
	void EnterStayAway(EventDefine eventName, object sender , EventArgs args )
	{
		GameObject obj = sender as GameObject;
		parItem.SetActive(false);
		parPers.SetActive(false);
		if ( obj.GetComponent<Person>() != null )
			parPers.SetActive(true);
		if ( obj.GetComponent<Item>() != null )
			parItem.SetActive(true);
	}

	void ExitStayAway(EventDefine eventName, object sender , EventArgs args )
	{
//		parItem.SetActive(false);
//		parPers.SetActive(false);
	}

	
	public void UpdateInfo(Vector3 position , float scale)
	{
		material.SetFloat("_PosX" , position.x * ratePos);
		material.SetFloat("_PosY" , position.y * ratePos);
		material.SetFloat("_Scale" , scale * rateScale );
		if ( scale < 0.12f)
			material.SetFloat("_PosX" , 1f );
	}
}
