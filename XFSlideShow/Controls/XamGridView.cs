using System;
using System.Collections;
using Xamarin.Forms;

namespace XFSlideShow.Controls
{
	public class XamGridView:Xamarin.Forms.View
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="XamGridView"/> class.
		/// </summary>
		public XamGridView()
		{
			SelectionEnabled = true;
		}

		//
		// Static Fields
		//
		/// <summary>
		/// The items source property
		/// </summary>
		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create ("ItemsSource", typeof (IEnumerable), typeof (XamGridView), null, BindingMode.OneWay, null, null, null, null);

		/// <summary>
		/// The item template property
		/// </summary>
		public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create ("ItemTemplate", typeof (DataTemplate), typeof (XamGridView), null, BindingMode.OneWay, null, null, null, null);

		/// <summary>
		/// The row spacing property
		/// </summary>
		public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create ("RowSpacing", typeof (double), typeof (XamGridView), 0.0, BindingMode.OneWay, null, null, null, null);

		/// <summary>
		/// The column spacing property
		/// </summary>
		public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create ("ColumnSpacing", typeof (double), typeof (XamGridView), 0.0, BindingMode.OneWay, null, null, null, null);

		/// <summary>
		/// The padding property
		/// </summary>
		public static readonly BindableProperty PaddingProperty = BindableProperty.Create<XamGridView, Thickness> (view => view.Padding, new Thickness (0), BindingMode.OneWay);

		/// <summary>
		/// The column spacing property
		/// </summary>
		public static readonly BindableProperty NoOfColumnsProperty = BindableProperty.Create ("NoOfColumns", typeof (int), typeof (XamGridView), 0, BindingMode.OneWay, null, null, null, null);

		//
		// Properties
		//
		/// <summary>
		/// Gets or sets the items source.
		/// </summary>
		/// <value>The items source.</value>
		public IEnumerable ItemsSource {
			get {
				return (IEnumerable)base.GetValue (XamGridView.ItemsSourceProperty);
			}
			set {
				base.SetValue (XamGridView.ItemsSourceProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the item template.
		/// </summary>
		/// <value>The item template.</value>
		public DataTemplate ItemTemplate {
			get {
				return (DataTemplate)base.GetValue (XamGridView.ItemTemplateProperty);
			}
			set {
				base.SetValue (XamGridView.ItemTemplateProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the row spacing.
		/// </summary>
		/// <value>The row spacing.</value>
		public double RowSpacing {
			get {
				return (double)base.GetValue (XamGridView.RowSpacingProperty);
			}
			set {
				base.SetValue (XamGridView.RowSpacingProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the column spacing.
		/// </summary>
		/// <value>The column spacing.</value>
		public double ColumnSpacing {
			get {
				return (double)base.GetValue (XamGridView.ColumnSpacingProperty);
			}
			set {
				base.SetValue (XamGridView.ColumnSpacingProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the column spacing.
		/// </summary>
		/// <value>The column spacing.</value>
		public int NoOfColumns {
			get {
				return (int)base.GetValue (XamGridView.NoOfColumnsProperty);
			}
			set {
				base.SetValue (XamGridView.NoOfColumnsProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the padding.
		/// </summary>
		/// <value>The padding.</value>
		public Thickness Padding {
			get {
				return (Thickness)base.GetValue (PaddingProperty);
			}
			set {
				base.SetValue (PaddingProperty, value);
			}
		}

		/// <summary>
		/// Occurs when item is selected.
		/// </summary>
		public event EventHandler<object> ItemSelected;

		/// <summary>
		/// Invokes the item selected event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="item">Item.</param>
		public void InvokeItemSelectedEvent (object sender, object item)
		{
			if (this.ItemSelected != null) {
				this.ItemSelected.Invoke (sender, item);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [selection enabled].
		/// </summary>
		/// <value><c>true</c> if [selection enabled]; otherwise, <c>false</c>.</value>
		public bool SelectionEnabled {
			get;
			set;
		}
	}
}
