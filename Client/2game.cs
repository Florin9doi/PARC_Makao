using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Connection;

namespace Client {
    public partial class game : Form {
        private TCPConnection con; // Connection instance
        private lobby lobby_inst; // window instance
        private PictureBox[] pb, cs;

        private UInt16 myPosition;
        private String myName;
        private String opponentName;

        private void setPermission ( bool perm ) {
            if (perm)
                pbStatus.BackColor = Color.Green;
            else
                pbStatus.BackColor = Color.Red;
        }

        private string GetHost () {
            if ( myPosition == 1 )
                return myName;
            else if ( myPosition == 2 )
                return opponentName;
            else return null;
        }

        private string GetGuest () {
            if ( myPosition == 2 )
                return myName;
            else if ( myPosition == 1 )
                return opponentName;
            else return null;
        }

        public game(TCPConnection con, lobby lobby_inst,
                        String host, String guest, UInt16 position ) {

            InitializeComponent ();

            pb = new PictureBox[20];
            pb[0] = player1card1;
            pb[1] = player1card2;
            pb[2] = player1card3;
            pb[3] = player1card4;
            pb[4] = player1card5;
            pb[5] = player1card6;
            pb[6] = player1card7;
            pb[7] = player1card8;
            pb[8] = player1card9;
            pb[9] = player1card10;
            pb[10] = player2card1;
            pb[11] = player2card2;
            pb[12] = player2card3;
            pb[13] = player2card4;
            pb[14] = player2card5;
            pb[15] = player2card6;
            pb[16] = player2card7;
            pb[17] = player2card8;
            pb[18] = player2card9;
            pb[19] = player2card10;
            backCard.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\back.bmp");

            this.con = con;
            con.OnExceptionRaised += con_OnExceptionRaised;
            con.OnReceiveCompleted += con_OnReceiveCompleted;

            this.lobby_inst = lobby_inst;
            myPosition = position;
            myName = position == 1 ? host : guest;
            opponentName = position == 1 ? guest : host;
            this.Text = myName;
            setPermission ( myPosition == 1 ? true : false );

            cs[0] = suit_0;
            cs[1] = suit_1;
            cs[2] = suit_2;
            cs[3] = suit_3;
            suit_0.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\suit_0.png");
            suit_1.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\suit_1.png");
            suit_2.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\suit_2.png");
            suit_3.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\suit_3.png");
        }

        void con_OnExceptionRaised ( object sender, ExceptionRaiseEventArgs args ) {
            if ( con != null )
                con.send ( Encoding.Unicode.GetBytes ( "0GE_" + myName ) );
            if (lobby_inst != null)
                lobby_inst.Show();
            this.Hide ();
        }

        private delegate void FunctionCall ( string text );
        void con_OnReceiveCompleted ( object sender, ReceiveCompletedEventArgs args ) {
            string text = Encoding.Unicode.GetString ( args.data );
            this.BeginInvoke ( new FunctionCall ( ReceieveMessage ), text );
        }

