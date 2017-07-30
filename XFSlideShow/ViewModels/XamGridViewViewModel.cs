using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using XFSlideShow.Models;

namespace XFSlideShow.ViewModels
{
    public class XamGridViewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<XamGridModel> _xamMockDataList;
        public XamGridViewViewModel()
        {
            MockDataBind();
        }

        public ObservableCollection<XamGridModel> XamMockDataList
        {
            get
            {
                return _xamMockDataList;
            }
            set
            {
                _xamMockDataList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("XamMockDataList"));
                }
            }
        }

        void MockDataBind()
        {
            List<XamGridModel> lstImages = new List<XamGridModel>();
            for (int i = 0; i < 40; i++)
            {
                lstImages.Add(new XamGridModel()
                {
                    Position = i,
                    ImageUrl = "https://unsplash.it/300/300?image=" + (i+10)
                });
            }

            XamMockDataList = new ObservableCollection<XamGridModel>(lstImages);
        }
    }
}
