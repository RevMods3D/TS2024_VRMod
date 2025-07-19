using System;
using System.Numerics;
using Valve.VR;

namespace VRTrackingLib
{
    public class VRHeadTracker
    {
        private CVRSystem? vrSystem;
        private CVRCompositor vrCompositor;

        public bool Initialize()
        {
            EVRInitError error = EVRInitError.None;
            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Scene); // <- important

            if (error != EVRInitError.None)
            {
                Console.WriteLine("OpenVR Init failed: " + error);
                return false;
            }

            vrSystem = OpenVR.System;

            int timeout = 0;
            while (OpenVR.Compositor == null)
            {
                if (timeout > 5000)
                {
                    Console.WriteLine("Timeout waiting for OpenVR Compositor.");
                    return false;
                }
                Console.WriteLine("Waiting for OpenVR Compositor...");
                Thread.Sleep(200);
                timeout += 200;
            }

            vrCompositor = OpenVR.Compositor;

            Console.WriteLine("OpenVR Compositor ready.");
            return vrSystem != null && vrCompositor != null;
        }

        public Vector3 GetHeadPosition()
        {
            if (vrSystem == null || vrCompositor == null)
                throw new InvalidOperationException("VR system or compositor not initialized.");

            TrackedDevicePose_t[] renderPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

            try
            {
                vrCompositor.WaitGetPoses(renderPoses, gamePoses);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception calling WaitGetPoses: " + ex.Message);
                return Vector3.Zero;
            }

            var hmdPose = renderPoses[0];

            if (!hmdPose.bPoseIsValid)
                return Vector3.Zero;

            var matrix = hmdPose.mDeviceToAbsoluteTracking;

            float x = matrix.m3;
            float y = matrix.m7;
            float z = matrix.m11;

            return new Vector3(x, y, z);
        }

        public void Shutdown()
        {
            OpenVR.Shutdown();
        }
    }
}