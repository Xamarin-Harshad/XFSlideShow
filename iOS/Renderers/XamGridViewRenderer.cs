using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XFSlideShow.iOS;
using XFSlideShow.Controls;

[assembly: ExportRenderer(typeof(XamGridView), typeof(XamGridViewRenderer))]
namespace XFSlideShow.iOS
{
    public class XamGridViewRenderer : ViewRenderer<XamGridView, GridCollectionView>
    {
        private GridDataSource _dataSource;

        public XamGridViewRenderer()
        {
        }

        public int RowsInSection(UICollectionView collectionView, nint section)
        {
            return ((ICollection)this.Element.ItemsSource).Count;
        }

        public void ItemSelected(UICollectionView tableView, NSIndexPath indexPath)
        {
            var item = this.Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
            this.Element.InvokeItemSelectedEvent(this, item);
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = this.Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
            var viewCellBinded = (this.Element.ItemTemplate.CreateContent() as ViewCell);
            if (viewCellBinded == null) return null;

            viewCellBinded.BindingContext = item;
            return this.GetCell(collectionView, viewCellBinded, indexPath);
        }

        protected virtual UICollectionViewCell GetCell(UICollectionView collectionView, ViewCell item, NSIndexPath indexPath)
        {
            var collectionCell = collectionView.DequeueReusableCell(new NSString(GridViewCell.Key), indexPath) as GridViewCell;

            if (collectionCell == null) return null;

            collectionCell.ViewCell = item;

            return collectionCell;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XamGridView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                Unbind(e.OldElement);
            }
            if (e.NewElement != null)
            {
                var margin = ((int)e.NewElement.Margin.Left == 0) ? (int)e.NewElement.Margin.Right : (int)e.NewElement.Margin.Left;
                var padding = ((int)e.NewElement.Padding.Left == 0) ? (int)e.NewElement.Padding.Right : (int)e.NewElement.Padding.Left;
                var spacing = (int)e.NewElement.ColumnSpacing - 2;
                int noOfColumns = (int)e.NewElement.NoOfColumns;
                var itemWidth = UIScreen.MainScreen.Bounds.Width / noOfColumns - spacing - padding - margin;
                var itemHeight = itemWidth;
                if (Control == null)
                {
                    var collectionView = new GridCollectionView()
                    {
                        AllowsMultipleSelection = false,
                        SelectionEnable = e.NewElement.SelectionEnabled,
                        ContentInset = new UIEdgeInsets((float)this.Element.Padding.Top, (float)this.Element.Padding.Left, (float)this.Element.Padding.Bottom, (float)this.Element.Padding.Right),
                        BackgroundColor = this.Element.BackgroundColor.ToUIColor(),
                        ItemSize = new CoreGraphics.CGSize(itemWidth, itemHeight),
                        RowSpacing = this.Element.RowSpacing,
                        ColumnSpacing = this.Element.ColumnSpacing
                    };

                    Bind(e.NewElement);

                    collectionView.Source = this.DataSource;
                    SetNativeControl(collectionView);
                }
            }
        }
        private void Unbind(XamGridView oldElement)
        {
            if (oldElement == null) return;

            oldElement.PropertyChanged -= this.ElementPropertyChanged;

            var itemsSource = oldElement.ItemsSource as INotifyCollectionChanged;
            if (itemsSource != null)
            {
                itemsSource.CollectionChanged -= this.DataCollectionChanged;
            }
        }

        private void Bind(XamGridView newElement)
        {
            if (newElement == null) return;

            newElement.PropertyChanged += this.ElementPropertyChanged;

            var source = newElement.ItemsSource as INotifyCollectionChanged;
            if (source != null)
            {
                source.CollectionChanged += this.DataCollectionChanged;
            }
        }

