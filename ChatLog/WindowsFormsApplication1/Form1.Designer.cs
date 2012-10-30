namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DebugOutput = new System.Windows.Forms.TextBox();
            this.testbutton = new System.Windows.Forms.Button();
            this.PickedResult = new System.Windows.Forms.TextBox();
            this.SoundTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // DebugOutput
            // 
            this.DebugOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DebugOutput.Location = new System.Drawing.Point(627, 12);
            this.DebugOutput.Multiline = true;
            this.DebugOutput.Name = "DebugOutput";
            this.DebugOutput.Size = new System.Drawing.Size(246, 620);
            this.DebugOutput.TabIndex = 0;
            // 
            // testbutton
            // 
            this.testbutton.Location = new System.Drawing.Point(26, 600);
            this.testbutton.Name = "testbutton";
            this.testbutton.Size = new System.Drawing.Size(75, 23);
            this.testbutton.TabIndex = 1;
            this.testbutton.Text = "text";
            this.testbutton.UseVisualStyleBackColor = true;
            this.testbutton.Click += new System.EventHandler(this.testbutton_Click);
            // 
            // PickedResult
            // 
            this.PickedResult.Location = new System.Drawing.Point(0, 2);
            this.PickedResult.Multiline = true;
            this.PickedResult.Name = "PickedResult";
            this.PickedResult.Size = new System.Drawing.Size(451, 199);
            this.PickedResult.TabIndex = 2;
            // 
            // SoundTimer
            // 
            this.SoundTimer.Interval = 400;
            this.SoundTimer.Tick += new System.EventHandler(this.SoundTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 644);
            this.Controls.Add(this.PickedResult);
            this.Controls.Add(this.testbutton);
            this.Controls.Add(this.DebugOutput);
            this.Name = "Form1";
            this.Text = "ChatLog";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DebugOutput;
        private System.Windows.Forms.Button testbutton;
        private System.Windows.Forms.TextBox PickedResult;
        private System.Windows.Forms.Timer SoundTimer;
    }
}

