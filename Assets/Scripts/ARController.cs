//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif




    //-----------------------------------------------------------------------------------------------
	// This file is a slightly altered version from the example HelloAR app. The script is altered
    // mostly between lines 158 and 168. So that it places custom markers instead of example markers
	//-----------------------------------------------------------------------------------------------



    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class ARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a vertical plane.
        /// </summary>
        public GameObject GameObjectVerticalPlanePrefab;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a horizontal plane.
        /// </summary>
        public GameObject GameObjectHorizontalPlanePrefab;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject GameObjectPointPrefab;

        /// <summary>
        /// The rotation in degrees need to apply to prefab when it is placed.
        /// </summary>
        private const float k_PrefabRotation = 180.0f;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error,
        /// otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        /// <summary>
        /// Whether the save anchor has been detected
        /// </summary>
        private bool save_anchor_is_detected = false;

        private Anchor SingleAnchor = null;
        private Student_Manager StudMan;

        private GlobalVars _GlobalVars;

        /// <summary>
        /// List of markers that will be scanned for
        /// </summary>
        private List<AugmentedImage> m_markers = new List<AugmentedImage>();


        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Enable ARCore to target 60fps camera capture frame rate on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;
            StudMan = GameObject.Find("Student_Manager").GetComponent<Student_Manager>();
            _GlobalVars = GameObject.Find("System_Scripts").transform.Find("GlobalVars").GetComponent<GlobalVars>();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();

            //_UpdateFindSaveAnchor();

            // If the player has not touched the screen, we are done with this update.
            Touch touch;

            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }
            //print("touch!");
            // Should not handle input if the player is pointing on UI.
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                //print("UI touched!");
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            bool valid = false;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                //print("hit found");
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    //Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    // Choose the prefab based on the Trackable that got hit.
                    if (hit.Trackable is FeaturePoint)
                    {
                        valid = false;
                    }
                    else if (hit.Trackable is DetectedPlane)
                    {
                        DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                        if (detectedPlane.PlaneType == DetectedPlaneType.Vertical)
                        {
                            valid = false;
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                    else
                    {
                        valid = false;
                    }

                    if (valid){
                        print("object placed");
                        // Make game object a child of the anchor.
                        if(SingleAnchor == null){
                            SingleAnchor = hit.Trackable.CreateAnchor(hit.Pose);
						}
                        
                        // Instantiate prefab at the hit pose.    
                        Vector3 airpos = new Vector3(hit.Pose.position.x,hit.Pose.position.y+1,hit.Pose.position.z);
                        StudMan.SetLocationInstance(airpos, SingleAnchor);
                    
					}
                }
            }
        }

        /// <summary>
        /// Try to find the anchor to which the students are relatively saved.
        /// </summary>
        private void _UpdateFindSaveAnchor()
        {
            if (save_anchor_is_detected) return;
            Session.GetTrackables<AugmentedImage>(
                m_markers, TrackableQueryFilter.Updated);
            //print(m_markers.Count.ToString() + " Images that will be detected");
            // Create anchor for updated augmented images that are tracking and do
            // not have an anchor. Remove visualizers for stopped images.
            foreach (var image in m_markers)
            {
                
                if (image.TrackingState == TrackingState.Tracking)
                {
                    // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                    if (_GlobalVars.saveAnchor == null)
                    _GlobalVars.saveAnchor = image.CreateAnchor(image.CenterPose);
                    save_anchor_is_detected = true;
                    
                    
                    //In future: implement option to save 
                    var saveAnchorButton = GameObject.FindWithTag("saveAnchorButton");
                    if (saveAnchorButton != null) saveAnchorButton.SetActive(true); // buggy way of testing saveanchors
                    

                    var debugMessage = "";

                    if (SingleAnchor == null)
                    {
                        var loadAnchorButton = GameObject.FindWithTag("loadAnchorButton");
                        if (loadAnchorButton != null) loadAnchorButton.SetActive(true);
                    } else
                    {
                        debugMessage = "Save Anchor has been detected";
                    }
                    _ShowAndroidToastMessage(debugMessage);
                    //visualizer = (AugmentedImageVisualizer)Instantiate(
                    //    AugmentedImageVisualizerPrefab, anchor.transform);
                    //visualizer.Image = image;
                    //m_Visualizers.Add(image.DatabaseIndex, visualizer);
                }
                //else if (image.TrackingState == TrackingState.Stopped)
                //{
                //    m_Visualizers.Remove(image.DatabaseIndex);
                //    GameObject.Destroy(visualizer.gameObject);
                //}
            }
        }

        private void _LoadSaveAnchorBitmap()
        {
            _LoadSaveAnchorBitmap("test_save_anchor");
        }

        private void _LoadSaveAnchorBitmap(string assetPath)
        {

        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to
            // appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject =
                        toastClass.CallStatic<AndroidJavaObject>(
                            "makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }

}