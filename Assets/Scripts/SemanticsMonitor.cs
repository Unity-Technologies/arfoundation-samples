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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SemanticsMonitor : MonoBehaviour
{
    private static readonly int ColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int TextureId = Shader.PropertyToID("_BaseMap");
    
    [SerializeField]
    private float overlayInterval;

    [SerializeField]
    private Renderer overlayRenderer; //semantics layer

    [Range(0f,1f)]
    [SerializeField]
    private float overlayAlpha;
    
    [Serializable]
    public class LabelMapping
    {
        public SemanticLabel label;
        public Sprite sprite;
        public Color color;
    }

    [FormerlySerializedAs("spriteMappings")] [SerializeField] 
    private LabelMapping[] mappings;

    [SerializeField]
    private Button overlayButton;

    [Range(0f,1f)]
    [SerializeField]
    private float overlayButtonAlpha;

    [Serializable]
    private class HumorMapping
    {
        public SemanticLabel label;
        public string message;
        public AudioClip sound;
    }

    [SerializeField]
    private Camera semanticCamera;
    
    private ARSemanticManager semanticsManager;
    
    private Texture2D inputTexture;
    private Texture2D overlayTexture;
    private Color[] overlayPixels;
    
    private void Awake()
    {
        semanticsManager = GetComponent<ARSemanticManager>();
    }

    private void Start()
    {
        overlayRenderer.enabled = false;
        semanticCamera.enabled = false;
    
        StartCoroutine(UpdateOverlayCoroutine());
    }
    
    private IEnumerator UpdateOverlayCoroutine()
    {
        while (true)
        {
            if (overlayRenderer.enabled)
            {
                Material material = overlayRenderer.material;
                if (FillOverlayTexture(ref overlayTexture))
                {
                    material.SetTexture(TextureId, overlayTexture);
                    material.SetColor(ColorId, Color.white);
                }
                else
                {
                    material.SetTexture(TextureId, null);
                    material.SetColor(ColorId, new Color(0f,0f,0f,overlayAlpha));
                }

                yield return new WaitForSeconds(overlayInterval);
            }
            else
            {
                yield return null;
            }
        }   
    }

    /// <summary>
    /// Fills the given texture with color coded data from semantics api.
    /// </summary>
    /// <param name="result">The texure to fill.</param>
    /// <returns>true if successful, false if there was an error and the texure was not properly filled</returns>
    private bool FillOverlayTexture(ref Texture2D result)
    {
        FeatureSupported supportedState = semanticsManager.IsSemanticModeSupported(SemanticMode.Enabled);
        if (supportedState != FeatureSupported.Supported)
        {
            Debug.Log($"Semantics mode: {supportedState}");
            return false;
        }

        if (!semanticsManager.TryGetSemanticTexture(ref inputTexture))
        {
            Debug.Log("Could not fill semantic texture");
            return false;
        }

        // the input texture is in landscape and flipped
        // we do the math here to convert it into the output texture
        // that matches the camera image on screen
        
        if (result == null || result.height != inputTexture.width ||
            result.width != inputTexture.height) 
        {
            // output texture is of reverse width/height
            result = new Texture2D(inputTexture.height, inputTexture.width, TextureFormat.RGBA32, false);
        }

        var rawTextureData = inputTexture.GetRawTextureData<byte>();
        if (overlayPixels == null || overlayPixels.Length != rawTextureData.Length)
        {
            overlayPixels = new Color[rawTextureData.Length];
        }

        for (int i = 0; i < inputTexture.height; i++)
        {
            for (int j = 0; j < inputTexture.width; j++)
            {
                var label = (SemanticLabel) rawTextureData[i * inputTexture.width + j]; // input[j,i]
                Color color = GetColor(label);
                overlayPixels[^(j * overlayTexture.width + i + 1)] = color; // output[w-i,h-j]
            }
        }

        if (!semanticsManager.TryGetSemanticConfidenceTexture(ref inputTexture))
        {
            Debug.Log("Could not fill semantic confidence texture");
            return false;
        }
        
        rawTextureData = inputTexture.GetRawTextureData<byte>();
        
        for (int i = 0; i < inputTexture.height; i++)
        {
            for (int j = 0; j < inputTexture.width; j++)
            {
                byte confidence = rawTextureData[i * inputTexture.width + j];
                overlayPixels[^(j * overlayTexture.width + i + 1)].a *= confidence / 255f * overlayAlpha;
            }
        }
        
        result.SetPixels(overlayPixels);
        result.Apply();
        return true;
    }

    private Color GetColor(SemanticLabel label)
    {
        if (label == SemanticLabel.Unlabeled) return Color.gray;
        
        foreach (var mapping in mappings)
        {
            if (mapping.label == label)
            {
                return mapping.color;
            }
        }

        return Color.black;
    }

    public void ToggleOverlayActive()
    {
        OverlayActive = !OverlayActive;
        var buttonColors = overlayButton.colors;
        buttonColors.normalColor = new Color(1f, 1f, 1f, OverlayActive ? 1f : overlayButtonAlpha);
        overlayButton.colors = buttonColors;
    }

    private bool OverlayActive
    {
        get => overlayRenderer.enabled;
        set
        {
            overlayRenderer.enabled = value;
            semanticCamera.enabled = value;
        }
    }
}
