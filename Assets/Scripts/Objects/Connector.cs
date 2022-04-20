// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System;
using System.ComponentModel;
using UnityEngine;

namespace TestConnectors.Scripts.Objects
{
    /// <summary>
    /// The <see cref="Connector"/> class.
    /// </summary>
    internal sealed class Connector : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Color colorNormal = Color.white;
        [SerializeField] private Color colorSelected = Color.yellow;
        [SerializeField] private Color colorAvailableToConnect = Color.blue;
        [SerializeField] private float connectionWidthMultiplier = 0.1f;

        #endregion

        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private Renderer _renderer;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private LineRenderer _lineRenderer;

        #endregion

        #region Properties

        /// <summary> Gets the value indicating whether the connector is selected or not. </summary>
        public bool IsSelected { get; private set; }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            _renderer = transform.GetComponent<Renderer>();
            _renderer.material.SetColor("_Color", colorNormal);

            _lineRenderer = transform.gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.widthMultiplier = connectionWidthMultiplier;
            _lineRenderer.enabled = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the visualState of the connector
        /// </summary>
        /// <param name="visualState"></param>
        public void SetVisualState(ConnectorVisualState visualState)
        {
            switch (visualState)
            {
                case ConnectorVisualState.Normal:  _renderer.material.SetColor("_Color", colorNormal); break;
                case ConnectorVisualState.Selected: _renderer.material.SetColor("_Color", colorSelected);  break;
                case ConnectorVisualState.Focused: _renderer.material.SetColor("_Color", colorSelected); break;
                case ConnectorVisualState.Available: _renderer.material.SetColor("_Color", colorAvailableToConnect); break;
            }
        }

        /// <summary>
        /// Selects the object.
        /// </summary>
        public void Select()
        {
            if (IsSelected)
                return;

            IsSelected = true;
            SetVisualState(ConnectorVisualState.Selected);
            Selected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Rejects the object.
        /// </summary>
        public void Reject()
        {
            if (!IsSelected)
                return;

            IsSelected = false;
            HideConnection();
            SetVisualState(ConnectorVisualState.Normal);
            Rejected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Connect the connector to the specified one.
        /// </summary>
        /// <param name="other">other connector.</param>
        public void Connect(Connector other)
        {
            if (other == null)
                return;
            
            Connection.Create(this, other);
            Connected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Draws the connection to the specified position.
        /// </summary>
        /// <param name="position">position.</param>
        public void DrawConnection(Vector3 position)
        {
            if (!_lineRenderer.enabled)
                _lineRenderer.enabled = true;

            _lineRenderer.SetPositions(new Vector3[] { transform.position, position });
        }

        /// <summary>
        /// Hides the connection.
        /// </summary>
        public void HideConnection()
        {
            if (_lineRenderer.enabled)
            {
                _lineRenderer.enabled = false;
                _lineRenderer.SetPositions(new Vector3[] { Vector3.zero });
            }
        }

        #endregion

        #region Events

        /// <summary> The event occurs when the connector is being selected. </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary> The event occurs when the connector is being rejected. </summary>
        public event EventHandler<EventArgs> Rejected;

        /// <summary> The event occurs when the connector is being connected. Works only for the one which calls 'Connect' method. </summary>
        public event EventHandler<EventArgs> Connected;

        #endregion
    }
}
