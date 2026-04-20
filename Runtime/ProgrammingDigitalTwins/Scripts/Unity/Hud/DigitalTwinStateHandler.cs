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

using LabBenchStudios.Pdt.Unity.Common;
using LabBenchStudios.Pdt.Unity.Hud;

namespace LabBenchStudios.Pdt.Unity.Dashboard
{
    public class DigitalTwinStateHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private int modelVersion = 1;

        [SerializeField]
        private ModelNameUtil.DtmiControllerEnum controllerID = ModelNameUtil.DtmiControllerEnum.Custom;

        [SerializeField]
        private GameObject isCustomTypeToggleObject;

        [SerializeField]
        private GameObject typeNameSelectorObject;

        [SerializeField]
        private GameObject typeNameTextObject;

        [SerializeField]
        private bool listenToAllDevices = false;

        [SerializeField]
        private bool useGuidInInstanceKey = false;

        [SerializeField]
        private GameObject deviceIDObject = null;

        [SerializeField]
        private GameObject provisionDeviceTwinButtonObject = null;

        [SerializeField]
        private GameObject deviceIDSelectorObject = null;

        [SerializeField]
        private GameObject modelPanel = null;

        [SerializeField]
        private GameObject modelPanelIDObject = null;

        [SerializeField]
        private GameObject modelPanelNameObject = null;

        [SerializeField]
        private GameObject modelPanelContentObject = null;

        [SerializeField]
        private GameObject modelPanelCloseButtonObject = null;

        [SerializeField]
        private GameObject modelPanelShowButtonObject = null;

        [SerializeField]
        private GameObject statusPanel = null;

        [SerializeField]
        private GameObject propsPanelShowButtonObject = null;

        [SerializeField]
        private GameObject constraintsPanelShowButtonObject = null;

        [SerializeField]
        private GameObject statusPanelPropsContentObject = null;

        [SerializeField]
        private GameObject statusPanelNameObject = null;

        [SerializeField]
        private GameObject statusPanelIDObject = null;

        [SerializeField]
        private GameObject statusPanelCommandResourceObject = null;

        [SerializeField]
        private GameObject statusPanelConnStateLabelObject = null;

        [SerializeField]
        private GameObject statusPanelStateImageObject = null;

        [SerializeField]
        private GameObject statusPanelStateContentObject = null;

        [SerializeField]
        private GameObject pauseTelemetryButtonObject = null;

        [SerializeField]
        private GameObject resumeTelemetryButtonObject = null;

        [SerializeField]
        private GameObject displayMaintenanceButtonObject = null;

        [SerializeField]
        private GameObject statusPanelCloseButtonObject = null;

        [SerializeField]
        private GameObject propsEditorPanel = null;

        [SerializeField]
        private GameObject constraintsEditorPanel = null;

        [SerializeField]
        private GameObject displayMaintenancePanel = null;

        [SerializeField]
        private GameObject eventListenerContainer = null;

        [SerializeField]
        private List<GameObject> animationListenerContainerList = null;

        private Toggle isCustomTypeToggle = null;
        private InputField customNameInputField = null;
        private Text customNameText = null;
        private TMP_Text typeNameText = null;
        private TMP_Text connStateLabelText = null;
        private TMP_Text deviceIDText = null;
        private TMP_Text deviceCmdResourceText = null;
        private TMP_Text deviceName = null;
        private TMP_Text statusPanelID = null;
        private TMP_Text statusPanelName = null;
        private TMP_Text statusContentText = null;
        private TMP_Text propsContentText = null;
        private TMP_Text maintenancePanelContentText = null;
        private TMP_Text modelDataLoadStatusText = null;
        private TMP_Text filePathEntryText = null;

        private TMP_Text modelPanelID = null;
        private TMP_Text modelPanelName = null;
        private TMP_Text modelContentText = null;

        private TMP_Dropdown typeNameSelector = null;
        private TMP_Dropdown deviceIDSelector = null;

        private Image statusPanelStateImage = null;

        private Button provisionDeviceTwinButton = null;

        private Button resumeTelemetryButton = null;
        private Button pauseTelemetryButton = null;
        private Button displayMaintenanceButton = null;
        private Button showModelPanelButton = null;
        private Button closeModelPanelButton = null;
        private Button closeStatusPanelButton = null;
        private Button showPropsPanelButton = null;
        private Button showConstraintsPanelButton = null;

        private bool enableDebugLogging = true;

        private bool hasModelPanel = false;
        private bool hasModelPanelJsonContainer = false;
        private bool isModelPanelActive = false;

        private bool hasStatusPanel = false;
        private bool hasStatusPanelPropsContainer = false;
        private bool hasStatusPanelTelemetryContainer = false;

