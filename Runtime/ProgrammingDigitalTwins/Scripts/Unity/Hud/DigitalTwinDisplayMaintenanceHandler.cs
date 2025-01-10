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
using LabBenchStudios.Pdt.Model;
using LabBenchStudios.Pdt.Plexus;
using LabBenchStudios.Pdt.Prediction;

using LabBenchStudios.Pdt.Unity.Common;

using System.Collections;
using System.Threading;
using System.Dynamic;

namespace LabBenchStudios.Pdt.Unity.Hud
{
    public class DigitalTwinDisplayMaintenanceHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private GameObject connStateObject = null;

        [SerializeField]
        private GameObject sessionIDInputObject = null;

        [SerializeField]
        private GameObject clearCacheButtonObject = null;

        [SerializeField]
        private GameObject deviceIDObject = null;

        [SerializeField]
        private GameObject deviceModelNameObject = null;

        [SerializeField]
        private GameObject deviceModelIDObject = null;

        [SerializeField]
        private GameObject aiUriTextInputObject = null;

        [SerializeField]
        private GameObject aiModelSelectorObject = null;

        [SerializeField]
        private GameObject selectedAiModelObject = null;

        [SerializeField]
        private GameObject queryContentObject = null;

        [SerializeField]
        private GameObject rememberQueryHistoryToggleObject;

        [SerializeField]
        private GameObject includeSystemSpecsToggleObject;

        [SerializeField]
        private GameObject includeSystemDataToggleObject;

        [SerializeField]
        private GameObject systemDataCacheHoursContentObject = null;

        [SerializeField]
        private GameObject submittedQueryContentObject = null;

        [SerializeField]
        private GameObject recommendationsContentObject = null;

        [SerializeField]
        private GameObject resetQueryButtonObject = null;

        [SerializeField]
        private GameObject reloadModelsButtonObject = null;

        [SerializeField]
        private GameObject sendGeneralQueryButtonObject = null;

        [SerializeField]
        private GameObject sendPdmQueryButtonObject = null;

        [SerializeField]
        private GameObject uploadDocsButtonObject = null;

        [SerializeField]
        private GameObject eventListenerContainer = null;


        // local vars

        private GameObject maintenancePanel = null;

        private TMP_Dropdown aiModelSelector = null;

        private Toggle rememberQueryHistoryToggle = null;
        private Toggle includeSystemSpecsToggle = null;
        private Toggle includeSystemDataToggle = null;

        private TMP_Text connStateLabelText = null;
        private TMP_Text deviceIDText = null;
        private TMP_Text deviceModelNameText = null;
        private TMP_Text deviceModelIDText = null;
        private TMP_Text selectedAiModelText = null;
        private TMP_Text submittedQueryContentText = null;
        private TMP_Text recommendationsContentText = null;

        private Text sessionIDTextInput = null;
        private Text aiUriTextInput = null;
        private Text queryContentText = null;
        private Text systemDataCacheHoursText = null;

        private Button clearCacheButton = null;
        private Button resetQueryButton = null;
        private Button reloadModelsButton = null;
        private Button sendGeneralQueryButton = null;
        private Button sendPdmQueryButton = null;
        private Button uploadDocsButton = null;

        private bool hasRecommendationsPanel = false;

        private string sessionID = ConfigConst.NOT_SET;
        private string deviceID = ConfigConst.NOT_SET;
        private string dtmiUri  = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelNameUtil.IOT_MODEL_CONTEXT_NAME;
        private string serverUri = "";

        private string selectedAiModel = "";
        
        private string queryRequestMsg = "";

        private bool updateAiModelList = false;
        private bool updateAiResponseMsg = false;

        private DigitalTwinModelState digitalTwinModelState = null;

        private ResourceNameContainer cmdResource = null;

        private IDataContextExtendedListener eventListener = null;

        private PredictionSystemManager predictionManager = null;

