/**
 * MIT License
 * 
 * Copyright (c) 2024 Andrew D. King
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
using System.IO;

using UnityEngine;

namespace LabBenchStudios.Pdt.Unity.Common
{
    public static class DigitalTwinUtil
    {
        //////////
        // 
        // Consts and utility (static) methods for values / paths / etc.
        //
        //
        public static readonly string RELATIVE_PDT_PLUGIN_PATH =
            "/../Packages/LabBenchStudios PDT Plugin for Unity";

        public static readonly string RELATIVE_MODELS_PATH = "/Models";

        public static readonly string RELATIVE_DTDL_MODELS_PATH = RELATIVE_MODELS_PATH + "/Dtdl";

        public static readonly string RELATIVE_DTDL_MQTT_MODELS_PATH =
            DigitalTwinUtil.RELATIVE_DTDL_MODELS_PATH + "/Mqtt";

        public static readonly string RELATIVE_TYPE_CONFIG_MODELS_PATH = RELATIVE_MODELS_PATH + "/Types";

        public static readonly string RELATIVE_DATA_PATH = "/Data";

        public static readonly string RELATIVE_STATE_DATA_PATH = "/State";

        public static readonly string STATE_DATA_EXT = ".dat";

        /// <summary>
        /// Always returns a non-null path for the requisite directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns>The absolute path as a string</returns>
        public static string GetProjectDtdlModelsPath(string projectName)
        {
            string path =
                Application.dataPath + projectName + DigitalTwinUtil.RELATIVE_DTDL_MODELS_PATH;

            return InitModelsPath(path, "DTDL Model");
        }

        /// <summary>
        /// Always returns a non-null path for the requisite directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns>The absolute path as a string</returns>
        public static string GetProjectTypeConfigModelsPath(string projectName)
        {
            string path =
                Application.dataPath + projectName + DigitalTwinUtil.RELATIVE_TYPE_CONFIG_MODELS_PATH;

            return InitModelsPath(path, "DTDL Model");
        }

        /// <summary>
        /// Always returns a non-null path for the requisite directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <returns>The absolute path as a string</returns>
        public static string GetDtdlModelsPath()
        {
            string path =
                Application.dataPath +
                DigitalTwinUtil.RELATIVE_PDT_PLUGIN_PATH + DigitalTwinUtil.RELATIVE_DTDL_MODELS_PATH;

            return InitModelsPath(path, "DTDL Model");
        }

        /// <summary>
        /// Always returns a non-null path for the requisite directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <returns>The absolute path as a string</returns>
        public static string GetTypeConfigModelsPath()
        {
            string path =
                Application.dataPath +
                DigitalTwinUtil.RELATIVE_PDT_PLUGIN_PATH + DigitalTwinUtil.RELATIVE_TYPE_CONFIG_MODELS_PATH;

            return InitModelsPath(path, "Type Config Mapping Model")
        }

        /// <summary>
        /// Always returns a non-null path for the requisite directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <returns>The absolute path as a string</returns>
        public static string GetStateDataPath()
        {
            string path =
                Application.dataPath +
                DigitalTwinUtil.RELATIVE_PDT_PLUGIN_PATH + DigitalTwinUtil.RELATIVE_STATE_DATA_PATH;

            return InitModelsPath(path, "State Info");
        }

        /// <summary>
        /// Always returns a non-null path for the specified 'path' directory.
        /// If the path has not yet been created, this method - on initial
        /// invocation - will attempt to create it, and log the appropriate
        /// error to the Unity console on success or failure.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loggingName"></param>
        /// <returns></returns>
        public static string InitModelsPath(string path, string loggingName)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);

                if (di != null)
                {
                    Debug.Log($"Created {loggingName} path {di.FullName} at {di.CreationTime}");
                } else
                {
                    Debug.LogError($"Failed to create {loggingName} path {path}. {loggingName} data will not be accessible.");
                }
            } else
            {
                Debug.Log($"{loggingName} path exists as expected: {path}. {loggingName} data should be accessible.");
            }

            return path;
        }

    }

}
