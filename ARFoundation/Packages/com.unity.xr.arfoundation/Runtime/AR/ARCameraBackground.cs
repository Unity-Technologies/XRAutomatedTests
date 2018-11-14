using System;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Add this component to a <c>Camera</c> to copy the color camera's texture onto the background.
    /// </summary>
    /// <remarks>
    /// This is the component-ized version of <c>UnityEngine.XR.ARBackgroundRenderer</c>.
    /// </remarks>
    [DisallowMultipleComponent, RequireComponent(typeof(Camera))]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/api/UnityEngine.XR.ARFoundation.ARCameraBackground.html")]
    public sealed class ARCameraBackground : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("m_OverrideMaterial")]
        bool m_UseCustomMaterial;

        /// <summary>
        /// When <c>false</c>, a material is generated automatically from the shader included in the platform-specific package.
        /// When <c>true</c>, <see cref="customMaterial"/> is used instead, overriding the automatically generated one.
        /// This is not necessary for most AR experiences.
        /// </summary>
        public bool useCustomMaterial
        {
            get { return m_UseCustomMaterial; }
            set
            {
                m_UseCustomMaterial = value;
                UpdateMaterial();
            }
        }

        [SerializeField, FormerlySerializedAs("m_Material")]
        Material m_CustomMaterial;

        /// <summary>
        /// If <see cref="useCustomMaterial"/> is <c>true</c>, this <c>Material</c> will be used
        /// instead of the one included with the platform-specific AR package.
        /// </summary>
        public Material customMaterial
        {
            get { return m_CustomMaterial; }
            set
            {
                m_CustomMaterial = value;
                UpdateMaterial();
            }
        }

        /// <summary>
        /// The current <c>Material</c> used for background rendering.
        /// </summary>
        public Material material
        {
            get
            {
                return backgroundRenderer.backgroundMaterial;
            }
            private set
            {
                backgroundRenderer.backgroundMaterial = value;
                if (ARSubsystemManager.cameraSubsystem != null)
                    ARSubsystemManager.cameraSubsystem.Material = value;
            }
        }

        [SerializeField]
        bool m_UseCustomRendererAsset;

        /// <summary>
        /// Whether to use a <see cref="ARBackgroundRendererAsset"/>. This can assist with
        /// usage of the light weight render pipeline.
        /// </summary>
        public bool useCustomRendererAsset
        {
            get { return m_UseCustomRendererAsset; }
            set { m_UseCustomRendererAsset = value; }
        }

        [SerializeField] 
        ARBackgroundRendererAsset m_CustomRendererAsset;

        /// <summary>
        /// Get the custom <see cref="ARBackgroundRendererAsset "/> to use. This can
        /// assist with usage of the light weight render pipeline.
        /// </summary>
        public ARBackgroundRendererAsset customRendererAsset
        {
            get { return m_CustomRendererAsset; }
        }

        ARFoundationBackgroundRenderer backgroundRenderer { get; set; }

        Material CreateMaterialFromSubsystemShader()
        {
            var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
            if (m_CameraSetupThrewException || (cameraSubsystem == null))
                return null;

            // Try to create a material from the plugin's provided shader.
            string shaderName = "";
            if (!cameraSubsystem.TryGetShaderName(ref shaderName))
                return null;

            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                // If an exception is thrown, then something is irrecoverably wrong.
                // Set this flag so we don't try to do this every frame.
                m_CameraSetupThrewException = true;

                throw new InvalidOperationException(string.Format(
                    "Could not find shader named \"{0}\" required for video overlay on camera subsystem named \"{1}\".",
                    shaderName,
                    cameraSubsystem.SubsystemDescriptor.id));
            }

            return new Material(shader);
        }

        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            ARSubsystemManager.cameraSubsystem.Camera = camera;
            UpdateMaterial();
            backgroundRenderer.mode = ARRenderMode.MaterialAsBackground;
        }

        void Awake()
        {
            var useRenderPipeline = GraphicsSettings.renderPipelineAsset != null;
            if (useRenderPipeline && m_UseCustomRendererAsset && m_CustomRendererAsset != null)
            {
                backgroundRenderer = m_CustomRendererAsset.CreateARBackgroundRenderer();
                m_CustomRendererAsset.CreateHelperComponents(gameObject);
            }
            else
            {
                backgroundRenderer = new ARFoundationBackgroundRenderer();
            }
            
            backgroundRenderer.camera = GetComponent<Camera>();
        }

        void OnEnable()
        {
            UpdateMaterial();
            if (ARSubsystemManager.cameraSubsystem != null)
                ARSubsystemManager.cameraSubsystem.Camera = camera;
            ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
            ARSubsystemManager.systemStateChanged += OnSystemStateChanged;
        }

        void OnDisable()
        {
            backgroundRenderer.mode = ARRenderMode.StandardBackground;
            ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
            ARSubsystemManager.systemStateChanged -= OnSystemStateChanged;
            m_CameraSetupThrewException = false;

            // Tell the camera subsystem to stop doing work if we are still the active camera
            var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
            if ((cameraSubsystem != null) && (cameraSubsystem.Camera == camera))
            {
                cameraSubsystem.Camera = null;
                cameraSubsystem.Material = null;
            }

            // We are no longer setting the projection matrix
            // so tell the camera to resume its normal projection
            // matrix calculations.
            camera.ResetProjectionMatrix();
        }

        void OnSystemStateChanged(ARSystemStateChangedEventArgs eventArgs)
        {
            // If the session goes away then return to using standard background mode
            if (eventArgs.state < ARSystemState.SessionInitializing && backgroundRenderer != null)
                backgroundRenderer.mode = ARRenderMode.StandardBackground;
        }

        void UpdateMaterial()
        {
            var useRenderPipeline = GraphicsSettings.renderPipelineAsset != null;
            if (useRenderPipeline && m_UseCustomRendererAsset && m_CustomRendererAsset != null)
            {
                material = lwrpMaterial;
            }
            else
            {
                material = m_UseCustomMaterial ? m_CustomMaterial : subsystemMaterial;
            }
        }

        bool m_CameraSetupThrewException;

#if UNITY_EDITOR
        new Camera camera
#else
        Camera camera
#endif
        { get { return backgroundRenderer.camera; } }

        Material m_SubsystemMaterial;

        private Material subsystemMaterial
        {
            get
            {
                if (m_SubsystemMaterial == null)
                    m_SubsystemMaterial = CreateMaterialFromSubsystemShader();

                return m_SubsystemMaterial;
            }
        }

        Material m_LwrpMaterial;

        Material lwrpMaterial
        {
            get
            {
                if (m_LwrpMaterial != null)
                    return m_LwrpMaterial;

                if (m_UseCustomRendererAsset && m_CustomRendererAsset != null)
                {
                    m_LwrpMaterial = m_CustomRendererAsset.CreateCustomMaterial();
                }

                return m_LwrpMaterial;
            }
        }
    }
}
