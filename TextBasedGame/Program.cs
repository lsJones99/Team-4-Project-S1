namespace TextBasedGame
{
    internal class Program
    {
        static void Main()
        {
            FunctionExample();
            Menu();
            Console.ReadLine();
        }

        static void FunctionExample()
        {
            Console.WriteLine("THIS IS A FUNCTION EXAMPLE");
        }

        static void Menu()
        {
            char user;
            Console.WriteLine("User input prompt");
            user = Convert.ToChar(Console.ReadLine());
            switch (user)
            {
                case 'f':
                    Console.WriteLine("Fight");
                    break;
                case 'r':
                    Console.WriteLine("Run");
                    break;
                default:
                    Console.WriteLine("Unrecognised command");
                    break;
            }
        }
    }
}