        private Queue<string> aiQueryRequestQueue = null;
        private Queue<string> aiQueryResponseQueue = null;

        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void ClosePropertiesPanel()
        {
            if (this.hasRecommendationsPanel)
            {
                this.maintenancePanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveProperties()
        {
            // TODO: implement this
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

        public void SetDigitalTwinCommandResource(ResourceNameContainer resource)
        {
            if (resource != null)
            {
                this.cmdResource = resource;
            }
        }

        public void SetDigitalTwinModelState(DigitalTwinModelState dtModelState)
        {
            if (dtModelState != null)
            {
                this.digitalTwinModelState = dtModelState;

                this.UpdateModelDataAndProperties();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtmiName"></param>
        /// <param name="dtmiUri"></param>
        public void UpdateDigitalTwinLabels(string dtmiName, string dtmiUri)
        {
            if (!string.IsNullOrEmpty(dtmiName))
            {
                this.dtmiName = dtmiName;
            }

            if (!string.IsNullOrEmpty(dtmiUri))
            {
                this.dtmiUri = dtmiUri;
            }

            this.UpdateLabelsAndNames();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnModelUpdateEvent()
        {
            this.UpdateModelDataAndProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnRememberQueryHistoryToggleClicked()
        {
            if (! this.rememberQueryHistoryToggle.isOn)
            {
                this.ResetQueryRequestMsg();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetQueryRequestMsg()
        {
            this.queryRequestMsg = "";
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnPredictionModelSelected()
        {
            this.UpdateUserSettings();

            if (this.aiModelSelector != null)
            {
                this.selectedAiModel = this.aiModelSelector.captionText.text;

                if (this.selectedAiModelText != null) {
                    this.selectedAiModelText.text = this.selectedAiModel;
                }

                Debug.Log($"AI model selected: {this.sessionID} - {this.selectedAiModel}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearPredictionEngineCache()
        {
            this.UpdateUserSettings();
            this.ResetPredictionEngineQueries();

            Debug.Log($"Clearing cached queries for all sessions.");

            this.predictionManager.ClearAllCachedQueries();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetPredictionEngineQueries()
        {
            this.UpdateUserSettings();

            if (this.queryContentText != null)
            {
                this.queryContentText.text = "";
            }

            if (this.recommendationsContentText != null)
            {
                this.recommendationsContentText.text = "";
            }

            if (this.submittedQueryContentText != null)
            {
                this.submittedQueryContentText.text = "";
            }

            Debug.Log($"Clearing cached queries for session: {this.sessionID}");

            this.predictionManager.ClearCachedQueries(this.sessionID);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReloadPredictionEngineModels()
        {
            this.UpdateUserSettings();

            Debug.Log($"Attempting to reload prediction models: {this.sessionID} - {this.serverUri}");

            StartCoroutine(this.ReloadPredictionEngineModelsCoroutine());
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendPredictionEngineQuery()
        {
            this.SendPredictionEngineQuery(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendPredictionEngineQuery(bool isPdmQuery)
        {
            this.UpdateUserSettings();

            StringBuilder builder = new StringBuilder();

            if (isPdmQuery)
            {
                builder.Append(this.GenerateBasicPdmQuery());

                bool sysSpecsIncluded = false;
                bool sysDataIncluded = false;

                if (this.includeSystemSpecsToggle != null && this.includeSystemSpecsToggle.isOn)
                {
                    builder.Append(this.GenerateSystemSpecsQuery());
                    sysSpecsIncluded = true;
                }

                if (this.includeSystemDataToggle != null && this.includeSystemDataToggle.isOn)
                {
                    if (!sysSpecsIncluded)
                    {
                        builder.Append(this.GenerateSystemSpecsQuery());
                        builder.Append('\n');
                    }

                    builder.Append(this.GenerateSystemDataQuery());
                    sysDataIncluded = true;
                }

                this.queryContentText.text = builder.ToString();
            } else {
                if (builder.Length > 0)
                {
                    builder.Append('\n');
                }

                builder.Append(this.queryContentText.text);
            }

            this.queryRequestMsg += builder.ToString();
            this.submittedQueryContentText.text = this.queryRequestMsg;
            this.queryContentText.text = "";
            
            Debug.Log($"Submitting query to prediction engine: {this.sessionID} - {this.serverUri}. Model: {this.selectedAiModel}");
            Debug.Log($"Prediction engine query: {this.queryRequestMsg}");

            StartCoroutine(this.SendPredictionEngineQueryCoroutine());
        }

        /// <summary>
        /// 
        /// </summary>
        public void UploadDocsClicked()
        {
            // TODO: implement this

            Debug.Log("Upload docs button clicked. Feature not yet implemented.");
        }

        // protected

        // TODO
        //
        // These calls to 'Generate...Pdm...Query()' are well suited
        // to a Builder pattern - re-work these
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GenerateBasicPdmQuery()
        {
            StringBuilder builder = new StringBuilder();

            SystemModelManager smm = EventProcessor.GetInstance().GetSystemModelManager();
            ConfigTypeModelManager ctmm = smm.GetConfigTypeModelManager();
            ConfigTypeModelContext configType = ctmm.GetConfigEntryByModelName(this.dtmiUri);

            if (configType == null)
            {
                configType = ctmm.GetConfigCategoryByModelName(this.dtmiUri);
            }

            builder.Append("Generate a predictive maintenance plan for a ");

            if (configType != null)
            {
                builder.Append(configType.GetConfigTypeDisplayName());
            } else {
                builder.Append(this.dtmiName);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GenerateSystemSpecsQuery()
        {
            StringBuilder builder = new StringBuilder();

            SystemModelManager smm = EventProcessor.GetInstance().GetSystemModelManager();
            ConfigTypeModelManager ctmm = smm.GetConfigTypeModelManager();
            ConfigTypeModelContext configType = ctmm.GetConfigEntryByModelName(this.dtmiUri);

            if (configType == null)
            {
                configType = ctmm.GetConfigCategoryByModelName(this.dtmiUri);
            }

            builder.Append("\nThe system's specifications include the following properties:\n");

            if (configType != null)
            {
                ConfigTypeModelConstraints constraints = configType.GetModelConstraints();

                // initial query details - further updates required

                builder.Append(configType.GetConfigTypeDisplayName());

                builder.Append("\nIts minimum data value is ").Append(constraints.GetMinReading());
                builder.Append("\nIts maximum data value is ").Append(constraints.GetMaxReading());
                builder.Append("\nIts duty cycle in seconds is ").Append(constraints.GetDutyCycleSeconds());
            } else {
                builder.Append("\nNone are available at this time.");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GenerateSystemDataQuery()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("\nThe system's performance includes the following measurements:\n");

            float cacheHours = 1;

            if (this.systemDataCacheHoursText != null)
            {
                string cacheHoursStr = this.systemDataCacheHoursText.text;

                try {
                    cacheHours = float.Parse(cacheHoursStr);
                } catch (Exception e) {
                    Debug.Log($"Cache hours for system data cannot be derived from text: {cacheHoursStr}. Using default: {cacheHours}");
                }

                if (cacheHours <= 0)
                {
                    cacheHours = 1;
                    Debug.Log($"Cache hours for system <= 0. Using default: {cacheHours}");
                }
            }

            builder.Append($"It's been running for {cacheHours} hours.");

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try
            {
                // first: retrieve the prediction manager and init queues
                this.predictionManager =
                    EventProcessor.GetInstance().GetSystemModelManager().GetPredictionSystemManager();

                this.aiQueryRequestQueue = new Queue<string>();
                this.aiQueryResponseQueue = new Queue<string>();
                
                // second: init the maintenance panel controls
                this.InitMaintenancePanel();

                // third: update the state properties
                this.UpdateModelDataAndProperties();

                // fourth: handle any remaining updates and register for events
                this.predictionManager.SetPredictionModelListener(this);
                this.predictionManager.SetSystemStatusEventListener(this);
                this.predictionManager.SetQueryResponseListener(this);

                base.RegisterForSystemStatusEvents((ISystemStatusEventListener) this);

                // fifth: start the prediction engine poller
                if (base.IsPredictionProcessingEnabled())
                {
                    InvokeRepeating(nameof(CheckPredictionEngineForUpdates), 1.0f, 1.0f);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize digital twin props editor HUD. Continuing without display data: {ex.StackTrace}");
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
            // ignore
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            if (this.connStateLabelText != null)
            {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.connStateLabelText != null) this.connStateLabelText.text = connStateMsg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessMessageData(MessageData data)
        {
            // ignore
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSensorData(SensorData data)
        {
            // ignore
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessSystemPerformanceData(SystemPerformanceData data)
        {
            // ignore
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelListContainer"></param>
        protected override void ProcessModelListUpdate(ModelListContainer modelListContainer)
        {
            string uri = modelListContainer.GetUri();
            List<string> modelList = modelListContainer.GetModelList();

            this.ProcessModelListUpdate(modelList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResponseContainer"></param>
        protected override void ProcessQueryResponseUpdate(QueryResponseContainer queryResponseContainer)
        {
            string sessionID = queryResponseContainer.GetSessionID();
            string uri = queryResponseContainer.GetUri();
            string response = queryResponseContainer.GetResponse();

            this.ProcessQueryResponseUpdate(response);
        }

        // private methods

        /// <summary>
        /// This is necessary as async updates via the prediction modeling lib are
        /// blocked from updating any UI components (log messages are processed properly).
        /// 
        /// The request to the prediction engine is issued asynchronously, and updates
        /// are handled via callback to notify the DT logging infrastructure. This routine
        /// handles the UI updates via a polling function, as all query / response data
        /// along with loaded model names are available via the prediction engine's cache.
        /// </summary>
        private void CheckPredictionEngineForUpdates()
        {
            List<string> modelList = this.predictionManager.GetCachedModelList(this.serverUri);

            if (modelList != null && this.updateAiModelList)
            {
                Debug.Log($"Updating model list: {modelList.Count}");
                this.ProcessModelListUpdate(modelList);

                this.updateAiModelList = false;
            }

            PredictionSystemQueryCache queryCache = this.predictionManager.GetQueryCache(this.sessionID);

            if (queryCache != null && this.updateAiResponseMsg) {
                string queryMsg = queryCache.GetLatestQueryMessage();
                string responseMsg = queryCache.GetLatestResponseMessage();

                Debug.Log($"Updating response text: {responseMsg?.Length}");
                this.ProcessQueryResponseUpdate(responseMsg);

                this.updateAiResponseMsg = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitMaintenancePanel()
        {
            this.maintenancePanel = gameObject;

            if (this.sessionIDInputObject != null)
            {
                this.sessionIDTextInput = this.sessionIDInputObject.GetComponent<Text>();
            }

            if (this.deviceIDObject != null)
            {
                this.deviceIDText = this.deviceIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.connStateObject != null)
            {
                this.connStateLabelText = this.connStateObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.aiUriTextInputObject != null)
            {
                this.aiUriTextInput = this.aiUriTextInputObject.GetComponent<Text>();
            }

            if (this.deviceModelNameObject != null)
            {
                this.deviceModelNameText = this.deviceModelNameObject.GetComponent<TextMeshProUGUI>();
                this.deviceModelNameText.text = this.dtmiName;
            }

            if (this.deviceModelIDObject != null)
            {
                this.deviceModelIDText = this.deviceModelIDObject.GetComponent<TextMeshProUGUI>();
                this.deviceModelIDText.text = this.dtmiUri;
            }

            if (this.aiModelSelectorObject != null)
            {
                this.aiModelSelector = this.aiModelSelectorObject.GetComponent<TMP_Dropdown>();

                this.aiModelSelector.onValueChanged.AddListener(
                    delegate { this.OnPredictionModelSelected(); }
                );
            }

            if (this.selectedAiModelObject != null)
            {
                this.selectedAiModelText = this.selectedAiModelObject.GetComponent<TextMeshProUGUI>();
                this.selectedAiModelText.text = ConfigConst.NOT_SET;
            }

            if (this.queryContentObject != null)
            {
                this.queryContentText = this.queryContentObject.GetComponent<Text>();
            }

            if (this.rememberQueryHistoryToggleObject != null)
            {
                this.rememberQueryHistoryToggle = this.rememberQueryHistoryToggleObject.GetComponent<Toggle>();
                this.rememberQueryHistoryToggle.isOn = true;

                this.rememberQueryHistoryToggle.onValueChanged.AddListener(
                    delegate
                    {
                        this.OnRememberQueryHistoryToggleClicked();
                    }
                );
            }

            if (this.includeSystemSpecsToggleObject != null)
            {
                // no click listener required - this will be checked when the query is submitted
                // and the system specs will be integrated along a prompt
                this.includeSystemSpecsToggle = this.includeSystemSpecsToggleObject.GetComponent<Toggle>();
            }

            if (this.includeSystemDataToggleObject != null)
            {
                // no click listener required - this will be checked when the query is submitted
                // and the system data will be integrated along a prompt
                this.includeSystemDataToggle = this.includeSystemDataToggleObject.GetComponent<Toggle>();
            }

            if (this.systemDataCacheHoursContentObject != null)
            {
                this.systemDataCacheHoursText = this.systemDataCacheHoursContentObject.GetComponent<Text>();
            }

            if (this.submittedQueryContentObject != null)
            {
                this.submittedQueryContentText = this.submittedQueryContentObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.recommendationsContentObject != null)
            {
                this.hasRecommendationsPanel = true;

                this.recommendationsContentText = this.recommendationsContentObject.GetComponent<TextMeshProUGUI>();
            }

            // init buttons
            if (this.clearCacheButtonObject != null)
            {
                this.clearCacheButton = this.clearCacheButtonObject.GetComponent<Button>();

                if (this.clearCacheButton != null)
                {
                    this.clearCacheButton.onClick.AddListener(() => this.ClearPredictionEngineCache());
                }
            }

            if (this.resetQueryButtonObject != null)
            {
                this.resetQueryButton = this.resetQueryButtonObject.GetComponent<Button>();

                if (this.resetQueryButton != null)
                {
                    this.resetQueryButton.onClick.AddListener(() => this.ResetPredictionEngineQueries());
                }
            }

            if (this.reloadModelsButtonObject != null)
            {
                this.reloadModelsButton = this.reloadModelsButtonObject.GetComponent<Button>();

                if (this.reloadModelsButton != null)
                {
                    this.reloadModelsButton.onClick.AddListener(() => this.ReloadPredictionEngineModels());
                }
            }

            if (this.sendGeneralQueryButtonObject != null)
            {
                this.sendGeneralQueryButton = this.sendGeneralQueryButtonObject.GetComponent<Button>();

                if (this.sendGeneralQueryButton != null)
                {
                    this.sendGeneralQueryButton.onClick.AddListener(() => this.SendPredictionEngineQuery(false));
                }
            }

            if (this.sendPdmQueryButtonObject != null)
            {
                this.sendPdmQueryButton = this.sendPdmQueryButtonObject.GetComponent<Button>();

                if (this.sendPdmQueryButton != null)
                {
                    this.sendPdmQueryButton.onClick.AddListener(() => this.SendPredictionEngineQuery(true));
                }
            }

            if (this.uploadDocsButtonObject != null)
            {
                this.uploadDocsButton = this.uploadDocsButtonObject.GetComponent<Button>();

                if (this.uploadDocsButton != null)
                {
                    this.uploadDocsButton.onClick.AddListener(() => this.UploadDocsClicked());
                }
            }

            // init event listener
            if (this.eventListenerContainer != null)
            {
                try
                {
                    this.eventListener = this.eventListenerContainer.GetComponent<IDataContextExtendedListener>();
                }
                catch (Exception e)
                {
                    this.eventListener = null;

                    Debug.LogError(
                        "Can't find IDataContextExtendedListener reference in event listener container GameObject. Ignoring.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelList"></param>
        private void ProcessModelListUpdate(List<string> modelList)
        {
            if (modelList != null && modelList.Count > 0)
            {
                Debug.Log($"Cached AI models retrieved from {this.serverUri}. Count {modelList.Count}");

                this.aiModelSelector.ClearOptions();
                this.aiModelSelector.AddOptions(modelList);
            } else {
                Debug.Log($"No Cached AI models retrieved from {this.serverUri}.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseMsg"></param>
        private void ProcessQueryResponseUpdate(string responseMsg)
        {
            Debug.Log($"Cached AI query response received: {this.sessionID} - {this.serverUri}");

            if (!string.IsNullOrEmpty(responseMsg)) {
                if (this.recommendationsContentText != null)
                {
                    StringBuilder builder = new StringBuilder(this.recommendationsContentText.text);

                    if (builder.Length > 0)
                    {
                        builder.Append("\n\n");
                    }

                    builder.Append(responseMsg);

                    this.recommendationsContentText.text = builder.ToString();
                }
            } else {
                Debug.Log($"Cached AI response was empty for {sessionID}.");
            }
        }

        /// <summary>
        /// Prediction engine may or may not block on the request, so handle
        /// this within an async co-routine.
        /// </summary>
        private IEnumerator ReloadPredictionEngineModelsCoroutine()
        {
            bool isComplete = false;

            new Thread(() => {
               this.predictionManager.GetAllRegisteredModels(this.sessionID, this.serverUri);

                isComplete = true;
                this.updateAiModelList = true;
            }).Start();

            while (! isComplete)
            {
                yield return null;
            }

            yield return true;
        }

        /// <summary>
        /// Prediction engine may or may not block on the request, so handle
        /// this within an async co-routine.
        /// </summary>
        private IEnumerator SendPredictionEngineQueryCoroutine()
        {
            bool isComplete = false;

            new Thread(() => {
                if (this.predictionManager.SubmitQuery(this.sessionID, this.selectedAiModel, this.serverUri, this.queryRequestMsg))
                {
                    Debug.Log($"Submitted AI query: {this.sessionID} - {this.selectedAiModel}");
                }

                isComplete = true;
                this.updateAiResponseMsg = true;
            }).Start();

            while (! isComplete)
            {
                yield return null;
            }

            yield return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateUserSettings()
        {
            if (this.sessionIDTextInput != null)
            {
                this.sessionID = this.sessionIDTextInput.text;
            }

            if (this.aiUriTextInput != null)
            {
                this.serverUri = this.aiUriTextInput.text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateLabelsAndNames()
        {
            if (this.deviceIDText != null) this.deviceIDText.text = this.deviceID;
            if (this.deviceModelIDText != null) this.deviceModelIDText.text = this.dtmiUri;
            if (this.deviceModelNameText != null) this.deviceModelNameText.text = this.dtmiName;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateModelDataAndProperties()
        {
            // it's possible the model state object hasn't been provisioned yet;
            // however, we may still be able to load the model's JSON, as it's
            // retrieved via the controller ID, which we might already know
            Debug.Log("NORMAL: Updating digital twin model properties...");

            // if the model state has already been created,
            // (re) build the model data and display it
            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.BuildModelData();

                this.deviceID = this.digitalTwinModelState.GetDeviceID();
                this.dtmiUri = this.digitalTwinModelState.GetModelID();
                this.dtmiName = this.digitalTwinModelState.GetModelControllerID().ToString();
            }

            this.UpdateLabelsAndNames();
        }

    }

}
