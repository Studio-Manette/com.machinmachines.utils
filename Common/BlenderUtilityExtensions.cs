// Copyright 2023 MachinMachines
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

namespace MachinMachines.Common
{
    public static class BlenderUtilityExtensions
    {
        public static Vector3 FromUnityToBlender(this Vector3 positionVector)
        {
            return new Vector3(-positionVector.x, -positionVector.z, positionVector.y);
        }

        public static Vector3 FromBlenderToUnity(this Vector3 positionVector)
        {
            return new Vector3(-positionVector.x, positionVector.z, -positionVector.y);
        }

        public static Vector3 EulerAnglesFromUnityToBlender(this Vector3 positionVector)
        {
            return new Vector3(positionVector.x, positionVector.z, -positionVector.y);
        }

        public static Vector3 EulerAnglesFromBlenderToUnity(this Vector3 positionVector)
        {
            return new Vector3(positionVector.x, -positionVector.z, positionVector.y);
        }

        public static Quaternion FromBlenderToUnity(this Quaternion rotation)
        {
            return new Quaternion(rotation.y, -rotation.w, rotation.z, rotation.x);
        }

        public static Quaternion QuaternionFromUnityToBlender(this Quaternion rotation)
        {
            return new Quaternion(rotation.w, rotation.x, rotation.z, -rotation.y);
        }

        public static Vector3 ScaleFromBlenderToUnity(this Vector3 positionVector)
        {
            return new Vector3(positionVector.x, positionVector.z, positionVector.y);
        }

        public static Vector3 ScaleFromUnityToBlender(this Vector3 positionVector)
        {
            return new Vector3(positionVector.x, positionVector.z, positionVector.y);
        }

        /// <summary>
        /// Apply the input PRS data to the given transform
        /// </summary>
        public static void FromBlenderToUnity(
                this Transform existingTransform,
                Vector3 blenderPosition,
                Quaternion blenderRotation,
                Vector3 blenderScale)
        {
            Vector3 newPosition = blenderPosition.FromBlenderToUnity();
            if (existingTransform.localPosition != newPosition)
            {
                existingTransform.localPosition = newPosition;
            }
            Quaternion newRotation = blenderRotation.FromBlenderToUnity();
            if (existingTransform.localRotation != newRotation)
            {
                existingTransform.localRotation = newRotation;
            }
            Vector3 newScale = blenderScale.ScaleFromBlenderToUnity();
            if (existingTransform.localScale != newScale)
            {
                existingTransform.localScale = newScale;
            }
        }
    }
}
