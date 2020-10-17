using UnityEngine;
using System.Collections;

namespace LogSystem
{
	public class LogDrawGUIContainer : MonoBehaviour
	{
		void OnGUI()
		{
			LogView logView = LogManager.GetLogView();
			if (logView != null) logView.DrawGUI();
		}
	}
}