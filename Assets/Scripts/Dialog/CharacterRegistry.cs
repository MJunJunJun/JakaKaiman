using System.Collections.Generic;
using UnityEngine;

public class CharacterRegistry : MonoBehaviour
{
    [Header("Characters")]
    public List<CharacterActor> characters;

    private Dictionary<string, CharacterActor> characterDictionary;

    private void Awake()
    {
        characterDictionary = new Dictionary<string, CharacterActor>();

        foreach (CharacterActor character in characters)
        {
            if (!characterDictionary.ContainsKey(character.characterID))
            {
                characterDictionary.Add(character.characterID, character);
            }
        }
    }

    public CharacterActor GetCharacter(string id)
    {
        if (characterDictionary.ContainsKey(id))
        {
            return characterDictionary[id];
        }

        Debug.LogWarning("Character not found: " + id);

        return null;
    }

    public void SetAllIdle()
    {
        foreach (CharacterActor character in characters)
        {
            character.PlayIdle();
        }
    }
}