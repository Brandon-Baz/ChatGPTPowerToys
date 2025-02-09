using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace ChatGPT.StandaloneApp.Controls
{
    public partial class FloatingBubble : UserControl
    {
        private Window _parentWindow;
        private bool _isExpanded;

        public FloatingBubble()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Attach to the parent window
            _parentWindow = Window.GetWindow(this);
            if (_parentWindow != null)
            {
                _parentWindow.LocationChanged += OnParentWindowLocationChanged;
                _parentWindow.SizeChanged += OnParentWindowSizeChanged;
                _parentWindow.Activated += OnParentWindowActivated;
                _parentWindow.Deactivated += OnParentWindowDeactivated;
            }

            // Ensure the bubble stays on top
            SetWindowAlwaysOnTop();
        }

        private void OnParentWindowLocationChanged(object sender, EventArgs e)
        {
            UpdateBubblePosition();
        }

        private void OnParentWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateBubblePosition();
        }

        private void OnParentWindowActivated(object sender, EventArgs e)
        {
            Opacity = 1.0; // Fully visible when active
        }

        private void OnParentWindowDeactivated(object sender, EventArgs e)
        {
            Opacity = 0.7; // Slightly transparent when inactive
        }

        private void UpdateBubblePosition()
        {
            if (_parentWindow == null) return;

            var parentPosition = _parentWindow.PointToScreen(new Point(0, 0));
            var bubbleOffset = new Point(parentPosition.X + _parentWindow.Width - 100, parentPosition.Y + 50);

            // Position the bubble near the top-right corner of the parent window
            Canvas.SetLeft(this, bubbleOffset.X);
            Canvas.SetTop(this, bubbleOffset.Y);
        }

        private void SetWindowAlwaysOnTop()
        {
            WindowHelper.SetWindowAlwaysOnTop(_parentWindow);
        }

        private void Bubble_Click(object sender, MouseButtonEventArgs e)
        {
            if (_isExpanded)
            {
                CollapseBubble();
            }
            else
            {
                ExpandBubble();
            }
        }

        private void ExpandBubble()
        {
            _isExpanded = true;

            // Show modal or panel
            var modal = new ModalWindow
            {
                Owner = _parentWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            modal.ShowDialog();
        }

        private void CollapseBubble()
        {
            _isExpanded = false;

            // Collapse logic (if any additional UI changes are needed)
        }

        #region Win32 API for Always-On-Top
        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        #endregion
    }
}
