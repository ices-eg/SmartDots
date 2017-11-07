using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace SmartDots.Helpers
{
    public class VisibilityAnimation
    {
        public enum AnimationType
        {
            None,
            Fade
        }

        private const int AnimationDuration = 300;
        private static readonly Dictionary<FrameworkElement, bool> _hookedElements = new Dictionary<FrameworkElement, bool>();
        public static readonly DependencyProperty AnimationTypeProperty = DependencyProperty.RegisterAttached("AnimationType", typeof(AnimationType),
            typeof(VisibilityAnimation), new FrameworkPropertyMetadata(AnimationType.None, new PropertyChangedCallback(OnAnimationTypePropertyChanged)));

        static VisibilityAnimation()
        {
            //UIElement.VisibilityProperty.AddOwner(
            //    typeof(FrameworkElement), new FrameworkPropertyMetadata(Visibility.Visible, VisibilityChanged, CoerceVisibility));
        }

        public static AnimationType GetAnimationType(DependencyObject obj)
        {
            return (AnimationType)obj.GetValue(AnimationTypeProperty);
        }

        public static void SetAnimationType(DependencyObject obj, AnimationType value)
        {
            obj.SetValue(AnimationTypeProperty, value);
        }

        private static void OnAnimationTypePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement == null) return;

            if (GetAnimationType(frameworkElement) != AnimationType.None) HookVisibilityChanges(frameworkElement);
            else UnHookVisibilityChanges(frameworkElement);
        }

        private static void HookVisibilityChanges(FrameworkElement frameworkElement)
        {
            _hookedElements.Add(frameworkElement, false);
        }

        private static void UnHookVisibilityChanges(FrameworkElement frameworkElement)
        {
            if (_hookedElements.ContainsKey(frameworkElement)) _hookedElements.Remove(frameworkElement);
        }

        private static void VisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) { }

        private static object CoerceVisibility(DependencyObject dependencyObject, object baseValue)
        {
            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement == null) return baseValue;

            var visibility = (Visibility)baseValue;

            if (visibility == frameworkElement.Visibility) return baseValue;
            if (!IsHookedElement(frameworkElement)) return baseValue;
            if (UpdateAnimationStartedFlag(frameworkElement)) return baseValue;

            var doubleAnimation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration)) };
            doubleAnimation.Completed += (sender, eventArgs) =>
            {
                if (visibility == Visibility.Visible) UpdateAnimationStartedFlag(frameworkElement);
                else if (BindingOperations.IsDataBound(frameworkElement, UIElement.VisibilityProperty))
                {
                    var bindingValue = BindingOperations.GetBinding(frameworkElement, UIElement.VisibilityProperty);
                    BindingOperations.SetBinding(frameworkElement, UIElement.VisibilityProperty, bindingValue);
                }
                else frameworkElement.Visibility = visibility;
            };

            if (visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
            {
                doubleAnimation.From = 1.0;
                doubleAnimation.To = 0.0;
            }
            else
            {
                doubleAnimation.From = 0.0;
                doubleAnimation.To = 1.0;
            }
            frameworkElement.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
            return Visibility.Visible;
        }

        private static bool IsHookedElement(FrameworkElement frameworkElement)
        {
            return _hookedElements.ContainsKey(frameworkElement);
        }

        private static bool UpdateAnimationStartedFlag(FrameworkElement frameworkElement)
        {
            var animationStarted = _hookedElements[frameworkElement];
            _hookedElements[frameworkElement] = !animationStarted;
            return animationStarted;
        }
    }
}
