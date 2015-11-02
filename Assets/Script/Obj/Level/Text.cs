using UnityEngine;
using System.Collections;

public class Text : MonoBehaviour {

	[SerializeField] tk2dTextMesh textMesh;
	[SerializeField] float showTimePerchar = 0.05f;
	[SerializeField] float fadeTime = 2f;
//	[SerializeField] int wordPerSecond = 10;

	bool ifShowing=false;

	void Awake()
	{
		textMesh = GetComponent<tk2dTextMesh>();
	}

	public void ShowText(string str)
	{
		str = str.Replace( "\\r" , "\r\n" );
		if ( !ifShowing)
			StartCoroutine(showTextOneByOne(str));
	}

	IEnumerator showTextOneByOne(string str)
	{
		ifShowing = true;
		float timer = 0 ;
//		bool ifShowedAll = false;
//		int length = 0;
		textMesh.text = str;
		textMesh.color = Global.getAlphaColor(textMesh.color , 0f );
//		int counter = 0;
		while(true)
		{
//			counter ++;
//			if ( !ifShowedAll && (counter % ( 60 / wordPerSecond ) == 0 ) )
//			{
//				textMesh.text = str.Substring(0,length);
//				length ++;
//			}else
//			{
				timer += Time.deltaTime;
//			}
//			if ( length >= str.Length )
//				ifShowedAll = true;

			float showTime = showTimePerchar * str.Length;

			if ( timer < fadeTime )
			{
				textMesh.color = Global.getAlphaColor(textMesh.color 
				                                     , timer / fadeTime );
			}

			if ( timer > showTime + fadeTime )
			{
				textMesh.color = Global.getAlphaColor(textMesh.color 
				                                      , 1f - ( timer - showTime - fadeTime ) / fadeTime );
			}

			if ( timer > showTime + fadeTime * 2 )
			{
				ifShowing = false; 
				yield break;
			}
			yield return null;
		}

	}
}
