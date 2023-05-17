using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData data;
    GameManager gm;

    public VolPref volData;
    public PlayerProgress progData;
    public CharInfo charData;
    public Inventory inventoryData;

    public string volDataFileName;
    public string progDataFileName;
    public string charDataFileName;
    public string inventoryDataFileName;

    public bool doneInit;


    void Awake()
    {
        data = this;

    }
    private void Start()
    {
        gm = GameManager.gm;
    }

    public void LoadData()
    {
        StartCoroutine(InitFiles());
    }

    IEnumerator InitFiles()
    {
        yield return new WaitUntil(() => gm != null);
        volData = FileExists(volDataFileName) ? JsonUtility.FromJson<VolPref>(ReadText(volDataFileName)) : new VolPref();
        if (!FileExists(volDataFileName))
        {
            volData.SFXVol = 0.5f;
            volData.musicVol = 0.5f;
        }

        progData = FileExists(progDataFileName) ? JsonUtility.FromJson<PlayerProgress>(ReadText(progDataFileName)) : new PlayerProgress();
        if (!FileExists(progDataFileName))
        {
            progData.levelNum = gm.originalLevel;
            progData.isNewGame = true;
            progData.haveReadTutorial = false;
        }

        charData = FileExists(charDataFileName) ? JsonUtility.FromJson<CharInfo>(ReadText(charDataFileName)) : new CharInfo();
        if (!FileExists(charDataFileName))
        {
            charData.stamina = new int[(int)eChar.none];
            for (int i = 0; i < charData.stamina.Length; i++)
            {
                charData.stamina[i] = gm.originalStamina;
            }
        }


        inventoryData = FileExists(inventoryDataFileName) ? JsonUtility.FromJson<Inventory>(ReadText(inventoryDataFileName)) : new Inventory();
        if (!FileExists(inventoryDataFileName))
        {
            inventoryData.ingredients = new int[(int)eChar.none];
            for (int i = 0; i < inventoryData.ingredients.Length; i++)
            {
                inventoryData.ingredients[i] = gm.originalIngredient;
            }

            inventoryData.money = gm.originalMoney;
        }

        doneInit = true;
    }

    bool FileExists(string _fileName)
    {
        return System.IO.File.Exists(Application.persistentDataPath + _fileName);
    }

    string ReadText(string _fileName)
    {
        return System.IO.File.ReadAllText(Application.persistentDataPath + _fileName);
    }

    void WriteData(string _fileName, string _data)
    {
        System.IO.File.WriteAllText(Application.persistentDataPath + _fileName, _data);
    }

    public void SavePrefData()
    {
        string vol = JsonUtility.ToJson(volData);
        WriteData(volDataFileName, vol);
    }

    public void SaveProgress()
    {
        string prog = JsonUtility.ToJson(progData);
        WriteData(progDataFileName, prog);
    }

    public void SaveChar()
    {
        string charInfo = JsonUtility.ToJson(charData);
        WriteData(charDataFileName, charInfo);
    }

    public void SaveInventory()
    {
        string inventory = JsonUtility.ToJson(inventoryData);
        WriteData(inventoryDataFileName, inventory);
    }

    public void ResetGameData()
    {
        gm = GameManager.gm;
        progData.levelNum = gm.originalLevel;
        Debug.Log("level num: " + progData.levelNum);
        progData.isNewGame = true;
        Debug.Log("is new game: " + progData.isNewGame);
        for (int i = 0; i < charData.stamina.Length; i++)
        {
            charData.stamina[i] = gm.originalStamina;
            Debug.Log("char" + i + " stamina: " + charData.stamina[i]);
            inventoryData.ingredients[i] = gm.originalIngredient;
            Debug.Log("ingredient" + i + ": " + inventoryData.ingredients[i]);
        }
        inventoryData.money = gm.originalMoney;
        Debug.Log("money: " + inventoryData.money);
        SaveProgress();
        SaveChar();
        SaveInventory();
        Debug.Log("recheck: level num: " + progData.levelNum);
        Debug.Log("recheck: is new game: " + progData.isNewGame);
        for (int i = 0; i < charData.stamina.Length; i++)
        {
            Debug.Log("recheck: char" + i + " stamina: " + charData.stamina[i]);
            Debug.Log("recheck: ingredient" + i + ": " + inventoryData.ingredients[i]);
        }
        Debug.Log("recheck: money: " + inventoryData.money);

    }

}

[System.Serializable]
public class VolPref
{
    public float SFXVol;
    public float musicVol;
}

[System.Serializable]
public class PlayerProgress
{
    public int levelNum;
    public bool isNewGame;
    public bool haveReadTutorial;
}

[System.Serializable]
public class CharInfo
{
    [NamedArray(typeof(eChar))] public int[] stamina;
}

[System.Serializable]
public class Inventory
{
    public int money;
    [NamedArray(typeof(eIngredient))] public int[] ingredients;

}