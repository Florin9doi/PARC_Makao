namespace Client
{
    partial class game
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
            this.chatOut = new System.Windows.Forms.TextBox();
            this.chatAdd = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.player1card1 = new System.Windows.Forms.PictureBox();
            this.player1card2 = new System.Windows.Forms.PictureBox();
            this.player1card3 = new System.Windows.Forms.PictureBox();
            this.player1card4 = new System.Windows.Forms.PictureBox();
            this.player1card5 = new System.Windows.Forms.PictureBox();
            this.player2card5 = new System.Windows.Forms.PictureBox();
            this.player2card4 = new System.Windows.Forms.PictureBox();
            this.player2card3 = new System.Windows.Forms.PictureBox();
            this.player2card2 = new System.Windows.Forms.PictureBox();
            this.player2card1 = new System.Windows.Forms.PictureBox();
            this.backCard = new System.Windows.Forms.PictureBox();
            this.btnStand = new System.Windows.Forms.Button();
            this.btnHit = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.player1card1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backCard)).BeginInit();
            this.SuspendLayout();
            // 
            // chatOut
            // 
            this.chatOut.BackColor = System.Drawing.Color.White;
            this.chatOut.Location = new System.Drawing.Point(397, 12);
            this.chatOut.Multiline = true;
            this.chatOut.Name = "chatOut";
            this.chatOut.ReadOnly = true;
            this.chatOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatOut.Size = new System.Drawing.Size(139, 274);
            this.chatOut.TabIndex = 21;
            // 
            // chatAdd
            // 
            this.chatAdd.Location = new System.Drawing.Point(397, 292);
            this.chatAdd.Name = "chatAdd";
            this.chatAdd.Size = new System.Drawing.Size(139, 20);
            this.chatAdd.TabIndex = 20;
            this.chatAdd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyUp);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnExit.Location = new System.Drawing.Point(397, 318);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(139, 32);
            this.btnExit.TabIndex = 24;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // player1card1
            // 
            this.player1card1.InitialImage = null;
            this.player1card1.Location = new System.Drawing.Point(12, 216);
            this.player1card1.Name = "player1card1";
            this.player1card1.Size = new System.Drawing.Size(71, 96);
            this.player1card1.TabIndex = 25;
            this.player1card1.TabStop = false;
            // 
            // player1card2
            // 
            this.player1card2.InitialImage = null;
            this.player1card2.Location = new System.Drawing.Point(89, 216);
            this.player1card2.Name = "player1card2";
            this.player1card2.Size = new System.Drawing.Size(71, 96);
            this.player1card2.TabIndex = 26;
            this.player1card2.TabStop = false;
            // 
            // player1card3
            // 
            this.player1card3.InitialImage = null;
            this.player1card3.Location = new System.Drawing.Point(166, 216);
            this.player1card3.Name = "player1card3";
            this.player1card3.Size = new System.Drawing.Size(71, 96);
            this.player1card3.TabIndex = 27;
            this.player1card3.TabStop = false;
            // 
            // player1card4
            // 
            this.player1card4.InitialImage = null;
            this.player1card4.Location = new System.Drawing.Point(243, 216);
            this.player1card4.Name = "player1card4";
            this.player1card4.Size = new System.Drawing.Size(71, 96);
            this.player1card4.TabIndex = 28;
            this.player1card4.TabStop = false;
            // 
            // player1card5
            // 
            this.player1card5.InitialImage = null;
            this.player1card5.Location = new System.Drawing.Point(320, 216);
            this.player1card5.Name = "player1card5";
            this.player1card5.Size = new System.Drawing.Size(71, 96);
            this.player1card5.TabIndex = 29;
            this.player1card5.TabStop = false;
            // 
            // player2card5
            // 
            this.player2card5.InitialImage = null;
            this.player2card5.Location = new System.Drawing.Point(320, 12);
            this.player2card5.Name = "player2card5";
            this.player2card5.Size = new System.Drawing.Size(71, 96);
            this.player2card5.TabIndex = 34;
            this.player2card5.TabStop = false;
            // 
            // player2card4
            // 
            this.player2card4.InitialImage = null;
            this.player2card4.Location = new System.Drawing.Point(243, 12);
            this.player2card4.Name = "player2card4";
            this.player2card4.Size = new System.Drawing.Size(71, 96);
            this.player2card4.TabIndex = 33;
            this.player2card4.TabStop = false;
            // 
            // player2card3
            // 
            this.player2card3.InitialImage = null;
            this.player2card3.Location = new System.Drawing.Point(166, 12);
            this.player2card3.Name = "player2card3";
            this.player2card3.Size = new System.Drawing.Size(71, 96);
            this.player2card3.TabIndex = 32;
            this.player2card3.TabStop = false;
            // 
            // player2card2
            // 
            this.player2card2.InitialImage = null;
            this.player2card2.Location = new System.Drawing.Point(89, 12);
            this.player2card2.Name = "player2card2";
            this.player2card2.Size = new System.Drawing.Size(71, 96);
            this.player2card2.TabIndex = 31;
            this.player2card2.TabStop = false;
            // 
            // player2card1
            // 
            this.player2card1.InitialImage = null;
            this.player2card1.Location = new System.Drawing.Point(12, 12);
            this.player2card1.Name = "player2card1";
            this.player2card1.Size = new System.Drawing.Size(71, 96);
            this.player2card1.TabIndex = 30;
            this.player2card1.TabStop = false;
            // 
            // backCard
            // 
            this.backCard.InitialImage = null;
            this.backCard.Location = new System.Drawing.Point(166, 114);
            this.backCard.Name = "backCard";
            this.backCard.Size = new System.Drawing.Size(71, 96);
            this.backCard.TabIndex = 35;
            this.backCard.TabStop = false;
            // 
            // btnStand
            // 
            this.btnStand.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStand.Location = new System.Drawing.Point(166, 318);
            this.btnStand.Name = "btnStand";
            this.btnStand.Size = new System.Drawing.Size(148, 32);
            this.btnStand.TabIndex = 36;
            this.btnStand.Text = "Stand";
            this.btnStand.UseVisualStyleBackColor = true;
            this.btnStand.Click += new System.EventHandler(this.btnStand_Click);
            // 
            // btnHit
            // 
            this.btnHit.BackColor = System.Drawing.Color.Transparent;
            this.btnHit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnHit.Location = new System.Drawing.Point(12, 318);
            this.btnHit.Name = "btnHit";
            this.btnHit.Size = new System.Drawing.Size(148, 32);
            this.btnHit.TabIndex = 37;
            this.btnHit.Text = "HIT";
            this.btnHit.UseVisualStyleBackColor = false;
            this.btnHit.Click += new System.EventHandler(this.btnHit_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnReset.Location = new System.Drawing.Point(320, 318);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(71, 32);
            this.btnReset.TabIndex = 38;
            this.btnReset.Text = "Restart";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SeaGreen;
            this.ClientSize = new System.Drawing.Size(548, 360);
            this.ControlBox = false;
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnHit);
            this.Controls.Add(this.btnStand);
            this.Controls.Add(this.backCard);
            this.Controls.Add(this.player2card5);
            this.Controls.Add(this.player2card4);
            this.Controls.Add(this.player2card3);
            this.Controls.Add(this.player2card2);
            this.Controls.Add(this.player2card1);
            this.Controls.Add(this.player1card5);
            this.Controls.Add(this.player1card4);
            this.Controls.Add(this.player1card3);
            this.Controls.Add(this.player1card2);
            this.Controls.Add(this.player1card1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.chatOut);
            this.Controls.Add(this.chatAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "game";
            this.Text = "Blackjack";
            ((System.ComponentModel.ISupportInitialize)(this.player1card1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1card5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player2card1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backCard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatOut;
        private System.Windows.Forms.TextBox chatAdd;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.PictureBox player1card1;
        private System.Windows.Forms.PictureBox player1card2;
        private System.Windows.Forms.PictureBox player1card3;
        private System.Windows.Forms.PictureBox player1card4;
        private System.Windows.Forms.PictureBox player1card5;
        private System.Windows.Forms.PictureBox player2card5;
        private System.Windows.Forms.PictureBox player2card4;
        private System.Windows.Forms.PictureBox player2card3;
        private System.Windows.Forms.PictureBox player2card2;
        private System.Windows.Forms.PictureBox player2card1;
        private System.Windows.Forms.PictureBox backCard;
        private System.Windows.Forms.Button btnStand;
        private System.Windows.Forms.Button btnHit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btnReset;
    }
}

