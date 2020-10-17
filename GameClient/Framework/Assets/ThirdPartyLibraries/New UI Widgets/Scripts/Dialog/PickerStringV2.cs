﻿namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// PickerStringV2.
	/// </summary>
	public class PickerStringV2 : Picker<string, PickerStringV2>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewString ListView;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(string defaultValue)
		{
			ListView.SelectedIndex = ListView.DataSource.IndexOf(defaultValue);

			ListView.OnSelectObject.AddListener(ListViewCallback);
		}

		void ListViewCallback(int index)
		{
			var value = ListView.DataSource[index];
			Selected(value);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			ListView.OnSelectObject.RemoveListener(ListViewCallback);
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			ListView.SetStyle(style);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}