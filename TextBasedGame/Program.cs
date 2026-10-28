
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        static int playerWpnStat = 0;
        static int playerAmrStat = 0;
        static string playerWpn = "";
        static string playerAmr = "";
        static int playerX = 2; // Start in Dark Forest
        static int playerY = 2;
        static int markerScreenX;
        static int markerScreenY;
        static bool keyall = false;

        //Randomizer
        static Random rand = new Random();
        
        //Shop
        static int healingPotionPrice = 4;
        static int rustyArmourPrice = 8;
        static int steelSwordPrice = 20;
        static int steelSwordLV = 5;
        static int steelArmourPrice = 20;
        static int steelArmourLV = 5;
        static bool angeredMerchant = false;

        //Items
        static int rustySwordDMG = 3;
        static int rustyArmourDEF = 3;
        static int steelSwordDMG = 8;
        static int steelArmourDEF = 8;
        static bool shellFound = false;

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

        static int enemyDamage = 0;
        public struct Enemies
        {
            public int enemyID;
            public string enemyName;
            public int enemyMaxHealth;
            public int enemyMinAttack;
            public int enemyMaxAttack;
            public int enemyPhysicalResistance;
            public int enemyMagicResistance;
            public int enemyDodgeChance;
            public int enemyExperienceReward;
            public int enemyGoldRewardRange;
        }

        //load available enemies
        //small enemies
        static Enemies[] enemies = new Enemies[4];
        static int enemyCount = 0;

        //bosses
        static Enemies[] bosses = new Enemies[4];
        static int bossCount = 0;
        //Basic enemy loader
        static int LoadEnemies(Enemies[] enemy, string filePath)
        {
            using StreamReader sr = new StreamReader(filePath);

            int loadedCount = 0;
            string? lineReader;

            while ((lineReader = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(lineReader))
                    continue;

                string[] enemySpecifics = lineReader.Split(',');

                for (int i = 0; i < enemySpecifics.Length; i++)
                    enemySpecifics[i] = enemySpecifics[i].Trim();

                if (enemySpecifics.Length != 10)
                    continue;

                if (loadedCount >= enemy.Length)
                    break;

                enemy[loadedCount] = new Enemies
                {
                    enemyID = int.Parse(enemySpecifics[0]),
                    enemyName = enemySpecifics[1],
                    enemyMaxHealth = int.Parse(enemySpecifics[2]),
                    enemyMinAttack = int.Parse(enemySpecifics[3]),
                    enemyMaxAttack = int.Parse(enemySpecifics[4]),
                    enemyPhysicalResistance = int.Parse(enemySpecifics[5]),
                    enemyMagicResistance = int.Parse(enemySpecifics[6]),
                    enemyDodgeChance = int.Parse(enemySpecifics[7]),
                    enemyExperienceReward = int.Parse(enemySpecifics[8]),
                    enemyGoldRewardRange = int.Parse(enemySpecifics[9])
                };

                loadedCount++;
            }

            return loadedCount;
        }

        //Combat Information
        static List<string> combatLog = new List<string>();

        //add more enemies based on level

        // Main void for running game then ending
        static void Main()
        {
            bool gameFinished = false;
            enemyCount = LoadEnemies(enemies, "../../../enemyList.txt");
            bossCount = LoadEnemies(bosses, "../../../bossList.txt");
            // all quest definitions
            RegisterQuests();
            // Loading a save if there is one
            Load();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool gameRunning = true;

            Console.WriteLine("\nThis game is best played maximised.");
            PauseStory();

            if (File.Exists(saveFile) == false)
            {
                Intro();
                CreateCharacter();
            }

            while (gameRunning && playerHealth > 0 && gameFinished == false)
            {
                gameRunning = Menu(ref gameFinished);
            }

            if(playerHealth > 0 && gameFinished == true)
            {
                Outro();
            }
            
            Console.Clear();
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

                $"playerWpnStat={playerWpnStat}",
                $"playerAmrStat={playerAmrStat}",

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

                else if (key == "playerWpnStat")
                    int.TryParse(value, out playerWpnStat);

                else if (key == "playerAmrStat")
                    int.TryParse(value, out playerAmrStat);

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

            if (padding > 0)
            {
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
            playerName = Console.ReadLine() ?? "";

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

        //Outro
        static void Outro()
        {
            CenterText($"{playerName}, thank you for saving our world!");
            Console.ReadLine();

            CenterText("You get transported back to where you were before");
            Console.ReadLine();

        }
        //Inventory Check for item
        static bool InvCheck(string itemNeeded)
        {
            bool result = false;
            foreach (string item in inventory)
            {
                if (item == itemNeeded)
                {
                    result = true;
                }
            }
            return result;
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
        static bool Menu(ref bool gameFinished)
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
                    Investigate(ref gameFinished);
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
            playerHealth = playerMaxHealth;
            playerMana = playerMaxMana;
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
                            string[] tempInv = new string[inventoryMaxSize];
                            for (int i = 0; i < inventoryMaxSize; i++)
                            {
                                tempInv[i] = inventory[i];
                            }
                            inventoryMaxSize = 5 + playerStrength;
                            inventory = new string[inventoryMaxSize];
                            for (int i = 0; i < tempInv.Length - 1; i++)
                            {
                                inventory[i] = tempInv[i];
                            }
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

            if (combatLog.Count > 21)
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


        static void EnemyEncounter(Enemies enemy)
        {
            Console.WriteLine("\n========== ENCOUNTER ==========");
            Console.WriteLine($"A {enemy.enemyName} approaches!");
            Console.WriteLine("\nPress Enter to begin combat.");
        }

        static void CombatInformation(Enemies enemy, int currentHealth)
        {
            Console.WriteLine("---------- COMBAT ----------");
            Console.WriteLine($"{playerName}:");
            Console.WriteLine($"{playerHealth}/{playerMaxHealth} HP");
            Console.WriteLine($"{playerMana}/{playerMaxMana} MP");
            Console.WriteLine($"\n{enemy.enemyName}: \n{currentHealth}/{enemy.enemyMaxHealth} HP");
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
        static bool AttackCommand(ref int playerDamage, ref string enemyName, ref int enemyHealth, Enemies enemy)
        {
            //variables for hit chance
            int baseHitChance = 75, finalHitChance;

            //variables for crit chance
            int finalCritChance;

            //variable for actual damage
            int finalAttackDamage;

            //enemy dodge
            int enemyDodge;

            //initial display
            Console.WriteLine("You chose attack!");
            Console.ReadLine();
            finalHitChance = Math.Min(100, baseHitChance + playerMeleeHit);
            
            int hitRoll = rand.Next(1, 101);

            if (hitRoll <= finalHitChance)
            {
                //enemy dodge
                enemyDodge = Math.Clamp(enemy.enemyDodgeChance, 0, 100);
                int enemyDodgeRoll = rand.Next(1, 101);
                if(enemyDodgeRoll <= enemyDodge) {
                    Console.WriteLine($"The {enemy.enemyName} dodges your attack!");
                    AddCombatLog($"The {enemy.enemyName} dodges your attack!");
                }

                //if attack connects
                else
                {
                    finalCritChance = Math.Clamp(playerCritChance, 0, 100);
                    int critRoll = rand.Next(1, 101);
                    bool isCritical = critRoll <= finalCritChance;

                    //if attack is critical
                    if (isCritical)
                    {
                        finalAttackDamage = (playerDamage + playerMeleeDmg) * 2;
                    }

                    else
                    {
                        finalAttackDamage = playerDamage + playerMeleeDmg;
                    }

                    //calculate damage base on player and enemy stats
                    finalAttackDamage = Math.Max(0, finalAttackDamage - enemy.enemyPhysicalResistance);
                    enemyHealth -= finalAttackDamage;


                    //display output
                    if (isCritical)
                    {
                        Console.WriteLine($"\nYou attack the {enemyName} for {finalAttackDamage} CRITICAL HIT.");
                        AddCombatLog($"You attack the {enemyName} for {finalAttackDamage} CRITICAL HIT.");
                    }
                    else
                    {
                        Console.WriteLine($"\nYou attack the {enemyName} for {finalAttackDamage} damage.");
                        AddCombatLog($"You attack the {enemyName} for {finalAttackDamage} damage.");
                    }

                    //add mana after attacking
                    playerMana = Math.Min(playerMana + (playerDamage / 10), playerMaxMana);
                    
                }
                
            }

            else
            {

                //if player misses attack
                Console.WriteLine("Your attack misses!");
                AddCombatLog("Your attack misses!");
            }



            return true;
        }


        //magic
        static void DisplayAvailableMagic()
        {
            bool staffFireball, wandLightning, scrollBlight, orbDisintegrate, ringGraviton;
            Console.WriteLine("\nYou chose magic!");
            Console.WriteLine();
            Console.WriteLine("[M] - Magic Missile - 3 MP");
            // Temporary bool to test unlocking magic
            staffFireball = InvCheck("Staff of Fireball");
            if (staffFireball == true)
            {
                Console.WriteLine("[F] - Fireball - 6 MP");
            }
            wandLightning = InvCheck("Wand of Lightning");
            if (wandLightning == true)
            {
                Console.WriteLine("[L] - Lightning Bolt - 8 MP");
            }
            scrollBlight = InvCheck("Scroll of Blight");
            if (scrollBlight == true)
            {
                Console.WriteLine("[B] - Blighting Winds - 11 MP");
            }
            orbDisintegrate = InvCheck("Orb of Disintegrate");
            if (orbDisintegrate == true)
            {
                Console.WriteLine("[D] - Disintegration Ray - 14 MP");
            }
            ringGraviton = InvCheck("Ring of Graviton");
            if (ringGraviton == true)
            {
                Console.WriteLine("[G] - Gravity Collapse - 17 MP");
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
                if (magicInput != "m" && magicInput != "0" && magicInput != "f" && magicInput != "l" && magicInput != "b" && magicInput != "d" && magicInput != "g")
                {
                    Console.WriteLine("Invalid selection, please try again");
                }
            } while (magicInput != "m" && magicInput != "0" && magicInput != "f" && magicInput != "l" && magicInput != "b" && magicInput != "d" && magicInput != "g");
            return magicInput;
        }


        static bool MagicSubMenu(char spellSelect, string enemyName, ref int enemyHealth, Enemies enemy)
        {
            int enemyMagicResist = Math.Max(0, enemy.enemyMagicResistance);
            int finalMagicDamage;
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
                        finalMagicDamage = Math.Max(0, playerDamage - enemyMagicResist);
                        enemyHealth -= finalMagicDamage;

                        string message =
                            $"You cast Magic Missile, blasting {enemyName} for {finalMagicDamage} damage.";

                        Console.WriteLine(message);
                        AddCombatLog(message);
                    }
                    else
                    {
                        playerDamage = rand.Next(8, 25) - playerMagicResist;
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
                    bool staffFireball = InvCheck("Staff of Fireball");
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
                            finalMagicDamage = Math.Max(0, playerDamage - enemyMagicResist);
                            enemyHealth -= finalMagicDamage;

                            string message =
                                $"You cast Fireball, scorching {enemyName} for {finalMagicDamage} damage.";

                            Console.WriteLine(message);
                            AddCombatLog(message);
                        }
                        else
                        {
                            playerDamage = rand.Next(8, 25) - playerMagicResist;
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
                case 'l':
                    bool wandLightning = InvCheck("Wand of Lightning");
                    while (wandLightning == true)
                    {
                        if (playerMana < 8)
                        {
                            Console.WriteLine("You do not have enough Mana to cast Lightning Bolt.");
                            return false;
                        }

                        playerMana -= 8;

                        casting = rand.Next(1, 101) + playerSpellMiscast;

                        if (casting <= 45)
                        {
                            playerDamage = rand.Next(20, 45) + (playerSpellDmg * 2);
                            finalMagicDamage = Math.Max(0, playerDamage - enemyMagicResist);
                            enemyHealth -= finalMagicDamage;

                            string message =
                                $"You cast Lightning Bolt, zapping {enemyName} for {finalMagicDamage} damage.";

                            Console.WriteLine(message);
                            AddCombatLog(message);
                        }
                        else
                        {
                            playerDamage = rand.Next(8, 25) - playerMagicResist;
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
                case 'b':
                    bool scrollBlight = InvCheck("Scroll of Blight");
                    while (scrollBlight == true)
                    {
                        if (playerMana < 11)
                        {
                            Console.WriteLine("You do not have enough Mana to cast Blighting Winds.");
                            return false;
                        }

                        playerMana -= 11;

                        casting = rand.Next(1, 101) + playerSpellMiscast;

                        if (casting <= 35)
                        {
                            playerDamage = (rand.Next(10, 30)) + (5 * playerSpellDmg);
                            finalMagicDamage = Math.Max(0, playerDamage - enemyMagicResist);
                            enemyHealth -= finalMagicDamage;

                            string message =
                                $"You cast Blighting Winds, {enemyName} decays away for {finalMagicDamage} damage.";

                            Console.WriteLine(message);
                            AddCombatLog(message);
                        }
                        else
                        {
                            playerDamage = rand.Next(8, 25) - playerMagicResist;
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
                case 'd':
                    bool orbDisintegrate = InvCheck("Orb of Disintegrate");
                    while (orbDisintegrate == true)
                    {
                        if (playerMana < 14)
                        {
                            Console.WriteLine("You do not have enough Mana to cast Disintegration Ray.");
                            return false;
                        }

                        playerMana -= 14;

                        casting = rand.Next(1, 101) + playerSpellMiscast;

                        if (casting <= 20)
                        {
                            playerDamage = (15 * playerSpellDmg) - rand.Next(0, (3 * playerSpellDmg));
                            finalMagicDamage = Math.Max(0, playerDamage - enemyMagicResist);
                            enemyHealth -= finalMagicDamage;

                            string message =
                                $"You cast Disintegration Ray, You atomize {enemyName} for {finalMagicDamage} damage.";

                            Console.WriteLine(message);
                            AddCombatLog(message);
                        }
                        else
                        {
                            playerDamage = rand.Next(8, 25) - playerMagicResist;
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
                case 'g':
                    bool ringGraviton = InvCheck("Ring of Graviton");
                    while (ringGraviton == true)
                    {
                        if (playerMana < 17)
                        {
                            Console.WriteLine("You do not have enough Mana to cast Gravity Collapse.");
                            return false;
                        }

                        playerMana -= 17;

                        casting = rand.Next(1, 101) + playerSpellMiscast;

                        if (casting <= 10)
                        {
                            playerDamage = playerSpellDmg * (rand.Next(10, 21));
                            finalMagicDamage = Math.Max(0, playerDamage - enemyMagicResist);
                            enemyHealth -= finalMagicDamage;

                            string message =
                                $"You cast Gravity Collapse, You crush {enemyName} for {finalMagicDamage} damage.";

                            Console.WriteLine(message);
                            AddCombatLog(message);
                        }
                        else
                        {
                            playerDamage = rand.Next(8, 25) - playerMagicResist;
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
        static bool CheckEnemyHealth(Enemies enemy, ref int enemyHealth)
        {
            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                Console.WriteLine($"You have defeated the {enemy.enemyName}!");

                playerGold += enemy.enemyGoldRewardRange;
                Console.WriteLine($"\nYou gained {enemy.enemyGoldRewardRange} gold!");

                EnemyItemDrop();

                playerLevel = GainXP(enemy.enemyExperienceReward);

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
                Console.WriteLine($"{playerName} has been defeated.");
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
            int dodgeRoll = rand.Next(1, 101);
            if (dodgeRoll <= playerDodgeChance)
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
            if (blockRoll <= playerBlock)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //enemy attack
        static void EnemyTurn(Enemies enemy)
        {
            bool dodgeAttack = CombatDodge();
            bool blockAttack;


            //physical resist
            int finalDamage;
            if (dodgeAttack == true)
            {
                Console.WriteLine($"The {enemy.enemyName} attacks you but you have dodged!");
                AddCombatLog($"The {enemy.enemyName} attacks you but you have dodged!");
                AddCombatLog("\n");
            }
            else
            {
                blockAttack = CombatBlock();
                enemyDamage = rand.Next(enemy.enemyMinAttack, enemy.enemyMaxAttack + 1);
                if (blockAttack == true)
                {
                    enemyDamage = Math.Max(0, enemyDamage - playerBlockReduction);

                    Console.WriteLine($"The {enemy.enemyName} attacks but you have blocked it!");
                    AddCombatLog($"The {enemy.enemyName} attacks but you have blocked it!");
                }
                else
                {
                    Console.WriteLine($"The {enemy.enemyName} attacks!");
                    AddCombatLog($"The {enemy.enemyName} attacks!");
                }

                finalDamage = Math.Max(0, enemyDamage - playerPhysicalResist);

                playerHealth -= finalDamage;
                Console.WriteLine($"You received {finalDamage} damage!");
                AddCombatLog($"You received {finalDamage} damage!");
                AddCombatLog("\n");
            }

        }
        static bool CombatMenu(Enemies enemy)
        {
            combatLog.Clear();
            string enemyName = enemy.enemyName;
            int enemyHealth = enemy.enemyMaxHealth;
            bool inCombat = true;
            bool playerTurnCompleted = false, playerEscape = false, runAway, enemyHPCheck, playerHPCheck;
            Console.Clear();
            EnemyEncounter(enemy);
            Console.ReadLine();

            while (inCombat && playerHealth > 0 && enemyHealth > 0)
            {
                playerTurnCompleted = false;

                //magic local variables
                string userselect;

                Console.Clear();
                DisplayCombatLog();
                CombatInformation(enemy, enemyHealth);
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
                            playerDamage = rand.Next(playerMeleeDmg + 5, playerMeleeDmg + 15);
                            playerTurnCompleted = AttackCommand(ref playerDamage, ref enemyName, ref enemyHealth, enemy);
                            break;

                        case 'm':
                            DisplayAvailableMagic();
                            userselect = MagicInput();
                            char spellselect = userselect[0];
                            playerTurnCompleted = MagicSubMenu(spellselect, enemyName, ref enemyHealth, enemy);
                            break;


                        case 'i':
                            ViewInventory();
                            continue;

                        case 'r':
                            runAway = RunCommand(enemyName);
                            if (runAway == true)
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
                enemyHPCheck = CheckEnemyHealth(enemy, ref enemyHealth);
                if (enemyHPCheck == true)
                {
                    inCombat = false;
                    playerTurnCompleted = false;
                }

                //check for player hp
                playerHPCheck = CheckPlayerHealth(ref playerHealth);
                if (playerHPCheck == true)
                {
                    playerTurnCompleted = false;
                    inCombat = false;
                }


                //enemy attacks after a player turn and enemy hp > 0
                if (playerTurnCompleted == true && enemyHPCheck == false)
                {
                    if (inCombat == true && playerTurnCompleted == true && enemyHealth > 0)
                    {
                        EnemyTurn(enemy);
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

            return enemyHealth <= 0;
        }

        //END OF COMBAT COMMANDS




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
                if(enemyCount == 0)
                {
                    Console.WriteLine("No enemies were loaded");
                    Console.ReadLine();
                    return;
                }

                int index = rand.Next(0, enemyCount);
                Enemies encounteredEnemy = enemies[index];

                Console.WriteLine($"\nA {encounteredEnemy.enemyName} suddenly appears!");
                AddCombatLog($"\nA {encounteredEnemy.enemyName} suddenly appears!");
                //Combat system function
                CombatMenu(encounteredEnemy);
            }
            else
            {
                Console.WriteLine("The path is clear.");
            }
        }
        // Boss Loader
        static bool BossLoader(int index)
        {
            Enemies encounteredBoss = bosses[index];
            Console.WriteLine($"{encounteredBoss.enemyName} arrives!");
            return CombatMenu(encounteredBoss);
        }
        //NPC
        //Merchant

        static void Merchant()
        {
            Console.Clear();
            Console.WriteLine("========== MERCHANT ==========");

            // All quests finished
            if (completedQuests.Contains("main_quest_01"))
            {
                Console.WriteLine("Merchant: Thank you for all your help!");
                return;
            }

            //Main quest in progress
            if (acceptedQuests.Contains("main_quest_01"))
            {
                Console.WriteLine("Merchant: You must slay those undead! They are ruining my business!");
                return;
            }
            //Second quest completed: offer main quest start
            if (completedQuests.Contains("side_merchant_02"))
            {
                Console.WriteLine($"Merchant: {playerName}! You must help an pair of powerful undead have been attacking my caravans!");
                Console.WriteLine("Merchant: Will you help me? please?");
                Console.WriteLine("[1] Accept quest to slay the undead");
                Console.WriteLine("[2] Decline");

                char choice = Console.ReadKey().KeyChar;

                if (choice == '1')
                {
                    bool given = GiveQuest("main_quest_01");

                    if (given)
                    {
                        Console.WriteLine("New quest: The Dark Twins");
                    }
                }

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
        static void MerchantShop()
        {
            Console.Clear();
            Console.WriteLine("\n========== MERCHANTS SHOP ==========");
            Console.WriteLine($"1. Healing Potion: {healingPotionPrice}G");
            Console.WriteLine($"2. Rusty Armour: {rustyArmourPrice}G");
            Console.WriteLine($"3. [LV5] Steel Sword: {steelSwordPrice}G");
            Console.WriteLine($"4. [LV5] Steel Armour: {steelArmourPrice}G");
            if (shellFound == true)
            {
                Console.WriteLine("9. Offer shell");
            }

            Console.WriteLine("\n[NUM] Purchase Item");
            Console.WriteLine("[ANY OTHER INPUT] Leave");

            Console.WriteLine($"\nGold: {playerGold}G");

            if (inventoryCount == inventoryMaxSize)
            {
                Console.WriteLine("\nYour inventory is full! Come back when you've got space.");
                PauseStory();
            }
            else
            {
                char marketChoice = Console.ReadKey(true).KeyChar;

                switch (marketChoice)
                {
                    case '1':
                        CheckGold("Healing Potion", healingPotionPrice, 0);
                        break;

                    case '2':
                        CheckGold("Rusty Armour", rustyArmourPrice, 0);
                        break;

                    case '3':
                        CheckGold("Steel Sword", steelSwordPrice, steelSwordLV);
                        break;

                    case '4':
                        CheckGold("Steel Armour", steelArmourPrice, steelArmourLV);
                        break;

                    case '9':
                        SellShell(ref angeredMerchant);

                        break;

                    default:
                        break;
                }
            }
        }

        static void SellShell(ref bool angeredMerchant)
        {
            if (angeredMerchant == false)
            {
                Console.WriteLine("\nYou offer the unique shell to the merchant...");
                Console.WriteLine("\nMerchant: Hmmm... I'll give you 6 gold for it.");
                Console.WriteLine("\n[y] Sell shell for 6 gold");
                Console.WriteLine("[n] Haggle for a better price");
                char shellChoice = Console.ReadKey(true).KeyChar;
                
                switch (shellChoice)
                {
                    case 'y':
                        Console.WriteLine("\nYou sold the shell for 6 gold!");
                        playerGold += 6;
                        PauseStory();
                        break;

                    case 'n':
                        Console.Clear();
                        Console.WriteLine("\n========== MERCHANTS SHOP ==========");
                        Console.WriteLine("Merchant: Grr.. Fine, 8 gold. Take it or leave it.");
                        Console.WriteLine("\n[y] Sell shell for 8 gold");
                        Console.WriteLine("[n] Haggle for a better price again");
                        char shellChoice2 = Console.ReadKey(true).KeyChar;

                        switch (shellChoice2)
                        {
                            case 'y':
                                Console.WriteLine("\nYou sold the shell for 8 gold!");
                                playerGold += 8;
                                PauseStory();
                                break;

                            case 'n':
                                Console.Clear();
                                Console.WriteLine("\n========== MERCHANTS SHOP ==========");
                                Console.WriteLine("Merchant: You know what?! I don't want it anymore. Get this shell out of my face, fool!");
                                Console.WriteLine("\nIt seems the Merchant has a short temper and has been haggled too much, now he won't buy the shell.");
                                PauseStory();
                                angeredMerchant = true;
                                break;

                            default:
                                Console.WriteLine("\nUnrecognised input.");
                                PauseStory();
                                break;
                        }
                        break;

                    default:
                        Console.WriteLine("\nUnrecognised input.");
                        PauseStory();
                        break;
                }

            }
            else
            {
                Console.WriteLine("\nMerchant: You had your chance. Get out of here with that stupid shell! I'm not trading with someone so foolish.");
                PauseStory();
            }
        }

        static void CheckGold(string item, int price, int level)
        {
            if (playerLevel >= level)
            {
                if (playerGold >= price)
                {
                    playerGold -= price;
                    Console.WriteLine($"\nYou bought the {item}! You now have {playerGold} gold.");
                    AddItem(item);
                }
                else
                {
                    Console.WriteLine("\nYou do not have enough gold to buy this.");
                }
            }
            else
            {
                Console.WriteLine("\nYou are not a high enough level to buy this. The merchant only sells his wares to those who deserve them.");
            }
        }

        // Investigation command
        static void Investigate(ref bool gameFinished)
        {
            switch ((playerX, playerY))
            {
                // MARKET (2,3)
                case (2, 3):
                    {
                        Console.Clear();
                        Console.WriteLine("\n========== MARKET ==========");
                        Console.WriteLine("[1] Talk to merchant");
                        Console.WriteLine("[2] Check the merchants shop");
                        Console.WriteLine("[3] Search for loot");
                        Console.WriteLine("[0] Leave");

                        char choice = Console.ReadKey(true).KeyChar;

                        switch (choice)
                        {
                            case '1':
                                Merchant();
                                break;

                            case '2':
                                MerchantShop();
                                break;

                            case '3':
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

                            case '0':
                                Console.WriteLine("You leave the market.");
                                break;

                            default:
                                Console.WriteLine("Invalid option.");
                                break;
                        }
                        break;
                    }

                // BEACH (2,4)
                case (2, 4):
                    Console.WriteLine("\nYou search the beach...");
                    if (shellFound == false)
                    {
                        Console.WriteLine("\nYou found a unique looking shell! You can hear the sounds of the ocean when you hold it up to your ear...");
                        Console.WriteLine("Someone might want to buy this.");
                        if (inventoryCount == inventoryMaxSize)
                        {
                            Console.WriteLine("\nBut wait! Your pockets are too full to carry the shell.");
                            PauseStory();
                        }
                        else
                        {
                            Console.WriteLine();
                            AddItem("Unique Shell");
                            shellFound = true;
                            PauseStory();
                        }
                    }
                    else if (shellFound == true)
                    {
                        Console.WriteLine("\nIt seems there aren't any more cool shells to find...");
                        PauseStory();
                    }
                    break;

                // THE LIGHT KINGDOM (3,4)
                case (3, 4):
                    Console.WriteLine("\nFrom the large fields in front, you stand and stare upon the great Light Kingdom.");
                    Console.WriteLine("Its sheer size and sense of importance to this world overwhelms you. How many stories are out there? How much does this world contain?");
                    Console.WriteLine("These questions are irrelevant to your purpose. Best to keep marching on your quest.");
                    PauseStory();
                    break;

                // DARK FOREST (2,2)
                case (2, 2):
                    Console.WriteLine("\nYou search the Dark Forest...");

                    int result = rand.Next(1, 5);

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

                        case 4:
                            Console.WriteLine("staffFireball test item");
                            AddItem("Staff of Fireball");
                            break;
                    }
                    PauseStory();
                    break;


                // WITCH'S HUT (0,4)
                case (0, 4):
                    {
                        Console.WriteLine("\nYou approach the Witch's Hut.");

                        if (acceptedQuests.Contains("side_merchant_02"))
                        {
                            Console.WriteLine("The witch accepts the merchant's delivery.");
                            GainXP(30);

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
                    Console.WriteLine("\nYou investigate the dark cave.");
                    Console.WriteLine("Strange noises echo through the tunnels.");
                    break;


                // ROYAL ARMY (0,3)
                case (0, 3):
                    Console.WriteLine("\nYou approach the Royal Army's camp.");
                    Console.WriteLine("Soldiers patrol the area.");

                    // Later: TalkToCommander();
                    break;

                //UNDEAD TWINS BOSS FIGHT (evil twins)
                case (4, 4):
                    Console.WriteLine("\nYou hear rustling from the twisting brambles");
                    Console.ReadLine();
                    if (acceptedQuests.Contains("main_quest_01"))
                    {
                        Console.WriteLine("Suddenly, a pair of undead rush you from the brambles!");
                        Console.ReadLine();
                        bool bossDefeated = BossLoader(0);
                        
                        if(bossDefeated)
                        {
                            Console.WriteLine("The Undead twins lie defeated");
                            Console.ReadLine();
                            GainXP(100);
                            CompleteQuest("main_quest_01");
                            AddItem("Rusted Key");
                            AddItem("Twin's Journal");
                            JournalHint();
                            CompleteQuest("main_quest_01");
                        }

                        bool given1 = GiveQuest("main_quest_02");
                        if (given1)
                        {
                            Console.WriteLine("New Quest: The Broken Paladin");
                        }
                        bool given2 = GiveQuest("main_quest_03");
                        if (given2)
                        {
                            Console.WriteLine("New Quest: The Devil's Dealer");
                        }
                        if (completedQuests.Contains("main_quest_01"))
                        {
                            Console.WriteLine("The brambles are now silent");
                        }
                    }
                    else if ((completedQuests.Contains("main_quest_01")) && ((InvCheck("Rusted Key") == false)))
                    {
                        Console.WriteLine("The Rusted key lies on the ground where you left it");
                        AddItem("Rusted Key");
                    }
                    else
                    {
                        Console.WriteLine("You found nothing!");
                    }
                    break;

                //PALADIN BOSS FIGHT (light knight camp)
                case (3, 1):
                    if (acceptedQuests.Contains("main_quest_02"))
                    {
                        Console.WriteLine("\nThe Paladin ambushes you!");
                        Console.ReadLine();
                        bool bossDefeated = BossLoader(1);
                        if (bossDefeated)
                        {
                            GainXP(150);
                            AddItem("Adorned Key");
                            CompleteQuest("main_quest_02");
                        }
                    }
                    if ((completedQuests.Contains("main_quest_02")) && (InvCheck("Adorned Key") == false))
                    {
                        Console.WriteLine("The Adorned key lies on the ground where you left it");
                        AddItem("Adorned Key");
                    }

                    break;

                //WARLOCK BOSS FIGHT (casino)
                case (2, 0):
                    if (acceptedQuests.Contains("main_quest_03"))
                    {
                        Console.WriteLine("\nThe Warlock curses you!");
                        Console.ReadLine();
                        bool bossDefeated = BossLoader(2);
                        if (bossDefeated)
                        {
                            GainXP(200);
                            Console.WriteLine("You have defeated the Warlock!");
                            Console.WriteLine("As their form dissolves into smoke and shadow, a key is left behind as laughter echoes around you");
                            AddItem("Twisted Key");
                            Console.WriteLine("You begin to suspect that defeating this particular warlock may be harder than you first thought. But you have the key you need");
                            GainXP(200);
                            CompleteQuest("main_quest_03");

                        }
                    }
                    if ((completedQuests.Contains("main_quest_03")) && (InvCheck("Twisted Key") == false))
                    {
                        Console.WriteLine("A Twisted key lies on the ground where you left it");
                        AddItem("Twisted Key");
                    }
                    break;

                //FINAL BOSS (temple)
                case (1, 3):
                    bool access = FinalBossReq();
                    if (access == true)
                    {
                        Console.ReadLine();
                        Console.WriteLine("THE GOD!");
                        bool bossDefeated = BossLoader(3);
                        if (bossDefeated)
                        {
                            Console.WriteLine("???");
                            Console.ReadLine();
                            CompleteQuest("main_quest_04");
                            gameFinished = true;
                        }
                    }
                    break;
                // ALL OTHER LOCATIONS
                default:
                    Console.WriteLine("\nThere is nothing special to investigate here yet.");
                    break;
            }
        }
        static void JournalHint()
        {
            Console.Clear();
            CenterText("---- Journal of Alabaster and Ronnie ----");
            CenterText("");
            CenterText("We're off to go and slay that lich, getting paid well for it too");
            CenterText("not just us though, Wizard hired some others as well.");
            CenterText("");
            CenterText("A Paladin from that light order bunch, keeps giving us the side eye");
            CenterText("and some Warlock who can't stop complaining about a run of bad bets.");
            CenterText("");
            CenterText("Said to face the lich at that old temple, but told us to find some keys first");
            CenterText("in case things went bad, but we'll be fine, just some old bag of bones with magic");
            CenterText("What's the worst that could happen?");
            CenterText("---- The next serveral pages are ragged and torn ----");
            CenterText("");
            CenterText("We all made it out, locked that bag of bones in there");
            CenterText("Can still hear them cackling though, in my head, Ronnie can too...");
            CenterText("");
            CenterText("Don't know what the lich did but the others are out of it as well");
            CenterText("");
            CenterText("Gotta leave before this cackling gets worse, was my skin always this grey...?");
            CenterText("");
            CenterText("Press ENTER to close the journal");
            Console.ReadLine();
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
        // Post combat item generator
        static void EnemyItemDrop()
        {
            Random rand = new Random();
            int pool = rand.Next(1, 11), select;
            switch (pool)
            {
                // Melee Weapon pool
                case 1:
                    select = rand.Next(1, 11);
                    switch (select)
                    {
                        case 1: case 2: case 3: case 4:
                            AddItem("Rusty Sword");
                            break;
                        case 5: case 6: case 7:
                            AddItem("Steel Sword");
                            break;
                        case 8: case 9:
                            AddItem("Spellfire Sword");
                            break;
                        case 10:
                            AddItem("Throne-Cleaver");
                            break;
                    }
                    break;
                // Armour pool
                case 2:
                    select = rand.Next(1, 10);
                    switch (select)
                    {
                        case 1: case 2: case 3:
                            AddItem("Rusty Armour");
                            break;
                        case 4: case 5: case 6:
                            AddItem("Leather Armour");
                            break;
                        case 7: case 8:
                            AddItem("Steel Armour");
                            break;
                        case 9: case 10:
                            AddItem("Studded Leather Armour");
                            break;

                    }
                    break;
                // Range Weapon pool
                case 3:
                    select = rand.Next(1, 11);
                    switch (select)
                    {
                        case 1: case 2: case 3:
                            AddItem("Old Bow");
                            break;
                        case 4: case 5:
                            AddItem("Steelframe Bow");
                            break;
                        case 6: case 8:
                            AddItem("Spellfire Bow");
                            break;
                        case 9:
                            AddItem("Wither-String");
                            break;
                    }
                    break;
                // Magic Item pool
                case 4:
                    select = rand.Next(1, 12);
                    switch (select)
                    {
                        case 1: case 2: case 3:
                            AddItem("Staff of Fireball");
                            break;
                        case 4: case 5: case 6:
                            AddItem("Wand of Lightning");
                            break;
                        case 7: case 8:
                            AddItem("Scroll of Blight");
                            break;
                        case 9: case 10:
                            AddItem("Orb of Disintegration");
                            break;
                        case 11:
                            AddItem("Ring of Graviton");
                            break;

                    }
                    break;
                // Potion pool
                case 5:
                    select = rand.Next(1, 11);
                    switch (select)
                    {
                        case 1: case 2: case 3:
                            AddItem("Healing Potion");
                            break;
                        case 4: case 5: case 6:
                            AddItem("Mana Potion");
                            break;
                        case 7: case 8:
                            AddItem("Greater Healing Potion");
                            break;
                        case 9: case 10:
                            AddItem("Greater Mana Potion");
                            break;
                    }
                    break;
                // No item drop
                default:
                    break;
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

            Console.WriteLine($"\nEquipped Weapon: {playerWpn}");
            Console.WriteLine($"Equipped Armour: {playerAmr}");

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
            // Use several items in a row
            static void UseItems()
            {
                bool useAnother = true;
                while (useAnother && inventoryCount > 0)
                {
                    UseOneItem();
                    if (inventoryCount == 0)
                    {
                        break;
                    }
                    Console.Clear();
                    Console.WriteLine("\n---------- INVENTORY ----------");
                    Console.Write("\nUse another item? (y/n): ");
                    string useAnswer = Console.ReadLine()?.Trim().ToLower() ?? "";
                    useAnother = useAnswer.StartsWith("y");
                }
            }
            // Using items
            static void UseOneItem()
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

                switch (inventory[useChoice - 1])
                {
                    // What happens when equipping weapons, calculates stat change as well.
                    case "Rusty Sword":
                        equipWpn(ref useChoice, ref slotEmpty);
                        playerMeleeDmg -= playerWpnStat;
                        playerWpnStat = rustySwordDMG;
                        playerMeleeDmg += playerWpnStat;
                        break;

                    case "Steel Sword":
                        equipWpn(ref useChoice, ref slotEmpty);
                        playerMeleeDmg -= playerWpnStat;
                        playerWpnStat = steelSwordDMG;
                        playerMeleeDmg += playerWpnStat;
                        break;

                    // What happens when equipping armours, calculates stat change as well.
                    case "Rusty Armour":
                        equipAmr(ref useChoice, ref slotEmpty);
                        playerPhysicalResist -= playerAmrStat;
                        playerAmrStat = rustyArmourDEF;
                        playerPhysicalResist += playerAmrStat;
                        break;

                    case "Steel Armour":
                        equipAmr(ref useChoice, ref slotEmpty);
                        playerPhysicalResist -= playerAmrStat;
                        playerAmrStat = steelArmourDEF;
                        playerPhysicalResist += playerAmrStat;
                        break;

                    // What happens when using a healing potion
                    case "Healing Potion":
                        playerHealth = playerHealth + 30;
                        slotEmpty = true;
                        Console.WriteLine("The healing potion was used! +30 HP");
                        if (playerHealth > playerMaxHealth)
                        {
                            playerHealth = playerMaxHealth;
                        }
                        Console.WriteLine($"Your HP is now {playerHealth}/{playerMaxHealth}.");
                        PauseStory();
                        break;

                    case "Mana Potion":
                        playerMana = playerMana + 10;
                        slotEmpty = true;
                        Console.WriteLine("The mana potion was used! +10 MP");
                        if (playerMana > playerMaxMana)
                        {
                            playerMana = playerMaxMana;
                        }
                        Console.WriteLine($"Your MP is now {playerMana}/{playerMaxMana}.");
                        PauseStory();
                        break;

                    case "Greater Healing Potion":
                        playerHealth = playerHealth + 100;
                        slotEmpty = true;
                        Console.WriteLine("The healing potion was used! +100 HP");
                        if (playerHealth > playerMaxHealth)
                        {
                            playerHealth = playerMaxHealth;
                        }
                        Console.WriteLine($"Your HP is now {playerHealth}/{playerMaxHealth}.");
                        PauseStory();
                        break;

                    case "Greater Mana Potion":
                        playerMana = playerMana + 25;
                        slotEmpty = true;
                        Console.WriteLine("The mana potion was used! +25 MP");
                        if (playerMana > playerMaxMana)
                        {
                            playerMana = playerMaxMana;
                        }
                        Console.WriteLine($"Your MP is now {playerMana}/{playerMaxMana}.");

                        PauseStory();
                        break;

                    case "Unique Shell":
                        Console.WriteLine("The sounds of the ocean are beautiful... Someone may be willing to purchase this.");
                        PauseStory();
                        break;

                    case "Twin's Journal":
                        JournalHint();
                        break;

                    default:
                        Console.WriteLine("There is no action to do with this item here.");
                        PauseStory();
                        break;
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

            // equip method for all weapons
            static void equipWpn(ref int useChoice, ref bool slotEmpty)
            {
                string temp = inventory[useChoice - 1];
                if (playerWpn != "")
                {
                    inventory[useChoice - 1] = playerWpn;
                }
                else
                {
                    slotEmpty = true;
                }
                playerWpn = temp;
                Console.WriteLine($"You equipped the {temp}.");
                PauseStory();
            }

            // equip method for all armours
            static void equipAmr(ref int useChoice, ref bool slotEmpty)
            {
                string temp = inventory[useChoice - 1];
                if (playerAmr != "")
                {
                    inventory[useChoice - 1] = playerAmr;
                }
                else
                {
                    slotEmpty = true;
                }
                playerAmr = temp;
                Console.WriteLine($"You equipped the {temp}.");
                PauseStory();
            }
            // Throw several items in a row
            static void ThrowItems()
            {
                bool throwAnother = true;
                while (throwAnother && inventoryCount > 0)
                {
                    ThrowOneItem();
                    if (inventoryCount == 0)
                    {
                        break;
                    }
                    Console.Clear();
                    Console.Write("Throw away another item? (y/n): ");
                    string throwAnswer = Console.ReadLine()?.Trim().ToLower() ?? "";
                    throwAnother = throwAnswer.StartsWith("y");
                }
            }
            // Throwing away items
            static void ThrowOneItem()
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

            if (activequest && questDefinitions.TryGetValue(trackedQuest, out QuestDefinition target) && playerX == target.MapX && playerY == target.MapY)
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
            if (markerScreenY < 0 || markerScreenY >= Console.BufferHeight)
            {
                return char.ToLower(Console.ReadKey(true).KeyChar);
            }
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
              false,
              0, 4,
              "side_merchant_01"
            );
            RegisterQuest(
                "main_quest_01",
                "The Dark Twins",
                "Slay a pair of undead twins in the north east",
                true,
                4, 4,
                "side_merchant_02"
            );
            RegisterQuest(
                "main_quest_02",
                "The Broken Paladin",
                "Find the Paladin of Light, in whatever state they're now in",
                true,
                3, 1,
                "main_quest_01"
            );
            RegisterQuest(
                "main_quest_03",
                "The Devil's Dealer",
                "Find the Warlock with a habit of bad bets",
                true,
                2, 0,
                "main_quest_01"
            );
            RegisterQuest(
               "main_quest_04",
               "Unlock the Threshold",
               "Find the keys to the locks",
               true,
               1, 3,
               "main_quest_01"
            );
            RegisterQuest(
                "main_quest_05",
                "Untangle",
                "Unmake that which made you",
                true,
                1, 3,
                "main_quest_04"
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
        public static bool FinalBossReq()
        {
            Console.WriteLine("The temple lies silent, a large black crystaline door calls from the back of the temple...");
            bool key1 = InvCheck("Rusted Key");
            bool key2 = InvCheck("Adorned Key");
            bool key3 = InvCheck("Twisted Key");
            if (key1 == true && key2 == true && key3 == true)
            {
                Console.WriteLine("The door begins to crack open");
                return true;
            }
            else if (completedQuests.Contains("main_quest_01"))
            {
                if (key1 = InvCheck("Rusted Key") == true)
                {
                    Console.WriteLine("A lock on the door thrums in the presence of the Rusted Key");
                    bool given = GiveQuest("main_quest_04");
                    return false;
                }
                else
                {
                    Console.WriteLine("One of the locks looks similar to the Rusted Key you left near the Dark Twins...");
                    bool given = GiveQuest("main_quest_04");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Nothing happens");
                Console.ReadLine();
                return false;
            }
        }
    }
}