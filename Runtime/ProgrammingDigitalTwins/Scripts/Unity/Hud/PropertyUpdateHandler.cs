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
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using LabBenchStudios.Pdt.Model;
using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

/**
 * Controller for managing impeller turbine rotational speed.
 * 
 */
namespace LabBenchStudios.Pdt.Unity.Controller
{
    public class PropertyUpdateHandler : MonoBehaviour, IPropertyManagementController
    {
        [SerializeField]
        private bool enableLogging = true;

        [SerializeField]
        private GameObject propertyLabelObject = null;

        [SerializeField]
        private GameObject propertyValueObject = null;

        [SerializeField]
        private GameObject propertyToggleObject = null;

        [SerializeField]
        private GameObject propertyMessageObject = null;

        [SerializeField]
        private GameObject targetValueObject = null;

        private TMP_Text propertyLabelText = null;
        private TMP_Text targetValueText = null;

        private Text propertyMessageText = null;
        private Text propertyValueText = null;
        private Toggle propertyToggle = null;

        private float curValue = float.MinValue;
        private int curCommand = 0;
        private bool isSelected = false;
        private string curMsgState = ConfigConst.NOT_SET;

        private int prevHash = 0;
        private int curHash = 0;

        private IotDataContext dataContext = null;

        private string propName = ConfigConst.NOT_SET;
        private string deviceID = ConfigConst.NOT_SET;
        private string locationID = ConfigConst.NOT_SET;
        private int typeCategoryID = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
        private int typeID = ConfigConst.DEFAULT_TYPE_ID;

        private bool enableDebugLogging = false;

        private DigitalTwinProperty digitalTwinProperty;

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            Debug.unityLogger.logEnabled = this.enableLogging;

            if (this.propertyLabelObject != null)
            {
                this.propertyLabelText = this.propertyLabelObject.GetComponent<TextMeshProUGUI>();
            }

            if (this.propertyMessageObject != null)
            {
                this.propertyMessageText = this.propertyMessageObject.GetComponent<Text>();
            }

            if (this.propertyToggleObject != null)
            {
                this.propertyToggle = this.propertyToggleObject.GetComponent<Toggle>();
            }

            if (this.propertyValueObject != null)
            {
                this.propertyValueText = this.propertyValueObject.GetComponent<Text>();
            }

            if (this.targetValueObject != null)
            {
                this.targetValueText = this.targetValueObject.GetComponent<TextMeshProUGUI>();
            }    

            this.UpdateLocalProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLabel()
        {
            return this.propName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            return this.curMsgState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsChanged()
        {
            this.UpdateLocalProperties();

            return (this.curHash != this.prevHash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsCommand()
        {
            if (this.digitalTwinProperty != null)
            {
                return this.digitalTwinProperty.IsCommand();
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSelected()
        {
            return this.isSelected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCommand()
        {
            return this.curCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetValue()
        {
            return this.curValue;
        }

        /// <summary>
        /// This call will reset the internal state once the ActuatorData
        /// is generated. If there's no change detected between the previously
        /// stored value and the current one, it will return null.
        /// </summary>
        /// <returns></returns>
        public ActuatorData GenerateCommand()
        {
            if (this.IsChanged())
            {
                ActuatorData data = new ActuatorData();

                // apply device and type-specific properties
                data.UpdateData(this.dataContext);

                // apply curCommand-specific properties
                data.SetCommand(this.curCommand);
                data.SetCommandName(this.propName);
                data.SetValue(this.curValue);
                data.SetStateData(this.curMsgState);

                Debug.Log($"Generated Outgoing Command: {data}");

                this.ResetPropertySettingsHash();

                return data;
            }
            else
            {
                Debug.Log($"State not changed. No curCommand needed: {this.propName}");
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DigitalTwinProperty GetDigitalTwinProperty()
        {
            this.UpdateLocalProperties();

            return this.digitalTwinProperty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetPropertyJson()
        {
            Debug.LogWarning("Not yet implemented.");

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digitalTwinProperty"></param>
        public void InitPropertyController(DigitalTwinModelState modelState, DigitalTwinProperty digitalTwinProperty)
        {
            if (digitalTwinProperty != null)
            {
                this.digitalTwinProperty = digitalTwinProperty;
            }

            if (modelState != null)
            {
                this.propName = this.digitalTwinProperty.GetPropertyName();
                this.deviceID = modelState.GetDeviceID();
                this.locationID = modelState.GetLocationID();
                this.typeCategoryID = modelState.GetTypeCategoryID();
                this.typeID = modelState.GetTypeID();

                if (this.enableDebugLogging)
                {
                    Debug.Log(
                        "Props state:" +
                        $"\n\tname:          {this.propName}" +
                        $"\n\tDeviceID:      {this.deviceID}" +
                        $"\n\tlocationID:    {this.locationID}" +
                        $"\n\typeCategoryID: {this.typeCategoryID}" +
                        $"\n\typeID:         {this.typeID}");
                }

                this.dataContext =
                    new IotDataContext(this.deviceID, this.deviceID, this.typeCategoryID, this.typeID);

                this.dataContext.SetLocationID(this.locationID);
            }

            this.UpdateLocalProperties();
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        public void ValidateCommandResponse(ActuatorData data)
        {
            if (data != null)
            {
                // TODO: implement this
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetPropertySettingsHash()
        {
            this.UpdateCurrentPropertiesHash();

            this.prevHash = this.curHash;
        }

        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void UpdateLocalProperties()
        {
            if (this.propertyMessageText != null)
            {
                this.curMsgState = this.propertyMessageText.text;

                if (string.IsNullOrEmpty(this.curMsgState))
                {
                    this.curMsgState = ConfigConst.NOT_SET;
                }
            }

            if (this.propertyToggle != null)
            {
                Debug.Log($"Checking property toggle set for this command: {this.digitalTwinProperty.GetPropertyName()}");
                if (this.propertyToggle.isOn)
                {
                    this.curCommand = ConfigConst.COMMAND_ON;
                }
                else
                {
                    this.curCommand = ConfigConst.COMMAND_OFF;
                }
            }

            if (this.propertyValueText != null && ! string.IsNullOrEmpty(this.propertyValueText.text))
            {
                string valueStr = this.propertyValueText.text.Trim();

                try
                {
                    this.curValue = float.Parse(valueStr);

                    Debug.Log($"Updated current value: {this.propertyValueText.text} -> {this.curValue}");

                    if (this.targetValueText != null)
                    {
                        this.targetValueText.text = this.curValue.ToString();
                    }
                }
                catch (Exception e)
                {
                    // it's likely this will be caught often - no need to log a message
                    //Debug.LogError($"Can't parse curValue entry - not a float: --{valueStr}--");
                }
            }

            this.UpdateCurrentPropertiesHash();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCurrentPropertiesHash()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.propName).Append('-');
            sb.Append(this.curValue).Append('-');
            sb.Append(this.curCommand).Append('-');
            sb.Append(this.curMsgState);

            string hashStr = sb.ToString();
            this.curHash = hashStr.GetHashCode();

            Debug.Log($"Properties hash: {hashStr} = {this.curHash} ; previous hash = {this.prevHash}");
        }

    }

}
