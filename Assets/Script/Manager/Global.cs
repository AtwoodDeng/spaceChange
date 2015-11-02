using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public static class Global {

	public static int SECOND_TO_TICKS = 10000000;

	public static Color getAlphaColor( Color col , float a )
	{
		Color _c = col;
		_c.a = a;
		return _c;
	}

	public static string ITEM = "[Item]";
	public static string PERSON = "[Person]";

//	public static void showSpirte( tk2dSprite sprite , float time , float fromAlpha , float toAlpha )
//	{
//		sprite.color = Global.getAlphaColor( sprite.color , fromAlpha );
//		DOTween.To( () => sprite.color ,
//		             c => sprite.color = c ,
//		           Global.getAlphaColor(sprite.color,toAlpha),
//		           time );
//	}
}
