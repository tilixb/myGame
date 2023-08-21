using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ButtonSequenceController:MonoBehaviour
{
    public enum ButtonType
    {
        Circle,
        Square,
        Triangle,
        Hexagon,
    }
    private CinemachineBrain cmBrain;

    private void Start()
    {
        cmBrain = FindObjectOfType<CinemachineBrain>();
    }
    public CinemachineVirtualCamera farCamera;
    
    private Stack<ButtonType> buttonSequence = new Stack<ButtonType>();
    public ButtonType[] correctSequence;
    public SpriteRenderer[] spriteRenderers;
    //color
    public float colorDuration = 0.2f;
    
    private IEnumerator ColorAnimation(int index)
    {
        float elapsedTime = 0;
        elapsedTime = 0;
        while (elapsedTime < colorDuration)
        {
            // Color animation
            spriteRenderers[index].color = Color.Lerp(spriteRenderers[index].color, Color.yellow, elapsedTime / colorDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderers[index].color=Color.yellow;
    }    
    private IEnumerator FadeColorAnimation(int index)
    {
        float elapsedTime = 0;
        elapsedTime = 0;
        while (elapsedTime < colorDuration)
        {
            // Color animation
            spriteRenderers[index].color = Color.Lerp(spriteRenderers[index].color, Color.white, elapsedTime / colorDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderers[index].color=Color.white;
    }
    
    
    
    public void ProcessButtonPress(ButtonType pressedButtonType)
    {
        buttonSequence.Push(pressedButtonType);

        if (IsSequenceCorrect())
        {
            //每个单独的，就调用相应的引用即可
            StartCoroutine(ColorAnimation(buttonSequence.Count-1));
            
            
            // Send a message to other GameObjects
            // For example: someGameObject.SendMessage("OnSequenceCompleted");
            if(buttonSequence.Count==correctSequence.Length)
            {
                EventManager.Instance.CallSequenceCompleted();
                
                CameraManager.Instance.SwitchToCamera(farCamera);
                
            }
            //事件：buttonPlane不改变颜色等，以及block moving（多个对象，用event）
        }
        else
        {
            // Clear the stack if the sequence is incorrect
            for (int i = 0; i < buttonSequence.Count; i++)
            {
                StartCoroutine(FadeColorAnimation(i));
            }
            buttonSequence.Clear();
        }
    }

    private bool IsSequenceCorrect()
    {
        // Check if the last button in the current sequence matches the correct sequence
        if (buttonSequence.Peek() != correctSequence[buttonSequence.Count-1])
        {
            return false;
        }

        return true;
    }
}