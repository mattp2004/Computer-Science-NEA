using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.game
{
    public interface IGame
    {
        #region Properties
        string GameName { get; }

        int RequiredPlayers { get; }

        int MaxPlayers { get; }
        #endregion // Properties

        #region Functions
        bool AddPlayer(Client player);

        void DisconnectClient(Client client);
        void Start();
        #endregion // Functions
    }
}
