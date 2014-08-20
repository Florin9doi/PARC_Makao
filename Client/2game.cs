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
        private PictureBox[] pb;

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
                // refresh screen
                string[] players = text.Substring(4).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var player in players) {
                    string[] tmp = player.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if ( tmp[0].Equals ( myName ) /*&& tmp.Length == 3*/ ) { // myCard
                        stack.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\Card_" + int.Parse(tmp[2]).ToString() + @".bmp");
                        stack.Tag = tmp[2];
                        for (int i = 3; i < tmp.Length && i <= 12; i++) {
                            pb[i - 3].Image = Image.FromFile ( Directory.GetCurrentDirectory () + @"\Imagini\Card_" + int.Parse ( tmp[i] ).ToString () + @".bmp" );
                            pb[i - 3].Tag = tmp[i];
                        }
                        for (int i = tmp.Length; i <= 12; i++) {
                            pb[i - 3].Image = null;
                            pb[i - 3].Tag = null;
                        }
                    } else if ( tmp[0].Equals ( opponentName ) /*&& tmp.Length == 3*/ ) { // oponentCard
                        for (int i = 3; i < tmp.Length && i <= 12; i++) {
                            pb[i + 7].Image = //Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\back.bmp");
                                    Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\Card_" + int.Parse(tmp[i]).ToString() + @".bmp");                                    
                            pb[i + 7].Tag = tmp[i];
                        }
                        for (int i = tmp.Length; i <= 12; i++) {
                            pb[i + 7].Image = null;
                            pb[i + 7].Tag = null;
                        }
                    }

                    if ( tmp[0].Equals ( myName ) || tmp[0].Equals ( opponentName ) )
                        setPermission (myPosition == UInt64.Parse(tmp[1]) ? true : false);
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
            else if (int.Parse((String)((PictureBox)sender).Tag) / 13 == int.Parse((string)stack.Tag) / 13
                   || int.Parse((String)((PictureBox)sender).Tag) % 13 == int.Parse((string)stack.Tag) % 13)
                    // && askforsuit == right color
                    // || stack % 13 == 2 && card % 13 == 2
                    // || stack % 13 == 3 && card % 13 == 3

                //if (int.Parse((String)((PictureBox)sender).Tag) % 13 == 0) // A
                //    ;// TODO: ask for suit
                //else
                con.send(Encoding.Unicode.GetBytes("0GM_" + myName + ";" + myPosition + ";" + ((PictureBox)sender).Tag));
        }
    }
}
