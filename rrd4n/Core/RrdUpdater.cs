using System;
using System.Collections.Generic;
using System.Text;

namespace rrd4n.Core
{
   public interface RrdUpdater
   {
      RrdBackend getRrdBackend();
      void copyStateTo(RrdUpdater updater);
      RrdAllocator getRrdAllocator();
   }
}
