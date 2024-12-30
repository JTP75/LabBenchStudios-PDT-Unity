/**
 * MIT License
 * 
 * Copyright (c) 2024 - 2025 Andrew D. King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using LabBenchStudios.Pdt.Common;

namespace LabBenchStudios.Pdt.Unity.Hud
{
    public class DataHistorianCacheHandler : MonoBehaviour, IDataHistorianFileEventListener
    {
        [SerializeField]
        private GameObject selectedPathObject = null;

        [SerializeField]
        private GameObject selectedFileObject = null;

        [SerializeField]
        private GameObject fileErrorObject = null;

        [SerializeField]
        private GameObject pathListingContainerObject = null;

        [SerializeField]
        private GameObject pathListingEntryObject = null;

        [SerializeField]
        private GameObject fileListingContainerObject = null;

        [SerializeField]
        private GameObject fileListingEntryObject = null;

        [SerializeField]
        private GameObject clearCacheButtonObject = null;

        [SerializeField]
        private GameObject deleteCacheButtonObject = null;

        [SerializeField]
        private GameObject loadCacheButtonObject = null;

        [SerializeField]
        private GameObject saveCacheButtonObject = null;

        private Text selectedPathText = null;
        private Text selectedFileText = null;
        private Text fileErrorText = null;

        private Button clearCacheButton = null;
        private Button deleteCacheButton = null;
        private Button loadCacheButton = null;
        private Button saveCacheButton = null;

        private IDataHistorianCache dataHistorianCache = null;

        private List<GameObject> fileEntryList = new List<GameObject>();
        private List<GameObject> pathEntryList = new List<GameObject>();

        private string selectedPath = null;
        private string selectedFile = null;
        private float verticalAnchorDelta = 400.0f;


        // auto-invoked methods

        private void Start()
        {
            this.InitControls();
        }


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ClearCache()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool LoadCache()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SaveCache()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableClearCacheButton(bool enable)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableLoadCacheButton(bool enable)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableSaveCacheButton(bool enable)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClearCache()
        {
            Debug.Log("Clear cache button clicked.");
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnDeleteCache()
        {
            Debug.Log("Delete cache button clicked.");
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnLoadCache()
        {
            Debug.Log("Load cache button clicked.");
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void OnSaveCache()
        {
            Debug.Log("Save cache button clicked.");
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnFileSelected(string entry)
        {
            Debug.Log($"File selection triggered: {entry}");

            if (this.selectedFileText != null)
            {
                if (this.selectedFile != entry)
                {
                    Debug.Log($"New file selected: {entry}. Old path: {this.selectedFile}");

                    this.ClearFileEntries();
                }

                this.selectedFile = entry;
                this.selectedFileText.text = this.selectedFile;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnPathSelected(string entry)
        {
            Debug.Log($"Path selection triggered: {entry}");

            if (this.selectedPathText != null)
            {
                // todo: clear existing path list
                // todo: clear existing file list and add new file listing to UI
                if (this.selectedPath != entry)
                {
                    Debug.Log($"New path selected: {entry}. Old path: {this.selectedPath}");

                    this.ClearFileEntries();
                }

                this.selectedPath = entry;
                this.selectedPathText.text = this.selectedPath;

                // todo: load all files for selected path
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public void OnSelectionError(string targetName, string msg, Exception e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool OnFileLoaded(string fileName, int bytes)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool OnFileSaved(string fileName, int bytes)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool OnFileDeleted(string fileName)
        {
            return false;
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void InitControls()
        {
            if (this.fileErrorObject != null)
            {
                this.fileErrorText = this.fileErrorObject.GetComponent<Text>();
            }

            if (this.clearCacheButtonObject != null)
            {
                this.clearCacheButton = this.clearCacheButtonObject.GetComponent<Button>();

                if (this.clearCacheButton != null)
                {
                    this.clearCacheButton.onClick.AddListener(() => this.OnClearCache());
                }
            }

            if (this.deleteCacheButtonObject != null)
            {
                this.deleteCacheButton = this.deleteCacheButtonObject.GetComponent<Button>();

                if (this.deleteCacheButton != null)
                {
                    this.deleteCacheButton.onClick.AddListener(() => this.OnDeleteCache());
                }
            }

            if (this.loadCacheButtonObject != null)
            {
                this.loadCacheButton = this.loadCacheButtonObject.GetComponent<Button>();

                if (this.loadCacheButton != null)
                {
                    this.loadCacheButton.onClick.AddListener(() => this.OnLoadCache());
                }
            }

            if (this.saveCacheButtonObject != null)
            {
                this.saveCacheButton = this.saveCacheButtonObject.GetComponent<Button>();

                if (this.saveCacheButton != null)
                {
                    this.saveCacheButton.onClick.AddListener(() => this.OnSaveCache());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        private void AddFileEntries(string[] entries)
        {
            float curYPosDelta = 0.0f;

            foreach (string entry in entries)
            {
                this.CreateFileEntry(curYPosDelta, entry);
                curYPosDelta += this.verticalAnchorDelta;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        private void AddPathEntries(string[] entries)
        {
            float curYPosDelta = 0.0f;

            foreach (string entry in entries)
            {
                this.CreatePathEntry(curYPosDelta, entry);
                curYPosDelta += this.verticalAnchorDelta;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yPosDelta"></param>
        /// <param name="entryName"></param>
        private void CreateFileEntry(float yPosDelta, string entryName)
        {
            GameObject go =
                this.CreateDirEntry(this.fileListingEntryObject, this.fileListingContainerObject, yPosDelta, entryName);
            
            if (go != null)
            {
                go.AddComponent<FileEntryListener>();
                this.fileEntryList.Add(go);

                try
                {
                    FileEntryListener listener = go.GetComponent<FileEntryListener>();
                    listener.SetListenerCallback(this);
                } catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yPosDelta"></param>
        /// <param name="entryName"></param>
        private void CreatePathEntry(float yPosDelta, string entryName)
        {
            GameObject go =
                this.CreateDirEntry(this.pathListingEntryObject, this.pathListingContainerObject, yPosDelta, entryName);

            if (go != null)
            {
                go.AddComponent<PathEntryListener>();
                this.pathEntryList.Add(go);
            }

            try
            {
                FileEntryListener listener = go.GetComponent<FileEntryListener>();
                listener.SetListenerCallback(this);
            } catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryObject"></param>
        /// <param name="containerObject"></param>
        /// <param name="yPosDelta"></param>
        /// <param name="entryName"></param>
        /// <returns></returns>
        private GameObject CreateDirEntry(GameObject entryObject, GameObject containerObject, float yPosDelta, string entryName)
        {
            if (entryObject != null)
            {
                GameObject go = Instantiate(entryObject, containerObject.transform);

                try
                {
                    TMP_Text textEntry = go.GetComponent<TextMeshProUGUI>();
                    textEntry.text = entryName;
                } catch (Exception e)
                {
                    Debug.LogError($"Can't create text for dir entry: {entryName}. Exception: {e.Message}");
                }

                this.AddDirEntry(containerObject, go, yPosDelta);

                return go;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="yPosDelta"></param>
        private void AddDirEntry(GameObject containerObject, GameObject go, float yPosDelta)
        {
            if (go != null)
            {
                // adjust location

                RectTransform propPosObj = go.GetComponent<RectTransform>();
                float xPos = propPosObj.anchoredPosition.x - 50.0f;

                if (containerObject != null)
                {
                    RectTransform parentPosObj = containerObject.GetComponent<RectTransform>();
                    xPos = parentPosObj.anchoredPosition.x - 5.0f;
                }

                Vector2 anchorPos = new Vector2(xPos, (propPosObj.anchoredPosition.y - yPosDelta));
                propPosObj.anchoredPosition = anchorPos;

                // activate component
                go.SetActive(true);

                Debug.Log($"Created GUI component for entry. Loc: {anchorPos}.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearFileEntries()
        {
            foreach (GameObject go in this.fileEntryList) {
                if (go != null)
                {
                    go.SetActive(false);
                    Destroy(go);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearPathEntries()
        {
            foreach (GameObject go in this.pathEntryList)
            {
                if (go != null)
                {
                    go.SetActive(false);
                    Destroy(go);
                }
            }
        }

        public bool DeleteDataHistorianCache(string pathName, string fileName)
        {
            return false;
        }

        public IDataHistorianCache LoadDataHistorianCache(string pathName, string fileName)
        {
            return null;
        }

        public IDataHistorianCache StoreDataHistorianCache(string pathName, string fileName)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public class FileEntryListener : MonoBehaviour
        {
            private string entryName = null;
            private IDataHistorianFileEventListener listener = null;

            private void Start()
            {
                try
                {
                    TMP_Text textEntry = gameObject.GetComponent<TextMeshProUGUI>();
                    this.entryName = textEntry.text;
                } catch (Exception e)
                {
                    Debug.LogError($"Can't create find TMP_Text in parent. Exception: {e.Message}");
                }
            }

            public void SetListenerCallback(IDataHistorianFileEventListener listener)
            {
                if (listener != null)
                {
                    this.listener = listener;
                }
            }

            public void OnMouseEnter()
            {
                if (this.listener != null)
                {
                    this.listener.OnFileSelected(this.entryName);
                }
            }

            public void OnMouseExit()
            {

            }

            public void OnMouseDown()
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class PathEntryListener : MonoBehaviour
        {
            private string entryName = null;
            private IDataHistorianFileEventListener listener = null;

            private void Start()
            {
                try
                {
                    TMP_Text textEntry = gameObject.GetComponent<TextMeshProUGUI>();
                    this.entryName = textEntry.text;
                } catch (Exception e)
                {
                    Debug.LogError($"Can't create find TMP_Text in parent. Exception: {e.Message}");
                }
            }

            public void SetListenerCallback(IDataHistorianFileEventListener listener)
            {
                if (listener != null)
                {
                    this.listener = listener;
                }
            }

            public void OnMouseEnter()
            {
                if (this.listener != null)
                {
                    this.listener.OnPathSelected(this.entryName);
                }
            }

            public void OnMouseExit()
            {

            }

            public void OnMouseDown()
            {

            }
        }

    }

}
