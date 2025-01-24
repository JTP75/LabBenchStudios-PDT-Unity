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

namespace LabBenchStudios.Pdt.Unity.Hud
{
    public class DigitalTwinConstraintsHandler : BaseAsyncDataMessageProcessor, ISystemStatusEventListener
    {
        [SerializeField]
        private GameObject connStateObject = null;

        [SerializeField]
        private GameObject deviceIDObject = null;

        [SerializeField]
        private GameObject deviceModelNameObject = null;

        [SerializeField]
        private GameObject deviceModelIDObject = null;

        [SerializeField]
        private GameObject primaryConfigTypeNameObject = null;

        [SerializeField]
        private GameObject propertyNameSelectorObject = null;

        [SerializeField]
        private GameObject primaryPropsContainerObject = null;

        [SerializeField]
        private GameObject primaryConstraintGuiTemplate = null;

        [SerializeField]
        private GameObject constraintContainerObject = null;

        [SerializeField]
        private GameObject updateConstraintsButtonObject = null;

        [SerializeField]
        private GameObject constraintGuiTemplate = null;

        [SerializeField]
        private GameObject configTypeModelNameObject = null;

        [SerializeField]
        private GameObject configTypeModelIDObject = null;

        [SerializeField]
        private GameObject configTypeModelContentObject = null;

        [SerializeField]
        private GameObject eventListenerContainer = null;

        private GameObject constraintsPanel = null;

        private TMP_Text connStateLabelText = null;
        private TMP_Text deviceIDText = null;
        private TMP_Text deviceModelNameText = null;
        private TMP_Text deviceModelIDText = null;
        private TMP_Text deviceCmdResourceText = null;
        private TMP_Text primaryConfigTypeNameText = null;
        private TMP_Text configTypeModelNameText = null;
        private TMP_Text configTypeModelIDText = null;
        private TMP_Text configTypeModelText = null;

        private TMP_Dropdown propertyNameSelector = null;

        private Button updateConstraintsButton = null;

        private bool hasPrimaryConstraintsPanel = false;
        private bool hasConstraintsPanel = false;

        private string modelName = ConfigConst.NOT_SET;
        private string deviceID = ConfigConst.NOT_SET;
        private string locationID = ConfigConst.NOT_SET;
        private string dtmiUri  = ModelNameUtil.IOT_MODEL_CONTEXT_MODEL_ID;
        private string dtmiName = ModelNameUtil.IOT_MODEL_CONTEXT_NAME;

        private string configTypeName = ConfigConst.NOT_SET;

        private string cmdResourceName =
            ConfigConst.PRODUCT_NAME + "/" + ConfigConst.EDGE_DEVICE + "/" + ConfigConst.ACTUATOR_CMD;

        private string modelProps = "";

        private float verticalAnchorDelta = 200.0f;

        private List<GameObject> digitalTwinGuiConstraintsList = new List<GameObject>();
        private List<IPropertyManagementController> propertyUpdateHandlerList = new List<IPropertyManagementController>();

        private DigitalTwinModelState digitalTwinModelState = null;

        private ResourceNameContainer cmdResource = null;

        private IDataContextExtendedListener eventListener = null;


        // public methods (button interactions)

