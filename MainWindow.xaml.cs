using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Utilities;

namespace SimpleTicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Set up window parameters for click-through, transparency, click and drag, etc
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const uint WS_EX_LAYERED = 0x00080000;
        public const uint WS_EX_TRANSPARENT = 0x00000020;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hWnd, GWL nIndex);
        [DllImport("user32.dll")]
        public static extern uint SetWindowLong(IntPtr hWnd, GWL nIndex, uint dwNewLong);
        public enum GWL { ExStyle = -20 }

        private KeyboardHook kh;

        public int RefreshTimerInterval { get; set; } = 5;
        private int counter = 0;

        public MainWindow()
        {
            InitializeComponent();

            Top = 100;
            Left = -400;

            // Set up global keyboard hook
            kh = new KeyboardHook { SelectedKey = Key.LeftAlt };
            kh.KeyCombinationPressed += GlobalKeyHook;

            // Initialize and start TopmostRefreshTimer
            //DispatcherTimer TopmostRefreshTimer = new DispatcherTimer();
            //TopmostRefreshTimer.Tick += new EventHandler(TopmostRefreshTimer_Tick);
            //TopmostRefreshTimer.Interval = new TimeSpan(0, 0, RefreshTimerInterval);
            //TopmostRefreshTimer.Start();

            // Initialize window in click-through mode
            KeyboardHook.ActivateWindow(this);
            uint ex_style = GetWindowLong(new WindowInteropHelper(this).Handle, GWL.ExStyle);
            SetWindowLong(new WindowInteropHelper(this).Handle, GWL.ExStyle, ex_style | WS_EX_LAYERED | WS_EX_TRANSPARENT);

        }

        private void TopmostRefreshTimer_Tick(object sender, EventArgs e)
        {
            lblHello.Content = counter++;
            Topmost = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //if (Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            lblHello.Content = "dragging";
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            DragMove();

            lblHello.Content = "stopped dragging";

            //}

            // Set window back to transparent (click-through)
            uint ex_style = GetWindowLong(new WindowInteropHelper(this).Handle, GWL.ExStyle);
            SetWindowLong(new WindowInteropHelper(this).Handle, GWL.ExStyle, ex_style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        private void GlobalKeyHook(object sender, EventArgs e)
        {
            lblHello.Content = "GlobalKeyHook called";
            Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(ShowMainWindow));

            uint ex_style = GetWindowLong(new WindowInteropHelper(this).Handle, GWL.ExStyle);
            SetWindowLong(new WindowInteropHelper(this).Handle, GWL.ExStyle, ex_style & WS_EX_LAYERED);
        }

        private void ShowMainWindow()
        {
            KeyboardHook.ActivateWindow(this);
        }
    }
}
