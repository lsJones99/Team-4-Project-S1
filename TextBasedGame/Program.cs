namespace TextBasedGame
{
    internal class Program
    {


        //Player information
        static int MAX_INVENTORY_SIZE = 5;
        static string playerName = "";
        static int playerHealth = 100;
        static int playerMaxHealth = 100;
        static string[] inventory = new string[MAX_INVENTORY_SIZE];
        static int inventoryCount = 0;
        static int playerDamage = 0;
        static int playerStrength = 0;
        static int playerFinesse = 0;
        static int playerResilience = 0;
        static int playerArcane = 0;


        //Randomizer
        static Random rand = new Random();
        static int randomEnemyEncounter = 0;
        static int escapeChance = 0;


        //Enemies
        static string[] enemies = ["Skeleton", "Ghoul", "Zombie"];
        static int enemyDamage = 0;

        // Main void for running game then ending

        static void Main()
        {
            bool gameRunning = true;

            Intro();
            CreateCharacter();
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
            Console.Clear();
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
        // Character Selection
        public static void CreateCharacter()
        {
            string userInput = "";
            string verify = "";
            int check = 0;
            bool valid = false;
            do
            {
                Console.WriteLine("Class selection prompt");
                do
                {
                    userInput = Console.ReadLine()?.Trim().ToLower() ?? "";
                } while (userInput.Length == 0);
                char user = Convert.ToChar(userInput[0]);
                switch (user)
                {
                    case 'w':
                        Console.WriteLine("Warrior text");
                        playerStrength = 5;
                        playerFinesse = 2;
                        playerResilience = 3;
                        valid = true;
                        break;
                    case 'r':
                        Console.WriteLine("Rogue text");
                        valid = true;
                        break;
                    case 'm':
                        Console.WriteLine("Mage text");
                        valid = true;
                        break;
                    case 'c':
                        CustomStats();
                        valid = true;
                        break;
                    default:
                        Console.WriteLine("Invalid");
                        break;
                }
                Console.WriteLine("Verify prompt");
                do
                {
                    verify = Console.ReadLine()?.Trim().ToLower() ?? "";
                } while (verify.Length == 0);
                char VerifyCheck = Convert.ToChar(verify[0]);
                do
                {
                    switch (VerifyCheck)
                    {
                        case 'y':
                            Console.WriteLine("Confirm prompt");
                            check = 1;
                            break;
                        case 'n':
                            Console.WriteLine("Negative prompt");
                            valid = false;
                            check = 1;
                            break;
                        default:
                            Console.WriteLine("Error prompt");
                            break;
                    }
                } while (check == 0);
            } while (valid == false);
            MAX_INVENTORY_SIZE = MAX_INVENTORY_SIZE + playerStrength;
            inventory = new string[MAX_INVENTORY_SIZE];
            playerMaxHealth = playerMaxHealth + (10 * playerResilience);
            playerHealth = playerHealth + (10 * playerResilience);

        }
        // Custom Character
        static void CustomStats()
        {
            Console.WriteLine("Custom Stats Text");
        }
        // Menu commands
        static bool Menu()
        {
            Console.Clear();
            Console.WriteLine("\n---------- EXPLORATION ----------");
            Console.WriteLine("[R] Start Navigating");
            Console.WriteLine("[I] Investigate");
            Console.WriteLine("[V] View inventory");
            Console.WriteLine("[Q] Quit");
            Console.WriteLine($"Name: {playerName}");
            Console.WriteLine($"Health: {playerHealth}/{playerMaxHealth}");

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
                    PauseStory();
                    break;

                case 'v':
                    ViewInventory();
                    break;
                case 'q':
                    return false;

                default:
                    Console.WriteLine("Unrecognised command.");
                    PauseStory();
                    break;
            }

            return playerHealth > 0;
        }


        //Combat commands
        static bool CombatMenu(string enemyName)
        {
            int enemyHealth = 50;
            bool inCombat = true;

            Console.Clear();

            Console.WriteLine("\n========== ENCOUNTER ==========");
            Console.WriteLine($"A {enemyName} approaches!");
            Console.WriteLine("\nPress Enter to begin combat.");
            Console.ReadLine();

            while (inCombat && playerHealth > 0 && enemyHealth > 0)
            {

                Console.Clear();

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
                        //implement magic as the hero progresses
                        Console.WriteLine("You have not learned any magic yet.");
                        Console.WriteLine("\nPress Enter to continue.");
                        Console.ReadLine();
                        continue;

                    case 'i':
                        //item use to be implemented later on
                        Console.WriteLine("Item use is not available during combat yet.");
                        Console.WriteLine("\nPress Enter to continue.");
                        Console.ReadLine();
                        continue;

                    case 'r':
                        escapeChance = rand.Next(1, 101);

                        if (escapeChance <= 50)
                        {
                            Console.WriteLine($"You have successfully escaped from {enemyName}!");
                            inCombat = false;
                        }
                        else
                        {
                            Console.WriteLine("You failed to escape!");
                        }
                        break;

                    default:
                        Console.WriteLine("Unrecognised combat command.");
                        continue;
                }

                if (!inCombat)
                {
                    Console.WriteLine("\nPress Enter to return to exploration.");
                    Console.ReadLine();
                    break;
                }


                //check if enemy hp is 0 or less
                if (enemyHealth <= 0)
                {
                    enemyHealth = 0;
                    Console.WriteLine($"You have defeated the {enemyName}!");
                    Console.WriteLine("\nPress Enter to return to exploration.");
                    Console.ReadLine();
                    break;
                }

                //enemy attacks
                enemyDamage = rand.Next(5, 11);
                playerHealth -= enemyDamage;
                Console.WriteLine($"The {enemyName} attacks you for {enemyDamage} damage!");

                //check for player hp
                if (playerHealth <= 0)
                {
                    playerHealth = 0;
                    Console.WriteLine($"{playerName} has been defeated.");
                    inCombat = false;
                }

                //Combat pause
                if (inCombat && playerHealth > 0 && enemyHealth > 0)
                {
                    Console.WriteLine("\nPress Enter for the next turn.");
                    Console.ReadLine();
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
            Console.Clear();
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
                    PauseStory();
                    return;

                default:
                    Console.WriteLine("Enter a valid navigation command.");
                    PauseStory();
                    return;
            }

            Console.WriteLine($"{playerName} moves {direction}.");

            CheckForEncounter();

            if (playerHealth > 0)
            {
                PauseStory();
            }
        }

        //Check for enemy encounter
        static void CheckForEncounter()
        {
            int encounterChance = rand.Next(1, 101);

            if (encounterChance <= 30)
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
            Console.Clear();

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

            char invChoice = char.ToLower(Console.ReadKey(true).KeyChar);

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