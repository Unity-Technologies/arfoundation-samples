// Copyright 2023 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
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
// adapted from https://github.com/google-ar/demo-megagolf

using UnityEngine;

public class FitToScreen : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = new Vector3(Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height, Camera.main.orthographicSize * 2.0f, 0.1f);
    }
}