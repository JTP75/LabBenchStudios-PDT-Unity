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

using TMPro;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Model;
using LabBenchStudios.Pdt.Plexus;
using UnityEditor.Search;

namespace LabBenchStudios.Pdt.Unity.Common
{
    public abstract class BaseAsyncDataMessageProcessor : MonoBehaviour, IDataContextEventListener, IPredictionModelListener
    {
        [SerializeField]
        private Boolean registerForDataCallbacks = true;

        [SerializeField]
        private Boolean enableDebugLogProcessing = false;

        [SerializeField]
        private Boolean enableWarningLogProcessing = false;

        [SerializeField]
        private Boolean enableErrorLogProcessing = false;

        [SerializeField]
        private Boolean enableMessageDataProcessing = false;

        [SerializeField]
        private Boolean enableActuatorDataProcessing = false;

        [SerializeField]
        private Boolean enableConnectionStateDataProcessing = false;

        [SerializeField]
        private Boolean enableSensorDataProcessing = false;

        [SerializeField]
        private Boolean enableSystemPerformanceDataProcessing = false;

        [SerializeField]
        private Boolean enablePredictionProcessing = true;

        [SerializeField]
        private Boolean enableTimeAndDateUpdates = true;

        [SerializeField]
        private GameObject timeDisplay = null;

        [SerializeField]
        private GameObject dateDisplay = null;

        private bool isInitialized = false;

        private EventProcessor eventProcessor = null;

        private TMP_Text timeLog = null;
        private TMP_Text dateLog = null;

        // queues for handling logging from framework
        private Queue<string> debugLogQueue = null;
        private Queue<string> warningLogQueue = null;
        private Queue<string> errorLogQueue = null;

        // queues for handling prediction engine results
        private Queue<ModelListContainer> predictionEngineModelListQueue = null;
        private Queue<QueryResponseContainer> predictionEngineResponseQueue = null;

        // queues for handling incoming IotDataContext messages
        private Queue<MessageData> msgDataQueue = null;
        private Queue<ActuatorData> actuatorDataQueue = null;
        private Queue<ConnectionStateData> connStateDataQueue = null;
        private Queue<SensorData> sensorDataQueue = null;
        private Queue<SystemPerformanceData> sysPerfDataQueue = null;

