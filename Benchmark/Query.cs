﻿using TDengineDriver;
using TDengineDriver.Impl;
namespace Benchmark
{
    internal class Query
    {
        string Host { get; set; }
        short Port { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        readonly string db = "benchmark";
        readonly string queryStb = "select * from stb;";
        readonly string queryJtb = "select" +
                                   "ts " +
                                   ",bl " +
                                   ",i8 " +
                                   ",i16" +
                                   ",i32" +
                                   ",i64" +
                                   ",u8 " +
                                   ",u16" +
                                   ",u32" +
                                   ",u64" +
                                   ",f32" +
                                   ",d64" +
                                   ",bnr" +
                                   ",nchr" +
                                   ",json_tag->\"jtag_bool\"" +
                                   ",json_tag->\"jtag_num\"" +
                                   ",json_tag->\"jtag_str\"" +
                                   ",json_tag->\"jtag_null\"" +
                                   " from bkb.jtb;";


        public Query(string host, string userName, string passwd, short port)
        {
            Host = host;
            Username = userName;
            Password = passwd;
            Port = port;
        }
        public void Run(string types, int times)
        {
            Console.WriteLine("Query ... ", types);

            IntPtr conn = TDengine.Connect(Host, Username, Password, db, Port);
            IntPtr res;
            if (conn != IntPtr.Zero)
            {
                res = TDengine.Query(conn, $"use {db}");
                IfTaosQuerySucc(res, $"use {db}");
                TDengine.FreeResult(res);

                if (types == "normal")
                {
                    QueryLoop(conn, times, queryStb);
                }
                if (types == "json")
                {
                    QueryLoop(conn, times, queryJtb);
                }
            }
            else
            {
                throw new Exception("create TD connection failed");
            }
            TDengine.Close(conn);

        }
        public void QueryLoop(IntPtr conn, int times, string sql)
        {
            IntPtr res;
            int i = 0;
            while (i < times)
            {
                res = TDengine.Query(conn, sql);
                IfTaosQuerySucc(res, sql);
                LibTaos.GetMeta(res);
                LibTaos.GetData(res);
                TDengine.FreeResult(res);
                i++;
            }
        }

        public bool IfTaosQuerySucc(IntPtr res, string sql)
        {
            if (TDengine.ErrorNo(res) == 0)
            {
                return true;
            }
            else
            {
                throw new Exception($"execute {sql} failed,reason {TDengine.Error(res)}, code{TDengine.ErrorNo(res)}");
            }
        }
    }
}
