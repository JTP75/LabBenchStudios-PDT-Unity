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

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Unity.Common;
using LabBenchStudios.Pdt.Plexus;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class StateManagementHudHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener, IUserEventStateListener
    {
        [SerializeField]
        private bool loadModelsAutomatically = true;

        [SerializeField]
        private GameObject liveDataStatusDisplay = null;

        [SerializeField]
        private GameObject messagingHostTextObject = null;

        [SerializeField]
        private GameObject messagingPortTextObject = null;

        [SerializeField]
        private GameObject clientIDTextObject = null;

        [SerializeField]
        private GameObject resourcePrefixTextObject = null;

        [SerializeField]
        private GameObject notificationTextObject = null;

        [SerializeField]
        private GameObject startDataFeedButtonObject = null;

        [SerializeField]
        private GameObject stopDataFeedButtonObject = null;

        [SerializeField]
        private GameObject simDataStatusDisplay = null;

        [SerializeField]
        private GameObject liveDataEnableButtonObject = null;

        [SerializeField]
        private GameObject launchHistorianCachePlaybackButtonObject = null;

        [SerializeField]
        private GameObject modelDataLoadStatusDisplay = null;

        [SerializeField]
        private GameObject filePathNameDisplay = null;

        [SerializeField]
        private GameObject configureModelsButtonObject = null;

        [SerializeField]
        private GameObject configureSimButtonObject = null;

        [SerializeField]
        private GameObject exitAppButtonObject = null;

        [SerializeField]
        private GameObject historianCachePlaybackPanel = null;

        [SerializeField]
        private GameObject historianCacheCapturePanel = null;

        private Text messagingHostText = null;
        private Text messagingPortText = null;
        private Text clientIDText = null;
        private Text resourcePrefixText = null;

        private TMP_Text notificationText = null;

        private TMP_Text liveDataStatusText = null;
        private TMP_Text simDataStatusText = null;
        private TMP_Text modelDataLoadStatusText = null;
        private TMP_Text filePathDisplayText = null;

        private Button startDataFeedButton = null;
        private Button stopDataFeedButton = null;
        private Button configureModelsButton = null;
        private Button launchHistorianCachePlaybackButton = null;
        private Button exitAppButton = null;

        private string msgHostName = ConfigConst.DEFAULT_HOST;
        private string clientID = "PDT_DTA_Client";
        private string resourcePrefix = ConfigConst.PRODUCT_NAME;
        private int msgPort = ConfigConst.DEFAULT_MQTT_PORT;

        private string dtdlModelPath = DigitalTwinUtil.GetDtdlModelsPath();

        private bool isDataFeedActive = false;
        private bool restartDataFeed = false;
        private bool isLiveDataEnabled = false;
        private bool isSimDataEngaged = false;
        private bool isLoadModelDataEngaged = false;


        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void ExitApplication()
        {
            try {
                this.StopDataFeed();

                string message = "Stopping connection resources...";

                EventProcessor.GetInstance().OnUserEventStateReceived(UserEventState.EventType.CloseOpenDialogs);
                EventProcessor.GetInstance().LogDebugMessage(message);
                EventProcessor.GetInstance().StopConnectionResources();

                message = "Exiting application now...";
                EventProcessor.GetInstance().LogDebugMessage(message);
            } catch (Exception e) {
                Debug.LogException(e);
            }

            Application.Quit();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartDataFeed()
        {
            if (this.startDataFeedButton != null && ! this.isDataFeedActive)
            {
                this.startDataFeedButton.interactable = false;
                this.stopDataFeedButton.interactable = true;
                this.liveDataStatusText.text = "Data Feed Started";

                ConnectionStateData connStateData = this.CreateLiveDataFeedConnectionStateData();

                EventProcessor.GetInstance().ProcessLiveDataFeedEngageRequest(connStateData, true);

                // call this separately - this avoids a potential issue where event processor
                // notifies other listeners of this internal update (although that may be useful)
                EventProcessor.GetInstance().OnMessagingSystemStatusUpdate(connStateData);

                this.isDataFeedActive = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopDataFeed()
        {
            if (this.stopDataFeedButton != null && this.isDataFeedActive)
            {
                this.startDataFeedButton.interactable = true;
                this.stopDataFeedButton.interactable = false;
                this.liveDataStatusText.text = "Data Feed Stopped";

                ConnectionStateData connStateData = this.CreateLiveDataFeedConnectionStateData();

                EventProcessor.GetInstance().ProcessLiveDataFeedEngageRequest(connStateData, false);

                // call this separately - this avoids a potential issue where event processor
                // notifies other listeners of this internal update (although that may be useful)
                EventProcessor.GetInstance().OnMessagingSystemStatusUpdate(connStateData);

                this.isDataFeedActive = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadModelData()
        {
            string message = $"NOTE: All DTDL model and type config mapping model data will be auto-loaded via DT System Manager at start.";

            EventProcessor.GetInstance().LogDebugMessage(message);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadModelDataPrev()
        {
            try {
                string message = $"NORMAL: Attempting to init and (re)load DTDL model data: {this.dtdlModelPath}";

                EventProcessor.GetInstance().LogDebugMessage(message);

                this.modelDataLoadStatusText.text = "(Re)loading model data...";

                if (EventProcessor.GetInstance().LoadDigitalTwinModels(this.dtdlModelPath)) {
                    Debug.Log($"NORMAL: Successfully (re)loaded DTDL model data: {this.dtdlModelPath}");
                    this.modelDataLoadStatusText.text = "Loaded model data.";
                } else {
                    Debug.LogError($"Failed to (re)load DTDL model data: {this.dtdlModelPath}");
                    this.modelDataLoadStatusText.text = "Failed to load model data!";
                }
            } catch (Exception e) {
                this.modelDataLoadStatusText.text = "Failed to load model data.";
                Debug.LogError($"Failed to load DTDL files from {this.dtdlModelPath}");
            }
        }

        // public callback methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        public void HandleUserEventState(UserEventState.EventType eventType)
        {
            switch (eventType)
            {
                case UserEventState.EventType.CloseOpenDialogs:
                    if (this.historianCacheCapturePanel != null)
                    {
                        this.historianCacheCapturePanel.SetActive(false);
                    }

                    if (this.historianCachePlaybackPanel != null)
                    {
                        this.historianCachePlaybackPanel.SetActive(false);
                    }

                    break;

                case UserEventState.EventType.RestoreOpenDialogs:
                    if (this.historianCacheCapturePanel != null)
                    {
                        this.historianCacheCapturePanel.SetActive(true);
                    }

                    if (this.historianCachePlaybackPanel != null)
                    {
                        this.historianCachePlaybackPanel.SetActive(false);
                    }

                    break;

                case UserEventState.EventType.DisableLiveDataProcessing:
                    this.StopDataFeed();

                    break;
            }
        }

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
            // nothing to do
        }


        // protected

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try {
                if (this.messagingHostTextObject != null) {
                    this.messagingHostText = this.messagingHostTextObject.GetComponent<Text>();
                }

                if (this.messagingPortTextObject != null) {
                    this.messagingPortText = this.messagingPortTextObject.GetComponent<Text>();
                }

                if (this.clientIDTextObject != null) {
                    this.clientIDText = this.clientIDTextObject.GetComponent<Text>();
                }

                if (this.resourcePrefixTextObject != null) {
                    this.resourcePrefixText = this.resourcePrefixTextObject.GetComponent<Text>();
                }

                if (this.notificationTextObject != null) {
                    this.notificationText = this.notificationTextObject.GetComponent<TextMeshProUGUI>();
                }

                if (this.liveDataStatusDisplay != null) {
                    this.liveDataStatusText = this.liveDataStatusDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.startDataFeedButtonObject != null) {
                    this.startDataFeedButton = this.startDataFeedButtonObject.GetComponent<Button>();

                    if (this.startDataFeedButton != null) {
                        this.startDataFeedButton.onClick.AddListener(() => this.StartDataFeed());
                    }
                }

                if (this.stopDataFeedButtonObject != null) {
                    this.stopDataFeedButton = this.stopDataFeedButtonObject.GetComponent<Button>();

                    if (this.stopDataFeedButton != null) {
                        this.stopDataFeedButton.onClick.AddListener(() => this.StopDataFeed());
                    }
                }

                if (this.configureModelsButtonObject != null) {
                    this.configureModelsButton = this.configureModelsButtonObject.GetComponent<Button>();

                    if (this.configureModelsButton != null) {
                        this.configureModelsButton.onClick.AddListener(() => this.LaunchConfigureModelsInterface());
                    }
                }

                if (this.launchHistorianCachePlaybackButtonObject != null) {
                    this.launchHistorianCachePlaybackButton = this.launchHistorianCachePlaybackButtonObject.GetComponent<Button>();

                    if (this.launchHistorianCachePlaybackButton != null) {
                        this.launchHistorianCachePlaybackButton.onClick.AddListener(() => this.LaunchHistorianPlaybackInterface());
                    }
                }

                if (this.simDataStatusDisplay != null) {
                    this.simDataStatusText = this.simDataStatusDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.modelDataLoadStatusDisplay != null) {
                    this.modelDataLoadStatusText = this.modelDataLoadStatusDisplay.GetComponent<TextMeshProUGUI>();
                }

                if (this.filePathNameDisplay != null) {
                    this.filePathDisplayText = this.filePathNameDisplay.GetComponent<TextMeshProUGUI>();

                    if (this.filePathDisplayText != null) {
                        this.filePathDisplayText.text = this.dtdlModelPath;
                    }
                }

                if (this.configureModelsButtonObject != null) {
                    this.configureModelsButton = this.configureModelsButtonObject.GetComponent<Button>();

                    if (this.configureModelsButton != null) {
                        this.configureModelsButton.onClick.AddListener(() => this.LoadModelData());
                    }
                }

                if (this.exitAppButtonObject != null) {
                    this.exitAppButton = this.exitAppButtonObject.GetComponent<Button>();

                    if (this.exitAppButton != null) {
                        this.exitAppButton.onClick.AddListener(() => this.ExitApplication());
                    }
                }

                base.RegisterForSystemStatusEvents((ISystemStatusEventListener) this);
                base.RegisterForUserStatusEvents((IUserEventStateListener) this);

                if (this.loadModelsAutomatically) {
                    this.LoadModelData();
                }
            } catch (Exception ex) {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void LaunchConfigureModelsInterface()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected void LaunchHistorianPlaybackInterface()
        {
            if (this.historianCachePlaybackPanel != null)
            {
                if (this.historianCachePlaybackPanel.activeInHierarchy)
                {
                    this.historianCachePlaybackPanel.SetActive(false);

                    EventProcessor.GetInstance().OnUserEventStateReceived(UserEventState.EventType.RestoreOpenDialogs);
                    EventProcessor.GetInstance().OnUserEventStateReceived(UserEventState.EventType.EnableLiveDataProcessing);

                    this.startDataFeedButton.interactable = true;
                    this.stopDataFeedButton.interactable = true;
                } else
                {
                    EventProcessor.GetInstance().OnUserEventStateReceived(UserEventState.EventType.CloseOpenDialogs);
                    EventProcessor.GetInstance().OnUserEventStateReceived(UserEventState.EventType.DisableLiveDataProcessing);

                    this.startDataFeedButton.interactable = false;
                    this.stopDataFeedButton.interactable = false;

                    this.historianCachePlaybackPanel.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessDebugLogMessage(string message)
        {
            if (message != null) {
                if (this.notificationText != null) {
                    this.notificationText.text = "DEBUG: " + message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessWarningLogMessage(string message)
        {
            if (message != null) {
                if (this.notificationText != null) {
                    this.notificationText.text = "WARNING: " + message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected new void ProcessErrorLogMessage(string message)
        {
            if (message != null) {
                if (this.notificationText != null) {
                    this.notificationText.text = "ERROR: " + message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessActuatorData(ActuatorData data)
        {
            if (data != null) {
                String jsonData = DataUtil.ActuatorDataToJson(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            if (data != null) {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.liveDataStatusText != null) this.liveDataStatusText.text = connStateMsg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessMessageData(MessageData data)
        {
            if (data != null) {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSensorData(SensorData data)
        {
            if (data != null) {
                // nothing to do
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            if (data != null) {
                // nothing to do
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
        private ConnectionStateData CreateLiveDataFeedConnectionStateData()
        {
            if (this.messagingHostText != null) {
                string hostNameText = this.messagingHostText.text;

                if (!string.IsNullOrEmpty(hostNameText)) {
                    this.msgHostName = hostNameText;
                }
            }

            if (this.messagingPortText != null) {
                string portNumText = this.messagingPortText.text;
                int portNum = ConfigConst.DEFAULT_MQTT_PORT;

                if (int.TryParse(portNumText, out portNum)) {
                    this.msgPort = portNum;
                }
            }

            if (this.clientIDText != null) {
                string cidText = this.clientIDText.text;

                if (!string.IsNullOrEmpty(cidText)) {
                    this.clientID = cidText;
                }
            }

            if (this.resourcePrefixText != null) {
                string rpText = this.resourcePrefixText.text;

                if (!string.IsNullOrEmpty(rpText)) {
                    this.resourcePrefix = rpText;
                }
            }

            ConnectionStateData connStateData = new ConnectionStateData(this.msgHostName, this.msgPort);

            connStateData.SetClientID(this.clientID);
            connStateData.SetResourcePrefix(this.resourcePrefix);

            this.ProcessDebugLogMessage("Updated connection state:\n" + connStateData.ToString());

            return connStateData;
        }

    }

}
