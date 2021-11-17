using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using DesktopWPFAppLowLevelKeyboardHook;
using System.Collections.Generic;

namespace KeyStrokeVisualizer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const int MaxDisplayLength = 28;

		private LowLevelKeyboardListener _listener;
		private System.Timers.Timer _timer;
		private int _timeSinceLastKeyStroke;
		private List<int> _currentKeys;
		private bool _replaceNext;

		public MainWindow()
		{
			_timeSinceLastKeyStroke = 0;
		}

		private void _listener_OnKeyUp(object sender, KeyUpArgs e)
		{
			if (_replaceNext)
			{
				textBlock_Input.Text = "";
				_replaceNext = false;
			}

			int hashCode = e.KeyPressed.GetHashCode();
			string keyText;

			if (hashCode == 118 || hashCode == 119 || hashCode == 120 || hashCode == 121 || hashCode == 70 || hashCode == 71)
			{
				keyText = "";
				_replaceNext = true;
			}
			// Return/Enter
			else if (hashCode == 6)
			{
				textBlock_Input.Text = "";
				_timeSinceLastKeyStroke = 0;
				return;
			}
			// Escape
			else if (hashCode == 13)
			{
				keyText = "esc";
				_replaceNext = true;
			}
			// If Shift is release clear it form the list of held keys and return.
			else if (hashCode == 116 || hashCode == 117)
			{
				_currentKeys.Remove(hashCode);
				return;
			}
			// Caps Lock
			else if (hashCode == 8)
			{
				_currentKeys.Remove(hashCode);
				return;
			}
			// Colon/Semi Colon
			else if (hashCode == 140)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = ":";
				else keyText = ";";
			}
			// Add/Equals
			else if (hashCode == 141)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "+";
				else keyText = "=";
			}
			// Comma/Less Than
			else if (hashCode == 142)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "<";
				else keyText = ",";
			}
			// Minus/Underscore
			else if (hashCode == 143)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "_";
				else keyText = "-";
			}
			// Period/Greator Than
			else if (hashCode == 143)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = ">";
				else keyText = ".";
			}
			// Slash/Question Mark
			else if (hashCode == 145)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "?";
				else keyText = "/";
			}
			// Accent/Tilde
			else if (hashCode == 146)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "~";
				else keyText = "`";
			}
			// Open Square Brackes/Curly Brackets
			else if (hashCode == 149)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "{";
				else keyText = "[";
			}
			// Back Slash/Pipe
			else if (hashCode == 150)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "|";
				else keyText = "\\";
			}
			// Close Brackets
			else if (hashCode == 151)
			{
				if (_currentKeys.Contains(116) || _currentKeys.Contains(117)) keyText = "}";
				else keyText = "]";
			}
			// Numbers
			else if (hashCode == 34 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = ")";
			else if (hashCode == 35 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "!";
			else if (hashCode == 36 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "@";
			else if (hashCode == 37 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "#";
			else if (hashCode == 38 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "$";
			else if (hashCode == 39 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "%";
			else if (hashCode == 40 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "^";
			else if (hashCode == 41 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "&";
			else if (hashCode == 42 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "*";
			else if (hashCode == 43 && (_currentKeys.Contains(116) || _currentKeys.Contains(117))) keyText = "(";
			else if (hashCode == 34) keyText = "0";
			else if (hashCode == 35) keyText = "1";
			else if (hashCode == 36) keyText = "2";
			else if (hashCode == 37) keyText = "3";
			else if (hashCode == 38) keyText = "4";
			else if (hashCode == 39) keyText = "5";
			else if (hashCode == 40) keyText = "6";
			else if (hashCode == 41) keyText = "7";
			else if (hashCode == 42) keyText = "8";
			else if (hashCode == 43) keyText = "9";
			// arrow keys
			else if (hashCode == 23)
			{
				textBlock_Input.Text = "";
				keyText = "⇐";
				_replaceNext = true;
			}
			else if (hashCode == 24)
			{
				textBlock_Input.Text = "";
				keyText = "⇑";
				_replaceNext = true;
			}
			else if (hashCode == 25)
			{
				textBlock_Input.Text = "";
				keyText = "⇒";
				_replaceNext = true;
			}
			else if (hashCode == 26)
			{
				textBlock_Input.Text = "";
				keyText = "⇓";
				_replaceNext = true;
			}
			// Backspace
			else if (hashCode == 2)
			{
				keyText = "";
			}
			// Space
			else if (hashCode == 18) keyText = " ";
			// Tab
			else if (hashCode == 3)
			{
				_replaceNext = true;
				keyText = "Tab";
			}
			// If Shift is held make the char upper case.
			else if ((_currentKeys.Contains(116) || _currentKeys.Contains(117)) && (hashCode != 116 && hashCode != 117))
			{
				textBlock_Input.Text += e.KeyPressed.ToString().ToUpper();
				_timeSinceLastKeyStroke = 0;
				return;
			}
			else keyText = e.KeyPressed.ToString().ToLower();

			// Remove characters from the beginning of the TextBlock's string
			// equal to the length of keyText so that the string never exceeds
			// its max length.
			if (textBlock_Input.Text.Length >= MaxDisplayLength)
            {
				textBlock_Input.Text = textBlock_Input.Text.Remove(0, keyText.Length) + keyText;
				_timeSinceLastKeyStroke = 0;
				return;
            }

			textBlock_Input.Text += keyText;
			_timeSinceLastKeyStroke = 0;
			_currentKeys.Remove(hashCode);
		}

		private void _listener_OnKeyDown(object? sender, KeyDownArgs e)
		{
			int hashCode = e.KeyPressed.GetHashCode();
			
			if (_currentKeys.Contains(118) ||
				_currentKeys.Contains(119) ||
				_currentKeys.Contains(120) ||
				_currentKeys.Contains(121) ||
				_currentKeys.Contains(70) ||
				_currentKeys.Contains(71))
			{
				if ((hashCode == 118 || hashCode == 119) && !(_currentKeys.Contains(118) || _currentKeys.Contains(119))) 
					textBlock_Input.Text += "ctrl ";
				else if (hashCode == 120 || hashCode == 121 && !(_currentKeys.Contains(120) || _currentKeys.Contains(121))) 
						textBlock_Input.Text += "alt ";
				else if (hashCode == 70 || hashCode == 71 && !(_currentKeys.Contains(70) || _currentKeys.Contains(71)))
						textBlock_Input.Text += "win ";
			}
			else if (hashCode == 118 || hashCode == 119 || hashCode == 120 || hashCode == 121 || hashCode == 70 || hashCode == 71)
			{
				textBlock_Input.Text = "";
				if (hashCode == 118 || hashCode == 119) textBlock_Input.Text += "ctrl ";
				else if (hashCode == 120 || hashCode == 121) textBlock_Input.Text += "alt ";
				else if (hashCode == 70 || hashCode == 71) textBlock_Input.Text += "win ";
			}
			// Backspace
			else if (hashCode == 2)
			{
				_currentKeys.Add(hashCode);
				if (textBlock_Input.Text.Length < 1) return;
				textBlock_Input.Text = textBlock_Input.Text.Remove(textBlock_Input.Text.Length - 1);
				return;
			}

			_currentKeys.Add(hashCode);
		}
		
		private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
			_timeSinceLastKeyStroke++;
			if (_timeSinceLastKeyStroke >= 8)
            {
				this.Dispatcher.Invoke(() =>
				{
					textBlock_Input.Text = "";
					_timeSinceLastKeyStroke = 0;
				});
				
            }
		}
        

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_listener = new LowLevelKeyboardListener();
			_listener.OnKeyUp += _listener_OnKeyUp;
            _listener.OnKeyDown += _listener_OnKeyDown;
			_listener.HookKeyboard();
			_timer = new(1000);
			_timer.Elapsed += _timer_Elapsed;
			_timer.AutoReset = true;
			_timer.Enabled = true;
			_currentKeys = new();
			Top = 14;
			Left = 14;
		}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_listener.UnHookKeyboard();
		}
	}
}
