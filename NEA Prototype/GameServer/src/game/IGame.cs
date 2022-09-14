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
        string Name { get; }

        int RequiredPlayers { get; }

        int MaxPlayers { get; }
        #endregion // Properties

        #region Functions
        bool AddPlayer(TcpClient player);

        void DisconnectClient(TcpClient client);
        void Start();
        #endregion // Functions
    }
}
