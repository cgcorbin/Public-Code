using UnityEngine;
using System.Collections;
using cgc.DataStructures;

public class CapsuleController : MonoBehaviour 
{
	iFSM fsm = new iFSM();
	bool up = true;
	float moveSpeed = 0.05f;

	// Use this for initialization
	void Start () 
	{
		fsm.AddAction(MoveUpAndDown);
		fsm.AddState("spinLeftState", SpinLeft);
		fsm.AddState("spinRightState", SpinRight);
	}
	
	// Update is called once per frame
	void Update () 
	{
		fsm.Run();
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 100, 50), "Spin Left"))
		{
			fsm.ChangeState("spinLeftState");
		}

		if (GUI.Button(new Rect(10, 70, 100, 20), "Spin Right"))
		{
			fsm.ChangeState("spinRightState");
		}
	}

	void MoveUpAndDown(object sender)
	{
		if (up)
		{
			if (this.gameObject.transform.position.y < 2.5f)
			{
				this.gameObject.transform.position += new Vector3(0f, moveSpeed, 0f);
			}
			else
			{
				this.up = false;
			}
		}
		else
		{
			if (this.gameObject.transform.position.y > 0.6f)
			{
				this.gameObject.transform.position -= new Vector3(0f, moveSpeed, 0f);
			}
			else
			{
				this.up = true;
			}
		}
	}

	void SpinLeft(object sender)
	{
		this.gameObject.transform.Rotate(new Vector3(0, 0, 1), moveSpeed*50);
	}

	void SpinRight(object sender)
	{
		this.gameObject.transform.Rotate(new Vector3(0, 0, 1), -moveSpeed*50);
	}
}
