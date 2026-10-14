using System.Security.Cryptography.X509Certificates;

namespace TextBasedGame
{
    internal class Program
    {


        //Player information
        static int MAX_INVENTORY_SIZE = 5;
        static string playerName = "";
        static int playerHealth = 100;
        static int playerMaxHealth = 100;
        static int playerMana = 5;
        static int playerMaxMana = 5;
        static string[] inventory = new string[MAX_INVENTORY_SIZE];
        static int inventoryCount = 0;
        static int playerDamage = 0;
        static int playerStrength = 0;
        static int playerMeleeHit = 0;
        static int playerMeleeDmg = 0;
        static int playerFinesse = 0;
        static int playerRangeHit = 0;
        static int playerRangeDmg = 0;
        static int playerResilience = 0;
        static int playerArcane = 0;
        static int playerSpellMiscast = 0;
        static int playerSpellDmg = 0;
        static string equippedWpn = "";
        static string equippedAmr = "";


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
            Console.OutputEncoding = System.Text.Encoding.UTF8;
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

            Console.WriteLine("A mysterious voice echoes inside your mind...");
            PauseStory();

            Console.Write("\"Hero, tell me your name.\" ");
            playerName = Console.ReadLine();

            //prevents the players from having a blank name
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Anon";
            }

            Console.WriteLine($"\"{playerName}... A fitting name for the one chosen to save our world.\"");
            PauseStory();

            Console.WriteLine("You demand answers, but the mysterious voice ignores your questions.");
            PauseStory();

            Console.WriteLine("\"Our kingdom is being ravaged by creatures of darkness.\"");
            PauseStory();

            Console.WriteLine("\"Travel north and find the village of Greymoor. Your journey begins there.\"");
            PauseStory();

            Console.WriteLine("Before you can ask anything else, the voice disappears.");
            PauseStory();

            Console.WriteLine("You are alone. You have no idea where you are or how you arrived here.");
            PauseStory();

            Console.WriteLine("Suddenly, you hear something moving in the nearby bushes...");
            PauseStory();

