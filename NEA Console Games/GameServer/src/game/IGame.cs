using GameServer.src.network;
using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.game
{
    public enum GameStatus
    {
        WAITING,
        STARTING,
        PLAYING,
        ENDING
    }

    interface IGame
    {
        #region Properties
        string GameName { get; }
        int RequiredPlayers { get; }
        int MaxPlayers { get; }
        int PlayerCount { get; }
        GameStatus GetStatus { get; }
        #endregion

        #region Functions
        bool OnPlayerJoin(Client player);
        void OnPlayerQuit(Client player);
        void Boot();
        void Start();
        void End();
        #endregion // Functions
    }
}
