using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using YongKoBan;

namespace Sokoban
{

    public enum EntityType
    {
        Player = 0,
        Wall = 1,
        Box = 2,
        NotUsedGoal = 3,
        UsedGoal = 4,

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
        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x - v2.x, v1.y - v2.y);
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

        private const int MaxEntityLimit = 10;

        private const int MIN_X = 0;
        private const int MIN_Y = 0;
        private const int MAX_X = 15;
        private const int MAX_Y = 15;


        //플레이어
        private static Vector2Int playerPos;
        static Vector2Int playerInputDir;

        // 엔티티
        private static Vector2Int[] wallPosList;
        private static int wallCount = 0;

        private static Vector2Int[] boxPosList;
        private static int boxCount = 0;

        private static Goal[] goalList;
        //private static Vector2Int[] goalList;
        //private static bool[] goalStateList;
        private static int goalCount = 0;
        

        // 엔티티 정보
        private static char[] entityText = new char[10];

        static void Main()
        {
            InitializeGame();
            RenderGameScreen();
            StartGameLoop();
        }

        static void InitializeGame()
        {
            InitializeConsole();
            InitializeGameData();
            InitializeEntityPos();
        }

        private static void InitializeEntityPos()
        {

            playerPos = new Vector2Int(5, 8);

            wallPosList[wallCount++] = new Vector2Int(5, 3);
            wallPosList[wallCount++] = new Vector2Int(5, 4);
            wallPosList[wallCount++] = new Vector2Int(5, 5);

            boxPosList[boxCount++] = new Vector2Int(4, 2);
            boxPosList[boxCount++] = new Vector2Int(4, 3);
            boxPosList[boxCount++] = new Vector2Int(4, 1);

            goalList[goalCount++] = new Goal(new Vector2Int(7, 1));
            goalList[goalCount++] = new Goal(new Vector2Int(7, 2));
            goalList[goalCount++] = new Goal(new Vector2Int(7, 3));
            

        }

        private static void InitializeGameData()
        {
            gameOver = false;
            wallPosList = new Vector2Int[MaxEntityLimit];
            boxPosList = new Vector2Int[MaxEntityLimit];
            goalList = new Goal[MaxEntityLimit];
            InitializeEntityList();
        }

