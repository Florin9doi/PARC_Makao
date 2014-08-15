using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connection;
using System.Net;
using System.Net.Sockets;

namespace Server {
    class Program {
        public const int port = 3000;
        private static TCPConnection con = new TCPConnection ();

        public class GameStruct {
            public UInt64 gameNr;
            public UInt64 who;
            public String player1, player2;
            public bool stand1, stand2;
            public UInt64 p1Score, p2Score;

            public UInt64 cardMax;
            public UInt64[] cardsArray;
            Random rnd;

            // GameStruct constructor
            public GameStruct ( UInt64 gameNr, string player1, string player2, UInt16 who ) {
                this.gameNr = gameNr;
                this.who = who;
                this.player1 = player1;
                this.player2 = player2;
                stand1 = false;
                stand2 = false;
                p1Score = 0; p2Score = 0;
                cardMax = 52;
                cardsArray = new UInt64[53];
                rnd = new Random ();
                for ( UInt64 i = 0; i < cardMax; i++ ) cardsArray[i] = i;
            }

            // get a random card
            public UInt64 GetCard ( UInt64 player ) {
                if ( cardMax > 0 ) {
                    UInt64 retPos = (UInt64)rnd.Next ( 0, (int)cardMax );
                    UInt64 returnCard = cardsArray[retPos];
                    cardMax--;
                    cardsArray[retPos] = cardsArray[cardMax];

                    // update score
                    UInt64 sc = 0;
                    if ( returnCard % 13 == 0 )
                        sc = 11;
                    else if ( returnCard % 13 >= 1 && returnCard % 13 <= 9 )
                        sc = returnCard % 13 + 1;
                    else if ( returnCard % 13 >= 10 && returnCard % 13 <= 12 )
                        sc = 10;

                    if ( player == 1 )
                        p1Score += sc;
                    else if ( player == 2 )
                        p2Score += sc;

                    Console.WriteLine ( player1 + " have " + p1Score + "pts; " + player2 + " have " + p2Score + "pts" );
                    return returnCard;
                }
                return 0;
            }

            // player choosed to stand
            public void SetStand ( UInt64 player, bool state ) {
                if ( player == 1 )
                    stand1 = state;
                else if ( player == 2 )
                    stand2 = state;
            }

            // get next player
            public UInt64 GetNext () {
                if ( who == 1 && stand2 == false )
                    who = 2;
                else if ( who == 1 && stand2 == true )
                    who = 1;
                else if ( who == 2 && stand1 == false )
                    who = 1;
                else if ( who == 2 && stand1 == true )
                    who = 2;
                return who;
            }
        } // end of GameStruct

        private static UInt64 nrOfGame = 0;
        private static Dictionary<string, UInt64> gamePointer = new Dictionary<string, UInt64> ();
        private static Dictionary<UInt64, GameStruct> gameRooms = new Dictionary<UInt64, GameStruct> ();

        static void Main ( string[] args ) {
            con.reserve ( port );
            con.OnReceiveCompleted += con_OnReceiveCompleted;
            con.OnExceptionRaised += con_OnExceptionRaised;

            Console.WriteLine ( "waiting connection from clients" );
            System.Threading.Thread.Sleep ( System.Threading.Timeout.Infinite );
        }

        static void con_OnExceptionRaised ( object sender, ExceptionRaiseEventArgs args ) {
            if ( !( sender.GetType () == typeof ( Socket ) ) ) {
                Console.WriteLine ( "exception source : " + args.raisedException.Source );
                Console.WriteLine ( "exception raised : " + args.raisedException.Message );
                Console.WriteLine ( "exception detail : " + args.raisedException.InnerException );
            }
        }

