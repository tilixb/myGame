using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    public GameObject tutorialUI; // 教学 UI 的引用
    public TextMeshProUGUI tutorialText; // 教学文本的引用

    public float mouseDeltaThreshold = 1.0f;
    
    private bool interruptCurrentText = false;
    private int tutorialStep = 0; // 当前教学步骤
    private bool tutorialFinished = false; // 教学是否完成

    private string[] tutorialMessages = new string[]
    {
        "Press the right button to raise your right foot." ,
        "Press the right button and move the mouse around the character" ,
        "Press the left button to raise your left foot." ,
        "Press the left button, move the mouse around the character" ,
        "Press the left and right buttons at the same time to slide."
    };

    private void Start()
    {
        DisplayTutorialText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Core"))
        {
            EndTutorial();
        }
    }

    private void Update()
    {
        if (tutorialFinished) return;

        if(!tutorialUI.activeSelf) return;
        bool rightClick = Mouse.current.rightButton.isPressed;
        bool leftClick = Mouse.current.leftButton.isPressed;
        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue(); 
        bool mouseMoved = Mathf.Abs(mouseX) > mouseDeltaThreshold || Mathf.Abs(mouseY) > mouseDeltaThreshold;

        switch (tutorialStep)
        {
            case 0:
                if (rightClick)
                {
                    NextTutorialStep();
                }
                break;
            case 1:
                if (rightClick && mouseMoved)
                {
                    NextTutorialStep();
                }
                break;
            case 2:
                if (leftClick)
                {
                    NextTutorialStep();
                }
                break;
            case 3:
                if (leftClick && mouseMoved)
                {
                    NextTutorialStep();
                }
                break;
            case 4:
                if (leftClick && rightClick)
                {
                    NextTutorialStep();
                }
                break;
            default:
                break;
        }
    }

    private void NextTutorialStep()
    {
        
        interruptCurrentText = true;
        tutorialStep++;
        if (tutorialStep >= tutorialMessages.Length)
        {
            EndTutorial();
        }
        else
        {
            DisplayTutorialText();
        }
    }

    private void DisplayTutorialText()
    {
        interruptCurrentText = false;
        tutorialText.text = "";
        tutorialText.text = tutorialMessages[tutorialStep];
    }

    private void EndTutorial()
    {
        tutorialFinished = true;
        tutorialUI.SetActive(false); // 隐藏教学 UI
    }
}