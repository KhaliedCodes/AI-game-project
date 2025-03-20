using System.Collections;
using UnityEngine;

public class SkillsFunctions
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static void activate(GameObject skill)
    {
        skill.SetActive(true);
        // deactivateShield(shield);
    }

    public static void deactivate(GameObject skill)
    {
        skill.SetActive(false);
    }


}
