using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct ClothesItem
{
	public int ClothesTypeIndex;
    public string Name;
    public Texture2D Texture;
    public string Price;
    [EnumFlagsAttribute] 
	public ClothingSize ClothingSize; 
    public string URL;
	public Vector2 Position;
	public Vector2 Offset;
	public float Scale;
	public Astra.JointType JointType;
}
    

public class EnumFlagsAttribute : PropertyAttribute 
{ 
    public EnumFlagsAttribute() { } 
} 
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumFlagsAttribute))] 
public class EnumFlagsAttributeDrawer : PropertyDrawer 
{ 
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label) 
    { 
        _property.intValue = EditorGUI.MaskField( _position, _label, _property.intValue, _property.enumNames ); 
    } 
}
#endif

[System.Flags] 
public enum ClothingSize 
{ 
    XS = (1 << 0), 
    S = (1 << 1), 
    M = (1 << 2), 
    L = (1 << 3), 
    XL = (1 << 4), 
    XXL = (1 << 5), 
} 

[RequireComponent(typeof(RawImage))]
public class ClothesItemBehaviour : MonoBehaviour
{
    private ClothesItem clothesItem;

    public ClothesItem ClothesItem
    {
        get
        {
            return clothesItem;
        }
        set
        {
            clothesItem = value;
            UpdateItem();
        }
    }

    private RawImage image;

    void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public void UpdateItem()
    {
        image.SetAndFitTexture(ClothesItem.Texture);
    }
}