        private void ReceieveMessage ( string text ) {

            // move received
            if ( text.StartsWith ( "0GM_" ) ) {
                // refresh game
                string[] players = text.Substring(4).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (players[0].StartsWith(GetHost() + ",")) { // this game ?

                    string[] g_gen = players[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    // who
                    setPermission(myPosition == UInt64.Parse(g_gen[1]) ? true : false);
                    // stack
                    stack.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\Card_" + int.Parse(g_gen[2]).ToString() + @".bmp");
                    stack.Tag = g_gen[2];
                    // cardsToTake
                    if (int.Parse(g_gen[3]) > 1)
                        cardsToTake.Text = "Cards to take: " + g_gen[3];
                    else
                        cardsToTake.Text = "";
                    // changeSuit
                    if (int.Parse(g_gen[4]) > 0) {
                        changeSuit_gb.Visible = true;
                        changeSuit_gb.Tag = int.Parse(g_gen[4])-1;
                        for (int i = 0; i < 4; i++)
                            if (i + 1 == int.Parse(g_gen[4]))
                                cs[i].Visible = true;
                            else
                                cs[i].Visible = false;
                    }
                    else {
                        changeSuit_gb.Visible = false;
                        changeSuit_gb.Tag = null;
                    }

                    string[] myCards = null, opCards = null;
                    if (myPosition == 1) {
                        myCards = players[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        opCards = players[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    } else if (myPosition == 2) {
                        myCards = players[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        opCards = players[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    // myCards
                    for (int i = 0; i < myCards.Length && i < 10; i++)
                    {
                        pb[i].Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\Card_" + int.Parse(myCards[i]).ToString() + @".bmp");
                        pb[i].Tag = myCards[i];
                    }
                    for (int i = myCards.Length; i < 10; i++) {
                        pb[i].Image = null;
                        pb[i].Tag = null;
                    }

                    // opCards
                    for (int i = 0; i < opCards.Length && i < 10; i++) {
                        pb[i + 10].Image = //Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\back.bmp");
                                Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\Card_" + int.Parse(opCards[i]).ToString() + @".bmp");
                        pb[i + 10].Tag = opCards[i];
                    }
                    for (int i = opCards.Length; i < 10; i++) {
                        pb[i + 10].Image = null;
                        pb[i + 10].Tag = null;
                    }
                }
            }

            // reset game
            else if ( text.StartsWith ( "0GR_" ) && text.Substring ( 4 ).Equals ( GetHost () ) ) {
                for ( int i = 0; i < 10; i++ )
                    pb[i].Image = null;
                chatOut.Text = "";
                setPermission ( myPosition == 1 ? true : false );
            }

           // win
           else if ( text.StartsWith ( "0GW_" ) ) {
                if ( text.Substring ( 4 ).Equals ( GetHost () ) || text.Substring ( 4 ).Equals ( GetGuest () ) ) {
                    setPermission ( false );
                    chatOut.Text += Environment.NewLine;
                    chatOut.Text += text.Substring ( 4 ) + " has won !!";
                }
            }

           // chat
           else if ( text.StartsWith ( "00C_" ) ) {
                string[] tmp = text.Substring ( 4 ).Split ( new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries );
                if ( tmp[0].Equals ( GetHost () ) || tmp[0].Equals ( GetGuest () ) ) {
                    chatOut.Text += Environment.NewLine;
                    chatOut.Text += text.Substring ( 4 );
                }
            }
        }

        // send chat message
        private void textBox1_KeyUp ( object sender, KeyEventArgs e ) {
            if ( e.KeyCode == Keys.Enter ) {
                con.send ( Encoding.Unicode.GetBytes ( "00C_" + myName + ": " + chatAdd.Text ) );
                chatAdd.Text = "";
            }
        }

        // return to lobby
        private void btnExit_Click ( object sender, EventArgs e ) {
            con.send ( Encoding.Unicode.GetBytes ( "0GE_" + GetHost () ) );
            lobby_inst.Show();
            this.Hide ();
        }


        // reset table
        private void btnReset_Click ( object sender, EventArgs e ) {
            con.send ( Encoding.Unicode.GetBytes ( "0GR_" + GetHost () ) );
        }

        // send game action
        private void card_Click ( object sender, EventArgs e ) {

            /* get a new card */
            if (((PictureBox)sender).Name.Equals("backCard"))
                con.send(Encoding.Unicode.GetBytes("0GM_" + myName + ";" + myPosition + ";GC"));

            /* invalid move */
            else if (((PictureBox)sender).Tag == null)
                System.Media.SystemSounds.Hand.Play();

            /* valid move */
            else if ( int.Parse((string)stack.Tag) / 13 == 2 || int.Parse((string)stack.Tag) / 13 == 3 ) {
                if (int.Parse((string)stack.Tag) / 13 == 2 && int.Parse((String)((PictureBox)sender).Tag) == 2
                    || int.Parse((string)stack.Tag) / 13 == 3 && int.Parse((String)((PictureBox)sender).Tag) == 3)
                con.send(Encoding.Unicode.GetBytes("0GM_" + myName + ";" + myPosition + ";" + ((PictureBox)sender).Tag));
            }
            else if ( changeSuit_gb.Tag != null && (int)changeSuit_gb.Tag == int.Parse((String)((PictureBox)sender).Tag) / 13
                    || changeSuit_gb.Tag == null && int.Parse((String)((PictureBox)sender).Tag) / 13 == int.Parse((string)stack.Tag) / 13
                    || changeSuit_gb.Tag == null && int.Parse((String)((PictureBox)sender).Tag) % 13 == int.Parse((string)stack.Tag) % 13) {
                if (int.Parse((String)((PictureBox)sender).Tag) % 13 == 0) {
                    for (int i = 0; i < 4; i++)
                        cs[i].Visible = true;
                    changeSuit_gb.Visible = true;
                } else {
                    con.send(Encoding.Unicode.GetBytes("0GM_" + myName + ";" + myPosition + ";" + ((PictureBox)sender).Tag));
                    changeSuit_gb.Tag = null;
                }
            }
        }

        private void csuit_Click(object sender, EventArgs e) {
            if (changeSuit_gb.Tag == null) {
                con.send(Encoding.Unicode.GetBytes("0GM_" + myName + ";" + myPosition + ";" + ((PictureBox)sender).Tag 
                    + ";" + (String)((PictureBox)sender).Name.Substring(4).ToString()));
                changeSuit_gb.Visible = false;
            }
        }
    }
}
