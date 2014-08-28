using System.ComponentModel;
using System.Windows.Media;

namespace Kindle_Quick.Entity
{
    class Program : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Name"));
                    // 通知Binding是“Age”这个属性的值改变了
                }
            }
        }
        private string _pingYing;

        public string PingYing
        {
            get { return _pingYing; }
            set
            {
                _pingYing = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PingYing"));
                    // 通知Binding是“Age”这个属性的值改变了
                }
            }
        }
        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                _link = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Link"));
                    // 通知Binding是“Age”这个属性的值改变了
                }
            }
        }
        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Icon"));
                    // 通知Binding是“Age”这个属性的值改变了
                }
            }
        }
    }
}
