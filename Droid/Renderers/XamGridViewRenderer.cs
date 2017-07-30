﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AGridView = Android.Widget.GridView;
using XamGridView = XFSlideShow.Controls.XamGridView;

[assembly: ExportRenderer(typeof(XamGridView), typeof(XFSlideShow.Droid.XamGridViewRenderer))]
namespace XFSlideShow.Droid
{
    public class XamGridViewRenderer : ViewRenderer<XamGridView, AGridView>
    {
		private readonly global::Android.Content.Res.Orientation _orientation = global::Android.Content.Res.Orientation.Undefined;
        public XamGridViewRenderer()
        {
        }

		protected override void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (newConfig.Orientation != _orientation)
                OnElementChanged(new ElementChangedEventArgs<XamGridView>(this.Element, this.Element));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XamGridView> e)
        {
            base.OnElementChanged(e);

            try
			{
				var context = Xamarin.Forms.Forms.Context;
				var collectionView = new AGridView(context);
				collectionView.SetGravity(GravityFlags.Center);
				collectionView.StretchMode = StretchMode.StretchColumnWidth;

				var metrics = Resources.DisplayMetrics;
				var width = metrics.WidthPixels;
				var spacing = (int)e.NewElement.ColumnSpacing * 3;
				int noOfColumns = (int)e.NewElement.NoOfColumns;
				int itemWidth = width / noOfColumns - spacing;

				collectionView.SetNumColumns(noOfColumns);
				collectionView.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
				collectionView.SetHorizontalSpacing(spacing);
				collectionView.SetVerticalSpacing(spacing);

				this.Unbind(e.OldElement);
				this.Bind(e.NewElement);

				_adapter = new GridViewAdapter(context, collectionView, Element, itemWidth);
				collectionView.Adapter = _adapter;
				collectionView.ItemClick += CollectionViewItemClick;

				base.SetNativeControl(collectionView);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex?.Message);
			}
		}

		void CollectionViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = this.Element.ItemsSource.Cast<object>().ElementAt(e.Position);
			this.Element.InvokeItemSelectedEvent(this, item);
        }

        private void Unbind(XamGridView oldElement)
        {
            if (oldElement != null)
            {
                oldElement.PropertyChanged -= ElementPropertyChanged;
                if (oldElement.ItemsSource is INotifyCollectionChanged)
                {
                    (oldElement.ItemsSource as INotifyCollectionChanged).CollectionChanged -= DataCollectionChanged;
                }
            }
        }

        private void Bind(XamGridView newElement)
        {
            if (newElement != null)
            {
                newElement.PropertyChanging += ElementPropertyChanging;
                if (newElement.ItemsSource is INotifyCollectionChanged)
                {
                    (newElement.ItemsSource as INotifyCollectionChanged).CollectionChanged += DataCollectionChanged;
                }
            }
        }

        private void ElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == XamGridView.ItemsSourceProperty.PropertyName)
            {
                if (Element.ItemsSource is INotifyCollectionChanged)
                {
                    (Element.ItemsSource as INotifyCollectionChanged).CollectionChanged += DataCollectionChanged;
                    ReloadData();
                }
            }
        }

        private void ElementPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName == XamGridView.ItemsSourceProperty.PropertyName)
            {
                if (this.Element.ItemsSource is INotifyCollectionChanged)
                {
                    (this.Element.ItemsSource as INotifyCollectionChanged).CollectionChanged -= DataCollectionChanged;
                }
            }
        }

        private void DataCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            if (_adapter != null)
                _adapter.NotifyDataSetChanged();
        }

        private GridViewAdapter _adapter;

        public global::Android.Views.View GetCell(int position, global::Android.Views.View convertView, ViewGroup parent)
        {
            var item = this.Element.ItemsSource.Cast<object>().ElementAt(position);
            var viewCellBinded = (Element.ItemTemplate.CreateContent() as ViewCell);
            viewCellBinded.BindingContext = item;
            return CellFactory.GetCell(viewCellBinded, convertView, parent, Context, Element);
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        void CleanUpAdapter()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
                _adapter = null;
            } 
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CleanUpAdapter();

                if (Element != null)
                {
                    //TODO: Clear all object - Harshad
                    //if (Element.ItemsSource != null && Element.ItemsSource is INotifyCollectionChanged)
                    //((INotifyCollectionChanged)Element.ItemsSource).CollectionChanged -= item;
                }

            }

            try
            {
                base.Dispose(disposing);
                GC.Collect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        } 
    }

    /// <summary>
    /// Class GridDataSource.
    /// </summary>
    public class GridViewAdapter : CellAdapter
    {
        Context _context;
        private readonly AGridView _aView;
        private readonly XamGridView _view;
        private readonly int _itemWidth;
        public GridViewAdapter(Context context, AGridView aView, XamGridView view, int itemWidth) : base(context)
        {
            _context = context;
            _aView = aView;
            _view = view;
            _itemWidth = itemWidth;
        }

        public override int Count
        {
            get
            {
                if (_view.ItemsSource != null)
                    return (_view.ItemsSource as ICollection).Count;
                return 0;
            }
        }

        public override object this[int position]
        {
            get
            {
                var cell = GetCellForPosition(position);
                return cell;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override global::Android.Views.View GetView(int position, global::Android.Views.View convertView, ViewGroup parent)
        {
            var cell = GetCellForPosition(position);
            var renderer = new GridViewCellRenderer();
            var view = renderer.GetCell(cell, convertView, parent, _context);
            view.LayoutParameters = new AGridView.LayoutParams(_itemWidth, _itemWidth);  
            return view;
        }

        protected override Cell GetCellForPosition(int position)
        {
            var item = _view.ItemsSource.Cast<object>().ElementAt(position);
            var cell = (_view.ItemTemplate.CreateContent() as ViewCell);
            cell.BindingContext = item;

            return cell;
        }
    }

    /// <summary>
    /// Class GridViewCellRenderer.
    /// </summary>
    public class GridViewCellRenderer : CellRenderer
    {
        protected override global::Android.Views.View GetCellCore(Cell item, global::Android.Views.View convertView, ViewGroup parent, Context context)
        {
            ViewCell viewCell = (ViewCell)item;
            var viewCellContainer = convertView as GridViewCellContainer;
            //if (viewCellContainer != null)
            //{
            //    // reuse existing container
            //    viewCellContainer.Update(viewCell);
            //    return viewCellContainer;
            //}

            IVisualElementRenderer renderer = Xamarin.Forms.Platform.Android.Platform.CreateRenderer(viewCell.View);
            return new GridViewCellContainer(context, renderer, viewCell, parent);
        }


        /// <summary>
        /// Class ViewCellContainer.
        /// </summary>
        private class GridViewCellContainer : ViewGroup
        {
            private IVisualElementRenderer _renderer;
            private global::Android.Views.View _parent;
            private ViewCell _viewCell;

            public GridViewCellContainer(Context context, IVisualElementRenderer renderer, ViewCell viewCell, global::Android.Views.View parent) : base(context)
            {
                _renderer = renderer;
                _parent = parent;
                _viewCell = viewCell;
                RemoveAllViews();
                AddView(renderer.ViewGroup);
            }

            public void Update(ViewCell cell)
            {
                IVisualElementRenderer visualElementRenderer = this.GetChildAt(0) as IVisualElementRenderer;
            }

            protected override void OnLayout(bool changed, int l, int t, int r, int b)
            {
                double width = Context.FromPixels(r - l);
                double height = Context.FromPixels(b - t);
                _renderer.Element.Layout(new Rectangle(0, 0, width, height));
                _renderer.UpdateLayout();
            }

            protected override void Dispose(bool disposing)
            {

                try
                {
                    base.Dispose(disposing);

                    if (_renderer != null)
                    {
                        _renderer = null;
                        _viewCell = null;
                    }

                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }
    }
}