        private void ElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == XamGridView.ItemsSourceProperty.PropertyName)
            {
                var newItemsSource = this.Element.ItemsSource as INotifyCollectionChanged;
                if (newItemsSource != null)
                {
                    newItemsSource.CollectionChanged += DataCollectionChanged;
                    this.Control.ReloadData();
                }
            }
            else if (e.PropertyName == "NoOfColumns")
            {
                var margin = ((int)this.Element.Margin.Left == 0) ? (int)this.Element.Margin.Right : (int)this.Element.Margin.Left;
				var padding = ((int)this.Element.Padding.Left == 0) ? (int)this.Element.Padding.Right : (int)this.Element.Padding.Left;
				var spacing = (int)this.Element.ColumnSpacing - 2;
				int noOfColumns = (int)this.Element.NoOfColumns;
				var itemWidth = UIScreen.MainScreen.Bounds.Width / noOfColumns - spacing - padding - margin;
                var itemHeight = itemWidth;
                this.Control.ItemSize = new CoreGraphics.CGSize(itemWidth, itemHeight);
            }
            //TODO: inprogress. 
            //         else if (e.PropertyName == "ItemWidth" || e.PropertyName == "ItemHeight") {
            //	this.Control.ItemSize = new CoreGraphics.CGSize ((float)this.Element.ItemWidth, (float)this.Element.ItemHeight);
            //}
        }

        private void ElementPropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
            {
                var oldItemsSource = this.Element.ItemsSource as INotifyCollectionChanged;
                if (oldItemsSource != null)
                {
                    oldItemsSource.CollectionChanged -= DataCollectionChanged;
                }
            }
        }

        private void DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    if (this.Control == null)
                        return;
                    this.Control.ReloadData();
                }
                catch
                {

                }
            });
        }

        private GridDataSource DataSource
        {
            get
            {
                return _dataSource ?? (_dataSource = new GridDataSource(GetCell, RowsInSection, ItemSelected));
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _dataSource != null)
            {
                Unbind(Element);
                _dataSource.Dispose();
                _dataSource = null;
            }
        }
    }

    public class GridDataSource : UICollectionViewSource
    {
        public delegate UICollectionViewCell OnGetCell(UICollectionView collectionView, NSIndexPath indexPath);

        public delegate int OnRowsInSection(UICollectionView collectionView, nint section);

        public delegate void OnItemSelected(UICollectionView collectionView, NSIndexPath indexPath);

        private readonly OnGetCell _onGetCell;

        private readonly OnRowsInSection _onRowsInSection;

        private readonly OnItemSelected _onItemSelected;

        public GridDataSource(OnGetCell onGetCell, OnRowsInSection onRowsInSection, OnItemSelected onItemSelected)
        {
            this._onGetCell = onGetCell;
            this._onRowsInSection = onRowsInSection;
            this._onItemSelected = onItemSelected;
        }

        #region implemented abstract members of UICollectionViewDataSource

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this._onRowsInSection(collectionView, section);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this._onItemSelected(collectionView, indexPath);
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = this._onGetCell(collectionView, indexPath);

            if (((GridCollectionView)collectionView).SelectionEnable)
            {
                cell.AddGestureRecognizer(new UITapGestureRecognizer(v =>
                {
                    ItemSelected(collectionView, indexPath);
                }));
            }
            else
            {
                cell.SelectedBackgroundView = new UIView();
            }

            return cell;
        }

        #endregion

    }

    public class GridCollectionView : UICollectionView
    {
        public GridCollectionView() : this(default(CGRect))
        {
        }

        public GridCollectionView(CGRect frm)
            : base(frm, new UICollectionViewFlowLayout())
        {
            AutoresizingMask = UIViewAutoresizing.All;
            ContentMode = UIViewContentMode.ScaleToFill;
            RegisterClassForCell(typeof(GridViewCell), new NSString(GridViewCell.Key));
        }

        public bool SelectionEnable
        {
            get;
            set;
        }

        public double RowSpacing
        {
            get
            {
                return ((UICollectionViewFlowLayout)this.CollectionViewLayout).MinimumLineSpacing;
            }
            set
            {
                ((UICollectionViewFlowLayout)this.CollectionViewLayout).MinimumLineSpacing = (nfloat)value;
            }
        }

        public double ColumnSpacing
        {
            get
            {
                return ((UICollectionViewFlowLayout)this.CollectionViewLayout).MinimumInteritemSpacing;
            }
            set
            {
                ((UICollectionViewFlowLayout)this.CollectionViewLayout).MinimumInteritemSpacing = (nfloat)value;
            }
        }

        public CGSize ItemSize
        {
            get
            {
                return ((UICollectionViewFlowLayout)this.CollectionViewLayout).ItemSize;
            }
            set
            {
                ((UICollectionViewFlowLayout)this.CollectionViewLayout).ItemSize = value;
            }
        }

        public override UICollectionViewCell CellForItem(NSIndexPath indexPath)
        {
            if (indexPath == null)
            {
                return null;
            }
            return base.CellForItem(indexPath);
        }

        public override void Draw(CGRect rect)
        {
            this.CollectionViewLayout.InvalidateLayout();

            base.Draw(rect);
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            return ItemSize;
        }
    }

    public class GridViewCell : UICollectionViewCell
    {

        public const string Key = "GridViewCell";


        private ViewCell _viewCell;

        private UIView _view;

        [Export("initWithFrame:")]
        public GridViewCell(CGRect frame) : base(frame)
        {

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var frame = this.ContentView.Frame;
            frame.X = (this.Bounds.Width - frame.Width) / 2;
            frame.Y = (this.Bounds.Height - frame.Height) / 2;
            this.ViewCell.View.Layout(frame.ToRectangle());
            this._view.Frame = frame;
        }

        public ViewCell ViewCell
        {
            get
            {
                return this._viewCell;
            }
            set
            {
                if (this._viewCell != value)
                {
                    UpdateCell(value);
                }
            }
        }


        private void UpdateCell(ViewCell cell)
        {
            if (this._viewCell != null)
            {
                //this.viewCell.SendDisappearing ();
                this._viewCell.PropertyChanged -= this.HandlePropertyChanged;
            }

            this._viewCell = cell;
            this._viewCell.PropertyChanged += this.HandlePropertyChanged;
            //this.viewCell.SendAppearing ();
            UpdateView();
        }


        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (this._view != null)
            {
                this._view.RemoveFromSuperview();
            }

            this._view = RendererFactory.GetRenderer(this._viewCell.View).NativeView;
            this._view.AutoresizingMask = UIViewAutoresizing.All;
            this._view.ContentMode = UIViewContentMode.ScaleToFill;

            AddSubview(this._view);
        }
    }
}