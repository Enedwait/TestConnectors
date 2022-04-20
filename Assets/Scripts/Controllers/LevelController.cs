// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TestConnectors.Scripts.Objects;
using UnityEngine;

namespace TestConnectors.Scripts.Controllers
{
    /// <summary>
    /// The <see cref="LevelController"/> class.
    /// </summary>
    internal sealed class LevelController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform plane;
        [SerializeField] private Transform connectablePool;
        [SerializeField] private GameObject connectablePrefab;
        [SerializeField] private float connectablesCount = 10;
        [SerializeField] private float radius = 10;

        #endregion

        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<Connector> _connectors = new List<Connector>();

        #endregion

        #region Properties

        /// <summary> Gets the <see cref="LevelController"/> class instance. </summary>
        public static LevelController Instance { get; private set; }

        /// <summary> Gets the main camera. </summary>
        public Camera MainCamera => mainCamera;

        /// <summary> Gets the connectables pool. </summary>
        public Transform ConnectablePool => connectablePool;

        /// <summary> Gets the 'Connector' layer value. </summary>
        public static int LayerConnector { get; private set; }

        /// <summary> Gets the 'Platform' layer value. </summary>
        public static int LayerPlatform { get; private set; }

        /// <summary> Gets the 'Ground' layer value. </summary>
        public static int LayerGround { get; private set; }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            Instance = this;

            // precompute the layer values
            LayerConnector = LayerMask.NameToLayer("Connector");
            LayerPlatform = LayerMask.NameToLayer("Platform");
            LayerGround = LayerMask.NameToLayer("Ground");

            float scaleFactor = radius / 5f;
            // resize plane accordingly
            plane.localScale = new Vector3(plane.localScale.x * scaleFactor, plane.localScale.y, plane.localScale.z * scaleFactor);

            SetCameraDistance(plane.GetComponent<MeshRenderer>());
        }

        /// <summary>
        /// Start is called just before any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            Generate();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the game level.
        /// </summary>
        void Generate()
        {
            // iterate through all the connectables and create them
            for (int i = 0; i < connectablesCount; i++)
            {
                Vector2 position = UnityEngine.Random.insideUnitCircle * radius;
                GameObject gameObject = Instantiate(connectablePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity, connectablePool);

                // get the connectors for every connectable
                Connector[] connectors = gameObject.GetComponentsInChildren<Connector>();
                if (connectors.Length > 0)
                {
                    // iterate through all the connectors and subscribe to events
                    foreach (Connector connector in connectors)
                    {
                        connector.Selected += Connector_Selected;
                        connector.Rejected += Connector_Rejected;
                        connector.Connected += Connector_Connected;
                        _connectors.Add(connector);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the camera distance accordingly to the size of the specified object's mesh.
        /// </summary>
        /// <param name="meshRenderer">mesh renderer.</param>
        private void SetCameraDistance(MeshRenderer meshRenderer)
        {
            if (meshRenderer == null)
                return;

            Vector3 size = meshRenderer.bounds.max - meshRenderer.bounds.min;
            float cameraView = Mathf.Tan(0.5f * Mathf.Deg2Rad * mainCamera.fieldOfView);
            float objectSize = Mathf.Max(size.x, size.y, size.z);
            float distance = 0.6f * objectSize / cameraView;
            mainCamera.transform.position = meshRenderer.bounds.center - distance * mainCamera.transform.forward;
        }

        #endregion

        #region Events Handling

        /// <summary>
        /// Handles the 'Connected' event of the connector.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event args.</param>
        private void Connector_Connected(object sender, EventArgs e)
        {
            foreach (Connector connector in _connectors)
                connector.SetVisualState(ConnectorVisualState.Normal);
        }

        /// <summary>
        /// Handles the 'Selected' event of the connector.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event args.</param>
        private void Connector_Selected(object sender, EventArgs e)
        {
            if (sender is Connector current)
                foreach (Connector connector in _connectors)
                    if (!connector.Equals(current))
                        connector.SetVisualState(ConnectorVisualState.Available);
        }

        /// <summary>
        /// Handles the 'Rejected' event of the connector.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event args.</param>
        private void Connector_Rejected(object sender, EventArgs e)
        {
            foreach (Connector connector in _connectors)
                connector.SetVisualState(ConnectorVisualState.Normal);
        }

        #endregion
    }
}
