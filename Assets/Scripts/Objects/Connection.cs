// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.ComponentModel;
using TestConnectors.Scripts.Controllers;
using UnityEngine;

namespace TestConnectors.Scripts.Objects
{
    /// <summary>
    /// The <see cref="Connection"/> class.
    /// </summary>
    internal sealed class Connection : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Connector connectorA;
        [SerializeField] private Connector connectorB;
        [SerializeField] private float connectionWidthMultiplier = 0.2f;

        #endregion

        #region Private

        [EditorBrowsable(EditorBrowsableState.Never)]
        private LineRenderer _lineRenderer;

        #endregion

        #region Lifecycle

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            _lineRenderer = transform.gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.widthMultiplier = connectionWidthMultiplier;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            _lineRenderer.SetPositions(new Vector3[]{connectorA.transform.position, connectorB.transform.position});
        }

        #endregion

        #region Static

        /// <summary>
        /// Creates the connection between two specified connectors.
        /// Doesn't check for duplicates.
        /// </summary>
        /// <param name="a">connector A/</param>
        /// <param name="b">connector B.</param>
        /// <returns>the connection.</returns>
        public static Connection Create(Connector a, Connector b)
        {
            GameObject gameObject = Instantiate(new GameObject($"Connection {a}-{b}"), LevelController.Instance.ConnectablePool);
            Connection connection = gameObject.AddComponent<Connection>();
            connection.connectorA = a;
            connection.connectorB = b;
            return connection;
        }

        #endregion
    }
}
