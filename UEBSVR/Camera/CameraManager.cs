﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;



namespace UEBSVR
{
    public static class CameraManager
    {

        public static VRCameraMode CurrentCameraMode;
        public static float NearClipPlaneDistance = 0.01f;
        public static float FarClipPlaneDistance = 59999f;
        public static bool DisableParticles = false;

        public static Transform OriginalCameraParent = null;
        public static GameObject VROrigin;
        public static GameObject LeftHand = null;
        public static GameObject RightHand = null;

        public static bool RightHandGrab = false;
        public static bool LeftHandGrab = false;

        public static float InitialHandDistance = 0f;
        public static Vector3 ZoomOrigin = Vector3.zero;
        public static float SpeedScalingFactor = 1f;
        public static Vector3 PreviousVelocityPosition = Vector3.zero;

        static CameraManager()
        {

            VROrigin = new GameObject();
        }

        
        public static void AddSkyBox()
        {/*
            // ADD THE LOADED SKYBOX !!!!
            var SceneSkybox = GameObject.Instantiate(AssetLoader.Skybox, Vector3.zeroVector, Quaternion.identityQuaternion);
            SceneSkybox.transform.localScale = new Vector3(999999, 999999, 999999);
            SceneSkybox.transform.eulerAngles = new Vector3(270, 0, 0);
               */
        }

        public static void SwitchPOV()
        {/*
            Logs.WriteInfo("Entered SwitchPOV function");

            // ADD A SKYBOX

            Camera OriginalCamera = Game.GetCamera();
            // If we are not in firstperson
            if (CameraManager.CurrentCameraMode != CameraManager.VRCameraMode.FirstPerson)
            {
                Logs.WriteInfo("Got past cameramod check");
                if (Game.Instance.Player.MainCharacter != null)
                {
                    Logs.WriteInfo("Got past maincharacter exist check");
                    // switch to first person
                    VROrigin.transform.parent = null;
                    VROrigin.transform.position = Game.Instance.Player.MainCharacter.Value.GetPosition();

                    if (!OriginalCameraParent)
                    {
                        OriginalCameraParent = OriginalCamera.transform.parent;
                    }

                    OriginalCamera.transform.parent = VROrigin.transform;
                    if (RightHand)
                        RightHand.transform.parent = VROrigin.transform;
                    if (LeftHand)
                        LeftHand.transform.parent = VROrigin.transform;
                    CameraManager.CurrentCameraMode = CameraManager.VRCameraMode.FirstPerson;
                }

            }
            else
            {
                VROrigin.transform.position = OriginalCameraParent.position;
                VROrigin.transform.rotation = OriginalCameraParent.rotation;
                VROrigin.transform.localScale = OriginalCameraParent.localScale;

                VROrigin.transform.parent = OriginalCameraParent;

                CameraManager.CurrentCameraMode = CameraManager.VRCameraMode.DemeoLike;
            }
            */
        }

        public static void SpawnHands()
        {
            Logs.WriteInfo($"LLLL: SpawnHands");
            VROrigin.transform.position =  CameraPatches.DummyCamera.transform.position;
            VROrigin.transform.rotation = CameraPatches.DummyCamera.transform.rotation;
            VROrigin.transform.localScale = CameraPatches.DummyCamera.transform.localScale;
            VROrigin.transform.parent = CameraPatches.DummyCamera.transform; 
            Logs.WriteInfo($"LLLL:  SpawnHands 2");
            if (!RightHand)
            {
                RightHand = new GameObject();//.Instantiate(AssetLoader.RightHandBase, Vector3.zeroVector, Quaternion.identityQuaternion);
                RightHand.transform.parent = VROrigin.transform;
            }
            if (!LeftHand)
            {
                LeftHand = new GameObject();//.Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector, Quaternion.identityQuaternion);
                LeftHand.transform.parent = VROrigin.transform;
            }
            Logs.WriteInfo($"LLLL: Exit SpawnHands");
        }

        public static void HandleDemeoCamera()
        {
            
            if (RightHand && LeftHand)
            {
                GameObject TheCameraRig = Camera.main.transform.parent.gameObject;
                Camera CurrentCamera = Camera.main;
                if (!VROrigin.GetComponent<Rigidbody>())
                {
                    Rigidbody tempvar = VROrigin.AddComponent<Rigidbody>();
                    tempvar.useGravity = false;
                }

                Rigidbody VROriginPhys = VROrigin.GetComponent<Rigidbody>();
                if (RightHandGrab && LeftHandGrab)
                {
                    if (InitialHandDistance == 0f)
                    {
                        InitialHandDistance = Vector3.Distance(CameraManager.RightHand.transform.position, CameraManager.LeftHand.transform.position);
                        ZoomOrigin = VROrigin.transform.position;
                    }
                    float HandDistance = Vector3.Distance(CameraManager.RightHand.transform.position, CameraManager.LeftHand.transform.position);
                    float scale = HandDistance / InitialHandDistance;

                    VROrigin.transform.position = Vector3.LerpUnclamped(Vector3.zero, ZoomOrigin, scale);
                }
                else if (RightHandGrab || LeftHandGrab)
                {
                    InitialHandDistance = 0f;

                    SpeedScalingFactor = Mathf.Clamp(Math.Abs(Vector3.Distance(Vector3.zero, VROrigin.transform.position)), 1.0f, FarClipPlaneDistance);
                    if (RightHandGrab)
                    {
                        Vector3 ScaledSpeed = SteamVR_Actions._default.RightHandPose.velocity * SpeedScalingFactor;
                        VROriginPhys.velocity = new Vector3(ScaledSpeed.x, -ScaledSpeed.y, ScaledSpeed.z);
                    }
                    if (LeftHandGrab)
                    {
                        Vector3 ScaledSpeed = SteamVR_Actions._default.LeftHandPose.velocity * SpeedScalingFactor;
                        VROriginPhys.velocity = new Vector3(ScaledSpeed.x, -ScaledSpeed.y, ScaledSpeed.z);
                    }
                }
                else
                {
                    InitialHandDistance = 0f;
                    VROriginPhys.velocity = Vector3.zero;
                }
            }
            
        }

        public static Vector3 EstimateVelocity(Vector3 Position)
        {
            if (PreviousVelocityPosition == Vector3.zero)
            {
                PreviousVelocityPosition = Position;
                return Vector3.zero;
            }
            else
            {
                Vector3 Value = (Position - PreviousVelocityPosition) / Time.deltaTime;
                PreviousVelocityPosition = Position;
                return Value;
            }

        }


        public enum VRCameraMode
        {
            DemeoLike,
            FirstPerson,
            Cutscene,
            UI
        }


    }

}