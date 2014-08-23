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
            public UInt64 stack;
            public UInt64 who;
            public UInt64 p1stay, p2stay;
            public UInt64 cardsToTake;
            public UInt64 changeSuit;

            // GameStruct constructor
            public GameStruct(string name1, string name2, UInt16 who) {
                this.name1 = name1;
                this.name2 = name2;
                p1cards = new Dictionary<UInt64, bool>();
                p2cards = new Dictionary<UInt64, bool>();
                this.who = who;
                p1stay = 0; // 4
                p2stay = 0; // 4
                cardsToTake = 1; // 2 + 3
                changeSuit = 0; // A
            }

            // get cards
            public void GetCard(UInt64 who) {
                for (UInt64 i = 0; i < cardsToTake; i++) {
                    Random rnd = new Random();
                    UInt64 card;
                    do card = (UInt64)rnd.Next(0, 52);
                    while( card==stack || p1cards.ContainsKey(card) || p2cards.ContainsKey(card) );

                    if (who == 1) {
                        p1cards.Add(card, true);
                    } else if (who == 2) {
                        p2cards.Add(card, true);
                    } else if (who == 3) {
                        stack = card;
                    }
                }
                cardsToTake = 1;
            }

            // get next player
            public UInt64 GetNext() {
                if (who == 1 && p2stay == 0)
                    who = 2;
                else if (who == 1 && p2stay > 0) {
                    who = 1; p2stay--;
                }
                else if (who == 2 && p1stay == 0)
                    who = 1;
                else if (who == 2 && p1stay > 0) {
                    who = 2; p1stay--;
                }
                return who;
            }

            public void SendStatus() {
                String msg = "0GM_" + name1 + "," + who + "," + stack + "," + cardsToTake + "," + changeSuit + ";";
                foreach (var card in p1cards) msg += "," + card.Key;
                msg += ";";
                foreach (var card in p2cards) msg += "," + card.Key;
                Console.WriteLine( msg );
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
                System.Threading.Thread.Sleep(1000);

                // first card
                gameRooms[gamePointer[player[0]]].GetCard(3);

                // players cards
                gameRooms[gamePointer[player[0]]].cardsToTake = 4;
                gameRooms[gamePointer[player[0]]].GetCard(1);
                gameRooms[gamePointer[player[0]]].cardsToTake = 4;
                gameRooms[gamePointer[player[0]]].GetCard(2);
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
               
                // first card
                gameRooms[nrOfGame].GetCard(3);

                // players cards
                gameRooms[nrOfGame].cardsToTake = 4;
                gameRooms[nrOfGame].GetCard(1);
                gameRooms[nrOfGame].cardsToTake = 4;
                gameRooms[nrOfGame].GetCard(2);
                gameRooms[nrOfGame].SendStatus();
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
                        gameRooms[gamePointer[tmp[0]]].GetCard( gameRooms[gamePointer[tmp[0]]].who );
                    } else {
                        gameRooms[gamePointer[tmp[0]]].stack = UInt64.Parse( tmp[2] );

                        if (UInt64.Parse(tmp[2]) % 13 == 0) // A
                            gameRooms[gamePointer[tmp[0]]].changeSuit = UInt64.Parse( tmp[3] );
                        else
                            gameRooms[gamePointer[tmp[0]]].changeSuit = 0;
                        
                        if ( UInt64.Parse( tmp[2] ) % 13 == 1 ) { // 2
                            if(gameRooms[gamePointer[tmp[0]]].cardsToTake == 1)
                                gameRooms[gamePointer[tmp[0]]].cardsToTake = 2;
                            else 
                                gameRooms[gamePointer[tmp[0]]].cardsToTake += 2;

                        } else if ( UInt64.Parse( tmp[2] ) % 13 == 2 ) { // 3
                            if(gameRooms[gamePointer[tmp[0]]].cardsToTake == 1)
                                gameRooms[gamePointer[tmp[0]]].cardsToTake = 3;
                            else 
                                gameRooms[gamePointer[tmp[0]]].cardsToTake += 3;

                        } else if ( UInt64.Parse( tmp[2] ) % 13 == 3 ) { // 4
                            if (gameRooms[gamePointer[tmp[0]]].who == 1)
                                gameRooms[gamePointer[tmp[0]]].p2stay = 1;
                            else if (gameRooms[gamePointer[tmp[0]]].who == 2)
                                gameRooms[gamePointer[tmp[0]]].p1stay = 1;
                        }

                        if (gameRooms[gamePointer[tmp[0]]].who == 1)
                            gameRooms[gamePointer[tmp[0]]].p1cards.Remove(UInt64.Parse(tmp[2]));
                        else if (gameRooms[gamePointer[tmp[0]]].who == 2)
                            gameRooms[gamePointer[tmp[0]]].p2cards.Remove(UInt64.Parse(tmp[2]));
                    }

                    if ( gameRooms[gamePointer[tmp[0]]].p1cards.Count == 0 ) {
                        con.send ( Encoding.Unicode.GetBytes ( "0GW_" + gameRooms[gamePointer[tmp[0]]].name1 ) );
                        Console.WriteLine ( gameRooms[gamePointer[tmp[0]]].name1 + " has won !!" );
                    }
                    else if (gameRooms[gamePointer[tmp[0]]].p2cards.Count == 0) {
                        con.send ( Encoding.Unicode.GetBytes ( "0GW_" + gameRooms[gamePointer[tmp[0]]].name2 ) );
                        Console.WriteLine ( gameRooms[gamePointer[tmp[0]]].name2 + " has won !!" );
                    } else {
                        gameRooms[gamePointer[tmp[0]]].GetNext ();
                        gameRooms[gamePointer[tmp[0]]].SendStatus();
                    }
                }
            } else
                Console.WriteLine ("Error: Unknown command: " + text );
        }
    }
}
