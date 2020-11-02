using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Common
{
    public static class Log
    {
        private static StringBuilder sb = new StringBuilder();
        public static void Debug(params object[] args)
        {
            sb.Clear();
            foreach (object obj in args)
            {
                sb.Append(obj.ToString() + " ");
            }
            UnityEngine.Debug.Log(sb.ToString());
        }
        
        public static void Error(params object[] args)
        {
            sb.Clear();
            foreach (object obj in args)
            {
                sb.Append(obj.ToString() + " ");
            }
            UnityEngine.Debug.LogError(sb.ToString());
        }
    }
}
