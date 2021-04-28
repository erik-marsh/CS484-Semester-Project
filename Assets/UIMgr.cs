using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMgr : MonoBehaviour
{
	public static UIMgr inst;

	public TMP_Text timerText;
	private float timerStartTime = 0.0f;
	private float timerAlarmPoint = 300.0f; // 5min
	private Color timerAlarmColor = new Color(255, 0, 0);
	private Color timerDefaultColor;

	public CanvasGroup[] canvasGroups; // main menu, instructions, user mode
	private UIState currState;

	private void Awake()
	{
		inst = this;
		timerDefaultColor = timerText.color;

		foreach (CanvasGroup cg in canvasGroups)
		{
			cg.alpha = 0.0f;
		}

		ChangeUIState(UIState.MAIN_MENU);
	}

	private void Update()
	{
		UpdateTimer();
	}

	private void UpdateTimer()
	{
		float currTime = Time.time - timerStartTime;
		int min = (int) (currTime / 60.0f);
		int sec = (int) (currTime % 60.0f);
		string text = min.ToString().PadLeft(2, '0') + ":" + sec.ToString().PadLeft(2, '0');
		timerText.text = text;

		if (currTime > timerAlarmPoint)
		{
			timerText.color = timerAlarmColor;
			// play ding?
		}
	}

	public void ChangeUIState(UIState newState)
	{
		// fade out current UI
		canvasGroups[(int)currState].alpha = 0.0f;

		if (newState == UIState.MAIN_MENU)
		{
			
		}
		else if (newState == UIState.INSTRUCTIONS)
		{
			
		}
		else if (newState == UIState.USER)
		{

		}

		// fade in new state
		canvasGroups[(int)newState].alpha = 1.0f;

		currState = newState;
	}

	// button callbacks
	public void Button_Start()
	{

	}
}

public enum UIState
{
	MAIN_MENU = 0,
	INSTRUCTIONS = 1,
	USER = 2
}
