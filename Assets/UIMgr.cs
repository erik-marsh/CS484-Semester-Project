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
	private List<List<BoxCollider>> menuColliders = new List<List<BoxCollider>>(); // nightmarish
	public List<BoxCollider> mainMenuColliders = new List<BoxCollider>();
	public List<BoxCollider> instructionsColliders = new List<BoxCollider>();
	public List<BoxCollider> userColliders = new List<BoxCollider>();
	private UIState currState;

	private bool isFadingInUserUI = false;
	private bool isFadingOutUserUI = false;
	private bool isFadingInMainMenu = false;
	private bool isFadingOutMainMenu = false;
	private float userUIFadeInTimer = 1.0f;
	private float userUIFadeOutTimer = 1.0f;
	private float mainMenuFadeInTimer = 1.0f;
	private float mainMenuFadeOutTimer = 1.0f;
	public float userUIFadeLength = 1.0f;
	public float transitionFadeLength = 1.0f;
	private float currUserUIAlpha = 0.0f;

	private void Awake()
	{
		inst = this;
		timerDefaultColor = timerText.color;

		foreach (CanvasGroup cg in canvasGroups)
		{
			cg.alpha = 0.0f;
		}

		menuColliders.Add(mainMenuColliders);
		menuColliders.Add(instructionsColliders);
		menuColliders.Add(userColliders);

		ChangeUIState(UIState.MAIN_MENU);
	}

	private void Update()
	{
		UpdateTimer();
		FadeComponents();
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

		//if (newState == UIState.MAIN_MENU)
		//{

		//}
		//else if (newState == UIState.INSTRUCTIONS)
		//{
			
		//}
		//else if (newState == UIState.USER)
		//{

		//}

		// activate the proper colliders with some over-engineered loop
		for (int i = 0; i < menuColliders.Count; i++)
		{
			foreach (BoxCollider bc in menuColliders[i])
			{
				bc.enabled = (i == (int) newState);
			}
		}

		// fade in new state if necessary
		if (newState != UIState.USER)
		{
			canvasGroups[(int)newState].alpha = 1.0f;
		}

        // fade in/out main menu
        if (currState == UIState.MAIN_MENU && newState == UIState.USER)
        {
            isFadingOutMainMenu = true;
            isFadingInMainMenu = false;
            mainMenuFadeOutTimer = 0.0f;
        }
        else if (currState == UIState.USER && newState == UIState.MAIN_MENU)
        {
            isFadingOutMainMenu = false;
            isFadingInMainMenu = true;
            mainMenuFadeInTimer = 0.0f;
        }

        currState = newState;
	}

	private void FadeComponents()
    {
		if (isFadingInUserUI && userUIFadeInTimer < userUIFadeLength)
        {
			var userUI = canvasGroups[2];
			userUIFadeInTimer += Time.deltaTime;
			userUI.alpha = Mathf.Lerp(currUserUIAlpha, 1.0f, userUIFadeInTimer / userUIFadeLength);
        }

		if (isFadingOutUserUI && userUIFadeOutTimer < userUIFadeLength)
        {
			var userUI = canvasGroups[2];
			userUIFadeOutTimer += Time.deltaTime;
			userUI.alpha = Mathf.Lerp(currUserUIAlpha, 0.0f, userUIFadeOutTimer / userUIFadeLength);
		}

        if (isFadingOutMainMenu && mainMenuFadeOutTimer < transitionFadeLength)
        {
            var mainMenu = canvasGroups[0];
            mainMenuFadeOutTimer += Time.deltaTime;
            mainMenu.alpha = Mathf.Lerp(1.0f, 0.0f, mainMenuFadeOutTimer / transitionFadeLength);
        }

        if (isFadingInMainMenu && mainMenuFadeInTimer < transitionFadeLength)
        {
            var mainMenu = canvasGroups[0];
            mainMenuFadeInTimer += Time.deltaTime;
            mainMenu.alpha = Mathf.Lerp(0.0f, 1.0f, mainMenuFadeInTimer / transitionFadeLength);
        }
    }

	public void StartFadingInUserUI()
    {
		isFadingInUserUI = true;
		isFadingOutUserUI = false;
        userUIFadeInTimer = 0.0f;
		currUserUIAlpha = canvasGroups[2].alpha;
    }

	public void StartFadingOutUserUI()
    {
		isFadingInUserUI = false;
		isFadingOutUserUI = true;
		userUIFadeOutTimer = 0.0f;
		currUserUIAlpha = canvasGroups[2].alpha;
	}
}

public enum UIState
{
	MAIN_MENU = 0,
	INSTRUCTIONS = 1,
	USER = 2
}
