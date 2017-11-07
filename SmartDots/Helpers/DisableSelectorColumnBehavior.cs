using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace SmartDots.Helpers
{
    public class DisableSelectorColumnBehavior : Behavior<TableView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.CheckBoxSelectorColumnHeaderTemplate = new DataTemplate();
            AssociatedObject.PreviewMouseDown += OnViewPreviewMouseDown;
        }

        protected virtual void OnViewPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var hi = AssociatedObject.CalcHitInfo((DependencyObject)e.OriginalSource);

            if (hi.InColumnHeader && hi.Column.FieldName == "DX$CheckboxSelectorColumn")
                e.Handled = true;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDown -= OnViewPreviewMouseDown;
            base.OnDetaching();
        }
    }
}
