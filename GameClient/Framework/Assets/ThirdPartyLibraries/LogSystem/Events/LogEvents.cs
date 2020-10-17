/********************************************************************
 Date: 2020-09-23
 Name: LogEvents
 author:  zhuzizheng
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace LogSystem
{
	/// <summary>
	/// 手势事件,双击,或者 画圆
	/// </summary>
	public static class LogEvents
	{
		static List<Vector2> gestureDetector = new List<Vector2>();
		static Vector2       gestureSum      = Vector2.zero;
		static float         gestureLength   = 0;
		static int           gestureCount    = 0;

		public static bool IsGestureDone()
		{
			if (Application.platform == RuntimePlatform.Android ||
			    Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (Input.touches.Length != 1)
				{
					gestureDetector.Clear();
					gestureCount = 0;
				}
				else
				{
					if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
						gestureDetector.Clear();
					else if (Input.touches[0].phase == TouchPhase.Moved)
					{
						Vector2 p = Input.touches[0].position;
						if (gestureDetector.Count                                      == 0 ||
						    (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
							gestureDetector.Add(p);
					}
				}
			}
			else
			{
				if (Input.GetMouseButtonUp(0))
				{
					gestureDetector.Clear();
					gestureCount = 0;
				}
				else
				{
					if (Input.GetMouseButton(0))
					{
						Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
						if (gestureDetector.Count                                      == 0 ||
						    (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
							gestureDetector.Add(p);
					}
				}
			}

			if (gestureDetector.Count < 10)
				return false;

			gestureSum    = Vector2.zero;
			gestureLength = 0;
			Vector2 prevDelta = Vector2.zero;
			for (int i = 0; i < gestureDetector.Count - 2; i++)
			{

				Vector2 delta       = gestureDetector[i + 1] - gestureDetector[i];
				float   deltaLength = delta.magnitude;
				gestureSum    += delta;
				gestureLength += deltaLength;

				float dot = Vector2.Dot(delta, prevDelta);
				if (dot < 0f)
				{
					gestureDetector.Clear();
					gestureCount = 0;
					return false;
				}

				prevDelta = delta;
			}

			int gestureBase = (Screen.width + Screen.height) / 4;

			if (gestureLength > gestureBase && gestureSum.magnitude < gestureBase / 2)
			{
				gestureDetector.Clear();
				gestureCount++;
				if (gestureCount >= 1)
					return true;
			}

			return false;
		}


		static float lastClickTime = -1;

		public static bool IsDoubleClickDone()
		{
			if (Application.platform == RuntimePlatform.Android ||
			    Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (Input.touches.Length != 1)
				{
					lastClickTime = -1;
				}
				else
				{
					if (Input.touches[0].phase == TouchPhase.Began)
					{
						if (lastClickTime == -1)
							lastClickTime = Time.realtimeSinceStartup;
						else if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
						{
							lastClickTime = -1;
							return true;
						}
						else
						{
							lastClickTime = Time.realtimeSinceStartup;
						}
					}
				}
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					if (lastClickTime == -1)
						lastClickTime = Time.realtimeSinceStartup;
					else if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
					{
						lastClickTime = -1;
						return true;
					}
					else
					{
						lastClickTime = Time.realtimeSinceStartup;
					}
				}
			}

			return false;
		}
	}
}