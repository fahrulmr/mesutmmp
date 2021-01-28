using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;

using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Reflection;
using UnityEngine.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(GenericPanelBehaviour), true)]
public class GenericPanelEditor : Editor
{
	private string scriptName = "";
	private bool waitForCompile = false;
	private string componentName;
	private GameObject targetGameObject;

	private bool wasEdited;
	private bool toggleValue;

	private const string PANEL_OPENED_STATE = "PanelOpened";
	private const string PANEL_IDLE_STATE = "PanelIdle";
	private const string PANEL_CLOSED_STATE = "PanelClosed";
	private const string LAYER_NAME = "Panel";

	private const string ROOT_ANIMATIONS_PATH = "Assets/Animations";
	private const string IS_OPENED = "IsOpened";

	private bool blurSelected = false;
	private bool whiteSelected = false;
	private bool darkSelected = false;
	private bool foldout = false;
	private bool eventsFoldout = false;

	public void OnEnable()
	{
		scriptName = target.name;

		// This is here to instantiate panel manager when first panel script is added
		if (PanelManager.Instance == null)
		{
			return;
		}
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(serializedObject.FindProperty("PanelType"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("PanelProperties"), true);

		eventsFoldout = EditorGUILayout.Foldout(eventsFoldout, "Events", true);

		if (eventsFoldout)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPanelOpen"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPanelOpenEnd"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPanelClose"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPanelCloseEnd"));
		}

		var prevColor = GUI.contentColor;
		GUI.contentColor = Color.red;
		GUIStyle style = new GUIStyle();

		GUI.contentColor = prevColor;
		GenericPanelBehaviour myScript = (GenericPanelBehaviour)target;
		var animator = myScript.gameObject.GetComponent<Animator>();

		if (targets.Length > 1)
			return;

		ProcessEffects();

		if (animator.runtimeAnimatorController == null || animator.runtimeAnimatorController.name == "Default Panel Controller")
		{
			var controllerName = target.name + " Controller.controller";

			if(GUILayout.Button("Create Mirrored \"" + controllerName + "\""))
			{
				CreateMirroredAnimator(myScript.gameObject, controllerName);
			}

			if(GUILayout.Button("Create \"" + controllerName + "\""))
			{
				CreateAnimator(myScript.gameObject, controllerName);
			}
		}
		else
		{
			bool found = false;

			for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
			{
				var clip = animator.runtimeAnimatorController.animationClips[i];
				if (clip.name == PANEL_OPENED_STATE)
				{
					found = true;
				}
			}

			if (!found)
			{
				if (GUILayout.Button("Add Mirrored states to controller"))
				{
					CreateAnimatorStates(animator.runtimeAnimatorController as AnimatorController, true);
				}

				if (GUILayout.Button("Add Open/Close states to controller"))
				{
					CreateAnimatorStates(animator.runtimeAnimatorController as AnimatorController);
				}
			}
		}

		if (target.GetType() == typeof(GenericPanelBehaviour))
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Script Name: ");
			var prevScriptName = scriptName;
			if (!wasEdited)
			{
				scriptName = target.name;
			}

			scriptName = GUILayout.TextField(scriptName);
			if (scriptName != prevScriptName)
			{
				wasEdited = true;
			}
			bool exists = false;

			if (!File.Exists(GetFilePath(scriptName)))
			{
				if (GUILayout.Button("Create\" " + GenerateFileName(scriptName) + "\""))
				{
					CreateScript(((GenericPanelBehaviour)target).gameObject, scriptName);
				}
			}
			else
			{
				if (GUILayout.Button("Replace with \"" + GenerateFileName(scriptName) + "\""))
				{
					CreateScript(((GenericPanelBehaviour)target).gameObject, scriptName);
				}
				exists = true;
			}

			GUILayout.EndHorizontal();

			var msg = "";

			if (waitForCompile)
			{
				msg = "Please, wait while the script is compiling. It will add automatically.";
			}
			else if (exists)
			{
				msg = "Script with this name already exists. ";
			}
			else
			{
				msg = "Component will be created and will replace GenericPanel on this gameobject.";
			}

			EditorGUILayout.HelpBox(msg, MessageType.Info);
		}

		if (waitForCompile)
		{
			if (!EditorApplication.isCompiling)
			{
				waitForCompile = false;
				DestroyImmediate(targetGameObject.GetComponent<GenericPanelBehaviour>());
				targetGameObject.AddComponent(GetType(componentName));
			}
		}

		if (!(target is GenericPanelEditor))
		{
			var it = serializedObject.GetIterator();
			it.NextVisible(true);

			while (it.NextVisible(false))
			{
				var name = it.name;

				if (name != "PanelType" && name != "PanelProperties" &&
					name != "OnPanelOpen" && name != "OnPanelOpenEnd" &&
					name != "OnPanelClose" && name != "OnPanelCloseEnd")
				{
					EditorGUILayout.PropertyField(it, true);
				}
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

	private void ProcessEffects()
	{
		GenericPanelBehaviour myScript = (GenericPanelBehaviour)target;

		foldout = EditorGUILayout.Foldout(foldout, "Effects", true);

		if (foldout)
		{
			blurSelected = GUILayout.Button("Add Blur");
			whiteSelected = GUILayout.Button("Add White Background");
			darkSelected = GUILayout.Button("Add Dark Background");
		}

		var effects = myScript.transform.Find("Effects");

		if (blurSelected || whiteSelected || darkSelected)
		{
			if (myScript.transform.Find("Effects") == null)
			{
				effects = CreateGameObject("Effects", myScript.transform).transform;
				effects.SetAsFirstSibling();
			}

			if (blurSelected)
			{
				var blur = effects.Find("Blur");
				if (blur == null)
				{
					var go = CreateGameObjectWithRawImage("Blur", effects);
					var blurMaterial = AssetDatabase.LoadAssetAtPath("Assets/Resources/Materials/Blurred.mat", typeof(Material)) as Material;
					Debug.Log(blurMaterial.name);
					go.GetComponent<RawImage>().material = blurMaterial;
				}
			}

			if (whiteSelected)
			{
				var blur = effects.Find("Whiten");
				if (blur == null)
				{
					var go = CreateGameObjectWithRawImage("Whiten", effects);
					go.GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
				}
			}
			if (darkSelected)
			{
				var blur = effects.Find("Darken");
				if (blur == null)
				{
					var go = CreateGameObjectWithRawImage("Darken", effects);
					go.GetComponent<RawImage>().color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
				}
			}
		}
	}

	private GameObject CreateGameObjectWithRawImage(string name, Transform transform)
	{
		var go = CreateGameObject(name, transform);

		go.AddComponent<RawImage>();

		return go;
	}

	private GameObject CreateGameObject(string name, Transform transform)
	{
		var go = new GameObject();

		go.transform.parent = transform;
		var panelRectTransform = go.AddComponent<RectTransform>();

		panelRectTransform.anchorMin = new Vector2(0, 0);
		panelRectTransform.anchorMax = new Vector2(1, 1);
		panelRectTransform.sizeDelta = new Vector2(0.0f, 0.0f);

		panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
		panelRectTransform.localScale = Vector3.one;
		panelRectTransform.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

		go.name = name;

		return go;
	}

	public Type GetType( string TypeName )
	{
		// Try Type.GetType() first. This will work with types defined
		// by the Mono runtime, in the same assembly as the caller, etc.
		var type = Type.GetType( TypeName );

		// If it worked, then we're done here
		if( type != null )
			return type;

		// If the TypeName is a full name, then we can try loading the defining assembly directly
		if( TypeName.Contains( "." ) )
		{

			// Get the name of the assembly (Assumption is that we are using 
			// fully-qualified type names)
			var assemblyName = TypeName.Substring( 0, TypeName.IndexOf( '.' ) );

			// Attempt to load the indicated Assembly
			var assembly = Assembly.Load( assemblyName );
			if( assembly == null )
				return null;

			// Ask that assembly to return the proper Type
			type = assembly.GetType( TypeName );
			if( type != null )
				return type;

		}

		// If we still haven't found the proper type, we can enumerate all of the 
		// loaded assemblies and see if any of them define the type
		var currentAssembly = Assembly.GetExecutingAssembly();
		var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
		foreach( var assemblyName in referencedAssemblies )
		{

			// Load the referenced assembly
			var assembly = Assembly.Load( assemblyName );
			if( assembly != null )
			{
				// See if that assembly defines the named type
				type = assembly.GetType( TypeName );
				if( type != null )
					return type;
			}
		}

		// The type just couldn't be found...
		return null;
	}

	private string GenerateScriptName(string name)
	{
		name = RemoveSpecialCharacters(scriptName);
		name = name.Replace("-","_");
		name = name.Replace(" ", "");
		name += "Behaviour";

		return name;
	}

	private string GenerateFileName(string name)
	{
		return GenerateScriptName(name) + ".cs";
	}

	private string GetFilePath(string name)
	{
		string copyPath = "Assets/Scripts/Panels/" + GenerateFileName(scriptName);

		return copyPath;
	}

	private void CreateScript(GameObject target, string scriptName)
	{
		string copyPath = GetFilePath(scriptName);

		if( File.Exists(copyPath) == false ){ // do not overwrite
			using (StreamWriter outfile = 
				new StreamWriter(copyPath))
			{
				outfile.WriteLine("using UnityEngine;");
				outfile.WriteLine("using UnityEngine.UI;");
				outfile.WriteLine("using System.Collections;");
				outfile.WriteLine("using System.Collections.Generic;");
				outfile.WriteLine("");
				outfile.WriteLine("public class " + GenerateScriptName(scriptName) + " : GenericPanelBehaviour");
				outfile.WriteLine("{");
				outfile.WriteLine("\tvoid OnInit ()");
				outfile.WriteLine("\t{");
				outfile.WriteLine(" ");
				outfile.WriteLine("\t}");
				outfile.WriteLine(" ");
				outfile.WriteLine("\tvoid OnOpen ()");
				outfile.WriteLine("\t{");
				outfile.WriteLine(" ");
				outfile.WriteLine("\t}");
				outfile.WriteLine(" ");
				outfile.WriteLine("\tvoid OnClose ()");
				outfile.WriteLine("\t{");
				outfile.WriteLine(" ");
				outfile.WriteLine("\t}");
				outfile.WriteLine(" ");
				outfile.WriteLine("\tvoid PanelUpdate ()");
				outfile.WriteLine("\t{");
				outfile.WriteLine(" ");
				outfile.WriteLine("\t}");
				outfile.WriteLine("}");
			}

			AssetDatabase.ImportAsset(copyPath, ImportAssetOptions.Default);
		}

		waitForCompile = true;
		componentName = GenerateScriptName(scriptName);
		targetGameObject = target;
	}

	public static string RemoveSpecialCharacters(string str) {
		StringBuilder sb = new StringBuilder();
		foreach (char c in str) {
			if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ') {
				sb.Append(c);
			}
		}
		return sb.ToString();
	}

	public void CreateMirroredAnimator(GameObject go, string name)
	{
		CreateAnimator(go, name, true);
	}

	public void CreateAnimator(GameObject go, string name, bool mirrored = false)
	{
		var path = ROOT_ANIMATIONS_PATH;

		System.IO.Directory.CreateDirectory(path);
		var filePath = AssetDatabase.GenerateUniqueAssetPath(path.Substring(path.IndexOf("Assets")) + "/" + name);
		var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(filePath);

		CreateAnimatorStates(controller, mirrored);

		go.GetComponent<Animator>().runtimeAnimatorController = controller;
	}

	private void CreateAnimatorStates(AnimatorController controller, bool mirrored = false)
	{
		AnimationClip PanelIdleClip = new AnimationClip();
		PanelIdleClip.name = PANEL_IDLE_STATE;

		AnimationClip PanelOpenedClip = new AnimationClip();
		PanelOpenedClip.name = PANEL_OPENED_STATE;

		AnimationClip PanelClosedClip = new AnimationClip();
		PanelClosedClip.name = PANEL_CLOSED_STATE;

		AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 0.0f, 1.0f);

		PanelOpenedClip.SetCurve("", typeof(CanvasGroup), "m_Alpha", curve);
		PanelOpenedClip.SetCurve("", typeof(CanvasGroup), "m_Interactable", curve);
		PanelOpenedClip.SetCurve("", typeof(CanvasGroup), "m_BlocksRaycasts", curve);

		curve = AnimationCurve.EaseInOut(0.0f, 1.0f, 0.0f, 0.0f);

		PanelClosedClip.SetCurve("", typeof(CanvasGroup), "m_Alpha", curve);
		PanelClosedClip.SetCurve("", typeof(CanvasGroup), "m_Interactable", curve);
		PanelClosedClip.SetCurve("", typeof(CanvasGroup), "m_BlocksRaycasts", curve);

		curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 0.0f, 0.0f);

		PanelIdleClip.SetCurve("", typeof(CanvasGroup), "m_Alpha", curve);
		PanelIdleClip.SetCurve("", typeof(CanvasGroup), "m_Interactable", curve);
		PanelIdleClip.SetCurve("", typeof(CanvasGroup), "m_BlocksRaycasts", curve);

		controller.AddParameter(IS_OPENED, AnimatorControllerParameterType.Bool);


		AnimatorState statePanelClosed;

		AnimatorControllerLayer layer = new AnimatorControllerLayer();
		layer.name = LAYER_NAME;
		layer.defaultWeight = 1.0f;
		layer.stateMachine = new AnimatorStateMachine();

		if (mirrored)
		{
			statePanelClosed = layer.stateMachine.AddState(PANEL_CLOSED_STATE);

			statePanelClosed.motion = PanelOpenedClip;
			statePanelClosed.speed = -1.0f;
		}
		else
		{
			statePanelClosed = controller.AddMotion(PanelClosedClip);
		}
		controller.AddLayer(layer);
		var layerIndex = GetLayerIndex(layer, controller);

		var statePanelIdle = controller.AddMotion(PanelIdleClip, layerIndex);
		var statePanelOpened = controller.AddMotion(PanelOpenedClip, layerIndex);

		layer.stateMachine.defaultState = statePanelIdle;
		var idleToOpenedTransition = statePanelIdle.AddTransition(statePanelOpened);
		idleToOpenedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1.0f, IS_OPENED);
		idleToOpenedTransition.duration = 0;

		var openedToClosedTransition = statePanelOpened.AddTransition(statePanelClosed);
		openedToClosedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 1.0f, IS_OPENED);
		openedToClosedTransition.duration = 0;

		var closedToOpenedTransition = statePanelClosed.AddTransition(statePanelOpened);
		closedToOpenedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1.0f, IS_OPENED);
		closedToOpenedTransition.duration = 0;


		layer.stateMachine.hideFlags = HideFlags.HideInInspector;
		AssetDatabase.AddObjectToAsset(layer.stateMachine, controller);

		AssetDatabase.AddObjectToAsset(statePanelIdle, controller);
		AssetDatabase.AddObjectToAsset(statePanelOpened, controller);
		AssetDatabase.AddObjectToAsset(statePanelClosed, controller);

		AssetDatabase.AddObjectToAsset(idleToOpenedTransition, controller);
		AssetDatabase.AddObjectToAsset(openedToClosedTransition, controller);
		AssetDatabase.AddObjectToAsset(closedToOpenedTransition, controller);

		AssetDatabase.AddObjectToAsset(PanelIdleClip, controller);
		AssetDatabase.AddObjectToAsset(PanelOpenedClip, controller);
		AssetDatabase.AddObjectToAsset(PanelClosedClip, controller);

		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(controller));
	}

	private int GetLayerIndex(AnimatorControllerLayer layer, AnimatorController controller)
	{
		var layerIndex = 0;
		for (int i = 0; i < controller.layers.Length; i++)
		{
			if (controller.layers[i].name == layer.name)
			{
				layerIndex = i;
			}
		}

		return layerIndex;
	}
}
