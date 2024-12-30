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

using UnityEngine;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;

namespace LabBenchStudios.Pdt.Unity.Sample
{
    public class SampleThresholdCrossingAnimationHandler : MonoBehaviour, IDataContextEventListener
    {
        // consts set here for convenience
        public const float DEFAULT_MAX_PERCENTAGE = 100.0f;
        public const float DEFAULT_MIN_PERCENTAGE = 1.0f;

        public const float DEFAULT_MID_PERCENTAGE = 50.0f;
        public const float DEFAULT_LOW_PERCENTAGE = 20.0f;
        public const float DEFAULT_HIGH_PERCENTAGE = 80.0f;

        public const float PERCENTAGE_DIVISOR = 100.0f;

        //
        // set default ranges for high, mid and low
        //

        // set bounds for high threshold
        [SerializeField, Range(61.0f, DEFAULT_MAX_PERCENTAGE)]
        private float thresholdHigh = DEFAULT_HIGH_PERCENTAGE;

        // set bounds for low threshold
        [SerializeField, Range(DEFAULT_MIN_PERCENTAGE, 39.0f)]
        private float thresholdLow = DEFAULT_LOW_PERCENTAGE;

        // set bounds for nominal (mid) threshold
        [SerializeField, Range(40.0f, 60.0f)]
        private float nominalMid = DEFAULT_MID_PERCENTAGE;

        private Renderer attachedObjRenderer = null;
        private Gradient attachedObjGradient = null;

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            // get renderer and create gradient
            this.attachedObjRenderer = gameObject.GetComponent<Renderer>();
            this.attachedObjGradient = new Gradient();

            // use three gradients: red (highest val), green (mid val), blue (low val)
            // add as many as necessary to support the gradient variations needed
            GradientColorKey[] colorKey = new GradientColorKey[3];
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];

            // convert %'s to decimal (float)
            float high = DEFAULT_MAX_PERCENTAGE / PERCENTAGE_DIVISOR;
            float mid  = this.nominalMid / this.thresholdHigh;
            float low  = this.thresholdLow / this.thresholdHigh;

            if (mid >= 1.0f) mid = DEFAULT_MID_PERCENTAGE / PERCENTAGE_DIVISOR;
            if (low >= 1.0f) low = DEFAULT_LOW_PERCENTAGE / PERCENTAGE_DIVISOR;

            //
            // provision color keys
            //

            // as curValue approaches thresholdHigh, color becomes more red
            colorKey[0].color = Color.blue;
            colorKey[0].time = high;

            // as curValue hovers at thresholdMidHigh, color is white
            colorKey[1].color = Color.green;
            colorKey[1].time = mid;

            // as curValue approaches thresholdLow, color becomes more blue
            colorKey[2].color = Color.grey;
            colorKey[2].time = low;

            //
            // provision alpha keys
            //

            // as curValue moves from thresholdHigh to thresholdLow
            // alpha curValue renders color more translucent
            alphaKey[0].alpha = 0.85f;
            alphaKey[0].time = high;

            alphaKey[1].alpha = 0.55f;
            alphaKey[1].time = mid;

            alphaKey[2].alpha = 0.25f;
            alphaKey[2].time = low;

            this.attachedObjGradient.SetKeys(colorKey, alphaKey);

            // set default color to configured mid-point
            this.UpdateComponentColor(this.nominalMid);
        }

        // callback methods - only HandleSensorData() is used
        // others are required to implement the interface

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleActuatorData(ActuatorData data)
        {
            // ignore for now
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleConnectionStateData(ConnectionStateData data)
        {
            // ignore for now
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleMessageData(MessageData data)
        {
            // ignore for now
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSystemPerformanceData(SystemPerformanceData data)
        {
            // ignore for now
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void HandleSensorData(SensorData data)
        {
            if (data != null)
            {
                this.UpdateComponentColor(data.GetValue());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        private void UpdateComponentColor(float val)
        {
            if (this.attachedObjRenderer != null)
            {
                // scale curValue to something between 0.0f and 1.0f
                float scaledVal = (val > 0.0f ? val / this.thresholdHigh : val);

                this.attachedObjRenderer.material.color = this.attachedObjGradient.Evaluate(scaledVal);
            }
        }

    }

}
