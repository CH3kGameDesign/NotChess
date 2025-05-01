//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//              Google Sheet Import Manager
//              Author: Nathan Crowe (Piggybacking off of some of Christopher Allport's Marvellous Work)
//              Date Created: February 13, 2024
//              Last Updated: February 13, 2024
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//		This script is for storing Google Sheet Importers and being able to import
//      them all at once.
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

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Animal Warriors/Google Importers/Import Manager", fileName = "New Google Sheet Import Manager")]
public class GSheetImportManager : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField] private List<GSheetImporter> importers = new List<GSheetImporter>();

    [System.NonSerialized] private System.Action _onFinished;

    public void BeginImportProcess(System.Action _onFinished)
    {
        this._onFinished = _onFinished;
        foreach (var item in importers)
        {
            item.BeginImportProcess(null);
        }
        ReadAllData();
        ImportFinished();
    }

    protected virtual void ReadAllData()
    {

    }

    protected virtual void ImportFinished()
    {
        _onFinished?.Invoke();
        _onFinished = null;
    }
#endif
}