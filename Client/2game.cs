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
        private UInt16 myCardPos = 0;
        private String opponentName;
        private UInt16 opponentCardPos = 5;

        private void setPermission ( bool perm ) {
            btnTakeCard.Visible = perm;
            btnDone.Visible = perm;
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

            pb = new PictureBox[10];
            pb[0] = player1card1;
            pb[1] = player1card2;
            pb[2] = player1card3;
            pb[3] = player1card4;
            pb[4] = player1card5;
            pb[5] = player2card1;
            pb[6] = player2card2;
            pb[7] = player2card3;
            pb[8] = player2card4;
            pb[9] = player2card5;
            backCard.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\back.bmp");
            //pb[myCardPos++].Image = Image.FromFile ( Directory.GetCurrentDirectory ()
            //    + @"\Imagini\Card_" + ( position == 1 ? hostCard : guestCard ) + @".bmp" );
            //pb[opponentCardPos++].Image = Image.FromFile ( Directory.GetCurrentDirectory ()
            //    + @"\Imagini\Card_" + ( position == 1 ? guestCard : hostCard ) + @".bmp" );

            this.con = con;
            con.OnExceptionRaised += con_OnExceptionRaised;
            con.OnReceiveCompleted += con_OnReceiveCompleted;

            
            myCardPos = 0;
            opponentCardPos = 5;

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
                string[] tmp = text.Substring(4).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if ( tmp[0].Equals ( myName ) && tmp.Length == 3 ) { // myCard
                        pb[myCardPos].Image =
                                Image.FromFile ( Directory.GetCurrentDirectory () + @"\Imagini\Card_" + int.Parse ( tmp[1] ).ToString () + @".bmp" );
                        pb[myCardPos++].Tag = tmp[1];
                    } else if ( tmp[0].Equals ( opponentName ) && tmp.Length == 3 ) { // oponentCard
                        pb[opponentCardPos].Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\Imagini\back.bmp");
                        pb[opponentCardPos++].Tag = tmp[1];
                    }

                    if ( tmp[0].Equals ( myName ) || tmp[0].Equals ( opponentName ) )
                        setPermission (myPosition == UInt64.Parse(tmp[1]) ? true : false);
            }

            // reset game
            else if ( text.StartsWith ( "0GR_" ) && text.Substring ( 4 ).Equals ( GetHost () ) ) {
                for ( int i = 0; i < 10; i++ )
                    pb[i].Image = null;
                myCardPos = 0;
                opponentCardPos = 5;
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

        // Stand
        private void btnStand_Click ( object sender, EventArgs e ) {
            setPermission ( false );
            con.send ( Encoding.Unicode.GetBytes ( "0GM_" + myName + ";" + myPosition + ";" + 0 ) );
        }

        // Hit
        private void btnHit_Click ( object sender, EventArgs e ) {
            setPermission ( false );
            con.send ( Encoding.Unicode.GetBytes ( "0GM_" + myName + ";" + myPosition + ";" + 1 ) );
        }

        private void card_Click(object sender, EventArgs e)
        {
            MessageBox.Show( ((PictureBox) sender).Name +"  "+ ((PictureBox) sender).Tag );
        }
    }
}