        private bool hasPropsEditorPanel = false;
        private bool isPropsEditorPanelActive = false;

        private bool hasConstraintsEditorPanel = false;
        private bool isConstraintsEditorPanelActive = false;

        private bool hasDisplayMaintenancePanel = false;
        private bool isDisplayMaintenancePanelActive = false;

        private bool enableIncomingTelemetry = true;

        private string customName = "";

        private string dtmiUri = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelNameUtil.IOT_MODEL_CONTEXT_NAME;
        private string deviceID = ConfigConst.NOT_SET;
        private string locationID = ConfigConst.NOT_SET;

        private int typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
        private int typeID = ConfigConst.DEFAULT_TYPE_ID;

        private string telemetryResourceName =
            ConfigConst.PRODUCT_NAME + "/" + ConfigConst.EDGE_DEVICE + "/" + ConfigConst.ACTUATOR_CMD;

        private string cmdResourceName =
            ConfigConst.PRODUCT_NAME + "/" + ConfigConst.EDGE_DEVICE + "/" + ConfigConst.ACTUATOR_CMD;

        private string modelProps = "";
        private string modelTelemetries = "";

        private DigitalTwinModelState digitalTwinModelState = null;
        private DigitalTwinPropertiesHandler digitalTwinPropsHandler = null;
        private DigitalTwinConstraintsHandler digitalTwinConstraintsHandler = null;
        private DigitalTwinDisplayMaintenanceHandler digitalTwinDisplayMaintenanceHandler = null;

        private ResourceNameContainer telemetryResource = null;
        private ResourceNameContainer cmdResource = null;

