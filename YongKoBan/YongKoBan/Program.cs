using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Sokoban
{

    public enum EntityType
    {
        PLAYER = 0,
        WALL = 1,
        BOX = 2,
    }
    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Int()
        {
            this.y = 0;
            this.y = 0;
        }

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x + v2.x , v1.y+ v2.y);
        }

        public static bool operator ==(Vector2Int v1, Vector2Int v2)
        {
            return (v1.x == v2.x && v1.y == v2.y);
        }

        public static bool operator !=(Vector2Int v1, Vector2Int v2)
        {
            return !(v1.x == v2.x && v1.y == v2.y);
        }
    }

    internal class Program
    {
        //게임 종료 여부
        static bool gameOver;

        //플레이어
        private static Vector2Int playerPos;
        static Vector2Int playerInput;

        // 엔티티
        private static Vector2Int wallPos;

        // 엔티티 정보
        private static char[] entityText = new char[10];

        static void Main()
        {
            InitializeGame();
            RenderGameScreen();
            StartGameLoop();
        }

        static void StartGameLoop()
        {
            while (true)
            {
                if (!gameOver)
                {
                    CheckPlayerInput();
                    CalculateGameLogic();
                    RenderGameScreen();
                    //CheckGameClear();
                }
                else
                {
                    //PrintGameClear();
                    break;
                }
            }
        }

        private static void CheckPlayerInput()
        {
            ConsoleKeyInfo currentKeyInfo = Console.ReadKey();
            Console.Clear();

            playerInput = new Vector2Int();
            switch (currentKeyInfo.Key)
            {
                case ConsoleKey.LeftArrow:

                    playerInput.x -= 1;
                    break;

                case ConsoleKey.RightArrow:
                    playerInput.x += 1;
                    break;

                case ConsoleKey.UpArrow:
                    playerInput.y -= 1;
                    break;

                case ConsoleKey.DownArrow:
                    playerInput.y += 1;
                    break;
            }
        }

        private static void CalculateGameLogic()
        {
            bool isPlayerMoveValid = CheckMoveValid();
            if (isPlayerMoveValid)
            {
                HandlePlayerTurn();
            }
            HandleEntityturn();
        }

        private static void HandleEntityturn()
        {
            
        }

        private static bool CheckMoveValid()
        {
            if ((playerPos + playerInput) == wallPos)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private static void HandlePlayerTurn()
        {
            
            MovePlayer();
        }

        private static void MovePlayer()
        {
            playerPos += playerInput;
        }

        static void InitializeGame()
        {
            InitializeConsole();
            InitializeGameData();
            InitializeEntityPos();
        }

        private static void InitializeEntityPos()
        {
            wallPos = new Vector2Int(5, 3);
            playerPos = new Vector2Int(5, 8);
        }

        private static void InitializeGameData()
        {
            gameOver = false;
            InitializeEntityList();
        }

        private static void InitializeEntityList()
        {
            entityText[(int)EntityType.PLAYER] = 'P';
            entityText[(int)EntityType.WALL] = '#';
            entityText[(int)EntityType.BOX] = 'O';
        }

        private static void InitializeConsole()
        {
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Title = "Yongkoban";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        static void RenderGameScreen()
        {
            Console.Clear();
            PrintEntityAtPos(playerPos, EntityType.PLAYER);
            PrintEntityAtPos(wallPos, EntityType.WALL);
        }

        static void PrintEntityAtPos(int xPos, int yPos, char symbol)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(symbol);
        }

        static void PrintEntityAtPos(Vector2Int pos, char symbol)
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(symbol);
        }

        static void PrintEntityAtPos(int xPos, int yPos, EntityType type)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(entityText[(int)type]);
        }

        static void PrintEntityAtPos(Vector2Int pos, EntityType type)
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(entityText[(int)type]);
        }




    }
}