using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class LogicManager : MonoBehaviour {

	public enum Process
	{
		MotherI,
		FirstFriend,
		ColorPencil,
		Father,
		MotherII,
		Book,
		QuietGirl,
		SecondFriend,
		Computer,
		ThirdFriend,
		MotherIII,
		CarAccident,
		ComputerCompetition,
		Bridge,
		EndGame,
	}
	public enum EndGameType
	{
		ArtistEnding,
		ProgrammerEnding,
		DeadEnding,
		NormalEnding,
		BadEnding,
	}

	public List<Level> levelList;

	int tempLevelIndex=0;
	public float levelChangeTime =2f;
	public float levelRate=10f;

	[SerializeField] Text text;           

	bool[] processList=new bool[30];
	public List<tk2dSprite> endGameSpriteList = new List<tk2dSprite>();
	public tk2dTextMesh endGameText;

	public int difficulty = 0; 
	int tempDifficulty = 0;

	public AudioSource bgm;

	public Level tempLevel
	{
		get{
			return levelList[tempLevelIndex];
		}
	}

	Level lastestLevel
	{
		get{
			return levelList[levelList.Count-1];
		}
	}

	[SerializeField] List<float> levelTimeList = new List<float>();

	void Awake()
	{
		BEventManager.Instance.PostEvent(EventDefine.NextLevelAfter);
		for(int i = 0 ; i < processList.Length; ++i )
			processList[i] = false;
		StartCoroutine(UpdateControlLevel());
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Space ))
			BEventManager.Instance.PostEvent(EventDefine.NextLevel);
//		if ( Input.GetKeyDown( KeyCode.B ))
//			BEventManager.Instance.PostEvent(EventDefine.LastLevel);
		if ( Input.GetKeyDown( KeyCode.R ))
			Application.LoadLevel(1);
		if (Input.GetKeyDown( KeyCode.Escape ))
			Application.Quit();
	}


	void OnEnable()
	{
		BEventManager.Instance.RegisterEvent(EventDefine.NextLevel , NextLevel );
		BEventManager.Instance.RegisterEvent(EventDefine.LastLevel , LastLevel );
		BEventManager.Instance.RegisterEvent(EventDefine.OnEnterCenter , OnEnterCenter );
		BEventManager.Instance.RegisterEvent(EventDefine.ExitStayAway , ExitStayAway );
	}
	
	void OnDisable()
	{
		BEventManager.Instance.UnregisterEvent(EventDefine.NextLevel , NextLevel );
		BEventManager.Instance.UnregisterEvent(EventDefine.LastLevel , LastLevel );
		BEventManager.Instance.UnregisterEvent(EventDefine.OnEnterCenter , OnEnterCenter );
		BEventManager.Instance.UnregisterEvent(EventDefine.ExitStayAway , ExitStayAway );
	}

	
	public void ExitStayAway(EventDefine eventName, object sender , EventArgs args )
	{
		if ( tempDifficulty > 0 )
			difficulty += tempDifficulty;
		tempDifficulty = 0;
	}
	
	public void OnEnterCenter(EventDefine eventName, object sender , EventArgs args )
	{
		MessageEventArgs msg = (MessageEventArgs)args;
		string message = msg.GetMessage("message");
		string type = msg.GetMessage("type");
		if ( type != null && type.Equals(Global.PERSON) )
			tempDifficulty ++;
		Process p = (Process)Enum.Parse( typeof( Process ) , msg.GetMessage("process"));

		if ( SetProcess(p) )
			text.ShowText(message);
	}

	bool SetProcess(Process p )
	{
		if ( p == Process.EndGame )
		{
			EndGame();
			return false;
		}
		processList[(int)p] = true;
		return true;
	}

	void EndGame()
	{
		EndGameType eg = EndGameType.NormalEnding;
		if ( processList[(int)Process.CarAccident] )
			eg = EndGameType.ArtistEnding;
		if ( processList[(int)Process.Bridge] )
			eg = EndGameType.DeadEnding;
		if ( countHuman() > 2 )
			eg = EndGameType.BadEnding;


		endGameSpriteList[(int)eg].color = 
		Global.getAlphaColor(endGameSpriteList[(int)eg].color , 0 );

		DOTween.To( () => endGameSpriteList[(int)eg].color ,
			           c => endGameSpriteList[(int)eg].color = c ,
			           Global.getAlphaColor(endGameSpriteList[(int)eg].color , 1f ),
			           2f);
		
		DOTween.To( () => endGameText.color ,
		           c => endGameText.color = c ,
		           Global.getAlphaColor(endGameText.color , 1f ),
		           2f);
	}

	int countHuman()
	{
		int sum = 0;
		if ( processList[(int)Process.MotherI] )
			sum ++;
		if ( processList[(int)Process.MotherII] )
			sum ++;
		if ( processList[(int)Process.MotherIII] )
			sum ++;
		if ( processList[(int)Process.Father] )
			sum ++;
		if ( processList[(int)Process.QuietGirl] )
			sum ++;
		if ( processList[(int)Process.FirstFriend] )
			sum ++;
		if ( processList[(int)Process.ThirdFriend] )
			sum ++;
		return sum;
	}


	public void NextLevel(EventDefine eventName, object sender , EventArgs args )
	{
		if ( tempLevelIndex >= levelList.Count-1 )
			return;

		lastestLevel.transform.DOScale( lastestLevel.transform.localScale / levelRate , levelChangeTime );
		tempLevelIndex++;
		BEventManager.Instance.PostEvent(EventDefine.NextLevelAfter);
	}

	public void LastLevel(EventDefine eventName, object sender , EventArgs args )
	{
		if ( tempLevelIndex <= 0 )
			return;
		
		lastestLevel.transform.DOScale( lastestLevel.transform.localScale * levelRate , levelChangeTime );
		tempLevelIndex --;
		BEventManager.Instance.PostEvent(EventDefine.LastLevelAfter);
	}

	public bool checkProcess(Process p)
	{
		if ( processList.Length > (int)p)
			return processList[(int)p];
		return false;
	}
	                      
	public void UpdateBGM(float resize )
	{
		bgm.pitch = 1f + ( resize - 1f ) * 0.2f;
	}

	IEnumerator UpdateControlLevel()
	{
		float timer = 0 ;
		int index = 0;
		while(true)
		{
			if ( index >= levelTimeList.Count )
				yield break;
			timer += Time.deltaTime;
			if ( timer > levelTimeList[index]) 
			{
				BEventManager.Instance.PostEvent(EventDefine.NextLevel);
				timer = 0;
			}
			yield return null;
		}
	}
	
	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;
}
