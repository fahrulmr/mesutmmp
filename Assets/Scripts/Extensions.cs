using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
	public static string ColorRed(this string value)
	{
		return value.SetColor("red");
	}

	public static string SetColor(this string value, string color)
	{
		return "<color=" + color + ">" + value + "</color>";
	}

	public static T ToEnum<T>(this string value)
	{
		return (T) System.Enum.Parse(typeof(T), value, true);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n);
			list.Swap(k, n);
		}
	}

	public static long GetUnixMillisecond(this System.DateTime dt)
	{
		var timeNow = (long)((System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalMilliseconds);
		return timeNow;
	}

	public static void SetAndFitTexture(this UnityEngine.UI.RawImage image, UnityEngine.Texture2D texture, bool keepHeight = true)
	{
		image.texture = texture;

		if (keepHeight)
		{
			var height = image.rectTransform.sizeDelta.y;

			image.SetNativeSize();


			var width = (image.texture.width * height) / image.texture.height;

			image.rectTransform.sizeDelta = new Vector2(width, height);
		}
		else
		{
			var width = image.rectTransform.sizeDelta.x;

			image.SetNativeSize();

			if (image.texture != null)
			{
				var height = (image.texture.height * width) / image.texture.width;

				image.rectTransform.sizeDelta = new Vector2(width, height);
			}
		}
	}

	public static void Swap<T>(this IList<T> list, int i, int j)
	{
		T value = list[i];
		list[i] = list[j];
		list[j] = value;
	}

	public static T AddContentElement<T>(this GameObject go, GameObject prefab) where T: MonoBehaviour
	{
		var tBehaviour = GameObject.Instantiate(prefab).GetComponent<T>();

		tBehaviour.transform.parent = go.transform;
		tBehaviour.transform.localScale = Vector3.one;
		tBehaviour.transform.localPosition = Vector3.zero;
		tBehaviour.transform.localRotation = Quaternion.identity;

		return tBehaviour;
	}

	public static void DestroyChildren(this GameObject go)
	{
		for (int i = go.transform.childCount - 1; i >= 0; i--)
		{
			GameObject.DestroyImmediate(go.transform.GetChild(i).gameObject);
		}
	}

	public static void SetParentAndResetPosition(this GameObject go, Transform parent)
	{
		go.transform.SetParent(parent);
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
	}

	public static void SetParentAndResetPosition(this GameObject go, GameObject parent)
	{
		go.transform.SetParent(parent.transform);
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
	}

	public static string ToRoman(this int number)
	{
		if ((number < 0) || (number > 3999)) return "0";
		if (number < 1) return string.Empty;
		if (number >= 1000) return "M" + ToRoman(number - 1000);
		if (number >= 900) return "CM" + ToRoman(number - 900); //EDIT: i've typed 400 instead 900
		if (number >= 500) return "D" + ToRoman(number - 500);
		if (number >= 400) return "CD" + ToRoman(number - 400);
		if (number >= 100) return "C" + ToRoman(number - 100);
		if (number >= 90) return "XC" + ToRoman(number - 90);
		if (number >= 50) return "L" + ToRoman(number - 50);
		if (number >= 40) return "XL" + ToRoman(number - 40);
		if (number >= 10) return "X" + ToRoman(number - 10);
		if (number >= 9) return "IX" + ToRoman(number - 9);
		if (number >= 5) return "V" + ToRoman(number - 5);
		if (number >= 4) return "IV" + ToRoman(number - 4);
		if (number >= 1) return "I" + ToRoman(number - 1);
		return "0";
	}

	public static void AddListenerSafe(this UnityEvent unityEvent, UnityAction unityAction)
	{
		if (unityAction != null)
		{
			unityEvent.AddListener(unityAction);
		}
	}

	public static void AddListenerSafe<T0>(this UnityEvent<T0> unityEvent, UnityAction<T0> unityAction)
	{
		if (unityAction != null)
		{
			unityEvent.AddListener(unityAction);
		}
	}

	public static Coroutine Tween(this MonoBehaviour mb, float time, float start, float end, System.Action<float> action, float? delay = null)
	{
		return mb.StartCoroutine(TweenHelper(time, start, end, action, delay));
	}

	public static Coroutine Tween(this MonoBehaviour mb, float time, int start, int end, System.Action<float> action, float? delay = null)
	{
		return mb.StartCoroutine(TweenHelper(time, (float)start, (float)end, action, delay));
	}

	private static IEnumerator TweenHelper(float time, float start, float end, System.Action<float> action, float? delay)
	{
		if (delay != null)
		{
			yield return new WaitForSeconds(delay.Value);
		}
		var timeCalled = Time.time;
		while (Time.time - timeCalled <= time)
		{
			action.Invoke(Mathf.Lerp(start, end, (Time.time - timeCalled) / time));
			yield return null;
		}
		action.Invoke(end);
	}

	public static void Invoke(this MonoBehaviour mb, float delay, System.Action action)
	{
		mb.StartCoroutine(InvokeHelper(new WaitForSeconds(delay), action));
	}

	public static Coroutine Invoke(this MonoBehaviour mb, YieldInstruction instruction, System.Action action)
	{
		return mb.StartCoroutine(InvokeHelper(instruction, action));
	}

	public static Coroutine Invoke(this MonoBehaviour mb, CustomYieldInstruction instruction, System.Action action)
	{
		return mb.StartCoroutine(InvokeHelper(instruction, action));
	}

	private static IEnumerator InvokeHelper(YieldInstruction instruction, System.Action action)
	{
		yield return instruction;
		action.Invoke();
	}

	private static IEnumerator InvokeHelper(CustomYieldInstruction instruction, System.Action action)
	{
		yield return instruction;
		action.Invoke();
	}

	public static void DrawVector2(this Vector2 val, float radius = 0.1f, Color? c = null, float duration = 0)
	{
		Color color = c ?? Color.white;
		int n = (int)(20 * radius / 0.2f);
		float dfi = Mathf.PI * 2.0f / (float)n;
		for (float fi = 0.0f; fi < Mathf.PI * 2.0f; fi += dfi)
		{
			Debug.DrawLine(val + new Vector2(Mathf.Cos(fi) * radius, Mathf.Sin(fi) * radius),
				val + new Vector2(Mathf.Cos(fi + dfi) * radius, Mathf.Sin(fi + dfi) * radius), color, duration);
		}
	}

	public static void DrawVector2(this Vector2 val, Color color)
	{
		val.DrawVector2(0.1f, color);
	}

	public static void DrawVector3(this Vector3 val, float radius = 0.1f, Color? c = null, float duration = 0)
	{
		Color color = c ?? Color.white;
		int n = (int)(20 * radius / 0.2f);
		float dfi = Mathf.PI * 2.0f / (float)n;
		for (float fi = 0.0f; fi < Mathf.PI * 2.0f; fi += dfi)
		{
			Debug.DrawLine(val.ToVector2() + new Vector2(Mathf.Cos(fi) * radius, Mathf.Sin(fi) * radius),
				val.ToVector2() + new Vector2(Mathf.Cos(fi + dfi) * radius, Mathf.Sin(fi + dfi) * radius), color, duration);
		}
	}

	public static void DrawVector3(this Vector3 val, Color color)
	{
		val.DrawVector3(0.1f, color);
	}

	public static Vector2 ToVector2(this Vector3 val)
	{
		Vector2 result = val;
		return result;
	}

	public static Vector2 ToWorldPoint(this Vector2 val)
	{
		Vector2 result = Camera.main.ScreenToWorldPoint(val);
		return result;
	}

	public static Vector3 ToWorldPoint(this Vector3 val)
	{
		Vector2 result = Camera.main.ScreenToWorldPoint(val);
		return result;
	}

	public static Vector3 ToVector3(this Vector2 val)
	{
		Vector3 result = val;
		return result;
	}

	public static Vector3[] ToVector3Array(this Vector2[] arr)
	{
		Vector3[] result = new Vector3[arr.Length];
		for (int i = 0; i < arr.Length; i++)
		{
			result[i] = arr[i];
		}
		return result;
	}

	public static Vector2[] ToVector2Array(this Vector3[] arr)
	{
		Vector2[] result = new Vector2[arr.Length];
		for (int i = 0; i < arr.Length; i++)
		{
			result[i] = arr[i];
		}
		return result;
	}
}