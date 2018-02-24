using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { neutral, holy, evil, lightness, darkness, Null }

// Singletone of managing spell
public class IVSpellManager : MonoBehaviour {

	public static Dictionary<SkillType, int> emptyforce = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 0 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 }
	};

    public static Dictionary<string, SkillType> KeywordDictionary = new Dictionary<string, SkillType>()
    {
       //Neutral Keywords
       { "을"   , SkillType.neutral },
       { "를"   , SkillType.neutral },
       { "에게" , SkillType.neutral },
       { "가"   , SkillType.neutral },
       { "이"   , SkillType.neutral },

       //Holy Keywords
       { "질서" , SkillType.holy },
       { "임무" , SkillType.holy },
       { "정의" , SkillType.holy },
       { "정화" , SkillType.holy },
       { "가호" , SkillType.holy },

       //Evil Keywords
       { "천벌" , SkillType.evil },
       { "혼란" , SkillType.evil },
       { "성불" , SkillType.evil },
       { "영혼" , SkillType.evil },
       { "타락" , SkillType.evil },

       //Lightness Keywords
       { "광명" , SkillType.lightness },
       { "마력" , SkillType.lightness },
       { "성모" , SkillType.lightness },
       { "성령" , SkillType.lightness },
       { "안식" , SkillType.lightness },

       //Darkness Keywords
       { "무질서" , SkillType.darkness },
       { "사령" , SkillType.darkness },
       { "침묵" , SkillType.darkness },
       { "암흑" , SkillType.darkness },
       { "사망" , SkillType.darkness }
    };

    public static Dictionary<SkillType, List<string>> SkillTypeDictionary = new Dictionary<SkillType, List<string>>()
    {
        { SkillType.neutral, new List<string>() { "을", "를", "에게", "가", "이" }},
        { SkillType.holy, new List<string>() { "질서", "임무", "정의", "정화", "가호" }},
        { SkillType.evil, new List<string>() { "천벌", "혼란", "성불", "영혼", "타락" }},
        { SkillType.lightness, new List<string>() { "광명", "마력", "성모", "성령", "안식" }},
        { SkillType.darkness, new List<string>() { "무질서", "사령", "침묵", "암흑", "사망" }}
    };

	// Type Checker. It returns true if str is noun; its type is neutral
	public static bool IsNoun(string str)
	{
		if (KeywordDictionary[str] == SkillType.neutral) return false;
		else return true;
	}

	// This function checks whether syntax is legal, and the type is attack or buff.
	// Invalid is 0, attack is 1, buff is 2, and debuff is 3.
	public static int SyntaxCheck(List<string> sentence)
	{
		bool isNoun = true;
		int count = 0;
		foreach (string s in sentence)  // Syntax Checking
		{
			count++;
			if (isNoun != IsNoun(s)) return 0;
			isNoun = !isNoun;
		}
		switch(count)
		{
			case 0:
				return 0;
			case 2:
				return 3;
			case 3:
				return 2;
			default:
				return 1;
		}
	}

	public static Dictionary<SkillType, int> ForceCalculator(List<string> sentence, Dictionary<SkillType, int> basis)
	{
		Dictionary<SkillType, int> force = new Dictionary<SkillType, int>();
		foreach(SkillType type in new List<SkillType>(basis.Keys))
			force[type] = basis[type];
		foreach(string s in sentence)
			force[KeywordDictionary[s]] += 1;
		return force;
	}

	public static int DamageCalculator(Dictionary <SkillType, int> f, Dictionary <SkillType, int> s)
	{
		int dmg = 0;
		int part = 0;
		for (int i = 0; i < (int)SkillType.Null; i++)
		{
			part = f[(SkillType)i] - s[(SkillType)i];
			part = part > 0 ? part : 0;
			dmg += part;
		}
		return dmg;
	}
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

// Exceptions of Spell
public class BrokenSyntaxException : System.Exception
{
	private int num = 0;                //position where syntax has just been broken.
	BrokenSyntaxException() { }
	BrokenSyntaxException(int num)
	{
		Num = num;
	}

	public int Num
	{
		get { return num; }
		set { num = value; }
	}
}
