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
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Plexus;
using LabBenchStudios.Pdt.Util;

using LabBenchStudios.Pdt.Unity.Common;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class DataHistorianPlaybackHandler : BaseAsyncDataMessageProcessor,
        ISystemStatusEventListener, IDataHistorianFileEventListener
    {
        [SerializeField]
        private GameObject cachePathObject = null;

        [SerializeField]
        private GameObject cacheFileObject = null;

        [SerializeField]
        private GameObject cacheNameObject = null;

        [SerializeField]
        private GameObject loadedEntriesCountObject = null;

        [SerializeField]
        private GameObject fileListingContainerObject = null;

        [SerializeField]
        private GameObject statusMessageObject = null;

        [SerializeField]
        private GameObject clearCacheButtonObject = null;

        [SerializeField]
        private GameObject loadCacheButtonObject = null;

        [SerializeField]
        private GameObject reverseDataFlowObject = null;

        [SerializeField]
        private GameObject loopAtEndObject = null;

        [SerializeField]
        private GameObject cachedEntriesCountObject = null;

        [SerializeField]
        private GameObject currentCachedItemIndexObject = null;

        [SerializeField]
        private GameObject resetPlaybackFactorButtonObject = null;

        [SerializeField]
        private GameObject playbackTimingFactorSelectionObject = null;

        [SerializeField]
        private GameObject playbackTimingFactorTextObject = null;

        [SerializeField]
        private GameObject cachedEntriesIndexSelectionObject = null;

        [SerializeField]
        private GameObject selectedCachedItemIndexObject = null;

        [SerializeField]
        private GameObject cachedEntriesMidIndexObject = null;

        [SerializeField]
        private GameObject cachedEntriesMaxIndexObject = null;

        [SerializeField]
        private GameObject startPlaybackButtonObject = null;

        [SerializeField]
        private GameObject pausePlaybackButtonObject = null;

        [SerializeField]
        private GameObject stopPlaybackButtonObject = null;

        private TMP_Dropdown fileListingSelector = null;

        private Text cachePathText = null;
        private Text cacheFileText = null;
        private Text cacheNameText = null;
        private Text loadedEntriesCountText = null;
        private Text cachedEntriesCountText = null;
        private Text statusMsgText = null;

        private Button clearCacheButton = null;
        private Button loadCacheButton = null;

        private Toggle reverseDataFlowToggle = null;
        private Toggle loopAtEndToggle = null;

        private Slider playbackTimingFactorSlider = null;
        private Button resetPlaybackFactorButton = null;
        private Text playbackTimingFactorText = null;

        private Slider cachedEntriesIndexSlider = null;
        private Text cachedEntriesMidIndexText = null;
        private Text cachedEntriesMaxIndexText = null;
        private Text selectedCachedItemIndexText = null;
        private Text currentCachedItemIndexText = null;

        private Button startPlaybackButton = null;
        private Button pausePlaybackButton = null;
        private Button stopPlaybackButton = null;

        private bool isHistorianPanelActive = false;
        private bool isReverseDataFlowEnabled = false;
        private bool isLoopAtEndEnabled = false;
        private bool isEventCaptureEnabled = false;
        private bool enableIncomingTelemetry = false;

        private const float _PLAYBACK_FACTOR = 1.0f;

        private int curPlayingCachedItemIndex = 0;
        private float playbackTimingFactor= 0.0f;

        private string selectedCacheName = "";
        private string selectedFileName = "";
        private string selectedPathName = "";

        private IDataHistorianPlayer dataHistorianPlayer = null;
        private DataHistorianPlayerCoroutineProcessor playbackCoroutineProcessor = null;


        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void HandleUserEventState(UserEventState.EventType eventType)
        {
            switch (eventType)
            {
                case UserEventState.EventType.CloseOpenDialogs:
                    // todo
                    break;

                default:
                    // ignore
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnFileSelected()
        {
            Debug.Log("File selection triggered.");

            if (this.fileListingSelector != null)
            {
                string fileName = this.fileListingSelector.captionText.text;

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    if (!fileName.Equals(this.cacheFileText))
                    {
                        // NOTE: no need to change the cache name -
                        // it should remain as-is until the LOAD
                        // button is clicked
                        this.selectedFileName = fileName;
                        this.UpdateFileAndPathName();

                        // enable load file button
                        this.SetLoadButtonInteractable(true);
                    } else
                    {
                        Debug.Log($"User just selected the same file again: {fileName}. Ignoring.");

                        // disable load file button
                        this.SetLoadButtonInteractable(false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PauseIncomingTelemetry()
        {
            this.enableIncomingTelemetry = false;

        }

        /// <summary>
        /// 
        /// </summary>
        public void ResumeIncomingTelemetry()
        {
            this.enableIncomingTelemetry = true;

        }


        // public callback methods

        public void LogDebugMessage(string message)
        {
            base.HandleDebugLogMessage(message);
        }

        public void LogErrorMessage(string message, Exception ex)
        {
            base.HandleErrorLogMessage(message);
        }

        public void LogWarningMessage(string message)
        {
            base.HandleWarningLogMessage(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool DeleteDataHistorianCache(string pathName, string fileName)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IDataHistorianCache LoadDataHistorianCache(string pathName, string fileName)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IDataHistorianCache StoreDataHistorianCache(string pathName, string fileName)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnFileSelected(string fileName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnPathSelected(string pathName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnSelectionError(string targetName, string msg, Exception e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public bool OnFileLoaded(string fileName, int bytes)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public bool OnFileSaved(string fileName, int bytes)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="NotImplementedException"></exception>
        public bool OnFileDeleted(string fileName)
        {
            return false;
        }

        public void OnMessagingSystemDataReceived(ActuatorData data)
        {
            base.HandleActuatorData(data);
        }

        public void OnMessagingSystemDataReceived(ConnectionStateData data)
        {
            base.HandleConnectionStateData(data);
        }

        public void OnMessagingSystemDataReceived(SensorData data)
        {
            base.HandleSensorData(data);
        }

        public void OnMessagingSystemDataReceived(SystemPerformanceData data)
        {
            base.HandleSystemPerformanceData(data);
        }

        public void OnMessagingSystemDataSent(ConnectionStateData data)
        {
            base.HandleConnectionStateData(data);
        }

        public void OnMessagingSystemStatusUpdate(ConnectionStateData data)
        {
            base.HandleConnectionStateData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnModelUpdateEvent()
        {
        }

        //
        // event callbacks
        //

        /// <summary>
        /// 
        /// </summary>
        public void OnClearCacheClicked()
        {
            Debug.Log("Clear cache button clicked.");

            if (this.dataHistorianPlayer != null)
            {
                this.dataHistorianPlayer.Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnLoadCacheButtonClicked()
        {
            this.selectedCacheName = FileUtil.TrimFileExt(this.selectedFileName);
            
            if (this.cacheNameText != null)
            {
                this.cacheNameText.text = this.selectedCacheName;
            }

            Debug.Log($"Load cache button clicked. File: {this.selectedFileName}. Cache: {this.selectedCacheName}");

            this.InitPlayer();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnStartPlaybackButtonClicked()
        {
            Debug.Log("Start playback button clicked.");

            if (this.playbackCoroutineProcessor != null)
            {
                if (this.playbackCoroutineProcessor.IsProcessingPaused())
                {
                    this.playbackCoroutineProcessor.UnpausePlayback();
                } else
                {
                    this.playbackCoroutineProcessor.StartPlayback();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnStopPlaybackButtonClicked()
        {
            Debug.Log("Stop playback button clicked.");

            if (this.playbackCoroutineProcessor != null)
            {
                this.playbackCoroutineProcessor.StopPlayback();
            }
        }

        public void OnPausePlaybackButtonClicked()
        {
            Debug.Log("Pause playback button clicked.");

            if (this.playbackCoroutineProcessor != null)
            {
                this.playbackCoroutineProcessor.PausePlayback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnResetPlaybackFactorButtonClicked()
        {
            if (this.playbackTimingFactorSlider != null)
            {
                Debug.Log($"Playback timing factor value reset to 0.0f");

                // setting this value on the slider will invoke the listener
                // that updates the slider UI element and sets the
                // local var playbackTimingFactor
                this.playbackTimingFactorSlider.value = 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void OnPlaybackTimingFactorChanged(float val)
        {
            if (this.playbackTimingFactorSlider != null)
            {
                this.playbackTimingFactor = (float) NumberUtil.CalculateDelayFactor(val);

                Debug.Log($"Playback timing factor changed: {this.playbackTimingFactor}");

                this.UpdatePlaybackTimingFactor();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void OnCacheItemIndexChanged(float val)
        {
            if (this.cachedEntriesIndexSlider != null)
            {
                if (val >= 0)
                {
                    int index = (int) val;

                    Debug.Log($"Selected cache item index changed: {index}");

                    if (this.selectedCachedItemIndexText != null)
                    {
                        this.selectedCachedItemIndexText.text = index.ToString();
                    }

                    if (this.dataHistorianPlayer != null)
                    {
                        if (index < this.dataHistorianPlayer.GetCacheSize())
                        {
                            Debug.Log($"Setting starting (or new) cache index: {index}");

                            this.dataHistorianPlayer.SetStartingIndex(index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnReverseDataFlowToggleClicked()
        {
            Debug.Log("Reverse data flow toggle clicked.");
            this.isReverseDataFlowEnabled = this.reverseDataFlowToggle.isOn;

            if (this.isReverseDataFlowEnabled)
            {
                this.GetDataHistorianPlayer().SetPlaybackDirection(MediaPlayerState.PlaybackDirection.Reverse);
            } else
            {
                this.GetDataHistorianPlayer().SetPlaybackDirection(MediaPlayerState.PlaybackDirection.Forward);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnLoopAtEndToggleClicked()
        {
            Debug.Log("Loop at end toggle clicked.");

            this.isLoopAtEndEnabled = this.loopAtEndToggle.isOn;
            this.GetDataHistorianPlayer().SetEnableLoopAtEndFlag(this.isLoopAtEndEnabled);
        }


        // protected methods

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try
            {
                this.playbackCoroutineProcessor = this.gameObject.AddComponent<DataHistorianPlayerCoroutineProcessor>();

                this.InitPlaybackComponents();
                base.RegisterForSystemStatusEvents((ISystemStatusEventListener)this);

            } catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize digital twin HUD. Continuing without display data: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessDebugLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessWarningLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessErrorLogMessage(string message)
        {
            if (message != null)
            {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessActuatorData(ActuatorData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                if (this.IsDataHistorianPlayerInitialized())
                {
                    this.GetDataHistorianPlayer().HandleActuatorData(data);
                }

                this.ProcessNextHistorianItem();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                if (this.IsDataHistorianPlayerInitialized())
                {
                    this.GetDataHistorianPlayer().HandleConnectionStateData(data);
                }

                this.ProcessNextHistorianItem();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessMessageData(MessageData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                if (this.IsDataHistorianPlayerInitialized())
                {
                    this.GetDataHistorianPlayer().HandleMessageData(data);
                }

                this.ProcessNextHistorianItem();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSensorData(SensorData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                if (this.IsDataHistorianPlayerInitialized())
                {
                    this.GetDataHistorianPlayer().HandleSensorData(data);
                }

                this.ProcessNextHistorianItem();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                if (this.IsDataHistorianPlayerInitialized())
                {
                    this.GetDataHistorianPlayer().HandleSystemPerformanceData(data);
                }

                this.ProcessNextHistorianItem();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelListContainer"></param>
        protected override void ProcessModelListUpdate(ModelListContainer modelListContainer)
        {
            // nothing to do
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResponseContainer"></param>
        protected override void ProcessQueryResponseUpdate(QueryResponseContainer queryResponseContainer)
        {
            // nothing to do
        }


        // private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IDataHistorianPlayer GetDataHistorianPlayer()
        {
            return this.dataHistorianPlayer;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitPlaybackFileManagementComponents()
        {
            if (this.cachePathObject != null)
            {
                this.cachePathText = this.cachePathObject.GetComponent<Text>();
            }

            if (this.cacheFileObject != null)
            {
                this.cacheFileText = this.cacheFileObject.GetComponent<Text>();
            }

            if (this.cacheNameObject != null)
            {
                this.cacheNameText = this.cacheNameObject.GetComponent<Text>();
            }

            if (this.loadedEntriesCountObject != null)
            {
                this.loadedEntriesCountText = this.loadedEntriesCountObject.GetComponent<Text>();
            }

            if (this.fileListingContainerObject != null)
            {
                this.fileListingSelector = this.fileListingContainerObject.GetComponent<TMP_Dropdown>();

                if (this.fileListingSelector != null)
                {
                    this.fileListingSelector.onValueChanged.AddListener(
                        delegate
                        {
                            this.OnFileSelected();
                        }
                    );
                }
            }

            if (this.statusMessageObject != null)
            {
                this.statusMsgText = this.statusMessageObject.GetComponent<Text>();
            }

            if (this.clearCacheButtonObject != null)
            {
                this.clearCacheButton = this.clearCacheButtonObject.GetComponent<Button>();

                if (this.clearCacheButton != null)
                {
                    this.clearCacheButton.onClick.AddListener(() => this.OnClearCacheClicked());
                }
            }

            if (this.loadCacheButtonObject != null)
            {
                this.loadCacheButton = this.loadCacheButtonObject.GetComponent<Button>();

                if (this.loadCacheButton != null)
                {
                    this.loadCacheButton.onClick.AddListener(() => this.OnLoadCacheButtonClicked());

                    this.SetLoadButtonInteractable(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitPlaybackControlComponents()
        {
            if (this.reverseDataFlowObject != null)
            {
                this.reverseDataFlowToggle = this.reverseDataFlowObject.GetComponent<Toggle>();

                if (this.reverseDataFlowToggle != null)
                {
                    this.reverseDataFlowToggle.onValueChanged.AddListener(
                        delegate
                        {
                            this.OnReverseDataFlowToggleClicked();
                        }
                    );
                }
            }

            if (this.loopAtEndObject != null)
            {
                this.loopAtEndToggle = this.loopAtEndObject.GetComponent<Toggle>();

                if (this.loopAtEndToggle != null)
                {
                    this.loopAtEndToggle.onValueChanged.AddListener(
                        delegate
                        {
                            this.OnLoopAtEndToggleClicked();
                        }
                    );
                }
            }

            if (this.cachedEntriesCountObject != null)
            {
                this.cachedEntriesCountText = this.cachedEntriesCountObject.GetComponent<Text>();
            }

            if (this.currentCachedItemIndexObject != null)
            {
                this.currentCachedItemIndexText = this.currentCachedItemIndexObject.GetComponent<Text>();
            }

            if (this.resetPlaybackFactorButtonObject != null)
            {
                this.resetPlaybackFactorButton = this.resetPlaybackFactorButtonObject.GetComponent<Button>();

                if (this.resetPlaybackFactorButton != null)
                {
                    this.resetPlaybackFactorButton.onClick.AddListener(() => this.OnResetPlaybackFactorButtonClicked());
                }
            }

            if (this.playbackTimingFactorSelectionObject != null)
            {
                this.playbackTimingFactorSlider = this.playbackTimingFactorSelectionObject.GetComponent<Slider>();
                this.playbackTimingFactorSlider.onValueChanged.AddListener(this.OnPlaybackTimingFactorChanged);
            }

            if (this.playbackTimingFactorTextObject != null)
            {
                this.playbackTimingFactorText = this.playbackTimingFactorTextObject.GetComponent<Text>();
            }

            if (this.cachedEntriesIndexSelectionObject != null)
            {
                this.cachedEntriesIndexSlider = this.cachedEntriesIndexSelectionObject.GetComponent<Slider>();
                this.cachedEntriesIndexSlider.onValueChanged.AddListener(this.OnCacheItemIndexChanged);
            }

            if (this.selectedCachedItemIndexObject != null)
            {
                this.selectedCachedItemIndexText = this.selectedCachedItemIndexObject.GetComponent<Text>();
            }

            if (this.cachedEntriesMidIndexObject != null)
            {
                this.cachedEntriesMidIndexText = this.cachedEntriesMidIndexObject.GetComponent<Text>();
            }

            if (this.cachedEntriesMaxIndexObject != null)
            {
                this.cachedEntriesMaxIndexText = this.cachedEntriesMaxIndexObject.GetComponent<Text>();
            }

            if (this.startPlaybackButtonObject != null)
            {
                this.startPlaybackButton = this.startPlaybackButtonObject.GetComponent<Button>();

                if (this.startPlaybackButton != null)
                {
                    this.startPlaybackButton.onClick.AddListener(() => this.OnStartPlaybackButtonClicked());
                }
            }

            if (this.stopPlaybackButtonObject != null)
            {
                this.stopPlaybackButton = this.stopPlaybackButtonObject.GetComponent<Button>();

                if (this.stopPlaybackButton != null)
                {
                    this.stopPlaybackButton.onClick.AddListener(() => this.OnStopPlaybackButtonClicked());
                }
            }

            if (this.pausePlaybackButtonObject != null)
            {
                this.pausePlaybackButton = this.pausePlaybackButtonObject.GetComponent<Button>();

                if (this.pausePlaybackButton != null)
                {
                    this.pausePlaybackButton.onClick.AddListener(() => this.OnPausePlaybackButtonClicked());
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void InitPlaybackComponents()
        {
            try
            {
                IDataHistorian dataHistorianManager = EventProcessor.GetInstance().GetDataHistorianManager();

                this.selectedPathName = dataHistorianManager.GetCacheFilePath();

                this.InitPlaybackFileManagementComponents();
                this.InitPlaybackControlComponents();

                this.UpdateFileAndPathName();

            } catch (Exception e)
            {
                Debug.LogError($"Failed to create data historian player. Message: {e.Message}. Stack: {e.StackTrace}");
            }
        }

        private void UpdateCachedItemsStats()
        {
            int cachedEntriesCount = this.dataHistorianPlayer.GetCacheSize();
            int halfEntriesCount = cachedEntriesCount > 0 ? cachedEntriesCount / 2 : cachedEntriesCount;
            string cachedEntriesStr = cachedEntriesCount.ToString();

            this.loadedEntriesCountText.text = cachedEntriesStr;

            if (this.cachedEntriesCountText != null)
            {
                this.cachedEntriesCountText.text = cachedEntriesStr;
            }

            if (this.cachedEntriesMidIndexText != null)
            {
                this.cachedEntriesMidIndexText.text = halfEntriesCount.ToString();
            }

            if (this.cachedEntriesMaxIndexText != null)
            {
                this.cachedEntriesMaxIndexText.text = cachedEntriesStr;
            }

            if (this.cachedEntriesIndexSlider != null)
            {
                this.cachedEntriesIndexSlider.maxValue = (float) cachedEntriesCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitPlayer()
        {
            try
            {
                if (this.dataHistorianPlayer != null && !this.selectedCacheName.Equals(this.dataHistorianPlayer.GetCacheName()))
                {
                    Debug.Log($"Stopping current historian (if running).");

                    this.OnStopPlaybackButtonClicked();
                }

                // cache will be auto-loaded if not already stored by the historian manager
                Debug.Log($"Loading data historian player. Selected cache name: {this.selectedCacheName}");
                this.dataHistorianPlayer = EventProcessor.GetInstance().GetDataHistorianPlayer(this.selectedCacheName);

                if (this.dataHistorianPlayer != null)
                {
                    // derived cache name and actual cache name should be identical - verify
                    Debug.Log($"Retrieved data historian player. Cache name: {this.dataHistorianPlayer.GetCacheName()}. File: {this.selectedFileName}");

                    this.UpdateCachedItemsStats();
                    this.UpdatePlaybackTimingFactor();

                    // this call will set cache filling to false, enable playback, and disable threading
                    this.playbackCoroutineProcessor.InitHandler(this.dataHistorianPlayer, this.currentCachedItemIndexText, true);

                    //this.dataHistorianPlayer.SetCacheFillingEnabledFlag(false);
                    //this.dataHistorianPlayer.SetPlaybackEnabledFlag(true);

                    this.SetLoadButtonInteractable(false);
                } else
                {
                    Debug.Log($"No data historian player loaded / created yet. Be sure to select an existing cache file. Invalid: {this.selectedCacheName}");
                }
            } catch (Exception e)
            {
                Debug.LogError($"Exception thrown setting path and file. Error: {e.Message}. Stack: {e.StackTrace}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsDataHistorianPlayerInitialized()
        {
            return (this.GetDataHistorianPlayer() != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsIncomingTelemetryProcessingEnabled(IotDataContext data)
        {
            // disallow processing of live telemetry within this component
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private double ProcessNextHistorianItem()
        {
            return this.ProcessNextHistorianItem(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private double ProcessNextHistorianItem(bool resetCounter)
        {
            if (resetCounter)
            {
                this.curPlayingCachedItemIndex = 0;
            }

            this.selectedCachedItemIndexText.text = this.curPlayingCachedItemIndex.ToString();

            if (this.dataHistorianPlayer != null)
            {
                DataCacheEntryContainer entryContainer = this.dataHistorianPlayer.GetCacheEntryAtIndex(this.curPlayingCachedItemIndex);

                if (entryContainer != null)
                {
                    // it's technically possible for the container to have more than one IotDataContext instance
                    // the playback delay will for now, however, be limited to the container's timestamp, not
                    // the actual data entry's timestamp
                    //
                    // while this limits timing granularity in playback for each possible individual data entry,
                    // it's much easier to manage and - with some minor margin for error - unlikely that
                    // the time stamps of multiple stored data entries in a single container will differ by
                    // a measurable amount for playback purposes
                    //
                    // a future version of this can be implemented with multiple timers for each data entry type
                    // and factor in individual timestamps as part of the playback delay accordingly
                    if (entryContainer.HasActuatorData())
                    {
                        EventProcessor.GetInstance().OnMessagingSystemDataReceived(entryContainer.GetActuatorData());
                    }

                    if (entryContainer.HasConnectionStateData())
                    {
                        EventProcessor.GetInstance().OnMessagingSystemDataReceived(entryContainer.GetConnectionStateData());
                    }

                    if (entryContainer.HasSensorData())
                    {
                        EventProcessor.GetInstance().OnMessagingSystemDataReceived(entryContainer.GetSensorData());
                    }

                    if (entryContainer.HasSystemPerformanceData())
                    {
                        EventProcessor.GetInstance().OnMessagingSystemDataReceived(entryContainer.GetSystemPerformanceData());
                    }

                    if (entryContainer.HasMessageData())
                    {
                        // todo: determine what to do with message data - will prob never happen, but...
                    }
                }

                int indexAdder = 1;
                if (this.isReverseDataFlowEnabled)
                {
                    indexAdder = -1;
                }

                this.curPlayingCachedItemIndex += indexAdder;

                if (this.curPlayingCachedItemIndex < 0 || this.curPlayingCachedItemIndex >= this.dataHistorianPlayer.GetCacheSize())
                {
                    this.curPlayingCachedItemIndex = 0;
                } else
                {
                }
            }

            return 0d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        private void SetLoadButtonInteractable(bool enabled)
        {
            if (this.loadCacheButton != null)
            {
                this.loadCacheButton.interactable = enabled;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCacheFileList()
        {
            if (this.fileListingSelector != null)
            {
                if (string.IsNullOrWhiteSpace(this.selectedPathName))
                {
                    this.selectedPathName = EventProcessor.GetInstance().GetDataHistorianManager().GetCacheFilePath();

                    this.UpdateFileAndPathName();
                }

                try
                {
                    //this.fileListingSelector.enabled = false;
                    Debug.Log($"Getting file listing from path: {this.selectedPathName}");

                    List<string> fileNameList = FileUtil.GetFileListing(this.selectedPathName, ConfigConst.JSON_EXT, false);

                    if (fileNameList != null && fileNameList.Count > 0)
                    {
                        Debug.Log($"Read {fileNameList.Count} files from path {this.selectedPathName}. {FileUtil.GetStringifiedFileListing(fileNameList)}");

                        this.fileListingSelector.ClearOptions();
                        this.fileListingSelector.AddOptions(fileNameList);

                        this.UpdateFileStatusMessage($"Updated cache files for selected path: {this.selectedPathName}");
                    } else
                    {
                        this.UpdateFileStatusMessage($"No cache files for selected path: {this.selectedPathName}");
                    }
                } catch (Exception e)
                {
                    this.UpdateFileStatusMessage($"Error reading selected path: {this.selectedPathName}. Error: {e.Message}");
                } finally
                {
                    //this.fileListingSelector.enabled = true;
                }
            } else
            {
                Debug.Log("No file listing selector initialized. Null.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateFileAndPathName()
        {
            if (this.cachePathText != null)
            {
                this.cachePathText.text = this.selectedPathName;
                Debug.Log($"Setting path name: {this.selectedPathName}");

                this.UpdateCacheFileList();
            }

            if (this.cacheFileText != null)
            {
                this.cacheFileText.text = this.selectedFileName;
                Debug.Log($"Setting file name: {this.selectedPathName}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateFileStatusMessage(string msg)
        {
            this.statusMsgText.text = msg;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePlaybackTimingFactor()
        {
            // clear the text first to avoid remnants from previous reading
            this.playbackTimingFactorText.text = "     ";
            this.playbackTimingFactorText.text = this.playbackTimingFactor.ToString();

            if (this.dataHistorianPlayer != null)
            {
                this.dataHistorianPlayer.SetPlaybackDelayFactor(this.playbackTimingFactor);
            }
        }

    }

}
