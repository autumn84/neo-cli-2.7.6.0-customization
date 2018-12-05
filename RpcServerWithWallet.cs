using Neo.Core;
using Neo.Implementations.Wallets.NEP6;
using Neo.IO;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.Wallets;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Network.RPC
{
    internal class RpcServerWithWallet : RpcServer
    {
        public RpcServerWithWallet(LocalNode localNode)
            : base(localNode)
        {
        }

        protected override JObject Process(string method, JArray _params)
        {
            switch (method)
            {
                case "getapplicationlog":
                case "getbalance":
                case "listaddress":
                case "sendfrom":
                case "sendtoaddress":
                case "sendmany":
                case "getnewaddress":
                case "dumpprivkey":
                    throw new RpcException(-400, "Access denied.");
                case "invoke":
                case "invokefunction":
                case "invokescript":
                    JObject result = base.Process(method, _params);
                    return result;
                case "sendrawtransaction":
                {
                        Transaction tx = Transaction.DeserializeFrom(_params[0].AsString().HexToBytes());
                        if (LocalNode.Relay(tx))
                        {
                            JObject json = new JObject();
                            json["txid"] = tx.Hash.ToString();
                            return json;
                        }
                        else
                        {
                            throw new RpcException(-400, "Invalid Transaction");
                        }
                    }
                default:
                    throw new RpcException(-400, "Access denied.");
            }
        }
    }
}
