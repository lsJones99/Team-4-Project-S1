
﻿using System;
using System.Collections.Generic;
using System.Collections.Generic;
﻿using System.ComponentModel.Design;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace TextBasedGame
{
    internal class Program
    {
        // savedata
        private static readonly string saveFile = "savedata.txt";

        // importing command to find console window
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        // importing command to change console window properties
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr wHandle, int nCmdShow);

        // 3 is the Windows constant for Maximized
        private const int SW_MAXIMIZE = 3;
        //Player information
        static int inventoryMaxSize = 5;
        static string playerName = "";
        static int playerHealth = 100;
        static int playerMaxHealth = 100;
        static int playerMana = 5;
        static int playerMaxMana = 5;
        static int playerLevel = 1;
        static int playerCurrentXP = 0;
        static int playerNextXP = 0;

        static int playerGold = 0;
        static string[] inventory = new string[inventoryMaxSize];
        static int inventoryCount = 0;
        static int playerDamage = 0;
        static int playerStrength = 0;
        static int playerMeleeHit = 0;
        static int playerMeleeDmg = 0;
        static int playerFinesse = 0;
        static int playerRangeHit = 0;
        static int playerRangeDmg = 0;
        static int playerRunBonus = 0;
        static int playerDodge = 0;
        static int playerCritChance = 0;
        static int playerResilience = 0;
        static int playerPhysicalResist = 0;
        static int playerBlockChance = 0;
        static int playerBlockReduction = 0;
        static int playerArcane = 0;
        static int playerSpellMiscast = 0;
        static int playerSpellDmg = 0;
        static int playerMagicResist = 0;
        static string playerWpn = "";
        static string playerAmr = "";
        static int playerX = 2; // Start in Dark Forest
        static int playerY = 2;
        static int markerScreenX;
        static int markerScreenY;


        //Randomizer
        static Random rand = new Random();
        static int randomEnemyEncounter = 0;
        static int escapeChance = 0;


        //Quest
        static bool activequest = false;
        static List<string> acceptedQuests = new List<string>();
        static List<string> completedQuests = new List<string>();
        static string trackedQuest = "";
        static Dictionary<string, QuestDefinition> questDefinitions =
            new Dictionary<string, QuestDefinition>();
        private static readonly string checkpointFolder = "quest_checkpoints";

        private sealed class QuestDefinition
        {
            public string Title;
            public string Description;
            public bool IsMainQuest;
            public string PrerequisiteId;
            public int MapX;
            public int MapY;

            public QuestDefinition(
                string title,
                string description,
                bool isMainQuest,
                string prerequisiteId,
                int mapX,
                int mapY)
            {
                Title = title;
                Description = description;
                IsMainQuest = isMainQuest;
                PrerequisiteId = prerequisiteId;
                MapX = mapX;
                MapY = mapY;
            }
        }
        //Enemies
        static string[] enemies = ["Skeleton", "Ghoul", "Zombie"];
        static int enemyDamage = 0;

        //Combat Information
        static List<string> combatLog = new List<string>();

        // Main void for running game then ending
        static void Main()
        {
            // all quest definitions
            RegisterQuests();
            // Loading a save if there is one
            Load();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool gameRunning = true;

            // getting console window ID then maximising window (for text centering purposes)
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_MAXIMIZE);

            Console.WriteLine("\nThis game is best played maximised.");
            PauseStory();

            if (File.Exists(saveFile) == false)
            {
                Intro();
                CreateCharacter();
            }

            while (gameRunning && playerHealth > 0)
            {
                gameRunning = Menu();
            }

            Console.WriteLine("\nThank you for playing.");
            Console.ReadLine();
        }
        // Saving progress
        static void Save()
        {
            Console.Clear();
            Console.WriteLine("\n---------- SAVE FILE ----------");
            Console.WriteLine("What do you wish to do?");
            Console.WriteLine("[S] Save (THIS WILL OVERWRITE ANY PREVIOUS SAVE)");
            Console.WriteLine("[D] Delete save");
            Console.WriteLine("[ANY OTHER INPUT] Return");

            string saveChoice = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(saveChoice))
            {
                return;
            }

            char saveInput = saveChoice[0];

            switch (saveInput)
            {
                case 's':
                    WriteGameSave(saveFile);

                    Console.Clear();
                    Console.WriteLine("\n---------- SAVE FILE ----------");
                    Console.WriteLine("Progress successfully saved!");
                    Console.WriteLine($"Save file: {Path.GetFullPath(saveFile)}");
                    PauseStory();
                    break;

                case 'd':
                    if (File.Exists(saveFile))
                    {
                        Console.Clear();
                        Console.WriteLine("\n---------- SAVE FILE ----------");
                        Console.WriteLine("Are you sure you want to delete the current save file?");
                        Console.WriteLine("[Y] Confirm");
                        Console.WriteLine("[ANY OTHER INPUT] Return");
                        string deleteChoice = Console.ReadLine()?.Trim().ToLower() ?? "";
                        if (string.IsNullOrWhiteSpace(deleteChoice))
                        {
                            break;
                        }

                        char deleteConf = deleteChoice[0];

                        switch (deleteConf)
                        {
                            case 'y':
                                File.Delete(saveFile);
                                if (Directory.Exists(checkpointFolder))
                                {
                                    Directory.Delete(checkpointFolder, true);
                                }

                                Console.Clear();
                                Console.WriteLine("\n---------- SAVE FILE ----------");
                                Console.WriteLine("Save file successfully deleted.");
                                PauseStory();
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\n---------- SAVE FILE ----------");
                        Console.WriteLine("\nThere is no save to delete!");
                        PauseStory();
                    }
                    break;

                default:
                    break;
            }
        }

        // Loading save
        static void Load()
        {
            if (ReadGameSave(saveFile))
            {
                Console.WriteLine("--- Settings Loaded from File ---");
                Thread.Sleep(3000);
            }
        }
        // Saves player information AND quest progress
        static void WriteGameSave(string path)
        {
            List<string> linesToSave = new List<string>
            {
                $"playerName={playerName}",
                $"inventory={string.Join(",", inventory)}",
                $"inventoryCount={inventoryCount}",
                $"inventoryMaxSize={inventoryMaxSize}",

                $"playerWpn={playerWpn}",
                $"playerAmr={playerAmr}",

                $"playerHealth={playerHealth}",
                $"playerMaxHealth={playerMaxHealth}",
                $"playerMana={playerMana}",
                $"playerMaxMana={playerMaxMana}",

                $"playerLevel={playerLevel}",
                $"playerCurrentXP={playerCurrentXP}",
                $"playerNextXP={playerNextXP}",

                $"playerGold={playerGold}",

                $"playerX={playerX}",
                $"playerY={playerY}",

                $"playerDamage={playerDamage}",
                $"playerStrength={playerStrength}",
                $"playerResilience={playerResilience}",
                $"playerFinesse={playerFinesse}",
                $"playerArcane={playerArcane}",

                $"playerMeleeDmg={playerMeleeDmg}",
                $"playerMeleeHit={playerMeleeHit}",
                $"playerRangeDmg={playerRangeDmg}",
                $"playerRangeHit={playerRangeHit}",

                $"playerRunBonus={playerRunBonus}",
                $"playerDodge={playerDodge}",
                $"playerCritChance={playerCritChance}",
                $"playerPhysicalResist={playerPhysicalResist}",
                $"playerBlockChance={playerBlockChance}",
                $"playerBlockReduction={playerBlockReduction}",
                $"playerMagicResist={playerMagicResist}",
                $"playerSpellDmg={playerSpellDmg}",
                $"playerSpellMiscast={playerSpellMiscast}",

                $"markerScreenX={markerScreenX}",
                $"markerScreenY={markerScreenY}",

                $"acceptedQuests={string.Join(",", acceptedQuests)}",
                $"completedQuests={string.Join(",", completedQuests)}",
                $"trackedQuest={trackedQuest}"
            };

            File.WriteAllLines(path, linesToSave);
        }
        // Restores player information AND quest progress
        static bool ReadGameSave(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            acceptedQuests.Clear();
            completedQuests.Clear();
            trackedQuest = "";
            activequest = false;

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] parts = line.Split('=', 2);

                if (parts.Length != 2)
                {
                    continue;
                }

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (key == "playerName") playerName = value;

                else if (key == "inventory")
                    inventory = string.IsNullOrEmpty(value)
                        ? new string[0] : value.Split(',');

                else if (key == "inventoryCount")
                    int.TryParse(value, out inventoryCount);

                else if (key == "inventoryMaxSize")
                    int.TryParse(value, out inventoryMaxSize);

                else if (key == "playerWpn") playerWpn = value;

                else if (key == "playerAmr") playerAmr = value;

                else if (key == "playerHealth")
                    int.TryParse(value, out playerHealth);

                else if (key == "playerMaxHealth")
                    int.TryParse(value, out playerMaxHealth);

                else if (key == "playerMana")
                    int.TryParse(value, out playerMana);

                else if (key == "playerMaxMana")
                    int.TryParse(value, out playerMaxMana);

                else if (key == "playerLevel")
                    int.TryParse(value, out playerLevel);

                else if (key == "playerCurrentXP")
                    int.TryParse(value, out playerCurrentXP);

                else if (key == "playerNextXP")
                    int.TryParse(value, out playerNextXP);

                else if (key == "playerGold")
                    int.TryParse(value, out playerGold);

                else if (key == "playerX")
                    int.TryParse(value, out playerX);

                else if (key == "playerY")
                    int.TryParse(value, out playerY);

                else if (key == "playerDamage")
                    int.TryParse(value, out playerDamage);

                else if (key == "playerStrength")
                    int.TryParse(value, out playerStrength);

                else if (key == "playerResilience")
                    int.TryParse(value, out playerResilience);

                else if (key == "playerFinesse")
                    int.TryParse(value, out playerFinesse);

                else if (key == "playerArcane")
                    int.TryParse(value, out playerArcane);

                else if (key == "playerMeleeDmg")
                    int.TryParse(value, out playerMeleeDmg);

                else if (key == "playerMeleeHit")
                    int.TryParse(value, out playerMeleeHit);

                else if (key == "playerRangeDmg")
                    int.TryParse(value, out playerRangeDmg);

                else if (key == "playerRangeHit")
                    int.TryParse(value, out playerRangeHit);

                else if (key == "playerRunBonus")
                    int.TryParse(value, out playerRunBonus);

                else if (key == "playerDodge")
                    int.TryParse(value, out playerDodge);

                else if (key == "playerCritChance")
                    int.TryParse(value, out playerCritChance);

                else if (key == "playerPhysicalResist")
                    int.TryParse(value, out playerPhysicalResist);

                else if (key == "playerBlockChance")
                    int.TryParse(value, out playerBlockChance);

                else if (key == "playerBlockReduction")
                    int.TryParse(value, out playerBlockReduction);

                else if (key == "playerMagicResist")
                    int.TryParse(value, out playerMagicResist);

                else if (key == "playerSpellDmg")
                    int.TryParse(value, out playerSpellDmg);

                else if (key == "playerSpellMiscast")
                    int.TryParse(value, out playerSpellMiscast);

                else if (key == "markerScreenX")
                    int.TryParse(value, out markerScreenX);

                else if (key == "markerScreenY")
                    int.TryParse(value, out markerScreenY);

                else if (key == "acceptedQuests" && value.Length > 0)
                    acceptedQuests.AddRange(value.Split(','));

                else if (key == "completedQuests" && value.Length > 0)
                    completedQuests.AddRange(value.Split(','));

                else if (key == "trackedQuest")
                    trackedQuest = value;
            }

            // Make sure the loaded inventory has enough slots
            if (inventoryMaxSize < 1)
                inventoryMaxSize = 1;

            if (inventory.Length < inventoryMaxSize)
                Array.Resize(ref inventory, inventoryMaxSize);

            if (inventoryCount < 0)
                inventoryCount = 0;

            if (inventoryCount > inventory.Length)
                inventoryCount = inventory.Length;

            // Restore whether a valid quest is being tracked
            activequest =
                trackedQuest.Length > 0 &&
                acceptedQuests.Contains(trackedQuest) &&
                questDefinitions.ContainsKey(trackedQuest);

            if (!activequest)
                trackedQuest = "";

            return true;
        }

        // General function for pausing
        static void PauseStory()
        {
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
            Console.Clear();
        }


        //center the text
        static void CenterText(string text)
        {
            int windowWidth = Console.WindowWidth;

            int padding = (windowWidth - text.Length) / 2;

            if (padding > 0) {
                Console.WriteLine(text.PadLeft(text.Length + padding));
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        // Intro
        static void Intro()
        {
            Console.Clear();
            CenterText("==============================");
            CenterText("|      MEDIEVAL FANTASY      |");
            CenterText("==============================");

            CenterText("You are a student from Otago Polytechnic, walking home after a long day of classes.");
            Console.ReadLine();

            CenterText("Without warning, a blinding light surrounds you.");
            Console.ReadLine();

            CenterText("When you open your eyes, you find yourself lying in an unfamiliar forest.");
            Console.ReadLine();

            CenterText("Towering trees surround you, and a cold wind moves through their branches.");
            Console.ReadLine();

            CenterText("A mysterious voice echoes inside your mind...");
            Console.ReadLine();

            CenterText("\"Hero, tell me your name.\"");
            CenterText("(Enter your name)");
            playerName = Console.ReadLine() ??"";

            //prevents the players from having a blank name
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Anon";
            }

            CenterText($"\"{playerName}... A fitting name for the one chosen to save our world.\"");
            Console.ReadLine();

            CenterText("You demand answers, but the mysterious voice ignores your questions.");
            Console.ReadLine();

            CenterText("\"Our kingdom is being ravaged by creatures of darkness.\"");
            Console.ReadLine();

            CenterText("\"Travel north and find the village of Greymoor. Your journey begins there.\"");
            Console.ReadLine();

            CenterText("Before you can ask anything else, the voice disappears.");
            Console.ReadLine();

            CenterText("You are alone. You have no idea where you are or how you arrived here.");
            Console.ReadLine();

            CenterText("Suddenly, you hear something moving in the nearby bushes...");
            Console.ReadLine();

            CenterText("You take a deep breath and prepare to move.");
            CenterText("Your journey begins now.");
            Console.ReadLine();


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
                        Console.WriteLine("\n------ Warrior ------");
                        Console.WriteLine("Prefers close quarters physical melee weapons, able to take a few hits and deal consistant damage, wearing the heaviest of equipment.\nJust maybe avoid anything magically inclined for your own safety");
                        playerStrength = 5;
                        playerFinesse = 1;
                        playerResilience = 4;
                        playerArcane = 0;
                        valid = true;
                        break;
                    case 'r':
                        Console.WriteLine("\n------ Rogue ------");
                        Console.WriteLine("A physical ranged combatant able to dodge most blows while dealing more frequent critical damage and more likely to escape a bad engagement.\nA little bit of arcane skill to bolster their natural skill but be wary of fights dragging on too long.");
                        playerStrength = 2;
                        playerFinesse = 5;
                        playerResilience = 2;
                        playerArcane = 1;
                        valid = true;
                        break;
                    case 'm':
                        Console.WriteLine("\n------ Mage ------");
                        Console.WriteLine("A master of the arcane arts, able to cast a much wider variety of spells with more frequency than other, with loss likely hood of self immolation to boot.\nHowever, your ability to use non-magical equipment is limited and are far more frail than most.");
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
                Console.WriteLine("\n---------- STAT ALLOCATION ----------\n");
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
            playerLevel = 1;
            playerCurrentXP = 0;
            playerNextXP = playerLevel * 50;
            inventoryMaxSize = inventoryMaxSize + playerStrength;
            inventory = new string[inventoryMaxSize];
            playerMaxHealth = playerMaxHealth + (10 * playerResilience) + (5 * playerLevel);
            playerHealth = playerHealth + (10 * playerResilience) + (5 * playerLevel);
            playerMaxMana = playerMaxMana + (3 * playerArcane);
            playerMana = playerMana + (3 * playerArcane);
            playerMeleeHit = playerMeleeHit + playerStrength;
            playerMeleeDmg = playerMeleeDmg + (playerStrength / 2);
            playerRangeHit = playerRangeHit + playerFinesse;
            playerRangeDmg = playerRangeDmg + (playerFinesse / 2);
            playerCritChance = playerCritChance + (playerFinesse * 4);
            playerSpellMiscast = playerSpellMiscast - (5 * playerArcane);
            playerSpellDmg = playerSpellDmg + (playerArcane / 2);
            playerDodge = playerDodge + (playerFinesse * 3);
            playerRunBonus = playerRunBonus + (2 * playerFinesse);
            playerBlockChance = playerBlockChance + (4 * playerResilience);
            playerBlockReduction = playerBlockReduction + (playerResilience / 2);
            playerPhysicalResist = playerPhysicalResist + (playerResilience / 3);
            playerMagicResist = playerMagicResist + (playerArcane / 3);
        }
        // Custom Character
        // Asks for one stat and returns the validated value
        static int AskStat(string name, string description, ref int pool)
        {
            Console.WriteLine($"----- {name} -----");
            Console.WriteLine(description);
            Console.WriteLine($"\nHow many points would you like to add to {name}?");
            Console.WriteLine($"You have {pool} points left to allocate");

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int value)
                    && value >= 0 && value <= 6 && value <= pool)
                {
                    pool -= value;
                    return value;
                }
                Console.WriteLine("Invalid value, please try again");
            }
        }

        // Custom Character
        static void CustomStats()
        {
            int pool = 10;

            Console.Clear();
            Console.WriteLine("------ Custom ------");
            Console.WriteLine("Build your own stat distribution to suit your playstyle! (max 6 points per stat)\n");

            playerStrength = AskStat("Strength",
                "Strength affects how much you can carry and proficiency with melee weapons", ref pool);
            Console.Clear();

            playerFinesse = AskStat("Finesse",
                "Finesse affects your proficiency with ranged weapons and ability to dodge and run away", ref pool);
            Console.Clear();

            playerResilience = AskStat("Resilience",
                "Resilience affects your total health and your ability to mitigate damage", ref pool);
            Console.Clear();

            playerArcane = AskStat("Arcane",
                "Arcane affects your total mana, ability to cast spells and their damage", ref pool);
            Console.Clear();

        }
        // Menu commands
        static bool Menu()
        {
            Console.Clear();
            Console.WriteLine("\n---------- EXPLORATION ----------");
            Console.WriteLine("[R] Start Navigating");
            Console.WriteLine("[I] Investigate");
            Console.WriteLine("[C] Character info");
            Console.WriteLine("[V] View inventory");
            Console.WriteLine("[E] Exit");
            Console.WriteLine("[M] Map");
            Console.WriteLine("[S] Save");
            Console.WriteLine("[Q] Quest");
            Console.WriteLine($"\nName: {playerName}");
            Console.WriteLine($"Level: {playerLevel}");
            Console.WriteLine($"XP: {playerCurrentXP}/{playerNextXP}");
            Console.WriteLine($"Gold: {playerGold}G");
            Console.WriteLine($"\nHealth: {playerHealth}/{playerMaxHealth}");
            Console.WriteLine($"Mana: {playerMana}/{playerMaxMana}");
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

                case 'e':
                    return false;

                case 'm':
                    Map();
                    break;
                case 'q':
                    Quest();
                    break;

                case 's':
                    Save();
                    break;
                case 'c':
                    ViewCharacter();
                    break;

                default:
                    Console.WriteLine("Unrecognised command.");
                    PauseStory();
                    break;
            }

            return playerHealth > 0;
        }
        //Viewing character stats
        static void ViewCharacter()
        {
            string user = "";
            bool valid = false;
            while (valid == false)
            {
                Console.Clear();
                Console.WriteLine($"----- {playerName} -----");
                Console.WriteLine($"[S] - Strength: {playerStrength}");
                Console.WriteLine($"[F] - Finesse: {playerFinesse}");
                Console.WriteLine($"[R] - Resilience: {playerResilience}");
                Console.WriteLine($"[A] - Arcane: {playerArcane}");
                Console.WriteLine("\nFor more detailed information on a stat, enter the assosiated character.\nOtherwise enter X to exit.");

                do
                {
                    user = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (user.Length == 0)
                    {
                        Console.WriteLine("\nCommand not entered, please enter a command");
                    }
                } while (user.Length == 0);

                char input = user[0];
                Console.Clear();
                switch (input)
                {
                    case 's':
                        Console.WriteLine($"Strength: {playerStrength}");
                        Console.WriteLine($"\nMelee Damage bonus: {playerMeleeDmg}");
                        Console.WriteLine("Ads a bonus to damage on melee attacks");
                        Console.WriteLine($"\nMelee hit chance: {playerMeleeHit}");
                        Console.WriteLine("Ads a bonus to hit chance against enemies with melee weapons");
                        Console.WriteLine($"\nInventory bonus: {playerStrength}");
                        Console.WriteLine("An added bonus to default inventory capacity of 5");
                        Console.WriteLine("\nPress ENTER to return");
                        Console.ReadLine();
                        break;
                    case 'f':
                        Console.WriteLine($"Finesse: {playerFinesse}");
                        Console.WriteLine($"\nCrit Chance: {playerCritChance}");
                        Console.WriteLine("Bonus added to base to critical hit chance");
                        Console.WriteLine($"\nDodge chance: {playerDodge}");
                        Console.WriteLine("A percent based chance to dodge all damage of a physical attack");
                        Console.WriteLine($"\nRun away bonus: {playerRunBonus}");
                        Console.WriteLine("Adds a flat bonus to your chances of running from combat");
                        Console.WriteLine($"\nRange Damage bonus: {playerRangeDmg}");
                        Console.WriteLine("Adds a damage bonus to your ranged weapons");
                        Console.WriteLine($"\nRange hit chance bonus: {playerRangeHit}");
                        Console.WriteLine("Adds a bonus to hit chance against enemies with ranged weapons");
                        Console.WriteLine("\nPress ENTER to return");
                        Console.ReadLine();
                        break;
                    case 'r':
                        Console.WriteLine($"Resilience: {playerResilience}");
                        Console.WriteLine($"\nBlock chance: {playerBlockChance}");
                        Console.WriteLine("Chance to block an attack to reduce incoming physical damage");
                        Console.WriteLine($"\nBlock damage reduction: {playerBlockReduction}");
                        Console.WriteLine("Amount of damage blocked when successfully blocking attacks");
                        Console.WriteLine($"\nPhysical resistance: {playerPhysicalResist}");
                        Console.WriteLine("Amount of damage reduced when taking any physical damage");
                        Console.WriteLine("\nPress ENTER to return");
                        Console.ReadLine();
                        break;
                    case 'a':
                        Console.WriteLine($"Arcane: {playerArcane}");
                        Console.WriteLine($"\nMiscast chance: {playerSpellMiscast}");
                        Console.WriteLine("Reduces the chance that a spell miscasts, dealing self damage");
                        Console.WriteLine($"\nSpell Damage Bonus: {playerSpellDmg}");
                        Console.WriteLine("Flat added bonus to all spell damage");
                        Console.WriteLine($"\nMagic Resistance: {playerMagicResist}");
                        Console.WriteLine("Flat reduction in damage taken from any magic damage");
                        Console.WriteLine("\nPress ENTER to return");
                        Console.ReadLine();
                        break;
                    case 'x':
                        Console.WriteLine("Exiting character stats");
                        valid = true;
                        break;
                    default:
                        Console.WriteLine("Invalid command, please input a valid command");
                        break;
                    }
            }
        }
        // Gaining XP
        static int GainXP(int XP)
        {
            Console.WriteLine($"\nYou have gained {XP} XP!");
            playerCurrentXP = playerCurrentXP + XP;
            if (playerCurrentXP >= playerNextXP)
            {
                Console.WriteLine("You have leveled up! Press enter to allocate your stats!");
                Console.ReadLine();
                playerLevel = LevelUp();
                return playerLevel;
            }
            else
            {
                return playerLevel;
            }
        }
        // Level Up
        static int LevelUp()
        {
            bool valid = false, test;
            while (valid == false)
            {
                test = false;
                int pool = 5, tempStrength, tempFinesse, tempResilience, tempArcane;
                Console.Clear();
                Console.WriteLine("---- Level Up! ----\n\n");
                tempStrength = playerStrength + AskStat("Strength",
                    "Strength affects how much you can carry and proficiency with melee weapons", ref pool);
                Console.Clear();

                tempFinesse = playerFinesse + AskStat("Finesse",
                    "Finesse affects your proficiency with ranged weapons and ability to dodge and run away", ref pool);
                Console.Clear();

                tempResilience = playerResilience + AskStat("Resilience",
                    "Resilience affects your total health and your ability to mitigate damage", ref pool);
                Console.Clear();

                tempArcane = playerArcane + AskStat("Arcane",
                    "Arcane affects your total mana, ability to cast spells and their damage", ref pool);
                Console.Clear();
                Console.WriteLine("\n---------- STAT ALLOCATION ----------\n");
                Console.WriteLine($"Strength   : {tempStrength}\n");
                Console.WriteLine($"Finesse    : {tempFinesse}\n");
                Console.WriteLine($"Resilience : {tempResilience}\n");
                Console.WriteLine($"Arcane     : {tempArcane}\n");
                Console.Write("Are you happy with these improvements? (Y/N): ");
                while (test == false)
                {
                    string check = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (check.Length == 0)
                    {
                        Console.WriteLine("No input detected, please input value");
                        continue;
                    }
                    char temp = check[0];
                    switch (temp)
                    {
                        case 'y':
                            playerLevel++;
                            playerCurrentXP = playerCurrentXP - playerNextXP;
                            playerNextXP = playerLevel * 50;
                            playerStrength = tempStrength;
                            playerFinesse = tempFinesse;
                            playerResilience = tempResilience;
                            playerArcane = tempArcane;
                            inventoryMaxSize = inventoryMaxSize + playerStrength;
                            inventory = new string[inventoryMaxSize];
                            playerMaxHealth = 100 + (10 * playerResilience) + (5 * playerLevel);
                            playerHealth = playerHealth + (10 * playerResilience) + (5 * playerLevel);
                            if (playerHealth > playerMaxHealth)
                            {
                                playerHealth = playerMaxHealth;
                            }
                            playerMaxMana = 5 + (3 * playerArcane);
                            playerMana = playerMana + (3 * playerArcane);
                            if (playerMana > playerMaxMana)
                            {
                                playerMana = playerMaxMana;
                            }
                            playerMeleeHit = 0 + playerStrength;
                            playerMeleeDmg = 0 + (playerStrength / 2);
                            playerRangeHit = 0 + playerFinesse;
                            playerRangeDmg = 0 + (playerFinesse / 2);
                            playerCritChance = 5 + (playerFinesse * 4);
                            playerSpellMiscast = 0 - (5 * playerArcane);
                            playerSpellDmg = 0 + (playerArcane / 2);
                            playerDodge = 0 + (playerFinesse * 3);
                            playerRunBonus = 0 + (2 * playerFinesse);
                            playerBlockChance = 0 + (4 * playerResilience);
                            playerBlockReduction = 0 + (playerResilience / 2);
                            playerPhysicalResist = 0 + (playerResilience / 3);
                            playerMagicResist = 0 + (playerArcane / 3);
                            test = true;
                            valid = true;
                            break;
                        case 'n':
                            test = true;
                            break;
                        default:
                            Console.WriteLine("Invalid Input, Please input either y or n");
                            break;
                    }
                }
            }
            return playerLevel;
        }


        //Combat commands
        static void AddCombatLog(string message)
        {
            combatLog.Add(message);

            if(combatLog.Count > 21)
            {
                combatLog.RemoveAt(0);
            }
        }

        static void DisplayCombatLog()
        {
            int originalColumn = Console.CursorLeft;
            int originalRow = Console.CursorTop;
            int startColumn = 55, currentRow = 0;

            Console.SetCursorPosition(startColumn, currentRow);
            Console.WriteLine("---------- COMBAT LOG ----------");
            currentRow++;
            foreach (string s in combatLog)
            {
                Console.SetCursorPosition(startColumn, currentRow);
                Console.WriteLine(s);
                currentRow++;
            }
            Console.SetCursorPosition(originalColumn, originalRow);
        }



        //Make combat commands into separate methods to save lines of code
        static void EnemyEncounter(string enemyName)
        {
            Console.WriteLine("\n========== ENCOUNTER ==========");
            Console.WriteLine($"A {enemyName} approaches!");
            Console.WriteLine("\nPress Enter to begin combat.");
        }

        static void CombatInformation(string enemyName, int enemyHealth)
        {
            Console.WriteLine("---------- COMBAT ----------");
            Console.WriteLine($"{playerName}:");
            Console.WriteLine($"{playerHealth}/{playerMaxHealth} HP");
            Console.WriteLine($"{playerMana}/{playerMaxMana} MP");
            Console.WriteLine($"\n{enemyName}: \n{enemyHealth}/50 HP");
        }


        //general combat input
        static void CombatCommands()
        {
            Console.WriteLine("\n[A] Attack");
            Console.WriteLine("[M] Magic");
            Console.WriteLine("[I] Use item");
            Console.WriteLine("[R] Run away");
        }

        //attack
        static bool AttackCommand(ref int playerDamage, ref string enemyName, ref int enemyHealth, ref bool playerTurnCompleted)
        {
            //variables for hit chance
            int baseHitChance = 75, finalHitChance;

            //variables for crit chance
            int baseCritChance = 5, finalCritChance;

            //variable for actual damage
            int finalAttackDamage;
            Console.WriteLine("You chose attack!");
            Console.ReadLine();
            finalHitChance = baseHitChance + playerMeleeHit;
            if(finalHitChance > 100)
            {
                finalHitChance = 100;
            }

            int hitRoll = rand.Next(1, 101);
            
            finalCritChance = baseCritChance + playerCritChance;
            if(hitRoll <= finalHitChance)
            {
                if(finalCritChance > 100)
                {
                    finalCritChance = 100;
                }
                int critRoll = rand.Next(1, 101);

                if (critRoll <= finalCritChance)
                {
                    enemyHealth -= (playerDamage + playerMeleeDmg) * 2;
                    finalAttackDamage = (playerDamage + playerMeleeDmg) * 2;
                    Console.WriteLine($"\nYou attack the {enemyName} for {finalAttackDamage} CRITICAL HIT.");
                    Console.ReadLine();
                    AddCombatLog($"You attack the {enemyName} for {finalAttackDamage} CRITICAL HIT.");

                }

                else
                {
                    enemyHealth -= playerDamage + playerMeleeDmg;
                    finalAttackDamage = playerDamage + playerMeleeDmg;
                    Console.WriteLine($"\nYou attack the {enemyName} for {finalAttackDamage} damage.");
                    Console.ReadLine();
                    AddCombatLog($"You attack the {enemyName} for {finalAttackDamage} damage.");

                }

                //add mana after attacking
                if (playerMana < playerMaxMana)
                {
                    playerMana += playerDamage / 10;
                }
                else
                {
                    playerMana = playerMaxMana;
                }
            }

            else
            {
                Console.WriteLine("Your attack misses!");
                AddCombatLog("Your attack misses!");
            }
            

            
            return true;
        }


        //magic
        static void DisplayAvailableMagic(bool staffFireball)
        {
            Console.WriteLine("\nYou chose magic!");
            Console.WriteLine();
            Console.WriteLine("[M] - Magic Missile - 3 MP");
            // Temporary bool to test unlocking magic 
            if (staffFireball == true)
            {
                Console.WriteLine("[F] - Fireball - 6 MP");
            }
            Console.WriteLine("[0] - Return to previous menu");
        }

        static string MagicInput()
        {
            string magicInput;
            Console.Write("\nChoose a spell to cast: ");
            do
            {

                magicInput = Console.ReadLine()?.Trim().ToLower() ?? "";
                if (magicInput != "m" && magicInput != "0" && magicInput != "f")
                {
                    Console.WriteLine("Invalid selection, please try again");
                }
            } while (magicInput != "m" && magicInput != "0" && magicInput != "f");
            return magicInput;
        }


        static bool MagicSubMenu(char spellSelect, string enemyName, ref int enemyHealth, bool staffFireball)
        {
            switch (spellSelect)
            {
                case 'm':
                    if (playerMana < 3)
                    {
                        Console.WriteLine("You do not have enough Mana to cast Magic Missile.");
                        return false;
                    }

                    playerMana -= 3;

                    int casting = rand.Next(1, 101) + playerSpellMiscast;

                    if (casting <= 75)
                    {
                        playerDamage = rand.Next(8, 25) + playerSpellDmg;
                        enemyHealth -= playerDamage;

                        string message =
                            $"You cast Magic Missile, blasting {enemyName} for {playerDamage} damage.";

                        Console.WriteLine(message);
                        AddCombatLog(message);
                    }
                    else
                    {
                        playerDamage = rand.Next(1, 11);
                        playerHealth -= playerDamage;

                        string message =
                            $"The spell miscasts! You take {playerDamage} damage.";

                        Console.WriteLine(message);
                        AddCombatLog(message);

                        if (playerHealth <= 0)
                        {
                            playerHealth = 0;
                        }
                    }

                    Console.ReadLine();
                    return true;
                case 'f':
                    
                    while (staffFireball == true)
                    {
                        if (playerMana < 6)
                        {
                            Console.WriteLine("You do not have enough Mana to cast Fireball.");
                            return false;
                        }

                        playerMana -= 6;

                        casting = rand.Next(1, 101) + playerSpellMiscast;
                        
                        if (casting <= 55)
                        {
                            playerDamage = rand.Next(15, 35) + (playerSpellDmg * 2);
                            enemyHealth -= playerDamage;

                            string message =
                                $"You cast Fireball, scorching {enemyName} for {playerDamage} damage.";

                            Console.WriteLine(message);
                            AddCombatLog(message);
                        }
                        else
                        {
                            playerDamage = rand.Next(2, 15);
                            playerHealth -= playerDamage;

                            string message =
                                $"The spell miscasts! You take {playerDamage} damage.";

                            Console.WriteLine(message);
                            AddCombatLog(message);

                            if (playerHealth <= 0)
                            {
                                playerHealth = 0;
                            }
                        }

                        Console.ReadLine();
                        return true;
                    }
                    Console.WriteLine("Invalid spell selection.");
                    return false;
                case '0':
                        Console.WriteLine("Returning to the combat menu.");
                        return false;

                default:
                        Console.WriteLine("Invalid spell selection.");
                        return false;
                    }
        }


        //RUN FROM COMBAT COMMAND
        static bool RunCommand(string enemyName)
        {
            int baseEscapeChance = 50;
            int finalEscapeChance = Math.Clamp(baseEscapeChance + playerRunBonus, 0, 100);
            int escapeRoll = rand.Next(1, 101);
            Console.WriteLine("You tried to run away!");
            if (escapeRoll <= finalEscapeChance)
            {
                Console.WriteLine("You have escaped!");
                AddCombatLog($"You have successfully escaped from {enemyName}!");
                AddCombatLog("\n");
                return true;
                
            }
            else
            {
                Console.WriteLine("You failed to escape!");
                AddCombatLog($"You failed to escape!");
                AddCombatLog("\n");
                return false;

            }
        }

        //check for enemy hp
        static bool CheckEnemyHealth(ref int enemyHealth, string enemyName)
        {
            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                Console.WriteLine($"You have defeated the {enemyName}!");

                int temp = rand.Next(2, 6);
                playerGold += temp;
                Console.WriteLine($"\nYou gained {temp} gold!");

                playerLevel = GainXP(50);

                Console.WriteLine("\nPress enter to continue exploration.");
                Console.ReadLine();
                AddCombatLog("You have won!");
                return true;
            }
            else
            {
                return false;
            }
        }

        //check for player hp
        static bool CheckPlayerHealth(ref int playerHealth)
        {
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                AddCombatLog($"{playerName} has been defeated.");

                if (activequest && trackedQuest.Length > 0)
                {
                    if (FailQuest(trackedQuest))
                    {
                        Console.WriteLine("Quest failed!");
                        Console.WriteLine("Returning to the last quest checkpoint.");
                        Console.WriteLine("Press Enter to continue.");
                        Console.ReadLine();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //Defensive Stats
        //dodge
        static bool CombatDodge()
        {
            int playerDodgeChance = Math.Min(playerDodge, 100);
            int dodgeRoll = rand.Next(1,101);
            if(dodgeRoll <= playerDodgeChance)
            { 
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool CombatBlock()
        {
            int playerBlock = Math.Min(playerBlockChance, 100);
            int blockRoll = rand.Next(1, 101);
            if(blockRoll <= playerBlock)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //enemy attack
        static void EnemyTurn(string enemyName)
        {
            bool dodgeAttack = CombatDodge();
            bool blockAttack;

            //variable if attack was blocked
            int reducedDamage;

            //physical resist
            int finalDamage;
            if(dodgeAttack == true)
            {
                Console.WriteLine("You have dodged!");
                AddCombatLog("You have dodged!");
                AddCombatLog("\n");
            }
            else
            {
                blockAttack = CombatBlock();
                enemyDamage = rand.Next(5, 11);
                if (blockAttack == true)
                {
                    reducedDamage = enemyDamage - playerBlockReduction;
                    if(reducedDamage < 0)
                    {
                        enemyDamage = 0;
                    }
                    else
                    {
                        enemyDamage = reducedDamage;
                    }
                    Console.WriteLine($"The {enemyName} attacks but you have blocked it!");
                    AddCombatLog($"The {enemyName} attacks but you have blocked it!");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"The {enemyName} attacks!");
                    Console.ReadLine();
                    AddCombatLog($"The {enemyName} attacks");
                }
                finalDamage = int.Max(0, enemyDamage - playerPhysicalResist);
                playerHealth -= finalDamage;
                Console.WriteLine($"You received {finalDamage} damage!");
                AddCombatLog($"You received {finalDamage} damage!");
                AddCombatLog("\n");
            }
            
        }
        static bool CombatMenu(string enemyName)
        {
            combatLog.Clear();
            int enemyHealth = 50;
            bool inCombat = true;
            bool playerTurnCompleted = false, playerEscape = false, runAway, enemyHPCheck, playerHPCheck;
            Console.Clear();
            EnemyEncounter(enemyName);
            Console.ReadLine();

            while (inCombat && playerHealth > 0 && enemyHealth > 0)
            {
                playerTurnCompleted = false;

                //magic local variables
                int valid = 0;
                string userselect;

                Console.Clear();
                DisplayCombatLog();
                CombatInformation(enemyName, enemyHealth);
                CombatCommands();

                Console.Write("\nChoose an action: ");
                string combatInput = Console.ReadLine()?.Trim().ToLower() ?? "";


                if (combatInput.Equals(""))
                {
                    Console.WriteLine("You did not enter any command! Try again!");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    char combatChoice = combatInput[0];
                    switch (combatChoice)
                    {
                        case 'a':
                            playerDamage = rand.Next(8, 16);
                            playerTurnCompleted = AttackCommand(ref playerDamage, ref enemyName, ref enemyHealth, ref playerTurnCompleted);
                            break;

                        case 'm':
                            // Temporary bool to test magic unlocking
                            bool staffFireball = true;
                            DisplayAvailableMagic(staffFireball);
                            userselect = MagicInput();
                            char spellselect = userselect[0];
                            playerTurnCompleted = MagicSubMenu(spellselect, enemyName, ref enemyHealth, staffFireball);
                            break;


                        case 'i':
                            ViewInventory();
                            continue;

                        case 'r':
                            runAway = RunCommand(enemyName);
                            if(runAway == true)
                            {
                                inCombat = false;
                                playerTurnCompleted = false;
                                playerEscape = true;
                            }
                            else
                            {
                                playerTurnCompleted = true;
                                playerEscape = false;
                            }
                            break;

                        default:
                            Console.WriteLine("Unrecognised combat command.");
                            continue;
                    }


                }

                //check if enemy hp is 0 or less
                enemyHPCheck = CheckEnemyHealth(ref enemyHealth, enemyName);
                if(enemyHPCheck == true)
                {
                    inCombat = false;
                    playerTurnCompleted = false;
                }

                //check for player hp
                playerHPCheck = CheckPlayerHealth(ref playerHealth);
                if(playerHPCheck == true)
                {
                    playerTurnCompleted = false;
                    inCombat = false;
                }


                //enemy attacks after a player turn and enemy hp > 0
                if (playerTurnCompleted == true && enemyHPCheck == false)
                {
                    if (inCombat == true && playerTurnCompleted == true && enemyHealth > 0)
                    {
                        EnemyTurn(enemyName);
                    }

                    //check for player hp
                    playerHPCheck = CheckPlayerHealth(ref playerHealth);
                    if (playerHPCheck == true)
                    {
                        playerTurnCompleted = false;
                        inCombat = false;
                    }
                }
                


                //command after running away
                if (!inCombat && playerEscape)
                {
                    Console.WriteLine("\nPress Enter to return to exploration.");
                    Console.ReadLine();
                    continue;
                }


                //Combat pause & turn ended
                if (inCombat && playerHealth > 0 && enemyHealth > 0)
                {
                    Console.WriteLine("\nPress Enter for the next turn.");
                    Console.ReadLine();
                }

            }

            return true;
        }

        //END OF COMBAT COMMANDS

        //Enemy generator
        static string Enemy(string[] enemyArray)
        {
            randomEnemyEncounter = rand.Next(0, enemyArray.Length);
            return enemyArray[randomEnemyEncounter];
        }


        // Run command
        static void Run()
        {
            //Variables
            bool isNavigating = true;
            while (isNavigating && playerHealth > 0)
            {
                Console.Clear();
                Map(true);
                Console.WriteLine("\n---------- NAVIGATION ----------");
                Console.WriteLine("[W] Move north");
                Console.WriteLine("[A] Move west");
                Console.WriteLine("[S] Move south");
                Console.WriteLine("[D] Move east");
                Console.WriteLine("[M] Map");
                Console.WriteLine("[Q] Stop navigating");


                Console.Write("\nChoose a direction: ");
                char playerChoice = ReadBlinkingDirection();

                Console.WriteLine();
                string direction = "";
                int nextX = playerX;
                int nextY = playerY;
                bool validMovement = false;
                switch (playerChoice)
                {
                    case 'w':
                        direction = "north";
                        validMovement = true;
                        nextY++;
                        break;

                    case 'a':
                        direction = "west";
                        validMovement = true;
                        nextX--;
                        break;

                    case 's':
                        direction = "south";
                        validMovement = true;
                        nextY--;
                        break;

                    case 'd':
                        direction = "east";
                        validMovement = true;
                        nextX++;
                        break;

                    case 'm':
                        Map();
                        validMovement = false;
                        continue;

                    case 'q':
                        Console.WriteLine("You stop navigating.");
                        isNavigating = false;
                        PauseStory();
                        return;

                    default:
                        Console.WriteLine("Enter a valid navigation command.");
                        PauseStory();
                        continue;
                }

                // Border map check
                if (nextX < 0 || nextX >= 5 || nextY < 0 || nextY >= 5)
                {
                    Console.WriteLine("You cannot move beyond the map!");
                    PauseStory();
                    return;
                }
                playerX = nextX;
                playerY = nextY;
                Map(true);
                Console.WriteLine($"{playerName} moves {direction}.");

                if (validMovement == true)
                {
                    Console.WriteLine($"{playerName} moves {direction}.");
                    CheckForEncounter();
                }
            }
        }

        //Check for enemy encounter
        static void CheckForEncounter()
        {
            if ((playerX == 2 && playerY == 3) || (playerX == 0 && playerY == 4))
            {
                return;
            }
            int encounterChance = rand.Next(1, 101);

            if (encounterChance <= 30)
            {
                string encounteredEnemy = Enemy(enemies);

                Console.WriteLine($"\nA {encounteredEnemy} suddenly appears!");
                AddCombatLog($"\nA {encounteredEnemy} suddenly appears!");
                //Combat system function
                CombatMenu(encounteredEnemy);
            }
            else
            {
                Console.WriteLine("The path is clear.");
            }
        }
        //NPC
        //Merchant

        static void Merchant()
        {
            Console.Clear();
            Console.WriteLine("========== MERCHANT ==========");

            // Both quests finished
            if (completedQuests.Contains("side_merchant_02"))
            {
                Console.WriteLine("Merchant: Thank you for all your help!");
                return;
            }

            // Second quest in progress
            if (acceptedQuests.Contains("side_merchant_02"))
            {
                Console.WriteLine("Merchant: Please deliver my package to the Witch's Hut.");
                return;
            }

            // First quest completed: offer second quest
            if (completedQuests.Contains("side_merchant_01"))
            {
                Console.WriteLine("Merchant: Thank you for finding my sword!");
                Console.WriteLine("Merchant: I have another job for you.");
                Console.WriteLine("[1] Accept delivery quest");
                Console.WriteLine("[2] Decline");

                char choice = Console.ReadKey().KeyChar;

                if (choice == '1')
                {
                    bool given = GiveQuest("side_merchant_02");

                    if (given)
                    {
                        Console.WriteLine("New quest: The Merchant's Delivery!");
                    }
                }

                return;
            }

            // First quest in progress
            if (acceptedQuests.Contains("side_merchant_01"))
            {
                Console.WriteLine("Merchant: Have you found my sword?");
                Console.WriteLine("[1] Give Rusty Sword");
                Console.WriteLine("[2] Leave");

                char choice = Console.ReadKey().KeyChar;

                if (choice == '1')
                {
                    if (Array.Exists(inventory, item => item == "Rusty Sword"))
                    {
                        CompleteQuest("side_merchant_01");
                        playerLevel = GainXP(20);
                        Console.WriteLine("Quest completed: The Missing Sword!");
                        Console.WriteLine("Merchant: Come back. I have another job!");
                    }
                    else
                    {
                        Console.WriteLine("You don't have a Rusty Sword.");
                    }
                }

                return;
            }

            // Neither quest accepted: offer first quest
            Console.WriteLine("Merchant: I lost my sword. Can you help?");
            Console.WriteLine("[1] Accept quest");
            Console.WriteLine("[2] Decline");

            char answer = Console.ReadKey().KeyChar;

            if (answer == '1')
            {
                bool given = GiveQuest("side_merchant_01");

                if (given)
                {
                    Console.WriteLine("New quest accepted: The Missing Sword!");
                }
            }
        }

        // Market menu
        static void Market()
        {
            int healthPotionPrice = 6;

            Console.Clear();
            Console.WriteLine("\n========== MARKETS ==========");
            Console.WriteLine($"1. Health Potion: {healthPotionPrice}G");

            Console.WriteLine("\n[NUM] Purchase Item");
            Console.WriteLine("[ANY OTHER INPUT] Leave");

            Console.WriteLine($"\nGold: {playerGold}G");

            if (inventoryCount == inventoryMaxSize)
            {
                Console.WriteLine("\nYour inventory is full! You cannot buy anything.");
                PauseStory();
            }
            else
            {

                char marketChoice = Console.ReadKey(true).KeyChar;
            
                switch (marketChoice)
                {
                    case '1':
                        if (playerGold >= healthPotionPrice)
                        {
                            playerGold -= healthPotionPrice;
                            Console.WriteLine($"\nYou bought the Health Potion! You now have {playerGold} gold.");
                            AddItem("Health Potion");
                        }
                        else
                        {
                            Console.WriteLine("\nYou do not have enough gold to buy this.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        // Investigation command
        static void Investigate()
        {
            switch ((playerX, playerY))
            {
                // MARKET (2,3)
                case (2, 3):
                    {
                        Console.Clear();
                        Console.WriteLine("\n========== MARKET ==========");
                        Console.WriteLine("[1] Talk to Merchant");
                        Console.WriteLine("[2] Search for loot");
                        Console.WriteLine("[3] Check the markets");
                        Console.WriteLine("[0] Leave");

                        char choice = Console.ReadKey(true).KeyChar;

                        switch (choice)
                        {
                            case '1':
                                Merchant();
                                break;

                            case '2':
                                Console.WriteLine("You search around the market...");

                                int lootChance = rand.Next(1, 4);

                                switch (lootChance)
                                {
                                    case 1:
                                        Console.WriteLine("You found a Healing Potion!");
                                        AddItem("Healing Potion");
                                        break;

                                    case 2:
                                        Console.WriteLine("You found a Rusty Sword!");
                                        AddItem("Rusty Sword");
                                        break;

                                    case 3:
                                        Console.WriteLine("You found nothing.");
                                        break;
                                }
                                break;

                            case '3':
                                Market();
                                break;

                            case '0':
                                Console.WriteLine("You leave the market.");
                                break;

                            default:
                                Console.WriteLine("Invalid option.");
                                break;
                        }
                        break;
                    }

                // DARK FOREST (2,2)
                case (2, 2):
                    Console.WriteLine("You search the Dark Forest...");

                    int result = rand.Next(1, 4);

                    switch (result)
                    {
                        case 1:
                            Console.WriteLine("You found a Healing Potion!");
                            AddItem("Healing Potion");
                            break;

                        case 2:
                            Console.WriteLine("You found a Rusty Sword!");
                            AddItem("Rusty Sword");
                            break;

                        case 3:
                            Console.WriteLine("You found nothing.");
                            break;
                    }
                    break;


                // WITCH'S HUT (0,4)
                case (0, 4):
                    {
                        Console.WriteLine("You approach the Witch's Hut.");

                        if (acceptedQuests.Contains("side_merchant_02"))
                        {
                            Console.WriteLine("The witch accepts the merchant's delivery.");

                            if (CompleteQuest("side_merchant_02"))
                            {
                                Console.WriteLine("Quest completed: The Merchant's Delivery!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("An old witch watches you from the doorway.");
                        }
                        break;
                        //Later add witch dialogue
                    }

                    


                // DARK CAVE (1,4)
                case (1, 4):
                    Console.WriteLine("You investigate the dark cave.");
                    Console.WriteLine("Strange noises echo through the tunnels.");
                    break;


                // ROYAL ARMY (0,3)
                case (0, 3):
                    Console.WriteLine("You approach the Royal Army's camp.");
                    Console.WriteLine("Soldiers patrol the area.");

                    // Later: TalkToCommander();
                    break;


                // ALL OTHER LOCATIONS
                default:
                    Console.WriteLine("There is nothing special to investigate here yet.");
                    break;
            }
        }

        // Adding items to the inventory
        static void AddItem(string item)
        {
            if (inventoryCount < inventoryMaxSize)
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
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < inventoryCount; i++)
            {
                Console.WriteLine($"{i + 1}. {inventory[i]}");
            }

            Console.WriteLine($"Inventory slots: {inventoryCount}/{inventoryMaxSize}");

            Console.WriteLine($"\nEquipped weapon: {playerWpn}");
            Console.WriteLine($"Equipped armor: {playerAmr}");

            Console.WriteLine("\nWhat do you want to do?");
            Console.WriteLine("\n[U] Use an item");
            Console.WriteLine("[T] Throw out an item");
            Console.WriteLine("[ANY OTHER INPUT] Return");

            string inputChoice = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(inputChoice))
            {
                return;
            }

            char invChoice = inputChoice[0];

            switch (invChoice)
            {
                case 'u':
                    UseItems();
                    break;

                case 't':
                    ThrowItems();
                    break;

                default:
                    break;
            }

            // Using items
            static void UseItems()
            {
                bool slotEmpty = false;
                Console.Clear();
                Console.WriteLine("\n---------- INVENTORY ----------");

                for (int i = 0; i < inventoryCount; i++)
                {
                    Console.WriteLine($"{i + 1}. {inventory[i]}");
                }

                Console.WriteLine($"Inventory slots: {inventoryCount}/{inventoryMaxSize}");

                Console.WriteLine("\nWhich slot do you wish to use?");

                if (!int.TryParse(Console.ReadLine(), out int useChoice) || useChoice < 1 || useChoice > inventoryCount)
                {
                    Console.WriteLine("\nInvalid input.");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                    return;
                }

                Console.Clear();
                Console.WriteLine("\n---------- INVENTORY ----------");

                // What happens when using a healing potion
                if (inventory[useChoice - 1] == "Healing Potion")
                {
                    playerHealth = playerHealth + 30;
                    slotEmpty = true;
                    Console.WriteLine("The healing potion was used! +30 HP");
                    if (playerHealth > playerMaxHealth)
                    {
                        playerHealth = playerMaxHealth;
                    }
                    Console.WriteLine($"Your HP is now {playerHealth}/{playerMaxHealth}.");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                // What happens when using a rusty sword
                else if (inventory[useChoice - 1] == "Rusty Sword")
                {
                    // Checks if equipped wpn is empty, clears old inv slot if true, replaces if not
                    if (playerWpn != "")
                    {
                        inventory[useChoice - 1] = playerWpn;
                    }
                    else
                    {
                        slotEmpty = true;
                    }
                    playerWpn = "Rusty Sword";
                    Console.WriteLine("You equipped the Rusty Sword.");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("That input is unrecognised.");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }

                if (slotEmpty == true)
                {
                    string usedItem = inventory[useChoice - 1];
                    for (int i = useChoice - 1; i < inventoryCount - 1; i++)
                    {
                        inventory[i] = inventory[i + 1];
                    }
                    inventory[inventoryCount - 1] = null;
                    inventoryCount--;
                }
            }

            // Throwing away items
            static void ThrowItems()
            {
                Console.Clear();
                Console.WriteLine("\n---------- THROW ITEM ----------");
                for (int i = 0; i < inventoryCount; i++)
                {
                    Console.WriteLine($"{i + 1}. {inventory[i]}");
                }
                Console.Write($"\nWhich slot to throw? (1 to {inventoryCount}): ");
                if (!int.TryParse(Console.ReadLine(), out int throwChoice) || throwChoice < 1 || throwChoice > inventoryCount)
                {
                    Console.WriteLine("\nInvalid input.");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                    return;
                }
                string removedItem = inventory[throwChoice - 1];
                for (int i = throwChoice - 1; i < inventoryCount - 1; i++)
                {
                    inventory[i] = inventory[i + 1];
                }
                inventory[inventoryCount - 1] = null;
                inventoryCount--;
                Console.WriteLine($"\nYou threw away the {removedItem}.");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
            }
        }
        // Map
        public static void Map(bool navigating = false)
        {
            Console.Clear();
            if (!navigating)
            {
                Console.WriteLine(@"
                                                  #####    #####     ###    #######   #####   ######   #######  ######    #####
                                                 ##   ##  ##   ##   ## ##      ##    ##   ##  ##   ##  ##       ##   ##  ##   ##
                                                 ##   ##  ##       ##   ##     ##    ##   ##  ##   ##  ##       ##   ##  ##
                                                 ##   ##  ##  ###  #######     ##    ##   ##  ######   ######   ######   ##  ###
                                                 ##   ##  ##   ##  ##   ##     ##    ##   ##  ##   ##  ##       ## ##    ##   ##
                                                 ##   ##  ##   ##  ##   ##     ##    ##   ##  ##   ##  ##       ##  ##   ##   ##
                                                  #####    #####   ##   ##     ##     #####   ######   #######  ##   ##   #####
                                            ");
            }

            string mapArt = @"
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
                                |        /   \         |        ||  ||        |       ||   ||        |       / \ / \        |     |__|   |__|      |
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
                                +----------------------+----------------------+----------------------+----------------------+----------------------+";

            string[] mapLines = mapArt.Split('\n');

            int markerLine = 2 + (4 - playerY) * 6;
            int leftBorder = mapLines[markerLine].IndexOf('|');
            int markerColumn = leftBorder + 2 + playerX * 23;

            char[] characters = mapLines[markerLine].ToCharArray();

            string markerText = "-> " + playerName;

            // Keep the marker inside its map cell
            int maxLength = 20;

            if (activequest && questDefinitions.TryGetValue(trackedQuest, out QuestDefinition target) &&playerX == target.MapX &&playerY == target.MapY)
            {
                maxLength = 16;
            }

            if (markerText.Length > maxLength)
            {
                markerText = markerText.Substring(0, maxLength);
            }

            for (int i = 0; i < markerText.Length; i++)
            {
                characters[markerColumn + i] = markerText[i];
            }

            mapLines[markerLine] = new string(characters);
            if (activequest && questDefinitions.TryGetValue(trackedQuest, out QuestDefinition destination))
            {
                int questLine = 2 + (4 - destination.MapY) * 6;

                int questBorder = mapLines[questLine].IndexOf('|');

                int questColumn = questBorder + 2 + destination.MapX * 23 + 17;

                char[] questCharacters = mapLines[questLine].ToCharArray();

                questCharacters[questColumn] = '[';
                questCharacters[questColumn + 1] = '!';
                questCharacters[questColumn + 2] = ']';

                mapLines[questLine] = new string(questCharacters);
            }
            Console.WriteLine(string.Join("\n", mapLines));
            markerScreenX = markerColumn;
            markerScreenY = Console.CursorTop - (mapLines.Length - markerLine);
            if (!navigating)
            {
                Console.ReadLine();
            }
        }
        //Blinking player location
        public static char ReadBlinkingDirection()
        {
            int cursorX = Console.CursorLeft;
            int cursorY = Console.CursorTop;

            bool visible = true;

            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(markerScreenX, markerScreenY);

                if (visible)
                {
                    Console.Write("  ");
                }
                else
                {
                    Console.Write("->");
                }

                visible = !visible;

                Console.SetCursorPosition(cursorX, cursorY);

                Thread.Sleep(500);
            }

            Console.SetCursorPosition(markerScreenX, markerScreenY);
            Console.Write("->");

            Console.SetCursorPosition(cursorX, cursorY);

            return char.ToLower(Console.ReadKey(true).KeyChar);
        }
        // QUEST REGISTRATION

        // The story/NPC team will add quest here.
        public static void RegisterQuests()
        {
            
                RegisterQuest(
                    "side_merchant_01",
                    "The Missing Sword",
                    "Find a Rusty Sword and return to the merchant.",
                    false,
                    2, 3
                );
            
                RegisterQuest(
                  "side_merchant_02",
                  "The Merchant's Delivery",
                  "Deliver the merchant's package to the Witch's Hut.",
                  true,
                  0, 4,
                  "side_merchant_01"
                );
        }

        // Stores the definition of a quest in the game.
        public static bool RegisterQuest(
            string questId,
            string title,
            string description,
            bool isMainQuest,
            int mapX,
            int mapY,
            string prerequisiteId = "")
        {
            if (!ValidQuestId(questId) ||
                string.IsNullOrWhiteSpace(title) ||
                mapX < 0 || mapX > 4 ||
                mapY < 0 || mapY > 4 ||
                questId == prerequisiteId ||
                questDefinitions.ContainsKey(questId))
            {
                return false;
            }

            questDefinitions.Add(
                questId,
                new QuestDefinition(
                    title,
                    description,
                    isMainQuest,
                    prerequisiteId,
                    mapX,
                    mapY
                )
            );

            return true;
        }

        // Makes sure a quest ID is safe to use.
        static bool ValidQuestId(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId))
                return false;

            foreach (char c in questId)
            {
                if (!char.IsLetterOrDigit(c) &&
                    c != '_' && c != '-')
                {
                    return false;
                }
            }

            return true;
        }

        // The location of a quest's retry checkpoint.
        static string QuestCheckpointPath(string questId)
        {
            return Path.Combine(
                checkpointFolder,
                questId + ".txt"
            );
        }


        // GIVE QUEST

        public static bool GiveQuest(string questId)
        {
            QuestDefinition definition;

            // The quest must exist.
            if (!questDefinitions.TryGetValue(questId, out definition))
                return false;

            // Prevent accepting the same quest twice.
            if (acceptedQuests.Contains(questId) ||
                completedQuests.Contains(questId))
            {
                return false;
            }

            // Check whether the required previous quest is completed.
            if (definition.PrerequisiteId.Length > 0 &&
                !completedQuests.Contains(definition.PrerequisiteId))
            {
                return false;
            }

            acceptedQuests.Add(questId);

            // Autosave accepted quest.
            WriteGameSave(saveFile);

            // Create checkpoint for retrying this quest.
            Directory.CreateDirectory(checkpointFolder);
            WriteGameSave(QuestCheckpointPath(questId));

            return true;
        }


        // TRACK QUEST

        public static bool TrackQuest(string questId)
        {
            if (!acceptedQuests.Contains(questId) ||
                !questDefinitions.ContainsKey(questId))
            {
                return false;
            }

            trackedQuest = questId;
            activequest = true;

            WriteGameSave(saveFile);

            return true;
        }


        // UNTRACK QUEST

        public static bool UntrackQuest()
        {
            if (!activequest)
                return false;

            trackedQuest = "";
            activequest = false;

            WriteGameSave(saveFile);

            return true;
        }


        // COMPLETE QUEST

        public static bool CompleteQuest(string questId)
        {
            if (!acceptedQuests.Contains(questId))
                return false;

            acceptedQuests.Remove(questId);
            completedQuests.Add(questId);

            // Remove tracking if this quest was selected.
            if (trackedQuest == questId)
            {
                trackedQuest = "";
                activequest = false;
            }

            // Remove checkpoint for completed quest.
            string checkpoint = QuestCheckpointPath(questId);

            if (File.Exists(checkpoint))
                File.Delete(checkpoint);

            WriteGameSave(saveFile);

            return true;
        }


        // FAIL QUEST

        public static bool FailQuest(string questId)
        {
            if (!ValidQuestId(questId) ||
                !acceptedQuests.Contains(questId))
            {
                return false;
            }

            string checkpoint = QuestCheckpointPath(questId);

            if (!File.Exists(checkpoint))
                return false;

            // Restore the player's last quest checkpoint.
            if (!ReadGameSave(checkpoint))
                return false;

            // Keep the failed quest selected for another try.
            if (acceptedQuests.Contains(questId) &&
                questDefinitions.ContainsKey(questId))
            {
                trackedQuest = questId;
                activequest = true;
            }

            // Save the restored state, NOT the failed state.
            WriteGameSave(saveFile);

            return true;
        }


        // GET QUEST DESCRIPTION

        public static string GetQuestDescription(string questId)
        {
            QuestDefinition definition;

            if (questDefinitions.TryGetValue(questId, out definition))
            {
                return definition.Description;
            }

            return "Quest details are not registered yet.";
        }


        // DISPLAY MAIN OR SIDE QUESTS

        static void PrintAvailableQuests(
            bool mainQuest,
            List<string> selectableQuests)
        {
            Console.WriteLine(
                mainQuest ? "MAIN QUESTS" : "SIDE QUESTS"
            );

            bool found = false;

            foreach (string id in acceptedQuests)
            {
                QuestDefinition definition;

                if (questDefinitions.TryGetValue(id, out definition) &&
                    definition.IsMainQuest == mainQuest)
                {
                    selectableQuests.Add(id);

                    string tracking =
                        id == trackedQuest ? " [TRACKING]" : "";

                    Console.WriteLine(
                        $"[{selectableQuests.Count}] {definition.Title}{tracking}"
                    );

                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No quests discovered.");
            }
        }


        // QUEST JOURNAL / QUEST BOARD

        public static void Quest()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("========== QUEST JOURNAL ==========");
                Console.WriteLine();

                // This list connects menu numbers to quest IDs.
                List<string> selectable = new List<string>();

                PrintAvailableQuests(true, selectable);

                Console.WriteLine("-----------------------------------");

                PrintAvailableQuests(false, selectable);

                Console.WriteLine("-----------------------------------");
                Console.WriteLine("COMPLETED QUESTS (LOCKED)");

                if (completedQuests.Count == 0)
                {
                    Console.WriteLine("No quests completed.");
                }
                else
                {
                    foreach (string id in completedQuests)
                    {
                        QuestDefinition definition;

                        string title =
                            questDefinitions.TryGetValue(id, out definition)
                            ? definition.Title
                            : id;

                        Console.WriteLine($"[X] {title}");
                    }
                }

                Console.WriteLine("-----------------------------------");

                Console.WriteLine("[Number] track a quest");
                Console.WriteLine("[U] Untrack current quest");
                Console.WriteLine("[0] Return to exploration");

                Console.Write("\nChoose: ");

                string input = Console.ReadLine()?.Trim() ?? "";

                if (input == "0")
                {
                    return;
                }

                if (input.Equals(
                    "u", StringComparison.OrdinalIgnoreCase))
                {
                    UntrackQuest();
                    continue;
                }

                int number;

                if (!int.TryParse(input, out number) ||
                    number < 1 ||
                    number > selectable.Count)
                {
                    Console.WriteLine("Choose a quest number shown above.");
                    Console.ReadLine();
                    continue;
                }

                string selectedId = selectable[number - 1];

                QuestDefinition selected = questDefinitions[selectedId];

                Console.Clear();
                Console.WriteLine($"========== {selected.Title} ==========");

                Console.WriteLine(
                    selected.IsMainQuest ? "Main Quest" : "Side Quest"
                );

                Console.WriteLine(GetQuestDescription(selectedId));

                Console.WriteLine(
                    $"Destination: map ({selected.MapX}, {selected.MapY})"
                );

                Console.WriteLine();
                Console.WriteLine("[T] Track this quest on the map");
                Console.WriteLine("[B] Back to journal");

                string action = Console.ReadLine()?.Trim() ?? "";

                if (action.Equals(
                    "t", StringComparison.OrdinalIgnoreCase))
                {
                    TrackQuest(selectedId);
                }
            }
        }
    }
}