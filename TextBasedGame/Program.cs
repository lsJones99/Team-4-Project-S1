using System;

namespace TextBasedGame
{
    internal class Program
    {


        //Player information
        const int MAX_INVENTORY_SIZE = 5;
        static string playerName = "";
        static int playerHealth = 100;
        static string[] inventory = new string[MAX_INVENTORY_SIZE];
        static int inventoryCount = 0;

        //Randomizer
        static Random rand = new Random();
        static int randomEnemyEncounter = 0;


        //Enemies
        static string[] enemies = ["Skeletons", "Ghouls", "Zombie"];
        static string currentEnemy = Enemy(enemies);

        // Main void for running game then ending

        static void Main()
        {
            bool gameRunning = true;

            Intro();

            while (gameRunning)
            {
                gameRunning = Menu();
            }

            Console.WriteLine("\nThank you for playing.");
            Console.ReadLine();
        }

        // Intro
        static void Intro()
        {
            Console.WriteLine("==============================");
            Console.WriteLine("       MEDIEVAL FANTASY");
            Console.WriteLine("==============================");
            Console.Write("Hero, what is your name? ");
            playerName = Console.ReadLine();
            Console.WriteLine("\nYou enter a dark forest.");
            Console.WriteLine("Something is moving between the trees...");
        }

        // Menu commands
        static bool Menu()
        {
            Console.WriteLine("\n---------- ACTIONS ----------");
            Console.WriteLine("[F] Fight");
            Console.WriteLine("[R] Run");
            Console.WriteLine("[I] Investigate");
            Console.WriteLine("[U] Use item");
            Console.WriteLine("[V] View inventory");
            Console.WriteLine("[Q] Quit");
            Console.WriteLine($"Name: {playerName}");
            Console.WriteLine($"Health: {playerHealth}/100");

            Console.Write("\nEnter command: ");
            string userInput = Console.ReadLine().Trim().ToLower();

            if (userInput.Length == 0)
            {
                Console.WriteLine("You did not enter a command.");
                return true;
            }

            char user = userInput[0];

            switch (user)
            {
                case 'f':
                    return Fight(currentEnemy);

                case 'r':
                    Run();
                    break;

                case 'i':
                    Investigate();
                    break;

                case 'v':
                    ViewInventory();
                    break;

                case 'q':
                    return false;

                default:
                    Console.WriteLine("Unrecognised command.");
                    break;
            }

            return true;
        }


        //Enemy generator
        static string Enemy(string[] enemyArray)
        {
            randomEnemyEncounter = rand.Next(0, enemyArray.Length);
            return enemyArray[randomEnemyEncounter];
        }


        // Fight command
        static bool Fight(string enemyName)
        {
            Console.WriteLine("An encounter happens!");
            Console.WriteLine($"-> You engage in battle with the {enemyName}");

            int damage = rand.Next(5, 16);
            playerHealth -= damage;

            Console.WriteLine($"You defeat the {enemyName} but take {damage} damage.");

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                Console.WriteLine("You have been defeated.");
                return false;
            }

            return true;
        }

        // Run command
        static void Run()
        {
            Console.WriteLine("Which direction do you want to go to?");
            Console.WriteLine("A for west,W for north, S for south, D for east");
            char playerChoice = char.ToLower(Console.ReadKey().KeyChar);
            switch (playerChoice)
            {
                case 'a':
                    Console.WriteLine($"{playerName} moves west");
                    break;
                case 'w':
                    Console.WriteLine($"{playerName} moves north");
                    break;
                case 'd':
                    Console.WriteLine($"{playerName} moves east");
                    break;
                case 's':
                    Console.WriteLine($"{playerName} moves south");
                    break;
            }
        }


        // Investigation command
        static void Investigate()
        {
            int result = rand.Next(1, 4);

            switch (result)
            {
                case 1:
                    Console.WriteLine("\nYou find a Healing Potion.");
                    AddItem("Healing Potion");
                    break;

                case 2:
                    Console.WriteLine("\nYou find a Rusty Sword.");
                    AddItem("Rusty Sword");
                    break;

                case 3:
                    Console.WriteLine("\nYou search the area but find nothing.");
                    break;
            }
        }

        // Adding items to the inventory
        static void AddItem(string item)
        {
            if (inventoryCount < MAX_INVENTORY_SIZE)
            {
                inventory[inventoryCount] = item;
                inventoryCount++;

                Console.WriteLine($"{item} was added to your inventory.");
            }
            else
            {
                Console.WriteLine("Your inventory is full.");
            }
        }

        // View inv & inv commands
        static void ViewInventory()
        {
            Console.WriteLine("\n---------- INVENTORY ----------");

            if (inventoryCount == 0)
            {
                Console.WriteLine("Your inventory is empty.");
                return;
            }

            for (int i = 0; i < inventoryCount; i++)
            {
                Console.WriteLine($"{i + 1}. {inventory[i]}");
            }

            Console.WriteLine($"Inventory slots: {inventoryCount}/{MAX_INVENTORY_SIZE}");

            Console.WriteLine("\nWhat do you want to do?");
            Console.WriteLine("\n[U] Use an item");
            Console.WriteLine("[T] Throw out an item");
            Console.WriteLine("[ANY OTHER INPUT] Nothing");

            char invChoice = Convert.ToChar(Console.ReadKey());

            switch (invChoice)
            {
                case 'u':
                    UseItems();
                    break;

                case 't':
                    ThrowItems();
                    break;

                default:
                    Menu();
                    break;
            }

            // Using items
            static void UseItems()
            {
                
            }

            // Disposing of items
            static void ThrowItems()
            {

            }
        }
    }
}