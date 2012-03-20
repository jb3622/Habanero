using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Disney.Menu
{
	[ToolboxBitmap(typeof(MessageBox), "Resources.MessageBox.bmp")]
	[Description("An enhanced message box.")]
	public partial class MessageBox : Component
	{
		#region Constants
		private const int CheckBoxVerticalSpace = 10;
		#endregion

		#region Constructors
		public MessageBox()
		{
			InitializeComponent();
			InitializeInstance();
		}

		public MessageBox(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
			InitializeInstance();
		}
		#endregion

		#region Fields

		MessageBoxChildCollection childWindowList;
		int seconds;

		#endregion

		#region Designable properties

		private string message;

		[Category("Behavior"), Description("The text to display in the message box.")]
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		public string Message
		{
			get { return message; }
			set { message = value; }
		}

		private string caption;
		/// <summary>
		/// Gets or sets the text to display in the title bar of the message box.
		/// </summary>
		[Category("Behavior"), Description("The text to display in the title bar of the message box.")]
		public string Caption
		{
			get { return caption; }
			set { caption = value; }
		}

		private MessageBoxButtons buttons = MessageBoxButtons.OK;
		/// <summary>
		/// Gets or sets a value indicating which buttons to display in the message box. 
		/// </summary>
		[DefaultValue(typeof(MessageBoxButtons), "OK"), Category("Behavior")]
		[Description("Specifies which buttons to display in the message box. ")]
		public MessageBoxButtons Buttons
		{
			get { return buttons; }
			set { buttons = value; }
		}

		private MessageBoxIcon icon = MessageBoxIcon.None;
		/// <summary>
		/// Gets or sets a value indicating which icon to display in the message box.
		/// </summary>
		[DefaultValue(typeof(MessageBoxIcon), "None"), Category("Behavior")]
		[Description("Specifies which icon to display in the message box.")]
		public MessageBoxIcon Icon
		{
			get { return icon; }
			set { icon = value; }
		}

		private MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1;
		/// <summary>
		/// Gets or sets a value indicating which button would be the default in the message box
		/// </summary>
		[DefaultValue(typeof(MessageBoxDefaultButton), "Button1"), Category("Behavior")]
		[Description("Specifies which button is the default button in the message box.")]
		public MessageBoxDefaultButton DefaultButton
		{
			get { return defaultButton; }
			set { defaultButton = value; }
		}

		private MessageBoxOptions? options;
		/// <summary>
		/// Gets or sets display and association options that will be used for the message box.
		/// </summary>
		[Category("Behavior")]
		[Description("Specifies which display and association options will be used for the message box. ")]
		public MessageBoxOptions? Options
		{
			get { return options; }
			set { options = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the CheckBox checked.
		/// </summary>
		[Category("CheckBox"), DefaultValue(false)]
		[Description("Specifies whether the CheckBox checked.")]
		public bool Checked
		{
			get { return this.checkBox.Checked; }
			set { this.checkBox.Checked = value; }
		}

		#region Timeout related properties
		private int timeout;
		/// <summary>
		/// Gets or sets the amount of seconds that the message box appears.
		/// Set the property to 0 to disable timeout function of <see cref="MessageBox"/>.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">Property value less than 0.</exception>
		[DefaultValue(0), Category("Timeout")]
		[Description("Specifies the amount of seconds that the message box appears. Set it to 0 to disable the function.")]
		public int Timeout
		{
			get { return timeout; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Timeout", "Timeout cannot be less than 0.");

				timeout = value;
			}
		}

		private bool displayTimeout = true;
		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="MessageBox"/> will display a countdown message.
		/// </summary>
		[DefaultValue(true), Category("Timeout")]
		[Description("Indicate the message box to display a countdown message.")]
		public bool DisplayTimeout
		{
			get { return displayTimeout; }
			set { displayTimeout = value; }
		}

		private string timeoutTextFormat;
		/// <summary>
		/// Gets or sets the format of then countdown message. The format should only contains one argument.
		/// </summary>
		/// <exception cref="System.FormatException">Format string contains more than one arguments.</exception>
		[DefaultValue((string)null), Category("Timeout")]
		[Description("Specifies the countdown message format. The format should only contains one argument.")]
		public string TimeoutTextFormat
		{
			get { return timeoutTextFormat; }
			set
			{
				string saved = this.timeoutTextFormat;
				timeoutTextFormat = value;
				try
				{
					GetFormattedCountdownText(0, true);
				}
				catch (FormatException)
				{
					this.timeoutTextFormat = saved;
					throw;
				}
			}
		}

		#endregion

		#region Checkbox related properties

		private bool checkBoxEnabled;
		/// <summary>
		/// Gets or sets a value indicating the <see cref="MessageBox"/> to display a <see cref="CheckBox"/> control.
		/// </summary>
		[Category("CheckBox"), DefaultValue(false)]
		[Description("Indicates the message box to display a CheckBox control.")]
		public bool CheckBoxEnabled
		{
			get { return checkBoxEnabled; }
			set { checkBoxEnabled = value; }
		}

		private string checkBoxText;
		/// <summary>
		/// Gets or sets the text in the <see cref="CheckBox"/> control.
		/// </summary>
		[Category("CheckBox")]
		[Description("Specifies the text of the CheckBox control.")]
		public string CheckBoxText
		{
			get { return checkBoxText; }
			set { checkBoxText = value; }
		}

		#endregion

		#endregion

		#region Runtime properties

		private bool isTimedOut;
		/// <summary>
		/// Gets a value indicating whether the message box be closed manually or automaticly by timeout.
		/// </summary>
		[Browsable(false)]
		public bool IsTimedOut
		{
			get { return isTimedOut; }
			set { isTimedOut = value; }
		}

		private bool TimeoutEnabled
		{
			get { return this.timeout > 0; }
		}

		#endregion

		#region Methods
		private void InitializeInstance()
		{
			this.checkBox.Font = SystemFonts.MessageBoxFont;
		}

		private string GetFormattedCountdownText(int remaining, bool throwError)
		{
			string format = this.timeoutTextFormat;
			string text;

			if (string.IsNullOrEmpty(format))
				format = Properties.Resources.MessageBox_TimeoutTextFormat;

			try
			{
				text = string.Format(format, remaining);
			}
			catch (FormatException fex)
			{
				if (throwError)
					throw fex;

				text = string.Format(Properties.Resources.MessageBox_TimeoutTextFormat);
			}

			return text;
		}

		private string GetCaption()
		{
			string caption = this.caption;

			if (string.IsNullOrEmpty(caption))
			{
				if (this.icon != MessageBoxIcon.None)
					caption = this.icon.ToString();
				else
					caption = Properties.Resources.MessageBox_Caption;
			}

			return caption;
		}

		public DialogResult ShowDialog()
		{
			return this.ShowDialog(null);
		}

		public DialogResult ShowDialog(IWin32Window owner)
		{
			DialogResult dr;

			string caption = this.GetCaption();

			this.seconds = -1;
			this.timer1.Interval = 10;
			this.timer1.Start();
			this.isTimedOut = false;

			if (this.Options.HasValue)
			{
				if ((this.Options.Value | MessageBoxOptions.ServiceNotification) > 0 &&
					owner != null)
					owner = null;

				dr = global::System.Windows.Forms.MessageBox.Show(owner, this.Message, caption,
					this.Buttons, this.Icon, this.DefaultButton, this.Options.Value);
			}
			else
				dr = global::System.Windows.Forms.MessageBox.Show(owner, this.Message, caption,
					this.Buttons, this.Icon, this.DefaultButton);

			return dr;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Timer timer = (Timer)sender;
			IntPtr hMsgBox = NativeMethods.FindWindow(null, this.GetCaption());

			if (hMsgBox != IntPtr.Zero)
			{
				if (this.seconds == -1)
				{
					timer.Stop();
					DecorateMessageBox(hMsgBox);

					if (this.TimeoutEnabled)
					{
						timer.Interval = 1000;
						timer.Start();
					}
					else
						return;
				}

				string text = this.GetFormattedCountdownText(this.Timeout - ++seconds, false);
				if (this.label != null)
					label.Text = text;

				if (seconds >= this.Timeout)
				{
					this.isTimedOut = true;
					
					//NativeMethods.SendMessage(hMsgBox, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

					MessageBoxChild bn = this.childWindowList.GetButton(this.defaultButton);
					NativeMethods.SendMessage(hMsgBox, NativeMethods.WM_COMMAND,
						NativeMethods.MakeWParam(bn.Id, NativeMethods.BN_CLICKED),
						bn.Handle);

					timer.Stop();
				}
			}
			else
			{
				timer.Stop();
			}
		}

		private void DecorateMessageBox(IntPtr hWndMsgBox)
		{
			if (this.childWindowList == null)
				this.childWindowList = new MessageBoxChildCollection();
			else
				this.childWindowList.Clear();

			NativeMethods.EnumChildWindows(hWndMsgBox, EnumChildren, IntPtr.Zero);

			if (this.childWindowList.Count > 0)
			{
				int y, heightIncreasement = 0;
				NativeMethods.RECT rect = new NativeMethods.RECT();
				NativeMethods.POINT point = new NativeMethods.POINT();

				// setup the checkbox control

				if (this.CheckBoxEnabled)
				{
					// move native buttons lower
					foreach (MessageBoxChild child in this.childWindowList.Buttons)
					{
						point.x = child.Rectangle.left;
						point.y = child.Rectangle.top;

						NativeMethods.ScreenToClient(hWndMsgBox, ref point);

						NativeMethods.SetWindowPos(child.Handle, IntPtr.Zero,
							point.x, point.y + CheckBoxVerticalSpace + this.checkBox.Height,
							child.Rectangle.Size.Width, child.Rectangle.Size.Height,
							NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOSIZE);
					}

					heightIncreasement += this.checkBox.Height + CheckBoxVerticalSpace;

					//this.checkBox = new CheckBox();
					//this.checkBox.Location = new Point(10, 20);
					//this.checkBox.Visible = true;
					if (string.IsNullOrEmpty(this.checkBoxText))
						this.checkBox.Text = Properties.Resources.MessageBox_CheckBoxText;
					else
						this.checkBox.Text = this.checkBoxText;

					y = point.y;

					if (this.childWindowList.Text != null)
					{
						NativeMethods.GetWindowRect(this.childWindowList.Text.Handle, ref rect);
						point.x = rect.left;
						point.y = rect.top;
						NativeMethods.ScreenToClient(hWndMsgBox, ref point);
						point.y = rect.Size.Width;
					}
					else
					{
						point.x = 20;
						point.y = 300;
					}

					NativeMethods.SetParent(this.checkBox.Handle, hWndMsgBox);
					this.checkBox.Location = new Point(point.x, y);
					this.checkBox.Width = point.y;
				}

				// setup the timeout label control

				if (this.DisplayTimeout && this.TimeoutEnabled)
				{
					//this.label = new Label();
					this.label.Visible = true;
					this.label.Font = SystemFonts.MessageBoxFont;

					NativeMethods.SetParent(this.label.Handle, hWndMsgBox);
					NativeMethods.GetClientRect(hWndMsgBox, ref rect);
					this.label.Top = rect.bottom + heightIncreasement;
					this.label.Width = rect.right;

					heightIncreasement += this.label.Height;
				}

				// adjusts MessageBox window size

				if (heightIncreasement > 0)
				{
					NativeMethods.GetWindowRect(hWndMsgBox, ref rect);

					NativeMethods.SetWindowPos(hWndMsgBox, IntPtr.Zero,
						rect.left, rect.top, rect.Size.Width, rect.Size.Height + heightIncreasement,
						NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOMOVE);
				}
			}
		}

		private bool EnumChildren(IntPtr hWnd, IntPtr param)
		{
			StringBuilder name = new StringBuilder(1024), caption = new StringBuilder(1024);
			NativeMethods.RECT rect = new NativeMethods.RECT();
			int style, id;

			NativeMethods.GetClassName(hWnd, name, 1024);
			NativeMethods.GetWindowText(hWnd, caption, 1024);
			NativeMethods.GetWindowRect(hWnd, ref rect);

			style = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);
			id = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_ID);

			//Console.WriteLine("hWnd=0x{0:X8}, class={1}, caption={2}, rect={3}, id={4}", hWnd, name, caption, rect, id);

			this.childWindowList.Add(new MessageBoxChild(hWnd, name.ToString(), caption.ToString(), rect, style, id));

			return true;
		}
		#endregion

		#region Helper classes
		class MessageBoxChildCollection : List<MessageBoxChild>
		{
			public MessageBoxChildCollection()
				: base()
			{
				this.buttons = new List<MessageBoxChild>(4);
			}

			public new void Add(MessageBoxChild child)
			{
				switch (child.ClassName.ToUpper())
				{
				case NativeMethods.CLS_BUTTON:
					this.buttons.Add(child);
					break;
				case NativeMethods.CLS_STATIC:
					if ((child.Style & NativeMethods.SS_ICON) == NativeMethods.SS_ICON)
						this.icon = child;
					else
						this.text = child;
					break;
				default:
					break;
				}

				base.Add(child);
			}

			public new bool Remove(MessageBoxChild child)
			{
				if (child.Equals(icon)) icon = null;
				else if (child.Equals(text)) text = null;

				buttons.Remove(child);

				return base.Remove(child);
			}

			public new void Clear()
			{
				this.icon = null;
				this.text = null;
				this.buttons.Clear();

				base.Clear();
			}

			private MessageBoxChild icon;
			public MessageBoxChild Icon
			{
				get { return icon; }
			}

			private MessageBoxChild text;
			public MessageBoxChild Text
			{
				get { return text; }
			}

			private List<MessageBoxChild> buttons;
			public List<MessageBoxChild> Buttons
			{
				get { return buttons; }
			}

			public MessageBoxChild GetButton(DialogResult result)
			{
				int id;

				switch (result)
				{
				case DialogResult.Abort:
					id = NativeMethods.IDABORT;
					break;
				case DialogResult.Cancel:
					id = NativeMethods.IDCANCEL;
					break;
				case DialogResult.Ignore:
					id = NativeMethods.IDIGNORE;
					break;
				case DialogResult.No:
					id = NativeMethods.IDNO;
					break;
				case DialogResult.OK:
					id = NativeMethods.IDOK;
					break;
				case DialogResult.Retry:
					id = NativeMethods.IDRETRY;
					break;
				case DialogResult.Yes:
					id = NativeMethods.IDYES;
					break;
				default:
					id = 0;
					break;
				}

				if (id > 0)
				{
					foreach (MessageBoxChild bn in this.buttons)
					{
						if (bn.Id == id)
							return bn;
					}
				}

				return null;
			}

			public MessageBoxChild GetButton(MessageBoxDefaultButton button)
			{
				int index = 0;

				if (button == MessageBoxDefaultButton.Button2)
					index = 1;
				else if (button == MessageBoxDefaultButton.Button3)
					index = 2;

				if (index >= this.buttons.Count)
					index = 0;

				return this.buttons[index];
			}

		}

		class MessageBoxChild : IWin32Window
		{
			public MessageBoxChild(IntPtr handle, string className, string caption, NativeMethods.RECT rect, int style, int id)
			{
				this.handle = handle;
				this.className = className;
				this.caption = caption;
				this.rectangle = rect;
				this.style = style;
				this.id = id;
			}

			private int id;
			public int Id
			{
				get { return id; }
			}

			private int style;
			public int Style
			{
				get { return style; }
			}

			private IntPtr handle;
			public IntPtr Handle
			{
				get { return handle; }
			}

			private string className;
			public string ClassName
			{
				get { return className; }
			}

			private string caption;
			public string Caption
			{
				get { return caption; }
			}

			private NativeMethods.RECT rectangle;
			public NativeMethods.RECT Rectangle
			{
				get { return rectangle; }
			}


			// override object.Equals
			public override bool Equals(object obj)
			{

				//       
				// See the full list of guidelines at
				//   http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconequals.asp    
				// and also the guidance for operator== at
				//   http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconimplementingequalsoperator.asp
				//

				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}

				return ((MessageBoxChild)obj).Handle == this.Handle;
			}

			// override object.GetHashCode
			public override int GetHashCode()
			{
				return this.Handle.GetHashCode();
			}

			#region IWin32Window

			IntPtr IWin32Window.Handle
			{
				get { return this.Handle; }
			}

			#endregion
		}

		#endregion
	}
}