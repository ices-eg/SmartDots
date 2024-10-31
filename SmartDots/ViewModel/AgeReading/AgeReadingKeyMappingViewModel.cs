using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SmartDots.ViewModel
{
    public class AgeReadingKeyMappingViewModel : AgeReadingBaseViewModel
    {
        public void ToggleEdgeSettings(bool showEdgeColumn)
        {
            foreach (var edgeRow in AgeReadingViewModel.AgeReadingKeyMappingView.Grid.RowDefinitions.Where(x => x.Tag != null && x.Tag.ToString() == "EdgeRow"))
            {
                edgeRow.Height = showEdgeColumn ? new GridLength((double)30) : new GridLength((double)0);
            }

            //AgeReadingViewModel.AgeReadingKeyMappingView.Grid.InvalidateVisual();

            //EdgeRowHeight = showEdgeColumn ? new GridLength((double)30) : new GridLength((double)0);
        }
    }
}
