//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//              Google Sheet Importer
//              Author: Nathan Crowe (Piggybacking off of some of Christopher Allport's Marvellous Work)
//              Date Created: February 13, 2024
//              Last Updated: March 07, 2024
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//		This script stores the information used to pull organised information
//		from Google Sheets as well as writing it to a definable location.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

using PlaySide;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Animal Warriors/Google Importers/Import Object", fileName = "New Google Sheet Importer")]
public class GSheetImporter : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField] private string googleDocId = "1Xj2goRWwj2FEE3fSFuIa4669sUiArNKwx4KHHVFmbuQ";
    [SerializeField] private string sheetId = "1932470863";
    [Space (10)]
    [SerializeField] private string directory = "./Assets/JSON/";

    //This is pretty much duct tape and will need to be revisited
    [SerializeField] private jsonDataType dataType = jsonDataType.troop;
    [SerializeField] private TroopList troopList;
    [SerializeField] private CardList cardList;
    [SerializeField] private DifficultyList difficultyList;

    [SerializeField] private TroopModList troopModList;
    [SerializeField] private CardModList cardModList;
    [SerializeField] private GameModList gameModList;

    [SerializeField] private RoundSkipList roundSkipList;

    [SerializeField] private StorePackList storePackList;

    [SerializeField] private ChallengeList challengeList;
    private bool applyToPrefab = true;

    public enum jsonDataType { troop, card, difficulty, troopMod, cardMod, gameMod, roundSkip, storePack, challenge};

    private List<string[]> allRowsContents = new List<string[]>();

    protected Dictionary<string, List<string>> allColumnsContents = new Dictionary<string, List<string>>();

    [System.NonSerialized] private System.Action _onFinished;

    public void BeginImportProcess(System.Action _onFinished)
    {
        this._onFinished = _onFinished;
        GoogleSpreadsheetStream.FetchSpreadsheetEditorMode(googleDocId, sheetId, OnGoogleFetchRequestFinished);
    }

    private void OnGoogleFetchRequestFinished(GoogleSpreadsheetStream _gss, GoogleSpreadsheetStream.FetchStatus _fetchResult)
    {
        if (_fetchResult == GoogleSpreadsheetStream.FetchStatus.Failed)
        {
            Debug.LogError("Failed to Fetch Sheet. ");
            return;
        }
        OrganizeRowsData(_gss.DocumentContents);
    }

    private void OrganizeRowsData(string _textAsset)
    {
        allRowsContents = new List<string[]>();
        string locTableContents = _textAsset.Replace("\r\n", "\n");
        string[] locTableRows = locTableContents.Split('\n');
        for (int rowNo = 3; rowNo < locTableRows.Length; rowNo++)
        {
            string rowContents = locTableRows[rowNo];
            if (string.IsNullOrEmpty(rowContents.Trim()) == false)
            {
                string[] stringsRead = rowContents.Split('\t');//(rowContents.Substring(rowContents.IndexOf('\t'))).Split('\t');
                allRowsContents.Add(stringsRead);
            }
        }
        CreateJsonFolder(allRowsContents);
    }
    private void CreateJsonFolder(List<string[]> allRowsContents)
    {
        string outputDirectory = directory;
        if (!outputDirectory.EndsWith("/"))
            outputDirectory += "/";

        Directory.CreateDirectory(outputDirectory);

        switch (dataType)
        {
            case jsonDataType.troop:
                if (applyToPrefab && troopList != null)
                    troopList.list.Clear();
                break;
            case jsonDataType.card:
                if (applyToPrefab && cardList != null)
                    cardList.list.Clear();
                break;
            case jsonDataType.difficulty:
                if (applyToPrefab && difficultyList != null)
                    difficultyList.list.Clear();
                break;
            case jsonDataType.troopMod:
                if (applyToPrefab && troopModList != null)
                    troopModList.list.Clear();
                break;
            case jsonDataType.cardMod:
                if (applyToPrefab && cardModList != null)
                    cardModList.list.Clear();
                break;
            case jsonDataType.gameMod:
                if (applyToPrefab && gameModList != null)
                    gameModList.list.Clear();
                break;
            case jsonDataType.roundSkip:
                if (applyToPrefab && roundSkipList != null)
                    roundSkipList.list.Clear();
                break;
            case jsonDataType.storePack:
                if (applyToPrefab && storePackList != null)
                    storePackList.list.Clear();
                break;
            default:
                break;
        }

        foreach (var item in allRowsContents)
        {
            CreateJsonFile(item[0], item[1], outputDirectory);
        }

        ReadAllData();
        ImportFinished();
    }
    private void CreateJsonFile(string fileName, string dataString, string outputDirectory)
    {
        dataString = dataString.Replace("[", "[\n  ");
        dataString = dataString.Replace("]", "]\n");
        //Create directory to store the json file									
        string path = outputDirectory + fileName + ".json";
        StreamWriter strmWriter = new StreamWriter(path, false, System.Text.Encoding.UTF8);
        strmWriter.Write(dataString);
        strmWriter.Close();
        Debug.Log("Created: " + fileName + ".json");

        switch (dataType)
        {
            case jsonDataType.troop:
                if (applyToPrefab && troopList != null)
                {
                    VarClass.troopClass temp = JsonUtility.FromJson<VarClass.troopClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < troopList.list.Count; i++)
                    {
                        if (troopList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        troopList.list.Add(temp);
                    else
                        troopList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadObject(temp.objAddressable), this);
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(troopList);
                }
                break;
            case jsonDataType.card:
                if (applyToPrefab && cardList != null)
                {
                    VarClass.cardClass temp = JsonUtility.FromJson<VarClass.cardClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < cardList.list.Count; i++)
                    {
                        if (cardList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        cardList.list.Add(temp);
                    else
                        cardList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(cardList);
                }
                break;
            case jsonDataType.difficulty:
                if (applyToPrefab && difficultyList != null)
                {
                    VarClass.difficultyClass temp = JsonUtility.FromJson<VarClass.difficultyClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < difficultyList.list.Count; i++)
                    {
                        if (difficultyList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        difficultyList.list.Add(temp);
                    else
                        difficultyList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(difficultyList);
                }
                break;
            case jsonDataType.troopMod:
                if (applyToPrefab && troopModList != null)
                {
                    VarClass.troopModifierClass temp = JsonUtility.FromJson<VarClass.troopModifierClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < troopModList.list.Count; i++)
                    {
                        if (troopModList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        troopModList.list.Add(temp);
                    else
                        troopModList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(troopModList);
                }
                break;
            case jsonDataType.cardMod:
                if (applyToPrefab && cardModList != null)
                {
                    VarClass.cardModifierClass temp = JsonUtility.FromJson<VarClass.cardModifierClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < cardModList.list.Count; i++)
                    {
                        if (cardModList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        cardModList.list.Add(temp);
                    else
                        cardModList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(cardModList);
                }
                break;
            case jsonDataType.gameMod:
                if (applyToPrefab && gameModList != null)
                {
                    VarClass.gameModifierClass temp = JsonUtility.FromJson<VarClass.gameModifierClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < gameModList.list.Count; i++)
                    {
                        if (gameModList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        gameModList.list.Add(temp);
                    else
                        gameModList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(gameModList);
                }
                break;
            case jsonDataType.roundSkip:
                if (applyToPrefab && roundSkipList != null)
                {
                    VarClass.roundSkipReward temp = JsonUtility.FromJson<VarClass.roundSkipReward>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < roundSkipList.list.Count; i++)
                    {
                        if (roundSkipList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        roundSkipList.list.Add(temp);
                    else
                        roundSkipList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(roundSkipList);
                }
                break;
            case jsonDataType.storePack:
                if (applyToPrefab && storePackList != null)
                {
                    VarClass.storePackClass temp = JsonUtility.FromJson<VarClass.storePackClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < storePackList.list.Count; i++)
                    {
                        if (storePackList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        storePackList.list.Add(temp);
                    else
                        storePackList.list[replaceID] = temp;
                    EditorCoroutineUtility.StartCoroutine(LoadSprite(temp.sprAddressable), this);
                    EditorUtility.SetDirty(storePackList);
                }
                break;
            case jsonDataType.challenge:
                if (applyToPrefab && challengeList != null)
                {
                    VarClass.challengeClass temp = JsonUtility.FromJson<VarClass.challengeClass>(dataString);
                    int replaceID = -1;
                    for (int i = 0; i < challengeList.list.Count; i++)
                    {
                        if (challengeList.list[i].name == temp.name)
                            replaceID = i;
                    }
                    if (replaceID == -1)
                        challengeList.list.Add(temp);
                    else
                        challengeList.list[replaceID] = temp;
                    EditorUtility.SetDirty(challengeList);
                }
                break;
            default:
                break;
        }
    }

    public IEnumerator LoadObject(VarClass.AddressableObjectClass addressable)
    {
        if (addressable.id != "N/A" && addressable.id != "")
        {
            AsyncOperationHandle<GameObject> opHandle = Addressables.LoadAssetAsync<GameObject>(addressable.id);
            yield return opHandle;
            if (opHandle.Status == AsyncOperationStatus.Succeeded)
                addressable.obj = opHandle.Result;
            else
                Debug.LogWarning("Couldn't find " + addressable.id + " in Addressables");
        }
    }
    public IEnumerator LoadSprite(VarClass.AddressableSpriteClass addressable)
    {
        if (addressable.id != "N/A" && addressable.id != "")
        {
            AsyncOperationHandle<Sprite> opHandle = Addressables.LoadAssetAsync<Sprite>(addressable.id);
            yield return opHandle;
            if (opHandle.Status == AsyncOperationStatus.Succeeded)
                addressable.spr = opHandle.Result;
            else
                Debug.LogWarning("Couldn't find " + addressable.id + " in Addressables");
        }
    }

    protected virtual void ReadAllData()
    {

    }

    protected virtual void ImportFinished()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.Refresh();
        _onFinished?.Invoke();
        _onFinished = null;
    }
#endif
}