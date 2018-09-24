using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SmartDots.Helpers;
using SmartDots.Model;

namespace SmartDots.ViewModel.AgeReading
{
    public class AgeReadingSampleViewModel : AgeReadingBaseViewModel
    {
        private Sample sample;
        private string propertyValues;
        

        public Sample Sample
        {
            get { return sample; }
            set
            {
                sample = value;

                RaisePropertyChanged("Sample");
                AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
                AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
            }
        }

        public string PropertyValues
        {
            get { return propertyValues; }
            set
            {
                propertyValues = value;
                RaisePropertyChanged("PropertyValues");
            }
        }

        public void SetSample()
        {
            EmptyValues();
            if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Sample != null)
            {
                Sample = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Sample;
                
            }
            else
            {
                Sample = null;
            }
           GetAllPropertiesWithValues();
        }


        public void GetAllPropertiesWithValues()
        {
            if (Sample != null && Sample.DisplayedProperties != null)
            {
                    AgeReadingViewModel.AgeReadingSampleView.Grid.Dispatcher.Invoke(() =>
                    {
                        AgeReadingViewModel.AgeReadingSampleView.Grid.RowDefinitions.Clear();
                        int startpos = 0;
                        foreach (KeyValuePair<string, string> pair in sample.DisplayedProperties)
                        {
                            AgeReadingViewModel.AgeReadingSampleView.Grid.RowDefinitions.Add(new RowDefinition()
                            {
                                Height = new GridLength(20)
                            });

                            Label key = new Label() { Content = pair.Key, Padding = new Thickness(0), FontWeight = FontWeights.Bold };
                            Label value = new Label() { Content = pair.Value, Padding = new Thickness(0) };

                            Grid.SetColumn(key, 0);
                            Grid.SetRow(key, startpos);

                            Grid.SetColumn(value, 1);
                            Grid.SetRow(value, startpos);
                            AgeReadingViewModel.AgeReadingSampleView.Grid.Children.Add(key);
                            AgeReadingViewModel.AgeReadingSampleView.Grid.Children.Add(value);
                            startpos++;
                        }
                        //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.AllowEditing = !Sample.StatusCode.Equals("Completed");
                    });
            }
        }

        public void EmptyValues()
        {
            AgeReadingViewModel.AgeReadingSampleView.Grid.Dispatcher.Invoke(() =>
            {
                foreach (var label in AgeReadingViewModel.AgeReadingSampleView.Grid.Children.OfType<Label>().ToList())
                {
                    AgeReadingViewModel.AgeReadingSampleView.Grid.Children.Remove(label);
                }
            });
        }
    }
}
