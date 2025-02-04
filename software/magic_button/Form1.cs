// -----------------------------------------------------------------------
// <copyright file="Form1.cs" company="Synkom">
//     Copyright (c) Synkom. All rights reserved.
// </copyright>
// <author>Oleg Serov</author>
// <author>Daniel Lumbagbas</author>
// <date>2023-06-05</date>
// <version>0.0.1</version>
// <summary>This application is a GUI for "Magic Button" project.
// For additional information please refer to official project documentation.</summary>
// -----------------------------------------------------------------------

// Including necessary libraries
using magic_button.Properties;
using Microsoft.Win32;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;



// Main namespace
namespace magic_button
{
    // Main form

    public partial class Form1 : Form
    {
        // Declaring variables for UI elements
        private SerialPort port;

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;

        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private PictureBox pictureBox7;
        private PictureBox pictureBox8;
        private PictureBox pictureBox9;
        private PictureBox pictureBox10;
        private PictureBox pictureBox11;
        private PictureBox pictureBox12;

        // Initial setup for UI elements
        private PictureBox pictureBox13;

        private PictureBox pictureBox14;
        private PictureBox pictureBox15;

        private System.Windows.Forms.ListBox listBox1;

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button button20;

        private System.Windows.Forms.NotifyIcon notifyIcon2;

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;

        private System.Windows.Forms.ToolStripMenuItem CloseApp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;

        // Panel for button setup
        private System.Windows.Forms.Panel panel1;

        // Panel for default settings
        private System.Windows.Forms.Panel panel2;

        // Panel for user guide
        private System.Windows.Forms.Panel panel3;

        private System.ComponentModel.IContainer components = null;

        // define comboBox1
        private System.Windows.Forms.ComboBox comboBox1;

        // define comboBox2
        private System.Windows.Forms.ComboBox comboBox2;

        // define Timer for tracking periodic messages from device
        private Timer timer = new Timer();

        // A variable to hold last ping received time
        private DateTime lastPingReceivedAt = new DateTime();

        // A variable to hold last button pressed time
        private DateTime lastButtonPressed;

        // Button press interval seconds
        private int buttonPressInterval = 2;

        // A flag to monitor second button press
        private bool buttonPressed = false;

        // A boolean variable to store previous button state
        private bool previuous_state = false;

        // A boolean variable to store current button state
        private bool current_state = false;

        // Create a dictionary with string keys and bool values
        private Dictionary<string, bool> default_value_changed = new Dictionary<string, bool>
        {
            { "application", false },
            { "contact", false },
            { "call", false },
            { "mail", false },
            { "emergency", false },
            { "link", false }
        };

        // String variable for storing main window header
        private string header = "Magic Button";

        // String array to hold current contact name and number
        private string[] CurrentContact = new string[2];

        // String array to hold current call name and number
        private string[] CurrentCall = new string[2];

        // String array to hold current contact name and number
        private string[] CurrentEmail = new string[2];

        // A string array to hold selected button and execution parameters
        private string[] CurrentApp = new string[2];

        // A string array to hold selected link
        private string[] CurrentLink = new string[2];

        // A string array to hold selected application to launch
        private string[] CurrentApplicationToLaunch = new string[2];

        // A string to hold currently selected app on the initial setup panel
        private string SetupAppToLaunch = "";

        // A string to hold current quick confinguration buttons values
        private string[] CurrentQuickConfig = new string[3];

        // Function to get active window title
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        // Function to get active window title
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        // A dictionary to store current button configuration
        private Dictionary<string, Dictionary<string, string>> ButtonConfig =
        new Dictionary<string, Dictionary<string, string>>
        {
            {
                "left", new Dictionary<string, string>
                {
                    { "action", "link" },
                    { "value", ConfigurationManager.AppSettings["defaultLink"]},
                    { "active", "false" }
                }
            },
            {
                "center", new Dictionary<string, string>
                {
                    { "action", "call" },
                    { "value", ConfigurationManager.AppSettings["defaultCall"] },
                    { "active", "false" }
                }
            },
            {
                "right", new Dictionary<string, string>
                {
                    { "action", "mail" },
                    { "value", ConfigurationManager.AppSettings["defaultMail"] },
                    { "active", "false"}
                }
            }
        };

