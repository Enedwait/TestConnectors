// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TestConnectors.Scripts.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestConnectors.Scripts.Controllers
{
    /// <summary>
    /// The <see cref="PlayerController"/> class.
    /// </summary>
    internal sealed class PlayerController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private PlayerInput input;

        #endregion

        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool _isLeftPressed = false;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private Movable _movable;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private Connector _connector;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private Connector _focusedConnector;

        #endregion

        #region Methods

        /// <summary>
        /// Acquires the raycast hits from the specified position on the screen sorted by the distance in ascending order.
        /// </summary>
        /// <param name="position">screen position.</param>
        /// <returns>raycast hits.</returns>
        private List<RaycastHit> GetPointerHits(Vector2 position)
        {
            List<RaycastHit> hits = Physics.RaycastAll(LevelController.Instance.MainCamera.ScreenPointToRay(position)).ToList();
            hits.Sort((x, y) => x.distance.CompareTo(y.distance));
            return hits;
        }

        #endregion

        #region Input Events Handling

        /// <summary>
        /// Processes the 'LeftClick' input action.
        /// </summary>
        /// <param name="value">input value.</param>
        void OnLeftClick(InputValue value)
        {
            float click = value.Get<float>();
            _isLeftPressed = click > 0;

            // check if the left button pressed
            if (_isLeftPressed)
            {
                // iterate through all the raycast hits and determine the proper action
                foreach (RaycastHit hit in GetPointerHits(Mouse.current.position.ReadValue()))
                {
                    // check if the hit represents the connector
                    if (hit.transform.gameObject.layer == LevelController.LayerConnector)
                    {
                        Connector newconnector = hit.transform.GetComponent<Connector>();

                        // check if the connector is selected already
                        if (_connector != null) // then we need to determine if it's the same connector as the new one or not
                        {
                            if (_connector == newconnector) // if it's the same connector then we need to reject / deselect it
                            {
                                _connector.Reject();
                                _connector = null;
                                break;
                            }

                            // if it's new connector then we need to connect them and release both
                            _connector.Connect(newconnector);
                            _connector.Reject();
                            _connector = null;
                            break;
                        }

                        // if there is no selected connector then we need select this one
                        _connector = newconnector;
                        _connector.Select();
                        break;
                    }

                    // check if the hit represents the platform
                    if (hit.transform.gameObject.layer == LevelController.LayerPlatform) // then we need to select it for future actions
                    {
                        _movable = hit.transform.parent.GetComponent<Movable>();
                        break;
                    }
                }
            }
            else // the left button was released
            {
                // if we moved smth then drop it
                _movable = null;

                // check if the connector is selected
                if (_connector != null)
                {
                    bool found = false;
                    // iterate through all the raycast hits and determine the proper action
                    foreach (var hit in GetPointerHits(Mouse.current.position.ReadValue()))
                    {
                        // check if the hit represents the connector
                        if (hit.transform.gameObject.layer == LevelController.LayerConnector)
                        {
                            found = true;
                            Connector newconnector = hit.transform.GetComponent<Connector>();
                            // check if the new connector is not the same as the old one
                            if (_connector != newconnector) // then connect them
                            {
                                _connector.Connect(newconnector);
                                _connector.Reject();
                                _connector = null;
                                break;
                            }
                        }
                    }

                    // check if we found nothing
                    if (!found) // then reject / deselect the connector
                    {
                        _connector.Reject();
                        _connector = null;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the 'Position' input action.
        /// </summary>
        /// <param name="value">input value.</param>
        void OnPosition(InputValue value)
        {
            Vector2 position = value.Get<Vector2>();

            // check if the left button is being held
            if (_isLeftPressed)
            {
                Vector3 newPosition = Vector3.zero;
                bool isGroundHit = false;

                // iterate through all the raycast hits and determine the proper action
                foreach (var hit in GetPointerHits(position))
                {
                    // check if the hit represents the connector
                    if (hit.transform.gameObject.layer == LevelController.LayerConnector)
                    {
                        Connector newconnector = hit.transform.GetComponent<Connector>();

                        // check if the old connector exists and doesn't equal to the new one
                        if (_connector != null) // then assume the new connector is focused but not selected
                        {
                            if (!_connector.Equals(newconnector))
                            {
                                _focusedConnector = newconnector;
                                _focusedConnector.SetVisualState(ConnectorVisualState.Focused);
                                _connector.DrawConnection(_focusedConnector.transform.position);
                                break;
                            }
                        }
                        else
                            break;
                    }

                    // we hit smth else than connector
                    // check if the focused connector is set
                    if (_focusedConnector != null) // then change it's state accordingly
                        if (_connector != null) _focusedConnector.SetVisualState(ConnectorVisualState.Available);
                        else _focusedConnector.SetVisualState(ConnectorVisualState.Normal);

                    // check if we hit ground
                    if (hit.transform.gameObject.layer == LevelController.LayerGround) // then get new position
                    {
                        isGroundHit = true;
                        newPosition = hit.point;
                        break;
                    }
                }

                // check if we have the flag set
                if (isGroundHit) // then we can move or draw connection
                {
                    if (_movable != null) _movable.Move(newPosition);
                    else if (_connector != null) _connector.DrawConnection(newPosition);
                }
            }
            else
            {
                _focusedConnector = null;
            }
        }

        #endregion
    }
}
