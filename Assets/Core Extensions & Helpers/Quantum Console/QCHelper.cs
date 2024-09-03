using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

namespace Core.Extensions
{
    public static class QCHelper
    {
        public static bool IsOpen => QuantumConsole.Instance == null ? false : QuantumConsole.Instance.IsActive;
    }
}