            Console.WriteLine("You take a deep breath and prepare to move.");
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
                Console.Clear();
                Console.WriteLine("\n---------- CHOOSE YOUR CLASS ----------");
                Console.WriteLine("[W] Warrior - strong and tough");
                Console.WriteLine("[R] Rogue   - fast and resilient");
                Console.WriteLine("[M] Mage    - arcane power");
                Console.WriteLine("[C] Custom  - choose your own stats");
                Console.Write("\nEnter a letter: ");
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
                        playerArcane = 0;
                        valid = true;
                        break;
                    case 'r':
                        Console.WriteLine("Rogue text");
                        playerStrength = 2;
                        playerFinesse = 5;
                        playerResilience = 2;
                        playerArcane = 1;
                        valid = true;
                        break;
                    case 'm':
                        Console.WriteLine("Mage text");
                        playerStrength = 1;
                        playerFinesse = 2;
                        playerResilience = 1;
                        playerArcane = 6;
                        valid = true;
                        break;
                    case 'c':
                        CustomStats();
                        valid = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter W, R, M, or C.");
                        continue;
                }
                Console.Clear();
                Console.WriteLine("\n---------- Stat Allocation ----------\n");
                Console.WriteLine($"Strength   : {playerStrength}\n");
                Console.WriteLine($"Finesse    : {playerFinesse}\n");
                Console.WriteLine($"Resilience : {playerResilience}\n");
                Console.WriteLine($"Arcane     : {playerArcane}\n");
                Console.Write("Are you happy with this class? (Y/N): ");
                check = 0;
                while (check == 0)
                {
                    verify = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (verify.StartsWith("y"))
                    {
                        Console.WriteLine("Confirm prompt");
                        check = 1;
                    }
                    else if (verify.StartsWith("n"))
                    {
                        Console.WriteLine("Negative prompt");
                        valid = false;
                        check = 1;
                    }
                    else
                    {
                        Console.WriteLine("Please enter Y for yes or N for no.");
                    }
                }
            } while (valid == false);
            MAX_INVENTORY_SIZE = MAX_INVENTORY_SIZE + playerStrength;
            inventory = new string[MAX_INVENTORY_SIZE];
            playerMaxHealth = playerMaxHealth + (10 * playerResilience);
            playerHealth = playerHealth + (10 * playerResilience);
            playerMaxMana = playerMaxMana + (3 * playerArcane);
            playerMana = playerMana + (3 * playerArcane);
            playerMeleeHit = playerMeleeHit + playerStrength;
            playerMeleeDmg = playerMeleeDmg + (playerStrength % 2);
            playerRangeHit = playerRangeHit + playerFinesse;
            playerRangeDmg = playerRangeDmg + (playerFinesse % 2);
            playerSpellMiscast = playerSpellMiscast - (5 * playerArcane);
            playerSpellDmg = playerSpellDmg + (playerArcane % 2);
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
            Console.WriteLine("[M] Map");
            Console.WriteLine($"Name: {playerName}");
            Console.WriteLine($"Health: {playerHealth}/{playerMaxHealth}");

            Console.Write("\nEnter command: ");
            string userInput = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (userInput.Length == 0)
            {
                Console.WriteLine("You did not enter a command.");
                PauseStory();
                return true;
            }

            char temp = userInput[0];

            switch (temp)
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
                case 'm':
                    Map();
                    break;

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
                Console.WriteLine($"{playerName}: {playerHealth}/{playerMaxHealth} HP");
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
                    PauseStory();
                    return true;
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
                PauseStory();
                return;
            }

            for (int i = 0; i < inventory.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {inventory[i]}");
            }

            Console.WriteLine($"Inventory slots: {inventoryCount}/{MAX_INVENTORY_SIZE}");

            Console.WriteLine($"\nEquipped weapon: {equippedWpn}");
            Console.WriteLine($"Equipped armor: {equippedAmr}");

            Console.WriteLine("\nWhat do you want to do?");
            Console.WriteLine("\n[U] Use an item");
            Console.WriteLine("[T] Throw out an item");
            Console.WriteLine("[ANY OTHER INPUT] Return");

            char invChoice = Console.ReadLine()[0];
            Console.Clear();

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
                Console.WriteLine("\n---------- INVENTORY ----------");

                for (int i = 0; i < inventory.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {inventory[i]}");
                }

                Console.WriteLine($"Inventory slots: {inventoryCount}/{MAX_INVENTORY_SIZE}");

                Console.WriteLine("\nWhich slot do you wish to use?");
                int useChoice = Convert.ToInt32(Console.ReadLine());
                Console.Clear();
                Console.WriteLine("\n---------- INVENTORY ----------");

                // What happens when using a healing potion
                if (inventory[useChoice-1] == "Healing Potion")
                {
                    playerHealth = playerHealth + 30;
                    inventory[useChoice - 1] = null;
                    Console.WriteLine("The healing potion was used! +30 HP");
                    if (playerHealth > playerMaxHealth)
                    {
                        playerHealth = playerMaxHealth;
                    }
                    Console.WriteLine($"Your HP is now {playerHealth}/{playerMaxHealth}.");
                    PauseStory();
                }
                // What happens when using a rusty sword
                else if (inventory[useChoice-1] == "Rusty Sword")
                {
                    // Checks if equipped wpn is empty, clears old inv slot if true, replaces if not
                    if (equippedWpn != "")
                    {
                        inventory[useChoice-1] = equippedWpn;
                    }
                    else
                    {
                        inventory[useChoice-1] = null;
                        inventoryCount -= 1;
                    }
                    equippedWpn = "Rusty Sword";
                    Console.WriteLine("You equipped the Rusty Sword.");
                    PauseStory();
                }
                else 
                {
                    Console.WriteLine("That input is unrecognised.");
                    PauseStory();
                }
            }

            // Disposing of items
            static void ThrowItems()
            {

            }
        }
        // Map
        public static void Map()
        {
            Console.Clear ();

            Console.WriteLine(@"
                                                  #####    #####     ###    #######   #####   ######   #######  ######    #####
                                                 ##   ##  ##   ##   ## ##      ##    ##   ##  ##   ##  ##       ##   ##  ##   ##
                                                 ##   ##  ##       ##   ##     ##    ##   ##  ##   ##  ##       ##   ##  ##
                                                 ##   ##  ##  ###  #######     ##    ##   ##  ######   ######   ######   ##  ###
                                                 ##   ##  ##   ##  ##   ##     ##    ##   ##  ##   ##  ##       ## ##    ##   ##
                                                 ##   ##  ##   ##  ##   ##     ##    ##   ##  ##   ##  ##       ##  ##   ##   ##
                                                  #####    #####   ##   ##     ##     #####   ######   #######  ##   ##   #####
                                            ");
            Console.WriteLine(@"
                                +----------------------+----------------------+----------------------+----------------------+----------------------+
                                |          /\          |        /^^^^\        |        \ | /         |      |^^| |^^|       |      .-.   .-.       |
                                |       __/~~\__       |       /  __  \       |     --- (O) ---      |     _|[]|_|[]|_      |     (o o) (o o)      |
                                |       |  []  |       |     /___/  \___\     |     ~~~/|~~~~~~      |     |___GATE__|      |      \_/   \_/       |
                                |    The witch hut     |      Dark cave       |        Beach         |  The light kingdom   |      Evil twin       |
                                |                      |                      |                      |                      |                      |
                                +----------------------+----------------------+----------------------+----------------------+----------------------+
                                |        /\  /\        |       .-====-.       |       _ _ _ _        |       /\   /\        |       __    __       |
                                |       /##\/##\       |       | ~~~~ |       |      /_/_/_/_\       |      /  \ /  \       |      |  |__|  |      |
                                |       \__||__/       |       |__(O)_|       |      |$|$|$|$|       |        ||  ||        |      |__|  |__|      |
                                |    The royal army    |    Temple of the     |      The market      |      Wilderness      |   Ancient grounds    |
                                |                      |    Spaghetti God     |                      |                      |                      |
                                +----------------------+----------------------+----------------------+----------------------+----------------------+
                                |        .-O-.         |       /\   /\        |      /\/\ /\/\       |        +   +         |     |^^|   |^^|      |
                                |       /|###|\        |      /  \ /  \       |      /\/\\/\/\       |       /|\ /|\        |     |  |___|  |      |
                                |        /   \         |        ||  ||        |       || @ ||        |       / \ / \        |     |__|   |__|      |
                                |    The lost Giant    |      Wilderness      |     Dark forest      |      Crusaders       |    The lost city     |
                                |                      |                      |                      |                      |                      |
                                +----------------------+----------------------+----------------------+----------------------+----------------------+
                                |       /\_/\_/\       |       .-----.        |      ~~~  ~~~~       |        /\  /\        |      [##]  ___       |
                                |       | 666 |        |      <  (O)  >       |      ~~~\ /~~~       |       /++\/++\       |      _||_ /__/       |
                                |       |_/\/_|        |       '-----'        |      ~~~~~V~~~~      |       |__||__|       |     (____) /_o_      |
                                |     Gate to Hell     |  The all seeing Eye  |    The holy river    |  Camp of the Light   |       Junkyard       |
                                |                      |                      |                      |       knights        |                      |
                                +----------------------+----------------------+----------------------+----------------------+----------------------+
                                |      .  .  :  .      |      |^^^|^^^|       |      .--------.      |     ~~~ \o/ ~~~      |      ~~~ /\ ~~~      |
                                |       .  __  .       |      | [] [] |       |      | 777 $$$|      |     ~~~~ | ~~~~      |      ~~ /__\ ~~      |
                                |      ~~  ~~  ~~      |      |___[]__|       |      '--------'      |     ~~~~ / \ ~~~     |      ~~~ || ~~~      |
                                |  Sands of lost ones  |    Renard's keep     |        Casino        |    song of siren     |     Gnome Island     |
                                |                      |                      |                      |                      |                      |
                                +----------------------+----------------------+----------------------+----------------------+----------------------+");

            Console.ReadLine();
        }
    }
}