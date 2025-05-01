//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//              Google Spreadsheet Stream
//              Author: Christopher Allport
//              Date Created: February 20, 2020
//              Last Updated: August 7, 2020
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//		  This Script handles the fetch request between Unity and Googles.
//      It converts the downloaded bytes into string format and returns the
//      data via a callback function.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

//#pragma warning disable 0618

namespace PlaySide
{
    public class GoogleSpreadsheetStream
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //	* Declarations
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public enum FetchStatus
        {
            Running,
            Failed,
            Success
        }

        public delegate void ResultCallback(GoogleSpreadsheetStream _gss, FetchStatus _result);

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //	* Non-Inspector Fields
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected ResultCallback onFetchEnded;
        protected UnityWebRequest googleCredentialsRequest = null;
        protected UnityWebRequest webRequestHandler = null;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //	* Properties
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public FetchStatus Status { get; protected set; }
        public string DocumentID { get; protected set; }
        public string SheetID { get; protected set; }
        public string DocumentContents { get; protected set; }

        public string FullURL
        {
            get
            {
                string url = "https://docs.google.com/spreadsheets/d/" + DocumentID + "/export?format=tsv";
                if (string.IsNullOrEmpty(SheetID) == false)
                {
                    url += "&gid=" + SheetID;
                }
                return url;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //	* Constructor
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected GoogleSpreadsheetStream(string _spreadsheetDocId, string _sheetId, ResultCallback _onFetchResolved)
        {
            this.onFetchEnded = _onFetchResolved;
            this.Status = FetchStatus.Running;
            this.DocumentID = _spreadsheetDocId;
            this.SheetID = _sheetId;
            this.DocumentContents = string.Empty;

            this.webRequestHandler = UnityWebRequest.Get(FullURL);
            this.webRequestHandler.SendWebRequest();
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //	* Static Methods
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
        /// <summary> Fetches from the Google Spreadsheet during Editor Mode. </summary>
        /// <param name="_spreadsheetDocId">ID of the document.</param>
        /// <param name="_sheetId">GID of the sheet. If the GID is 0, set this as null.</param>
        /// <param name="_onFetchEnded">Callback to handle the output of the doc.</param>
        public static GoogleSpreadsheetStream FetchSpreadsheetEditorMode(string _spreadsheetDocId, string _sheetId, ResultCallback _onFetchEnded)
        {
            GoogleSpreadsheetStream gss = new GoogleSpreadsheetStream(_spreadsheetDocId, _sheetId, _onFetchEnded);
            UnityEditor.EditorApplication.update += gss.EditorUpdate;
            return gss;
        }
#endif

        /// <summary> Fetches from the Google Spreadsheet during Editor Mode. </summary>
        /// <param name="_spreadsheetDocId">ID of the document.</param>
        /// <param name="owningScript">The caller of this Fetch Request. We need an object that can run a Coroutine for us.</param>
        /// <param name="_sheetId">GID of the sheet. If the GID is 0, set this as null.</param>
        /// <param name="_onFetchEnded">Callback to handle the output of the doc.</param>
        public static GoogleSpreadsheetStream FetchSpreadsheet(string _spreadsheetDocId, MonoBehaviour owningScript, ResultCallback _onFetchEnded, string _sheetId = null)
        {
            // During Application Play we SHOULD be using Coroutines.
            if (owningScript == null)
            {
                throw new UnityException($"{nameof(FetchSpreadsheet)} Invoked. However, you CANNOT have the {nameof(owningScript)} be NULL. This is a critical item");
            }

            GoogleSpreadsheetStream gss = new GoogleSpreadsheetStream(_spreadsheetDocId, _sheetId, _onFetchEnded);
            owningScript.StartCoroutine(gss.GoogleDocInteractionCoroutine());
            return gss;
        }

        /// <summary> Removes html tags and replaces them with readable C# equivalents. </summary>
        /// <param name="_value">The contents of the CSV File.</param>
        /// <returns>A string with html tags removed and replaced with C# equivalents if applicable.</returns>
        public static string DecodeHTMLChars(string _value)
        {
            char[] TRIM_CHARS = new char[] { '\"' };
            string output = _value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS)
                                  .Replace("\\", "")
                                  .Replace("<br>", "\n")
                                  .Replace("<c>", ",");
            return output;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //	* Instance Methods
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
        protected void EditorUpdate()
        {
            UpdateFetchRequest();

            if (Status != FetchStatus.Running)
            {
                UnityEditor.EditorApplication.update -= this.EditorUpdate;
            }
        }
#endif

        protected IEnumerator GoogleDocInteractionCoroutine()
        {
            while (Status == FetchStatus.Running)
            {
                UpdateFetchRequest();
                yield return null;
            }
        }

        protected void UpdateFetchRequest()
        {
            if (webRequestHandler.result != UnityWebRequest.Result.InProgress && webRequestHandler.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to communicate with \"{FullURL}\"... Error returned as {webRequestHandler.error}");
                Status = FetchStatus.Failed;
                onFetchEnded?.Invoke(this, Status);
                return;
            }

            if (webRequestHandler.downloadHandler.isDone == false)
            {
                // Still ongoing, check if we're finished again next frame.
                return;
            }

            Status = FetchStatus.Success;
            DocumentContents = webRequestHandler.downloadHandler.text;
            onFetchEnded?.Invoke(this, Status);
        }
    }
}