        private static void InitializeEntityList()
        {
            entityText[(int)EntityType.Player] = 'P';
            entityText[(int)EntityType.Wall] = '#';
            entityText[(int)EntityType.Box] = 'O';
            entityText[(int)EntityType.NotUsedGoal] = 'G';
            entityText[(int)EntityType.UsedGoal] = 'U';
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

        static void StartGameLoop()
        {
            while (true)
            {
                if (!gameOver)
                {
                    CheckPlayerInput();
                    CalculateGameLogic();
                    RenderGameScreen();
                    gameOver = CheckGameClear();
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

            playerInputDir = new Vector2Int();
            switch (currentKeyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    playerInputDir.x -= 1;
                    break;

                case ConsoleKey.RightArrow:
                    playerInputDir.x += 1;
                    break;

                case ConsoleKey.UpArrow:
                    playerInputDir.y -= 1;
                    break;

                case ConsoleKey.DownArrow:
                    playerInputDir.y += 1;
                    break;
            }
        }

        private static void CalculateGameLogic()
        {
            HandlePlayerTurn();
            HandleEntityturn();
        }

        private static void HandleEntityturn()
        {
            CheckBoxGoal();
        }

        private static void CheckBoxGoal()
        {
            for (int goalIndex = 0; goalIndex < goalCount; goalIndex++)
            {
                for (int boxIndex = 0; boxIndex < boxCount; boxIndex++)
                {
                    if (!goalList[goalIndex].isGoalUsed && goalList[goalIndex].pos == boxPosList[boxIndex])
                    {
                        boxPosList[boxIndex] = new Vector2Int(-1, -1);
                        goalList[goalIndex].isGoalUsed = true;
                    }
                }
            }
        }

        private static void HandlePlayerTurn()
        {
            bool isPlayerMoveNotValid = false;
            
            // 한번에 충돌이나 오류의 유형이 한 가지밖에 일어날 수 없음. 복잡도 줄이기.
            isPlayerMoveNotValid |= CheckPlayerPosValid();
            if(!isPlayerMoveNotValid) isPlayerMoveNotValid |= CheckCollisionWall();
            if(!isPlayerMoveNotValid) isPlayerMoveNotValid |= CheckCollisionBox();

            if (!isPlayerMoveNotValid)
            {
                MovePlayer();
            }
        }

        private static bool CheckPlayerPosValid()
        {
            Vector2Int playerPosUpdated = playerPos + playerInputDir;
            if (playerPosUpdated.x < MIN_X || playerPosUpdated.y < MIN_Y || playerPosUpdated.x > MAX_X ||
                playerPosUpdated.y > MAX_Y)
            {
                return true;
            }

            return false;
        }
        private static bool CheckCollisionBox()
        {
            bool isPlayerBlocked = false;
            for (int i = 0; i < boxCount; i++)
            {
                if (playerPos + playerInputDir == boxPosList[i])
                {
                    if (CheckPushedEntityBlocked(boxPosList[i], playerInputDir))
                    {
                        return true;
                    }
                    else
                    {
                        boxPosList[i] += playerInputDir;
                    }
                }
            }

            return false;
        }

        private static bool CheckPushedEntityBlocked(Vector2Int pos, Vector2Int dir)
        {
            bool isBlocked = false;

            isBlocked |= CheckPushedEntityBlockedByBox(pos, dir);
            isBlocked |= CheckPushedEntityBlockedByWall(pos, dir);

            return isBlocked;
        }

        private static bool CheckPushedEntityBlockedByWall(Vector2Int pos, Vector2Int dir)
        {
            for (int i = 0; i < wallCount; i++)
            {
                if (pos + dir == wallPosList[i])
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckPushedEntityBlockedByBox(Vector2Int pos, Vector2Int dir)
        {
            for (int i = 0; i < boxCount; i++)
            {
                if (pos + dir == boxPosList[i])
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckCollisionWall()
        {
            for (int i = 0; i < wallCount; i++)
            {
                if ((playerPos + playerInputDir) == wallPosList[i])
                {
                    return true;
                }
            }

            return false;
        }

        private static void MovePlayer()
        {
            playerPos += playerInputDir;
        }

        private static bool CheckGameClear()
        {
            for (int i = 0; i < goalCount; i++)
            {
                if (!goalList[i].isGoalUsed)
                {
                    return false;
                }
            }

            return true;
        }

        static void RenderGameScreen()
        {
            Console.Clear();
            PrintEntities();
        }

        private static void PrintEntities()
        {
            PrintWalls();
            PrintBoxes();
            PrintGoals();
            PrintEntityAtPos(playerPos, EntityType.Player);
        }

        private static void PrintWalls()
        {
            for (int i = 0; i < wallCount; i++)
            {
                PrintEntityAtPos(wallPosList[i], EntityType.Wall);
            }
        }

        private static void PrintBoxes()
        {
            for (int i = 0; i < boxCount; i++)
            {
                PrintEntityAtPos(boxPosList[i], EntityType.Box);
            }
        }

        private static void PrintGoals()
        {
            for (int i = 0; i < goalCount; i++)
            {
                if (!goalList[i].isGoalUsed)
                {
                    PrintEntityAtPos(goalList[i].pos, EntityType.NotUsedGoal);
                }
                else
                {
                    PrintEntityAtPos(goalList[i].pos, EntityType.UsedGoal);
                }
            }
        }

        static void PrintEntityAtPos(Vector2Int pos, EntityType type)
        {
            // false value
            if (pos.x == -1 && pos.y == -1)
            {
                return;
            }
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(entityText[(int)type]);
        }
    }
}