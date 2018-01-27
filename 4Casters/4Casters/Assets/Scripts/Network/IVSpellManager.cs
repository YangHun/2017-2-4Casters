using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { neutral, holy, evil, lightness, darkness, Null }

public class IVSpellManager : MonoBehaviour {
    public Dictionary<string, SkillType> KeywordDictionary = new Dictionary<string, SkillType>()
    {
       //Neutral Keywords
       { "을" ,SkillType.neutral },
       { "를" ,SkillType.neutral },
       { "에게" ,SkillType.neutral },
       { "가" ,SkillType.neutral },
       { "이" ,SkillType.neutral },

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

    public Dictionary<SkillType, List<string>> SkillTypeDictionary = new Dictionary<SkillType, List<string>>()
    {
        { SkillType.neutral, new List<string>() { "을", "를", "에게", "가", "이" }},
        { SkillType.holy, new List<string>() { "선1", "선2", "선3","선4", "선5" }},
        { SkillType.evil, new List<string>() { "악1", "악2", "악3","악4", "악5" }},
        { SkillType.lightness, new List<string>() { "빛1", "빛2", "빛3","빛4", "빛5" }},
        { SkillType.darkness, new List<string>() { "어둠1", "어둠2", "어둠3", "어둠4", "어둠5" }}
    };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
