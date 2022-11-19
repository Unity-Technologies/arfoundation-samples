using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Renders all points in an <see cref="ARPointCloud"/> as a <c>ParticleSystem</c>, persisting them all.
    /// </summary>
    [RequireComponent(typeof(ARPointCloud))]
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class ARAllPointCloudPointsParticleVisualizer : MonoBehaviour
    {
        public enum Mode
        {
            /// <summary>
            /// Draw all the feature points from the start of the session
            /// </summary>
            All,

            /// <summary>
            /// Only draw the feature points from the current frame
            /// </summary>
            CurrentFrame,
        }

        [SerializeField]
        [Tooltip("Whether to draw all the feature points or only the ones from the current frame.")]
        Mode m_Mode;

        public Mode mode
        {
            get => m_Mode;
            set
            {
                m_Mode = value;
                RenderPoints();
            }
        }

        public int totalPointCount => m_Points.Count;

        void OnPointCloudChanged(ARPointCloudUpdatedEventArgs eventArgs)
        {
            RenderPoints();
        }

        void SetParticlePosition(int index, Vector3 position)
        {
            m_Particles[index].startColor = m_ParticleSystem.main.startColor.color;
            m_Particles[index].startSize = m_ParticleSystem.main.startSize.constant;
            m_Particles[index].position = position;
            m_Particles[index].remainingLifetime = 1f;
        }

        void RenderPoints()
        {
            if (!m_PointCloud.positions.HasValue)
                return;

            var positions = m_PointCloud.positions.Value;

            // Store all the positions over time associated with their unique identifiers
            if (m_PointCloud.identifiers.HasValue)
            {
                var identifiers = m_PointCloud.identifiers.Value;
                for (int i = 0; i < positions.Length; ++i)
                {
                    m_Points[identifiers[i]] = positions[i];
                }
            }

            // Make sure we have enough particles to store all the ones we want to draw
            int numParticles = (mode == Mode.All) ? m_Points.Count : positions.Length;
            if (m_Particles == null || m_Particles.Length < numParticles)
            {
                m_Particles = new ParticleSystem.Particle[numParticles];
            }

            switch (mode)
            {
                case Mode.All:
                {
                    // Draw all the particles
                    int particleIndex = 0;
                    foreach (var kvp in m_Points)
                    {
                        SetParticlePosition(particleIndex++, kvp.Value);
                    }
                    break;
                }
                case Mode.CurrentFrame:
                {
                    // Only draw the particles in the current frame
                    for (int i = 0; i < positions.Length; ++i)
                    {
                        SetParticlePosition(i, positions[i]);
                    }
                    break;
                }
            }

            // Remove any existing particles by setting remainingLifetime
            // to a negative value.
            for (int i = numParticles; i < m_NumParticles; ++i)
            {
                m_Particles[i].remainingLifetime = -1f;
            }

            m_ParticleSystem.SetParticles(m_Particles, Math.Max(numParticles, m_NumParticles));
            m_NumParticles = numParticles;
        }

        void Awake()
        {
            m_PointCloud = GetComponent<ARPointCloud>();
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }

        void OnEnable()
        {
            m_PointCloud.updated += OnPointCloudChanged;
            UpdateVisibility();
        }

        void OnDisable()
        {
            m_PointCloud.updated -= OnPointCloudChanged;
            UpdateVisibility();
        }

        void Update()
        {
            UpdateVisibility();
        }

        void UpdateVisibility()
        {
            SetVisible(enabled && (m_PointCloud.trackingState != TrackingState.None));
        }

        void SetVisible(bool visible)
        {
            if (m_ParticleSystem == null)
                return;

            var renderer = m_ParticleSystem.GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = visible;
        }

        ARPointCloud m_PointCloud;

        ParticleSystem m_ParticleSystem;

        ParticleSystem.Particle[] m_Particles;

        int m_NumParticles;

        Dictionary<ulong, Vector3> m_Points = new Dictionary<ulong, Vector3>();
    }
}
