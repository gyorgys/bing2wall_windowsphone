using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace BingWall
{
    public class FlyoutAnimation
    {
        private DoubleAnimation translateAnimation;
        private Storyboard flyOutStoryBoard;

        private List<FrameworkElement> elements;
        private double relativePos = 0;

        TranslateTransform transform = new TranslateTransform();

        private bool appearWhenDone;
        public bool IsRunning { get; private set; }

        public FlyoutAnimation()
        {
            elements = new List<FrameworkElement>();

            flyOutStoryBoard = new Storyboard();

            translateAnimation = new DoubleAnimation();
            translateAnimation.Duration = TimeSpan.FromMilliseconds(250);
            Storyboard.SetTargetProperty(translateAnimation, new PropertyPath(TranslateTransform.XProperty));
            flyOutStoryBoard.Children.Add(translateAnimation);

            flyOutStoryBoard.Completed += new EventHandler(flyOutStoryBoard_Completed);
        }

        public event EventHandler Completed;

        void flyOutStoryBoard_Completed(object sender, EventArgs e)
        {
            IsRunning = false;
            flyOutStoryBoard.Stop();
            relativePos = 0;

            //if (appearWhenDone)
            {
                ResetElements();
            }
            if (Completed != null) this.Completed(this, null);
        }

        private void ResetElements()
        {
            transform = new TranslateTransform();

            foreach (FrameworkElement element in elements)
            {
                element.RenderTransform = null;
                element.Opacity = 1;
            }
        }

        public void Attach(FrameworkElement element)
        {
            elements.Add(element);
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(300);
           // opacityAnimation.BeginTime = TimeSpan.FromMilliseconds(100);
            opacityAnimation.To = 0;
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(FrameworkElement.OpacityProperty));
            flyOutStoryBoard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, element);
        }

        public void FlyoutLeft()
        {
            if (IsRunning)
            {
                IsRunning = false;
                flyOutStoryBoard.Stop();
                if (Completed != null) this.Completed(this, null);
            }

            translateAnimation.Duration = TimeSpan.FromMilliseconds(250 * (1-relativePos));

            appearWhenDone = false;
            translateAnimation.To = -300;
            SetAnimationTargets();
            flyOutStoryBoard.Begin();
            IsRunning = true;
        }

        public void FlyoutRight()
        {
            if (IsRunning)
            {
                IsRunning = false;
                flyOutStoryBoard.Stop();
                if (Completed != null) this.Completed(this, null);
            }

            translateAnimation.Duration = TimeSpan.FromMilliseconds(250 * (1 - relativePos));

            appearWhenDone = false;
            translateAnimation.To = 300;
            SetAnimationTargets();
            flyOutStoryBoard.Begin();
            IsRunning = true;
        }

        public void SetPosition(double pos)
        {
            if (pos == 0)
            {
                ResetElements();
                return;
            }

            if (pos < -300) pos = -300;
            if (pos > 300) pos = 300;
            relativePos = (Math.Abs(pos) / 300);
            double opacity = 1- relativePos;

            foreach (FrameworkElement element in elements)
            {
                element.RenderTransform = transform;
                element.Opacity = opacity;
            }

            transform.X = pos;
        }

        public void Appear()
        {
            appearWhenDone = true;
            if (!IsRunning) ResetElements();
        }

        private void SetAnimationTargets()
        {
            Storyboard.SetTarget(translateAnimation, transform);
            foreach (FrameworkElement element in elements)
            {
                element.RenderTransform = transform;
            }
        }

    }
}
