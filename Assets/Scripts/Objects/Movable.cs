// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace TestConnectors.Scripts.Objects
{
    /// <summary>
    /// The <see cref="Movable"/> class.
    /// </summary>
    internal sealed class Movable : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float speed = 1f;

        #endregion

        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private Coroutine _moveCoroutine;

        #endregion

        #region Methods

        /// <summary>
        /// Initiates movement of the object to the specified location.
        /// </summary>
        /// <param name="location">location.</param>
        public void Move(Vector3 location)
        {
            // check if the coroutine already exists
            if (_moveCoroutine != null) // then stop it and nullify
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }

            _moveCoroutine = StartCoroutine(_Move(location));
        }

        /// <summary>
        /// This coroutine moves the object to the specified location.
        /// </summary>
        /// <param name="location">location.</param>
        IEnumerator _Move(Vector3 location)
        {
            Vector3 original = transform.position;
            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime * speed;
                transform.position = Vector3.LerpUnclamped(original, location, t);

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}
