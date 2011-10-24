using System;

namespace Ants
{
    public abstract class Bot
    {
        public abstract void DoTurn(GameState state);

        public abstract void Init();
    }
}