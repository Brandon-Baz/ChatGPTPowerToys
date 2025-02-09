using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ChatGPT.StandaloneApp.Helpers
{
    public static class WindowHelper
    {
        #region Win32 API Constants and Methods

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion

        /// <summary>
        /// Sets the specified window to always stay on top of other windows.
        /// </summary>
        /// <param name="window">The window to set as always on top.</param>
        public static void SetWindowAlwaysOnTop(Window window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));

            var hwnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        /// <summary>
        /// Removes the always-on-top property from the specified window.
        /// </summary>
        /// <param name="window">The window to remove the always-on-top property from.</param>
        public static void RemoveWindowAlwaysOnTop(Window window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));

            var hwnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        /// <summary>
        /// Gets the position of the currently active (foreground) window.
        /// </summary>
        /// <returns>A <see cref="Rect"/> representing the position and size of the active window, or <c>null</c> if no active window is found.</returns>
        public static Rect? GetActiveWindowPosition()
        {
            var hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero) return null;

            if (GetWindowRect(hwnd, out RECT rect))
            {
                return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }

            return null;
        }

        /// <summary>
        /// Updates the position of a floating window to follow the active window.
        /// </summary>
        /// <param name="floatingWindow">The floating window to reposition.</param>
        /// <param name="offsetX">The horizontal offset from the active window.</param>
        /// <param name="offsetY">The vertical offset from the active window.</param>
        public static void UpdateFloatingWindowPosition(Window floatingWindow, double offsetX, double offsetY)
        {
            if (floatingWindow == null) throw new ArgumentNullException(nameof(floatingWindow));

            var activeWindowPosition = GetActiveWindowPosition();
            if (activeWindowPosition.HasValue)
            {
                floatingWindow.Left = activeWindowPosition.Value.Right + offsetX;
                floatingWindow.Top = activeWindowPosition.Value.Top + offsetY;
            }
        }
    }
}
