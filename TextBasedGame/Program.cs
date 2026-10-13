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
        static int playerDamage = 0;

        //Randomizer
        static Random rand = new Random();
        static int randomEnemyEncounter = 0;


        //Enemies
        static string[] enemies = ["Skeleton", "Ghoul", "Zombie"];
        static int enemyDamage = 0;

        // Main void for running game then ending

        static void Main()
        {
            bool gameRunning = true;

            Intro();

            while (gameRunning && playerHealth > 0)
            {
                gameRunning = Menu();
            }

            Console.WriteLine("\nThank you for playing.");
            Console.ReadLine();
        }

        static void PauseStory()
        {
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }
        // Intro
        static void Intro()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       MEDIEVAL FANTASY");
            Console.WriteLine("==============================");

            Console.WriteLine("\nYou are a student from Otago Polytechnic, walking home after a long day of classes.");
            PauseStory();

            Console.WriteLine("Without warning, a blinding light surrounds you.");
            PauseStory();

            Console.WriteLine("When you open your eyes, you find yourself lying in an unfamiliar forest.");
            PauseStory();

            Console.WriteLine("Towering trees surround you, and a cold wind moves through their branches.");
            PauseStory();

            Console.WriteLine("\nA mysterious voice echoes inside your mind...");
            PauseStory();

            Console.Write("\n\"Hero, tell me your name.\" ");
            playerName = Console.ReadLine();

            //prevents the players from having a blank name
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Anon";
            }
            
            Console.WriteLine($"\n\"{playerName}... A fitting name for the one chosen to save our world.\"");
            PauseStory();

            Console.WriteLine("\nYou demand answers, but the mysterious voice ignores your questions.");
            PauseStory();

            Console.WriteLine("\"Our kingdom is being ravaged by creatures of darkness.\"");
            PauseStory();

            Console.WriteLine("\"Travel north and find the village of Greymoor. Your journey begins there.\"");
            PauseStory();

            Console.WriteLine("\nBefore you can ask anything else, the voice disappears.");
            PauseStory();

            Console.WriteLine("You are alone. You have no idea where you are or how you arrived here.");
            PauseStory();

            Console.WriteLine("\nSuddenly, you hear something moving in the nearby bushes...");
            PauseStory();

            Console.WriteLine("\nYou take a deep breath and prepare to move.");
            Console.WriteLine("Your journey begins now.");

            PauseStory();
        }

        // Menu commands
        static bool Menu()
        {
            Console.WriteLine("\n---------- EXPLORATION ----------");
            Console.WriteLine("[R] Start Navigating");
            Console.WriteLine("[I] Investigate");
            Console.WriteLine("[V] View inventory");
            Console.WriteLine("[Q] Quit");
            Console.WriteLine($"Name: {playerName}");
            Console.WriteLine($"Health: {playerHealth}/100");

            Console.Write("\nEnter command: ");
            string userInput = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (userInput.Length == 0)
            {
                Console.WriteLine("You did not enter a command.");
                return true;
            }

            char user = userInput[0];

            switch (user)
            {
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

            return playerHealth > 0;
        }


        //Combat commands
        static bool CombatMenu(string enemyName)
        {
            int enemyHealth = 50;
            bool inCombat = true;
            Console.WriteLine("\n========== ENCOUNTER ==========");
            Console.WriteLine($"A {enemyName} approaches!");

            while (inCombat && playerHealth > 0 && enemyHealth > 0)
            {
                Console.WriteLine("\n---------- COMBAT ----------");
                Console.WriteLine($"{playerName}: {playerHealth}/100 HP");
                Console.WriteLine($"{enemyName}: {enemyHealth}/50 HP");

                Console.WriteLine("\n[A] Attack");
                Console.WriteLine("[M] Magic");
                Console.WriteLine("[I] Use item");
                Console.WriteLine("[R] Run away");

                Console.Write("\nChoose an action: ");
                string combatInput = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (combatInput.Length == 0)
                {
                    Console.WriteLine("You did not enter a command.");
                    continue;
                }

                char combatChoice = combatInput[0];

                switch (combatChoice)
                {
                    case 'a':
                        playerDamage = rand.Next(8, 16);
                        enemyHealth -= playerDamage;
                        Console.WriteLine($"You attack the {enemyName} for {playerDamage} damage.");
                        break;

                    case 'm':
                        // Magic can be implemented later.
                        Console.WriteLine("You have not learned any magic yet.");
                        continue;

                    case 'i':
                        // Inventory use can be connected later.
                        Console.WriteLine("Item use is not available during combat yet.");
                        continue;

                    case 'r':
                        // Generate an escape chance.
                        // Change inCombat to false if successful.
                        break;

                    default:
                        Console.WriteLine("Unrecognised combat command.");
                        continue;
                }


                //check if enemy hp is 0 or less
                if (enemyHealth <= 0)
                {
                    enemyHealth = 0;
                    Console.WriteLine($"You have defeated the {enemyName}");
                    inCombat = false;
                    continue;
                }

                //enemy attacks
                enemyDamage = rand.Next(5, 11);
                playerHealth -= enemyDamage;
                Console.WriteLine($"The {enemyName} attacks you for {enemyDamage} damage!");

                //check for player hp
                if(playerHealth <= 0)
                {
                    playerHealth = 0;
                    Console.WriteLine($"{playerName} has been defeated.");
                    inCombat = false;
                }

                
            }
                return true;
        }

        //Enemy generator
        static string Enemy(string[] enemyArray)
        {
            randomEnemyEncounter = rand.Next(0, enemyArray.Length);
            return enemyArray[randomEnemyEncounter];
        }


        // Run command
        static void Run()
        {
            Console.WriteLine("\n---------- NAVIGATION ----------");
            Console.WriteLine("[W] Move north");
            Console.WriteLine("[A] Move west");
            Console.WriteLine("[S] Move south");
            Console.WriteLine("[D] Move east");
            Console.WriteLine("[Q] Stop navigating");

            Console.Write("\nChoose a direction: ");
            char playerChoice = char.ToLower(Console.ReadKey(true).KeyChar);

            Console.WriteLine();

            string direction = "";

            switch (playerChoice)
            {
                case 'w':
                    direction = "north";
                    break;

                case 'a':
                    direction = "west";
                    break;

                case 's':
                    direction = "south";
                    break;

                case 'd':
                    direction = "east";
                    break;

                case 'q':
                    Console.WriteLine("You stop navigating.");
                    return;

                default:
                    Console.WriteLine("Enter a valid navigation command.");
                    return;
            }

            Console.WriteLine($"{playerName} moves {direction}.");

            CheckForEncounter();
        }

        //Check for enemy encounter
        static void CheckForEncounter()
        {
            int encounterChance = rand.Next(1, 101);

            if(encounterChance <= 30)
            {
                string encounteredEnemy = Enemy(enemies);

                Console.WriteLine($"\nA {encounteredEnemy} suddenly appears!");

                //Combat system function
                CombatMenu(encounteredEnemy);
            }
            else
            {
                Console.WriteLine("The path is clear.");
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
                    Console.WriteLine("Returning to exploration.");
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