        /// <summary>
        /// 
        /// </summary>
        public void ClosePropertiesPanel()
        {
            if (this.hasConstraintsPanel)
            {
                this.constraintsPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveConstraints()
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

                this.UpdateCommandResource();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtModelState"></param>
        public void SetDigitalTwinModelState(DigitalTwinModelState dtModelState)
        {
            if (dtModelState != null)
            {
                Debug.Log($"Setting digital twin model state: {dtModelState.GetModelID()}");

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


        // protected

        /// <summary>
        /// 
        /// </summary>
        protected void OnConfigTypeNameSelected()
        {
            if (this.propertyNameSelector != null)
            {
                this.configTypeName = this.propertyNameSelector.captionText.text;
                this.configTypeModelNameText.text = this.configTypeName;

                // TODO: switch to the appropriate config type content

                Debug.Log($"Config type name selected: {this.configTypeName} - {this.dtmiName} - {this.dtmiUri}");

                this.UpdateModelPropsAndConstraints();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitMessageHandler()
        {
            try
            {
                // first: init controls
                this.InitConstraintsPanelControls();

                // second: update model data and config type info
                this.UpdateModelDataAndProperties();

                // finally: register for events
                base.RegisterForSystemStatusEvents((ISystemStatusEventListener) this);
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
        private void InitConstraintsPanelControls()
        {
            this.constraintsPanel = gameObject;

            if (this.deviceIDObject != null)
            {
                this.deviceIDText = this.deviceIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.connStateObject != null)
            {
                this.connStateLabelText = this.connStateObject.GetComponent<TextMeshProUGUI>();
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

            if (this.primaryConfigTypeNameObject != null)
            {
                this.primaryConfigTypeNameText = this.primaryConfigTypeNameObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.configTypeModelContentObject != null)
            {
                this.configTypeModelText = this.configTypeModelContentObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.primaryPropsContainerObject != null)
            {
                this.hasPrimaryConstraintsPanel = true;
            }

            if (this.constraintContainerObject != null)
            {
                this.hasConstraintsPanel = true;
            }

            if (this.configTypeModelIDObject != null)
            {
                this.configTypeModelIDText = this.configTypeModelIDObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.configTypeModelNameObject != null)
            {
                this.configTypeModelNameText = this.configTypeModelNameObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.propertyNameSelectorObject != null)
            {
                this.propertyNameSelector = this.propertyNameSelectorObject.GetComponent<TMP_Dropdown>();

                this.propertyNameSelector.onValueChanged.AddListener(
                    delegate { this.OnConfigTypeNameSelected(); }
                );
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
            if (this.updateConstraintsButtonObject != null)
            {
                this.updateConstraintsButton = this.updateConstraintsButtonObject.GetComponent<Button>();

                if (this.updateConstraintsButton != null)
                {
                    this.updateConstraintsButton.onClick.AddListener(() => this.SaveConstraints());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCommandResource()
        {
            if (this.deviceCmdResourceText != null)
            {
                if (this.cmdResource != null)
                {
                    this.deviceCmdResourceText.text = this.cmdResource.GetFullResourceName();
                }
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
            if (this.primaryConfigTypeNameText != null) this.primaryConfigTypeNameText.text = this.modelName;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateConfigTypeNames()
        {
            Debug.Log("Updating config type names...");

            if (this.propertyNameSelector != null)
            {
                if (this.digitalTwinModelState != null)
                {
                    Debug.Log($"Looking up config categories by model name: {this.digitalTwinModelState.GetModelID()} = {this.dtmiName} - {this.dtmiUri}");

                    // get json
                    ConfigTypeModelContainer container =
                        EventProcessor.GetInstance().GetConfigTypeModelManager().GetConfigCategoryByModelName(
                            this.digitalTwinModelState.GetModelID());

                    string modelJson = container.GetJsonData();

                    if (this.configTypeModelText != null)
                    {
                        this.configTypeModelText.text = modelJson;
                    }

                    List<string> modelPropNames = this.digitalTwinModelState.GetModelPropertyKeys();

                    if (modelPropNames != null && modelPropNames.Count > 0)
                    {
                        this.propertyNameSelector.ClearOptions();
                        this.propertyNameSelector.AddOptions(modelPropNames);

                        Debug.Log($"Added {modelPropNames.Count} config type names.");

                        // default to the first index
                        this.propertyNameSelector.SetValueWithoutNotify(0);

                        this.OnConfigTypeNameSelected();
                    }
                } else {
                    Debug.LogWarning($"DT model state not yet created: {this.dtmiName}");
                }
            } else {
                Debug.LogError($"Property name selector UI control not initilized. Fix in Editor.");
            }
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
                this.modelName = this.digitalTwinModelState.GetTypeName();

                List<string> propKeys = this.digitalTwinModelState.GetModelPropertyKeys();
                StringBuilder propKeysStr = new StringBuilder();

                foreach (string key in propKeys)
                {
                    propKeysStr.Append(key).Append(',');
                }

                Debug.Log($"Property Keys for {this.digitalTwinModelState.GetModelSyncKey()} - model {this.modelName}: {propKeysStr}");

                this.UpdateConfigTypeNames();
                this.UpdateCommandResource();
                this.UpdateLabelsAndNames();
            }
        }

        /// <summary>
        /// Update all primary and sub-model properties and constraints.
        /// These will be represented by the config type JSON models associated
        /// with the selected device (e.g., windTurbine).
        /// 
        /// TODO: Still some work remains here - for instance, windTurbine has an
        /// 'airSpeed' property and associated constraints that needs to render.
        /// </summary>
        private void UpdateModelPropsAndConstraints()
        {
            // first clear existing stored constraints
            this.ClearDigitalTwinConstraints();

            // now rebuild the constraints based on current selections
            float propsYPosDelta = 0.0f;

            ConfigTypeModelConstraints primaryConstraints =
                this.digitalTwinModelState.GetPrimaryModelConstraints();

            this.RenderDigitalTwinConstraints(
                primaryConstraints, propsYPosDelta,
                this.primaryPropsContainerObject, this.primaryConstraintGuiTemplate);

            ConfigTypeModelContainer configTypeContainer =
                EventProcessor.GetInstance().GetConfigTypeModelManager().GetConfigCategoryByModelName(
                    this.digitalTwinModelState.GetModelID());

            Debug.Log($"Checking on config type info for type name: {this.configTypeName}");

            if (!string.IsNullOrEmpty(this.configTypeName))
            {
                ConfigTypeModelEntry configTypeEntry = configTypeContainer.GetConfigType(this.configTypeName);

                if (configTypeEntry != null)
                {
                    ConfigTypeModelConstraints selectedConstraints = configTypeEntry.GetModelConstraints();

                    this.RenderDigitalTwinConstraints(
                        selectedConstraints, propsYPosDelta,
                        this.constraintContainerObject, this.constraintGuiTemplate);
                }
            } else {
                Debug.LogWarning($"No config type info available for type name: {this.configTypeName}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearDigitalTwinConstraints()
        {
            // clear the controller list first
            this.propertyUpdateHandlerList.Clear();

            // now destroy all the props objects
            foreach (GameObject go in this.digitalTwinGuiConstraintsList)
            {
                go.SetActive(false);

                Destroy(go);
            }

            this.digitalTwinGuiConstraintsList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraints"></param>
        /// <param name="yPosDelta"></param>
        private void RenderDigitalTwinConstraints(
            ConfigTypeModelConstraints constraints, float yPosDelta, GameObject itemContainer, GameObject itemTemplate)
        {
            if (constraints != null)
            {
                Dictionary<string, string> constraintTable = constraints.GetAllDataAsTable();

                foreach (string key in constraintTable.Keys)
                {
                    this.CreateDigitalTwinConstraintProperty(key, constraintTable[key], yPosDelta, itemContainer, itemTemplate);
                    yPosDelta += this.verticalAnchorDelta;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="yPosDelta"></param>
        private void CreateDigitalTwinConstraintProperty(
            string name, string valStr, float yPosDelta, GameObject itemContainer, GameObject itemTemplate)
        {
            if (this.constraintGuiTemplate != null)
            {
                GameObject go = Instantiate(itemTemplate, itemContainer.transform);

                if (go != null)
                {
                    // set the constraint label
                    try
                    {
                        Transform pt = go.transform.Find("PropsLabel");
                        TMP_Text guiLabel = pt.GetComponent<TextMeshProUGUI>();
                        guiLabel.text = name;

                        Transform vt = go.transform.Find("PropsValue");
                        InputField valEntry = vt.GetComponent<InputField>();
                        valEntry.text = valStr;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Can't find constraints label for GameObject {go}");
                    }

                    // adjust location

                    RectTransform propPosObj = go.GetComponent<RectTransform>();
                    float xPos = propPosObj.anchoredPosition.x - 50.0f;

                    if (itemContainer != null)
                    {
                        RectTransform parentPosObj = itemContainer.GetComponent<RectTransform>();
                        xPos = parentPosObj.anchoredPosition.x - 5.0f;
                    }

                    Vector2 anchorPos = new Vector2(xPos, (propPosObj.anchoredPosition.y - yPosDelta));
                    propPosObj.anchoredPosition = anchorPos;

                    // activate component
                    go.SetActive(true);

                    Debug.Log($"Created GUI for constraint. Loc: {anchorPos}. Name: {name}");

                    // add to internal list
                    this.digitalTwinGuiConstraintsList.Add(go);
                }
                else
                {
                    Debug.LogWarning($"GUI GameObject for constraint is null: {name}");
                }
            }
        }

    }

}
