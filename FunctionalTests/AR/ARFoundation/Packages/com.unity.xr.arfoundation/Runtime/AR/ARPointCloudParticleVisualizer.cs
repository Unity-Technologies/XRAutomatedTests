using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Renders an <see cref="ARPointCloud"/> as a <c>ParticleSystem</c>.
    /// </summary>
    [RequireComponent(typeof(ARPointCloud))]
    [RequireComponent(typeof(ParticleSystem))]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/api/UnityEngine.XR.ARFoundation.ARPointCloudParticleVisualizer.html")]
    public sealed class ARPointCloudParticleVisualizer : MonoBehaviour
    {
        void OnPointCloudChanged(ARPointCloud pointCloud)
        {
            var points = s_Vertices;
            pointCloud.GetPoints(points, Space.Self);

            int numParticles = points.Count;
            if (m_Particles == null || m_Particles.Length < numParticles)
                m_Particles = new ParticleSystem.Particle[numParticles];

            var color = m_ParticleSystem.main.startColor.color;
            var size = m_ParticleSystem.main.startSize.constant;

            for (int i = 0; i < numParticles; ++i)
            {
                m_Particles[i].startColor = color;
                m_Particles[i].startSize = size;
                m_Particles[i].position = points[i];
                m_Particles[i].remainingLifetime = 1f;
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
            ARSubsystemManager.systemStateChanged += OnSystemStateChanged;
            UpdateVisibility();
        }

        void OnDisable()
        {
            m_PointCloud.updated -= OnPointCloudChanged;
            ARSubsystemManager.systemStateChanged -= OnSystemStateChanged;
            UpdateVisibility();
        }

        void OnSystemStateChanged(ARSystemStateChangedEventArgs eventArgs)
        {
            UpdateVisibility();
        }

        void UpdateVisibility()
        {
            var visible = enabled &&
                (ARSubsystemManager.systemState == ARSystemState.SessionTracking);

            SetVisible(visible);
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

        static List<Vector3> s_Vertices = new List<Vector3>();
    }
}
