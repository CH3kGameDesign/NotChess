using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EdUtils
{
	public static string[] scientificAbbreviations = { "", "K", "M", "B", "t", "q", "Q", "s", "S", "o", "n", "d", "U", "D", "T", "Qt", "Qd", "Sd", "St", "O", "N", "v", "c" };
	public static string[] clickerAbbreviations = { "", "K", "M", "B", "T" };
	static readonly int charA = 'A';
	
    public static string ConvertToMoveString(this VarClass.intArray[] _i, string _middleString)
    {
		string _temp = "";
        for (int x = 0; x < _i.Length; x++)
        {
			for (int y = 0; y < _i[x].array.Length; y++)
			{
				string _tarSprite = "";
				if (x == 3 && y == 3 && _middleString != "")
					_tarSprite = _middleString;
				else
					_tarSprite = _i[x].array[y].ToString();
				if (x == 0 || x == 6 || y == 0 || y == 6)
					_tarSprite += "a";

				_temp += _tarSprite.ToSpriteString();
			}
			_temp += "<br>";
		}


		return _temp;
    }
	public static string ConvertToMoveString(this VarClass.intArray[] _i)
    {
		return _i.ConvertToMoveString("-1");
    }
	public static string ConvertToEffectString(this VarClass.intArray[] _i, string _middleString)
	{
		return _i.ConvertToMoveString(_middleString);
	}
	public static string AbbreviatedString(this int i, string stringFormat = "Int_To_AA_Notation")
	{
		return ((float)i).AbbreviatedString(stringFormat);
	}
	public static string AbbreviatedString(this long i)
	{
		return ((double)i).AbbreviatedString();
	}
	public static string AbbreviatedString(this float f, string stringFormat = "Int_To_AA_Notation")
	{
		if (float.IsNaN(f))
			return (-1).ToString();

		switch (stringFormat)
		{
			case "Float":
				return f.ToString("N2");
			case "Percent":
				return f.ToString("P");
			case "Integer":
				return Mathf.Round(f).ToString();
			case "Float_To_AA_Notation":
				if (f >= 1000f)
					return AANotation(f);
				return f.ToString("N2");
			case "Int_To_AA_Notation":
				if (f >= 1000f)
					return AANotation(f);
				return Mathf.Round(f).ToString();
			case "Multiplier":
				return "x" + f.ToString("N2");
			case "Duration":
				return ToDuration(f);
			default:
				Debug.LogWarning("stringFormat " + stringFormat + " can not be found");
				return f.ToString("N2");
		}
	}
	public static string AbbreviatedString(this double d, string stringFormat = "Int_To_AA_Notation")
	{
		if (d < double.Epsilon)
			return "0";

		switch (stringFormat)
		{
			case "Float":
				return d.ToString("N2");
			case "Percent":
				return d.ToString("P");
			case "Integer":
				return Mathf.Round((float)d).ToString();
			case "Float_To_AA_Notation":
				if (d >= 1000d)
					return AANotation(d);
				return d.ToString("N2");
			case "Int_To_AA_Notation":
				if (d >= 1000d)
					return AANotation(d);
				return Mathf.Round((float)d).ToString();
			case "Multiplier":
				return "x" + d.ToString("N2");
			case "Duration":
				return ToDuration((float)d);
			default:
				Debug.LogWarning("stringFormat " + stringFormat + " can not be found");
				return d.ToString("N2");
		}
	}

	public static string AANotation(float f)
	{
		int n = (int)Math.Log(f, 1000d);
		double m = f / Math.Pow(1000d, n);
		string unit = "";

		if (n < 0)
			return "0";
		if (n < clickerAbbreviations.Length)
			unit = clickerAbbreviations[n];
		else
		{
			int unitInt = n - clickerAbbreviations.Length;
			int secondUnit = unitInt % 26;
			int firstUnit = unitInt / 26;
			unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
		}

		// Math.Floor(m * 100) / 100) fixes rounding errors
		if (m < 10)
			return (Math.Floor(m * 100d) / 100d).ToString("N2") + unit;
		if (m < 100)
			return (Math.Floor(m * 100d) / 100d).ToString("N1") + unit;

		return (Math.Floor(m * 100d) / 100d).ToString("N0") + unit;
	}

	public static string AANotation(double d)
    {
		int n = (int)Math.Log(d, 1000d);
		double m = d / Math.Pow(1000d, n);
		string unit = "";
		if (n < 0)
			return "0";
		if (n < clickerAbbreviations.Length)
			unit = clickerAbbreviations[n];
		else
		{
			int unitInt = n - clickerAbbreviations.Length;
			int secondUnit = unitInt % 26;
			int firstUnit = unitInt / 26;
			unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
		}

		// Math.Floor(m * 100) / 100) fixes rounding errors
		if (m < 10)
			return (Math.Floor(m * 100d) / 100d).ToString("N2") + unit;
		if (m < 100)
			return (Math.Floor(m * 100d) / 100d).ToString("N1") + unit;

		return (Math.Floor(m * 100d) / 100d).ToString("N0") + unit;
	}


	public static float AAValue(this float f)
	{
		int n = (int)Mathf.Log(f, 1000f);
		float m = f / Mathf.Pow(1000f, n);
		return Mathf.Floor(m) * n * 1000;
	}

	public static double AAValue(this double d)
	{
		int n = (int)Math.Log(d, 1000d);
		double m = d / Math.Pow(1000d, n);
		return Math.Floor(m) * n * 1000;
	}

	public static bool AbbreviatedStringMatches(this int a, int b) => string.Equals(a.AbbreviatedString(), b.AbbreviatedString());
	public static bool AbbreviatedStringMatches(this double a, double b) => double.IsNaN(a) || double.IsNaN(b) || string.Equals(a.AbbreviatedString(), b.AbbreviatedString());
	
	public static string ToDuration(this float amt)
	{
		string minutes = Mathf.FloorToInt(amt / 60).ToString();
		if (minutes.Length == 1) minutes = "0" + minutes;

		string seconds = Mathf.RoundToInt(amt % 60).ToString();
		if (seconds.Length == 1) seconds = "0" + seconds;

		return minutes + ":"+ seconds;
	}

	public static string ToPercent(this float value)
	{
		return (Mathf.Round(value * 10_000) / 100).ToString() + "%";
	}

	public static string ToCoinCost(this float value)
	{
		return "<sprite name=\"Coins\">" + value.AbbreviatedString();
	}
	public static string ToCoinCost(this double d)
	{
		return "<sprite name=\"Coins\">" + d.AbbreviatedString();
	}
	public static string ToSpriteString(this string s)
	{
		return "<sprite name=\""+s+"\">";
	}
	public static string ToCashAmount(this int value, int _cutoff)
	{
		if (value <= _cutoff)
        {
			string temp = "";
			for (int i = 0; i < value; i++)
				temp += "$";
			return temp;
        }
			else
		return "$" + value.AbbreviatedString();
	}

	public static void SetActive(this CanvasGroup _canvasGroup, bool _active, bool _modifyInteractables = true)
	{
		_canvasGroup.alpha = _active ? 1 : 0;
		if (_modifyInteractables)
		{
			_canvasGroup.blocksRaycasts = _modifyInteractables && _active;
			_canvasGroup.interactable = _modifyInteractables && _active;
		}
	}

	public static bool IsCanvasGroupActive(this CanvasGroup _canvasGroup)
	{
		// Mark as active if the alpha is greater than 0
		bool result = _canvasGroup.alpha > 0;
		return result;
	}

	private static System.Random rng = new System.Random();

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
