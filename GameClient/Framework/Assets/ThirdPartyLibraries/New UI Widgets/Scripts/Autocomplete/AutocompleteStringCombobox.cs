﻿namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Autocomplete as Combobox.
	/// </summary>
	public class AutocompleteStringCombobox : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public AutocompleteString Autocomplete;

		/// <summary>
		/// Button to show all options.
		/// </summary>
		[SerializeField]
		public Button AutocompleteToggle;

		/// <summary>
		/// Return focus to InputField if input not found.
		/// </summary>
		[SerializeField]
		public bool FocusIfInvalid = false;

		/// <summary>
		/// Index of the selected option.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public int Index
		{
			get
			{
				return Autocomplete.DataSource.IndexOf(Autocomplete.InputFieldAdapter.text);
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Autocomplete.InputFieldAdapter.onEndEdit.AddListener(Validate);
			AutocompleteToggle.onClick.AddListener(Autocomplete.ShowAllOptions);
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Autocomplete.InputFieldAdapter.onEndEdit.RemoveListener(Validate);
			AutocompleteToggle.onClick.RemoveListener(Autocomplete.ShowAllOptions);
		}

		/// <summary>
		/// Validate input.
		/// </summary>
		/// <param name="value">Value.</param>
		protected void Validate(string value)
		{
			if (Autocomplete.DataSource.Contains(value))
			{
				return;
			}

			if (FocusIfInvalid)
			{
				Autocomplete.InputFieldAdapter.Focus();
			}
			else
			{
				Autocomplete.InputFieldAdapter.text = string.Empty;
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			if (Autocomplete != null)
			{
				Autocomplete.SetStyle(style);
			}

			if (AutocompleteToggle != null)
			{
				style.Combobox.ToggleButton.ApplyTo(AutocompleteToggle.GetComponent<Image>());
			}

			return true;
		}
		#endregion

	}
}