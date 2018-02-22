using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { neutral, holy, evil, lightness, darkness, Null }

// Singletone of managing spell
public class IVSpellManager : MonoBehaviour {

    public static Dictionary<string, SkillType> KeywordDictionary = new Dictionary<string, SkillType>()
    {
       //Neutral Keywords
       { "을"   , SkillType.neutral },
       { "를"   , SkillType.neutral },
       { "에게" , SkillType.neutral },
       { "가"   , SkillType.neutral },
       { "이"   , SkillType.neutral },

       //Holy Keywords
       { "선1" , SkillType.holy },
       { "선2" , SkillType.holy },
       { "선3" , SkillType.holy },
       { "선4" , SkillType.holy },
       { "선5" , SkillType.holy },

       //Evil Keywords
       { "악1" , SkillType.evil },
       { "악2" , SkillType.evil },
       { "악3" , SkillType.evil },
       { "악4" , SkillType.evil },
       { "악5" , SkillType.evil },

       //Lightness Keywords
       { "빛1" , SkillType.lightness },
       { "빛2" , SkillType.lightness },
       { "빛3" , SkillType.lightness },
       { "빛4" , SkillType.lightness },
       { "빛5" , SkillType.lightness },

       //Darkness Keywords
       { "어둠1" , SkillType.darkness },
       { "어둠2" , SkillType.darkness },
       { "어둠3" , SkillType.darkness },
       { "어둠4" , SkillType.darkness },
       { "어둠5" , SkillType.darkness }
    };

    public static Dictionary<SkillType, List<string>> SkillTypeDictionary = new Dictionary<SkillType, List<string>>()
    {
        { SkillType.neutral, new List<string>() { "을", "를", "에게", "가", "이" }},
        { SkillType.holy, new List<string>() { "선1", "선2", "선3","선4", "선5" }},
        { SkillType.evil, new List<string>() { "악1", "악2", "악3","악4", "악5" }},
        { SkillType.lightness, new List<string>() { "빛1", "빛2", "빛3","빛4", "빛5" }},
        { SkillType.darkness, new List<string>() { "어둠1", "어둠2", "어둠3", "어둠4", "어둠5" }}
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
