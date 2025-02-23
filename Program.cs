class Program
{
    static void Main(string[] args)
    {
        bool isRunning = true;
        while (isRunning)
        {
            Console.WriteLine("1. Start Game");
            Console.WriteLine("2. Load Game");
            Console.WriteLine("3. Exit");
            string input = Console.ReadLine() ?? string.Empty;

            switch (input)
            {
                case "1":
                    StartGame();
                    isRunning = false; // Exit menu to enter game
                    break;
                case "2":
                    LoadGame();
                    isRunning = false; // Exit menu to enter game
                    break;
                case "3":
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    static void StartGame()
    {
        Console.WriteLine("Game started!");
        // Initialize player (General is no longer used for a special ability)
        Player player = new Player { Gold = 100, Food = 50, Army = new Army { Name = "Player Army", Troops = 10, Strength = 5 } };
        GameLoop(player);
    }

    static void LoadGame()
    {
        Player player = LoadPlayerData();
        GameLoop(player);
    }

    static void GameLoop(Player player)
    {
        bool gameRunning = true;
        while (gameRunning)
        {
            Console.WriteLine("\nGame Menu:");
            Console.WriteLine("1. Save Game");
            // Console.WriteLine("2. Recruit Troops");
            Console.WriteLine("2. Show Army Status");
            // Removed general special ability option.
            Console.WriteLine("3. Prepare and Simulate Battle");
            Console.WriteLine("4. Exit Game");
            string input = Console.ReadLine() ?? string.Empty;

            switch (input)
            {
                case "1":
                    SaveGame(player);
                    break;
                case "2":
                    ShowStatus(player);
                    break;
                case "3":
                    PrepareAndSimulateBattle(player);
                    break;
                case "4":
                    gameRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    static void ShowStatus(Player player)
    {
        Console.WriteLine($"Gold: {player.Gold}, Food: {player.Food}, Troops: {player.Army.Troops}, Army Strength: {player.Army.Strength}");
    }

    static Player LoadPlayerData()
    {
        Player player = new Player();
        using (StreamReader reader = new StreamReader("save.txt"))
        {
            player.Gold = int.Parse(reader.ReadLine() ?? "0");
            player.Food = int.Parse(reader.ReadLine() ?? "0");
            player.Army = new Army { Name = "Player Army", Troops = int.Parse(reader.ReadLine() ?? "0"), Strength = 5 };
        }
        Console.WriteLine("Game loaded!");
        return player;
    }

    static void SaveGame(Player player)
    {
        using (StreamWriter writer = new StreamWriter("save.txt"))
        {
            writer.WriteLine(player.Gold);
            writer.WriteLine(player.Food);
            writer.WriteLine(player.Army.Troops);
        }
        Console.WriteLine("Game saved!");
    }

    // Modified PrepareAndSimulateBattle without general's special ability.
    static void PrepareAndSimulateBattle(Player player)
    {
        Console.WriteLine("Preparing for battle...");

        Random random = new Random();

        // Increased enemy base strength between 8 and 10.
        int enemyTroops = random.Next(8, 16);     // 8 to 15 troops.
        int enemyStrength = random.Next(8, 11);     // Increased enemy strength between 8 and 10.
        Army enemy = new Army { Name = "Enemy Army", Troops = enemyTroops, Strength = enemyStrength };
        Console.WriteLine($"Enemy generated with {enemy.Troops} troops and strength {enemy.Strength}.");

        // Generate random terrain and weather.
        Array terrains = Enum.GetValues(typeof(Terrain));
        Array weathers = Enum.GetValues(typeof(Weather));
        Terrain battleTerrain = (Terrain)(terrains.GetValue(random.Next(terrains.Length)) ?? Terrain.Plains);
        Weather battleWeather = (Weather)(weathers.GetValue(random.Next(weathers.Length)) ?? Weather.Clear);
        Console.WriteLine($"Battle Terrain: {battleTerrain}, Weather: {battleWeather}");

        // Automatically invest available gold to recruit extra troops.
        if (player.Gold >= 50)
        {
            int extraTroops = player.Gold / 50; // For every 50 gold, gain 1 troop.
            player.RecruitTroops(extraTroops * 50, extraTroops);
            Console.WriteLine("Extra troops recruited using available gold for battle preparation.");
        }
        else
        {
            Console.WriteLine("Not enough gold for additional troop recruitment.");
        }

        // Removed general's ability call.

        // Display updated player status before battle.
        ShowStatus(new Player { Gold = player.Gold, Food = player.Food, Army = player.Army });

        // Start the battle simulation, now with terrain and weather.
        SimulateBattle(player.Army, enemy, battleTerrain, battleWeather);
    }

    // In SimulateBattle, additional enemy tactical bonuses are applied.
    static void SimulateBattle(Army playerArmy, Army enemyArmy, Terrain terrain, Weather weather)
    {
        Console.WriteLine("Battle begins!");

        Random random = new Random();

        // Determine decision bonus based on terrain and weather.
        int decisionBonus = 0;
        if (terrain == Terrain.Hills)
            decisionBonus += 10; // Hills give enemy an advantage.
        if (terrain == Terrain.Forest)
            decisionBonus += 5;  // Forest offers moderate cover.
        if (weather == Weather.Rain)
            decisionBonus += 5;  // Rain makes it harder for the player.
        if (weather == Weather.Snow)
            decisionBonus += 3;  // Snow slightly benefits enemy tactics.

        // Extra bonus from enemy tactical improvements.
        decisionBonus += 10; // Boost enemy decision-making power.

        // Optionally boost enemy strength further under adverse weather.
        if (weather == Weather.Rain)
        {
            enemyArmy.Strength += 1;
            Console.WriteLine($"{enemyArmy.Name} benefits from the rain, boosting its strength to {enemyArmy.Strength}.");
        }

        while (playerArmy.Troops > 0 && enemyArmy.Troops > 0)
        {
            // Player always attacks.
            playerArmy.Attack(enemyArmy);
            Console.WriteLine($"{playerArmy.Name} attacks! {enemyArmy.Name} now has {enemyArmy.Troops} troops.");

            if (enemyArmy.Troops <= 0)
                break;

            int decisionScore = random.Next(0, 100);
            int aggressiveThreshold = enemyArmy.Troops < playerArmy.Troops ? (70 + decisionBonus) : (40 + decisionBonus);

            if (decisionScore < aggressiveThreshold)
            {
                if (random.Next(0, 100) < 30)
                {
                    Console.WriteLine($"{enemyArmy.Name} launches a double attack!");
                    enemyArmy.Attack(playerArmy);
                    enemyArmy.Attack(playerArmy);
                }
                else
                {
                    enemyArmy.Attack(playerArmy);
                    Console.WriteLine($"{enemyArmy.Name} counterattacks!");
                }
            }
            else if (decisionScore < 90)
            {
                Console.WriteLine($"{enemyArmy.Name} holds position and regroups.");
                enemyArmy.Strength += 1;
            }
            else
            {
                Console.WriteLine($"{enemyArmy.Name} maneuvers strategically, reducing its exposure.");
                enemyArmy.Strength = Math.Max(enemyArmy.Strength - 1, 1);
            }

            Console.WriteLine($"{playerArmy.Name} troops: {playerArmy.Troops}, {enemyArmy.Name} troops: {enemyArmy.Troops}");
        }

        if (playerArmy.Troops > 0)
            Console.WriteLine($"{playerArmy.Name} wins the battle!");
        else
            Console.WriteLine($"{enemyArmy.Name} wins the battle!");
    }
}

class Army
{
    public string Name { get; set; } = string.Empty; // Initialize with a default value
    public int Troops { get; set; }
    public int Strength { get; set; }

    public void Attack(Army enemy)
    {
        // Adjusted damage calculation to avoid overkilling the enemy.
        // For example, use only the army’s Strength instead of multiplying by Troops.
        int damage = Strength;
        enemy.Troops -= damage;
        if (enemy.Troops < 0)
            enemy.Troops = 0;
    }
}

class Player
{
    public int Gold { get; set; }
    public int Food { get; set; }
    public Army Army { get; set; } = new Army(); // Initialize with a default value

    public void RecruitTroops(int cost, int troops)
    {
        if (Gold >= cost)
        {
            Gold -= cost;
            Army.Troops += troops;
            Console.WriteLine($"Recruited {troops} troops!");
        }
        else
        {
            Console.WriteLine("Not enough gold.");
        }
    }
}

class General
{
    public string Name { get; set; } = string.Empty; // Initialize with a default value
    public int Leadership { get; set; }
    public int Tactics { get; set; }

    // Removed special ability implementation.
    // public void UseAbility(Army army)
    // {
    //     army.Strength += Leadership;
    //     Console.WriteLine($"{Name} inspires the troops and increases their strength to {army.Strength}!");
    // }
}

enum Terrain { Plains, Hills, Forest }
enum Weather { Clear, Rain, Snow }
