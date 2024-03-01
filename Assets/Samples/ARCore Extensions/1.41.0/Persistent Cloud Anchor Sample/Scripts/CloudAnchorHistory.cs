//-----------------------------------------------------------------------
// <copyright file="CloudAnchorHistory.cs" company="Google LLC">
//
// Copyright 2020 Google LLC
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

namespace Google.XR.ARCoreExtensions.Samples.PersistentCloudAnchors
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A serializable struct that stores the basic information of a persistent cloud anchor.
    /// </summary>
    [Serializable]
    public struct CloudAnchorHistory
    {
        /// <summary>
        /// An informative name given by the user.
        /// </summary>
        public string Name;

        /// <summary>
        /// The Cloud Anchor Id which is used for resolving.
        /// </summary>
        public string Id;

        /// <summary>
        /// The created time of this Cloud Anchor.
        /// </summary>
        public string SerializedTime;

        /// <summary>
        /// Construct a Cloud Anchor history.
        /// </summary>
        /// <param name="name">An informative name given by the user.</param>
        /// <param name="id">The Cloud Anchor Id which is used for resolving.</param>
        /// <param name="time">The time this Cloud Anchor was created.</param>
        public CloudAnchorHistory(string name, string id, DateTime time)
        {
            Name = name;
            Id = id;
            SerializedTime = time.ToString();
        }

        /// <summary>
        /// Construct a Cloud Anchor history.
        /// </summary>
        /// <param name="name">An informative name given by the user.</param>
        /// <param name="id">The Cloud Anchor Id which is used for resolving.</param>
        public CloudAnchorHistory(string name, string id) : this(name, id, DateTime.Now)
        {
        }

        /// <summary>
        /// Gets created time in DataTime format.
        /// </summary>
        public DateTime CreatedTime
        {
            get
            {
                return Convert.ToDateTime(SerializedTime);
            }
        }

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
    /// A wrapper class for serializing a collection of <see cref="CloudAnchorHistory"/>.
    /// </summary>
    [Serializable]
    public class CloudAnchorHistoryCollection
    {
        /// <summary>
        /// A list of Cloud Anchor History Data.
        /// </summary>
        public List<CloudAnchorHistory> Collection = new List<CloudAnchorHistory>();
    }
}
