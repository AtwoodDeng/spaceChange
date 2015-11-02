using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Center : MonoBehaviour {

	[System.Serializable]
	public struct statusProcess
	{
		public LogicManager.Process process;
		public bool active;
	}

	[SerializeField] bool isItem = false;
	[SerializeField] bool isPerson = false;
	[SerializeField] string message;
	[SerializeField] StayAway stayAway;
	[SerializeField] LogicManager.Process process;
	[SerializeField] List<statusProcess> formerProcess = new List<statusProcess>();

	void Awake()
	{
		if ( stayAway == null )
			stayAway = transform.parent.GetComponent<StayAway>();
	}

	void OnTriggerEnter( Collider collider )
	{
		if ( !stayAway.level.Equals(LogicManager.Instance.tempLevel.level ) )
			return;
		Character character = collider.GetComponent<Character>();
		if ( character != null )
		{
			Debug.Log("Center Enter" + transform.parent.name );
			MessageEventArgs msg = new MessageEventArgs();
			msg.AddMessage("message" , message );
			if ( isItem )
				msg.AddMessage("type" , Global.ITEM);
			if ( isPerson )
				msg.AddMessage("type" , Global.PERSON );
			msg.AddMessage("process" , process.ToString());
			BEventManager.Instance.PostEvent( EventDefine.OnEnterCenter , this , msg );
		}

	}

	public void Init()
	{
		if ( stayAway == null )
			return;
		for(int i = 0 ; i < formerProcess.Count; ++ i )
		{
			if ( LogicManager.Instance.checkProcess(formerProcess[i].process)!=formerProcess[i].active )
			{
				stayAway.gameObject.SetActive(false);
				return;
			}
		}
		stayAway.gameObject.SetActive(true);
	}
}
