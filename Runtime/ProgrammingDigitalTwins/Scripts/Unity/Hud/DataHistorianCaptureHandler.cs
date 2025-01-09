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
using System.Text;

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
    public class DataHistorianCaptureHandler : BaseAsyncDataMessageProcessor,
        ISystemStatusEventListener, IDataHistorianFileEventListener
    {
        [SerializeField]
        private GameObject captureEventsSelectionObject = null;

        [SerializeField]
        private GameObject saveCaptureOnClearSelectionObject = null;

        [SerializeField]
        private GameObject capturedEventsCounterObject = null;

        [SerializeField]
        private GameObject capturedEventsMemoryObject = null;

        [SerializeField]
        private GameObject selectedPathObject = null;

        [SerializeField]
        private GameObject selectedFileObject = null;

        [SerializeField]
        private GameObject saveCacheButtonObject = null;

        [SerializeField]
        private GameObject clearCacheButtonObject = null;

        private Toggle enableEventCaptureToggle = null;
        private Toggle saveCaptureOnClearToggle = null;
        private Text capturedEventsCounterText = null;
        private Text capturedEventsMemoryText = null;

        private Text selectedPathText = null;
        private Text selectedFileText = null;

        private string selectedPathName = null;
        private string selectedCacheName = null;

        private Button saveCacheButton = null;
        private Button clearCacheButton = null;

        private bool isEventCaptureEnabled = false;
        private bool enableIncomingTelemetry = false;

        private IDataHistorianPlayer dataHistorianPlayer = null;

        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void HandleUserEventState(UserEventState.EventType eventType)
        {
            switch (eventType)
            {
                case UserEventState.EventType.CloseOpenDialogs:
                    this.CloseHistorianPanel();
                    break;

                default:
                    // ignore
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseHistorianPanel()
        {
            Debug.Log("Close historian panel clicked.");
            this.gameObject.SetActive(false);
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

        public void OnEventCaptureToggle()
        {
            this.isEventCaptureEnabled = this.enableEventCaptureToggle.isOn;
            Debug.Log($"Event capture toggle clicked: {this.isEventCaptureEnabled}");

            // todo:

            if (this.isEventCaptureEnabled)
            {
                // todo: disable cache load control while capturing events
                if (this.IsDataHistorianPlayerInitialized()) {
                    this.GetDataHistorianPlayer().SetCacheFillingEnabledFlag(true);
                }

                // todo: disable playback controls while capturing events

            } else
            {
                if (this.IsDataHistorianPlayerInitialized())
                {
                    this.GetDataHistorianPlayer().SetCacheFillingEnabledFlag(false);
                }
            }
        }

        public void OnClearCache()
        {
            Debug.Log("OnClearCacheClicked invoked.");
            
            if (this.saveCaptureOnClearToggle != null)
            {
                if (this.saveCaptureOnClearToggle.isOn)
                {
                    this.OnSaveCache();
                }
            }

            bool success = this.GetDataHistorianPlayer().Clear();

            Debug.Log($"Cleared {this.GetDataHistorianPlayer().GetCacheName()} with {this.GetDataHistorianPlayer().GetCacheSize()} items. Success? {success}");

            this.UpdateCapturedEventsMetrics();
        }

        public void OnSaveCache()
        {
            Debug.Log("Save cache button clicked.");

            bool success = this.GetDataHistorianPlayer().StoreHistorianCache();
            string cacheFileName = this.GetDataHistorianPlayer().GetCacheFileName();
            string cacheFilePath = this.GetDataHistorianPlayer().GetCacheStorageUri();

            Debug.Log($"Stored {this.GetDataHistorianPlayer().GetCacheName()} with {this.GetDataHistorianPlayer().GetCacheSize()} items to path {cacheFilePath}; file {cacheFileName}. Success? {success}");
        }

        public void OnResetHistorian()
        {
            Debug.Log("Reset historian button clicked.");
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try
            {
                this.InitCaptureComponents();

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

                this.UpdateCapturedEventsMetrics();
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

                this.UpdateCapturedEventsMetrics();
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

                this.UpdateCapturedEventsMetrics();
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

                this.UpdateCapturedEventsMetrics();
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

                this.UpdateCapturedEventsMetrics();
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
        private void InitCaptureComponents()
        {
            try
            {
                string actionMsg = "Created";

                this.dataHistorianPlayer = EventProcessor.GetInstance().GetDataHistorianPlayer();
                this.dataHistorianPlayer.SetCacheOnlyDataEventsFlag(true);

                if (this.dataHistorianPlayer != null)
                {
                    Debug.Log($"Clearing historian player state: {this.dataHistorianPlayer.GetCacheName()}");

                    this.dataHistorianPlayer.Stop();
                    this.dataHistorianPlayer.StoreHistorianCache();
                    this.dataHistorianPlayer.Clear();

                    actionMsg = "Re-initialized";
                }

                Debug.Log($"{actionMsg} historian player. Cache name: {this.dataHistorianPlayer.GetCacheName()}");
                Debug.Log($"{actionMsg} historian player. Storage Path: {this.dataHistorianPlayer.GetCacheStorageUri()}");
                Debug.Log($"{actionMsg} historian storage. Cache File: {this.dataHistorianPlayer.GetCacheFileName()}");

                this.InitHistorianEventCapturePanel();
                this.InitHistorianStoragePanel();

                this.InitPathInfo();

            } catch (Exception e)
            {
                Debug.LogError($"Failed to create data historian player. Message: {e.Message}. Stack: {e.StackTrace}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitHistorianEventCapturePanel()
        {
            if (this.captureEventsSelectionObject != null)
            {
                this.enableEventCaptureToggle = this.captureEventsSelectionObject.GetComponent<Toggle>();

                if (this.enableEventCaptureToggle != null)
                {
                    this.enableEventCaptureToggle.onValueChanged.AddListener(
                        delegate
                        {
                            this.OnEventCaptureToggle();
                        }
                    );
                }
            }

            if (this.saveCaptureOnClearSelectionObject != null)
            {
                this.saveCaptureOnClearToggle = this.saveCaptureOnClearSelectionObject.GetComponent<Toggle>();
            }

            if (this.capturedEventsCounterObject != null)
            {
                this.capturedEventsCounterText = this.capturedEventsCounterObject.GetComponent<Text>();
            }

            if (this.capturedEventsMemoryObject != null)
            {
                this.capturedEventsMemoryText = this.capturedEventsMemoryObject.GetComponent<Text>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitHistorianStoragePanel()
        {
            if (this.selectedPathObject != null)
            {
                this.selectedPathText = this.selectedPathObject.GetComponent<Text>();
            }

            if (this.selectedFileObject != null)
            {
                this.selectedFileText = this.selectedFileObject.GetComponent<Text>();
            }

            if (this.saveCacheButtonObject != null)
            {
                this.saveCacheButton = this.saveCacheButtonObject.GetComponent<Button>();

                if (this.saveCacheButton != null)
                {
                    this.saveCacheButton.onClick.AddListener(() => this.OnSaveCache());
                }
            }

            if (this.clearCacheButtonObject != null)
            {
                this.clearCacheButton = this.clearCacheButtonObject.GetComponent<Button>();

                if (this.clearCacheButton != null)
                {
                    this.clearCacheButton.onClick.AddListener(() => this.OnClearCache());
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitPathInfo()
        {
            try
            {
                if (this.IsDataHistorianPlayerInitialized())
                {
                    this.selectedPathName = this.GetDataHistorianPlayer().GetCacheStorageUri();
                    this.selectedCacheName = this.GetDataHistorianPlayer().GetCacheName();

                    this.selectedPathText.text = this.selectedPathName;
                    this.selectedFileText.text = this.selectedCacheName;

                    Debug.Log($"Setting cache URI and filename: {this.GetDataHistorianPlayer().GetCacheFileName()}");
                }
            } catch (Exception e)
            {
                Debug.LogError($"Exception thrown setting path and file. Message: {e.Message}. Stack: {e.StackTrace}");
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
            return (this.enableEventCaptureToggle.isOn);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCapturedEventsMetrics()
        {
            this.UpdateCapturedEventsMetrics(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCapturedEventsMetrics(bool isCacheCleared)
        {
            if (isCacheCleared)
            {
                // nothing to do for now
            }

            this.capturedEventsCounterText.text = NumberUtil.GetFormattedCounter(this.GetDataHistorianPlayer().GetCacheSize());
            this.capturedEventsMemoryText.text = NumberUtil.GetFormattedBytes(this.GetDataHistorianPlayer().GetCacheMemoryUsage());
        }

    }

}
