//-----------------------------------------------------------------------
// <copyright file="GeospatialAnchorHistory.cs" company="Google LLC">
//
// Copyright 2022 Google LLC
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

namespace Google.XR.ARCoreExtensions.Samples.Geospatial
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Describes the current type of anchor created by screen tap.
    /// </summary>
    public enum AnchorType
    {
        /// <summary>
        /// Type <c><see cref="GeospatialAnchor"/></c>.
        /// </summary>
        Geospatial = 0,

        /// <summary>
        /// Type <c><see cref="RooftopAnchor"/></c>.
        /// </summary>
        Rooftop = 1,

        /// <summary>
        /// Type <c><see cref="TerrainAnchor"/></c>.
        /// </summary>
        Terrain = 2,
    }

    /// <summary>
    /// A serializable struct that stores the basic information of a persistent geospatial anchor.
    /// </summary>
    [Serializable]
    public struct GeospatialAnchorHistory
    {
        /// <summary>
        /// The created time of this geospatial anchor.
        /// </summary>
        public string SerializedTime;

        /// <summary>
        /// Latitude of the creation pose in degrees.
        /// </summary>
        public double Latitude;

        /// <summary>
        /// Longitude of the creation pose in degrees.
        /// </summary>
        public double Longitude;

        /// <summary>
        /// Altitude of the creation pose in meters above the WGS84 ellipsoid.
        /// </summary>
        public double Altitude;

        /// <summary>
        /// Heading of the creation pose in degrees, used to calculate the original orientation.
        /// </summary>
        public double Heading;

        /// <summary>
        /// Anchor type of the creation, used to instantiate the original anchor type.
        /// </summary>
        public AnchorType AnchorType;

        /// <summary>
        /// Rotation of the creation pose as a quaternion, used to calculate the original
        /// orientation.
        /// </summary>
        public Quaternion EunRotation;

        /// <summary>
        /// Construct a Geospatial Anchor history.
        /// </summary>
        /// <param name="time">The time this Geospatial Anchor was created.</param>
        /// <param name="latitude">
        /// Latitude of the creation pose in degrees.</param>
        /// <param name="longitude">
        /// Longitude of the creation pose in degrees.</param>
        /// <param name="altitude">
        /// Altitude of the creation pose in meters above the WGS84 ellipsoid.</param>
        /// <param name="anchorType">
        /// Anchor type of the creation.</param>
        /// <param name="eunRotation">
        /// Rotation of the creation pose as a quaternion, used to calculate the original
        /// orientation.
        /// </param>
        public GeospatialAnchorHistory(DateTime time, double latitude, double longitude,
            double altitude, AnchorType anchorType, Quaternion eunRotation)
        {
            SerializedTime = time.ToString();
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Heading = 0.0f;
            AnchorType = anchorType;
            EunRotation = eunRotation;
        }

        /// <summary>
        /// Construct a Geospatial Anchor history.
        /// </summary>
        /// <param name="latitude">
        /// Latitude of the creation pose in degrees.</param>
        /// <param name="longitude">
        /// Longitude of the creation pose in degrees.</param>
        /// <param name="altitude">
        /// Altitude of the creation pose in meters above the WGS84 ellipsoid.</param>
        /// <param name="anchorType">
        /// Anchor type of the creation.</param>
        /// <param name="eunRotation">
        /// Rotation of the creation pose as a quaternion, used to calculate the original
        /// orientation.
        /// </param>
        public GeospatialAnchorHistory(
            double latitude, double longitude, double altitude, AnchorType anchorType,
            Quaternion eunRotation) :
            this(DateTime.Now, latitude, longitude, altitude, anchorType,
            eunRotation)
        {
        }

        /// <summary>
        /// Gets created time in DataTime format.
        /// </summary>
        public DateTime CreatedTime => Convert.ToDateTime(SerializedTime);

        /// <summary>
        /// Overrides ToString() method.
        /// </summary>
        /// <returns>Return the json string of this object.</returns>
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    /// <summary>
    /// A wrapper class for serializing a collection of <see cref="GeospatialAnchorHistory"/>.
    /// </summary>
    [Serializable]
    public class GeospatialAnchorHistoryCollection
    {
        /// <summary>
        /// A list of Geospatial Anchor History Data.
        /// </summary>
        public List<GeospatialAnchorHistory> Collection = new List<GeospatialAnchorHistory>();
    }
}