        static void con_OnReceiveCompleted ( object sender, ReceiveCompletedEventArgs args ) {
            string text = Encoding.Unicode.GetString ( args.data );
            IPEndPoint iep = ( args.remoteSock.RemoteEndPoint as IPEndPoint );
            string clientAddr = iep.Address.ToString () + iep.Port;

            // list games
            if ( text.StartsWith ( "0GL" ) ) {
                foreach ( var game in gameRooms ) {
                    con.sendBySpecificSocket ( Encoding.Unicode.GetBytes ( "0GH_" + game.Value.player1 + ";" + game.Value.who ), args.remoteSock );
                }
            }

            // host game
            else if ( text.StartsWith ( "0GH_" ) ) {
                string player1 = text.Substring ( 4 );
                Console.WriteLine ( player1 + " has created a game" );

                gameRooms.Add ( nrOfGame, new GameStruct ( nrOfGame, player1, "", 0 ) );
                gamePointer.Add ( player1, nrOfGame );

                /* register new game */
                con.send ( Encoding.Unicode.GetBytes ( "0GH_" + gameRooms[nrOfGame].player1 + ";" + gameRooms[nrOfGame].who ) );
                nrOfGame++;
            }

            // join game
            else if ( text.StartsWith ( "0GJ_" ) ) {
                string[] player = text.Substring ( 4 ).Split ( new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries );
                Console.WriteLine ( player[1] + " has joied " + player[0] + "'s game" );

                UInt64 nrOfGame = gamePointer[player[0]];
                gameRooms[nrOfGame] = new GameStruct ( nrOfGame, player[0], player[1], 1 );
                gamePointer.Add ( player[1], nrOfGame );

                //start games
                con.send ( Encoding.Unicode.GetBytes ( "0GS_" + player[0] + ";" + 1 ) );
                con.send ( Encoding.Unicode.GetBytes ( "0GJ_" + player[0] + ";" + gameRooms[gamePointer[player[0]]].GetCard ( 1 ) + ";"
                                                              + player[1] + ";" + gameRooms[gamePointer[player[1]]].GetCard ( 2 ) ) );
            }

            // exit game
            else if ( text.StartsWith ( "0GE_" ) ) {
                string player = text.Substring ( 4 );
                Console.WriteLine ( player + " has closed the game" );

                UInt64 nrOfGame = gamePointer[player];
                con.send ( Encoding.Unicode.GetBytes ( "0GE_" + gameRooms[nrOfGame].player1 ) );
                con.send ( Encoding.Unicode.GetBytes ( "0GE_" + gameRooms[nrOfGame].player2 ) );
                gamePointer.Remove ( gameRooms[nrOfGame].player1 );
                gamePointer.Remove ( gameRooms[nrOfGame].player2 );
                gameRooms.Remove ( nrOfGame );
            }

            //reset game
            else if ( text.StartsWith ( "0GR_" ) ) {
                string game = text.Substring ( 4 );

                UInt64 nrOfGame = gamePointer[game];
                gameRooms[nrOfGame] = new GameStruct ( nrOfGame, gameRooms[nrOfGame].player1, gameRooms[nrOfGame].player2, 1 );

                con.send ( Encoding.Unicode.GetBytes ( text ) );
                con.send ( Encoding.Unicode.GetBytes ( "0GM_" + gameRooms[nrOfGame].player1 + ";" + 1 + ";" + gameRooms[nrOfGame].GetCard ( 1 ) ) );
                con.send ( Encoding.Unicode.GetBytes ( "0GM_" + gameRooms[nrOfGame].player2 + ";" + 1 + ";" + gameRooms[nrOfGame].GetCard ( 2 ) ) );
            }

            // chat
            else if ( text.StartsWith ( "00C_" ) ) {
                Console.WriteLine ( text.Substring ( 4 ) );
                con.send ( Encoding.Unicode.GetBytes ( "00C_" + text.Substring ( 4 ) ) );
            }

            // game move
            else if ( text.StartsWith ( "0GM_" ) ) {
                string[] tmp = text.Substring ( 4 ).Split ( new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries );
                if ( gameRooms[gamePointer[tmp[0]]].who == UInt64.Parse ( tmp[1] ) ) { // right player
                    if ( UInt64.Parse ( tmp[2] ) == 0 ) { // stand
                        gameRooms[gamePointer[tmp[0]]].SetStand ( UInt64.Parse ( tmp[1] ), true );
                        Console.WriteLine ( tmp[0] + " has choosed to stand" );
                    } else if ( UInt64.Parse ( tmp[2] ) == 1 ) { // hit
                        UInt64 card = gameRooms[gamePointer[tmp[0]]].GetCard ( UInt64.Parse ( tmp[1] ) );
                        con.send ( Encoding.Unicode.GetBytes ( "0GM_" + tmp[0] + ";" + gameRooms[gamePointer[tmp[0]]].GetNext () + ";" + card ) );
                        Console.WriteLine ( tmp[0] + " has choosed to hit" );
                    }

                    UInt64 win = 0;
                    if ( tmp[0].Equals ( gameRooms[gamePointer[tmp[0]]].player1 ) ) { // player 1
                        if ( gameRooms[gamePointer[tmp[0]]].p1Score == 21 ) // p1 win
                            win = 1;
                        else if ( gameRooms[gamePointer[tmp[0]]].p1Score > 21 ) // p2 win
                            win = 2;
                    } else if ( tmp[0].Equals ( gameRooms[gamePointer[tmp[0]]].player2 ) ) { // player 2
                        if ( gameRooms[gamePointer[tmp[0]]].p2Score == 21 ) // p2 win
                            win = 2;
                        else if ( gameRooms[gamePointer[tmp[0]]].p2Score > 21 ) // p1 win
                            win = 1;
                    }
                    if ( gameRooms[gamePointer[tmp[0]]].stand1 == true && gameRooms[gamePointer[tmp[0]]].stand2 == true ) {
                        if ( gameRooms[gamePointer[tmp[0]]].p1Score > gameRooms[gamePointer[tmp[0]]].p2Score )
                            win = 1;
                        else win = 2;
                    }

                    if ( win == 1 ) {
                        con.send ( Encoding.Unicode.GetBytes ( "0GW_" + gameRooms[gamePointer[tmp[0]]].player1 ) );
                        Console.WriteLine ( gameRooms[gamePointer[tmp[0]]].player1 + " has won !!" );
                    } else if ( win == 2 ) {
                        con.send ( Encoding.Unicode.GetBytes ( "0GW_" + gameRooms[gamePointer[tmp[0]]].player2 ) );
                        Console.WriteLine ( gameRooms[gamePointer[tmp[0]]].player2 + " has won !!" );
                    } else if ( UInt64.Parse ( tmp[2] ) == 0 ) { // stand
                        con.send ( Encoding.Unicode.GetBytes ( "0GM_" + tmp[0] + ";" + gameRooms[gamePointer[tmp[0]]].GetNext () ) );
                    }
                }
            } else
                Console.WriteLine ( text );

        }
    }
}
