using UnityEngine;
using System.Collections;

public class SubCameraManager : CameraManager {

	[SerializeField] float followingRate = 1.0f;
	Vector3 lastCharacterPosition;



	protected override Vector3 getDesPosition ()
	{
		Vector3 des = ( mainCharacter.transform.position - lastCharacterPosition ) * followingRate + transform.position;
		lastCharacterPosition = mainCharacter.transform.position;
		return des;
	}

	protected override void Init ()
	{
		base.Init();
		lastCharacterPosition = mainCharacter.transform.position;
	}
}
