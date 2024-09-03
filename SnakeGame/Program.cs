using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static int width = 20;
        static int height = 10;
        static int[,] grid;
        static int score = 0;
        static int level = 1;
        static int speed = 500; // Initial speed
        static int pointsPerLevel = 50; // Points required to level up
        static Direction currentDirection;
        static bool gameOver = false;

        static Snake snake;
        static Point food;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            do
            {
                InitializeGame();

                while (!gameOver)
                {
                    Input();
                    GameLogic();
                    Draw();
                    Thread.Sleep(speed);
                }

                Console.Clear();
                Console.SetCursorPosition(width / 2 - 5, height / 2);
                Console.WriteLine($"Game Over! Score: {score}");
            } while (RestartGame());
        }

        static void InitializeGame()
        {
            grid = new int[height, width];
            snake = new Snake(width / 2, height / 2);
            currentDirection = Direction.Right;
            score = 0;
            level = 1;
            speed = 500;
            gameOver = false;
            GenerateFood();
        }

        static void Draw()
        {
            Console.Clear();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        Console.Write("#"); // Draw border
                    }
                    else if (x == food.X && y == food.Y)
                    {
                        Console.Write("F"); // Draw food
                    }
                    else if (grid[y, x] > 0)
                    {
                        Console.Write("o"); // Draw snake
                    }
                    else
                    {
                        Console.Write(" "); // Empty space
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine($"Score: {score}  Level: {level}");
        }

        static void Input()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentDirection != Direction.Down)
                            currentDirection = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentDirection != Direction.Up)
                            currentDirection = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (currentDirection != Direction.Right)
                            currentDirection = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (currentDirection != Direction.Left)
                            currentDirection = Direction.Right;
                        break;
                }
            }
        }

        static void GameLogic()
        {
            Point nextPosition = GetNextPosition(snake.Head, currentDirection);

            // Check for collisions
            if (nextPosition.X <= 0 || nextPosition.X >= width - 1 ||
                nextPosition.Y <= 0 || nextPosition.Y >= height - 1 ||
                grid[nextPosition.Y, nextPosition.X] > 0)
            {
                gameOver = true;
                return;
            }

            if (nextPosition.X == food.X && nextPosition.Y == food.Y)
            {
                snake.Grow(nextPosition);
                score += 10;
                GenerateFood();
                CheckLevelUp();
            }
            else
            {
                snake.Move(nextPosition);
            }

            // Update grid
            Array.Clear(grid, 0, grid.Length);
            foreach (var part in snake.Parts)
            {
                grid[part.Y, part.X] = 1;
            }
        }

        static Point GetNextPosition(Point currentPosition, Direction direction)
        {
            Point nextPosition = new Point(currentPosition.X, currentPosition.Y);
            switch (direction)
            {
                case Direction.Up:
                    nextPosition.Y--;
                    break;
                case Direction.Down:
                    nextPosition.Y++;
                    break;
                case Direction.Left:
                    nextPosition.X--;
                    break;
                case Direction.Right:
                    nextPosition.X++;
                    break;
            }
            return nextPosition;
        }

        static void GenerateFood()
        {
            Random random = new Random();
            do
            {
                food = new Point(random.Next(1, width - 1), random.Next(1, height - 1));
            } while (grid[food.Y, food.X] > 0);
        }

        static bool RestartGame()
        {
            Console.SetCursorPosition(width / 2 - 10, height / 2 + 2);
            Console.WriteLine("Do you want to play again? (y/n)");

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
            } while (keyInfo.Key != ConsoleKey.Y && keyInfo.Key != ConsoleKey.N);

            return keyInfo.Key == ConsoleKey.Y;
        }

        static void CheckLevelUp()
        {
            if (score >= level * pointsPerLevel)
            {
                level++;
                speed = Math.Max(50, speed - 20); // Increase speed (lower sleep time) each level
            }
        }
    }

    enum Direction { Up, Down, Left, Right }

    class Snake
    {
        public LinkedList<Point> Parts { get; private set; }
        public Point Head => Parts.First.Value;

        public Snake(int startX, int startY)
        {
            Parts = new LinkedList<Point>();
            Parts.AddFirst(new Point(startX, startY));
        }

        public void Move(Point newHead)
        {
            Parts.AddFirst(newHead);
            Parts.RemoveLast(); // Remove tail
        }

        public void Grow(Point newHead)
        {
            Parts.AddFirst(newHead);
        }
    }

    struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
