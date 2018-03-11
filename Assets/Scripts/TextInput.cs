using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TextInput : MonoBehaviour {

	//these are the UI elements required - public so they can be hooked up in inspector
	public TMP_InputField inputField;

	//event to announce creation of new chat message
	public ChatMessageEvent OnNewChatMessageSubmitted = new ChatMessageEvent();

	//simple class derived from unityEvent to pass int variable
	public class ChatMessageEvent : UnityEvent<string>{};

	//unityActions
	private UnityAction<string> stringInputSubmitCallBackAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		stringInputSubmitCallBackAction = (inputText) => {inputSubmitCallBack(inputText);};

		//add listener for the input submit event
		inputField.onEndEdit.AddListener(stringInputSubmitCallBackAction);
		
	}
	
	//called when input is submitted
	private void inputSubmitCallBack(string userInput){

		if (userInput != "" && (Input.GetKey(KeyCode.Return) == true || Input.GetKey(KeyCode.KeypadEnter) == true)) {

			//invoke the OnNewChatMessage event
			OnNewChatMessageSubmitted.Invoke(userInput);

			//clear the text input field
			inputField.text = "";

			//activate the text input field again
			inputField.ActivateInputField ();
			inputField.Select ();

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listener for the input submit event
		if (inputField != null) {
			
			inputField.onEndEdit.RemoveListener (stringInputSubmitCallBackAction);

		}

	}
		
}
