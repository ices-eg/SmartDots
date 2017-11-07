using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartDots.View.UserControls
{
    public class CleanableComboBox : ComboBox
    {
        private int _hoveredIndex;

        public ObservableCollection<ComboBoxItem> ListItems { get; set; }
        public IEnumerable<string> ListItemsText => ListItems.Select(i => i.Content?.ToString());

        public CleanableComboBox() : base()
        {
            IsEditable = true;
            ListItems = new ObservableCollection<ComboBoxItem>();
            ItemsSource = ListItems;
            PreviewKeyDown += Field_KeyDown;
        }

        public void AddListItem(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var li = new ComboBoxItem();
            li.Content = text;
            li.MouseMove += Li_MouseEnter;
            ListItems.Add(li);
        }

        public void AddListItemRange(IEnumerable<string> list)
        {
            foreach (var obj in list) AddListItem(obj);
        }

        private void Li_MouseEnter(object sender, MouseEventArgs e)
        {
            _hoveredIndex = ListItems.IndexOf((ComboBoxItem)sender);
        }

        private void Field_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && IsDropDownOpen && ListItems.Any() && _hoveredIndex >= 0)
                ListItems.RemoveAt(_hoveredIndex);
        }

    }
}
