## Banking Mod
#### What is this mod?
 - This mod handles Banking in the game.
 - Adds a ATM to the game, where you can add, remove, and check the amount of cash you have.
 - This Mod lets you add, remove, and check the amount of cash you have. Aswell for all other players.
 - Handles syncing of all players cash with the host and players.

#### Requirements:
- `SimpleNetworkEvents` needs to be installed for this mod to work. 
- Can be found here - [SimpleNetworkEvents](https://sotf-mods.com/mods/smokyace/simplenetworkevents)


#### Where does Banking Mod Work?
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

#### How to use the Banking Mod in your mod:
1. Add the Banking Mod as a reference in your mod.
    - In Visual Studio, right-click on your project and click on `Add` -> `Reference...`
	- Click on the `Browse` button and navigate to the `Banking.dll` file.
- How To Add Cash To Player:
```csharp
// Two Ways to add cash to a player

// 1. Using PlayerID
Banking.API.AddCash(Banking.API.GetCurrencyType.SteamID, playerId, amount);

// 2. Using PlayerName / SteamName
Banking.API.AddCash(Banking.API.GetCurrencyType.PlayerName, SteamName, amount);

// Note: amount needs to be a positive value, and type int
// Note: SteamName needs to be the full steam name of the player. (Case sensitive)
// Note: SteamName and PlayerName are both strings
```
- How To Remove Cash From Player:
```csharp
// Two Ways to remove cash from a player

// 1. Using PlayerID
Banking.API.RemoveCash(Banking.API.GetCurrencyType.SteamID, playerId, amount);

// 2. Using PlayerName / SteamName
Banking.API.RemoveCash(Banking.API.GetCurrencyType.PlayerName, SteamName, amount);

// Note: amount needs to be a positive value, and type int
// Note: SteamName needs to be the full steam name of the player. (Case sensitive)
// Note: SteamName and PlayerName are both strings
```
- How To Force Sync Cash Between all players: (Note: This is done automatically when a player joins or leaves)
```csharp
Banking.API.ForceSyncCash();
```
- How To Force Sync Cash Of Spesific Player:
```csharp
// Two Ways to Force Sync Cash from a player

// 1. Using PlayerID
Banking.API.ForceSyncCashForSpesificPlayer(Banking.API.GetCurrencyType.SteamID, playerId);

// 2. Using PlayerName / SteamName
Banking.API.ForceSyncCashForSpesificPlayer(Banking.API.GetCurrencyType.PlayerName, SteamName);
```
- How To Get Cash Of Player:
```csharp
// Two Ways to get cash of a specified player

// 1. Using PlayerID
int? cashAmount = Banking.API.GetCash(Banking.API.GetCurrencyType.SteamID, playerId);

// 2. Using PlayerName / SteamName
int? cashAmount = Banking.API.GetCash(Banking.API.GetCurrencyType.PlayerName, SteamName);

// Note: cashAmount is a nullable int. It will return null if the player is not found/or if cashAmount is invalid.
```
- How To Get Cash Of All Players:
```csharp
Dictionary<string, int> allPlayersCash = Banking.API.GetAllCash();
// Dictionary<PlayerID>, <Cash>

// Note: allPlayersCash is a dictionary with the key being the player's id and the value being the amount of cash they have.
```
- How To Get LocalPlayer Name:
```csharp
string localPlayerName = Banking.API.GetLocalPlayerName();

// This will return the name of the local player. (The PlayerName Of The Player Thats Running This Mod)
// Note: localPlayerName is a string
```
- How To Get LocalPlayer ID:
```csharp
string localPlayerID = Banking.API.GetLocalPlayerID();

// This will return the ID of the local player. (The PlayerID Of The Player Thats Running This Mod)
// Note: localPlayerID is a string
```
- Subscriable Events:
```csharp
// Subscribe To Event
Banking.API.SubscribableEvents.OnPlayerJoin += OnPlayerJoin;  // When a player joins the game in MP
Banking.API.SubscribableEvents.OnPlayerLeave += OnPlayerLeave;  // When a player leaves the game in MP
Banking.API.SubscribableEvents.OnCashChange += OnCashChange;  // When a player's cash changes on the network
Banking.API.SubscribableEvents.OnLeaveWorld += OnLeaveWorld;  // When a THE LOCALPLAYER leaves the world
Banking.API.SubscribableEvents.OnJoinWorld += OnJoinWorld;  // When a THE LOCALPLAYER joins the world

// Unsubscribe To Event
Banking.API.SubscribableEvents.OnPlayerJoin -= OnPlayerJoin;
Banking.API.SubscribableEvents.OnPlayerLeave -= OnPlayerLeave;
Banking.API.SubscribableEvents.OnCashChange -= OnCashChange;
Banking.API.SubscribableEvents.OnLeaveWorld -= OnLeaveWorld;
Banking.API.SubscribableEvents.OnJoinWorld -= OnJoinWorld;

// Event Example Functions
public void OnPlayerJoin()
{
    // This is called when a player joins the server or host.
    // Add Your Code Here
}
public void OnPlayerLeave()
{
    // This is called when a player leaves the server or host.
    // Add Your Code Here
}
public void OnCashChange()
{
    // This is called when a player's (on any of the players on multilayer) cash changes.
    // Add Your Code Here
}
public void OnLeaveWorld()
{
    // This is called when the LocalPlayer leaves the world.
    // Use this to clean up any code that needs to be cleaned up.
    // Add Your Code Here
}
public void OnJoinWorld()
{
    // This is called when the LocalPlayer joins the world.
    // Add Your Code Here
}

// IMPORTANT NOTE:
// The OnJoinWorld Event Is More Important Than We Think
// It gets called when the player joins the world, and the player is ready to be interacted with.
// The built in OnGameStart method can cause issues with the mod, as it gets called before the player is ready.
// So, if you have any code that needs to be run when the player joins the world, use the OnJoinWorld event.
```