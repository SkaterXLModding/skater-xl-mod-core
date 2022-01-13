using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SXLMod.Customization;

namespace SXLMod.Console
{
    class SXLMultiplayerCommands
    {
        [RegisterCommand(Name = "mp_numbergen", Help = "Generate a random integer from input range", Hint = "mp_numbergen <int min|int max>", ArgMin = 0, ArgMax = 2)]
        static void CommandRandomNumber(CommandArg[] args)
        {
            int min = 0;
            int max = 100;

            System.Random rnd = new System.Random();

            switch(args.Length)
            {
                case 2:
                    min = args[0].Int;
                    max = args[1].Int;
                    break;
                case 1:
                    max = args[0].Int;
                    break;
            }

            SXLConsole.Log($"Random number from {min} to {max}: {rnd.Next(min, max)}.");
        }

        [RegisterCommand(Name = "mp_listall", Help = "List all connected players and their IDs", Hint = "mp_listall", ArgMin = 0, ArgMax = 0)]
        static void CommandListAllMultiplayer(CommandArg[] args)
        {
            foreach(NetworkPlayerController netPlayer in SXLMultiplayer.GetAllPlayers())
            {
                SXLConsole.Log($">>> {netPlayer.NickName}: {netPlayer.UserId}");
            }
        }

        [RegisterCommand(Name = "mp_spectate", Help = "Spectate player on server", Hint = "mp_spectate <string>", ArgMin = 0, ArgMax = 1)]
        static void CommandMultiplayerSpectatePlayer(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                SXLMultiplayer.SpectateCamera.enabled = false;
                SXLMultiplayer.SpectatorUI.enabled = false;
                return;
            }

            string playerName = args[0].String.Replace('_', ' ');
            SXLMultiplayer.AttachSpectateCameraToPlayer(playerName);
        }

    }
}
