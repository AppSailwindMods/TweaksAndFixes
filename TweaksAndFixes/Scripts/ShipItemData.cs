using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TweaksAndFixes.Scripts
{
    public sealed class ShipItemData : MonoBehaviour
    {
        internal Dictionary<string, object> data = new Dictionary<string, object>();

        public void SetData(string key, object value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
            ModLogger.LogWarning(TweaksAndFixesMain.instance.Info, "set_" + key + "_" + data[key]);
        }

        public T GetData<T>(string key, T defaultValue)
        {
            if (data.ContainsKey(key))
            {
                ModLogger.LogWarning(TweaksAndFixesMain.instance.Info, "load_" + key + "_" + data[key]);
                try
                {
                    return (T)data[key];
                }
                catch
                {
                    ModLogger.LogError(TweaksAndFixesMain.instance.Info, $"Failed to cast {data[key].GetType()} to {typeof(T)}");
                }
            }
            return defaultValue;
        }
    }
}
