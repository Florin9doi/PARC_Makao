using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connection;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program {
        public const int port = 3000;
        private static TCPConnection con = new TCPConnection();

        public class GameStruct {
            public String name1, name2;
            public Dictionary<UInt64, bool> p1cards;
            public Dictionary<UInt64, bool> p2cards;
            public UInt64 who;
            public UInt64 p1stay, p2stay;
            public UInt64 cardsToTake;
            public UInt64 changeSuit;

            public UInt64 cardMax;
            public UInt64[] cardsArray;
            Random rnd;

            // GameStruct constructor
            public GameStruct(string name1, string name2, UInt16 who) {
                this.name1 = name1;
                this.name2 = name2;
                p1cards = new Dictionary<UInt64, bool>();
                p2cards = new Dictionary<UInt64, bool>();
                this.who = who;
                p1stay = 0; // 4
                p2stay = 0; // 4
                cardsToTake = 0; // 2 + 3
                changeSuit = 0; // A

                cardMax = 52;
                cardsArray = new UInt64[53];
                rnd = new Random();
                for (UInt64 i = 0; i < cardMax; i++) cardsArray[i] = i;
            }

            // get a random card
            public UInt64 GetCard() {
                if (cardMax > 0) {
                    UInt64 retPos = (UInt64)rnd.Next(0, (int)cardMax);
                    UInt64 returnCard = cardsArray[retPos];
                    cardMax--;
                    cardsArray[retPos] = cardsArray[cardMax];

                    return returnCard;
                }
                return 0;
            }

            // get next player
            public UInt64 GetNext() {
                if (who == 1 && p2stay == 0)
                    who = 2;
                else if (who == 1 && p2stay > 0)
                    who = 1;
                else if (who == 2 && p1stay == 0)
                    who = 1;
                else if (who == 2 && p1stay > 0)
                    who = 2;
                return who;
            }

            internal void SendStatus() {
                String msg = "0GM_" + name1 + "," + who;
                foreach (var card in p1cards) msg += "," + card.Key;
                msg += ";" + name2 + "," + who;
                foreach (var card in p2cards) msg += "," + card.Key;
                Console.WriteLine(msg);
                con.send(Encoding.Unicode.GetBytes( msg ));
            }
        } // end of GameStruct

        private static UInt64 nrOfGame = 0;
        private static Dictionary<string, UInt64> gamePointer = new Dictionary<string, UInt64>();
        private static Dictionary<UInt64, GameStruct> gameRooms = new Dictionary<UInt64, GameStruct>();

        static void Main(string[] args)
        {
            con.reserve(port);
            con.OnReceiveCompleted += con_OnReceiveCompleted;
            con.OnExceptionRaised += con_OnExceptionRaised;

            Console.WriteLine("waiting connection from clients");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        static void con_OnExceptionRaised(object sender, ExceptionRaiseEventArgs args)
        {
            if (!(sender.GetType() == typeof(Socket)))
            {
                Console.WriteLine("exception source : " + args.raisedException.Source);
                Console.WriteLine("exception raised : " + args.raisedException.Message);
                Console.WriteLine("exception detail : " + args.raisedException.InnerException);
            }
        }

        static void con_OnReceiveCompleted(object sender, ReceiveCompletedEventArgs args) {
            string text = Encoding.Unicode.GetString(args.data);
            Console.WriteLine(text);
            IPEndPoint iep = ( args.remoteSock.RemoteEndPoint as IPEndPoint );
            string clientAddr = iep.Address.ToString () + iep.Port;

            // list games
            if ( text.StartsWith ( "0GL" ) ) {
                foreach ( var game in gameRooms ) {
                    con.sendBySpecificSocket ( Encoding.Unicode.GetBytes ( "0GH_" + game.Value.name1 + ";" + game.Value.who ), args.remoteSock );
                }
            }

            // host game
            else if ( text.StartsWith ( "0GH_" ) ) {
                string player1 = text.Substring ( 4 );
                Console.WriteLine ( player1 + " has created a game" );

                gameRooms.Add ( nrOfGame, new GameStruct ( player1, "", 0 ) );
                gamePointer.Add ( player1, nrOfGame );

                /* register new game */
                con.send ( Encoding.Unicode.GetBytes ( "0GH_" + gameRooms[nrOfGame].name1 + ";" + gameRooms[nrOfGame].who ) );
                nrOfGame++;
            }

            // join game
            else if ( text.StartsWith ( "0GJ_" ) ) {
                string[] player = text.Substring ( 4 ).Split ( new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries );
                Console.WriteLine ( player[1] + " has joied " + player[0] + "'s game" );

                UInt64 nrOfGame = gamePointer[player[0]];
                gameRooms[nrOfGame] = new GameStruct ( player[0], player[1], 1 );
                gamePointer.Add ( player[1], nrOfGame );

                //start game
                con.send(Encoding.Unicode.GetBytes("0GS_" + player[0] + ";" + 1));
                con.send(Encoding.Unicode.GetBytes("0GJ_" + player[0] + ";" + player[1] ) );
                System.Threading.Thread.Sleep(500);

                for (UInt64 i = 1; i <= 10; i++) {
                    UInt64 card = gameRooms[gamePointer[player[0]]].GetCard();
                    if (i % 2 == 1) {
                        gameRooms[gamePointer[player[0]]].p1cards.Add( card, true);
                    } else {
                        gameRooms[gamePointer[player[1]]].p2cards.Add( card, true);           
                    }
                }
                gameRooms[gamePointer[player[0]]].SendStatus();
            }

            // exit game
            else if ( text.StartsWith ( "0GE_" ) ) {
                string player = text.Substring ( 4 );
                Console.WriteLine ( player + " has closed the game" );

                UInt64 nrOfGame = gamePointer[player];
                con.send ( Encoding.Unicode.GetBytes ( "0GE_" + gameRooms[nrOfGame].name1 ) );
                con.send ( Encoding.Unicode.GetBytes ( "0GE_" + gameRooms[nrOfGame].name2 ) );
                gamePointer.Remove ( gameRooms[nrOfGame].name1 );
                gamePointer.Remove ( gameRooms[nrOfGame].name2 );
                gameRooms.Remove ( nrOfGame );
            }

            //reset game
            else if ( text.StartsWith ( "0GR_" ) ) {
                string game = text.Substring ( 4 );

                UInt64 nrOfGame = gamePointer[game];
                gameRooms[nrOfGame] = new GameStruct ( gameRooms[nrOfGame].name1, gameRooms[nrOfGame].name2, 1 );

                con.send ( Encoding.Unicode.GetBytes ( text ) );
               
                for (UInt64 i = 1; i <= 10; i++) {
                    UInt64 card = gameRooms[gamePointer[game]].GetCard();
                    if (i % 2 == 1) {
                        gameRooms[gamePointer[game]].p1cards.Add(card, true);
                    } else {
                        gameRooms[gamePointer[game]].p2cards.Add(card, true);           
                    }
                }
                gameRooms[gamePointer[game]].SendStatus();
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
                    if ( tmp[2].Equals("GC")  ) { // getCard
                        UInt64 card = gameRooms[gamePointer[tmp[0]]].GetCard();
                        if ( gameRooms[gamePointer[tmp[0]]].who == 1) {
                            gameRooms[gamePointer[tmp[0]]].p1cards.Add(card, true);
                            gameRooms[gamePointer[tmp[0]]].who = 2;
                        } else {
                            gameRooms[gamePointer[tmp[0]]].p2cards.Add(card, true);
                            gameRooms[gamePointer[tmp[0]]].who = 1;
                        }
                        gameRooms[gamePointer[tmp[0]]].SendStatus();
                    }
                    //    gameRooms[gamePointer[tmp[0]]].SetStand ( UInt64.Parse ( tmp[1] ), true );
                    //    Console.WriteLine ( tmp[0] + " has choosed to stand" );
                    //} else if ( UInt64.Parse ( tmp[2] ) == 1 ) { // hit
                    //    UInt64 card = gameRooms[gamePointer[tmp[0]]].GetCard ( UInt64.Parse ( tmp[1] ) );
                    //    con.send ( Encoding.Unicode.GetBytes ( "0GM_" + tmp[0] + ";" + gameRooms[gamePointer[tmp[0]]].GetNext () + ";" + card ) );
                    //    Console.WriteLine ( tmp[0] + " has choosed to hit" );
                    //}

                    UInt64 win = 0;
                    //if ( tmp[0].Equals ( gameRooms[gamePointer[tmp[0]]].name1 ) ) { // player 1
                    //    if ( gameRooms[gamePointer[tmp[0]]].p1Score == 21 ) // p1 win
                    //        win = 1;
                    //    else if ( gameRooms[gamePointer[tmp[0]]].p1Score > 21 ) // p2 win
                    //        win = 2;
                    //} else if ( tmp[0].Equals ( gameRooms[gamePointer[tmp[0]]].name2 ) ) { // player 2
                    //    if ( gameRooms[gamePointer[tmp[0]]].p2Score == 21 ) // p2 win
                    //        win = 2;
                    //    else if ( gameRooms[gamePointer[tmp[0]]].p2Score > 21 ) // p1 win
                    //        win = 1;
                    //}
                    //if ( gameRooms[gamePointer[tmp[0]]].p1stay == true && gameRooms[gamePointer[tmp[0]]].p2stay == true ) {
                    //    if ( gameRooms[gamePointer[tmp[0]]].p1Score > gameRooms[gamePointer[tmp[0]]].p2Score )
                    //        win = 1;
                    //    else win = 2;
                    //}

                    if ( win == 1 ) {
                        con.send ( Encoding.Unicode.GetBytes ( "0GW_" + gameRooms[gamePointer[tmp[0]]].name1 ) );
                        Console.WriteLine ( gameRooms[gamePointer[tmp[0]]].name1 + " has won !!" );
                    } else if ( win == 2 ) {
                        con.send ( Encoding.Unicode.GetBytes ( "0GW_" + gameRooms[gamePointer[tmp[0]]].name2 ) );
                        Console.WriteLine ( gameRooms[gamePointer[tmp[0]]].name2 + " has won !!" );
                    } else if ( UInt64.Parse ( tmp[2] ) == 0 ) { // stand
                        con.send ( Encoding.Unicode.GetBytes ( "0GM_" + tmp[0] + ";" + gameRooms[gamePointer[tmp[0]]].GetNext () ) );
                    }
                }
            } else
                Console.WriteLine ( text );
        }
    }
}
