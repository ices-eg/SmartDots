using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SmartDots.ViewModel
{
    public class AgeReadingKeyMappingViewModel : AgeReadingBaseViewModel
    {
        public void ToggleEdgeSettings(bool showEdgeColumn, bool showQa)
        {
            foreach (var edgeRow in AgeReadingViewModel.AgeReadingKeyMappingView.Grid.RowDefinitions.Where(x => x.Tag != null && x.Tag.ToString().Contains("EdgeRow") && !x.Tag.ToString().Contains("QARow")))
            {
                edgeRow.Height = showEdgeColumn ? new GridLength((double)30) : new GridLength((double)0);
            }

            foreach (var qaRow in AgeReadingViewModel.AgeReadingKeyMappingView.Grid.RowDefinitions.Where(x => x.Tag != null && x.Tag.ToString().Contains("QARow") && !x.Tag.ToString().Contains("EdgeRow")))
            {
                qaRow.Height = showQa ? new GridLength((double)30) : new GridLength((double)0);
            }

            foreach (var qaEdgeRow in AgeReadingViewModel.AgeReadingKeyMappingView.Grid.RowDefinitions.Where(x => x.Tag != null && x.Tag.ToString().Contains("QARow") && x.Tag.ToString().Contains("EdgeRow")))
            {
                qaEdgeRow.Height = showQa && showEdgeColumn ? new GridLength((double)30) : new GridLength((double)0);
            }

            //AgeReadingViewModel.AgeReadingKeyMappingView.Grid.InvalidateVisual();

            //EdgeRowHeight = showEdgeColumn ? new GridLength((double)30) : new GridLength((double)0);
        }
    }
}
