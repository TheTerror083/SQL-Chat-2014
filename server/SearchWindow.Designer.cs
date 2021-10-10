namespace server
{
    partial class SearchWindow
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
            this.MessageGridView = new System.Windows.Forms.DataGridView();
            this.UserGridView = new System.Windows.Forms.DataGridView();
            this.SearchName = new System.Windows.Forms.Button();
            this.SearchMessage = new System.Windows.Forms.Button();
            this.SearchWordBox = new System.Windows.Forms.TextBox();
            this.SearchDateBox = new System.Windows.Forms.TextBox();
            this.SearchIDBox = new System.Windows.Forms.TextBox();
            this.SearchUserNameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.DeleteUserBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MessageGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MessageGridView
            // 
            this.MessageGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MessageGridView.Location = new System.Drawing.Point(293, 252);
            this.MessageGridView.Name = "MessageGridView";
            this.MessageGridView.Size = new System.Drawing.Size(262, 150);
            this.MessageGridView.TabIndex = 0;
            // 
            // UserGridView
            // 
            this.UserGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserGridView.Location = new System.Drawing.Point(13, 252);
            this.UserGridView.Name = "UserGridView";
            this.UserGridView.Size = new System.Drawing.Size(240, 150);
            this.UserGridView.TabIndex = 1;
            // 
            // SearchName
            // 
            this.SearchName.Location = new System.Drawing.Point(76, 170);
            this.SearchName.Name = "SearchName";
            this.SearchName.Size = new System.Drawing.Size(75, 23);
            this.SearchName.TabIndex = 2;
            this.SearchName.Text = "Search";
            this.SearchName.UseVisualStyleBackColor = true;
            this.SearchName.Click += new System.EventHandler(this.SearchName_Click);
            // 
            // SearchMessage
            // 
            this.SearchMessage.Location = new System.Drawing.Point(389, 202);
            this.SearchMessage.Name = "SearchMessage";
            this.SearchMessage.Size = new System.Drawing.Size(75, 23);
            this.SearchMessage.TabIndex = 3;
            this.SearchMessage.Text = "Search";
            this.SearchMessage.UseVisualStyleBackColor = true;
            this.SearchMessage.Click += new System.EventHandler(this.SearchMessage_Click);
            // 
            // SearchWordBox
            // 
            this.SearchWordBox.Location = new System.Drawing.Point(367, 144);
            this.SearchWordBox.Name = "SearchWordBox";
            this.SearchWordBox.Size = new System.Drawing.Size(100, 20);
            this.SearchWordBox.TabIndex = 4;
            // 
            // SearchDateBox
            // 
            this.SearchDateBox.Location = new System.Drawing.Point(367, 70);
            this.SearchDateBox.Name = "SearchDateBox";
            this.SearchDateBox.Size = new System.Drawing.Size(100, 20);
            this.SearchDateBox.TabIndex = 5;
            // 
            // SearchIDBox
            // 
            this.SearchIDBox.Location = new System.Drawing.Point(76, 69);
            this.SearchIDBox.Name = "SearchIDBox";
            this.SearchIDBox.Size = new System.Drawing.Size(100, 20);
            this.SearchIDBox.TabIndex = 6;
            // 
            // SearchUserNameBox
            // 
            this.SearchUserNameBox.Location = new System.Drawing.Point(76, 144);
            this.SearchUserNameBox.Name = "SearchUserNameBox";
            this.SearchUserNameBox.Size = new System.Drawing.Size(100, 20);
            this.SearchUserNameBox.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Search user by ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Search user by UserName:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(290, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Search message by Date (Year-Month-Day) :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(293, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Search message by Word:";
            // 
            // DeleteUserBtn
            // 
            this.DeleteUserBtn.Location = new System.Drawing.Point(76, 201);
            this.DeleteUserBtn.Name = "DeleteUserBtn";
            this.DeleteUserBtn.Size = new System.Drawing.Size(75, 23);
            this.DeleteUserBtn.TabIndex = 12;
            this.DeleteUserBtn.Text = "Delete";
            this.DeleteUserBtn.UseVisualStyleBackColor = true;
            this.DeleteUserBtn.Click += new System.EventHandler(this.DeleteUserBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 227);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(353, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "To Delete a user, enter his ID in the \"Search by ID\" box and press Delete";
            // 
            // SearchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 414);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DeleteUserBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SearchUserNameBox);
            this.Controls.Add(this.SearchIDBox);
            this.Controls.Add(this.SearchDateBox);
            this.Controls.Add(this.SearchWordBox);
            this.Controls.Add(this.SearchMessage);
            this.Controls.Add(this.SearchName);
            this.Controls.Add(this.UserGridView);
            this.Controls.Add(this.MessageGridView);
            this.Name = "SearchWindow";
            this.Text = "SearchWindow";
            ((System.ComponentModel.ISupportInitialize)(this.MessageGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView MessageGridView;
        private System.Windows.Forms.DataGridView UserGridView;
        private System.Windows.Forms.Button SearchName;
        private System.Windows.Forms.Button SearchMessage;
        private System.Windows.Forms.TextBox SearchWordBox;
        private System.Windows.Forms.TextBox SearchDateBox;
        private System.Windows.Forms.TextBox SearchIDBox;
        private System.Windows.Forms.TextBox SearchUserNameBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button DeleteUserBtn;
        private System.Windows.Forms.Label label5;
    }
}