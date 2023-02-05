using UnityEngine;

[System.Serializable]
public class EggQuestMap
{
	public int QuestID;
	public int EggItemID;
	public string SpeciesID;
	public string QuestName;
	public Color ForceTintColor = Color.clear;
	public bool GenerateRandomTint = false;



    public EggQuestMap(int questID, int eggItemID, string speciesID, string questName, Color forceTintColor, bool generateRandomTint)
    {
        QuestID = questID;
        EggItemID = eggItemID;
        SpeciesID = speciesID;
        QuestName = questName;
		ForceTintColor = forceTintColor;
		GenerateRandomTint = generateRandomTint;
	}
}