        private List<IDataContextEventListener> animationListenerList = new List<IDataContextEventListener>();
        private IDataContextExtendedListener eventListener = null;


        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void HandleUserEventState(UserEventState.EventType eventType)
        {
            switch (eventType)
            {
                case UserEventState.EventType.CloseOpenDialogs:
                    this.CloseStatusPanel();
                    this.CloseModelPanel();
                    this.ClosePropsEditorPanel();
                    this.CloseConstraintsEditorPanel();
                    this.CloseDisplayMaintenancePanel();
                    break;

                default:
                    // ignore
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseStatusPanel()
        {
            if (this.hasStatusPanel)
            {
                this.statusPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseModelPanel()
        {
            if (this.hasModelPanel)
            {
                this.modelPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClosePropsEditorPanel()
        {
            if (this.hasPropsEditorPanel)
            {
                this.propsEditorPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseConstraintsEditorPanel()
        {
            if (this.hasConstraintsEditorPanel)
            {
                this.constraintsEditorPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseDisplayMaintenancePanel()
        {
            if (this.hasDisplayMaintenancePanel)
            {
                this.displayMaintenancePanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnDeviceIDSelected()
        {
            if (this.deviceIDSelector != null)
            {
                this.deviceID = this.deviceIDSelector.captionText.text;
                this.locationID = this.deviceID;
            }

            this.deviceIDText.text = this.deviceID;

            // should already be created by now - if not, the deviceID
            // will be applied as soon as the model manager creates the
            // referential state object
            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.SetConnectedDeviceID(this.deviceID);
            }

            // allow the twin to be provisioned
            if (this.provisionDeviceTwinButton != null) this.provisionDeviceTwinButton.interactable = true;

            // update connection state - by the time we process the deviceName ID in this
            // UI component, the target remote deviceName may have already sent it's
            // connection state update, so we need to check if the local cache
            // has one we can process
            this.UpdateConnectionState();
            this.UpdateCommandResourceName();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnTypeNameSelected()
        {
            if (this.typeNameSelector != null)
            {
                this.customName = this.typeNameSelector.captionText.text;
                this.typeNameText.text = this.customName;

                switch (this.controllerID) {
                    case ModelNameUtil.DtmiControllerEnum.Custom:
                        this.EnableCustomTypeName(true);

                        this.dtmiUri =
                            ModelNameUtil.CreateModelID(ModelNameUtil.DTMI_PREFIX, this.customName, this.modelVersion);

                        break;
                }

                this.dtmiName = ModelNameUtil.GetNameFromDtmiURI(this.dtmiUri);

                Debug.Log($"Custom type name selected: {this.customName} - {this.dtmiName} - {this.dtmiUri}");

                this.UpdateModelDataAndProperties(false);
                this.UpdatePanelLabelNames();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateDeviceIDList()
        {
            if (this.deviceIDSelector != null)
            {
                List<string> deviceIDList = EventProcessor.GetInstance().GetAllKnownDeviceIDs();

                if (deviceIDList != null && deviceIDList.Count > 0)
                {
                    this.deviceIDSelector.ClearOptions();
                    this.deviceIDSelector.AddOptions(deviceIDList);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateModelPanelVisibility()
        {
            if (this.hasModelPanel)
            {
                this.isModelPanelActive = !this.isModelPanelActive;
                this.modelPanel.SetActive(this.isModelPanelActive);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePropsEditorPanelVisibility()
        {
            if (this.hasPropsEditorPanel)
            {
                this.isPropsEditorPanelActive = !this.isPropsEditorPanelActive;
                this.propsEditorPanel.SetActive(this.isPropsEditorPanelActive);

                if (this.propsEditorPanel.activeInHierarchy) {
                    if (this.hasDisplayMaintenancePanel) {
                        this.displayMaintenancePanel.SetActive(false);
                    }

                    if (this.hasConstraintsEditorPanel) {
                        this.constraintsEditorPanel.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateConstraintsEditorPanelVisibility()
        {
            if (this.hasConstraintsEditorPanel)
            {
                this.isConstraintsEditorPanelActive = !this.isConstraintsEditorPanelActive;
                this.constraintsEditorPanel.SetActive(this.isConstraintsEditorPanelActive);

                if (this.constraintsEditorPanel.activeInHierarchy) {
                    if (this.hasDisplayMaintenancePanel) {
                        this.displayMaintenancePanel.SetActive(false);
                    }

                    if (this.hasPropsEditorPanel) {
                        this.propsEditorPanel.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateDisplayMaintenancePanelVisibility()
        {
            if (this.hasDisplayMaintenancePanel)
            {
                this.isDisplayMaintenancePanelActive = !this.isDisplayMaintenancePanelActive;
                this.displayMaintenancePanel.SetActive(this.isDisplayMaintenancePanelActive);

                if (this.displayMaintenancePanel.activeInHierarchy) {
                    if (this.hasPropsEditorPanel) {
                        this.propsEditorPanel.SetActive(false);
                    }

                    if (this.hasConstraintsEditorPanel)
                    {
                        this.constraintsEditorPanel.SetActive(false);
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

            if (this.pauseTelemetryButton != null) this.pauseTelemetryButton.interactable = false;
            if (this.resumeTelemetryButton != null) this.resumeTelemetryButton.interactable = true;

            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.EnableIncomingTelemetryProcessing(this.enableIncomingTelemetry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResumeIncomingTelemetry()
        {
            this.enableIncomingTelemetry = true;

            if (this.pauseTelemetryButton != null) this.pauseTelemetryButton.interactable = true;
            if (this.resumeTelemetryButton != null) this.resumeTelemetryButton.interactable = false;

            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.EnableIncomingTelemetryProcessing(this.enableIncomingTelemetry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PauseDeviceTelemetry()
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

        /// <summary>
        /// 
        /// </summary>
        public void OnModelUpdateEvent()
        {
            this.UpdateModelDataAndProperties();
        }


        // protected

        /// <summary>
        /// 
        /// </summary>
        private void InitPropsEditorPanelControls()
        {
            if (this.propsEditorPanel != null)
            {
                this.digitalTwinPropsHandler = this.propsEditorPanel.GetComponent<DigitalTwinPropertiesHandler>();

                this.hasPropsEditorPanel = true;
                this.propsEditorPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitConstraintsEditorPanelControls()
        {
            if (this.constraintsEditorPanel != null)
            {
                this.digitalTwinConstraintsHandler = this.constraintsEditorPanel.GetComponent<DigitalTwinConstraintsHandler>();

                this.hasConstraintsEditorPanel = true;
                this.constraintsEditorPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitDisplayMaintenancePanelControls()
        {
            if (this.displayMaintenancePanel != null)
            {
                this.digitalTwinDisplayMaintenanceHandler = this.displayMaintenancePanel.GetComponent<DigitalTwinDisplayMaintenanceHandler>();

                this.hasDisplayMaintenancePanel = true;
                this.displayMaintenancePanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitMainStatusPanelControls()
        {
            if (this.statusPanel != null)
            {
                this.hasStatusPanel = true;
            }

            if (this.typeNameTextObject != null)
            {
                this.typeNameText = this.typeNameTextObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.deviceIDObject != null)
            {
                this.deviceIDText = this.deviceIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.isCustomTypeToggleObject != null)
            {
                this.isCustomTypeToggle = this.isCustomTypeToggleObject.GetComponent<Toggle>();
            }

            if (this.typeNameTextObject != null) {
                this.typeNameText = this.typeNameTextObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.typeNameSelectorObject != null)
            {
                this.typeNameSelector = this.typeNameSelectorObject.GetComponent<TMP_Dropdown>();
            }

            if (this.deviceIDSelectorObject != null)
            {
                this.deviceIDSelector = this.deviceIDSelectorObject.GetComponent<TMP_Dropdown>();

                this.deviceIDSelector.onValueChanged.AddListener(
                    delegate { this.OnDeviceIDSelected(); }
                );
            }

            if (this.statusPanelCommandResourceObject != null)
            {
                this.deviceCmdResourceText = this.statusPanelCommandResourceObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.statusPanelConnStateLabelObject != null)
            {
                this.connStateLabelText = this.statusPanelConnStateLabelObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.statusPanelStateImageObject != null)
            {
                this.statusPanelStateImage = this.statusPanelStateImageObject.GetComponent<Image>();
            }

            if (this.statusPanelNameObject != null)
            {
                this.statusPanelName = this.statusPanelNameObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.statusPanelIDObject != null)
            {
                this.statusPanelID = this.statusPanelIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.statusPanelPropsContentObject != null)
            {
                this.hasStatusPanelPropsContainer = true;

                this.propsContentText = this.statusPanelPropsContentObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.statusPanelStateContentObject != null)
            {
                this.hasStatusPanelTelemetryContainer = true;

                this.statusContentText = this.statusPanelStateContentObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.animationListenerContainerList != null && this.animationListenerContainerList.Count > 0)
            {
                try
                {
                    foreach (GameObject go in this.animationListenerContainerList)
                    {
                        IDataContextEventListener animationListener = go.GetComponent<IDataContextEventListener>();
                        this.animationListenerList.Add(animationListener);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        "Can't find IDataContextEventListener reference in animation listener container list. Ignoring.");
                }
            }

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

            // init buttons
            if (this.provisionDeviceTwinButtonObject != null)
            {
                this.provisionDeviceTwinButton = this.provisionDeviceTwinButtonObject.GetComponent<Button>();

                if (this.provisionDeviceTwinButton != null)
                {
                    if (this.provisionDeviceTwinButton != null) this.provisionDeviceTwinButton.interactable = false;
                    this.provisionDeviceTwinButton.onClick.AddListener(() => this.ProvisionModelState());
                }
            }

            if (this.statusPanelCloseButtonObject != null)
            {
                this.closeStatusPanelButton = this.statusPanelCloseButtonObject.GetComponent<Button>();

                if (this.closeStatusPanelButton != null)
                {
                    this.closeStatusPanelButton.onClick.AddListener(() => this.CloseStatusPanel());
                }
            }

            if (this.propsPanelShowButtonObject != null)
            {
                this.showPropsPanelButton = this.propsPanelShowButtonObject.GetComponent<Button>();

                if (this.showPropsPanelButton != null)
                {
                    this.showPropsPanelButton.onClick.AddListener(() => this.UpdatePropsEditorPanelVisibility());
                }
            }

            if (this.constraintsPanelShowButtonObject != null)
            {
                this.showConstraintsPanelButton = this.constraintsPanelShowButtonObject.GetComponent<Button>();

                if (this.showConstraintsPanelButton != null)
                {
                    this.showConstraintsPanelButton.onClick.AddListener(() => this.UpdateConstraintsEditorPanelVisibility());
                }
            }

            if (this.displayMaintenanceButtonObject != null)
            {
                this.displayMaintenanceButton = this.displayMaintenanceButtonObject.GetComponent<Button>();

                if (this.displayMaintenanceButton != null)
                {
                    this.displayMaintenanceButton.onClick.AddListener(() => this.UpdateDisplayMaintenancePanelVisibility());
                }
            }

            if (this.resumeTelemetryButtonObject != null)
            {
                this.resumeTelemetryButton = this.resumeTelemetryButtonObject.GetComponent<Button>();

                if (this.resumeTelemetryButton != null)
                {
                    this.resumeTelemetryButton.onClick.AddListener(() => this.ResumeIncomingTelemetry());
                }
            }

            if (this.pauseTelemetryButtonObject != null)
            {
                this.pauseTelemetryButton = this.pauseTelemetryButtonObject.GetComponent<Button>();

                if (this.pauseTelemetryButton != null)
                {
                    this.pauseTelemetryButton.onClick.AddListener(() => this.PauseIncomingTelemetry());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitModelPanelControls()
        {
            if (this.modelPanel != null)
            {
                this.hasModelPanel = true;
                this.modelPanel.SetActive(false);
            }

            if (this.modelPanelNameObject != null)
            {
                this.modelPanelName = this.modelPanelNameObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.modelPanelIDObject != null)
            {
                this.modelPanelID = this.modelPanelIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.modelPanelContentObject != null)
            {
                this.modelContentText = this.modelPanelContentObject.GetComponent<TextMeshProUGUI>();

                if (this.modelContentText != null) this.hasModelPanelJsonContainer = true;
            }

            // init buttons
            if (this.modelPanelCloseButtonObject != null)
            {
                this.closeModelPanelButton = this.modelPanelCloseButtonObject.GetComponent<Button>();

                if (this.closeModelPanelButton != null)
                {
                    this.closeModelPanelButton.onClick.AddListener(() => this.CloseModelPanel());
                }
            }

            if (this.modelPanelShowButtonObject != null)
            {
                this.showModelPanelButton = this.modelPanelShowButtonObject.GetComponent<Button>();

                if (this.showModelPanelButton != null)
                {
                    this.showModelPanelButton.onClick.AddListener(() => this.UpdateModelPanelVisibility());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try
            {
                // =====
                // Initialize basic UI controls
                // =====

                // Step 1: Init main UI panel - these are needed
                //                for the remaining steps
                this.InitMainStatusPanelControls();

                // =====
                // Configure Types and State
                // =====

                // Step 2: Check if the controller ID is custom or static
                //         if static (preset), configure DTMI info -
                //         if custom, need to wait for user to select the type
                switch (this.controllerID) {
                    // Step 2a: Enable custom type name - selected and initialized later
                    case ModelNameUtil.DtmiControllerEnum.Custom:
                        this.EnableCustomTypeName(true);
                        break;

                    // Step 2b: Configure static controller model - initialized now
                    //          Set DTMI labels and ID's
                    default:
                        this.EnableCustomTypeName(false);

                        this.dtmiUri =
                            ModelNameUtil.CreateModelID(this.controllerID, this.modelVersion);

                        break;
                }

                // Step 3: Create the DTMI name for this asset
                this.dtmiName = ModelNameUtil.GetNameFromDtmiURI(this.dtmiUri);

                // =====
                // Init Internal Panels
                // =====

                // Step 4: Init remaining panels and their event controllers
                this.InitModelPanelControls();
                this.InitPropsEditorPanelControls();
                this.InitConstraintsEditorPanelControls();
                this.InitDisplayMaintenancePanelControls();

                // =====
                // Update ID's and Names
                // =====

                // Step 5: Update deviceName ID list - this populates the
                //         telemetry source device ID's (names)
                this.UpdateDeviceIDList();

                // Step 6: Update the curCommand resource name
                this.UpdateCommandResourceName();

                // Step 7: Update all the panel's label names
                this.UpdatePanelLabelNames();
                
                // =====
                // Handle local state updates (if any exist)
                // =====

                // Step 8: Update any loaded model data

                // Retrieve any loaded model data (DTDL and config type info)
                //  - if this asset is pre-configured (non-custom), this will be mapped
                //    via the pre-provisioned ID that maps to the DTMI for the asset
                //  - if this asset is custom, the DTDL will be loaded once the user
                //    selects the custom type from the drop down list
                this.UpdateModelDataAndProperties();

                // =====
                // Handle event registrations and state 'start'
                // =====

                // Step 9: Register for events
                base.RegisterForSystemStatusEvents((ISystemStatusEventListener)this);

                // Step 10: Start in 'resume incoming telemetry processing' state
                this.ResumeIncomingTelemetry();
            }
            catch (Exception ex)
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
                String jsonData = DataUtil.ActuatorDataToJson(data);

                if (this.animationListenerList.Count > 0)
                {
                    foreach (IDataContextEventListener animationListener in this.animationListenerList)
                    {
                        animationListener.HandleActuatorData(data);
                    }
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleActuatorData(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessConnectionStateData(ConnectionStateData data)
        {
            this.UpdateDeviceIDList();

            if (this.IsIncomingTelemetryProcessingEnabled(data))
            {
                String connStateMsg = "...";

                if (data.IsClientConnected()) connStateMsg = "Connected";
                else if (data.IsClientConnecting()) connStateMsg = "Connecting...";
                else if (data.IsClientDisconnected()) connStateMsg = "Disconnected";
                else connStateMsg = "Unknown";

                if (this.connStateLabelText != null) this.connStateLabelText.text = connStateMsg;

                if (this.animationListenerList.Count > 0)
                {
                    foreach (IDataContextEventListener animationListener in this.animationListenerList)
                    {
                        animationListener.HandleConnectionStateData(data);
                    }
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleConnectionStateData(data);
                }
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
                // nothing to do

                if (this.animationListenerList.Count > 0)
                {
                    foreach (IDataContextEventListener animationListener in this.animationListenerList)
                    {
                        animationListener.HandleMessageData(data);
                    }
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleMessageData(data);
                }
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
                if (this.enableDebugLogging)
                {
                    StringBuilder sb = new StringBuilder(this.modelTelemetries);

                    sb.Append("\n==========");
                    sb.Append($"\nSensor Name: {data.GetName()}");
                    sb.Append($"\nSensor Value: {data.GetValue()}");
                    if (!string.IsNullOrEmpty(data.GetStateData()))
                        sb.Append($"\nState Data: {data.GetStateData()}");
                    sb.Append("\n==========");
                    sb.Append($"\nIncoming Key: {ModelNameUtil.GenerateDataSyncKey(data)}");

                    if (this.digitalTwinModelState != null)
                    {
                        sb.Append($"\nData Sync Key: {this.digitalTwinModelState.GetDataSyncKey()}");
                        sb.Append($"\nModel Sync Key: {this.digitalTwinModelState.GetModelSyncKey()}");
                    }

                    this.statusContentText.text = sb.ToString();
                }

                if (this.animationListenerList.Count > 0)
                {
                    foreach (IDataContextEventListener animationListener in this.animationListenerList)
                    {
                        animationListener.HandleSensorData(data);
                    }
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleSensorData(data);
                }
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
                if (this.enableDebugLogging)
                {
                    StringBuilder sb = new StringBuilder(this.modelTelemetries);

                    sb.Append("\n==========");
                    sb.Append($"\nCPU Util: {data.GetCpuUtilization()}");
                    sb.Append($"\nMem Util: {data.GetMemoryUtilization()}");
                    sb.Append($"\nDisk Util: {data.GetDiskUtilization()}");

                    this.statusContentText.text = sb.ToString();
                }

                if (this.animationListenerList.Count > 0)
                {
                    foreach (IDataContextEventListener animationListener in this.animationListenerList)
                    {
                        animationListener.HandleSystemPerformanceData(data);
                    }
                }

                if (this.eventListener != null)
                {
                    this.eventListener.HandleSystemPerformanceData(data);
                }
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
        /// <param name="enable"></param>
        private void EnableCustomTypeName(bool enable)
        {
            if (this.typeNameSelector != null) {
            }

            if (enable) {
                if (this.isCustomTypeToggle != null) {
                    this.isCustomTypeToggle.isOn = true;
                }

                if (this.typeNameSelector != null) {
                    this.typeNameSelector.interactable = true;
                    this.typeNameSelector.onValueChanged.AddListener(
                        delegate { this.OnTypeNameSelected(); }
                    );
                }
            } else {
                if (this.isCustomTypeToggle != null) {
                    this.isCustomTypeToggle.isOn = false;
                }

                if (this.typeNameSelector != null) {
                    this.typeNameSelector.interactable = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsIncomingTelemetryProcessingEnabled(IotDataContext data)
        {
            // placeholder for further narrowing of IotDataContext data processing
            // for now, this is moot - the model manager distributes updates to the
            // appropriate model state, which in turn notifies its listener (this)
            // of those updates
            if (data != null)
            {
                return this.enableIncomingTelemetry;
            }
            else
            {
                Debug.LogWarning("Incoming telemetry object is NULL: " + data);
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void ProvisionModelState()
        {
            DigitalTwinModelManager dtModelManager =
                EventProcessor.GetInstance().GetDigitalTwinModelManager();

            SystemModelManager sysModelManager =
                EventProcessor.GetInstance().GetSystemModelManager();

            if (dtModelManager != null)
            {
                IotDataContext dataContext = null;
                bool isCustom = false;

                switch (this.controllerID)
                {
                    case ModelNameUtil.DtmiControllerEnum.Custom:
                        isCustom = true;
                        dataContext =
                            ModelNameUtil.GenerateDataContext(this.controllerID, this.deviceID, this.locationID, this.customName);
                        break;

                    default:
                        dataContext =
                            ModelNameUtil.GenerateDataContext(this.controllerID, this.deviceID, this.locationID);
                        break;
                }

                this.typeCategoryID = dataContext.GetTypeCategoryID();
                this.typeID = dataContext.GetTypeID();

                if (this.enableDebugLogging)
                {
                    Debug.Log(
                        $"NORMAL: Provisioning DT model state instance with " +
                        $"\n\tURI = {this.dtmiUri}" +
                        $"\n\tName = {this.dtmiName}" +
                        $"\n\tModel Name = {this.customName}" +
                        $"\n\tDevice ID = {this.deviceID}" +
                        $"\n\tLocation ID = {this.locationID}" +
                        $"\n\tType Category ID = {this.typeCategoryID}" +
                        $"\n\tType ID = {this.typeID}" +
                        $"\n\tController ID = {this.controllerID}" +
                        $"\n\tIs Custom = {isCustom}");
                }

                if (this.digitalTwinModelState == null)
                {
                    /*
                    if (isCustom)
                    {
                        this.digitalTwinModelState =
                            dtModelManager.CreateModelState(
                                this.deviceID,
                                this.locationID,
                                this.typeCategoryID,
                                this.typeID,
                                this.useGuidInInstanceKey,
                                this.customName,
                                (IDataContextEventListener)this);
                    } else {
                        this.digitalTwinModelState =
                            dtModelManager.CreateModelState(
                                this.deviceID,
                                this.locationID,
                                this.typeCategoryID,
                                this.typeID,
                                this.useGuidInInstanceKey,
                                this.controllerID,
                                (IDataContextEventListener)this);
                    }
                    */

                    if (isCustom)
                    {
                        this.digitalTwinModelState =
                            sysModelManager.CreateDigitalTwinModelState(
                                dataContext, this.customName, this.useGuidInInstanceKey, (IDataContextEventListener) this);
                    } else {
                        this.digitalTwinModelState =
                            sysModelManager.CreateDigitalTwinModelState(
                                dataContext, this.controllerID, this.useGuidInInstanceKey, (IDataContextEventListener) this);
                    }

                    if (this.digitalTwinModelState == null)
                    {
                        Debug.LogError($"Failed to create digital twin model state: {this.controllerID}");
                    }
                }
                else
                {
                    this.digitalTwinModelState.UpdateConnectionState(this.deviceID, this.locationID);

                    dtModelManager.UpdateModelState(this.digitalTwinModelState);
                }

                this.UpdateCommandResourceName();

                if (this.digitalTwinPropsHandler != null)
                {
                    this.digitalTwinPropsHandler.SetDigitalTwinModelState(this.digitalTwinModelState);
                }

                if (this.digitalTwinConstraintsHandler != null)
                {
                    this.digitalTwinConstraintsHandler.SetDigitalTwinModelState(this.digitalTwinModelState);
                }

                if (this.digitalTwinDisplayMaintenanceHandler != null)
                {
                    this.digitalTwinDisplayMaintenanceHandler.SetDigitalTwinModelState(this.digitalTwinModelState);
                }

                if (this.eventListener != null)
                {
                    this.eventListener.SetDigitalTwinStateProcessor(this.digitalTwinModelState);
                }

                // trigger model updates (retrieve from SystemModelManager)
                this.OnModelUpdateEvent();

                Debug.Log($"NORMAL: Created model state with URI {this.dtmiUri} and instance {this.digitalTwinModelState.GetModelSyncKey()}");
                Debug.Log($"NORMAL: Raw DTDL JSON\n==========\n{this.digitalTwinModelState.GetModelJson()}\n==========\n");

                // once provisioned, we're done with the provisioning button
                if (this.provisionDeviceTwinButton != null) this.provisionDeviceTwinButton.interactable = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateConnectionState()
        {
            ConnectionStateData connStateData =
                EventProcessor.GetInstance().GetConnectionState(this.deviceID);

            if (connStateData != null)
            {
                this.OnMessagingSystemStatusUpdate(connStateData);
            }
            else
            {
                Debug.LogWarning($"No cached connection state for {this.deviceID}");
            }
        }

        /// <summary>
        /// Updates all DTMI, model, type and device names in the various ref'd panels.
        /// </summary>
        private void UpdatePanelLabelNames()
        {
            if (this.typeNameText != null) {
                if (string.IsNullOrWhiteSpace(this.customName)) {
                    this.customName = this.dtmiName;
                }

                this.typeNameText.text = customName;
            }

            if (this.modelPanelName != null) {
                this.modelPanelName.text = this.dtmiName;
            }

            if (this.modelPanelID != null) {
                this.modelPanelID.text = this.dtmiUri;
            }

            if (this.statusPanelName != null) {
                this.statusPanelName.text = this.dtmiName;
            }

            if (this.statusPanelID != null) {
                this.statusPanelID.text = this.dtmiUri;
            }

            if (this.deviceCmdResourceText != null)
            {
                this.deviceCmdResourceText.text = this.cmdResourceName;
            }

            // make sure all the hidden panels get updated
            this.digitalTwinPropsHandler?.UpdateDigitalTwinLabels(this.dtmiName, this.dtmiUri);
            this.digitalTwinConstraintsHandler?.UpdateDigitalTwinLabels(this.dtmiName, this.dtmiUri);
            this.digitalTwinDisplayMaintenanceHandler?.UpdateDigitalTwinLabels(this.dtmiName, this.dtmiUri);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateConfigTypeModelList()
        {
            Debug.Log("Updating config type names...");

            if (this.typeNameSelector != null)
            {
                List<string> typeNameList =
                    EventProcessor.GetInstance().GetSystemModelManager().GetConfigTypeModelManager().GetLoadedConfigTypeNames();

                if (typeNameList != null && typeNameList.Count > 0)
                {
                    this.typeNameSelector.ClearOptions();
                    this.typeNameSelector.AddOptions(typeNameList);
                    
                    Debug.Log($"Added {typeNameList.Count} config type names.");

                    // set the option index to that of the currently selected name
                    string optionName = "";

                    if (!string.IsNullOrEmpty(this.customName))
                    {
                        optionName = this.customName;
                    } else {
                        optionName = this.dtmiName;
                    }

                    try {
                        int index = typeNameList.IndexOf(optionName);
                        this.typeNameSelector.SetValueWithoutNotify(index);
                    } catch (Exception e) {
                        // ignore - this doesn't functionally matter, it simply
                        // makes the UI display look cleaner
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateDigitalTwinModel()
        {
            if (this.modelContentText != null)
            {
                DigitalTwinModelManager modelMgr = EventProcessor.GetInstance().GetDigitalTwinModelManager();

                if (this.digitalTwinModelState != null)
                {
                    Debug.Log($"Updating model JSON via DT Model State instance: {this.digitalTwinModelState.GetModelControllerID()}");
                    this.modelContentText.text =
                        this.digitalTwinModelState.GetModelJson();
                }
                else
                {
                    Debug.Log($"Updating model JSON via DT Model Manager (state not yet created): {this.controllerID}");

                    switch (this.controllerID) {
                        case ModelNameUtil.DtmiControllerEnum.Custom:
                            this.modelContentText.text =
                                modelMgr.GetDigitalTwinModelJson(this.dtmiName);
                            break;

                        default:
                            this.modelContentText.text =
                                modelMgr.GetDigitalTwinModelJson(this.controllerID);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateModelDataAndProperties()
        {
            this.UpdateModelDataAndProperties(true);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateModelDataAndProperties(bool inclConfigTypes)
        {
            // it's possible the model state object hasn't been provisioned yet;
            // however, we may still be able to load the model's JSON, as it's
            // retrieved via the controller ID, which we might already know
            Debug.Log("NORMAL: Updating digital twin model JSON data...");

            this.UpdateDigitalTwinModel();

            if (inclConfigTypes) {
                Debug.Log("NORMAL: Updating config type model JSON data...");

                this.UpdateConfigTypeModelList();
            }

            // if the model state has already been created,
            // (re) build the model data and display it
            if (this.digitalTwinModelState != null)
            {
                this.digitalTwinModelState.BuildModelData();

                List<string> propKeys = this.digitalTwinModelState.GetModelPropertyKeys();
                StringBuilder propKeysStr = new StringBuilder();

                foreach (string key in propKeys)
                {
                    propKeysStr.Append(key).Append("\n");
                }

                this.modelProps = propKeysStr.ToString();
                this.propsContentText.text = this.modelProps;

                Debug.Log(
                    $"Property Keys for {this.digitalTwinModelState.GetModelSyncKey()}:\n{this.propsContentText.text}");

                List<string> telemetryKeys = this.digitalTwinModelState.GetModelPropertyTelemetryKeys();
                StringBuilder telemetryKeysStr = new StringBuilder();

                foreach (string key in telemetryKeys)
                {
                    telemetryKeysStr.Append(key).Append("\n");
                }

                this.modelTelemetries = telemetryKeysStr.ToString();
                this.statusContentText.text = this.modelTelemetries;

                Debug.Log(
                    $"Telemetry Keys for {this.digitalTwinModelState.GetModelSyncKey()}:\n{this.statusContentText.text}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCommandResourceName()
        {
            if (this.cmdResource == null)
            {
                this.cmdResource =
                    new ResourceNameContainer(
                        this.deviceID, this.locationID, ConfigConst.ACTUATOR_CMD);

                this.cmdResource.TypeCategoryID = this.typeCategoryID;
                this.cmdResource.TypeID = this.typeID;
            }
            else
            {
                this.cmdResource.DeviceName = this.deviceID;
            }

            if (this.digitalTwinModelState != null)
            {
                this.cmdResource.ResourcePrefix = this.digitalTwinModelState.GetResourcePrefix();
            }

            this.cmdResource.InitFullResourceName();

            this.cmdResourceName = this.cmdResource.GetFullResourceName();

            if (this.digitalTwinPropsHandler != null)
            {
                this.digitalTwinPropsHandler.SetDigitalTwinCommandResource(this.cmdResource);
            }

            if (this.digitalTwinDisplayMaintenanceHandler != null)
            {
                this.digitalTwinDisplayMaintenanceHandler.SetDigitalTwinCommandResource(this.cmdResource);
            }
        }

    }

}
