﻿namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewVariableWidth component.
	/// </summary>
	public class ListViewVariableWidthComponent : ListViewItem, IViewData<ListViewVariableWidthItemDescription>
	{
		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[]
				{
					Utilities.GetGraphic(NameAdapter),
					Utilities.GetGraphic(TextAdapter),
				};
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NameAdapter.")]
		public Text Name;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		public Text Text;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewVariableWidthItemDescription item)
		{
			NameAdapter.text = item.Name;
			TextAdapter.text = item.Text.Replace("\\n", "\n");
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
			Utilities.GetOrAddComponent(Text, ref TextAdapter);
#pragma warning restore 0612, 0618
		}
	}
}