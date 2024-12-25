## Currency Mod
#### What is this mod?
 - This mod handles currency in the game.
 - It uses the cash item (ItemID 496) as currency
 - This Mod lets you add, remove, and check the amount of cash you have. Answell for all other players.
 - Handles syncing of all players cash with the host and players.

#### Requirements:
- `SimpleNetworkEvents` needs to be installed for this mod to work. 
- Can be found here - [SimpleNetworkEvents](https://sotf-mods.com/mods/smokyace/simplenetworkevents)


#### Where does Currency Mod Work?
- This mod works **only** in any sort of multiplayer game. It will **not** work in singleplayer.


### For Modders:
#### Commands: (Note: These are most for testing, or adding cash to player)
- `addcurrency <SteamName> <amount>` - Adds the specified amount of cash to the selected player.
   - Note: `<amount>` needs to be a positive value
   - Note: `<SteamName>` needs to be the full steam name of the player. (Case sensitive)
   - Note: This command can only be used by the **host**.
- `removecurrency <SteamName> <amount>` - Removes the specified amount of cash from the selected player. (Amount needs to be a positive value)
   - Note: `<amount>` needs to be a positive value
   - Note: `<SteamName>` needs to be the full steam name of the player. (Case sensitive)
   - Note: This command can only be used by the **host**.
- `getcurrency <SteamName>` or `getcurrency all` - Gets the amount of cash the selected player has.
   - Note: `<SteamName>` needs to be the full steam name of the player. (Case sensitive)
   - Note: `all` will get the amount of cash all players have.
   - Note: This command can be used by **all players**.
- `synccurrency` - Syncs all players cash with the host -> then host sends updated cash value to all clients.
   - Note: This command can be used by **all players**.

#### How to use the Currency Mod in your mod:
1. Add the Currency Mod as a reference in your mod.
    - In Visual Studio, right-click on your project and click on `Add` -> `Reference...`
	- Click on the `Browse` button and navigate to the `Currency.dll` file.
- How To Add Cash To Player:
```csharp
// Two Ways to add cash to a player

// 1. Using PlayerID
Currency.API.AddCash(Currency.API.GetCurrencyType.SteamID, playerId, amount);

// 2. Using PlayerName / SteamName
Currency.API.AddCash(Currency.API.GetCurrencyType.PlayerName, SteamName, amount);

// Note: amount needs to be a positive value, and type int
// Note: SteamName needs to be the full steam name of the player. (Case sensitive)
// Note: SteamName and PlayerName are both strings
```
- How To Remove Cash From Player:
```csharp
// Two Ways to remove cash from a player

// 1. Using PlayerID
Currency.API.RemoveCash(Currency.API.GetCurrencyType.SteamID, playerId, amount);

// 2. Using PlayerName / SteamName
Currency.API.RemoveCash(Currency.API.GetCurrencyType.PlayerName, SteamName, amount);

// Note: amount needs to be a positive value, and type int
// Note: SteamName needs to be the full steam name of the player. (Case sensitive)
// Note: SteamName and PlayerName are both strings
```
- How To Force Sync Cash Between all players: (Note: This is done automatically when a player joins or leaves)
```csharp
Currency.API.ForceSyncCash();
```
- How To Force Sync Cash Of Spesific Player:
```csharp
// Two Ways to Force Sync Cash from a player

// 1. Using PlayerID
Currency.API.ForceSyncCashForSpesificPlayer(Currency.API.GetCurrencyType.SteamID, playerId);

// 2. Using PlayerName / SteamName
Currency.API.ForceSyncCashForSpesificPlayer(Currency.API.GetCurrencyType.PlayerName, SteamName);
```
- How To Get Cash Of Player:
```csharp
// Two Ways to get cash of a specified player

// 1. Using PlayerID
int? cashAmount = Currency.API.GetCash(Currency.API.GetCurrencyType.SteamID, playerId);

// 2. Using PlayerName / SteamName
int? cashAmount = Currency.API.GetCash(Currency.API.GetCurrencyType.PlayerName, SteamName);

// Note: cashAmount is a nullable int. It will return null if the player is not found/or if cashAmount is invalid.
```
- How To Get Cash Of All Players:
```csharp
Dictionary<string, int> allPlayersCash = Currency.API.GetAllCash();
// Dictionary<PlayerID>, <Cash>

// Note: allPlayersCash is a dictionary with the key being the player's id and the value being the amount of cash they have.
```