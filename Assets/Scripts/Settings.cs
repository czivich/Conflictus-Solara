using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour {

	//variable to hold the window
	public GameObject settingsWindow;

	//variable to hold the exit button
	public Button exitButton;

	//variable to hold the settings menu button
	public Button settingsMenuButton;

	//variable to hold the resolution dropdown
	public TMP_Dropdown resolutionDropdown;

	//variable to hold the resolution apply button
	public Button resolutionApplyButton;

	//variable to hold the restoreDefaults button
	public Button restoreDefaultsButton;

	//variable to hold the acceptButton
	public Button acceptButton;

	//variable for the full screen toggle
	public Toggle fullScreenToggle;

	//variable for the mouse zoom toggle
	public Toggle mouseZoomToggle;

	//variable for the zoom sensitivity slider
	public Slider mouseZoomSensitivitySlider;

	//variable for the zoom sensitivity increase button
	public Button mouseZoomSensitivityUpButton;

	//variable for the zoom sensitivity decrease button
	public Button mouseZoomSensitivityDownButton;

	//variable for the zoom sensitivity value toggle
	public Toggle mouseZoomSensitivityValue;

	//variable for the scroll sensitivity slider
	public Slider mouseScrollSensitivitySlider;

	//variable for the scroll sensitivity increase button
	public Button mouseScrollSensitivityUpButton;

	//variable for the scroll sensitivity decrease button
	public Button mouseScrollSensitivityDownButton;

	//variable for the scroll sensitivity value toggle
	public Toggle mouseScrollSensitivityValue;


	//variable for the music volume slider
	public Slider musicVolumeSlider;

	//variable for the music volume increase button
	public Button musicVolumeUpButton;

	//variable for the music volume decrease button
	public Button musicVolumeDownButton;

	//variable for the music volume value toggle
	public Toggle musicVolumeValue;


	//variable for the sfx volume slider
	public Slider sfxVolumeSlider;

	//variable for the sfx volume increase button
	public Button sfxVolumeUpButton;

	//variable for the sfx volume decrease button
	public Button sfxVolumeDownButton;

	//variable for the sfx volume value toggle
	public Toggle sfxVolumeValue;


	//this array holds scroll rects affected by mouseScroll
	public ScrollRect[] scrollRects;


	//this variable holds the supported screen resolutions
	private	Resolution[] dropdownResolutions;

	//this variable holds the current resolution
	private	Resolution currentResolution = new Resolution();


	//variables for mouse settings
	private const bool mouseZoomInvertedDefault = false;
	private const int mouseZoomSensitivityDefault = 32;
	private const int mouseScrollSensitivityDefault = 32;
	private const int musicVolumeDefault = 32;
	private const int sfxVolumeDefault = 32;

	private bool mouseZoomInverted;
	private int mouseZoomSensitivity;
	private int mouseScrollSensitivity;
	private int musicVolume;
	private int sfxVolume;

	//event for changing resolution
	public UnityEvent OnChangeResolution = new UnityEvent();

	//event for changing mouse zoom toggle
	public MouseInversionEvent OnChangeMouseZoomInversion = new MouseInversionEvent();

	//class for passing bool event
	public class MouseInversionEvent : UnityEvent<bool>{};

	//event for changing mouse zoom sensitivity
	public MouseZoomSensitivityEvent OnChangeMouseZoomSensitivity = new MouseZoomSensitivityEvent();
	public MouseZoomSensitivityEvent OnChangeMouseScrollSensitivity = new MouseZoomSensitivityEvent();

	//events for changing volumes
	public MouseZoomSensitivityEvent OnChangeMusicVolume = new MouseZoomSensitivityEvent();
	public MouseZoomSensitivityEvent OnChangeSfxVolume = new MouseZoomSensitivityEvent();

	//class for passing bool event
	public class MouseZoomSensitivityEvent : UnityEvent<int>{};

	//scene index
	private int mainSceneBuildIndex = 1;

	//unityAction for toggle
	private UnityAction<bool> boolSetFullScreenAction;
	private UnityAction<bool> boolSetMouseZoomInversionAction;

	private UnityAction mouseZoomUpButtonAction;
	private UnityAction mouseZoomDownButtonAction;
	private UnityAction<float> floatMouseZoomSliderAction;

	private UnityAction mouseScrollUpButtonAction;
	private UnityAction mouseScrollDownButtonAction;
	private UnityAction<float> floatMouseScrollSliderAction;

	private UnityAction musicVolumeUpButtonAction;
	private UnityAction musicVolumeDownButtonAction;
	private UnityAction<float> floatMusicVolumeSliderAction;

	private UnityAction sfxVolumeUpButtonAction;
	private UnityAction sfxVolumeDownButtonAction;
	private UnityAction<float> floatSfxVolumeSliderAction;

	private UnityAction settingsFileSaveAction;

	private UnityAction<GameManager.ActionMode> actionModeButtonStatusAction;


	//settings file
	private SettingsData settingsData;


	private string settingsFileName = "Settings";



	//this class will hold all of the save data
	public class SettingsData : IXmlSerializable{

		//variable to hold the parent class
		private Settings settings;

		//variables for mouse settings
		public bool mouseZoomInverted {get; private set;}
		public int mouseZoomSensitivity {get; private set;}
		public int mouseScrollSensitivity {get; private set;}
		public int musicVolume {get; private set;}
		public int sfxVolume {get; private set;}

		//constructor
		public SettingsData(){

			//set the parent
			this.settings = GameObject.FindGameObjectWithTag("UIManager").GetComponent<Settings>();

			//set the data
			this.mouseZoomInverted = settings.mouseZoomInverted;
			this.mouseZoomSensitivity = settings.mouseZoomSensitivity;
			this.mouseScrollSensitivity = settings.mouseScrollSensitivity;
			this.musicVolume = settings.musicVolume;
			this.sfxVolume = settings.sfxVolume;

		}

		//I don't know what this does, but it is required
		public XmlSchema GetSchema(){

			return null;

		}

		//this function is for save info
		public void WriteXml(XmlWriter writer){

			writer.WriteStartElement ("mouseZoomInverted");
			writer.WriteValue (this.mouseZoomInverted.ToString().ToLowerInvariant());
			writer.WriteEndElement();

			writer.WriteStartElement ("mouseZoomSensitivity");
			writer.WriteValue (this.mouseZoomSensitivity.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement ("mouseScrollSensitivity");
			writer.WriteValue (this.mouseScrollSensitivity.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement ("musicVolume");
			writer.WriteValue (this.musicVolume.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement ("sfxVolume");
			writer.WriteValue (this.sfxVolume.ToString());
			writer.WriteEndElement();

		}

		//this function is for load info
		public void ReadXml(XmlReader reader){

			//this first read element is the SettingsData
			reader.ReadStartElement();


			//read the element and store value - mouseZoomInverted
			this.mouseZoomInverted = reader.ReadElementContentAsBoolean();

			//read the element and store value - mouseZoomSensitivity
			this.mouseZoomSensitivity = reader.ReadElementContentAsInt();

			//read the element and store value - mouseScrollSensitivity
			this.mouseScrollSensitivity = reader.ReadElementContentAsInt();

			//read the element and store value - musicVolume
			this.musicVolume = reader.ReadElementContentAsInt();

			//read the element and store value - sfxVolume
			this.sfxVolume = reader.ReadElementContentAsInt();

			//this last ReadEndElement is the SettingsData
			reader.ReadEndElement();

		}

		//this function populates the settingsData with default values
		public void SetDefaultValues(){

			//set the data
			this.mouseZoomInverted = Settings.mouseZoomInvertedDefault;
			this.mouseZoomSensitivity = Settings.mouseZoomSensitivityDefault;
			this.mouseScrollSensitivity = Settings.mouseScrollSensitivityDefault;
			this.musicVolume = Settings.musicVolumeDefault;
			this.sfxVolume = Settings.sfxVolumeDefault;

		}

	}


	// Use this for initialization
	public void Init () {

		//set unityActions
		SetUnityActions();

		//add event listeners
		AddEventListeners();

		//set the full screen toggle state to match the game full screen mode
		fullScreenToggle.isOn = Screen.fullScreen;

		//load settings
		LoadSettings(settingsFileName);

		//update all scroll rects to the settings values
		UpdateScrollRectSensitivity();

		//invoke the mouse zoom sensitivity event
		OnChangeMouseZoomSensitivity.Invoke((int)this.mouseZoomSensitivity);

		//invoke the mouse zoom scroll event
		OnChangeMouseScrollSensitivity.Invoke((int)this.mouseScrollSensitivity);

		//invoke the mouse zoom inversion event
		OnChangeMouseZoomInversion.Invoke(this.mouseZoomInverted);

		//invoke the music volume event
		OnChangeMusicVolume.Invoke((int)this.musicVolume);

		//invoke the sfx volume event
		OnChangeSfxVolume.Invoke((int)this.sfxVolume);

	}

	//this function opens the settings window
	private void OpenSettingsWindow(){

		//activate the window
		settingsWindow.SetActive(true);

		//get the current window resolution
		currentResolution.width = Screen.width;
		currentResolution.height = Screen.height;
		currentResolution.refreshRate = Screen.currentResolution.refreshRate;

		//set the resolution options, passing the screen current resolution
		SetResolutionDropdownOptions(currentResolution);

		//set the UI elements
		SetSettingsUIState();

	}

	//this function closes the settings window
	private void CloseSettingsWindow(){

		//deactivate the window
		settingsWindow.SetActive(false);

	}

	//this function sets the settings button status
	private void SetSettingsButtonStatus(GameManager.ActionMode actionMode){

		//check if we are in a locked action mode
		if (UIManager.lockMenuActionModes.Contains (actionMode)) {

			settingsMenuButton.interactable = false;

		} else {

			//if the mode isn't locked, the button can be interactable
			settingsMenuButton.interactable = true;

		}

	}

	//this function populates the dropdown options for resolution
	private void SetResolutionDropdownOptions(Resolution firstResolution){

		//start by clearing the existing dropdown options
		resolutionDropdown.ClearOptions();

		//create a list of new dropdown options to populate the choices
		List<TMP_Dropdown.OptionData> dropDownOptions = new List<TMP_Dropdown.OptionData> ();

		//create array of dropdown resolutions that are supported by the monitor
		dropdownResolutions = Screen.resolutions;

		//loop through the availale supported resolutions
		for (int i = 0; i < dropdownResolutions.Length; i++) {

			//add a dropdown option for each supported resolution
			dropDownOptions.Add(new TMP_Dropdown.OptionData(dropdownResolutions[i].width.ToString() + " X " 
				+ dropdownResolutions[i].height.ToString()));

		}

		//add the options to the dropdown
		resolutionDropdown.AddOptions (dropDownOptions);

		//find the current resolution in the list of options
		int currentResolutionIndex = System.Array.IndexOf(dropdownResolutions,firstResolution);

		//set the dropdown default to the current resolution
		resolutionDropdown.value = currentResolutionIndex;

	}

	//this function sets the screen resolution
	private void SetResolution(){

		//Debug.Log (resolutionDropdown.value);

		//set the resolution to the resolution in the dropdown
		Screen.SetResolution(dropdownResolutions[resolutionDropdown.value].width,
			dropdownResolutions[resolutionDropdown.value].height,
			Screen.fullScreen,
			dropdownResolutions[resolutionDropdown.value].refreshRate);

		//cache the new current resolution
		currentResolution.width = dropdownResolutions [resolutionDropdown.value].width;
		currentResolution.height = dropdownResolutions [resolutionDropdown.value].height;
		currentResolution.refreshRate = dropdownResolutions [resolutionDropdown.value].refreshRate;

		//invoke the resolution change event
		OnChangeResolution.Invoke();

	}

	//this function sets the options sliders and toggles based on the current settings
	private void SetSettingsUIState(){

		//set the mouse settings
		mouseZoomToggle.isOn = mouseZoomInverted;
		mouseZoomSensitivitySlider.value = mouseZoomSensitivity;
		mouseScrollSensitivitySlider.value = mouseScrollSensitivity;

		//set the volume settings
		musicVolumeSlider.value = musicVolume;
		sfxVolumeSlider.value = sfxVolume;

	}

	//this function resolves clicking the apply button
	private void ResolveApplyButton(){

		//set the resolution
		SetResolution();

		//reset the options
		SetResolutionDropdownOptions(currentResolution);

	}

	//this function resolves a fullscreen toggle
	private void ResolveFullScreenToggle(bool isFullScreen){

		//change the full screen mode to match the toggle state
		Screen.fullScreen = isFullScreen;

	}

	//this function resolves the mouse zoom invert toggle
	private void ResolveMouseZoomToggle(bool isInverted){

		//set the inverted toggle
		mouseZoomInverted = isInverted;

		//invoke the zoom inversion event
		OnChangeMouseZoomInversion.Invoke(mouseZoomInverted);

	}

	//this function adjusts the value toggle qty based on an up button press
	private void ResolveUpButtonPress(Toggle valueToggle, Slider valueSlider, ref int valueToUpdate){

		//parse the string in the value toggle to convert to an int
		int value = 0;
		System.Int32.TryParse (valueToggle.GetComponentInChildren<TextMeshProUGUI> ().text, out value);

		//increment the value
		value++;

		//cap the value at 100
		if (value > 100) {

			value = 100;

		}

		//write the value back to the value toggle
		valueToggle.GetComponentInChildren<TextMeshProUGUI> ().text = value.ToString();

		//write the value back to the slider
		valueSlider.value = value;

		//update the passed value
		valueToUpdate = value;

		//Debug.Log ("Updated Value = " + value);

	}

	//this function adjusts the value toggle qty based on a down button press
	private void ResolveDownButtonPress(Toggle valueToggle, Slider valueSlider, ref int valueToUpdate){

		//parse the string in the value toggle to convert to an int
		int value = 0;
		System.Int32.TryParse (valueToggle.GetComponentInChildren<TextMeshProUGUI> ().text, out value);

		//decrement the value
		value--;

		//floor the value at 0
		if (value < 0) {

			value = 0;

		}

		//write the value back to the value toggle
		valueToggle.GetComponentInChildren<TextMeshProUGUI> ().text = value.ToString();

		//write the value back to the slider
		valueSlider.value = value;

		//update the passed value
		valueToUpdate = value;

	}

	//this function adjusts the value toggle qty based on a slider adjustment
	private void ResolveSliderValueChange(int newValue, Toggle valueToggle, ref int valueToUpdate){

		//write the value back to the value toggle
		valueToggle.GetComponentInChildren<TextMeshProUGUI> ().text = newValue.ToString();

		//update the passed value
		valueToUpdate = newValue;

	}

	//this is a helper function to return the settings base path
	public static string SettingsFileBasePath(){

		string filePath = System.IO.Path.Combine (Application.persistentDataPath, "Settings");

		return filePath;

	}

	//this function resolves saving the settings
	private void SaveSettings(string fileName){

		//create an instance of settingsData
		settingsData = new SettingsData ();

		//write to the XML serializer using the populated SsettingseData file
		XmlSerializer serializer = new XmlSerializer (typeof(SettingsData));
		TextWriter writer = new StringWriter ();
		serializer.Serialize (writer, settingsData);
		writer.Close ();

		//filepath is the full save file
		string filePath = System.IO.Path.Combine(SettingsFileBasePath(), fileName + ".txt");

		//check if the save directory exists
		if (Directory.Exists (SettingsFileBasePath ()) == false) {

			//if it doesn't exist, create it
			Directory.CreateDirectory (SettingsFileBasePath ());

		}

		//write the save data to file
		File.WriteAllText(filePath, writer.ToString());

	}

	//this function resolves loading the settings
	private void LoadSettings(string fileName){

		//filepath is the full save file
		string filePath = System.IO.Path.Combine(SettingsFileBasePath(), fileName + ".txt");

		//create an instance of settingsData
		settingsData = new SettingsData ();

		//check if the file exits
		if (File.Exists (filePath)) {

			//read to the XML serializer using the populated SaveGameData file
			XmlSerializer serializer = new XmlSerializer (typeof(SettingsData));
			TextReader reader = new StringReader (File.ReadAllText (filePath));
			settingsData = (SettingsData)serializer.Deserialize (reader);
			reader.Close ();

			//Debug.Log ("mouseZoomSensitivity = " + settingsData.mouseZoomSensitivity);

		} else {

			//if the file doesn't exist, populate settingsData with the default values
			settingsData.SetDefaultValues();

		}

		//set the parent variables to the settingsData
		this.mouseZoomInverted = settingsData.mouseZoomInverted;
		this.mouseZoomSensitivity = settingsData.mouseZoomSensitivity;
		this.mouseScrollSensitivity = settingsData.mouseScrollSensitivity;
		this.musicVolume = settingsData.musicVolume;
		this.sfxVolume = settingsData.sfxVolume;

	}

	//this function populates the Settings with default values
	private void SetDefaultValues(){

		//set the data to default values
		this.mouseZoomToggle.isOn = Settings.mouseZoomInvertedDefault;
		this.mouseZoomSensitivitySlider.value = Settings.mouseZoomSensitivityDefault;
		this.mouseScrollSensitivitySlider.value = Settings.mouseScrollSensitivityDefault;
		this.musicVolumeSlider.value = Settings.musicVolumeDefault;
		this.sfxVolumeSlider.value = Settings.sfxVolumeDefault;

	}

	//this function updates the scroll speeds of all UI scroll rects
	private void UpdateScrollRectSensitivity(){

		//loop through the scroll rects array
		for (int i = 0; i < scrollRects.Length; i++) {

			//set the scroll speed
			scrollRects[i].scrollSensitivity = this.mouseScrollSensitivity;

		}

	}
	
	//this function sets unityActions
	private void SetUnityActions(){

		//set the toggle action
		boolSetFullScreenAction = ((newToggleValue) => {ResolveFullScreenToggle(newToggleValue);});

		boolSetMouseZoomInversionAction = ((newToggleValue) => {ResolveMouseZoomToggle(newToggleValue);});

		//set mouse zoom actions
		mouseZoomUpButtonAction = (() => {ResolveUpButtonPress(mouseZoomSensitivityValue,mouseZoomSensitivitySlider, ref mouseZoomSensitivity);});
		mouseZoomDownButtonAction = (() => {ResolveDownButtonPress(mouseZoomSensitivityValue,mouseZoomSensitivitySlider,ref mouseZoomSensitivity);});
		floatMouseZoomSliderAction = ((sliderValue) => {

			//update the slider value
			ResolveSliderValueChange ((int)sliderValue, mouseZoomSensitivityValue, ref mouseZoomSensitivity);
		
			//invoke the mouse zoom sensitivity event
			OnChangeMouseZoomSensitivity.Invoke((int)this.mouseZoomSensitivity);
		
		});

		//set mouse scroll actions
		mouseScrollUpButtonAction = (() => {ResolveUpButtonPress(mouseScrollSensitivityValue,mouseScrollSensitivitySlider, ref mouseScrollSensitivity);});
		mouseScrollDownButtonAction = (() => {ResolveDownButtonPress(mouseScrollSensitivityValue,mouseScrollSensitivitySlider,ref mouseScrollSensitivity);});
		floatMouseScrollSliderAction = ((sliderValue) => {

			//update the slider value
			ResolveSliderValueChange ((int)sliderValue, mouseScrollSensitivityValue, ref mouseScrollSensitivity);

			//update the scroll rects
			UpdateScrollRectSensitivity();

		});

		//set music volume actions
		musicVolumeUpButtonAction = (() => {ResolveUpButtonPress(musicVolumeValue,musicVolumeSlider, ref musicVolume);});
		musicVolumeDownButtonAction = (() => {ResolveDownButtonPress(musicVolumeValue,musicVolumeSlider,ref musicVolume);});
		floatMusicVolumeSliderAction = ((sliderValue) => {

			//update the slider value
			ResolveSliderValueChange ((int)sliderValue, musicVolumeValue, ref musicVolume);

		});

		//set sfx volume actions
		sfxVolumeUpButtonAction = (() => {ResolveUpButtonPress(sfxVolumeValue,sfxVolumeSlider, ref sfxVolume);});
		sfxVolumeDownButtonAction = (() => {ResolveDownButtonPress(sfxVolumeValue,sfxVolumeSlider,ref sfxVolume);});
		floatSfxVolumeSliderAction = ((sliderValue) => {

			//update the slider value
			ResolveSliderValueChange ((int)sliderValue, sfxVolumeValue, ref sfxVolume);

		});

		//set the save settings file action
		settingsFileSaveAction = (() => {SaveSettings(settingsFileName);});

		//set the action for setting the button mode
		actionModeButtonStatusAction = ((actionMode) => {SetSettingsButtonStatus(actionMode);});

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for opening the window
		settingsMenuButton.onClick.AddListener(OpenSettingsWindow);

		//add listener for the close button
		exitButton.onClick.AddListener(CloseSettingsWindow);

		//add listener for the accept button
		acceptButton.onClick.AddListener(CloseSettingsWindow);

		//add listener for hitting the apply button
		resolutionApplyButton.onClick.AddListener(ResolveApplyButton);

		//add listener for fullscreen toggle value change
		fullScreenToggle.onValueChanged.AddListener(boolSetFullScreenAction);

		//add listener for mouse zoom inversion toggle value change
		mouseZoomToggle.onValueChanged.AddListener(boolSetMouseZoomInversionAction);

		//add listeners for mouse zoom sensitivity
		mouseZoomSensitivityUpButton.onClick.AddListener(mouseZoomUpButtonAction);
		mouseZoomSensitivityDownButton.onClick.AddListener(mouseZoomDownButtonAction);
		mouseZoomSensitivitySlider.onValueChanged.AddListener (floatMouseZoomSliderAction);

		//add listeners for mouse scroll sensitivity
		mouseScrollSensitivityUpButton.onClick.AddListener(mouseScrollUpButtonAction);
		mouseScrollSensitivityDownButton.onClick.AddListener(mouseScrollDownButtonAction);
		mouseScrollSensitivitySlider.onValueChanged.AddListener (floatMouseScrollSliderAction);

		//add listeners for music volume
		musicVolumeUpButton.onClick.AddListener(musicVolumeUpButtonAction);
		musicVolumeDownButton.onClick.AddListener(musicVolumeDownButtonAction);
		musicVolumeSlider.onValueChanged.AddListener (floatMusicVolumeSliderAction);

		//add listeners for sfx volume
		sfxVolumeUpButton.onClick.AddListener(sfxVolumeUpButtonAction);
		sfxVolumeDownButton.onClick.AddListener(sfxVolumeDownButtonAction);
		sfxVolumeSlider.onValueChanged.AddListener (floatSfxVolumeSliderAction);

		//add listeners for saving settings
		exitButton.onClick.AddListener(settingsFileSaveAction);
		acceptButton.onClick.AddListener(settingsFileSaveAction);

		//add listener for restore default button
		restoreDefaultsButton.onClick.AddListener(SetDefaultValues);

		//check if we are in the main scene
		if (SceneManager.GetActiveScene ().buildIndex == mainSceneBuildIndex) {

			//add a listener for actionMode changes
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().OnActionModeChange.AddListener(actionModeButtonStatusAction);

		}

	}

	//this function handles onDestroy
	private void OnDestroy(){

		//remove the event listeners
		RemoveEventListeners();

	}

	//this function removes all event listeners
	private void RemoveEventListeners(){

		if (settingsMenuButton != null) {
			
			//remove listener for opening the window
			settingsMenuButton.onClick.RemoveListener (OpenSettingsWindow);

		}

		if (exitButton != null) {

			//remove listener for the close button
			exitButton.onClick.RemoveListener (CloseSettingsWindow);

		}

		if (acceptButton != null) {
			
			//remove listener for the accept button
			acceptButton.onClick.RemoveListener (CloseSettingsWindow);

		}

		if (resolutionApplyButton != null) {

			//remove listener for hitting the apply button
			resolutionApplyButton.onClick.RemoveListener (ResolveApplyButton);

		}

		if (fullScreenToggle != null) {
			
			//remove listener for fullscreen toggle value change
			fullScreenToggle.onValueChanged.RemoveListener (boolSetFullScreenAction);

		}

		if (mouseZoomToggle != null) {

			//remove listener for fullscreen toggle value change
			mouseZoomToggle.onValueChanged.RemoveListener (boolSetMouseZoomInversionAction);

		}

		if (mouseZoomSensitivityUpButton != null) {
			
			//remove listeners for mouse zoom sensitivity
			mouseZoomSensitivityUpButton.onClick.RemoveListener(mouseZoomUpButtonAction);

		}

		if (mouseZoomSensitivityDownButton != null) {

			//remove listeners for mouse zoom sensitivity
			mouseZoomSensitivityDownButton.onClick.RemoveListener(mouseZoomDownButtonAction);

		}

		if (mouseZoomSensitivitySlider != null) {

			//remove listeners for mouse zoom sensitivity
			mouseZoomSensitivitySlider.onValueChanged.RemoveListener (floatMouseZoomSliderAction);

		}

		if (mouseScrollSensitivityUpButton != null) {

			//remove listeners for mouse Scroll sensitivity
			mouseScrollSensitivityUpButton.onClick.RemoveListener(mouseScrollUpButtonAction);

		}

		if (mouseScrollSensitivityDownButton != null) {

			//remove listeners for mouse Scroll sensitivity
			mouseScrollSensitivityDownButton.onClick.RemoveListener(mouseScrollDownButtonAction);

		}

		if (mouseScrollSensitivitySlider != null) {

			//remove listeners for mouse Scroll sensitivity
			mouseScrollSensitivitySlider.onValueChanged.RemoveListener (floatMouseScrollSliderAction);

		}

		if (musicVolumeUpButton != null) {

			//remove listeners for music volume
			musicVolumeUpButton.onClick.RemoveListener(musicVolumeUpButtonAction);

		}

		if (musicVolumeDownButton != null) {

			//remove listeners for music volume
			musicVolumeDownButton.onClick.RemoveListener(musicVolumeDownButtonAction);

		}

		if (musicVolumeSlider != null) {

			//remove listeners for music volume
			musicVolumeSlider.onValueChanged.RemoveListener (floatMusicVolumeSliderAction);

		}

		if (sfxVolumeUpButton != null) {

			//remove listeners for sfx volume
			sfxVolumeUpButton.onClick.RemoveListener(sfxVolumeUpButtonAction);

		}

		if (sfxVolumeDownButton != null) {

			//remove listeners for sfx volume
			sfxVolumeDownButton.onClick.RemoveListener(sfxVolumeDownButtonAction);

		}

		if (sfxVolumeSlider != null) {

			//remove listeners for sfx volume
			sfxVolumeSlider.onValueChanged.RemoveListener (floatSfxVolumeSliderAction);

		}

		if (exitButton != null) {

			//remove listeners for saving settings
			exitButton.onClick.RemoveListener(settingsFileSaveAction);

		}

		if (acceptButton != null) {

			//remove listeners for saving settings
			acceptButton.onClick.RemoveListener(settingsFileSaveAction);

		}

		if (restoreDefaultsButton != null) {

			//add listener for restore default button
			restoreDefaultsButton.onClick.RemoveListener (SetDefaultValues);

		}

		if (GameObject.FindGameObjectWithTag("GameManager") != null) {

			//remove a listener for actionMode changes
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().OnActionModeChange.RemoveListener(actionModeButtonStatusAction);

		}

	}

}
