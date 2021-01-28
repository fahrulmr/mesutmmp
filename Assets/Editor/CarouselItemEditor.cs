using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.IO;
using System.Text;
using UnityEditor.Animations;

[CanEditMultipleObjects]
[CustomEditor(typeof(CarouselItemBehaviour), true)]
public class CarouselItemEditor : Editor 
{
	private string scriptName = "";
	private bool waitForCompile = false;
	private string componentName;
	private GameObject targetGameObject;

	private bool wasEdited;
	private bool toggleValue;
	private bool eventsFoldout;

	private const string LAYER_NAME = "Carousel";
	private const string ITEM_IDLE_STATE_NAME = "ItemIdle";
	private const string ITEM_SELECTED_STATE_NAME = "ItemSelected";
	private const string ITEM_UNSELECTED_STATE_NAME = "ItemUnselected";
	private const string IS_SELECTED = "IsSelected";
	private const string ANIMATIONS_ROOT_PATH = "Assets/Animations";

	public void OnEnable()
	{
		scriptName = target.name;
	}

	public override void OnInspectorGUI()
	{
		eventsFoldout = EditorGUILayout.Foldout(eventsFoldout, "Events", true);

		if (eventsFoldout)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemSelected"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemUnselected"));
		}

		if (targets.Length > 1)
			return;
		
		var myScript = (CarouselItemBehaviour)target;
		var animator = myScript.gameObject.GetComponent<Animator>();
		if (animator == null)
		{
			animator = myScript.gameObject.AddComponent<Animator>();
		}
		if (animator.runtimeAnimatorController == null)
		{
			var controllerName = target.name + " Controller.controller";

			if (GUILayout.Button("Create Mirrored \"" + controllerName + "\""))
			{
				CreateMirroredAnimator(myScript.gameObject, controllerName);
			}

			if (GUILayout.Button("Create \"" + controllerName + "\""))
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
				if (clip.name == "ItemSelected")
				{
					found = true;
				}
			}

			if (!found)
			{
				if (GUILayout.Button("Add mirrored states to controller"))
				{
					CreateAnimatorStates(animator.runtimeAnimatorController as AnimatorController, true);
				}

				if (GUILayout.Button("Add selected/unselected states to controller"))
				{
					CreateAnimatorStates(animator.runtimeAnimatorController as AnimatorController);
				}
			}
		}
		if (target.GetType() == typeof(CarouselItemBehaviour))
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
				if (GUILayout.Button("Create \"" + GenerateFileName(scriptName) + "\""))
				{
					CreateScript(((CarouselItemBehaviour)target).gameObject, scriptName);
				}
			}
			else
			{
				if (GUILayout.Button("Replace with \"" + GenerateFileName(scriptName) + "\""))
				{
					CreateScript(((CarouselItemBehaviour)target).gameObject, scriptName);
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
				msg = "Component will be created and will replace GenericItem on this gameobject.";
			}

			EditorGUILayout.HelpBox(msg, MessageType.Info);
		}

		if (waitForCompile)
		{
			if (!EditorApplication.isCompiling)
			{
				waitForCompile = false;
				DestroyImmediate(targetGameObject.GetComponent<CarouselItemBehaviour>());
				targetGameObject.AddComponent(GetType(componentName));
			}
		}
	}

	[ MenuItem( "GameObject/Create Other/Create Carousel #n" ) ]
	private static void CreatePanel()
	{
		var selectedGo = Selection.activeObject as GameObject;

		var carouselGameObject = CreateGameObjectWithParent(selectedGo.transform, "Carousel");
		var carouselBehaviour = carouselGameObject.AddComponent<CarouselBehaviour>();
		var selectorGameObject = CreateGameObjectWithParent(carouselGameObject.transform, "Selector");
		var viewportGameObject = CreateGameObjectWithParent(carouselGameObject.transform, "Viewport");
		var viewportRectTransform = viewportGameObject.GetComponent<RectTransform>();
		viewportRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
		viewportRectTransform.anchorMin = new Vector2(0.0f, 0.0f);
		viewportRectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
		viewportGameObject.AddComponent<RectMask2D>();

		var contentGameObject = CreateGameObjectWithParent(viewportGameObject.transform, "Content");
		var emptyImage = contentGameObject.AddComponent<RawImage>();
		emptyImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		var contentRectTransform = contentGameObject.GetComponent<RectTransform>();
		contentRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
		contentRectTransform.anchorMin = new Vector2(0.0f, 0.0f);
		contentRectTransform.sizeDelta = new Vector2(0.0f, 0.0f);

		carouselBehaviour.Content = contentGameObject.transform;
		carouselBehaviour.Selector = selectorGameObject.transform;

		Selection.activeGameObject = carouselGameObject;
	}

	private static GameObject CreateGameObjectWithParent(Transform parent, string name)
	{
		var go = new GameObject();
		go.transform.parent = parent;
		var rectTransform = go.AddComponent<RectTransform>();
		rectTransform.localScale = Vector3.one;
		rectTransform.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		go.name = name;

		return go;
	}


	private string GenerateScriptName(string name)
	{
		name = RemoveSpecialCharacters(scriptName);
		name = name.Replace("-","_");
		name = name.Replace(" ", "");
		name += "ItemBehaviour";

		return name;
	}

	private string GenerateFileName(string name)
	{
		return GenerateScriptName(name) + ".cs";
	}

	private string GetFilePath(string name)
	{
		string copyPath = "Assets/Scripts/Carousels/" + GenerateFileName(scriptName);

		return copyPath;
	}

	private void CreateScript(GameObject target, string scriptName)
	{
		string copyPath = GetFilePath(scriptName);
		Directory.CreateDirectory("Assets/Scripts/Carousels/");
		if( File.Exists(copyPath) == false ){ // do not overwrite
			using (StreamWriter outfile = 
				new StreamWriter(copyPath))
			{
				outfile.WriteLine("using UnityEngine;");
				outfile.WriteLine("using UnityEngine.UI;");
				outfile.WriteLine("using System.Collections;");
				outfile.WriteLine("using System.Collections.Generic;");
				outfile.WriteLine("");
				outfile.WriteLine("public class " + GenerateScriptName(scriptName) + " : CarouselItemBehaviour");
				outfile.WriteLine("{");
				outfile.WriteLine("");
				outfile.WriteLine("}");
			}

			AssetDatabase.ImportAsset(copyPath, ImportAssetOptions.Default);
		}

		waitForCompile = true;
		componentName = GenerateScriptName(scriptName);
		targetGameObject = target;
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
		var path = ANIMATIONS_ROOT_PATH;

		System.IO.Directory.CreateDirectory(path);
		var filePath = AssetDatabase.GenerateUniqueAssetPath(path.Substring(path.IndexOf("Assets")) + "/" + name);
		var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(filePath);

		CreateAnimatorStates(controller, mirrored);

		go.GetComponent<Animator>().runtimeAnimatorController = controller;
	}

	private void CreateAnimatorStates(AnimatorController controller, bool mirrored = false)
	{
		AnimationClip ItemIdleClip = new AnimationClip();
		ItemIdleClip.name = ITEM_IDLE_STATE_NAME;

		AnimationClip ItemSelectedClip = new AnimationClip();
		ItemSelectedClip.name = ITEM_SELECTED_STATE_NAME;

		AnimationClip ItemUnselectedClip = new AnimationClip();
		ItemUnselectedClip.name = ITEM_UNSELECTED_STATE_NAME;


		controller.AddParameter(IS_SELECTED, AnimatorControllerParameterType.Bool);

		AnimatorState statePanelClosed;

		AnimatorControllerLayer layer = new AnimatorControllerLayer();
		layer.name = LAYER_NAME;
		layer.defaultWeight = 1.0f;
		layer.stateMachine = new AnimatorStateMachine();

		if (mirrored)
		{
			statePanelClosed = layer.stateMachine.AddState(ITEM_UNSELECTED_STATE_NAME);

			statePanelClosed.motion = ItemSelectedClip;
			statePanelClosed.speed = -1.0f;
		}
		else
		{
			statePanelClosed = controller.AddMotion(ItemUnselectedClip);
		}

		controller.AddLayer(layer);
		var layerIndex = GetLayerIndex(layer, controller);

		var statePanelIdle = controller.AddMotion(ItemIdleClip, layerIndex);
		var statePanelOpened = controller.AddMotion(ItemSelectedClip, layerIndex);
		layer.stateMachine.defaultState = statePanelIdle;

		var idleToOpenedTransition = statePanelIdle.AddTransition(statePanelOpened);
		idleToOpenedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1.0f, IS_SELECTED);
		idleToOpenedTransition.duration = 0;

		var openedToClosedTransition = statePanelOpened.AddTransition(statePanelClosed);
		openedToClosedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 1.0f, IS_SELECTED);
		openedToClosedTransition.duration = 0;

		var closedToOpenedTransition = statePanelClosed.AddTransition(statePanelOpened);
		closedToOpenedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1.0f, IS_SELECTED);
		closedToOpenedTransition.duration = 0;

		layer.stateMachine.hideFlags = HideFlags.HideInInspector;
		AssetDatabase.AddObjectToAsset(layer.stateMachine, controller);

		AssetDatabase.AddObjectToAsset(statePanelIdle, controller);
		AssetDatabase.AddObjectToAsset(statePanelOpened, controller);
		AssetDatabase.AddObjectToAsset(statePanelClosed, controller);

		AssetDatabase.AddObjectToAsset(idleToOpenedTransition, controller);
		AssetDatabase.AddObjectToAsset(openedToClosedTransition, controller);
		AssetDatabase.AddObjectToAsset(closedToOpenedTransition, controller);

		AssetDatabase.AddObjectToAsset(ItemIdleClip, controller);
		AssetDatabase.AddObjectToAsset(ItemSelectedClip, controller);
		AssetDatabase.AddObjectToAsset(ItemUnselectedClip, controller);
		AssetDatabase.SaveAssets();
			
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