        public Form1()
        {
            // Initializing buttons, image boxes and other UI elements
            InitializeComponent();
            // Load default settings for example preconfigured links, emails, etc.
            LoadSettings();
            // Check if it is the first time the application is started
            // if it is, let user pick default startup action for button
            // and set this action data (for example, default contact name and number)
            if (CheckAndSaveFirstStartupDate())
            {
                // First startup
                textBox1.Visible = false;
                textBox3.Visible = false;
                label23.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;
                panel1.Visible = false;
                panel2.BringToFront();
            }
            else
            {
                // Not first startup
                panel1.Visible = false;
                listBox1.Visible = false;
                textBox2.Visible = false;
                textBox4.Visible = false;
                button3.Visible = false;
                button9.Visible = false;
                button14.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
                button7.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                button16.Visible = false;
                label6.Visible = false;
                button17.Visible = false;
                pictureBox4.Visible = false;
                panel3.Visible = false;
                panel2.Visible = false;
                label7.Visible = false;
                comboBox2.Visible = false;
                pictureBox12.Visible = false;
                button20.Visible = false;

                // Setting up a tray icon
                notifyIcon2.ContextMenuStrip = contextMenuStrip1;

                // Searching for device id among available ports
                SetupButtonConnection();

                // Last ping time for monitoring device
                lastPingReceivedAt = DateTime.MinValue;
                // Check every second
                timer.Interval = 500;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Function run on form load
        }

        // Function to load settings
        private void LoadSettings()
        {
            // Current action which will be executed on button press
            CurrentApp[0] = Properties.Settings.Default.defaultStartupApp;
            // Current link which will be opened on button press
            CurrentLink[0] = Properties.Settings.Default.defaultLink;
            // Current contact name and number which will be called on button press
            CurrentCall[0] = Properties.Settings.Default.defaultCallName;
            CurrentCall[1] = Properties.Settings.Default.defaultCallNumber;
            // Current email subject and address which will be opened on button press
            CurrentEmail[0] = Properties.Settings.Default.defaultEmailSubject;
            CurrentEmail[1] = Properties.Settings.Default.defaultEmailAddress;
            // Current application which will be launched on button press
            CurrentApplicationToLaunch[0] = Properties.Settings.Default.defaultApplicationDirectory;
            CurrentApplicationToLaunch[1] = Properties.Settings.Default.defaultApplicationName;
            // Current quick configuration buttons layout
            CurrentQuickConfig[0] = Properties.Settings.Default.defaultQuickConfigLeft;
            CurrentQuickConfig[1] = Properties.Settings.Default.defaultQuickConfigCenter;
            CurrentQuickConfig[2] = Properties.Settings.Default.defaultQuickConfigRight;
            // Configure buttons pictures accordingly to their functions
            setup_button_background(CurrentQuickConfig[0], button4);
            setup_button_background(CurrentQuickConfig[1], button5);
            setup_button_background(CurrentQuickConfig[2], button7);
        }

        private void setup_button_background(string function, System.Windows.Forms.Button button)
        {
            switch (function)
            {
                case "link":
                    button.BackgroundImage = Properties.Resources.browser_img;
                    break;

                case "call":
                    button.BackgroundImage = Properties.Resources.call_img;
                    break;

                case "mail":
                    button.BackgroundImage = Properties.Resources.mail_plus_img;
                    break;

                case "application":
                    button.BackgroundImage = Properties.Resources.app_img;
                    break;

                default:
                    break;
            }
            button.BackgroundImageLayout = ImageLayout.Zoom;
        }

        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.button17 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button16 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button14 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button20 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox13 = new System.Windows.Forms.PictureBox();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.button13 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.notifyIcon2 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseApp = new System.Windows.Forms.ToolStripMenuItem();
            this.button15 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.button18 = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button1.Location = new System.Drawing.Point(674, 485);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(185, 45);
            this.button1.TabIndex = 11;
            this.button1.Text = "仕上げる";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Gainsboro;
            this.button2.BackgroundImage = global::magic_button.Properties.Resources.usb_nok_img;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.button2.Location = new System.Drawing.Point(11, 366);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 40);
            this.button2.TabIndex = 0;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.textBox2.Location = new System.Drawing.Point(23, 320);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(454, 42);
            this.textBox2.TabIndex = 11;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            this.textBox2.Enter += new System.EventHandler(this.textBox2_Enter);
            this.textBox2.Leave += new System.EventHandler(this.textBox2_Leave);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.textBox1.Location = new System.Drawing.Point(223, 380);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(454, 42);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.Enter += new System.EventHandler(this.textBox1_Enter);
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(683, 380);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(185, 42);
            this.textBox3.TabIndex = 11;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            this.textBox3.Enter += new System.EventHandler(this.textBox3_Enter);
            this.textBox3.Leave += new System.EventHandler(this.textBox3_Leave);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.LightYellow;
            this.pictureBox2.Location = new System.Drawing.Point(6, 365);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(49, 49);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button3.Location = new System.Drawing.Point(674, 263);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(185, 52);
            this.button3.TabIndex = 11;
            this.button3.Text = "新しいリンクを保存";
            this.button3.UseCompatibleTextRendering = true;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 32;
            this.listBox1.Items.AddRange(new object[] {
            "https://web.whatsapp.com/",
            "https://www.facebook.com/",
            "https://www.google.com/",
            "https://www.synkom.co.jp/",
            "https://www.nytimes.com/",
            "https://mainichi.jp"});
            this.listBox1.Location = new System.Drawing.Point(23, 368);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(645, 164);
            this.listBox1.TabIndex = 11;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button5
            // 
            this.button5.BackgroundImage = global::magic_button.Properties.Resources.call_img;
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(361, 142);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(301, 390);
            this.button5.TabIndex = 6;
            this.button5.Text = "電話";
            this.button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button7
            // 
            this.button7.BackgroundImage = global::magic_button.Properties.Resources.mail_plus_img;
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(667, 142);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(270, 390);
            this.button7.TabIndex = 8;
            this.button7.Text = "メール";
            this.button7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button4
            // 
            this.button4.BackgroundImage = global::magic_button.Properties.Resources.browser_img;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(84, 142);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(271, 390);
            this.button4.TabIndex = 9;
            this.button4.Text = "リンク";
            this.button4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button9.Location = new System.Drawing.Point(674, 320);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(185, 53);
            this.button9.TabIndex = 11;
            this.button9.Text = "セット";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.Gainsboro;
            this.button10.BackgroundImage = global::magic_button.Properties.Resources.setup_small_img;
            this.button10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button10.Location = new System.Drawing.Point(6, 425);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(50, 50);
            this.button10.TabIndex = 12;
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // panel1
            // 
            this.panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.panel1.Controls.Add(this.pictureBox12);
            this.panel1.Controls.Add(this.pictureBox11);
            this.panel1.Controls.Add(this.button17);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.button16);
            this.panel1.Controls.Add(this.textBox4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button14);
            this.panel1.Controls.Add(this.button11);
            this.panel1.Controls.Add(this.button9);
            this.panel1.Controls.Add(this.comboBox2);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button20);
            this.panel1.Location = new System.Drawing.Point(78, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(874, 545);
            this.panel1.TabIndex = 11;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // pictureBox12
            // 
            this.pictureBox12.BackgroundImage = global::magic_button.Properties.Resources.grid_img;
            this.pictureBox12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox12.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox12.Location = new System.Drawing.Point(23, 236);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(34, 36);
            this.pictureBox12.TabIndex = 15;
            this.pictureBox12.TabStop = false;
            // 
            // pictureBox11
            // 
            this.pictureBox11.BackgroundImage = global::magic_button.Properties.Resources.grid_img;
            this.pictureBox11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox11.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox11.Location = new System.Drawing.Point(23, 199);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(34, 36);
            this.pictureBox11.TabIndex = 14;
            this.pictureBox11.TabStop = false;
            // 
            // button17
            // 
            this.button17.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button17.Location = new System.Drawing.Point(674, 432);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(185, 47);
            this.button17.TabIndex = 11;
            this.button17.Text = "起動アプリケーションとして設定";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(1215, 160);
            this.label5.TabIndex = 11;
            this.label5.Text = resources.GetString("label5.Text");
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // button16
            // 
            this.button16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button16.Location = new System.Drawing.Point(674, 379);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(185, 47);
            this.button16.TabIndex = 11;
            this.button16.Text = "リセット";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(483, 320);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(185, 42);
            this.textBox4.TabIndex = 11;
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            this.textBox4.Enter += new System.EventHandler(this.textBox4_Enter);
            this.textBox4.Leave += new System.EventHandler(this.textBox4_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 37);
            this.label3.TabIndex = 11;
            this.label3.Text = "label3";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 35F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 81);
            this.label1.TabIndex = 13;
            this.label1.Text = "設定";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // button14
            // 
            this.button14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button14.Location = new System.Drawing.Point(23, 320);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(185, 50);
            this.button14.TabIndex = 11;
            this.button14.Text = "アプリケーションの選択";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button11.Location = new System.Drawing.Point(674, 485);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(185, 45);
            this.button11.TabIndex = 11;
            this.button11.Text = "設定タブを閉じる";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "link",
            "call",
            "application",
            "mail"});
            this.comboBox2.Location = new System.Drawing.Point(62, 236);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(215, 37);
            this.comboBox2.TabIndex = 11;
            this.comboBox2.Text = "関数";
            this.comboBox2.DropDown += new System.EventHandler(this.comboBox2_DropDown);
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged_1);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Left",
            "Center",
            "Right"});
            this.comboBox1.Location = new System.Drawing.Point(62, 202);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(215, 37);
            this.comboBox1.TabIndex = 11;
            this.comboBox1.Text = "要素の変更";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            // 
            // button20
            // 
            this.button20.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button20.Location = new System.Drawing.Point(674, 209);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(185, 47);
            this.button20.TabIndex = 11;
            this.button20.Text = "すべてリセット";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.pictureBox13);
            this.panel2.Controls.Add(this.pictureBox14);
            this.panel2.Controls.Add(this.pictureBox15);
            this.panel2.Controls.Add(this.label21);
            this.panel2.Controls.Add(this.label22);
            this.panel2.Controls.Add(this.label23);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.textBox3);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(952, 545);
            this.panel2.TabIndex = 18;
            // 
            // pictureBox13
            // 
            this.pictureBox13.BackgroundImage = global::magic_button.Properties.Resources.browser_img;
            this.pictureBox13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox13.Location = new System.Drawing.Point(100, 150);
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.Size = new System.Drawing.Size(140, 140);
            this.pictureBox13.TabIndex = 18;
            this.pictureBox13.TabStop = false;
            this.pictureBox13.Click += new System.EventHandler(this.pictureBox13_Click);
            // 
            // pictureBox14
            // 
            this.pictureBox14.BackgroundImage = global::magic_button.Properties.Resources.mail_plus_img;
            this.pictureBox14.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox14.Location = new System.Drawing.Point(400, 150);
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.Size = new System.Drawing.Size(140, 140);
            this.pictureBox14.TabIndex = 18;
            this.pictureBox14.TabStop = false;
            this.pictureBox14.Click += new System.EventHandler(this.pictureBox14_Click);
            // 
            // pictureBox15
            // 
            this.pictureBox15.BackgroundImage = global::magic_button.Properties.Resources.call_img;
            this.pictureBox15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox15.Location = new System.Drawing.Point(700, 150);
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.Size = new System.Drawing.Size(140, 140);
            this.pictureBox15.TabIndex = 18;
            this.pictureBox15.TabStop = false;
            this.pictureBox15.Click += new System.EventHandler(this.pictureBox15_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 35F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(180, 12);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(1035, 79);
            this.label21.TabIndex = 18;
            this.label21.Text = "OneClick アプリケーションへようこそ!";
            this.label21.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(110, 80);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(986, 64);
            this.label22.TabIndex = 18;
            this.label22.Text = "アプリケーションを初めて起動するときです。 デフォルトのボタン機能を選択してください。\n次回の起動時に、このアクションは自動的に選択されます。";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label22.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(110, 325);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(372, 37);
            this.label23.TabIndex = 18;
            this.label23.Text = "フィールドに記入してください";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label23.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // panel3
            // 
            this.panel3.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.panel3.Controls.Add(this.label14);
            this.panel3.Controls.Add(this.label20);
            this.panel3.Controls.Add(this.label19);
            this.panel3.Controls.Add(this.label18);
            this.panel3.Controls.Add(this.label15);
            this.panel3.Controls.Add(this.label13);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.pictureBox10);
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.pictureBox9);
            this.panel3.Controls.Add(this.pictureBox8);
            this.panel3.Controls.Add(this.pictureBox7);
            this.panel3.Controls.Add(this.pictureBox5);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Location = new System.Drawing.Point(84, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(871, 542);
            this.panel3.TabIndex = 12;
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(84, 220);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(113, 29);
            this.label14.TabIndex = 27;
            this.label14.Text = "機能変更";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(84, 450);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(269, 29);
            this.label20.TabIndex = 33;
            this.label20.Text = "アプリケーションを停止する";
            this.label20.Click += new System.EventHandler(this.label20_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(84, 380);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(835, 58);
            this.label19.TabIndex = 32;
            this.label19.Text = "インターフェースの左上隅にある「機能切り替え」ボタンのアクションをカスタマイズするには、\r\n「ボタン設定」タブに移動します。";
            this.label19.Click += new System.EventHandler(this.label19_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(84, 356);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 29);
            this.label18.TabIndex = 31;
            this.label18.Text = "設定";
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(85, 292);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(153, 29);
            this.label15.TabIndex = 28;
            this.label15.Text = "ユーザーガイド";
            this.label15.Click += new System.EventHandler(this.label15_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(84, 244);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(909, 29);
            this.label13.TabIndex = 26;
            this.label13.Text = "ボタンの機能を変更するには、「機能の切り替え」ボタンを押して、希望のアクションを選択します。";
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(85, 152);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(934, 58);
            this.label9.TabIndex = 21;
            this.label9.Text = "アプリケーションアイコンをダブルクリックしてアプリケーションを起動します。 メインウィンドウが表示され、\r\nアクティブボタン機能を備えたユーザーインターフェイス。" +
    "\r\n";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(85, 131);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 29);
            this.label12.TabIndex = 25;
            this.label12.Text = "始める";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // pictureBox10
            // 
            this.pictureBox10.BackgroundImage = global::magic_button.Properties.Resources.icon;
            this.pictureBox10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox10.Location = new System.Drawing.Point(19, 139);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(59, 50);
            this.pictureBox10.TabIndex = 24;
            this.pictureBox10.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(84, 315);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(718, 29);
            this.label11.TabIndex = 23;
            this.label11.Text = "ユーザーガイドを表示するには、「ユーザーガイド」ボタンをクリックしてください。";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(84, 479);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(879, 29);
            this.label10.TabIndex = 22;
            this.label10.Text = "アプリケーションを終了するには、インターフェースの左下隅にある「終了」ボタンをクリックします。";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(148, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(773, 58);
            this.label8.TabIndex = 20;
            this.label8.Text = "OneClick アプリケーションへようこそ! このユーザー ガイドは、アプリケーションの\r\n機能を効果的にナビゲートして使用するのに役立ちます。";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackgroundImage = global::magic_button.Properties.Resources.information_icon_bold;
            this.pictureBox9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox9.Location = new System.Drawing.Point(20, 291);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(59, 50);
            this.pictureBox9.TabIndex = 5;
            this.pictureBox9.TabStop = false;
            this.pictureBox9.Click += new System.EventHandler(this.pictureBox9_Click);
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackColor = System.Drawing.Color.IndianRed;
            this.pictureBox8.BackgroundImage = global::magic_button.Properties.Resources.exit_no_border_img;
            this.pictureBox8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox8.Location = new System.Drawing.Point(22, 457);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(56, 50);
            this.pictureBox8.TabIndex = 4;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackgroundImage = global::magic_button.Properties.Resources.setup_img;
            this.pictureBox7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox7.Location = new System.Drawing.Point(20, 364);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(59, 50);
            this.pictureBox7.TabIndex = 3;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = global::magic_button.Properties.Resources.grid_img;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox5.Location = new System.Drawing.Point(17, 220);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(59, 50);
            this.pictureBox5.TabIndex = 1;
            this.pictureBox5.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(139, -2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(763, 82);
            this.label7.TabIndex = 0;
            this.label7.Text = "OneClick ユーザーガイド";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.SlateGray;
            this.pictureBox3.Location = new System.Drawing.Point(0, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(63, 545);
            this.pictureBox3.TabIndex = 14;
            this.pictureBox3.TabStop = false;
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.Color.IndianRed;
            this.button13.BackgroundImage = global::magic_button.Properties.Resources.exit_no_border_img;
            this.button13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button13.Location = new System.Drawing.Point(6, 485);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(50, 50);
            this.button13.TabIndex = 12;
            this.button13.UseVisualStyleBackColor = false;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(95, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(908, 93);
            this.label2.TabIndex = 16;
            this.label2.Text = "デバイスを検索しています...";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // notifyIcon2
            // 
            this.notifyIcon2.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon2.Icon")));
            this.notifyIcon2.Text = "MB";
            this.notifyIcon2.Visible = true;
            this.notifyIcon2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon2_MouseDoubleClick_1);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.CloseApp});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(131, 68);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(130, 32);
            this.toolStripMenuItem1.Text = "設定";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // CloseApp
            // 
            this.CloseApp.Name = "CloseApp";
            this.CloseApp.Size = new System.Drawing.Size(130, 32);
            this.CloseApp.Text = "やめる";
            this.CloseApp.Click += new System.EventHandler(this.Close_Click);
            // 
            // button15
            // 
            this.button15.BackColor = System.Drawing.Color.Gainsboro;
            this.button15.BackgroundImage = global::magic_button.Properties.Resources.grid_img;
            this.button15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button15.Location = new System.Drawing.Point(6, 10);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(50, 50);
            this.button15.TabIndex = 17;
            this.button15.UseVisualStyleBackColor = false;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(91, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(304, 116);
            this.label4.TabIndex = 0;
            this.label4.Text = "Name: {0} \nNumber: {1}";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(84, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(676, 64);
            this.label6.TabIndex = 1;
            this.label6.Text = "ボタン機能を選択してください";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::magic_button.Resource1.instruction_gif;
            this.pictureBox4.Location = new System.Drawing.Point(101, 199);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(836, 333);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 18;
            this.pictureBox4.TabStop = false;
            // 
            // button18
            // 
            this.button18.BackColor = System.Drawing.Color.Gainsboro;
            this.button18.BackgroundImage = global::magic_button.Properties.Resources.information_icon_bold;
            this.button18.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button18.Location = new System.Drawing.Point(6, 365);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(50, 50);
            this.button18.TabIndex = 14;
            this.button18.UseVisualStyleBackColor = false;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(0, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(100, 23);
            this.label24.TabIndex = 0;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(0, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(100, 23);
            this.label25.TabIndex = 0;
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(949, 542);
            this.Controls.Add(this.button18);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "OneClick";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            //textBox2.BackColor = Color.Coral;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            // if textbox is not empty, clear it, else write text in it
            if (textBox2.Text != "")
            {
            }
            else
            {
                switch (comboBox2.Text.ToString())
                {
                    case "link":
                        textBox2.Text = "URL";
                        break;

                    case "mail":
                        textBox2.Text = "Email";
                        break;

                    case "call":
                        textBox2.Text = "Number";
                        break;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            // if textbox is not empty, clear it, else write tex in it
            if (textBox1.Text != "")
            {
            }
            else
            {
                switch (SetupAppToLaunch)
                {
                    case "link":
                        textBox1.Text = "URL";
                        break;

                    case "mail":
                        textBox1.Text = "Email";
                        break;

                    case "call":
                        textBox1.Text = "Number";
                        break;
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            // if textbox is not empty, clear it, else write tex in it
            if (textBox3.Text != "")
            {
            }
            else
            {
                switch (SetupAppToLaunch)
                {
                    case "link":
                        textBox3.Text = "";
                        break;

                    case "mail":
                        textBox3.Text = "Subject";
                        break;

                    case "call":
                        textBox3.Text = "Name";
                        break;
                }
            }
        }

        /*
         * This button controls com port connection and may be used to reconnect
         * if connection is lost
        */

        private void button2_Click(object sender, EventArgs e)
        {
            if (port == null || !port.IsOpen)
            {
                // Search for the device among available ports
                SetupButtonConnection();
                // If device was found
                if (port != null)
                {
                    port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                }
                else
                {
                    MessageBox.Show("Device not found! Please connect USB cable to your computer.");
                    if (port != null)
                    {
                        if (port.IsOpen)
                        {
                            port.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Device connected");
            }
        }

        /*
         * This button adds new URL, entered by user in text box to a list of URLs
        */

        private void button3_Click(object sender, EventArgs e)
        {
            // Check if the text box is empty
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                // textBox1 is empty Notify user
                MessageBox.Show("No input");
            }
            else
            {
                // textBox1 is not empty, check if address is already in the list
                if (listBox1.Items.Contains(textBox2.Text))
                {
                    // text is already is in listBox1
                    textBox2.BackColor = Color.LightGreen;
                }
                else
                {
                    // text is not is not in listBox1, add!
                    if (IsValidUrl(textBox2.Text))
                    {
                        this.listBox1.Items.Add(textBox2.Text);
                    }
                    else
                    {
                        MessageBox.Show("Not valid url");
                    }
                }
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.Text = "";
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            // if textbox is not empty, clear it, else write tex in it
            if (textBox4.Text != "")
            {
            }
            else
            {
                switch (comboBox2.Text.ToString())
                {
                    case "link":
                        textBox4.Text = "";
                        break;

                    case "mail":
                        textBox4.Text = "Subject";
                        break;

                    case "call":
                        textBox4.Text = "Name";
                        break;
                }
            }
        }

        /*
         * This function is activated on Serial Port input data and is
         * responsible for handling different kinds of messages from the controller
         * accordingly to the mode of operation selected by user in the GUI
        */

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Get serial port instance
            SerialPort sp = (SerialPort)sender;
            // Read message
            string data = sp.ReadLine().Trim();
            // Process incoming message
            this.Invoke((MethodInvoker)delegate
            {
                // Check which button 4, 5 or 7 is in focus
                if (data.Contains("PING"))
                {
                    // Start a timer to change the color back to red after 2 seconds
                    lastPingReceivedAt = DateTime.Now;
                    sp.Write(CurrentApp[0]);
                }
                else if (data.Contains("BUTTON PRESSED"))
                {
                    panel1.Visible = false;
                    hide_buttons();
                    show_description();
                    hide_user_guide();

                    // Change the color of onlineIndicator to green when a PING message is received
                    // Ckeck if 10 seconds have passed since last button pressed
                    if (lastButtonPressed.AddSeconds(buttonPressInterval) < DateTime.Now)
                    {
                        // Button pressed threshold is passed
                        lastButtonPressed = DateTime.Now;
                        //Check if button was pressed once already (for different action on second press)
                        if (!buttonPressed)
                        {
                            // Check which operation mode (url, button, etc.) is chosen by user
                            switch (CurrentApp[0])
                            {
                                // Link entered in URL text box
                                case "link":
                                    if (!IsValidUrl(CurrentLink[0]))
                                    {
                                        this.WindowState = FormWindowState.Normal;
                                        MessageBox.Show("URL が無効か空です。 入力を確認してください");// URL is not valid or empty. Please input data and press SET button");
                                        panel1.Visible = true;
                                        show_link_setup_interface();
                                        comboBox1.SelectedItem = "Left";
                                        comboBox2.SelectedItem = "link";
                                    }
                                    else
                                    {
                                        System.Diagnostics.Process.Start(CurrentLink[0]);
                                        buttonPressed = true;
                                        this.WindowState = FormWindowState.Minimized;
                                    }
                                    break;
                                // A button to open an application is chosen
                                case "application":
                                    if (!DirectoryExists(CurrentApplicationToLaunch[0]))
                                    {
                                        this.WindowState = FormWindowState.Normal;
                                        MessageBox.Show("アプリケーション パスが無効か空です。 アプリケーションへのパスを確認してください");//Application path is not valid or empty. Please specify path to your application");
                                        panel1.Visible = true;
                                        show_application_setup_interface();
                                        comboBox1.SelectedItem = "Left";
                                        comboBox2.SelectedItem = "application";
                                    }
                                    else
                                    {
                                        startApplication(CurrentApplicationToLaunch[0]);
                                        this.WindowState = FormWindowState.Minimized;
                                    }
                                    break;
                                // An email button is chosen
                                case "mail":
                                    // Check if default email is set
                                    if (!IsValidEmail(CurrentEmail[1]))
                                    {
                                        this.WindowState = FormWindowState.Normal;
                                        MessageBox.Show("電子メール受信者が無効か空です。 入力を確認してSETボタンを押してください");// Default e-mail receiver is not valid or empty. Please input data and press SET button");
                                        panel1.Visible = true;
                                        show_mail_setup_interface();
                                        comboBox1.SelectedItem = "Right";
                                        comboBox2.SelectedItem = "mail";
                                    }
                                    else
                                    {
                                        // Default email is set
                                        System.Diagnostics.Process.Start(string.Format(@"mailto:{0}?subject={1}&body=すぐにご連絡ください", CurrentEmail[1], CurrentEmail[0]));//Please contact me soon
                                        //buttonPressed = true;
                                        this.WindowState = FormWindowState.Minimized;
                                    }
                                    break;

                                case "call":
                                    if (IsValidMobileNumber(CurrentCall[1]) == -1)
                                    {
                                        this.WindowState = FormWindowState.Normal;
                                        string message = String.Format("発信する番号が設定されていないか空です。 入力を確認してSETボタンを押してください");// Default number to call is not set or empty. Please input data and press SET button");
                                        MessageBox.Show(message);
                                        panel1.Visible = true;
                                        show_video_call_setup_interface();
                                        comboBox1.SelectedItem = "Center";
                                        comboBox2.SelectedItem = "Call";
                                    }
                                    else
                                    {
                                        if (IsValidMobileNumber(CurrentCall[1]) == 1)
                                        {
                                            //Process.Start(string.Format("callto:{0}", CurrentCall[1]));
                                            // Check if callto protocol is supported
                                            if (Process.GetProcessesByName("Skype").Length > 0)
                                            {

                                                Process.Start(string.Format("callto:{0}", CurrentCall[1]));
                                            }
                                            else
                                            {
                                                Process.Start(string.Format("tel:{0}", CurrentCall[1]));
                                            }
                                            buttonPressed = true;
                                        }
                                        else
                                        {
                                            if (Process.GetProcessesByName("Skype").Length > 0)
                                            {

                                                Process.Start(string.Format("skype:{0}?call", CurrentCall[1]));
                                            }
                                            else
                                            {
                                                Process.Start(string.Format("skype:{0}?call", CurrentCall[1]));
                                            }
                                            
                                            
                                        }
                                        
                                        this.WindowState = FormWindowState.Minimized;
                                    }
                                    break;

                                default:
                                    this.WindowState = FormWindowState.Normal;
                                    MessageBox.Show("Unknown action");
                                    break;
                            }
                        }
                        else
                        {
                            if (CurrentApp[0] == "link")
                            {

                                // Check if "teams.microsoft.com" is CurrentCall[1] string
                                if (CurrentLink[0].Contains("teams.microsoft.com"))
                                {
                                    // Check if microsoft teams window is active
                                    if (IsActiveWindowTeams())
                                    {
                                        // wait for 5 seconds
                                        //System.Threading.Thread.Sleep(5000);
                                        // Input "name" to the search field
                                        SendKeys.Send("name");
                                        // press tab 8 times
                                        for (int i = 0; i < 8; i++)
                                        {
                                            // Wait for 0.3 second
                                            System.Threading.Thread.Sleep(300);
                                            SendKeys.Send("{TAB}");
                                        }
                                        // press enter  
                                        SendKeys.Send("{ENTER}");
                                    }

                                }
                            }
                            
                            else
                            {
                                SendKeys.Send("{ENTER}");
                                // Send leftclick to the active window using sendkeys
                                //SendKeys.Send("{LEFTCLICK}");
                            }



                            buttonPressed = false;
                        }
                    }
                    else
                    {
                    }
                }
            });
        }

        /*
         * This function activates when user picks new line in the text box
        */

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selected = listBox1.SelectedItem.ToString();
                CurrentLink[0] = selected;
                textBox2.Clear();
                textBox2.AppendText(selected);
                textBox2.BackColor = Color.LightGreen;
            }
        }

        /*
         * Timer function for monitoring device status
        */

        private void Timer_Tick(object sender, EventArgs e)
        {
            previuous_state = current_state;

            if ((DateTime.Now - lastPingReceivedAt).TotalSeconds > 5) // If more than 1 second passed since last ping, assume device is offline
            {
                current_state = false;
                // Device is offline
                pictureBox2.BackColor = Color.Coral;
                //this.button2.Text = "reconnect";
                this.button2.BackgroundImage = Resources.usb_nok_img;
                label4.Visible = false;
                pictureBox4.Visible = false;
                label6.Visible = false;
                if (port == null || !port.IsOpen)
                {
                    // Search for the device among available ports
                    SetupButtonConnection();
                }
            }
            else
            {
                current_state = true;
                // Device is online
                pictureBox2.BackColor = Color.LightGreen;
                this.button2.BackgroundImage = Resources.usb_ok;
                if (previuous_state == false &
                    current_state == true)
                {
                    // Show Magic Button Interface
                    Show();
                    this.ShowInTaskbar = true;
                    this.WindowState = FormWindowState.Normal;
                    notifyIcon2.Visible = false;
                    this.TopMost = true;
                }
                if (!button5.Visible)
                {
                    show_description();
                }
                else
                {
                    label4.Visible = false;
                }
            }
            header_text_update();
        }

        /*
         * When a user presses this button current action on button press
         * switches to smth
        */

        private void button7_Click(object sender, EventArgs e)
        {
            if (isActionSetupOk(CurrentQuickConfig[2]))
            {
                CurrentApp[0] = CurrentQuickConfig[2];
                hide_buttons();
                hide_user_guide();
                show_description();
            }
        }

        /*
         * When a user presses on label nothing happens
        */

        private void label1_Click(object sender, EventArgs e)
        {
        }

        /*
         * A function to check if the URL entered by user is correct
        */

        private bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result && Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        /*
         * A function to find the device among available com port and
         * to connect to it
        */

        private async void SetupButtonConnection()
        {
            string description = string.Empty;
            string[] ports = SerialPort.GetPortNames();
            foreach (var portName in ports)
            {
                description = GetUSBDeviceDescription(portName);
                if (description.Contains("CH340") || description.Contains("CP210")) //Different versions of the device (in our case - ESP32) have different descriptions
                {
                    port = new SerialPort(portName, 115200);
                    port.Open();
                    port.Write(CurrentApp[0]);
                    if (port.IsOpen)
                    {
                        //pictureBox2.BackColor = Color.LightYellow;
                    }
                    //Handler on port data received
                    port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                    break;
                }
                else     
                {
                    port = null;
                    // MessageBox.Show("Device not found! Connect and restart.");
                    label2.Text = "ボタンを接続してください"; // Please connect the button
                    label2.ForeColor = Color.Red;
                    button4.Visible = false;
                    button5.Visible = false;
                    button7.Visible = false;
                }
            }
        }

        /*
         * When a user presses this button current action on button press
         * switches to contact
        */

        private void button8_Click(object sender, EventArgs e)
        {
        }

        /*
         * When a user presses this button current action on button press
         * switches to contact
        */

        public void button4_Click(object sender, EventArgs e)
        {
            if (isActionSetupOk(CurrentQuickConfig[0]))
            {
                CurrentApp[0] = CurrentQuickConfig[0];
                hide_user_guide();
                hide_buttons();
                show_description();
            }
        }

        /*
         * When a user presses this button current action on button press
         * switches to specific application
        */

        private void button6_Click(object sender, EventArgs e)
        {
        }

        /*
         * SET BUTTON CONFIGURATION
         * When a user presses this button current action on button press
         * switches to link
        */

        private void button9_Click(object sender, EventArgs e)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["action"] = comboBox2.Text.ToString().ToLower();
            ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["value"] = textBox2.Text;
            CurrentApp[0] = comboBox2.Text.ToString().ToLower();
            CurrentApp[1] = textBox2.Text;
            default_value_changed[comboBox2.SelectedItem.ToString().ToLower()] = true;
            buttonPressed = false;
            switch (comboBox2.SelectedItem.ToString().ToLower())
            {
                case "call":
                    default_value_changed["call"] = true;
                    CurrentCall[0] = textBox4.Text;
                    CurrentCall[1] = textBox2.Text;
                    Properties.Settings.Default.defaultCallNumber = textBox2.Text;
                    Properties.Settings.Default.defaultCallName = textBox4.Text;
                    config.AppSettings.Settings["defaultCall"].Value = textBox2.Text;
                    config.AppSettings.Settings["defaultName"].Value = textBox4.Text;
                    config.Save(ConfigurationSaveMode.Modified);
                    //MessageBox.Show(String.Format("DEFAUT VALUE SET\nCalling to: {0}\nNumber: {1}", textBox4.Text, textBox2.Text));
                    break;

                case "link":
                    default_value_changed["link"] = true;
                    CurrentLink[0] = textBox2.Text;
                    Properties.Settings.Default.defaultLink = textBox2.Text;
                    config.AppSettings.Settings["defaultLink"].Value = textBox2.Text;
                    config.Save(ConfigurationSaveMode.Modified);
                    break;

                case "mail":
                    CurrentEmail[0] = textBox4.Text;
                    CurrentEmail[1] = textBox2.Text;
                    default_value_changed["mail"] = true;
                    Properties.Settings.Default.defaultEmailAddress = textBox2.Text;
                    Properties.Settings.Default.defaultEmailSubject = textBox4.Text;
                    config.AppSettings.Settings["defaultReciever"].Value = textBox2.Text;
                    config.AppSettings.Settings["defaultSubject"].Value = textBox4.Text;
                    config.Save(ConfigurationSaveMode.Modified);
                    //MessageBox.Show(String.Format("DEFAUT VALUE SET\nE-mail to: {0}\nSubject: {1}", textBox4.Text, textBox2.Text));
                    break;

                case "application":
                    default_value_changed["application"] = true;
                    Properties.Settings.Default.defaultApplicationDirectory = CurrentApplicationToLaunch[0];
                    Properties.Settings.Default.defaultApplicationName = CurrentApplicationToLaunch[1];
                    config.AppSettings.Settings["defaultAppPath"].Value = CurrentApplicationToLaunch[0];
                    config.AppSettings.Settings["defaultAppName"].Value = CurrentApplicationToLaunch[1];
                    config.Save(ConfigurationSaveMode.Modified);
                    break;

                default:
                    break;
            }
            Properties.Settings.Default.Save();
            updateQuickSettingConfig();
            panel1.Visible = false;
        }

        /*
         * A function to get USB device description
         * which then may be used to identify if
         * the device ok to connect
        */

        public static string GetUSBDeviceDescription(string portName)
        {
            using (var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_PnPEntity WHERE Caption like '%" + portName + "%'"))
            {
                foreach (var device in searcher.Get())
                {
                    string caption = (string)device.GetPropertyValue("Caption");
                    if (caption.Contains(portName))
                    {
                        return caption;
                    }
                }
            }
            return string.Empty;
        }

        /*
         * When a user presses this button current action on button press
         * switches to contact
        */

        private void button5_Click(object sender, EventArgs e)
        {
            if (isActionSetupOk(CurrentQuickConfig[1]))
            {
                CurrentApp[0] = CurrentQuickConfig[1];
                hide_user_guide();
                hide_buttons();
                show_description();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        public static string GetWebsiteName(string url)
        {
            string[] words = url.Split('.');
            string websiteName = words[1];
            return websiteName;
        }

        // Opens setup panel
        private void button10_Click(object sender, EventArgs e)
        {
            // If device connection is established
            if (current_state)
            {
                // If not visible already make it visible
                if (panel1.Visible == false)
                {
                    panel1.Visible = true;
                    panel3.Visible = false;
                    button20.Visible = true;
                    panel1.BringToFront();
                    hide_buttons();
                    hide_user_guide();
                }
                // If visible already make it invisible
                else
                {
                    panel1.Visible = false;
                    button20.Visible = false;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox4.Text = "";
            textBox2.BackColor = Color.White;
            comboBox2.Visible = true;
            pictureBox12.Visible = true;
            switch (comboBox1.SelectedItem)
            {
                case "Left":
                    comboBox2.SelectedItem = ButtonConfig["left"]["action"];
                    break;

                case "Center":
                    comboBox2.SelectedItem = ButtonConfig["center"]["action"];
                    break;

                case "Right":
                    comboBox2.SelectedItem = ButtonConfig["right"]["action"];
                    break;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            System.Windows.Forms.Button selected_button = null;
            int index = 0;
            // Hide all the interfaces
            listBox1.Visible = false;
            textBox2.Visible = false;
            textBox4.Visible = false;
            button3.Visible = false;
            button9.Visible = false;
            button14.Visible = false;
            label3.Visible = false;
            // Check if the item is selected in ComboBox1 object if not notify user
            if (comboBox1.SelectedItem != null)
            {
                switch (comboBox1.SelectedItem)
                {
                    case "Left":
                        selected_button = button4;
                        index = 0;
                        break;

                    case "Center":
                        selected_button = button5;
                        index = 1;
                        break;

                    case "Right":
                        selected_button = button7;
                        index = 2;
                        break;
                }
                // Check the selected item and show the appropriate interface
                switch (comboBox2.SelectedItem.ToString())
                {
                    case "contact":
                        selected_button.Text = "Contact";
                        selected_button.BackgroundImage = Resources.user_icon_img;
                        pictureBox12.BackgroundImage = Resources.user_icon_img;
                        show_conatct_setup_interface();
                        CurrentQuickConfig[index] = "contact";
                        ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["action"] = "contact";
                        ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["value"] = textBox2.Text.ToString();
                        break;

                    case "application":
                        selected_button.Text = "Application";
                        selected_button.BackgroundImage = Resources.app_img;
                        pictureBox12.BackgroundImage = Resources.app_img;
                        ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["action"] = "application";
                        show_application_setup_interface();
                        CurrentQuickConfig[index] = "application";

                        break;

                    case "link":
                        selected_button.Text = "Link";
                        selected_button.BackgroundImage = Resources.browser_img;
                        pictureBox12.BackgroundImage = Resources.browser_img;
                        show_link_setup_interface();
                        CurrentQuickConfig[index] = "link";

                        ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["action"] = "link";
                        break;

                    case "mail":
                        selected_button.Text = "Mail";
                        selected_button.BackgroundImage = Resources.mail_plus_img;
                        pictureBox12.BackgroundImage = Resources.mail_plus_img;
                        ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["action"] = "mail";
                        show_mail_setup_interface();
                        CurrentQuickConfig[index] = "mail";

                        break;

                    case "call":
                        selected_button.Text = "Call";
                        selected_button.BackgroundImage = Resources.call_img;
                        pictureBox12.BackgroundImage = Resources.call_img;
                        ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["action"] = "call";
                        show_video_call_setup_interface();
                        CurrentQuickConfig[index] = "call";

                        break;

                    case "emergency":
                        selected_button.Text = "Emergency";
                        selected_button.BackgroundImage = Resources.emergency_img;
                        pictureBox12.BackgroundImage = Resources.emergency_img;
                        ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["action"] = "emergency";
                        show_emergency_setup_interface();
                        CurrentQuickConfig[index] = "emergency";
                        break;

                    default:
                        selected_button.Text = "Setup";
                        break;
                }
                updateQuickSettingConfig();
            }
            else
            {
                // Notify the user that the item is not selected
                MessageBox.Show("リストから要素を選択してください");//Please select an Element from the list");
            }
        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            string[] actions = { "application", "link", "mail", "call" };
            comboBox2.Items.Clear();
            foreach (string action in actions)
            {
                // check if current action string exists in array
                if (Array.IndexOf(CurrentQuickConfig, action) == -1)
                {
                    comboBox2.Items.Add(action);
                }
            }
        }

        private void change_button_function(System.Windows.Forms.Button button)
        {
        }

        private void show_conatct_setup_interface()
        {
            textBox2.Visible = true;
            button9.Visible = true;
            label3.Text = "連絡先ページへのリンクを入力します (例: Google 連絡先):";//"Enter the link to the contact page (ex. Google Contact) :";
            label3.Visible = true;
            button16.Visible = true;
            button17.Visible = true;
        }

        private void show_application_setup_interface()
        {
            button9.Visible = true;
            label3.Visible = true;
            label3.Text = "アプリケーションまたはファイルのパスを選択します:";//"Choose path for application or file:";
            button14.Visible = true;
            button16.Visible = true;
            button17.Visible = true;
        }

        private void show_link_setup_interface()
        {
            listBox1.Visible = true;
            textBox2.Visible = true;
            button3.Visible = true;
            button9.Visible = true;
            label3.Text = "リンクを入力するか、リストからリンクを選択します:";//"Enter your link, or pick one from the list:";
            label3.Visible = true;
            button16.Visible = true;
            button17.Visible = true;
            textBox2.Text = Settings.Default.defaultLink;
        }

        private void show_mail_setup_interface()
        {
            textBox2.Visible = true;
            textBox4.Visible = true;
            button9.Visible = true;
            label3.Text = "受信者の電子メールと件名を入力してください:";//"Enter the receiver's email and subject:";
            label3.Visible = true;
            button16.Visible = true;
            button17.Visible = true;
            textBox2.Text = Settings.Default.defaultEmailAddress;
            textBox4.Text = Settings.Default.defaultEmailSubject;
        }

        private void show_video_call_setup_interface()
        {
            textBox2.Visible = true;
            textBox4.Visible = true;
            button9.Visible = true;
            label3.Text = "番号と名前を入力してください:";//"Enter the number and name:";
            label3.Visible = true;
            button16.Visible = true;
            button17.Visible = true;
            textBox2.Text = Settings.Default.defaultCallNumber;
            textBox4.Text = Settings.Default.defaultCallName;
        }

        private void show_emergency_setup_interface()
        {
        }

        private void setup_current_app(string function, string data)
        {
            CurrentApp[0] = function;
            CurrentApp[1] = data;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button12_Click(object sender, EventArgs e)
        {
        }

        private void button14_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "exe files (*.exe)|*.exe|All files (*.*)|*.*"; // Filter for .exe files
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of selected file
                    CurrentApplicationToLaunch[0] = openFileDialog.FileName;

                    // Save this filePath somewhere (like a class-level variable) so that it can be used when the special button is clicked
                    ButtonConfig[comboBox1.SelectedItem.ToString().ToLower()]["value"] = CurrentApplicationToLaunch[0];

                    //GetApplicationIcon(filePath, selected_button);

                    CurrentApplicationToLaunch[1] = System.IO.Path.GetFileNameWithoutExtension(CurrentApplicationToLaunch[0]);

                    //GetApplicationName(CurrentApplicationToLaunch[0], CurrentApplicationToLaunch[1]);
                }
            }
        }

        private void startApplication(string filePath)
        {
            // The 'filePath' parameter is the full path of the executable file of the application you want to start

            System.Diagnostics.Process process = new System.Diagnostics.Process();

            process.StartInfo.FileName = filePath;

            process.Start();
        }

        private void GetApplicationIcon(string filePath, System.Windows.Forms.Button selected_button)
        {
            Icon appIcon;

            try
            {
                // Extract the icon from the file
                appIcon = Icon.ExtractAssociatedIcon(filePath);

                // You can then convert the icon to an Image if you want to display it in a PictureBox or similar
                selected_button.BackgroundImage = appIcon.ToBitmap();
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during icon extraction
                Console.WriteLine(ex.Message);
            }
        }

        private void GetApplicationName(string filePath, string selected_button)
        {
            string appName = "";

            try
            {
                // Extract the icon from the file
                appName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                // You can then convert the icon to an Image if you want to display it in a PictureBox or similar
                selected_button = appName;
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during icon extraction
                Console.WriteLine(ex.Message);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon2.Visible = true;
            }
        }

        private void notifyIcon2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon2.Visible = true;
            }
        }

        private void notifyIcon2_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon2.Visible = false;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void header_text_update()
        {
            if (current_state == true)
            {
                // Check which string is in the current_app variable and update the header text after translation to japanese
                if (CurrentApp[0] == "mail")
                {
                    label2.Text = "ボタンを押してください:メールを送る"; 
                }
                else if (CurrentApp[0] == "video_call")
                {
                    label2.Text = "ボタンを押してください:電話をかける";
                }
                else if (CurrentApp[0] == "call")
                {
                    label2.Text = "ボタンを押してください:電話をかける";
                }
                else if (CurrentApp[0] == "emergency")
                {
                    label2.Text = "ボタンを押してください:緊急通報";
                }
                else if (CurrentApp[0] == "link")
                {
                    label2.Text = "ボタンを押してください:リンクを開く";
                }
                else if (CurrentApp[0] == "application")
                {
                    label2.Text = "ボタンを押してください:アプリを開く";
                }
                else if (CurrentApp[0] == "none")
                {
                    label2.Text = "ボタンを押してください";
                }
                //label2.Text = String.Format("ボタンを押してください: {0}", CurrentApp[0]);
                label2.ForeColor = Color.Green;
            }
            else
            {
                label2.Text = "ボタンを接続してください"; // Update the header text to "Please connect the button"
                label2.ForeColor = Color.Red;
                button4.Visible = false;
                button5.Visible = false;
                button7.Visible = false;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon2.Visible = false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (current_state == true)
            {
                hide_user_guide();
                if (label4.Visible)
                {
                    label4.Visible = false;
                    pictureBox4.Visible = false;
                }
                else
                {
                    label4.Visible = true;
                    pictureBox4.Visible = true;
                }
                if (button5.Visible == true)
                {
                    hide_buttons();
                }
                else
                {
                    panel1.Visible = false;
                    show_buttons();
                }
            }
        }

        private void hide_buttons()
        {
            button4.Visible = false;
            button5.Visible = false;
            button7.Visible = false;
            label6.Visible = false;
        }

        private void show_buttons()
        {
            button4.Visible = true;
            button5.Visible = true;
            button7.Visible = true;
            label6.Visible = true;
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

        private void show_description()
        {
            switch (CurrentApp[0])
            {
                case "call":
                    label4.Visible = true;
                    pictureBox4.Visible = true;
                    label4.Text = String.Format("名前: {0} \n番号: {1}", CurrentCall[0], CurrentCall[1]);
                    //pictureBox4.BackgroundImage = Resources.call_img;
                    break;

                case "link":
                    label4.Visible = true;
                    pictureBox4.Visible = true;
                    label4.Text = String.Format("リンク: {0}", CurrentLink[0]);
                    //pictureBox4.BackgroundImage = Resources.browser_img;
                    // Load the animated GIF from a file or resource
                    //Image gifImage = Resource1.instruction_gif; 
                    // Set the PictureBox's Image property to the loaded GIF
                    //check if image is already loaded
                    //if (pictureBox4.Image == null)
                    //{
                    //    pictureBox4.Image = gifImage;
                    //   pictureBox4.BackgroundImage = null;
                        // resize the image to fit the picturebox
                    //    pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
                    //}
                    
                    break;

                case "mail":
                    label4.Visible = true;
                    pictureBox4.Visible = true;
                    //pictureBox4.BackgroundImage = Resources.mail_plus_img;
                    label4.Text = String.Format("メールアドレス: {1} \n主題: {0}", CurrentEmail[0], CurrentEmail[1]);
                    break;

                case "application":
                    label4.Visible = true;
                    pictureBox4.Visible = true;
                    //pictureBox4.BackgroundImage = Resources.app_img;
                    label4.Text = String.Format("応用: {0}", CurrentApplicationToLaunch[1]);
                    break;

                default:
                    break;
            }
            pictureBox4.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void button16_Click(object sender, EventArgs e)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            switch (comboBox2.SelectedItem.ToString())
            {
                case "link":
                    config.AppSettings.Settings["defaultLink"].Value = "none";
                    config.Save(ConfigurationSaveMode.Modified);
                    Properties.Settings.Default.defaultLink = "";
                    MessageBox.Show(String.Format("Link data reset. You will be asked to enter Link data on next application launch.", textBox4.Text, textBox2.Text));
                    break;

                case "mail":
                    config.AppSettings.Settings["defaultReciever"].Value = "none";
                    config.AppSettings.Settings["defaultSubject"].Value = "none";
                    config.Save(ConfigurationSaveMode.Modified);
                    Properties.Settings.Default.defaultEmailAddress = "";
                    Properties.Settings.Default.defaultEmailSubject = "";
                    MessageBox.Show(String.Format("Mail data reset. You will be asked to enter mail data on next application launch.", textBox4.Text, textBox2.Text));
                    break;

                case "call":
                    config.AppSettings.Settings["defaultName"].Value = "none";
                    config.AppSettings.Settings["defaultCall"].Value = "none";
                    config.Save(ConfigurationSaveMode.Modified);
                    Properties.Settings.Default.defaultCallName = "";
                    Properties.Settings.Default.defaultCallNumber = "";
                    MessageBox.Show(String.Format("Call data reset. You will be asked to enter call data on next application launch.", textBox4.Text, textBox2.Text));
                    break;

                case "application":
                    config.AppSettings.Settings["defaultAppPath"].Value = "none";
                    config.AppSettings.Settings["defaultAppName"].Value = "none";
                    config.Save(ConfigurationSaveMode.Modified);
                    Properties.Settings.Default.defaultApplicationDirectory = "";
                    Properties.Settings.Default.defaultApplicationName = "";
                    MessageBox.Show(String.Format("Application data reset. You will be asked to enter application data on next application launch.", textBox4.Text, textBox2.Text));
                    break;

                default:

                    break;
            }
            Properties.Settings.Default.Save();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            // Old way of reset
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["defaultLink"].Value = "none";
            config.AppSettings.Settings["defaultReciever"].Value = "none";
            config.AppSettings.Settings["defaultSubject"].Value = "none";
            config.AppSettings.Settings["defaultName"].Value = "none";
            config.AppSettings.Settings["defaultCall"].Value = "none";
            config.AppSettings.Settings["defaultAppPath"].Value = "none";
            config.AppSettings.Settings["defaultAppName"].Value = "none";
            config.Save(ConfigurationSaveMode.Modified);
            // New way of reset
            Settings.Default.defaultLink = "";
            Settings.Default.defaultEmailAddress = "";
            Settings.Default.defaultEmailSubject = "";
            Settings.Default.defaultCallName = "";
            Settings.Default.defaultCallNumber = "";
            Settings.Default.defaultApplicationDirectory = "";
            Settings.Default.defaultApplicationName = "";
            Settings.Default.StartupAppSet = false;
            Settings.Default.defaultQuickConfigLeft = "link";
            Settings.Default.defaultQuickConfigRight = "mail";
            Settings.Default.defaultQuickConfigCenter = "call";
            ResetFirstStartupDate();
            Settings.Default.Save();
            Application.Exit();
            Environment.Exit(0);
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        // A function to set currently selected app to startup
        private void button17_Click(object sender, EventArgs e)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["startupApp"].Value = comboBox2.SelectedItem.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            Settings.Default.defaultStartupApp = comboBox2.SelectedItem.ToString();
            Settings.Default.Save();
            MessageBox.Show(String.Format("起動アプリセット： {0}", comboBox2.SelectedItem.ToString())); //Startup app set to
        }

        // A function to make first character of string capital and then add string "default" to the beginning without space
        private static string find_default(string str)
        {
            string default_str = "default";
            string new_str = str.First().ToString().ToUpper() + str.Substring(1);
            return default_str + new_str;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (panel3.Visible == false)
            {
                panel3.Visible = true;
                panel1.Visible = false;
                panel3.BringToFront();
                hide_buttons();
                show_user_guide();
            }
            // If visible already make it invisible
            else
            {
                panel3.Visible = false;
                hide_user_guide();
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void show_user_guide()
        {
            panel3.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            label12.Visible = true;
            label13.Visible = true;
            label14.Visible = true;
            label15.Visible = true;
            //label16.Visible = true;
            //label17.Visible = true;
            label18.Visible = true;
            label19.Visible = true;
            label20.Visible = true;
            pictureBox10.Visible = true;
            pictureBox5.Visible = true;
            //pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            pictureBox8.Visible = true;
            pictureBox9.Visible = true;
            pictureBox10.Visible = true;
        }

        private void hide_user_guide()
        {
            panel3.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            //label16.Visible = false;
            //label17.Visible = false;
            label18.Visible = false;
            label19.Visible = false;
            label20.Visible = false;
            pictureBox10.Visible = false;
            pictureBox5.Visible = false;
            //pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
            pictureBox10.Visible = false;
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }

        private void label10_Click(object sender, EventArgs e)
        {
        }

        private void label11_Click(object sender, EventArgs e)
        {
        }

        private void label12_Click(object sender, EventArgs e)
        {
        }

        private void label14_Click(object sender, EventArgs e)
        {
        }

        private void label13_Click(object sender, EventArgs e)
        {
        }

        private void label15_Click(object sender, EventArgs e)
        {
        }

        private void label16_Click(object sender, EventArgs e)
        {
        }

        private void label17_Click(object sender, EventArgs e)
        {
        }

        private void label18_Click(object sender, EventArgs e)
        {
        }

        private void label19_Click(object sender, EventArgs e)
        {
        }

        private void label20_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["startupApp"].Value = SetupAppToLaunch;

            switch (SetupAppToLaunch)
            {
                case "link":
                    //Check if the text is a valid URL
                    if (IsValidUrl(textBox1.Text))
                    {
                        config.AppSettings.Settings["defaultLink"].Value = textBox1.Text;
                        Settings.Default.defaultLink = textBox1.Text;
                        Settings.Default.defaultStartupApp = "link";
                        Settings.Default.StartupAppSet = true;
                    }
                    else
                    {
                        MessageBox.Show("無効なURL");// "Invalid URL");
                        Settings.Default.StartupAppSet = false;
                    }
                    break;

                case "mail":
                    if (IsValidEmail(textBox1.Text))
                    {
                        config.AppSettings.Settings["defaultReciever"].Value = textBox1.Text;
                        config.AppSettings.Settings["defaultSubject"].Value = textBox3.Text;
                        Settings.Default.defaultEmailAddress = textBox1.Text;
                        Settings.Default.defaultEmailSubject = textBox3.Text;
                        Settings.Default.defaultStartupApp = "mail";
                        Settings.Default.StartupAppSet = true;
                    }
                    else
                    {
                        MessageBox.Show("無効なメール");// "Invalid mail");
                        Settings.Default.StartupAppSet = false;
                    }
                    break;

                case "call":
                    if (IsValidMobileNumber(textBox1.Text) != -1)
                    {
                        config.AppSettings.Settings["defaultCall"].Value = textBox1.Text;
                        config.AppSettings.Settings["defaultName"].Value = textBox3.Text;
                        Settings.Default.defaultCallNumber = textBox1.Text;
                        Settings.Default.defaultCallName = textBox3.Text;
                        Settings.Default.defaultStartupApp = "call";
                        Settings.Default.StartupAppSet = true;
                    }
                    else
                    {
                        MessageBox.Show("無効な番号");// "Invalid number");
                        Settings.Default.StartupAppSet = false;
                    }
                    break;
            }
            config.Save(ConfigurationSaveMode.Modified);
            Settings.Default.Save();
            if (Settings.Default.StartupAppSet == true)
            {
                //MessageBox.Show("Default app set successfully");
                Properties.Settings.Default.FirstStartupDate = DateTime.Now;
                Properties.Settings.Default.Save();
                AddToAutoStart();
                Application.Restart();
                //Environment.Exit(0);
            }
        }

        // Function to check and save startup date
        public static bool CheckAndSaveFirstStartupDate()
        {
            // Check if the first startup date is already set in the application settings
            if (Properties.Settings.Default.FirstStartupDate == DateTime.MinValue)
            {
                // First startup, save the current date as the first startup date
                //Properties.Settings.Default.FirstStartupDate = DateTime.Now;
                //Properties.Settings.Default.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckDefaultApp()
        {
            if (Properties.Settings.Default.StartupAppSet == false)
            {
                // First startup, save the current date as the first startup date
                return true;
            }
            else
            {
                return false;
            }
        }

        // Fuction to get first startup date
        public static DateTime GetFirstStartupDate()
        {
            // Retrieve the first startup date from the application settings
            return Properties.Settings.Default.FirstStartupDate;
        }

        // Fuction to reset startup date
        public static void ResetFirstStartupDate()
        {
            // Reset the first startup date in the application settings
            Properties.Settings.Default.FirstStartupDate = DateTime.MinValue;
            Properties.Settings.Default.Save();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            pictureBox13.BackColor = Color.LightGreen;
            pictureBox14.BackColor = Color.Transparent;
            pictureBox15.BackColor = Color.Transparent;
            label23.Visible = true;
            label23.Text = "リンクを入力してください:";// "Please enter link:";
            textBox1.Visible = true;
            textBox3.Visible = false;
            textBox1.Text = "リンク";// "Link";
            SetupAppToLaunch = "link";
            textBox1.Enabled = false;
            textBox3.Enabled = false;
            textBox1.Enabled = true;
            textBox3.Enabled = true;
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            pictureBox14.BackColor = Color.LightGreen;
            pictureBox13.BackColor = Color.Transparent;
            pictureBox15.BackColor = Color.Transparent;
            label23.Visible = true;
            label23.Text = "デフォルトの受信者の電子メールと件名を入力してください:";//"Please enter default receiver's email and subject:";
            textBox1.Visible = true;
            textBox1.Text = "メール";// "Email";
            textBox3.Visible = true;
            textBox3.Text = "主題";// "Subject";
            SetupAppToLaunch = "mail";
            textBox1.Enabled = false;
            textBox3.Enabled = false;
            textBox1.Enabled = true;
            textBox3.Enabled = true;
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            pictureBox15.BackColor = Color.LightGreen;
            pictureBox13.BackColor = Color.Transparent;
            pictureBox14.BackColor = Color.Transparent;
            label23.Visible = true;
            label23.Text = "電話番号と名前を入力してください:";// "Please enter number to call and you contact number:";
            textBox1.Visible = true;
            textBox1.Text = "番号"; //"Number";
            textBox3.Visible = true;
            textBox3.Text = "名前";// "Name";
            SetupAppToLaunch = "call";
            textBox1.Enabled = false;
            textBox3.Enabled = false;
            textBox1.Enabled = true;
            textBox3.Enabled = true;
        }

        // Function to check if entered email is valid
        public static bool IsValidEmail(string email)
        {
            try
            {
                // Check if the email is valid
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                // Email is not valid
                return false;
            }
        }

        // Function to check if entered mobile number is valid
        public static int IsValidMobileNumber(string telephoneNumber)
        {
            // Check if the string is empty, return -1 if so
            if (string.IsNullOrEmpty(telephoneNumber))
            {
                return -1;
            }
            // Check if the mobile number is valid
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            // Parse the mobile number
            try
            {
                PhoneNumber phoneNumber = phoneNumberUtil.Parse(telephoneNumber, null);
                if (phoneNumberUtil.IsValidNumber(phoneNumber))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (NumberParseException)
            {
                // Mobile number is not valid
                return 0;
                
            }
        }

        // Function to check if directory exist
        public static bool DirectoryExists(string path)
        {
            return File.Exists(path);
        }

        public bool isActionSetupOk(string action)
        {
            switch (action)
            {
                // Link entered in URL text box
                case "link":
                    if (!IsValidUrl(CurrentLink[0]))
                    {
                        this.WindowState = FormWindowState.Normal;
                        MessageBox.Show("URL が無効か空です。 確認してSETボタンを押してください");//"URL is not valid or empty. Please input data and press SET button");
                        panel1.Visible = true;
                        panel3.Visible = false;
                        button20.Visible = true;
                        panel1.BringToFront();
                        hide_buttons();
                        hide_user_guide();
                        panel1.Visible = true;
                        show_link_setup_interface();
                        comboBox1.SelectedItem = "Left";
                        comboBox2.SelectedItem = "link";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    break;
                // A button to open an application is chosen
                case "application":
                    if (!DirectoryExists(CurrentApplicationToLaunch[0]))
                    {
                        this.WindowState = FormWindowState.Normal;
                        MessageBox.Show("アプリケーション パスが無効か空です。 アプリケーションへのパスを指定してください。");//"Application path is not valid or empty. Please specify path to your application");
                        panel1.Visible = true;
                        panel1.Visible = true;
                        panel3.Visible = false;
                        button20.Visible = true;
                        panel1.BringToFront();
                        hide_buttons();
                        hide_user_guide();
                        show_application_setup_interface();
                        comboBox1.SelectedItem = "Left";
                        comboBox2.SelectedItem = "application";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                // An email button is chosen
                case "mail":
                    // Check if default email is set
                    if (!IsValidEmail(CurrentEmail[1]))
                    {
                        this.WindowState = FormWindowState.Normal;
                        MessageBox.Show("デフォルトの電子メール受信者が無効であるか空です。 データを入力してSETボタンを押してください。");
                        panel1.Visible = true;
                        panel1.Visible = true;
                        panel3.Visible = false;
                        button20.Visible = true;
                        panel1.BringToFront();
                        hide_buttons();
                        hide_user_guide();
                        show_mail_setup_interface();
                        comboBox1.SelectedItem = "Right";
                        comboBox2.SelectedItem = "mail";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case "call":
                    if (IsValidMobileNumber(CurrentCall[1]) == -1)
                    {
                        this.WindowState = FormWindowState.Normal;
                        string message = String.Format("デフォルトの電話番号が正しくありません。 SETボタンを押して確認してください。");// "Default number to call is not correct. Please check press SET button.");
                        MessageBox.Show(message);
                        panel1.Visible = true;
                        panel1.Visible = true;
                        panel3.Visible = false;
                        button20.Visible = true;
                        panel1.BringToFront();
                        hide_buttons();
                        hide_user_guide();
                        show_video_call_setup_interface();
                        comboBox1.SelectedItem = "Center";
                        comboBox2.SelectedItem = "Call";
                        return false;
                    }
                    else
                    {
                        // If CurrentCall[1] is empty, then it means that user didn't set default number to call
                        return true;
                    }

                default:
                    return false;
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
        }

        public static bool IsAutoStartEnabled()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string appName = Assembly.GetEntryAssembly().GetName().Name;

                // Check if the registry value exists for your application
                string value = (string)key.GetValue(appName);
                return (value != null && value == Assembly.GetEntryAssembly().Location);
            }
            catch (Exception ex)
            {
                // Handle any exceptions if needed
                Console.WriteLine("Error checking autostart: " + ex.Message);
                return false;
            }
        }

        public static void AddToAutoStart()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string appName = Assembly.GetEntryAssembly().GetName().Name;
                string appPath = Assembly.GetEntryAssembly().Location;

                // Check if the application is already in autostart
                if (!IsAutoStartEnabled())
                {
                    // Add your application to autostart with the specified name and path
                    key.SetValue(appName, appPath);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions if needed
                Console.WriteLine("Error adding to autostart: " + ex.Message);
            }
        }

        public static void RemoveFromAutoStart()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string appName = Assembly.GetEntryAssembly().GetName().Name;

                // Remove your application from autostart by deleting the registry value
                key.DeleteValue(appName, false);
            }
            catch (Exception ex)
            {
                // Handle any exceptions if needed
                Console.WriteLine("Error removing from autostart: " + ex.Message);
            }
        }

        public void updateQuickSettingConfig()
        {
            Properties.Settings.Default.defaultQuickConfigLeft = CurrentQuickConfig[0];
            Properties.Settings.Default.defaultQuickConfigCenter = CurrentQuickConfig[1];
            Properties.Settings.Default.defaultQuickConfigRight = CurrentQuickConfig[2];
            Properties.Settings.Default.Save();
        }

        // A function to check if there any repeated strings in the array
        // the function should take an array as an argument and return string which repeats
        public string checkForRepeats(string[] array)
        {
            string repeatedString = "";
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] == array[j])
                    {
                        repeatedString = array[i];
                        return repeatedString;
                    }
                }
            }
            return repeatedString;
        }
        // 
        public static string GetActiveWindowTitle()
        {
            IntPtr foregroundWindowHandle = GetForegroundWindow();
            if (foregroundWindowHandle != IntPtr.Zero)
            {
                const int maxWindowTitleLength = 256;
                StringBuilder windowTitle = new StringBuilder(maxWindowTitleLength);
                if (GetWindowText(foregroundWindowHandle, windowTitle, maxWindowTitleLength) > 0)
                {
                    return windowTitle.ToString();
                }
            }
            return string.Empty;
        }

        public static bool IsActiveWindowTeams()
        {
            string activeWindowTitle = GetActiveWindowTitle();
            return activeWindowTitle.Contains("Teams");
        }

    }
}