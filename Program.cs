using System;
using System.IO;
using System.Collections.Generic;

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
        Random random = new Random();

        // Randomize player's starting stats within reasonable ranges
        int startingGold = random.Next(80, 121);      // 80-120 gold
        int startingFood = random.Next(40, 61);       // 40-60 food
        int startingTroops = random.Next(8, 13);      // 8-12 troops
        int startingStrength = random.Next(4, 7);     // 4-6 strength

        Player player = new Player
        {
            Gold = startingGold,
            Food = startingFood,
            Army = new Army
            {
                Name = "Player Army",
                Troops = startingTroops,
                Strength = startingStrength
            }
        };

        // Add a general
        player.General = new General { 
            Name = "Commander Mars", 
            Leadership = random.Next(1, 4),  // 1-3 leadership
            Tactics = random.Next(1, 4)      // 1-3 tactics
        };
        
        Console.WriteLine($"Your general is {player.General.Name} with {player.General.Leadership} leadership and {player.General.Tactics} tactics.");

        Console.WriteLine("Your starting army:");
        ShowStatus(player);

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
            // Add to GameLoop method before the menu
            player.Food -= player.Army.Troops / 2; // Troops consume food
            if (player.Food <= 0)
            {
                Console.WriteLine("Your army is starving! Troops are deserting...");
                player.Army.Troops = Math.Max(player.Army.Troops - 2, 0);
                player.Food = 0;
                
                if (player.Army.Troops <= 0)
                {
                    Console.WriteLine("Game Over - All your troops have deserted!");
                    AskToPlayAgain();
                    gameRunning = false;
                    continue;
                }
            }

            Console.WriteLine("\nGame Menu:");
            Console.WriteLine("1. Save Game");
            // Console.WriteLine("2. Recruit Troops");
            Console.WriteLine("2. Show Army Status");
            Console.WriteLine("3. Use General's Special Ability");
            // Removed general special ability option.
            Console.WriteLine("4. Prepare and Simulate Battle");
            Console.WriteLine("5. Exit Game");
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
                    // Implement general's special ability usage
                    Console.WriteLine("Choose your general's special ability: (1-Rally, 2-Flank, 3-Retreat, 0-None)");
                    string abilityChoice = Console.ReadLine() ?? "0";
                    if (int.TryParse(abilityChoice, out int choice) && choice > 0 && choice <= 3)
                    {
                        player.General.UseAbility(player.Army, null, (General.SpecialAbility)(choice - 1));
                    }
                    break;
                case "4":
                    PrepareAndSimulateBattle(player);
                    if (player.Army.Troops <= 0)
                    {
                        Console.WriteLine("Game Over - Your army was defeated!");
                        AskToPlayAgain();
                        gameRunning = false;
                    }
                    break;
                case "5":
                    AskToPlayAgain();
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
        try
        {
            if (File.Exists("save.txt"))
            {
                using (StreamReader reader = new StreamReader("save.txt"))
                {
                    player.Gold = int.Parse(reader.ReadLine() ?? "0");
                    player.Food = int.Parse(reader.ReadLine() ?? "0");
                    player.Army = new Army
                    {
                        Name = "Player Army",
                        Troops = int.Parse(reader.ReadLine() ?? "0"),
                        Strength = int.Parse(reader.ReadLine() ?? "0")
                    };
                }
                Console.WriteLine("Game loaded!");
            }
            else
            {
                Console.WriteLine("No save file found. Starting new game...");
                return StartNewPlayer();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading game: {ex.Message}");
            return StartNewPlayer();
        }
        return player;
    }

    static Player StartNewPlayer()
    {
        Random random = new Random();
        return new Player
        {
            Gold = random.Next(80, 121),
            Food = random.Next(40, 61),
            Army = new Army
            {
                Name = "Player Army",
                Troops = random.Next(8, 13),
                Strength = random.Next(4, 7)
            }
        };
    }

    static void SaveGame(Player player)
    {
        using (StreamWriter writer = new StreamWriter("save.txt"))
        {
            writer.WriteLine(player.Gold);
            writer.WriteLine(player.Food);
            writer.WriteLine(player.Army.Troops);
            writer.WriteLine(player.Army.Strength); // Ensure strength is also saved
            writer.WriteLine(player.General.Name);
            writer.WriteLine(player.General.Leadership);
            writer.WriteLine(player.General.Tactics);
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
        SimulateBattle(player.Army, enemy, battleTerrain, battleWeather, player);
    }

    // In SimulateBattle, additional enemy tactical bonuses are applied.
    static void SimulateBattle(Army playerArmy, Army enemyArmy, Terrain terrain, Weather weather, Player player)
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
            playerArmy.Attack(enemyArmy, terrain, weather);
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
                    enemyArmy.Attack(playerArmy, terrain, weather);
                    enemyArmy.Attack(playerArmy, terrain, weather);
                }
                else
                {
                    enemyArmy.Attack(playerArmy, terrain, weather);
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
        {
            Console.WriteLine($"{playerArmy.Name} wins the battle!");
            
            // Award gold, food and experience based on battle difficulty
            Random rnd = new Random();
            int goldReward = rnd.Next(20, 51) * (int)terrain + 10 * enemyArmy.Strength;
            int foodReward = rnd.Next(10, 31);
            
            player.Gold += goldReward;
            player.Food += foodReward;
            player.Army.Strength += 1; // Experience gain
            
            Console.WriteLine($"Rewards: {goldReward} gold, {foodReward} food");
            Console.WriteLine($"Your army's strength increased to {player.Army.Strength}!");
        }
        else
        {
            Console.WriteLine($"{enemyArmy.Name} wins the battle!");
        }
    }

    static void AskToPlayAgain()
    {
        Console.WriteLine("\nWould you like to play again? (Y/N)");
        string input = Console.ReadLine()?.ToUpper() ?? "N";

        if (input.StartsWith("Y"))
        {
            Console.Clear(); // Clear the console for a fresh start
            Main(new string[] { }); // Restart the game
        }
        else
        {
            Console.WriteLine("Thanks for playing!");
            Environment.Exit(0);
        }
    }
}

