// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using UnityEngine;

namespace TestConnectors.Scripts.Objects
{
    /// <summary>
    /// The <see cref="Connectable"/> class.
    /// </summary>
    internal sealed class Connectable : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Connector connector;
        [SerializeField] private Transform platform;

        #endregion
    }
}
