using System.Collections.Generic;
using UnityEngine;

namespace Mobiray.Common
{
    [CreateAssetMenu(fileName = "ConfigBundle", menuName = "Tools/ConfigBundle")]
    public class ConfigBundle : ScriptableObject
    {
        public List<ScriptableObject> tools;
    }
}