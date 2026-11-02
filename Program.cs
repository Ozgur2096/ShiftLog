Console.WriteLine("Type 'quit' to exit the program.");



while (true)
{
    Console.WriteLine("Enter a command:");
    string? input = Console.ReadLine();

    if (input?.ToLower() == "quit")
    {
        break;
    }
    Console.WriteLine($"You entered: {input}");
}

