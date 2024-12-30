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
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using LabBenchStudios.Pdt.Common;
using LabBenchStudios.Pdt.Data;
using LabBenchStudios.Pdt.Util;

namespace LabBenchStudios.Pdt.Unity.Common
{
    public class DataHistorianPlayerCoroutineProcessor : MonoBehaviour
    {
        private IDataHistorianPlayer dataHistorianPlayer = null;
        private Text curIndexLabel = null;

        private Coroutine playbackCoroutine = null;
        private WaitForSeconds pausedWaitCycles = null;

        private bool doProcessing = false;
        private bool isPaused = false;
        private bool enableDebugging = false;

        // 1 second default delay time
        private double defaultWaitTime = 1.0f;


        // public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="curIndexLabel"></param>
        /// <param name="enableDebugging"></param>
        /// <returns></returns>
        public bool InitHandler(IDataHistorianPlayer player, Text curIndexLabel, bool enableDebugging)
        {
            Debug.Log("Initializing co-routine handler for data historian player.");

            this.enableDebugging = enableDebugging;
            this.pausedWaitCycles = new WaitForSeconds(2.0f);
            this.curIndexLabel = curIndexLabel;

            if (player != null)
            {
                this.dataHistorianPlayer = player;
                
                // since this player will be used in this processor, be sure to
                // set internal player capabilities according - that is...
                //  - disable cache filling
                //  - disable threaded playback
                //  - enable playback (via call to public trigger method
                this.dataHistorianPlayer.SetCacheFillingEnabledFlag(false);
                this.dataHistorianPlayer.SetEnableThreadedPlaybackFlag(false);
                this.dataHistorianPlayer.SetPlaybackEnabledFlag(true);
            }

            return this.IsHandlerInitialized();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsHandlerInitialized()
        {
            return (dataHistorianPlayer != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsProcessingPaused()
        {
            return this.isPaused;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsProcessingActive()
        {
            return this.doProcessing;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PausePlayback()
        {
            if (this.IsHandlerInitialized() && this.playbackCoroutine != null)
            {
                Debug.Log($"Data historian player playback pausing: {this.dataHistorianPlayer.GetCacheName()}");

                this.isPaused = true;
            } else
            {
                Debug.Log("Data historian player is not initialized for co-routine usage. Ignoring pause.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnpausePlayback()
        {
            if (this.IsHandlerInitialized() && this.playbackCoroutine != null)
            {
                Debug.Log($"Data historian player playback un-pausing: {this.dataHistorianPlayer.GetCacheName()}");

                this.isPaused = false;
            } else
            {
                Debug.Log("Data historian player is not initialized for co-routine usage. Ignoring pause.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartPlayback()
        {
            if (this.IsHandlerInitialized())
            {
                if (this.doProcessing)
                {
                    Debug.Log("Data historian player already processing. Ignoring start.");
                } else
                {
                    Debug.Log($"Data historian player playback starting: {this.dataHistorianPlayer.GetCacheName()}");

                    this.isPaused = false;

                    this.dataHistorianPlayer.Play();

                    this.doProcessing = true;
                    this.playbackCoroutine = StartCoroutine(ProcessDataCache());
                }
            } else
            {
                Debug.Log("Data historian player is not initialized for co-routine usage. Ignoring start.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopPlayback()
        {
            if (this.IsHandlerInitialized() && this.playbackCoroutine != null)
            {
                Debug.Log($"Data historian player playback stopping: {this.dataHistorianPlayer.GetCacheName()}");

                this.dataHistorianPlayer.Stop();

                StopCoroutine(this.playbackCoroutine);

                this.doProcessing = false;
                this.playbackCoroutine = null;
            } else
            {
                Debug.Log("Data historian player is not initialized for co-routine usage. Ignoring stop.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator ProcessDataCache()
        {
            while (this.doProcessing)
            {
                if (!this.isPaused)
                {
                    if (this.enableDebugging)
                    {
                        Debug.Log($"Processing next playback event for historian: {this.dataHistorianPlayer.GetCacheName()}");
                    }

                    // trigger call returns milliseconds
                    // convert to seconds - divide by 1000
                    double delayTimeSecs = this.dataHistorianPlayer.TriggerNextCachePlaybackEvent() / 1000;

                    if (delayTimeSecs <= 0.0d)
                    {
                        delayTimeSecs = this.defaultWaitTime;
                    }

                    delayTimeSecs = Math.Round(delayTimeSecs, NumberUtil.DELAY_DEC_ROUNDING);

                    DataCacheEntryContainer eventContainer = this.dataHistorianPlayer.GetLastProcessedEventData();

                    if (this.curIndexLabel != null)
                    {
                        this.curIndexLabel.text = this.dataHistorianPlayer.GetLastProcessedEventIndex().ToString();
                    }

                    if (this.enableDebugging)
                    {
                        Debug.Log($"Processed playback event - waiting {delayTimeSecs} secs. Timestamp: {eventContainer.GetTimeStamp()}");
                        Debug.Log($"Data:\n{this.dataHistorianPlayer.GetLastProcessedEventData()}");
                    }

                    yield return new WaitForSeconds((float) delayTimeSecs);

                    if (this.enableDebugging)
                    {
                        Debug.Log($"Processed playback event for historian: {this.dataHistorianPlayer.GetCacheName()}. Delay secs: {delayTimeSecs}");
                    }
                } else
                {
                    yield return this.pausedWaitCycles;
                }
            }
        }

    }

}