        void Start()
        {
            // first
            this.InitQueues();

            // second
            this.InitTimeDisplay();

            // third
            this.InitEventProcessing();

            // fourth
            this.InitMessageHandler();

            // final
            this.isInitialized = true;

            // update time and date every second
            if (this.enableTimeAndDateUpdates)
            {
                InvokeRepeating(nameof(UpdateTimeAndDate), 1.0f, 1.0f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (this.enableDebugLogProcessing)
            {
                this.ProcessDebugLogMessage(this.GetDebugLogMessageFromQueue());
            }

            if (this.enableWarningLogProcessing)
            {
                this.ProcessWarningLogMessage(this.GetWarningLogMessageFromQueue());
            }

            if (this.enableErrorLogProcessing)
            {
                this.ProcessErrorLogMessage(this.GetErrorLogMessageFromQueue());
            }

            if (this.enableActuatorDataProcessing)
            {
                ActuatorData data = this.GetActuatorDataFromQueue();

                if (data != null) { this.ProcessActuatorData(data); }
            }

            if (this.enableConnectionStateDataProcessing)
            {
                ConnectionStateData data = this.GetConnectionStateDataFromQueue();

                if (data != null) { this.ProcessConnectionStateData(data); }
            }

            if (this.enableMessageDataProcessing)
            {
                MessageData data = this.GetMessageDataFromQueue();

                if (data != null) { this.ProcessMessageData(data); }
            }

            if (this.enableSensorDataProcessing)
            {
                SensorData data = this.GetSensorDataFromQueue();

                if (data != null) { this.ProcessSensorData(data); }
            }

            if (this.enableSystemPerformanceDataProcessing)
            {
                SystemPerformanceData data = this.GetSystemPerformanceDataFromQueue();

                if (data != null) { this.ProcessSystemPerformanceData(data); }
            }

            if (this.enablePredictionProcessing)
            {
                QueryResponseContainer queryData = this.GetQueryResponseDataFromQueue();

                if (queryData != null) { this.ProcessQueryResponseUpdate(queryData); }

                ModelListContainer modelData = this.GetModelListDataFromQueue();

                if (modelData != null) { this.ProcessModelListUpdate(modelData); }
            }

        }

        // public callback methods

        public void HandleDebugLogMessage(string message)
        {
            if (this.enableDebugLogProcessing && message != null)
            {
                this.debugLogQueue.Enqueue(message);
            }
        }

        public void HandleWarningLogMessage(string message)
        {
            if (this.enableWarningLogProcessing && message != null)
            {
                this.warningLogQueue.Enqueue(message);
            }
        }

        public void HandleErrorLogMessage(string message)
        {
            this.HandleErrorLogMessage(message, null);
        }

        public void HandleErrorLogMessage(string message, Exception e)
        {
            if (this.enableErrorLogProcessing && message != null)
            {
                if (e != null)
                {
                    StringBuilder sb = new();
                    sb.Append(e.Message);
                    sb.Append($"\nException:\n{e}");

                    message = sb.ToString();
                }

                this.errorLogQueue.Enqueue(message);
            }
        }

        public void HandleActuatorData(ActuatorData data)
        {
            if (this.enableActuatorDataProcessing && data != null)
            {
                this.actuatorDataQueue.Enqueue(data);
            }
        }

        public void HandleConnectionStateData(ConnectionStateData data)
        {
            if (this.enableConnectionStateDataProcessing && data != null)
            {
                this.connStateDataQueue.Enqueue(data);
            }
        }

        public void HandleMessageData(MessageData data)
        {
            if (this.enableMessageDataProcessing && data != null)
            {
                this.msgDataQueue.Enqueue(data);
            }
        }

        public void HandleSensorData(SensorData data)
        {
            if (this.enableSensorDataProcessing && data != null)
            {
                this.sensorDataQueue.Enqueue(data);
            }
        }

        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            if (this.enableSystemPerformanceDataProcessing && data != null)
            {
                this.sysPerfDataQueue.Enqueue(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelListContainer"></param>
        public void OnModelListRetrieved(ModelListContainer modelListContainer)
        {
            if (modelListContainer != null)
            {
                Debug.Log($"AI models received: {modelListContainer.GetUri()} - {modelListContainer.GetModelList().Count}");

                this.predictionEngineModelListQueue.Enqueue(modelListContainer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResponseContainer"></param>
        public void OnQueryResponseReceived(QueryResponseContainer queryResponseContainer)
        {
            if (queryResponseContainer != null)
            {
                Debug.Log($"AI response received: {queryResponseContainer.GetSessionID()} - {queryResponseContainer.GetUri()}:\n{queryResponseContainer.GetResponse()}");

                this.predictionEngineResponseQueue.Enqueue(queryResponseContainer);
            }
        }


        // protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primaryStateProcessor"></param>
        /// <param name="otherStateProcessor"></param>
        /// <param name="controllerID"></param>
        /// <returns></returns>
        protected IDigitalTwinStateProcessor CreateOrUpdateDigitalTwinStateProcessor(
            IDigitalTwinStateProcessor primaryStateProcessor,
            IDigitalTwinStateProcessor otherStateProcessor,
            ModelNameUtil.DtmiControllerEnum controllerID)
        {
            DigitalTwinModelManager dtModelManager =
                EventProcessor.GetInstance().GetDigitalTwinModelManager();

            if (otherStateProcessor == null)
            {
                otherStateProcessor =
                    dtModelManager.CreateModelState(
                        primaryStateProcessor,
                        false, // no GUID
                        true,  // add to parent
                        controllerID,
                        (IDataContextEventListener)this);
            }
            else
            {
                otherStateProcessor.UpdateConnectionState(primaryStateProcessor);

                if (otherStateProcessor is DigitalTwinModelState)
                {
                    dtModelManager.UpdateModelState((DigitalTwinModelState)otherStateProcessor);
                }
            }

            return otherStateProcessor;
        }

        protected void ProcessDebugLogMessage(string message)
        {
            if (message != null) Debug.Log(message);
        }

        protected void ProcessWarningLogMessage(string message)
        {
            if (message != null) Debug.LogWarning(message);
        }

        protected void ProcessErrorLogMessage(string message)
        {
            if (message != null) Debug.LogError(message);
        }

        protected void RegisterForSystemStatusEvents(ISystemStatusEventListener listener)
        {
            if (listener != null)
            {
                Debug.Log("Registering for system status events...");
                this.eventProcessor.RegisterListener(listener);
            }
        }

        protected void RegisterForUserStatusEvents(IUserEventStateListener listener)
        {
            if (listener != null)
            {
                Debug.Log("Registering for user status events...");
                this.eventProcessor.RegisterListener(listener);
            }
        }

        // protected template methods

        protected abstract void InitMessageHandler();

        protected abstract void ProcessActuatorData(ActuatorData data);

        protected abstract void ProcessConnectionStateData(ConnectionStateData data);

        protected abstract void ProcessMessageData(MessageData data);

        protected abstract void ProcessSensorData(SensorData data);

        protected abstract void ProcessSystemPerformanceData(SystemPerformanceData data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelListContainer"></param>
        protected abstract void ProcessModelListUpdate(ModelListContainer modelListContainer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResponseContainer"></param>
        protected abstract void ProcessQueryResponseUpdate(QueryResponseContainer queryResponseContainer);


        // private methods

        /// <summary>
        /// 
        /// </summary>
        private void InitQueues()
        {
            this.debugLogQueue = new Queue<string>();
            this.warningLogQueue = new Queue<string>();
            this.errorLogQueue = new Queue<string>();

            this.predictionEngineModelListQueue = new Queue<ModelListContainer>();
            this.predictionEngineResponseQueue = new Queue<QueryResponseContainer>();

            this.msgDataQueue = new Queue<MessageData>();
            this.actuatorDataQueue = new Queue<ActuatorData>();
            this.connStateDataQueue = new Queue<ConnectionStateData>();
            this.sensorDataQueue = new Queue<SensorData>();
            this.sysPerfDataQueue = new Queue<SystemPerformanceData>();
        }

        private void InitTimeDisplay()
        {
            if (this.timeDisplay != null)
            {
                this.timeLog = this.timeDisplay.GetComponent<TextMeshProUGUI>();
            }

            if (this.dateDisplay != null)
            {
                this.dateLog = this.dateDisplay.GetComponent<TextMeshProUGUI>();
            }
        }

        private void InitEventProcessing()
        {
            this.eventProcessor = EventProcessor.GetInstance();

            if (this.registerForDataCallbacks)
            {
                this.eventProcessor.RegisterListener((IDataContextEventListener) this);
            }
        }

        private string GetDebugLogMessageFromQueue()
        {
            string msg = null;

            if (this.debugLogQueue.Count > 0)
            {
                msg = this.debugLogQueue.Dequeue();
            }

            return msg;
        }

        private string GetWarningLogMessageFromQueue()
        {
            string msg = null;

            if (this.warningLogQueue.Count > 0)
            {
                msg = this.warningLogQueue.Dequeue();
            }

            return msg;
        }

        private string GetErrorLogMessageFromQueue()
        {
            string msg = null;

            if (this.errorLogQueue.Count > 0)
            {
                msg = this.errorLogQueue.Dequeue();
            }

            return msg;
        }

        private ActuatorData GetActuatorDataFromQueue()
        {
            ActuatorData data = null;

            if (this.actuatorDataQueue.Count > 0)
            {
                data = this.actuatorDataQueue.Dequeue();
            }

            return data;
        }

        private ConnectionStateData GetConnectionStateDataFromQueue()
        {
            ConnectionStateData data = null;

            if (this.connStateDataQueue.Count > 0)
            {
                data = this.connStateDataQueue.Dequeue();
            }

            return data;
        }

        private MessageData GetMessageDataFromQueue()
        {
            MessageData data = null;

            if (this.msgDataQueue.Count > 0)
            {
                data = this.msgDataQueue.Dequeue();
            }

            return data;
        }

        private SensorData GetSensorDataFromQueue()
        {
            SensorData data = null;

            if (this.sensorDataQueue.Count > 0)
            {
                data = this.sensorDataQueue.Dequeue();
            }

            return data;
        }

        private SystemPerformanceData GetSystemPerformanceDataFromQueue()
        {
            SystemPerformanceData data = null;

            if (this.sysPerfDataQueue.Count > 0)
            {
                data = this.sysPerfDataQueue.Dequeue();
            }

            return data;
        }

        private QueryResponseContainer GetQueryResponseDataFromQueue()
        {
            QueryResponseContainer queryResponseData = null;

            if (this.predictionEngineResponseQueue.Count > 0)
            {
                queryResponseData = this.predictionEngineResponseQueue.Dequeue();
            }

            return queryResponseData;
        }

        private ModelListContainer GetModelListDataFromQueue()
        {
            ModelListContainer modelListContainer= null;

            if (this.predictionEngineModelListQueue.Count > 0)
            {
                modelListContainer = this.predictionEngineModelListQueue.Dequeue();
            }

            return modelListContainer;
        }

        private void UpdateTimeAndDate()
        {
            DateTime dateTime = DateTime.Now;

            if (this.timeLog != null)
            {
                this.timeLog.text = dateTime.ToString("t");// "h:MM:ss tt");
            }

            if (this.dateLog != null)
            {
                this.dateLog.text = dateTime.ToString("ddd, MMM dd yyyy");
            }
        }

    }
}