class Army
{
    public string Name { get; set; } = string.Empty; // Initialize with a default value
    public int Troops { get; set; }
    public int Strength { get; set; }
    public Dictionary<UnitType, int> Units { get; set; } = new Dictionary<UnitType, int>();

    public void Attack(Army enemy, Terrain terrain, Weather weather)
    {
        Random random = new Random();
        
        // Base damage from strength
        int baseDamage = Strength;
        
        // Apply terrain modifiers
        double terrainModifier = 1.0;
        switch (terrain)
        {
            case Terrain.Hills: terrainModifier = Name.Contains("Player") ? 0.8 : 1.2; break;
            case Terrain.Forest: terrainModifier = 0.9; break;
        }
        
        // Apply weather modifiers
        double weatherModifier = 1.0;
        switch (weather)
        {
            case Weather.Rain: weatherModifier = 0.8; break;
            case Weather.Snow: weatherModifier = 0.7; break;
        }
        
        // Calculate final damage with some randomness
        int finalDamage = (int)(baseDamage * terrainModifier * weatherModifier);
        finalDamage = Math.Max(1, finalDamage + random.Next(-1, 2)); // Add -1, 0, or +1 variance
        
        enemy.Troops -= finalDamage;
        if (enemy.Troops < 0)
            enemy.Troops = 0;
            
        Console.WriteLine($"{Name} deals {finalDamage} damage to {enemy.Name}!");
    }
}

class Player
{
    public int Gold { get; set; }
    public int Food { get; set; }
    public Army Army { get; set; } = new Army(); // Initialize with a default value
    public General General { get; set; } = new General();

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

    public enum SpecialAbility { Rally, Flank, Retreat }

    public void UseAbility(Army army, Army? enemy, SpecialAbility ability)
    {
        switch (ability)
        {
            case SpecialAbility.Rally:
                int boost = Leadership;
                army.Strength += boost;
                Console.WriteLine($"{Name} rallies the troops, boosting strength by {boost}!");
                break;
                
            case SpecialAbility.Flank:
                if (enemy != null)
                {
                    int damage = Tactics * 2;
                    enemy.Troops -= damage;
                    if (enemy.Troops < 0) enemy.Troops = 0;
                    Console.WriteLine($"{Name} orders a flanking maneuver, dealing {damage} damage!");
                }
                else
                {
                    Console.WriteLine($"{Name} cannot flank with no enemy present. Defaulting to rally.");
                    army.Strength += Leadership;
                }
                break;
                
            case SpecialAbility.Retreat:
                army.Strength += 1;
                Console.WriteLine($"{Name} orders a tactical retreat, regrouping and increasing strength by 1!");
                break;
        }
    }
}

enum Terrain { Plains, Hills, Forest }
enum Weather { Clear, Rain, Snow }
enum UnitType { Infantry, Cavalry, Archers }
