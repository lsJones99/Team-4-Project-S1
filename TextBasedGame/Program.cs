namespace TextBasedGame
{
    internal class Program
    {
        const int MAX_INVENTORY_SIZE = 5;

        static int playerHealth = 100;
        static string[] inventory = new string[MAX_INVENTORY_SIZE];
        static int inventoryCount = 0;

        static Random rand = new Random();

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

        static void Intro()
        {
            Console.WriteLine("==============================");
            Console.WriteLine("       MEDIEVAL FANTASY");
            Console.WriteLine("==============================");

            Console.WriteLine("\nYou enter a dark forest.");
            Console.WriteLine("Something is moving between the trees...");
        }

        static bool Menu()
        {
            Console.WriteLine("\n---------- ACTIONS ----------");
            Console.WriteLine("[F] Fight");
            Console.WriteLine("[R] Run");
            Console.WriteLine("[I] Investigate");
            Console.WriteLine("[U] Use item");
            Console.WriteLine("[V] View inventory");
            Console.WriteLine("[Q] Quit");
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
                    return Fight();

                case 'r':
                    Run();
                    break;

                case 'i':
                    Investigate();
                    break;

                case 'u':
                    UseItem();
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

        static bool Fight()
        {
            Console.WriteLine("\nA forest ghoul attacks!");

            int damage = rand.Next(5, 16);
            playerHealth -= damage;

            Console.WriteLine($"You defeat the ghoul but take {damage} damage.");

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                Console.WriteLine("You have been defeated.");
                return false;
            }

            return true;
        }

        static void Run()
        {
            Console.WriteLine("\nYou escape deeper into the forest.");
        }

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

            Console.WriteLine(
                $"Inventory slots: {inventoryCount}/{MAX_INVENTORY_SIZE}");
            
        }
        static void UseItem()
        {
            for (int i = 0; i < inventoryCount; i++)
            {
                if (inventory[i] == "Healing Potion")
                    Console.WriteLine("The potion has been used");
                {

                    playerHealth += 30;
                    if (playerHealth > 100)
                    {
                        playerHealth = 100;
                        
                    }
                }
            }
        }
    }
}