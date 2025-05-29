using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using LibBO.BusinessObjects;
using System.Diagnostics;


var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<igniteConfiguration clientMode=""true"" igniteInstanceName=""Cluster"" xmlns=""http://ignite.apache.org/schema/dotnet/IgniteConfigurationSection"">
<discoverySpi type=""Apache.Ignite.Core.Discovery.Tcp.TcpDiscoverySpi"" localPortRange=""1"">
<ipFinder type=""Apache.Ignite.Core.Discovery.Tcp.Static.TcpDiscoveryStaticIpFinder"">
<endpoints>
<string>10.0.10.113:47500</string>
</endpoints>
</ipFinder>
</discoverySpi>
<communicationSpi type=""Apache.Ignite.Core.Communication.Tcp.TcpCommunicationSpi"">
<usePairedConnections>true</usePairedConnections>
<idleConnectionTimeout>1</idleConnectionTimeout>
</communicationSpi>
</igniteConfiguration>
";

var cfg = IgniteConfiguration.FromXml(xml);

var ignite = Ignition.Start(cfg);

var cacheCfg = new CacheConfiguration(
                "RiskLimits",
                new QueryEntity[] { new QueryEntity(typeof(string), typeof(RiskLimit)) });

var traderCacheConfig = new CacheConfiguration(
                "TraderAccount",
                new QueryEntity[] { new QueryEntity(typeof(string), typeof(TraderRelationWith)) });

var riskCache = ignite.GetOrCreateCache<string, RiskLimit>(cacheCfg);
var traderCache = ignite.GetOrCreateCache<string, TraderRelationWith>(traderCacheConfig);

var _trader = new TraderRelationWith
{
    Username = "QA3",
    BoothID = "ADM",
    RecordID = "Jawad_Account100"
};

riskCache.Put(_trader.RecordID, new RiskLimit
{
    BoothID = _trader.BoothID,
    Account = _trader.RecordID,
    ClientID = _trader.RecordID,
    SodEquity = 9384983,
    CurrentEquity = -234234553,
});

var traders = traderCache.Query(new SqlQuery(typeof(TraderRelationWith),
    "select * FROM \"TraderAccount\".TRADERRELATIONWITH WHERE username = 'QA3'"))
    .Select(i => i.Value).ToArray();


foreach (var trader in traders)
{
    riskCache.Put(trader.RecordID, new RiskLimit
    {
        BoothID = trader.BoothID,
        Account = trader.RecordID,
        ClientID = trader.RecordID,
    });

}

Console.ReadKey();

